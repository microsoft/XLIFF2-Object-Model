namespace Localization.Xliff.OM.Modules.Glossary
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.Glossary.XmlNames;

    /// <summary>
    /// This class represents a translation of the sibling <see cref="Term"/> element expressed in the target language
    /// of the enclosing xliff element. Multiple translations can be specified as synonyms. This corresponds to a
    /// &lt;gls:translation> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;gls:translation [id=string]
    ///                        [ref=string]
    ///                        [source=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#translation">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    /// <seealso cref="ISelectable"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                         "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                         Justification = "Example contains literals described in the specification.")]
    public class Translation : XliffElement, IExtensible, ISelectable
    {
        #region Member Variables
        /// <summary>
        /// The order in which to write data to a file so the output conforms to a defined schema.
        /// </summary>
        private static readonly IEnumerable<OutputItem> OutputOrderValues;

        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;
        #endregion Member Variables

        /// <summary>
        /// Initializes static members of the <see cref="Translation"/> class.
        /// </summary>
        static Translation()
        {
            Translation.OutputOrderValues = new[] 
                { 
                    new OutputItem(OutputItemType.Extension, null, 1)
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Translation"/> class.
        /// </summary>
        public Translation()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Translation"/> class.
        /// </summary>
        /// <param name="id">The Id of the translation.</param>
        public Translation(string id)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.Id = id;
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
        /// Gets or sets the Id of the translation.
        /// </summary>
        [SchemaEntity(AttributeNames.Id, Requirement.Optional)]
        public string Id
        {
            get
            {
                return (string)this.GetPropertyValue(Translation.PropertyNames.Id);
            }

            set
            {
                value = string.IsNullOrEmpty(value) ? null : value;
                ArgValidator.Create(value, Translation.PropertyNames.Id).IsValidId(this);
                this.SetPropertyValue(value, Translation.PropertyNames.Id);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the element represents a leaf fragment in a selector path. If so, the
        /// selector path shouldn't contain any other fragments after this fragment.
        /// </summary>
        public bool IsLeafFragment
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the order in which to write data to a file so the output conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be output in order to be compliant. This method is
        /// used during serialization to ensure the elements and text are output in the order specified by that schema.
        /// </summary>
        protected override IEnumerable<OutputItem> OutputOrder
        {
            get { return Translation.OutputOrderValues; }
        }

        /// <summary>
        /// Gets or sets reference that points to a span of source or target text within the same unit, to which the
        /// translation is relevant.
        /// </summary>
        [SchemaEntity(AttributeNames.ReferenceAbbreviated, Requirement.Optional)]
        public string Reference
        {
            get { return (string)this.GetPropertyValue(Translation.PropertyNames.Reference); }
            set { this.SetPropertyValue(value, Translation.PropertyNames.Reference); }
        }

        /// <summary>
        /// Gets the selector Id of the item.
        /// </summary>
        /// <example>For a <see cref="Translation"/> item, this value might look like "gls=trans1" where "trans1" is the Id.
        /// </example>
        public string SelectorId
        {
            get { return Utilities.CreateSelectorId(Translation.Constants.SelectorPrefix, this.Id); }
        }

        /// <summary>
        /// Gets the full path of the item from the root document.
        /// </summary>
        public string SelectorPath
        {
            get { return this.BuildSelectorPath(); }
        }

        /// <summary>
        /// Gets or sets a value that indicates the origin of the content of the element where the attribute is defined.
        /// </summary>
        [SchemaEntity(AttributeNames.Source, Requirement.Optional)]
        public string Source
        {
            get { return (string)this.GetPropertyValue(Translation.PropertyNames.Source); }
            set { this.SetPropertyValue(value, Translation.PropertyNames.Source); }
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
        /// Gets or sets the translation in the glossary.
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
        /// This class contains constant values that are used in this class.
        /// </summary>
        private static class Constants
        {
            /// <summary>
            /// The selector path prefix for this element.
            /// </summary>
            public const string SelectorPrefix = "gls";
        }

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Translation.Id"/> property.
            /// </summary>
            public const string Id = "Id";

            /// <summary>
            /// The name of the <see cref="Translation.Reference"/> property.
            /// </summary>
            public const string Reference = "Reference";

            /// <summary>
            /// The name of the <see cref="Translation.Source"/> property.
            /// </summary>
            public const string Source = "Source";
        }
    }
}
