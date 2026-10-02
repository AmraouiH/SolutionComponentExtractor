using System;
using System.Collections.Generic;

namespace SolutionComponentExtractor.Core
{
    /// <summary>
    /// Known solution component types (solutioncomponent.componenttype) and where their
    /// definitions live inside customizations.xml.
    /// </summary>
    public static class ComponentTypes
    {
        public const int Entity = 1;
        public const int Attribute = 2;
        public const int OptionSet = 9;
        public const int EntityRelationship = 10;
        public const int Role = 20;
        public const int SavedQuery = 26;
        public const int Workflow = 29;
        public const int Report = 31;
        public const int EmailTemplate = 36;
        public const int SavedQueryVisualization = 59;
        public const int SystemForm = 60;
        public const int WebResource = 61;
        public const int SiteMap = 62;

        private static readonly Dictionary<int, string> Names = new Dictionary<int, string>
        {
            [1] = "Table (Entity)",
            [2] = "Column (Attribute)",
            [3] = "Relationship",
            [9] = "Choice (Global Option Set)",
            [10] = "Table Relationship",
            [11] = "Relationship Role",
            [13] = "Managed Property",
            [14] = "Table Key",
            [16] = "Privilege",
            [20] = "Security Role",
            [21] = "Role Privilege",
            [22] = "Display String",
            [24] = "Form",
            [25] = "Organization",
            [26] = "View (Saved Query)",
            [29] = "Process (Workflow / Flow / Action)",
            [31] = "Report",
            [36] = "Email Template",
            [37] = "Contract Template",
            [38] = "Article Template",
            [39] = "Mail Merge Template",
            [44] = "Duplicate Detection Rule",
            [46] = "Table Map",
            [47] = "Column Map",
            [48] = "Ribbon Command",
            [50] = "Ribbon Customization",
            [55] = "Ribbon Diff",
            [59] = "Chart",
            [60] = "Form / Dashboard (System Form)",
            [61] = "Web Resource",
            [62] = "Site Map",
            [63] = "Connection Role",
            [65] = "Hierarchy Rule",
            [66] = "Custom Control (PCF)",
            [68] = "Custom Control Default Config",
            [70] = "Field Security Profile",
            [71] = "Field Permission",
            [80] = "Model-driven App",
            [90] = "Plug-in Type",
            [91] = "Plug-in Assembly",
            [92] = "Plug-in Step",
            [93] = "Plug-in Step Image",
            [95] = "Service Endpoint",
            [150] = "Routing Rule",
            [152] = "SLA",
            [154] = "Convert Rule",
            [161] = "Mobile Offline Profile",
            [165] = "Similarity Rule",
            [166] = "Data Source Mapping",
            [208] = "Import Map",
            [300] = "Canvas App",
            [371] = "Connector",
            [372] = "Connector",
            [380] = "Environment Variable Definition",
            [381] = "Environment Variable Value",
            [400] = "AI Project Type",
            [401] = "AI Project",
            [402] = "AI Configuration",
            [430] = "Table Analytics Configuration",
            [431] = "Column Image Configuration",
            [432] = "Table Image Configuration",
        };

        /// <summary>customizations.xml sections (children of ImportExportXml) holding the type's definitions.</summary>
        private static readonly Dictionary<int, string[]> Sections = new Dictionary<int, string[]>
        {
            [1] = new[] { "Entities" },
            [9] = new[] { "optionsets" },
            [10] = new[] { "EntityRelationships" },
            [20] = new[] { "Roles" },
            [29] = new[] { "Workflows" },
            [44] = new[] { "DuplicateRules" },
            [61] = new[] { "WebResources" },
            [62] = new[] { "AppModuleSiteMaps", "SiteMap" },
            [63] = new[] { "ConnectionRoles" },
            [66] = new[] { "CustomControls" },
            [70] = new[] { "FieldSecurityProfiles" },
            [80] = new[] { "AppModules" },
            [91] = new[] { "SolutionPluginAssemblies" },
            [92] = new[] { "SdkMessageProcessingSteps" },
            [95] = new[] { "ServiceEndpoints" },
            [300] = new[] { "CanvasApps" },
            [371] = new[] { "Connectors" },
            [372] = new[] { "Connectors" },
        };

        /// <summary>
        /// Sub-components stored deep inside other definitions (e.g. a form inside an entity),
        /// located by their own id element.
        /// </summary>
        private static readonly Dictionary<int, string> DeepIdElements = new Dictionary<int, string>
        {
            [26] = "savedqueryid",
            [31] = "reportid",
            [36] = "templateid",
            [59] = "savedqueryvisualizationid",
            [60] = "formid",
        };

        /// <summary>Sections that only make sense together with tables; never used for generic matching.</summary>
        public static readonly string[] EntitySections = { "Entities", "EntityRelationships", "EntityMaps" };

        public static string GetName(int type)
        {
            return Names.TryGetValue(type, out var name) ? name : null;
        }

        public static string[] GetSections(int type)
        {
            return Sections.TryGetValue(type, out var sections) ? sections : null;
        }

        public static string GetDeepIdElement(int type)
        {
            return DeepIdElements.TryGetValue(type, out var name) ? name : null;
        }

        public static string GetBehaviorLabel(int type, string behavior)
        {
            if (type != Entity || string.IsNullOrEmpty(behavior)) return null;
            switch (behavior)
            {
                case "0": return "include all subcomponents";
                case "1": return "do not include subcomponents";
                case "2": return "include as shell only";
                default: return "behavior " + behavior;
            }
        }

        /// <summary>Labels of sections/folders holding components whose type code is org-specific.</summary>
        private static readonly Dictionary<string, string> ContainerLabels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["connectionreferences"] = "Connection Reference",
            ["environmentvariabledefinitions"] = "Environment Variable Definition",
            ["environmentvariablevalues"] = "Environment Variable Value",
            ["bots"] = "Copilot (Bot)",
            ["botcomponents"] = "Copilot Component",
            ["aiplugins"] = "AI Plugin",
            ["aipluginoperations"] = "AI Plugin Operation",
            ["dvtablesearchs"] = "Dataverse Search",
            ["customapis"] = "Custom API",
            ["customapirequestparameters"] = "Custom API Request Parameter",
            ["customapiresponseproperties"] = "Custom API Response Property",
            ["pluginpackages"] = "Plug-in Package",
            ["appactions"] = "Command (App Action)",
            ["catalogs"] = "Catalog",
            ["catalogassignments"] = "Catalog Assignment",
        };

        /// <summary>Turns a section/folder name such as "connectionreferences" into a readable label.</summary>
        public static string LabelFromContainer(string container)
        {
            if (string.IsNullOrEmpty(container)) return null;
            if (ContainerLabels.TryGetValue(container.Trim('/'), out var label)) return label;
            var name = container.Trim('/');
            if (name.EndsWith("ies", StringComparison.OrdinalIgnoreCase)) name = name.Substring(0, name.Length - 3) + "y";
            else if (name.EndsWith("s", StringComparison.OrdinalIgnoreCase)) name = name.Substring(0, name.Length - 1);
            return char.ToUpperInvariant(name[0]) + name.Substring(1);
        }
    }
}
