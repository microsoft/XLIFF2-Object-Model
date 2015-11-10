namespace Localization.Xliff.OM.Extensibility
{
    /// <summary>
    /// This class defines the name that uniquely identifies an extension member.
    /// </summary>
    /// <seealso cref="IExtensionNameInfo"/>
    public class ExtensionNameInfo : IExtensionNameInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionNameInfo"/> class.
        /// </summary>
        /// <param name="name">The name that identifies the member.</param>
        public ExtensionNameInfo(IExtensionNameInfo name)
        {
            this.LocalName = name.LocalName;
            this.Namespace = name.Namespace;
            this.Prefix = name.Prefix;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionNameInfo"/> class.
        /// </summary>
        /// <param name="prefix">The Xml prefix of the member used when writing information to a file.</param>
        /// <param name="ns">The namespace of the member.</param>
        /// <param name="localName">The local name of the member.</param>
        public ExtensionNameInfo(string prefix, string ns, string localName)
        {
            ArgValidator.Create(ns, "ns").IsNotNullOrWhitespace();
            ArgValidator.Create(localName, "localName").IsNotNullOrWhitespace();

            this.LocalName = localName;
            this.Namespace = ns;
            this.Prefix = prefix;
        }

        /// <summary>
        /// Gets the local name of the member.
        /// </summary>
        public string LocalName { get; private set; }

        /// <summary>
        /// Gets the namespace of the member.
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// Gets the Xml prefix of the member used when writing information to a file.
        /// </summary>
        public string Prefix { get; private set; }
    }
}
