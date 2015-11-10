namespace Localization.Xliff.OM.Serialization
{
    /// <summary>
    /// Describes how much detail to write in the output of data.
    /// </summary>
    public enum OutputDetail
    {
        /// <summary>
        /// Write only the minimal required to describe content.
        /// </summary>
        Minimal,

        /// <summary>
        /// Write the minimal amount plus the data that was explicitly set by the developer.
        /// </summary>
        Explicit,

        /// <summary>
        /// Write all content, even if defaults are used.
        /// </summary>
        Full
    }
}
