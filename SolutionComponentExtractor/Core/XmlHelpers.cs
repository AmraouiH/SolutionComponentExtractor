using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SolutionComponentExtractor.Core
{
    internal static class XmlHelpers
    {
        private static readonly StringComparer IgnoreCase = StringComparer.OrdinalIgnoreCase;

        public static bool Is(this XElement e, string localName)
        {
            return IgnoreCase.Equals(e.Name.LocalName, localName);
        }

        public static IEnumerable<XElement> Kids(this XElement e, string localName)
        {
            return e.Elements().Where(x => x.Is(localName));
        }

        public static XElement Kid(this XElement e, string localName)
        {
            return e.Kids(localName).FirstOrDefault();
        }

        public static string KidValue(this XElement e, string localName)
        {
            return e.Kid(localName)?.Value.Trim();
        }

        public static string Attr(this XElement e, string localName)
        {
            return e.Attributes().FirstOrDefault(a => IgnoreCase.Equals(a.Name.LocalName, localName))?.Value;
        }

        /// <summary>Normalizes a GUID string ("{ABC...}" / "abc...") to lowercase without braces; null if not a GUID.</summary>
        public static string NormalizeGuid(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return Guid.TryParse(value.Trim(), out var g) ? g.ToString("D") : null;
        }

        public static bool SameGuid(string value, string normalizedId)
        {
            return normalizedId != null && NormalizeGuid(value) == normalizedId;
        }

        /// <summary>Names of attributes/elements that hold the unique name of a definition.</summary>
        public static bool IsNameKey(string localName)
        {
            var n = localName.ToLowerInvariant();
            return n == "name" || n == "uniquename" || n == "schemaname" || n == "logicalname"
                   || n.EndsWith("uniquename") || n.EndsWith("logicalname") || n.EndsWith("schemaname");
        }

        /// <summary>Values (attributes and text-only elements) found in an element and its descendants.</summary>
        public static IEnumerable<string> AllValues(XElement root)
        {
            foreach (var e in root.DescendantsAndSelf())
            {
                foreach (var a in e.Attributes()) yield return a.Value;
                if (!e.HasElements && !e.IsEmpty) yield return e.Value;
            }
        }

        public static XDocument Load(byte[] content, out bool hasBom)
        {
            hasBom = content.Length >= 3 && content[0] == 0xEF && content[1] == 0xBB && content[2] == 0xBF;
            using (var ms = new MemoryStream(content))
            {
                return XDocument.Load(ms, LoadOptions.PreserveWhitespace);
            }
        }

        public static byte[] Save(XDocument doc, bool withBom)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(withBom),
                Indent = false,
                NewLineHandling = NewLineHandling.None,
                OmitXmlDeclaration = doc.Declaration == null,
            };
            using (var ms = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(ms, settings))
                {
                    doc.Save(writer);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Removes an element together with the indentation in front of it, and collapses its parent
        /// to an empty element (e.g. &lt;WebResources /&gt;, as Dataverse exports it) when nothing but whitespace is left.
        /// </summary>
        public static void RemoveClean(this XElement element)
        {
            var parent = element.Parent;
            if (element.PreviousNode is XText indentation && string.IsNullOrWhiteSpace(indentation.Value))
            {
                indentation.Remove();
            }
            element.Remove();

            if (parent != null && parent.Nodes().All(n => n is XText text && string.IsNullOrWhiteSpace(text.Value)))
            {
                parent.RemoveNodes();
            }
        }

        /// <summary>Maps every element of <paramref name="original"/> to the same element in <paramref name="clone"/>.</summary>
        public static Dictionary<XElement, XElement> MapClone(XDocument original, XDocument clone)
        {
            var source = original.Root.DescendantsAndSelf().ToList();
            var target = clone.Root.DescendantsAndSelf().ToList();
            if (source.Count != target.Count) throw new InvalidOperationException("Cloned document does not match its source.");
            var map = new Dictionary<XElement, XElement>(source.Count);
            for (var i = 0; i < source.Count; i++) map[source[i]] = target[i];
            return map;
        }
    }
}
