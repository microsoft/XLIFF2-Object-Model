namespace Localization.Xliff.OM.Modules.Glossary
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.Glossary.XmlNames;

    /// <summary>
    /// This class represents a term in the glossary, expressed in the source language of the enclosing xliff element.
    /// This corresponds to a &lt;gls:term> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;gls:term [source=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#term">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class Term : XliffElement, IExtensible
    {
        /// <summary>
        /// The order in which to write data to a file so the output conforms to a defined schema.
        /// </summary>
        private static readonly IEnumerable<OutputItem> OutputOrderValues;

        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// Initializes static members of the <see cref="Term"/> class.
        /// </summary>
        static Term()
        {
            Term.OutputOrderValues = new[] 
                { 
                    new OutputItem(OutputItemType.Extension, null, 1)
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Term"/> class.
        /// </summary>
        public Term()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
        }

        #region Properties
        /// <summary>
        /// Gets the list of registered extensions on the object.
        /// </summary>
        IList<IExtension> IExtensible.Extensions
        {
            get { return this.extensions.Value; }
        }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        bool IExtensible.HasExtensions
        {
            get { return this.extensions.IsValueCreated && (this.extensions.Value.Count > 0); }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has text.
        /// </summary>
        protected override bool HasInnerText
        {
            get { return !string.IsNullOrEmpty(this.Text); }
        }

        /// <summary>
        /// Gets the order in which to write data to a file so the output conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be output in order to be compliant. This method is
        /// used during serialization to ensure the elements and text are output in the order specified by that schema.
        /// </summary>
        protected override IEnumerable<OutputItem> OutputOrder
        {
            get { return Term.OutputOrderValues; }
        }

        /// <summary>
        /// Gets or sets a value that indicates the origin of the content of the element where the attribute is defined.
        /// </summary>
        [SchemaEntity(AttributeNames.Source, Requirement.Optional)]
        public string Source
        {
            get { return (string)this.GetPropertyValue(Term.PropertyNames.Source); }
            set { this.SetPropertyValue(value, Term.PropertyNames.Source); }
        }

        /// <summary>
        /// Gets a value indicating whether attribute extensions are supported by the object.
        /// </summary>
        bool IExtensible.SupportsAttributeExtensions
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether element extensions are supported by the object.
        /// </summary>
        bool IExtensible.SupportsElementExtensions
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the term in the glossary.
        /// </summary>
        public string Text { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Gets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <returns>The text within the <see cref="XliffElement"/>.</returns>
        protected override string GetInnerText()
        {
            return this.Text;
        }

        /// <summary>
        /// Sets the text associated with the object.
        /// </summary>
        /// <param name="text">The text to set.</param>
        protected override void SetInnerText(string text)
        {
            this.Text = text;
        }
        #endregion Methods

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Term.Source"/> property.
            /// </summary>
            public const string Source = "Source";
        }
    }
}
