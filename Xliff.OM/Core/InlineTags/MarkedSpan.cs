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
    using Localization.Xliff.OM.Modules.SizeRestriction;
    using Localization.Xliff.OM.XmlNames;
    using FsXmlNames = Localization.Xliff.OM.Modules.FormatStyle.XmlNames;
    using SizeXmlNames = Localization.Xliff.OM.Modules.SizeRestriction.XmlNames;

    /// <summary>
    /// This class represents an annotation pertaining to the marked span. This corresponds to a &lt;mrk> element in
    /// the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;mrk id=string
    ///             [translate=(yes|no)]
    ///             [type=(generic|comment|term|string)]
    ///             [ref=string]
    ///             [value=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#mrk">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="ResourceStringContent"/>
    /// <seealso cref="IExtensible"/>
    /// <seealso cref="IFormatStyleAttributes"/>
    /// <seealso cref="IResourceStringContentContainer"/>
    /// <seealso cref="ISelectable"/>
    /// <seealso cref="ISizeRestrictionAttributes"/>
    [SchemaChild(NamespaceValues.Core, ElementNames.CDataTag, typeof(CDataTag))]
    [SchemaChild(NamespaceValues.Core, ElementNames.CodePoint, typeof(CodePoint))]
    [SchemaChild(NamespaceValues.Core, ElementNames.CommentTag, typeof(CommentTag))]
    [SchemaChild(NamespaceValues.Core, ElementNames.PlainText, typeof(PlainText))]
    [SchemaChild(NamespaceValues.Core, ElementNames.ProcessingInstructionTag, typeof(ProcessingInstructionTag))]
    [SchemaChild(NamespaceValues.Core, ElementNames.StandaloneCode, typeof(StandaloneCode))]
    [SchemaChild(NamespaceValues.Core, ElementNames.SpanningCode, typeof(SpanningCode))]
    [SchemaChild(NamespaceValues.Core, ElementNames.SpanningCodeEnd, typeof(SpanningCodeEnd))]
    [SchemaChild(NamespaceValues.Core, ElementNames.SpanningCodeStart, typeof(SpanningCodeStart))]
    [SchemaChild(NamespaceValues.Core, ElementNames.MarkedSpan, typeof(MarkedSpan))]
    [SchemaChild(NamespaceValues.Core, ElementNames.MarkedSpanEnd, typeof(MarkedSpanEnd))]
    [SchemaChild(NamespaceValues.Core, ElementNames.MarkedSpanStart, typeof(MarkedSpanStart))]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class MarkedSpan : ResourceStringContent,
                              IExtensible,
                              IFormatStyleAttributes,
                              IOutputResolver,
                              IResourceStringContentContainer,
                              ISelectable,
                              ISizeRestrictionAttributes
    {
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkedSpan"/> class.
        /// </summary>
        /// <param name="id">The Id of the marked span.</param>
        public MarkedSpan(string id)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.Id = id;
            this.SubFormatStyle = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.Text = new ParentAttachedList<ResourceStringContent>(this);

            this.EnableAttribute(MarkedSpan.PropertyNames.EquivalentStorage, false);
            this.EnableAttribute(MarkedSpan.PropertyNames.SizeInfo, false);
            this.EnableAttribute(MarkedSpan.PropertyNames.SizeInfoReference, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkedSpan"/> class.
        /// </summary>
        internal MarkedSpan()
            : this(null)
        {
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets a means to specify how much storage space an inline element will use in the native format.
        /// This property is not supported on this object so a NotSupportedException will always be thrown.
        /// </summary>
        [SchemaEntity(
                      NamespacePrefixes.SizeRestrictionModule,
                      NamespaceValues.SizeRestrictionModule,
                      SizeXmlNames.AttributeNames.EquivalentStorage,
                      Requirement.Optional)]
        public string EquivalentStorage
        {
            get
            {
                string message;

                message = string.Format(
                                        Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                        MarkedSpan.PropertyNames.EquivalentStorage);
                throw new NotSupportedException(message);
            }

            set
            {
                string message;

                message = string.Format(
                                        Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                        MarkedSpan.PropertyNames.EquivalentStorage);
                throw new NotSupportedException(message);
            }
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
            get { return (FormatStyleValue?)this.GetPropertyValue(MarkedSpan.PropertyNames.FormatStyle); }
            set { this.SetPropertyValue(value, MarkedSpan.PropertyNames.FormatStyle); }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.Text.Count > 0); }
        }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        bool IExtensible.HasExtensions
        {
            get { return this.extensions.IsValueCreated && (this.extensions.Value.Count > 0); }
        }

        /// <summary>
        /// Gets or sets the Id of the marked span.
        /// </summary>
        [SchemaEntity(AttributeNames.Id, Requirement.Required)]
        public string Id
        {
            get
            {
                return (string)this.GetPropertyValue(MarkedSpan.PropertyNames.Id);
            }

            set
            {
                value = string.IsNullOrEmpty(value) ? null : value;
                ArgValidator.Create(value, MarkedSpan.PropertyNames.Id).IsValidId(this);
                this.SetPropertyValue(value, MarkedSpan.PropertyNames.Id);
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
        /// Gets or sets the reference for the associated annotation.
        /// </summary>
        [SchemaEntity(AttributeNames.ReferenceAbbreviated, Requirement.Optional)]
        public string Reference
        {
            get { return (string)this.GetPropertyValue(MarkedSpan.PropertyNames.Reference); }
            set { this.SetPropertyValue(value, MarkedSpan.PropertyNames.Reference); }
        }

        /// <summary>
        /// Gets the selector Id of the item.
        /// </summary>
        /// <example>For an inline tag item, this value might look like "1".
        /// </example>
        public string SelectorId
        {
            get
            {
                ResourceString parent;
                string id;

                id = this.Id;

                parent = this.Parent as ResourceString;
                if ((parent != null) && (parent.ChildSelectorIdPrefix != null))
                {
                    id = Utilities.CreateSelectorId(parent.ChildSelectorIdPrefix, id);
                }

                return id;
            }
        }

        /// <summary>
        /// Gets the full path of the item from the root document.
        /// </summary>
        public string SelectorPath
        {
            get { return this.BuildSelectorPath(); }
        }

        /// <summary>
        /// Gets or sets profile specific information to inline elements so that size information can be decoupled from
        /// the native format or represented when the native data is not available in the XLIFF document.
        /// This property is not supported on this object so a NotSupportedException will always be thrown.
        /// </summary>
        [SchemaEntity(
                      NamespacePrefixes.SizeRestrictionModule,
                      NamespaceValues.SizeRestrictionModule,
                      SizeXmlNames.AttributeNames.SizeInfo,
                      Requirement.Optional)]
        public string SizeInfo
        {
            get
            {
                string message;

                message = string.Format(
                                        Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                        MarkedSpan.PropertyNames.SizeInfo);
                throw new NotSupportedException(message);
            }

            set
            {
                string message;

                message = string.Format(
                                        Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                        MarkedSpan.PropertyNames.SizeInfo);
                throw new NotSupportedException(message);
            }
        }

        /// <summary>
        /// Gets or sets a reference to data that provide the same information that could be otherwise put in a sizeInfo
        /// attribute. This property is not supported on this object so a NotSupportedException will always be thrown. 
        /// </summary>
        [SchemaEntity(
                     NamespacePrefixes.SizeRestrictionModule,
                     NamespaceValues.SizeRestrictionModule,
                     SizeXmlNames.AttributeNames.SizeInfoReference,
                     Requirement.Optional)]
        public string SizeInfoReference
        {
            get
            {
                string message;

                message = string.Format(
                                        Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                        MarkedSpan.PropertyNames.SizeInfoReference);
                throw new NotSupportedException(message);
            }

            set
            {
                string message;

                message = string.Format(
                                        Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                        MarkedSpan.PropertyNames.SizeInfoReference);
                throw new NotSupportedException(message);
            }
        }

        /// <summary>
        /// Gets or sets the size restriction to apply to the collection descendants of the element it is defined on.
        /// Interpretation of the value is dependent on the selected <see cref="Profiles.GeneralProfile"/>. It must
        /// represent the restriction to apply to the indicated sub part of the document.
        /// </summary>
        [SchemaEntity(
                      NamespacePrefixes.SizeRestrictionModule,
                      NamespaceValues.SizeRestrictionModule,
                      SizeXmlNames.AttributeNames.SizeRestriction,
                      Requirement.Optional)]
        public string SizeRestriction
        {
            get { return (string)this.GetPropertyValue(MarkedSpan.PropertyNames.SizeRestriction); }
            set { this.SetPropertyValue(value, MarkedSpan.PropertyNames.SizeRestriction); }
        }

        /// <summary>
        /// Gets or sets the storage restriction to apply to the collection descendants of the element it is defined on.
        /// Interpretation of the value is dependent on the selected <see cref="Profiles.StorageProfile"/>. It must
        /// represent the restriction to apply to the indicated sub part of the document.
        /// </summary>
        [SchemaEntity(
                      NamespacePrefixes.SizeRestrictionModule,
                      NamespaceValues.SizeRestrictionModule,
                      SizeXmlNames.AttributeNames.StorageRestriction,
                      Requirement.Optional)]
        public string StorageRestriction
        {
            get { return (string)this.GetPropertyValue(MarkedSpan.PropertyNames.StorageRestriction); }
            set { this.SetPropertyValue(value, MarkedSpan.PropertyNames.StorageRestriction); }
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
            get { return (IDictionary<string, string>)this.GetPropertyValue(MarkedSpan.PropertyNames.SubFormatStyle); }
            private set { this.SetPropertyValue(value, MarkedSpan.PropertyNames.SubFormatStyle); }
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
        /// Gets a value indicating whether the object supports the EquivalentStorage property.
        /// </summary>
        public bool SupportsEquivalentStorage
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the object supports the SizeInfo property.
        /// </summary>
        public bool SupportsSizeInfo
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the object supports the SizeInfoReference property.
        /// </summary>
        public bool SupportsSizeInfoReference
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the object supports the SizeRestriction property.
        /// </summary>
        public bool SupportsSizeRestriction
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the object supports the StorageRestriction property.
        /// </summary>
        public bool SupportsStorageRestriction
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the source content is intended to be translated.
        /// </summary>
        [Converter(typeof(BoolConverter))]
        [DefaultValue(true)]
        [ExplicitOutputDependency]
        [InheritValue(Inheritance.AncestorType, typeof(MarkedSpan))]
        [InheritValue(Inheritance.AncestorType, typeof(Unit))]
        [SchemaEntity(AttributeNames.Translate, Requirement.Optional)]
        public bool Translate
        {
            get { return (bool)this.GetPropertyValue(MarkedSpan.PropertyNames.Translate); }
            set { this.SetPropertyValue(value, MarkedSpan.PropertyNames.Translate); }
        }

        /// <summary>
        /// Gets the elements that describe the text.
        /// </summary>
        public IList<ResourceStringContent> Text { get; private set; }

        /// <summary>
        /// Gets or sets the type of the element.
        /// </summary>
        /// <remarks>If the value is not null, 'generic', 'comment', or 'term', the format of the value must be
        /// "prefix:value" where prefix must not be "xlf" and prefix and value must not be empty
        /// strings.</remarks>
        [DefaultValue(MarkedSpanTypes.Generic)]
        [SchemaEntity(AttributeNames.Type, Requirement.Optional)]
        public string Type
        {
            get { return (string)this.GetPropertyValue(MarkedSpan.PropertyNames.Type); }
            set { this.SetPropertyValue(value, MarkedSpan.PropertyNames.Type); }
        }

        /// <summary>
        /// Gets or sets the value for the associated annotation.
        /// </summary>
        [SchemaEntity(AttributeNames.Value, Requirement.Optional)]
        public string Value
        {
            get { return (string)this.GetPropertyValue(MarkedSpan.PropertyNames.Value); }
            set { this.SetPropertyValue(value, MarkedSpan.PropertyNames.Value); }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Gets the child <see cref="XliffElement"/>s contained within this object.
        /// </summary>
        /// <returns>A list of child elements stored as a pair consisting of the XLIFF Name for the child and
        /// the child itself.</returns>
        protected override List<ElementInfo> GetChildren()
        {
            List<ElementInfo> result;

            result = null;
            this.AddChildElementsToList(this.Text, ref result);

            return result;
        }

        /// <summary>
        /// Returns a value indicating whether the specified property is required to be output to the XLIFF file. This
        /// value is based on other property values.
        /// </summary>
        /// <param name="property">The property being inquired about whether it needs to be output.</param>
        /// <returns>True if the property needs to be written to the XLIFF file, false if the property output is
        /// optional.
        /// </returns>
        bool IOutputResolver.IsOutputRequired(string property)
        {
            if (property == AttributeNames.Translate)
            {
                return this.Type == MarkedSpanTypes.Generic;
            }

            return false;
        }

        /// <summary>
        /// Sets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="text">The text to set.</param>
        protected override void SetInnerText(string text)
        {
            this.Text.Add(new PlainText(text));
        }

        /// <summary>
        /// Stores the <see cref="XliffElement"/> as a child of this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>True if the child was stored, otherwise false.</returns>
        protected override bool StoreChild(ElementInfo child)
        {
            bool result;

            result = true;
            if (child.Element is ResourceStringContent)
            {
                this.Text.Add((ResourceStringContent)child.Element);
            }
            else
            {
                result = base.StoreChild(child);
            }

            return result;
        }
        #endregion Methods

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the EquivalentStorage property.
            /// </summary>
            public const string EquivalentStorage = "EquivalentStorage";

            /// <summary>
            /// The name of the <see cref="MarkedSpan.FormatStyle"/> property.
            /// </summary>
            public const string FormatStyle = "FormatStyle";

            /// <summary>
            /// The name of the <see cref="MarkedSpan.Id"/> property.
            /// </summary>
            public const string Id = "Id";

            /// <summary>
            /// The name of the <see cref="MarkedSpan.Reference"/> property.
            /// </summary>
            public const string Reference = "Reference";

            /// <summary>
            /// The name of the <see cref="MarkedSpan.SizeInfo"/> property.
            /// </summary>
            public const string SizeInfo = "SizeInfo";

            /// <summary>
            /// The name of the <see cref="MarkedSpan.SizeInfoReference"/> property.
            /// </summary>
            public const string SizeInfoReference = "SizeInfoReference";

            /// <summary>
            /// The name of the <see cref="MarkedSpan.SizeRestriction"/> property.
            /// </summary>
            public const string SizeRestriction = "SizeRestriction";

            /// <summary>
            /// The name of the <see cref="MarkedSpan.StorageRestriction"/> property.
            /// </summary>
            public const string StorageRestriction = "StorageRestriction";

            /// <summary>
            /// The name of the <see cref="MarkedSpan.SubFormatStyle"/>  property.
            /// </summary>
            public const string SubFormatStyle = "SubFormatStyle";

            /// <summary>
            /// The name of the <see cref="MarkedSpan.Translate"/> property.
            /// </summary>
            public const string Translate = "Translate";

            /// <summary>
            /// The name of the <see cref="MarkedSpan.Type"/> property.
            /// </summary>
            public const string Type = "Type";

            /// <summary>
            /// The name of the <see cref="MarkedSpan.Value"/> property.
            /// </summary>
            public const string Value = "Value";
        }
    }
}
