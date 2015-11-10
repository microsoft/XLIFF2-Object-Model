namespace Localization.Xliff.OM.Extensibility
{
    /// <summary>
    /// This interface defines a data member that stores the raw data of the extension.
    /// </summary>
    public interface IExtensionAttribute : IExtensionNameInfo
    {
        /// <summary>
        /// Gets the information related to a member that stores attribute or text information.
        /// </summary>
        string Value { get; }
    }
}
