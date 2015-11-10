namespace Localization.Xliff.OM.Core
{
    /// <summary>
    /// This interface indicates that an object contains resource strings.
    /// </summary>
    internal interface IContainerResource
    {
        /// <summary>
        /// Gets or sets the source content to be translated.
        /// </summary>
        Source Source { get; set; }

        /// <summary>
        /// Gets or sets the translated content.
        /// </summary>
        Target Target { get; set; }
    }
}
