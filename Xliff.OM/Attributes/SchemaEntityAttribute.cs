namespace Localization.Xliff.OM.Attributes
{
    using System;

    /// <summary>
    /// This class is used to indicate a property represents data that is stored in an XLIFF attribute.
    /// </summary>
    /// <seealso cref="Attribute"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SchemaEntityAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaEntityAttribute"/> class.
        /// </summary>
        /// <param name="localName">The local name of the Xml attribute.</param>
        /// <param name="requirement">Indicates whether the attribute is required in the XLIFF document.</param>
        public SchemaEntityAttribute(string localName, Requirement requirement)
            : this(null, null, localName, requirement)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaEntityAttribute"/> class.
        /// </summary>
        /// <param name="prefix">The prefix of the Xml attribute.</param>
        /// <param name="ns">The namespace of the Xml attribute.</param>
        /// <param name="localName">The local name of the Xml attribute.</param>
        /// <param name="requirement">Indicates whether the attribute is required in the XLIFF document.</param>
        public SchemaEntityAttribute(string prefix, string ns, string localName, Requirement requirement)
        {
            ArgValidator.Create(localName, "localName").IsNotNullOrWhitespace();

            this.Name = new XmlNameInfo(prefix, ns, localName);
            this.Requirement = requirement;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the XLIFF attribute Name associated with the property.
        /// </summary>
        public XmlNameInfo Name { get; private set; }
        
        /// <summary>
        /// Gets a value that indicates whether the attribute is required in the XLIFF document.
        /// </summary>
        public Requirement Requirement { get; private set; }
        #endregion Properties
    }
}
