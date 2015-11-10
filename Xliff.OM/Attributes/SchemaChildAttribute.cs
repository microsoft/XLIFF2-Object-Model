namespace Localization.Xliff.OM.Attributes
{
    using System;

    /// <summary>
    /// This class is used to indicate a class contains child XLIFF elements.
    /// </summary>
    /// <seealso cref="Attribute"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class SchemaChildAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaChildAttribute"/> class.
        /// </summary>
        /// <param name="ns">The namespace of the XLIFF fragment.</param>
        /// <param name="localName">The local name of the XLIFF fragment.</param>
        /// <param name="type">The the type of the class associated with the XLIFF element.</param>
        public SchemaChildAttribute(string ns, string localName, Type type)
            : this(null, ns, localName, type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaChildAttribute"/> class.
        /// </summary>
        /// <param name="prefix">The prefix of the XLIFF fragment.</param>
        /// <param name="ns">The namespace of the XLIFF fragment.</param>
        /// <param name="localName">The local name of the XLIFF fragment.</param>
        /// <param name="type">The the type of the class associated with the XLIFF element.</param>
        public SchemaChildAttribute(string prefix, string ns, string localName, Type type)
        {
            // The value of localName may be null if the child is the inner text (ex. PlainText).
            ArgValidator.Create(localName, "name").IsNotWhitespaceOnly();
            ArgValidator.Create(type, "type").IsNotNull();

            this.ChildType = type;
            this.Name = new XmlNameInfo(prefix, ns, localName);
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the type of the class associated with the XLIFF element.
        /// </summary>
        public Type ChildType { get; private set; }

        /// <summary>
        /// Gets the XLIFF element Name of the child.
        /// </summary>
        public XmlNameInfo Name { get; private set; }
        #endregion Properties
    }
}
