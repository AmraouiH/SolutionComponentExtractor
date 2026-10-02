using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SolutionComponentExtractor.Core
{
    /// <summary>Lines (1-based, inclusive) of an element in a previewed XML file.</summary>
    public struct LineRange
    {
        public LineRange(int start, int end)
        {
            Start = start;
            End = end < start ? start : end;
        }

        public int Start { get; }
        public int End { get; }
    }

    /// <summary>solution.xml and customizations.xml as they will be written in the new solution.</summary>
    public sealed class SolutionPreview
    {
        public string SolutionXml { get; private set; }
        public string CustomizationsXml { get; private set; }
        public ExtractResult Result { get; private set; }

        /// <summary>Number of files the new zip will contain.</summary>
        public int FileCount { get; private set; }

        /// <summary>Block of each kept component in customizations.xml (first definition).</summary>
        public IReadOnlyDictionary<SolutionComponent, LineRange> CustomizationsLines { get; private set; }

        /// <summary>RootComponent line of each kept component in solution.xml.</summary>
        public IReadOnlyDictionary<SolutionComponent, LineRange> SolutionLines { get; private set; }

        internal static SolutionPreview Create(ExtractResult result, XDocument solution, XDocument customizations,
            Dictionary<XElement, XElement> solutionMap, Dictionary<XElement, XElement> customizationsMap, int fileCount)
        {
            var preview = new SolutionPreview { Result = result, FileCount = fileCount };

            preview.SolutionXml = ToText(solution);
            preview.SolutionLines = LocateLines(solution, preview.SolutionXml, result.Kept
                .Select(c => (c, solutionMap[c.RootComponentElement])));

            preview.CustomizationsXml = ToText(customizations);
            preview.CustomizationsLines = LocateLines(customizations, preview.CustomizationsXml, result.Kept
                .Where(c => c.Definitions.Count > 0)
                .Select(c => (c, customizationsMap[c.Definitions[0]])));

            return preview;
        }

        private static string ToText(XDocument doc)
        {
            return Encoding.UTF8.GetString(XmlHelpers.Save(doc, false));
        }

        /// <summary>
        /// Finds the lines of the given elements in the serialized text: the text is parsed again with line information
        /// and elements are matched by their position in document order.
        /// </summary>
        private static Dictionary<SolutionComponent, LineRange> LocateLines(XDocument doc, string text, IEnumerable<(SolutionComponent component, XElement element)> targets)
        {
            var lines = new Dictionary<SolutionComponent, LineRange>();
            var wanted = targets.Where(t => t.element.Document == doc).ToList();
            if (wanted.Count == 0) return lines;

            var order = new Dictionary<XElement, int>();
            var index = 0;
            foreach (var e in doc.Root.DescendantsAndSelf()) order[e] = index++;

            XDocument parsed;
            using (var reader = new StringReader(text))
            {
                parsed = XDocument.Load(reader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
            }
            var parsedElements = parsed.Root.DescendantsAndSelf().ToList();
            if (parsedElements.Count != order.Count) return lines;

            var lastLine = text.Count(ch => ch == '\n') + 1;
            foreach (var (component, element) in wanted)
            {
                if (!order.TryGetValue(element, out var position)) continue;
                var match = parsedElements[position];
                lines[component] = new LineRange(Line(match), EndLine(match, lastLine));
            }
            return lines;
        }

        private static int Line(XElement element)
        {
            return ((IXmlLineInfo)element).LineNumber;
        }

        /// <summary>
        /// Line of the closing tag: the line before the next element in document order,
        /// minus one line per closing tag of the ancestors left on the way (indented XML).
        /// </summary>
        private static int EndLine(XElement element, int lastLine)
        {
            var levels = 0;
            for (var current = element; current != null; current = current.Parent, levels++)
            {
                var next = current.ElementsAfterSelf().FirstOrDefault();
                if (next != null) return Line(next) - 1 - levels;
            }
            return lastLine;
        }
    }
}
