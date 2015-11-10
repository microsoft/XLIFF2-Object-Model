namespace Localization.Xliff.OM.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Indicators;
    using Localization.Xliff.OM.Modules.FormatStyle;
    using Localization.Xliff.OM.XmlNames;
    using FsXmlNames = Localization.Xliff.OM.Modules.FormatStyle.XmlNames;

    /// <summary>
    /// This class represents a note associated with elements of an XLIFF document. This corresponds to a
    /// &lt;note> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;note [id=string]
    ///              [appliesTo=(source|target)]
    ///              [category=string]
    ///              [priority=int] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#note">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    /// <seealso cref="IFormatStyleAttributes"/>
    /// <seealso cref="ISelectable"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class Note : XliffElement,
                        IExtensible,
                        IFormatStyleAttributes,
                        ISelectable
    {
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Note"/> class.
        /// </summary>
        /// <param name="text">The text to put in the note.</param>
        public Note(string text)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.SubFormatStyle = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Note"/> class.
        /// </summary>
        internal Note()
            : this(null)
        {
        }

        #region Properties
        /// <summary>
        /// Gets or sets the subject the note applies to.
        /// </summary>
        [Converter(typeof(TranslationSubjectConverter))]
        [SchemaEntity(AttributeNames.AppliesTo, Requirement.Optional)]
        public TranslationSubject? AppliesTo
        {
            get { return (TranslationSubject?)this.GetPropertyValue(Note.PropertyNames.AppliesTo); }
            set { this.SetPropertyValue(value, Note.PropertyNames.AppliesTo); }
        }

        /// <summary>
        /// Gets or sets the category that is used to categorize notes.
        /// </summary>
        [SchemaEntity(AttributeNames.Category, Requirement.Optional)]
        public string Category
        {
            get { return (string)this.GetPropertyValue(Note.PropertyNames.Category); }
            set { this.SetPropertyValue(value, Note.PropertyNames.Category); }
        }

        /// <summary>
        /// Gets the list of registered extensions on the object.
        /// </summary>
        IList<IExtension> IExtensible.Extensions
        {
            get { return this.extensions.Value; }
        }

        /// <summary>
        /// Gets or sets basic formatting information using a predefined subset of HTML formatting elements. It enables
        /// the generation of HTML pages or snippets for preview and review purposes.
        /// </summary>
        [Converter(typeof(FormatStyleValueConverter))]
        [SchemaEntity(
                      NamespacePrefixes.FormatStyleModule,
                      NamespaceValues.FormatStyleModule,
                      FsXmlNames.AttributeNames.FormatStyle,
                      Requirement.Optional)]
        public FormatStyleValue? FormatStyle
        {
            get { return (FormatStyleValue?)this.GetPropertyValue(Note.PropertyNames.FormatStyle); }
            set { this.SetPropertyValue(value, Note.PropertyNames.FormatStyle); }
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
        /// Gets or sets the Id of the note.
        /// </summary>
        [SchemaEntity(AttributeNames.Id, Requirement.Optional)]
        public string Id
        {
            get
            {
                return (string)this.GetPropertyValue(Note.PropertyNames.Id);
            }

            set
            {
                value = string.IsNullOrEmpty(value) ? null : value;
                ArgValidator.Create(value, Note.PropertyNames.Id).IsValidId(this);
                this.SetPropertyValue(value, Note.PropertyNames.Id);
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
        /// Gets or sets the priority of the note.
        /// </summary>
        [Converter(typeof(IntConverter))]
        [DefaultValue(1)]
        [SchemaEntity(AttributeNames.Priority, Requirement.Optional)]
        public int Priority
        {
            get { return (int)this.GetPropertyValue(Note.PropertyNames.Priority); }
            set { this.SetPropertyValue(value, Note.PropertyNames.Priority); }
        }

        /// <summary>
        /// Gets the selector Id of the item.
        /// </summary>
        /// <example>For a <see cref="Note"/> item, this value might look like "n=note1" where "note1" is the Id.
        /// </example>
        public string SelectorId
        {
            get { return Utilities.CreateSelectorId(Note.Constants.SelectorPrefix, this.Id); }
        }

        /// <summary>
        /// Gets the full path of the item from the root document.
        /// </summary>
        public string SelectorPath
        {
            get { return this.BuildSelectorPath(); }
        }

        /// <summary>
        /// Gets extra HTML attributes to use along with the HTML element declared in the FormatStyle attribute. The
        /// key is the HTML attribute name and the value is the value for that attribute.
        /// </summary>
        [Converter(typeof(SubFormatStyleConverter))]
        [HasValueIndicator(typeof(DictionaryValueIndicator<string, string>))]
        [SchemaEntity(
                      NamespacePrefixes.FormatStyleModule,
                      NamespaceValues.FormatStyleModule,
                      FsXmlNames.AttributeNames.SubFormatStyle,
                      Requirement.Optional)]
        public IDictionary<string, string> SubFormatStyle
        {
            get { return (IDictionary<string, string>)this.GetPropertyValue(Note.PropertyNames.SubFormatStyle); }
            private set { this.SetPropertyValue(value, Note.PropertyNames.SubFormatStyle); }
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
        /// Gets or sets the text of the note.
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
        internal static class Constants
        {
            /// <summary>
            /// The selector path prefix for this element.
            /// </summary>
            public const string SelectorPrefix = "n";
        }

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Note.AppliesTo"/> property.
            /// </summary>
            public const string AppliesTo = "AppliesTo";

            /// <summary>
            /// The name of the <see cref="Note.Category"/> property.
            /// </summary>
            public const string Category = "Category";

            /// <summary>
            /// The name of the <see cref="Note.FormatStyle"/> property.
            /// </summary>
            public const string FormatStyle = "FormatStyle";

            /// <summary>
            /// The name of the <see cref="Note.Id"/> property.
            /// </summary>
            public const string Id = "Id";

            /// <summary>
            /// The name of the <see cref="Note.Priority"/> property.
            /// </summary>
            public const string Priority = "Priority";

            /// <summary>
            /// The name of the <see cref="Note.SubFormatStyle"/> property.
            /// </summary>
            public const string SubFormatStyle = "SubFormatStyle";
        }
    }
}