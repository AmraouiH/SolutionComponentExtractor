using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SolutionComponentExtractor.Core
{
    /// <summary>Turns Dataverse solution import errors into messages a user can act on.</summary>
    public static class ImportDiagnostics
    {
        private static readonly Regex PrivilegeRegex = new Regex(@"PrivilegeName:\s*(?<name>prv\w+)", RegexOptions.IgnoreCase);
        private static readonly Regex PrivilegeVerbRegex = new Regex("^prv(?<verb>Create|Read|Write|Delete|Append|AppendTo|Assign|Share|Import|Export|Publish)(?<target>.*)$", RegexOptions.IgnoreCase);

        /// <summary>Readable names of the privileges a solution import typically needs.</summary>
        private static readonly Dictionary<string, string> PrivilegeTargets = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Entity"] = "tables",
            ["Attribute"] = "columns",
            ["Relationship"] = "relationships",
            ["Customization"] = "customizations",
            ["Solution"] = "solutions",
            ["Workflow"] = "processes",
            ["WebResource"] = "web resources",
            ["PluginAssembly"] = "plug-in assemblies",
            ["PluginType"] = "plug-in types",
            ["SdkMessageProcessingStep"] = "plug-in steps",
            ["Role"] = "security roles",
            ["SystemForm"] = "forms and dashboards",
            ["Query"] = "views",
            ["CustomControl"] = "custom controls (PCF)",
            ["AppModule"] = "model-driven apps",
            ["SiteMap"] = "site maps",
            ["CanvasApp"] = "canvas apps",
            ["FieldSecurityProfile"] = "field security profiles",
            ["ServiceEndpoint"] = "service endpoints",
            ["OptionSet"] = "choices",
        };

        /// <summary>Short explanation and advice for a raw import error; returns the raw message when it is not recognized.</summary>
        public static string Explain(string rawMessage, string environment)
        {
            if (string.IsNullOrWhiteSpace(rawMessage)) return "The import failed without an error message.";
            environment = string.IsNullOrWhiteSpace(environment) ? "the target environment" : environment;

            var privilege = PrivilegeRegex.Match(rawMessage);
            if (privilege.Success || rawMessage.IndexOf("CheckPrivilege", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var name = privilege.Success ? privilege.Groups["name"].Value : null;
                var action = name == null ? "perform this import" : DescribePrivilege(name);
                return $"Your user is not allowed to {action} in {environment}"
                       + (name == null ? "." : $" (missing privilege {name}).")
                       + Environment.NewLine + Environment.NewLine
                       + "Ask an administrator to give your user the System Customizer or System Administrator security role in this environment, "
                       + "or import with a user that has it.";
            }

            if (rawMessage.IndexOf("missing", StringComparison.OrdinalIgnoreCase) >= 0
                && rawMessage.IndexOf("depend", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return $"Some components of the solution depend on components that do not exist in {environment}."
                       + Environment.NewLine + Environment.NewLine
                       + "Keep those components in the new solution, or import them in the target environment first."
                       + Environment.NewLine + Environment.NewLine + rawMessage;
            }

            return rawMessage;
        }

        /// <summary>"prvCreateEntity" → "create tables".</summary>
        public static string DescribePrivilege(string privilegeName)
        {
            var match = PrivilegeVerbRegex.Match(privilegeName ?? string.Empty);
            if (!match.Success) return $"use the privilege {privilegeName}";

            var target = match.Groups["target"].Value;
            var readableTarget = PrivilegeTargets.TryGetValue(target, out var known) ? known : target;
            return $"{match.Groups["verb"].Value.ToLowerInvariant()} {readableTarget}";
        }

        /// <summary>
        /// Components reported as failed in an import job (importjob.data): every element that has
        /// a &lt;result result="failure" errortext="..." /&gt; child.
        /// </summary>
        public static List<string> ParseFailures(string importJobData)
        {
            var failures = new List<string>();
            if (string.IsNullOrWhiteSpace(importJobData)) return failures;

            XDocument doc;
            try
            {
                doc = XDocument.Parse(importJobData);
            }
            catch (System.Xml.XmlException)
            {
                return failures;
            }

            foreach (var result in doc.Descendants().Where(e => e.Is("result")))
            {
                if (!string.Equals(result.Attr("result"), "failure", StringComparison.OrdinalIgnoreCase)) continue;

                var component = result.Parent;
                var name = new[] { "LocalizedName", "OriginalName", "name", "id" }
                    .Select(a => component?.Attr(a))
                    .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v)) ?? component?.Name.LocalName ?? "Component";
                var kind = component?.Name.LocalName;
                var error = result.Attr("errortext");
                failures.Add($"{(kind != null && !string.Equals(kind, name, StringComparison.OrdinalIgnoreCase) ? kind + " " : string.Empty)}{name}: {(string.IsNullOrWhiteSpace(error) ? result.Attr("errorcode") : error)}");
            }
            return failures.Distinct().ToList();
        }
    }
}
