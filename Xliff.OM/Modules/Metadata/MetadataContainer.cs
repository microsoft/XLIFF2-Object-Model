namespace Localization.Xliff.OM.Modules.Metadata
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Modules.Metadata.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a container for metadata associated with the enclosing element. This corresponds to a
    /// &lt;mda:metadata> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;mda:metadata [id=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#metadata">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="ISelectable"/>
    [SchemaChild(NamespacePrefixes.MetadataModule, NamespaceValues.MetadataModule, ElementNames.Group, typeof(MetaGroup))]
    [SchemaChild(NamespacePrefixes.MetadataModule, NamespaceValues.MetadataModule, ElementNames.Meta, typeof(Meta))]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class MetadataContainer : XliffElement, ISelectable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataContainer"/> class.
        /// </summary>
        public MetadataContainer()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.Groups = new ParentAttachedList<MetaGroup>(this);
        }

        #region Properties
        /// <summary>
        /// Gets the list of groups for organizing metadata.
        /// </summary>
        public IList<MetaGroup> Groups { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || this.HasGroups; }
        }

        /// <summary>
        /// Gets a value indicating whether the container has groups.
        /// </summary>
        public bool HasGroups
        {
            get { return this.Groups.Count > 0; }
        }

        /// <summary>
        /// Gets or sets the Id of the container.
        /// </summary>
        [SchemaEntity(AttributeNames.Id, Requirement.Optional)]
        public string Id
        {
            get
            {
                return (string)this.GetPropertyValue(MetadataContainer.PropertyNames.Id);
            }

            set
            {
                value = string.IsNullOrEmpty(value) ? null : value;
                ArgValidator.Create(value, MetadataContainer.PropertyNames.Id).IsValidId(this);
                this.SetPropertyValue(value, MetadataContainer.PropertyNames.Id);
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
        /// Gets the selector Id of the item.
        /// </summary>
        /// <example>For a <see cref="MetadataContainer"/> item, this value might look like "mda=metadata1" where
        /// "metadata1" is the Id.
        /// </example>
        public string SelectorId
        {
            get { return Utilities.CreateSelectorId(MetadataContainer.Constants.SelectorPrefix, this.Id); }
        }

        /// <summary>
        /// Gets the full path of the item from the root document.
        /// </summary>
        public string SelectorPath
        {
            get { return this.BuildSelectorPath(); }
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

            result = base.GetChildren();
            this.AddChildElementsToList(this.Groups, ref result);

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
            if (child.Element is MetaGroup)
            {
                this.Groups.Add((MetaGroup)child.Element);
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
            public const string SelectorPrefix = "mda";
        }

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="MetadataContainer.Id"/> property.
            /// </summary>
            public const string Id = "Id";
        }
    }
}
