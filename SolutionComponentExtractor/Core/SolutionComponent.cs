using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SolutionComponentExtractor.Core
{
    /// <summary>A root component of the solution (solution.xml / RootComponents / RootComponent).</summary>
    public sealed class SolutionComponent
    {
        public int Type { get; internal set; }

        /// <summary>Label used to group components in the UI.</summary>
        public string TypeName { get; internal set; }

        public string SchemaName { get; internal set; }

        /// <summary>Normalized id (lowercase GUID without braces), when the root component is identified by id.</summary>
        public string Id { get; internal set; }

        public string Behavior { get; internal set; }

        public string DisplayName { get; internal set; }

        /// <summary>The RootComponent element in solution.xml.</summary>
        internal XElement RootComponentElement { get; set; }

        /// <summary>Definition elements found in customizations.xml.</summary>
        internal List<XElement> Definitions { get; } = new List<XElement>();

        /// <summary>Zip folders (e.g. "environmentvariabledefinitions/new_var/") owned by this component.</summary>
        internal List<string> Folders { get; } = new List<string>();

        /// <summary>Component whose definition contains this one (e.g. the table of a form).</summary>
        public SolutionComponent Parent { get; internal set; }

        public List<SolutionComponent> Children { get; } = new List<SolutionComponent>();

        /// <summary>Whether the component is kept in the generated solution.</summary>
        public bool Selected { get; set; } = true;

        public bool IsLocated => Definitions.Count > 0 || Folders.Count > 0;

        /// <summary>
        /// Managed component exported as a bare reference (unmodified="1"): it has no content of its own,
        /// so importing it changes nothing in the target environment.
        /// </summary>
        public bool IsUnmodified => Definitions.Count > 0 && Definitions.All(d => d.Attr("unmodified") == "1");

        public string Identifier => SchemaName ?? Id;

        public override string ToString()
        {
            return DisplayName == null || DisplayName == Identifier ? Identifier : $"{DisplayName} ({Identifier})";
        }
    }
}
