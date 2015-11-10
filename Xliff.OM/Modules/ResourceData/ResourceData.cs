namespace Localization.Xliff.OM.Modules.ResourceData
{
    using System;
    using System.Collections.Generic;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Modules.ResourceData.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a parent container for resource data associated with the enclosing element. This
    /// corresponds to a &lt;resourceData> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;resourceData>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#res_resourceData">XLIFF
    /// specification</a> for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    [SchemaChild(
                 NamespacePrefixes.ResourceDataModule,
                 NamespaceValues.ResourceDataModule,
                 ElementNames.ResourceItem,
                 typeof(ResourceItem))]
    [SchemaChild(
                 NamespacePrefixes.ResourceDataModule,
                 NamespaceValues.ResourceDataModule,
                 ElementNames.ResourceItemRef,
                 typeof(ResourceItemRef))]
    public class ResourceData : XliffElement
    {
        #region Member Variables
        /// <summary>
        /// The order in which to write data to a file so the output conforms to a defined schema.
        /// </summary>
        private static readonly IEnumerable<OutputItem> OutputOrderValues;
        #endregion Member Variables

        /// <summary>
        /// Initializes static members of the <see cref="ResourceData"/> class.
        /// </summary>
        static ResourceData()
        {
            ResourceData.OutputOrderValues = new[]
                {
                    new OutputItem(OutputItemType.Child, typeof(ResourceItemRef), 1),
                    new OutputItem(OutputItemType.Child, typeof(ResourceItem), 1)
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceData"/> class.
        /// </summary>
        public ResourceData()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.ResourceItemReferences = new ParentAttachedList<ResourceItemRef>(this);
            this.ResourceItems = new ParentAttachedList<ResourceItem>(this);
        }

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.ResourceItemReferences.Count > 0) || (this.ResourceItems.Count > 0); }
        }

        /// <summary>
        /// Gets the order in which to write data to a file so the output conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be output in order to be compliant. This method is
        /// used during serialization to ensure the elements and text are output in the order specified by that schema.
        /// </summary>
        protected override IEnumerable<OutputItem> OutputOrder
        {
            get { return ResourceData.OutputOrderValues; }
        }

        /// <summary>
        /// Gets the list of containers for specific resource data that is either intended for modification, or to be
        /// used as contextual reference during translation.
        /// </summary>
        public IList<ResourceItem> ResourceItems { get; private set; }

        /// <summary>
        /// Gets the list of references to an associated <see cref="ResourceItem"/> element located at the
        /// <see cref="Localization.Xliff.OM.Core.File"/>level.
        /// </summary>
        public IList<ResourceItemRef> ResourceItemReferences { get; private set; }
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
            this.AddChildElementsToList(this.ResourceItemReferences, ref result);
            this.AddChildElementsToList(this.ResourceItems, ref result);

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
            if (child.Element is ResourceItemRef)
            {
                this.ResourceItemReferences.Add((ResourceItemRef)child.Element);
            }
            else if (child.Element is ResourceItem)
            {
                this.ResourceItems.Add((ResourceItem)child.Element);
            }
            else
            {
                result = base.StoreChild(child);
            }

            return result;
        }
        #endregion Methods
    }
}
