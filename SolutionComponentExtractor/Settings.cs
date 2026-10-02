namespace SolutionComponentExtractor
{
    /// <summary>
    /// Settings persisted by XrmToolBox between sessions.
    /// </summary>
    /// <remarks>
    /// This class must be XML serializable
    /// </remarks>
    public class Settings
    {
        public string LastUsedOrganizationWebappUrl { get; set; }

        /// <summary>Folder of the last opened or generated solution.</summary>
        public string LastFolder { get; set; }
    }
}
