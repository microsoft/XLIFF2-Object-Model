namespace Localization.Xliff.OM.Extensibility
{
    /// <summary>
    /// This class stores the contents of an extensible data member for use with an extension.
    /// </summary>
    /// <seealso cref="ExtensionNameInfo"/>
    /// <seealso cref="IExtensionAttribute"/>
    public class GenericExtensionAttribute : ExtensionNameInfo, IExtensionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericExtensionAttribute"/> class to store attribute information.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public GenericExtensionAttribute(IExtensionNameInfo name, string value)
            : base(name)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericExtensionAttribute"/> class to store attribute information.
        /// </summary>
        /// <param name="prefix">The Xml prefix of the attribute.</param>
        /// <param name="ns">The namespace of the attribute.</param>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public GenericExtensionAttribute(string prefix, string ns, string localName, string value)
            : base(prefix, ns, localName)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the information related to a member that stores attribute or text information.
        /// </summary>
        public string Value { get; private set; }
    }
}
