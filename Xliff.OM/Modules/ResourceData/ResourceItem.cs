namespace Localization.Xliff.OM.Modules.ResourceData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.ResourceData.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a container for specific resource data that is either intended for modification, or to be
    /// used as contextual reference during translation. This corresponds to a &lt;resourceItem> element in the XLIFF
    /// 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;resourceItem [id=string]
    ///                      [context=(yes|no)]
    ///                      [mimeType=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#res_resourceItem">XLIFF
    /// specification</a> for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    /// <seealso cref="ISelectable"/>
    [SchemaChild(
                 NamespacePrefixes.ResourceDataModule,
                 NamespaceValues.ResourceDataModule,
                 ElementNames.Source,
                 typeof(ResourceItemSource))]
    [SchemaChild(
                 NamespacePrefixes.ResourceDataModule,
                 NamespaceValues.ResourceDataModule,
                 ElementNames.Target,
                 typeof(ResourceItemTarget))]
    [SchemaChild(
                 NamespacePrefixes.ResourceDataModule,
                 NamespaceValues.ResourceDataModule,
                 ElementNames.Reference,
                 typeof(Reference))]
    public class ResourceItem : XliffElement, IExtensible, ISelectable
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

        /// <summary>
        /// The reference to the actual resource data that is either intended for modification, or to be used as
        /// contextual reference during translation.
        /// </summary>
        private ResourceItemSource source;

        /// <summary>
        /// The reference to the localized counterpart of the sibling Source element.
        /// </summary>
        private ResourceItemTarget target;
        #endregion Member Variables

        /// <summary>
        /// Initializes static members of the <see cref="ResourceItem"/> class.
        /// </summary>
        static ResourceItem()
        {
            ResourceItem.OutputOrderValues = new[]
                {
                    new OutputItem(OutputItemType.Child, typeof(ResourceItemSource), 1),
                    new OutputItem(OutputItemType.Child, typeof(ResourceItemTarget), 2),
                    new OutputItem(OutputItemType.Child, typeof(Reference), 3),
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceItem"/> class.
        /// </summary>
        public ResourceItem()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.References = new ParentAttachedList<Reference>(this);
        }

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether an external resource is to be used for context only and not
        /// modified (true).
        /// </summary>
        [Converter(typeof(BoolConverter))]
        [DefaultValue(true)]
        [SchemaEntity(AttributeNames.Context, Requirement.Optional)]
        public bool Context
        {
            get { return (bool)this.GetPropertyValue(ResourceItem.PropertyNames.Context); }
            set { this.SetPropertyValue(value, ResourceItem.PropertyNames.Context); }
        }

        /// <summary>
        /// Gets the list of registered extensions on the object.
        /// </summary>
        IList<IExtension> IExtensible.Extensions
        {
            get { return this.extensions.Value; }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get
            {
                return base.HasChildren ||
                       (this.References.Count > 0) ||
                       (this.Source != null) ||
                       (this.Target != null);
            }
        }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        bool IExtensible.HasExtensions
        {
            get { return this.extensions.IsValueCreated && (this.extensions.Value.Count > 0); }
        }

        /// <summary>
        /// Gets or sets the Id of the resource item.
        /// </summary>
        [SchemaEntity(AttributeNames.Id, Requirement.Optional)]
        public string Id
        {
            get { return (string)this.GetPropertyValue(ResourceItem.PropertyNames.Id); }
            set { this.SetPropertyValue(value, ResourceItem.PropertyNames.Id); }
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
        /// Gets or sets the MIME type that indicates the type of a resource object. This generally corresponds to the
        /// content type of <a href="http://tools.ietf.org/rfc/rfc2045.txt">[RFC 2045]</a>, the MIME specification.
        /// </summary>
        [SchemaEntity(AttributeNames.MimeType, Requirement.Optional)]
        public string MimeType
        {
            get { return (string)this.GetPropertyValue(ResourceItem.PropertyNames.MimeType); }
            set { this.SetPropertyValue(value, ResourceItem.PropertyNames.MimeType); }
        }

        /// <summary>
        /// Gets the order in which to write data to a file so the output conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be output in order to be compliant. This method is
        /// used during serialization to ensure the elements and text are output in the order specified by that schema.
        /// </summary>
        protected override IEnumerable<OutputItem> OutputOrder
        {
            get { return ResourceItem.OutputOrderValues; }
        }

        /// <summary>
        /// Gets the selector Id of the item.
        /// </summary>
        /// <example>For a <see cref="ResourceItem"/> item, this value might look like "res=item1" where "item1" is the Id.
        /// </example>
        public string SelectorId
        {
            get { return Utilities.CreateSelectorId(ResourceItem.Constants.SelectorPrefix, this.Id); }
        }

        /// <summary>
        /// Gets the full path of the item from the root document.
        /// </summary>
        public string SelectorPath
        {
            get { return this.BuildSelectorPath(); }
        }

        /// <summary>
        /// Gets or sets a reference to the actual resource data that is either intended for modification, or to be
        /// used as contextual reference during translation.
        /// </summary>
        public ResourceItemSource Source
        {
            get
            {
                return this.source;
            }

            set
            {
                ArgValidator.ParentIsNull(value);
                Utilities.SetParent(this.source, null);
                Utilities.SetParent(value, this);
                this.source = value;
            }
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
        /// Gets or sets a reference to the localized counterpart of the sibling Source element.
        /// </summary>
        public ResourceItemTarget Target
        {
            get
            {
                return this.target;
            }

            set
            {
                ArgValidator.ParentIsNull(value);
                Utilities.SetParent(this.target, null);
                Utilities.SetParent(value, this);
                this.target = value;
            }
        }

        /// <summary>
        /// Gets a list of references to contextual data relating to the sibling Source and Target elements, such as a
        /// German screenshot for a Luxembourgish translator.
        /// </summary>
        public IList<Reference> References { get; private set; }
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

            result = new List<ElementInfo>();
            this.AddChildElementsToList(this.References, ref result);

            if (this.Source != null)
            {
                XmlNameInfo name;

                name = new XmlNameInfo(NamespaceValues.ResourceDataModule, ElementNames.Source);
                result.Add(new ElementInfo(name, this.Source));
            }

            if (this.Target != null)
            {
                XmlNameInfo name;

                name = new XmlNameInfo(NamespaceValues.ResourceDataModule, ElementNames.Target);
                result.Add(new ElementInfo(name, this.Target));
            }

            return result;
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
            if (child.Element is ResourceItemSource)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.Source, this.Source);
                this.Source = (ResourceItemSource)child.Element;
                Utilities.SetParent(this.Source, this);
            }
            else if (child.Element is ResourceItemTarget)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.Target, this.Target);
                this.Target = (ResourceItemTarget)child.Element;
                Utilities.SetParent(this.Target, this);
            }
            else if (child.Element is Reference)
            {
                this.References.Add((Reference)child.Element);
            }
            else
            {
                result = base.StoreChild(child);
            }

            return result;
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
            public const string SelectorPrefix = "res";
        }

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="ResourceItem.Context"/> property.
            /// </summary>
            public const string Context = "Context";

            /// <summary>
            /// The name of the <see cref="ResourceItem.Id"/> property.
            /// </summary>
            public const string Id = "Id";

            /// <summary>
            /// The name of the <see cref="ResourceItem.MimeType"/> property.
            /// </summary>
            public const string MimeType = "MimeType";

            /// <summary>
            /// The name of the <see cref="ResourceItem.Source"/> property.
            /// </summary>
            public const string Source = "Source";

            /// <summary>
            /// The name of the <see cref="ResourceItem.Target"/> property.
            /// </summary>
            public const string Target = "Target";
        }
    }
}
