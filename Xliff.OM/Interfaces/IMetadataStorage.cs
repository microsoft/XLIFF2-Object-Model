namespace Localization.Xliff.OM.Modules.Metadata
{
    /// <summary>
    /// This interface indicates that a class stores metadata.
    /// </summary>
    public interface IMetadataStorage
    {
        /// <summary>
        /// Gets or sets the container for metadata associated with the enclosing element.
        /// </summary>
        MetadataContainer Metadata { get; set; }
    }
}
