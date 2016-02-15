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
    /// This class represents a start marker of an annotation where the spanning marker cannot be used to
    /// indicate text is well-formed. This corresponds to a &lt;sm> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;sm id=string
    ///            [translate=(yes|no)]
    ///            [type=(generic|comment|term|string)]
    ///            [ref=string]
    ///            [value=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#sm">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="ResourceStringContent"/>
    /// <seealso cref="IExtensible"/>
    /// <seealso cref="IFormatStyleAttributes"/>
    /// <seealso cref="ISelectable"/>
    /// <seealso cref="ISizeRestrictionAttributes"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class MarkedSpanStart : ResourceStringContent,
                                   IExtensible,
                                   IFormatStyleAttributes,
                                   IOutputResolver,
                                   ISelectable,
                                   ISizeRestrictionAttributes
    {
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkedSpanStart"/> class.
        /// </summary>
        /// <param name="id">The Id of the marked span.</param>
        public MarkedSpanStart(string id)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.Id = id;
            this.SubFormatStyle = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            this.EnableAttribute(MarkedSpanStart.PropertyNames.EquivalentStorage, false);
            this.EnableAttribute(MarkedSpanStart.PropertyNames.SizeInfo, false);
            this.EnableAttribute(MarkedSpanStart.PropertyNames.SizeInfoReference, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkedSpanStart"/> class.
        /// </summary>
        internal MarkedSpanStart()
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
                                        MarkedSpanStart.PropertyNames.EquivalentStorage);
                throw new NotSupportedException(message);
            }

            set
            {
                string message;

                message = string.Format(
                                        Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                        MarkedSpanStart.PropertyNames.EquivalentStorage);
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
            get { return (FormatStyleValue?)this.GetPropertyValue(MarkedSpanStart.PropertyNames.FormatStyle); }
            set { this.SetPropertyValue(value, MarkedSpanStart.PropertyNames.FormatStyle); }
        }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        bool IExtensible.HasExtensions
        {
            get { return this.extensions.IsValueCreated && (this.extensions.Value.Count > 0); }
        }

        /// <summary>
        /// Gets or sets the Id of the start marker.
        /// </summary>
        [SchemaEntity(AttributeNames.Id, Requirement.Required)]
        public string Id
        {
            get
            {
                return (string)this.GetPropertyValue(MarkedSpanStart.PropertyNames.Id);
            }

            set
            {
                value = string.IsNullOrEmpty(value) ? null : value;
                ArgValidator.Create(value, MarkedSpanStart.PropertyNames.Id).IsValidId(this);
                this.SetPropertyValue(value, MarkedSpanStart.PropertyNames.Id);
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
            get { return (string)this.GetPropertyValue(MarkedSpanStart.PropertyNames.Reference); }
            set { this.SetPropertyValue(value, MarkedSpanStart.PropertyNames.Reference); }
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
                                        MarkedSpanStart.PropertyNames.SizeInfo);
                throw new NotSupportedException(message);
            }

            set
            {
                string message;

                message = string.Format(
                                        Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                        MarkedSpanStart.PropertyNames.SizeInfo);
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
                                        MarkedSpanStart.PropertyNames.SizeInfoReference);
                throw new NotSupportedException(message);
            }

            set
            {
                string message;

                message = string.Format(
                                        Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                        MarkedSpanStart.PropertyNames.SizeInfoReference);
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
            get { return (string)this.GetPropertyValue(MarkedSpanStart.PropertyNames.SizeRestriction); }
            set { this.SetPropertyValue(value, MarkedSpanStart.PropertyNames.SizeRestriction); }
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
            get { return (string)this.GetPropertyValue(MarkedSpanStart.PropertyNames.StorageRestriction); }
            set { this.SetPropertyValue(value, MarkedSpanStart.PropertyNames.StorageRestriction); }
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
            get { return (IDictionary<string, string>)this.GetPropertyValue(MarkedSpanStart.PropertyNames.SubFormatStyle); }
            private set { this.SetPropertyValue(value, MarkedSpanStart.PropertyNames.SubFormatStyle); }
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
            get { return (bool)this.GetPropertyValue(MarkedSpanStart.PropertyNames.Translate); }
            set { this.SetPropertyValue(value, MarkedSpanStart.PropertyNames.Translate); }
        }

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
            get { return (string)this.GetPropertyValue(MarkedSpanStart.PropertyNames.Type); }
            set { this.SetPropertyValue(value, MarkedSpanStart.PropertyNames.Type); }
        }

        /// <summary>
        /// Gets or sets the value for the associated annotation.
        /// </summary>
        [SchemaEntity(AttributeNames.Value, Requirement.Optional)]
        public string Value
        {
            get { return (string)this.GetPropertyValue(MarkedSpanStart.PropertyNames.Value); }
            set { this.SetPropertyValue(value, MarkedSpanStart.PropertyNames.Value); }
        }
        #endregion Properties

        #region ISelectable Implementation
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

                parent = this.Parent as ResourceString;
                if (parent != null)
                {
                    id = parent.ChildSelectorIdPrefix;
                    if (id != null)
                    {
                        id = Utilities.CreateSelectorId(id, this.Id);
                    }
                }
                else
                {
                    id = this.Id;
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
        #endregion ISelectable Implementation

        #region Methods
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
            /// The name of the <see cref="MarkedSpanStart.FormatStyle"/> property.
            /// </summary>
            public const string FormatStyle = "FormatStyle";

            /// <summary>
            /// The name of the <see cref="MarkedSpanStart.Id"/> property.
            /// </summary>
            public const string Id = "Id";

            /// <summary>
            /// The name of the <see cref="MarkedSpanStart.Reference"/> property.
            /// </summary>
            public const string Reference = "Reference";

            /// <summary>
            /// The name of the <see cref="MarkedSpanStart.SizeInfo"/> property.
            /// </summary>
            public const string SizeInfo = "SizeInfo";

            /// <summary>
            /// The name of the <see cref="MarkedSpanStart.SizeInfoReference"/> property.
            /// </summary>
            public const string SizeInfoReference = "SizeInfoReference";

            /// <summary>
            /// The name of the <see cref="MarkedSpanStart.SizeRestriction"/> property.
            /// </summary>
            public const string SizeRestriction = "SizeRestriction";

            /// <summary>
            /// The name of the <see cref="MarkedSpanStart.StorageRestriction"/> property.
            /// </summary>
            public const string StorageRestriction = "StorageRestriction";

            /// <summary>
            /// The name of the <see cref="MarkedSpanStart.SubFormatStyle"/> property.
            /// </summary>
            public const string SubFormatStyle = "SubFormatStyle";

            /// <summary>
            /// The name of the <see cref="MarkedSpanStart.Translate"/> property.
            /// </summary>
            public const string Translate = "Translate";

            /// <summary>
            /// The name of the <see cref="MarkedSpanStart.Type"/> property.
            /// </summary>
            public const string Type = "Type";

            /// <summary>
            /// The name of the <see cref="MarkedSpanStart.Value"/> property.
            /// </summary>
            public const string Value = "Value";
        }
    }
}
