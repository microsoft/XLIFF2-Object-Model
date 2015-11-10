namespace Localization.Xliff.OM.Modules.Metadata
{
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Modules.Metadata.XmlNames;

    /// <summary>
    /// This class represents a container for a single metadata component. This corresponds to a &lt;mda:meta>
    /// element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;mda:meta type=string .../>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#meta">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="MetaElement"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class Meta : MetaElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Meta"/> class.
        /// </summary>
        /// <param name="type">The type of metadata contained by the enclosing element.</param>
        /// <param name="text">The non-translatable text.</param>
        public Meta(string type, string text)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.NonTranslatableText = text;

            if (type != null)
            {
                this.Type = type;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Meta"/> class.
        /// </summary>
        /// <param name="text">The non-translatable text.</param>
        public Meta(string text)
            : this(null, text)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Meta"/> class.
        /// </summary>
        internal Meta()
            : this(null)
        {
        }

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the provider has text.
        /// </summary>
        protected override bool HasInnerText
        {
            get { return !string.IsNullOrEmpty(this.NonTranslatableText); }
        }

        /// <summary>
        /// Gets or sets the non-translatable text.
        /// </summary>
        public string NonTranslatableText { get; set; }

        /// <summary>
        /// Gets or sets the type of metadata contained by the enclosing element.
        /// </summary>
        [SchemaEntity(AttributeNames.Type, Requirement.Required)]
        public string Type
        {
            get { return (string)this.GetPropertyValue(Meta.PropertyNames.Type); }
            set { this.SetPropertyValue(value, Meta.PropertyNames.Type); }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Gets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <returns>The text within the <see cref="XliffElement"/>.</returns>
        protected override string GetInnerText()
        {
            return this.NonTranslatableText;
        }

        /// <summary>
        /// Sets the text associated with the object.
        /// </summary>
        /// <param name="text">The text to set.</param>
        protected override void SetInnerText(string text)
        {
            this.NonTranslatableText = text;
        }
        #endregion Methods

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Meta.Type"/> property.
            /// </summary>
            public const string Type = "Type";
        }
    }
}
