namespace Localization.Xliff.OM.Extensibility
{
    /// <summary>
    /// This interface defines an extension that is registered on extensible objects.
    /// </summary>
    public interface IExtension : IExtensionData
    {
        /// <summary>
        /// Gets the name of the extension.
        /// </summary>
        string Name { get; }
    }
}
