using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SolutionComponentExtractor.Core
{
    /// <summary>Finds the customizations.xml elements that define a root component.</summary>
    internal static class ComponentLocator
    {
        public static void Locate(SolutionComponent component, XElement customizationsRoot)
        {
            if (customizationsRoot == null) return;

            var knownSections = ComponentTypes.GetSections(component.Type);
            var sections = customizationsRoot.Elements().Where(s => knownSections != null
                ? knownSections.Any(s.Is)
                : !ComponentTypes.EntitySections.Any(s.Is));

            var found = sections.SelectMany(s => s.Elements()).Where(item => IsDefinitionOf(item, component)).ToList();

            if (found.Count == 0 && knownSections != null && component.Type != ComponentTypes.Entity)
            {
                // Fall back to every non-table section (layout differences between versions).
                found = customizationsRoot.Elements().Where(s => !ComponentTypes.EntitySections.Any(s.Is))
                    .SelectMany(s => s.Elements()).Where(item => IsDefinitionOf(item, component)).ToList();
            }

            var deepId = ComponentTypes.GetDeepIdElement(component.Type);
            if (found.Count == 0 && deepId != null && component.Id != null)
            {
                found = customizationsRoot.Descendants()
                    .Where(e => e.Kids(deepId).Any(k => XmlHelpers.SameGuid(k.Value, component.Id)))
                    .ToList();
            }

            component.Definitions.AddRange(found);
        }

        /// <summary>
        /// An item matches when its own id (attribute/child named "id" or "{ElementName}Id")
        /// or its unique name (Name, UniqueName, *LogicalName...) equals the root component key.
        /// </summary>
        private static bool IsDefinitionOf(XElement item, SolutionComponent component)
        {
            var selfId = item.Name.LocalName + "id";
            bool Matches(string key, string value)
            {
                if (component.Id != null && (key.Equals("id", StringComparison.OrdinalIgnoreCase) || key.Equals(selfId, StringComparison.OrdinalIgnoreCase)))
                {
                    if (XmlHelpers.SameGuid(value, component.Id)) return true;
                }
                return component.SchemaName != null && XmlHelpers.IsNameKey(key)
                       && string.Equals(value?.Trim(), component.SchemaName, StringComparison.OrdinalIgnoreCase);
            }

            if (item.Attributes().Any(a => Matches(a.Name.LocalName, a.Value))) return true;
            return item.Elements().Any(child => !child.HasElements && Matches(child.Name.LocalName, child.Value));
        }

        public static string ResolveDisplayName(SolutionComponent component)
        {
            var definition = component.Definitions.FirstOrDefault();
            if (definition == null) return null;

            var nameElement = definition.Kid("Name");
            var localized = nameElement?.Attr("LocalizedName");
            if (!string.IsNullOrWhiteSpace(localized)) return localized;

            var fromLocalizedNames = definition.Descendants()
                .Where(e => e.Is("LocalizedNames"))
                .Take(1)
                .SelectMany(e => e.Elements())
                .Select(e => e.Attr("description"))
                .FirstOrDefault(d => !string.IsNullOrWhiteSpace(d));
            if (fromLocalizedNames != null) return fromLocalizedNames;

            foreach (var key in new[] { "localizedName", "displayname", "Name" })
            {
                var value = definition.Attr(key);
                if (!string.IsNullOrWhiteSpace(value)) return value;
            }

            // e.g. <connectionreferencedisplayname>, <DisplayName>
            var displayName = definition.Elements()
                .FirstOrDefault(e => !e.HasElements && e.Name.LocalName.EndsWith("displayname", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(e.Value));
            if (displayName != null) return displayName.Value.Trim();

            var name = definition.Kid("Name");
            if (name != null && !name.HasElements && !string.IsNullOrWhiteSpace(name.Value)) return name.Value.Trim();

            // Plug-in assemblies: FullName="Contoso.Plugins, Version=1.0.0.0, Culture=neutral, PublicKeyToken=..."
            var fullName = definition.Attr("FullName");
            if (!string.IsNullOrWhiteSpace(fullName)) return fullName.Split(',')[0].Trim();
            return null;
        }

        /// <summary>Links sub-components (e.g. a form) to the component whose definition contains them (e.g. its table).</summary>
        public static void LinkParents(IReadOnlyList<SolutionComponent> components)
        {
            var owners = new Dictionary<XElement, SolutionComponent>();
            foreach (var c in components)
            {
                foreach (var d in c.Definitions)
                {
                    if (!owners.ContainsKey(d)) owners[d] = c;
                }
            }

            foreach (var c in components)
            {
                foreach (var ancestor in c.Definitions.SelectMany(d => d.Ancestors()))
                {
                    if (owners.TryGetValue(ancestor, out var parent) && parent != c)
                    {
                        c.Parent = parent;
                        parent.Children.Add(c);
                        break;
                    }
                }
            }
        }
    }
}
