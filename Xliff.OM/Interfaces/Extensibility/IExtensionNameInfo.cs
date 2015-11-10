namespace Localization.Xliff.OM.Extensibility
{
    /// <summary>
    /// This interface defines the name that uniquely identifies an extension member.
    /// </summary>
    public interface IExtensionNameInfo
    {
        /// <summary>
        /// Gets the local name of the member.
        /// </summary>
        string LocalName { get; }

        /// <summary>
        /// Gets the namespace of the member.
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Gets the Xml prefix of the member.
        /// </summary>
        string Prefix { get; }
    }
}
