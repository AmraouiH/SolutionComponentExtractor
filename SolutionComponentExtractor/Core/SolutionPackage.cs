using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace SolutionComponentExtractor.Core
{
    internal sealed class ZipEntryData
    {
        public string FullName;
        public DateTimeOffset LastWriteTime;
        public byte[] Content;

        public string Normalized => SolutionPackage.NormalizePath(FullName);
    }

    /// <summary>An exported solution zip loaded in memory.</summary>
    public sealed class SolutionPackage
    {
        public const string SolutionXmlName = "solution.xml";
        public const string CustomizationsXmlName = "customizations.xml";

        public string SourcePath { get; private set; }
        public string UniqueName { get; private set; }
        public string DisplayName { get; private set; }
        public string Version { get; private set; }
        public string PublisherName { get; private set; }
        public bool IsManaged { get; private set; }
        public IReadOnlyList<SolutionComponent> Components => components;

        internal List<ZipEntryData> Entries { get; } = new List<ZipEntryData>();
        internal XDocument SolutionXml { get; private set; }
        internal XDocument CustomizationsXml { get; private set; }
        internal bool SolutionXmlHasBom { get; private set; }
        internal bool CustomizationsXmlHasBom { get; private set; }

        private readonly List<SolutionComponent> components = new List<SolutionComponent>();

        public static SolutionPackage Load(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var package = Load(stream);
                package.SourcePath = path;
                return package;
            }
        }

        public static SolutionPackage Load(Stream stream)
        {
            var package = new SolutionPackage();
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Read, true))
            {
                foreach (var entry in zip.Entries)
                {
                    using (var es = entry.Open())
                    using (var ms = new MemoryStream())
                    {
                        es.CopyTo(ms);
                        package.Entries.Add(new ZipEntryData { FullName = entry.FullName, LastWriteTime = entry.LastWriteTime, Content = ms.ToArray() });
                    }
                }
            }

            var solutionEntry = package.FindEntry(SolutionXmlName);
            var customizationsEntry = package.FindEntry(CustomizationsXmlName);
            if (solutionEntry == null || customizationsEntry == null)
            {
                throw new InvalidDataException("This file is not a Dataverse solution: solution.xml and customizations.xml must be at the root of the zip.");
            }

            package.SolutionXml = XmlHelpers.Load(solutionEntry.Content, out var solutionBom);
            package.SolutionXmlHasBom = solutionBom;
            package.CustomizationsXml = XmlHelpers.Load(customizationsEntry.Content, out var customizationsBom);
            package.CustomizationsXmlHasBom = customizationsBom;

            package.ReadManifest();
            package.ReadComponents();
            return package;
        }

        internal static string NormalizePath(string path)
        {
            return path?.Trim().Replace('\\', '/').TrimStart('/').ToLowerInvariant();
        }

        internal ZipEntryData FindEntry(string path)
        {
            var normalized = NormalizePath(path);
            return Entries.FirstOrDefault(e => e.Normalized == normalized);
        }

        internal XElement Manifest => SolutionXml.Root?.Kid("SolutionManifest")
                                      ?? throw new InvalidDataException("solution.xml does not contain a SolutionManifest element.");

        private void ReadManifest()
        {
            var manifest = Manifest;
            UniqueName = manifest.KidValue("UniqueName");
            Version = manifest.KidValue("Version");
            // 0 = unmanaged, 1 = managed, 2 = both (SolutionPackager); only 0 can be filtered by the tool.
            var managed = manifest.KidValue("Managed");
            IsManaged = managed == "1" || managed == "2";
            DisplayName = manifest.Kid("LocalizedNames")?.Elements().Select(e => e.Attr("description")).FirstOrDefault(d => !string.IsNullOrWhiteSpace(d)) ?? UniqueName;

            var publisher = manifest.Kid("Publisher");
            PublisherName = publisher?.Kid("LocalizedNames")?.Elements().Select(e => e.Attr("description")).FirstOrDefault(d => !string.IsNullOrWhiteSpace(d))
                            ?? publisher?.KidValue("UniqueName");
        }

        private void ReadComponents()
        {
            var rootComponents = Manifest.Kid("RootComponents");
            if (rootComponents == null) return;

            var customizationsRoot = CustomizationsXml.Root;
            foreach (var element in rootComponents.Kids("RootComponent"))
            {
                int.TryParse(element.Attr("type"), out var type);
                var component = new SolutionComponent
                {
                    Type = type,
                    SchemaName = string.IsNullOrWhiteSpace(element.Attr("schemaName")) ? null : element.Attr("schemaName").Trim(),
                    Id = XmlHelpers.NormalizeGuid(element.Attr("id")),
                    Behavior = element.Attr("behavior"),
                    RootComponentElement = element,
                };
                if (component.SchemaName == null && component.Id == null) component.SchemaName = element.Attr("id");

                ComponentLocator.Locate(component, customizationsRoot);
                LocateFolders(component);
                component.DisplayName = ComponentLocator.ResolveDisplayName(component) ?? component.Identifier;
                component.TypeName = ResolveTypeName(component);
                components.Add(component);
            }

            ComponentLocator.LinkParents(components);
        }

        /// <summary>Folders such as "environmentvariabledefinitions/{schemaName}/" that hold a component's definition.</summary>
        private void LocateFolders(SolutionComponent component)
        {
            var keys = new[] { component.SchemaName?.ToLowerInvariant(), component.Id }.Where(k => !string.IsNullOrEmpty(k)).ToList();
            if (keys.Count == 0) return;

            foreach (var entry in Entries)
            {
                var segments = entry.Normalized.Split('/');
                if (segments.Length < 3) continue;
                var folderName = segments[1];
                var matches = keys.Any(k => folderName == k || (component.Id != null && folderName.Trim('{', '}') == component.Id));
                if (!matches) continue;

                var folder = segments[0] + "/" + segments[1] + "/";
                if (!component.Folders.Contains(folder)) component.Folders.Add(folder);
            }
        }

        private static string ResolveTypeName(SolutionComponent component)
        {
            var known = ComponentTypes.GetName(component.Type);
            if (known != null) return known;

            // Dynamic types (solution-aware tables such as connection references) have org-specific type codes:
            // label them from the section or folder where their definition was found.
            var container = component.Definitions.Select(d => d.Parent?.Name.LocalName).FirstOrDefault()
                            ?? component.Folders.Select(f => f.Split('/')[0]).FirstOrDefault();
            var label = ComponentTypes.LabelFromContainer(container);
            return label ?? $"Component type {component.Type}";
        }
    }
}
