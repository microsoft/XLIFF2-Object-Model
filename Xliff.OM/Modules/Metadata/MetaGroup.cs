namespace Localization.Xliff.OM.Modules.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Modules.Metadata.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a way to organize metadata into a structured hierarchy. This corresponds to a
    /// &lt;mda:metaGroup> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;mda:metaGroup [id=string]
    ///                       [category=string]
    ///                       [appliesTo=(source|target|ignorable)] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#metagroup">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="MetaElement"/>
    /// <seealso cref="ISelectable"/>
    [SchemaChild(NamespacePrefixes.MetadataModule, NamespaceValues.MetadataModule, ElementNames.Group, typeof(MetaGroup))]
    [SchemaChild(NamespacePrefixes.MetadataModule, NamespaceValues.MetadataModule, ElementNames.Meta, typeof(Meta))]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class MetaGroup : MetaElement, ISelectable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaGroup"/> class.
        /// </summary>
        public MetaGroup()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.Containers = new ParentAttachedList<MetaElement>(this);
        }

        #region Properties
        /// <summary>
        /// Gets or sets the element to which the content of the <see cref="MetaGroup"/> applies.
        /// </summary>
        [Converter(typeof(MetaGroupSubjectConverter))]
        [SchemaEntity(AttributeNames.AppliesTo, Requirement.Optional)]
        public MetaGroupSubject? AppliesTo
        {
            get { return (MetaGroupSubject?)this.GetPropertyValue(MetaGroup.PropertyNames.AppliesTo); }
            set { this.SetPropertyValue(value, MetaGroup.PropertyNames.AppliesTo); }
        }

        /// <summary>
        /// Gets or sets the category for metadata contained in the enclosing <see cref="MetaGroup"/> element.
        /// </summary>
        [SchemaEntity(AttributeNames.Category, Requirement.Optional)]
        public string Category
        {
            get { return (string)this.GetPropertyValue(MetaGroup.PropertyNames.Category); }
            set { this.SetPropertyValue(value, MetaGroup.PropertyNames.Category); }
        }

        /// <summary>
        /// Gets the elements used to organize metadata.
        /// </summary>
        public IList<MetaElement> Containers { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.Containers.Count > 0); }
        }

        /// <summary>
        /// Gets or sets the Id group.
        /// </summary>
        [SchemaEntity(AttributeNames.Id, Requirement.Optional)]
        public string Id
        {
            get
            {
                return (string)this.GetPropertyValue(MetaGroup.PropertyNames.Id);
            }

            set
            {
                value = string.IsNullOrEmpty(value) ? null : value;
                ArgValidator.Create(value, MetaGroup.PropertyNames.Id).IsValidId(this);
                this.SetPropertyValue(value, MetaGroup.PropertyNames.Id);
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
        /// <example>For a <see cref="MetaGroup"/> item, this value might look like "mda=group1" where "group1" is the Id.
        /// </example>
        public string SelectorId
        {
            get { return Utilities.CreateSelectorId(MetaGroup.Constants.SelectorPrefix, this.Id); }
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
            this.AddChildElementsToList(this.Containers, ref result);

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
            if (child.Element is MetaElement)
            {
                this.Containers.Add((MetaElement)child.Element);
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
            /// The name of the <see cref="MetaGroup.AppliesTo"/> property.
            /// </summary>
            public const string AppliesTo = "AppliesTo";

            /// <summary>
            /// The name of the <see cref="MetaGroup.Category"/> property.
            /// </summary>
            public const string Category = "Category";

            /// <summary>
            /// The name of the <see cref="MetaGroup.Id"/> property.
            /// </summary>
            public const string Id = "Id";
        }
    }
}
