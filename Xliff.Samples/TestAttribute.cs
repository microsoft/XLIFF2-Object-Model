namespace Localization.Xliff.Samples
{
    using Localization.Xliff.OM.Extensibility;

    /// <summary>
    /// This class stores custom attributes of an extensible data member for use with an extension.
    /// </summary>
    public class TestAttribute : IExtensionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestAttribute"/> class to store attribute information.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public TestAttribute(IExtensionNameInfo name, string value)
        {
            this.LocalName = name.LocalName;
            this.Namespace = name.Namespace;
            this.Prefix = name.Prefix;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAttribute"/> class to store attribute information.
        /// </summary>
        /// <param name="prefix">The Xml prefix of the attribute.</param>
        /// <param name="ns">The namespace of the attribute.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public TestAttribute(string prefix, string ns, string name, string value)
        {
            this.LocalName = name;
            this.Namespace = ns;
            this.Prefix = prefix;
            this.Value = value;
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
        /// Gets the Xml prefix of the member.
        /// </summary>
        public string Prefix { get; private set; }

        /// <summary>
        /// Gets the information related to a member that stores attribute or text information.
        /// </summary>
        public string Value { get; private set; }
    }
}
