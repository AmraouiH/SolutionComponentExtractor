using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SolutionComponentExtractor.Core
{
    public sealed class ExtractOptions
    {
        /// <summary>New solution unique name; null keeps the source one.</summary>
        public string UniqueName { get; set; }

        /// <summary>New solution display name; null keeps the source one.</summary>
        public string DisplayName { get; set; }

        /// <summary>New solution version; null keeps the source one.</summary>
        public string Version { get; set; }
    }

    public sealed class ExtractResult
    {
        public byte[] ZipContent { get; internal set; }
        public string FileName { get; internal set; }
        public List<SolutionComponent> Kept { get; } = new List<SolutionComponent>();
        public List<SolutionComponent> Removed { get; } = new List<SolutionComponent>();
        public List<string> RemovedFiles { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();

        /// <summary>Every kept component is an unmodified managed reference: the import would leave the solution empty.</summary>
        public bool HasNothingToImport { get; internal set; }
    }

    /// <summary>Builds a new solution zip that only contains the selected root components.</summary>
    public static class SolutionExtractor
    {
        private static readonly Regex UniqueNameRegex = new Regex("^[A-Za-z_][A-Za-z0-9_]*$");
        private static readonly Regex VersionRegex = new Regex(@"^\d+(\.\d+){1,3}$");
        private static readonly StringComparer IgnoreCase = StringComparer.OrdinalIgnoreCase;

        public const string InvalidUniqueNameMessage = "The unique name can only contain letters, digits and underscores, and cannot start with a digit.";
        public const string InvalidVersionMessage = "The version must look like 1.0.0.0.";

        /// <summary>Empty values are valid: the source solution value is kept.</summary>
        public static bool IsValidUniqueName(string value)
        {
            return string.IsNullOrWhiteSpace(value) || UniqueNameRegex.IsMatch(value.Trim());
        }

        /// <summary>Empty values are valid: the source solution value is kept.</summary>
        public static bool IsValidVersion(string value)
        {
            return string.IsNullOrWhiteSpace(value) || VersionRegex.IsMatch(value.Trim());
        }

        public const string ManagedNotSupportedMessage =
            "This is a managed solution. Solution Component Extractor only works with unmanaged solutions: "
            + "a managed solution is locked and cannot be split into a new solution."
            + "\r\n\r\nExport the solution as Unmanaged from the source environment (Solutions > Export > Unmanaged) and open that file instead.";

        public static IEnumerable<string> Validate(SolutionPackage package, ExtractOptions options)
        {
            if (package.IsManaged)
            {
                yield return ManagedNotSupportedMessage;
                yield break;
            }
            if (!package.Components.Any(c => c.Selected)) yield return "Select at least one component to keep.";
            if (!IsValidUniqueName(options.UniqueName)) yield return InvalidUniqueNameMessage;
            if (!IsValidVersion(options.Version)) yield return InvalidVersionMessage;
        }

        /// <summary>The XML of the new solution, before it is zipped.</summary>
        private sealed class BuildOutput
        {
            public ExtractResult Result;
            public XDocument Solution;
            public XDocument Customizations;
            public Dictionary<XElement, XElement> SolutionMap;
            public Dictionary<XElement, XElement> CustomizationsMap;
            public HashSet<string> DeletedEntries;
        }

        /// <summary>
        /// The source documents are shared by every build (generation and live preview, which run on background threads):
        /// LINQ to XML does not guarantee thread-safe reads, so builds never overlap.
        /// </summary>
        private static readonly object BuildLock = new object();

        public static ExtractResult Extract(SolutionPackage package, ExtractOptions options)
        {
            var errors = Validate(package, options).ToList();
            if (errors.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, errors));

            var output = Build(package, options, new HashSet<SolutionComponent>(package.Components.Where(c => c.Selected)));
            var result = output.Result;

            var solutionBytes = XmlHelpers.Save(output.Solution, package.SolutionXmlHasBom);
            var customizationsBytes = XmlHelpers.Save(output.Customizations, package.CustomizationsXmlHasBom);

            using (var ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (var entry in package.Entries)
                    {
                        var path = entry.Normalized;
                        if (output.DeletedEntries.Contains(path))
                        {
                            result.RemovedFiles.Add(entry.FullName);
                            continue;
                        }

                        var content = path == SolutionPackage.SolutionXmlName ? solutionBytes
                            : path == SolutionPackage.CustomizationsXmlName ? customizationsBytes
                            : entry.Content;

                        var zipEntry = zip.CreateEntry(entry.FullName, CompressionLevel.Optimal);
                        zipEntry.LastWriteTime = entry.LastWriteTime;
                        if (entry.FullName.EndsWith("/")) continue;
                        using (var stream = zipEntry.Open())
                        {
                            stream.Write(content, 0, content.Length);
                        }
                    }
                }
                result.ZipContent = ms.ToArray();
            }

            var manifest = output.Solution.Root.Kid("SolutionManifest");
            var version = manifest.KidValue("Version") ?? "1.0.0.0";
            result.FileName = $"{manifest.KidValue("UniqueName")}_{version.Replace('.', '_')}.zip";
            return result;
        }

        /// <summary>
        /// Computes solution.xml and customizations.xml of the new solution for a snapshot of the selection,
        /// without zipping anything. Safe to call from a background thread.
        /// </summary>
        public static SolutionPreview Preview(SolutionPackage package, ExtractOptions options, ICollection<SolutionComponent> selected)
        {
            var output = Build(package, options, new HashSet<SolutionComponent>(selected));
            return SolutionPreview.Create(output.Result, output.Solution, output.Customizations, output.SolutionMap, output.CustomizationsMap,
                package.Entries.Count - output.DeletedEntries.Count);
        }

        private static BuildOutput Build(SolutionPackage package, ExtractOptions options, HashSet<SolutionComponent> selected)
        {
            lock (BuildLock)
            {
                var result = new ExtractResult();
                var removed = ResolveRemoved(package, result, selected);
                result.Removed.AddRange(package.Components.Where(removed.Contains));
                result.Kept.AddRange(package.Components.Where(c => !removed.Contains(c)));

                var solution = new XDocument(package.SolutionXml);
                var solutionMap = XmlHelpers.MapClone(package.SolutionXml, solution);
                var customizations = new XDocument(package.CustomizationsXml);
                var customizationsMap = XmlHelpers.MapClone(package.CustomizationsXml, customizations);

                UpdateManifest(solution, solutionMap, package, result, options);

                var entries = package.Entries.ToDictionary(e => e.Normalized, e => e);
                var removedReferences = RemoveDefinitions(customizations, customizationsMap, result, entries);
                RemoveOrphanTableItems(customizations, result);
                CollectWarnings(customizations, result);

                var keptReferences = new HashSet<string>(
                    XmlHelpers.AllValues(customizations.Root).Concat(XmlHelpers.AllValues(solution.Root))
                        .Select(SolutionPackage.NormalizePath)
                        .Where(p => p != null && entries.ContainsKey(p)));

                return new BuildOutput
                {
                    Result = result,
                    Solution = solution,
                    Customizations = customizations,
                    SolutionMap = solutionMap,
                    CustomizationsMap = customizationsMap,
                    DeletedEntries = ResolveDeletedEntries(package, result, removedReferences, keptReferences),
                };
            }
        }

        /// <summary>Unselected components plus sub-components whose definition lives inside a removed component.</summary>
        private static HashSet<SolutionComponent> ResolveRemoved(SolutionPackage package, ExtractResult result, HashSet<SolutionComponent> selected)
        {
            var removed = new HashSet<SolutionComponent>(package.Components.Where(c => !selected.Contains(c)));
            foreach (var component in package.Components.Where(selected.Contains))
            {
                for (var parent = component.Parent; parent != null; parent = parent.Parent)
                {
                    if (!removed.Contains(parent)) continue;
                    removed.Add(component);
                    result.Warnings.Add($"{component.TypeName} '{component}' was removed because it is defined inside {parent.TypeName} '{parent}', which is not kept.");
                    break;
                }
            }
            return removed;
        }

        private static void UpdateManifest(XDocument solution, Dictionary<XElement, XElement> map, SolutionPackage package, ExtractResult result, ExtractOptions options)
        {
            foreach (var component in result.Removed)
            {
                map[component.RootComponentElement].RemoveClean();
            }

            var manifest = solution.Root.Kid("SolutionManifest");
            if (!string.IsNullOrWhiteSpace(options.UniqueName))
            {
                var uniqueName = manifest.Kid("UniqueName");
                if (uniqueName != null) uniqueName.Value = options.UniqueName.Trim();
            }
            if (!string.IsNullOrWhiteSpace(options.DisplayName))
            {
                foreach (var localizedName in manifest.Kid("LocalizedNames")?.Elements() ?? Enumerable.Empty<XElement>())
                {
                    localizedName.SetAttributeValue("description", options.DisplayName.Trim());
                }
            }
            if (!string.IsNullOrWhiteSpace(options.Version))
            {
                var version = manifest.Kid("Version");
                if (version != null) version.Value = options.Version.Trim();
            }

            FilterMissingDependencies(manifest, result);
        }

        /// <summary>Drops missing dependencies declared by components that are no longer in the solution.</summary>
        private static void FilterMissingDependencies(XElement manifest, ExtractResult result)
        {
            var missingDependencies = manifest.Kid("MissingDependencies");
            if (missingDependencies == null) return;

            var removedIds = new HashSet<string>(result.Removed.Where(c => c.Id != null).Select(c => c.Id));
            var removedNames = new HashSet<string>(result.Removed.Where(c => c.SchemaName != null).Select(c => c.Type + "|" + c.SchemaName), IgnoreCase);
            var removedTables = new HashSet<string>(result.Removed.Where(c => c.Type == ComponentTypes.Entity && c.SchemaName != null).Select(c => c.SchemaName), IgnoreCase);
            var keptTables = new HashSet<string>(result.Kept.Where(c => c.Type == ComponentTypes.Entity && c.SchemaName != null).Select(c => c.SchemaName), IgnoreCase);

            foreach (var dependency in missingDependencies.Kids("MissingDependency").ToList())
            {
                var dependent = dependency.Kid("Dependent");
                if (dependent == null) continue;

                var id = XmlHelpers.NormalizeGuid(dependent.Attr("id"));
                var schemaName = dependent.Attr("schemaName");
                var parentSchemaName = dependent.Attr("parentSchemaName");
                var type = dependent.Attr("type");

                var isRemoved = (id != null && removedIds.Contains(id))
                                || (schemaName != null && removedNames.Contains(type + "|" + schemaName))
                                || (parentSchemaName != null && removedTables.Contains(parentSchemaName) && !keptTables.Contains(parentSchemaName));
                if (isRemoved) dependency.RemoveClean();
            }
        }

        /// <summary>Removes the definitions of removed components and returns the zip paths they referenced.</summary>
        private static HashSet<string> RemoveDefinitions(XDocument customizations, Dictionary<XElement, XElement> map, ExtractResult result, Dictionary<string, ZipEntryData> entries)
        {
            var keptDefinitions = new HashSet<XElement>(result.Kept.SelectMany(c => c.Definitions).Select(d => map[d]));
            var references = new HashSet<string>();

            foreach (var component in result.Removed)
            {
                if (!component.IsLocated)
                {
                    result.Warnings.Add($"{component.TypeName} '{component}': no definition was found in customizations.xml, only its RootComponent entry was removed.");
                }

                foreach (var definition in component.Definitions.Select(d => map[d]))
                {
                    if (definition.Parent == null) continue; // already removed with an ancestor
                    if (definition.Descendants().Any(keptDefinitions.Contains))
                    {
                        result.Warnings.Add($"{component.TypeName} '{component}' was kept in customizations.xml because it contains components you selected.");
                        continue;
                    }

                    foreach (var value in XmlHelpers.AllValues(definition))
                    {
                        var path = SolutionPackage.NormalizePath(value);
                        if (path != null && entries.ContainsKey(path)) references.Add(path);
                    }
                    definition.RemoveClean();
                }
            }
            return references;
        }

        /// <summary>Relationships and mappings that belong to removed tables.</summary>
        private static void RemoveOrphanTableItems(XDocument customizations, ExtractResult result)
        {
            var removedTables = new HashSet<string>(result.Removed.Where(c => c.Type == ComponentTypes.Entity && c.SchemaName != null).Select(c => c.SchemaName), IgnoreCase);
            if (removedTables.Count == 0) return;

            var keptTables = new HashSet<string>(result.Kept.Where(c => c.Type == ComponentTypes.Entity && c.SchemaName != null).Select(c => c.SchemaName), IgnoreCase);
            var keptRelationships = new HashSet<string>(result.Kept.Where(c => c.Type == ComponentTypes.EntityRelationship && c.SchemaName != null).Select(c => c.SchemaName), IgnoreCase);
            var root = customizations.Root;

            foreach (var relationship in root.Kids("EntityRelationships").SelectMany(s => s.Kids("EntityRelationship")).ToList())
            {
                if (keptRelationships.Contains(relationship.Attr("Name") ?? string.Empty)) continue;

                bool remove;
                if (string.Equals(relationship.KidValue("EntityRelationshipType"), "ManyToMany", StringComparison.OrdinalIgnoreCase))
                {
                    var sides = new[] { relationship.KidValue("FirstEntityName"), relationship.KidValue("SecondEntityName") }.Where(s => s != null).ToList();
                    remove = sides.Any(removedTables.Contains) && !sides.Any(keptTables.Contains);
                }
                else
                {
                    // The lookup column of a 1:N relationship lives on the referencing table.
                    var referencing = relationship.KidValue("ReferencingEntityName");
                    remove = referencing != null && removedTables.Contains(referencing);
                }
                if (remove) relationship.RemoveClean();
            }

            foreach (var entityMap in root.Kids("EntityMaps").SelectMany(s => s.Kids("EntityMap")).ToList())
            {
                if (removedTables.Contains(entityMap.KidValue("EntitySource") ?? string.Empty)
                    || removedTables.Contains(entityMap.KidValue("EntityTarget") ?? string.Empty))
                {
                    entityMap.RemoveClean();
                }
            }
        }

        private static void CollectWarnings(XDocument customizations, ExtractResult result)
        {
            foreach (var component in result.Kept.Where(c => c.IsUnmodified))
            {
                result.Warnings.Add($"{component.TypeName} '{component}' is a managed component exported unmodified (reference only, no content): "
                                    + "importing it changes nothing and it will not appear in the imported solution.");
            }
            if (result.Kept.Count > 0 && result.Kept.All(c => c.IsUnmodified))
            {
                result.HasNothingToImport = true;
            }

            var removedTables = new HashSet<string>(result.Removed.Where(c => c.Type == ComponentTypes.Entity && c.SchemaName != null).Select(c => c.SchemaName), IgnoreCase);
            if (removedTables.Count == 0) return;

            foreach (var component in result.Kept.Where(c => c.Type == ComponentTypes.Workflow))
            {
                var primaryEntity = component.Definitions.Select(d => d.KidValue("PrimaryEntity")).FirstOrDefault(v => v != null);
                if (primaryEntity != null && removedTables.Contains(primaryEntity))
                {
                    result.Warnings.Add($"Process '{component}' runs on table '{primaryEntity}', which is not in the new solution: it must already exist in the target environment.");
                }
            }
        }

        /// <summary>Files of removed components that no kept component references.</summary>
        private static HashSet<string> ResolveDeletedEntries(SolutionPackage package, ExtractResult result, HashSet<string> removedReferences, HashSet<string> keptReferences)
        {
            var deleted = new HashSet<string>(removedReferences.Where(r => !keptReferences.Contains(r)));
            var keptFolders = new HashSet<string>(result.Kept.SelectMany(c => c.Folders));

            bool FolderInUse(string folder) => keptFolders.Contains(folder) || keptReferences.Any(r => r.StartsWith(folder, StringComparison.Ordinal));

            // A component stored in its own folder (e.g. PluginAssemblies/MyAssembly-{id}/, Controls/ns.Control/)
            // owns every file of that folder, including the ones customizations.xml does not reference.
            var folders = removedReferences
                .Select(r => r.Split('/'))
                .Where(s => s.Length >= 3)
                .Select(s => s[0] + "/" + s[1] + "/")
                .Concat(result.Removed.SelectMany(c => c.Folders))
                .Distinct()
                .Where(f => !FolderInUse(f))
                .ToList();

            foreach (var entry in package.Entries)
            {
                var path = entry.Normalized;
                if (folders.Any(f => path.StartsWith(f, StringComparison.Ordinal))) deleted.Add(path);
            }

            deleted.Remove(SolutionPackage.SolutionXmlName);
            deleted.Remove(SolutionPackage.CustomizationsXmlName);
            deleted.Remove("[content_types].xml");
            return deleted;
        }
    }
}
