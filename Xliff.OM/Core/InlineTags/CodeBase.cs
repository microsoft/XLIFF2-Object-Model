namespace Localization.Xliff.OM.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
    /// This class is a base class for inline tags that represent original codes in the source content.
    /// </summary>
    /// <seealso cref="ResourceStringContent"/>
    /// <seealso cref="IFormatStyleAttributes"/>
    /// <seealso cref="ISelectable"/>
    /// <seealso cref="ISizeRestrictionAttributes"/>
    public abstract class CodeBase : ResourceStringContent,
                                     IFormatStyleAttributes,
                                     ISelectable,
                                     ISizeRestrictionAttributes
    {
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeBase"/> class.
        /// </summary>
        /// <param name="id">The Id of the span.</param>
        protected CodeBase(string id)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.Id = id;
            this.SubFormatStyle = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the inline code can be copied.
        /// </summary>
        [Converter(typeof(BoolConverter))]
        [DefaultValue(true)]
        [SchemaEntity(AttributeNames.CanCopy, Requirement.Optional)]
        public bool CanCopy
        {
            get { return (bool)this.GetPropertyValue(CodeBase.PropertyNames.CanCopy); }
            set { this.SetPropertyValue(value, CodeBase.PropertyNames.CanCopy); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the inline code can be deleted.
        /// </summary>
        [Converter(typeof(BoolConverter))]
        [DefaultValue(true)]
        [SchemaEntity(AttributeNames.CanDelete, Requirement.Optional)]
        public bool CanDelete
        {
            get { return (bool)this.GetPropertyValue(CodeBase.PropertyNames.CanDelete); }
            set { this.SetPropertyValue(value, CodeBase.PropertyNames.CanDelete); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the inline code can be re-ordered.
        /// </summary>
        [Converter(typeof(CanReorderValueConverter))]
        [DefaultValue(CanReorderValue.Yes)]
        [SchemaEntity(AttributeNames.CanReorder, Requirement.Optional)]
        public CanReorderValue CanReorder
        {
            get { return (CanReorderValue)this.GetPropertyValue(CodeBase.PropertyNames.CanReorder); }
            set { this.SetPropertyValue(value, CodeBase.PropertyNames.CanReorder); }
        }

        /// <summary>
        /// Gets or sets the Id of the base code of a copied code.
        /// </summary>
        [SchemaEntity(AttributeNames.CopyOf, Requirement.Optional)]
        public string CopyOf
        {
            get { return (string)this.GetPropertyValue(CodeBase.PropertyNames.CopyOf); }
            set { this.SetPropertyValue(value, CodeBase.PropertyNames.CopyOf); }
        }

        /// <summary>
        /// Gets or sets a means to specify how much storage space an inline element will use in the native format.
        /// This size contribution is then added to the size contributed by the textual parts. Interpretation of the
        /// value is dependent on the selected <see cref="Profiles.StorageProfile"/>. It must represent the equivalent
        /// storage size represented by the inline element.
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
                if (!this.SupportsEquivalentStorage)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                            CodeBase.PropertyNames.EquivalentStorage);
                    throw new NotSupportedException(message);
                }

                return (string)this.GetPropertyValue(CodeBase.PropertyNames.EquivalentStorage);
            }

            set
            {
                if (!this.SupportsEquivalentStorage)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                            CodeBase.PropertyNames.EquivalentStorage);
                    throw new NotSupportedException(message);
                }

                this.SetPropertyValue(value, CodeBase.PropertyNames.EquivalentStorage);
            }
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
            get { return (FormatStyleValue?)this.GetPropertyValue(CodeBase.PropertyNames.FormatStyle); }
            set { this.SetPropertyValue(value, CodeBase.PropertyNames.FormatStyle); }
        }

        /// <summary>
        /// Gets a value indicating whether the element contains sub-flow values.
        /// </summary>
        public abstract bool HasSubFlows { get; }

        /// <summary>
        /// Gets or sets the Id of the inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.Id, Requirement.Required)]
        public virtual string Id
        {
            get
            {
                return (string)this.GetPropertyValue(CodeBase.PropertyNames.Id);
            }

            set
            {
                value = string.IsNullOrEmpty(value) ? null : value;
                ArgValidator.Create(value, CodeBase.PropertyNames.Id).IsValidId(this);
                this.SetPropertyValue(value, CodeBase.PropertyNames.Id);
            }
        }

        /// <summary>
        /// Gets or sets profile specific information to inline elements so that size information can be decoupled from
        /// the native format or represented when the native data is not available in the XLIFF document.
        /// Interpretation of the value is dependent on selected <see cref="Profiles.GeneralProfile"/>. It must
        /// represent information related to how the element it is attached to contributes to the size of the text or
        /// entity in which it occurs or represents.
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
                if (!this.SupportsSizeInfo)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                            CodeBase.PropertyNames.SizeInfo);
                    throw new NotSupportedException(message);
                }

                return (string)this.GetPropertyValue(CodeBase.PropertyNames.SizeInfo);
            }

            set
            {
                if (!this.SupportsSizeInfo)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                            CodeBase.PropertyNames.SizeInfo);
                    throw new NotSupportedException(message);
                }

                this.SetPropertyValue(value, CodeBase.PropertyNames.SizeInfo);
            }
        }

        /// <summary>
        /// Gets or sets a reference to data that provide the same information that could be otherwise put in a sizeInfo
        /// attribute. The reference must point to an element in a <see cref="ProfileData"/> element that is a sibling
        /// to the element this attribute is attached to or a sibling to one of its ancestors. 
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
                if (!this.SupportsSizeInfoReference)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                            CodeBase.PropertyNames.SizeInfoReference);
                    throw new NotSupportedException(message);
                }

                return (string)this.GetPropertyValue(CodeBase.PropertyNames.SizeInfoReference);
            }

            set
            {
                if (!this.SupportsSizeInfoReference)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                            CodeBase.PropertyNames.SizeInfoReference);
                    throw new NotSupportedException(message);
                }

                this.SetPropertyValue(value, CodeBase.PropertyNames.SizeInfoReference);
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
            get
            {
                if (!this.SupportsSizeRestriction)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                            CodeBase.PropertyNames.SizeRestriction);
                    throw new NotSupportedException(message);
                }

                return (string)this.GetPropertyValue(CodeBase.PropertyNames.SizeRestriction);
            }

            set
            {
                if (!this.SupportsSizeRestriction)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                            CodeBase.PropertyNames.SizeRestriction);
                    throw new NotSupportedException(message);
                }

                this.SetPropertyValue(value, CodeBase.PropertyNames.SizeRestriction);
            }
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
            get
            {
                if (!this.SupportsStorageRestriction)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                            CodeBase.PropertyNames.StorageRestriction);
                    throw new NotSupportedException(message);
                }

                return (string)this.GetPropertyValue(CodeBase.PropertyNames.StorageRestriction);
            }

            set
            {
                if (!this.SupportsStorageRestriction)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.XliffElement_PropertyNotSupported_Format,
                                            CodeBase.PropertyNames.StorageRestriction);
                    throw new NotSupportedException(message);
                }

                this.SetPropertyValue(value, CodeBase.PropertyNames.StorageRestriction);
            }
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
            get { return (IDictionary<string, string>)this.GetPropertyValue(CodeBase.PropertyNames.SubFormatStyle); }
            private set { this.SetPropertyValue(value, CodeBase.PropertyNames.SubFormatStyle); }
        }

        /// <summary>
        /// Gets or sets the secondary level type of an inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.SubType, Requirement.Optional)]
        public string SubType
        {
            get { return (string)this.GetPropertyValue(CodeBase.PropertyNames.SubType); }
            set { this.SetPropertyValue(value, CodeBase.PropertyNames.SubType); }
        }

        /// <summary>
        /// Gets a value indicating whether the object supports the EquivalentStorage property.
        /// </summary>
        public virtual bool SupportsEquivalentStorage
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the object supports the SizeInfo property.
        /// </summary>
        public virtual bool SupportsSizeInfo
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the object supports the SizeInfoReference property.
        /// </summary>
        public virtual bool SupportsSizeInfoReference
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the object supports the SizeRestriction property.
        /// </summary>
        public virtual bool SupportsSizeRestriction
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the object supports the StorageRestriction property.
        /// </summary>
        public virtual bool SupportsStorageRestriction
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the type of an element.
        /// </summary>
        [Converter(typeof(CodeTypeConverter))]
        [ExplicitOutputDependency(CodeBase.PropertyNames.SubType)]
        [SchemaEntity(AttributeNames.Type, Requirement.Optional)]
        public CodeType? Type
        {
            get { return (CodeType?)this.GetPropertyValue(CodeBase.PropertyNames.Type); }
            set { this.SetPropertyValue(value, CodeBase.PropertyNames.Type); }
        }
        #endregion Properties

        #region ISelectable Implementation
        /// <summary>
        /// Gets a value indicating whether the element represents a leaf fragment in a selector path. If so, the
        /// selector path shouldn't contain any other fragments after this fragment.
        /// </summary>
        public bool IsLeafFragment
        { 
            get { return true; }
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

                parent = this.Parent as ResourceString;
                if (parent != null)
                {
                    id = parent.ChildSelectorIdPrefix;
                    if (id != null)
                    {
                        id = Utilities.CreateSelectorId(id, this.Id);
                    }
                    else
                    {
                        id = this.Id;
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

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the CanCopy property.
            /// </summary>
            public const string CanCopy = "CanCopy";

            /// <summary>
            /// The name of the CanDelete property.
            /// </summary>
            public const string CanDelete = "CanDelete";

            /// <summary>
            /// The name of the CanReorder property.
            /// </summary>
            public const string CanReorder = "CanReorder";

            /// <summary>
            /// The name of the CopyOf property.
            /// </summary>
            public const string CopyOf = "CopyOf";

            /// <summary>
            /// The name of the EquivalentStorage property.
            /// </summary>
            public const string EquivalentStorage = "EquivalentStorage";

            /// <summary>
            /// The name of the FormatStyle property.
            /// </summary>
            public const string FormatStyle = "FormatStyle";

            /// <summary>
            /// The name of the Id property.
            /// </summary>
            public const string Id = "Id";

            /// <summary>
            /// The name of the SizeInfo property.
            /// </summary>
            public const string SizeInfo = "SizeInfo";

            /// <summary>
            /// The name of the SizeInfoReference property.
            /// </summary>
            public const string SizeInfoReference = "SizeInfoReference";

            /// <summary>
            /// The name of the SizeRestriction property.
            /// </summary>
            public const string SizeRestriction = "SizeRestriction";

            /// <summary>
            /// The name of the StorageRestriction property.
            /// </summary>
            public const string StorageRestriction = "StorageRestriction";

            /// <summary>
            /// The name of the SubFormatStyle property.
            /// </summary>
            public const string SubFormatStyle = "SubFormatStyle";

            /// <summary>
            /// The name of the SubType property.
            /// </summary>
            public const string SubType = "SubType";

            /// <summary>
            /// The name of the Type property.
            /// </summary>
            public const string Type = "Type";
        }
    }
}
