namespace Localization.Xliff.OM.Core
{
    using System;
    using System.Collections.Generic;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class is a base class for XLIFF containers (namely <see cref="Ignorable"/> and <see cref="Segment"/>).
    /// </summary>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="ISelectable"/>
    /// <seealso cref="ISelectNavigable"/>
    [SchemaChild(NamespaceValues.Core, ElementNames.Source, typeof(Source))]
    [SchemaChild(NamespaceValues.Core, ElementNames.Target, typeof(Target))]
    public abstract class ContainerResource : XliffElement, IContainerResource, ISelectable, ISelectNavigable
    {
        #region Member Variables
        /// <summary>
        /// The order in which to write data to a file so the output conforms to a defined schema.
        /// </summary>
        private static readonly IEnumerable<OutputItem> OutputOrderValues;

        /// <summary>
        /// The source content to be translated.
        /// </summary>
        private Source source;

        /// <summary>
        /// The translated content.
        /// </summary>
        private Target target;
        #endregion Member Variables

        /// <summary>
        /// Initializes static members of the <see cref="ContainerResource"/> class.
        /// </summary>
        static ContainerResource()
        {
            ContainerResource.OutputOrderValues = new[] 
                { 
                    new OutputItem(OutputItemType.Child, typeof(Source), 1),
                    new OutputItem(OutputItemType.Child, typeof(Target), 2)
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerResource"/> class.
        /// </summary>
        /// <param name="id">The Id of the container.</param>
        protected ContainerResource(string id)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            if (id != null)
            {
                this.Id = id;
            }
        }

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.Source != null) || (this.Target != null); }
        }

        /// <summary>
        /// Gets or sets the Id of the container.
        /// </summary>
        [SchemaEntity(AttributeNames.Id, Requirement.Optional)]
        public string Id
        {
            get
            {
                return (string)this.GetPropertyValue(ContainerResource.PropertyNames.Id);
            }

            set
            {
                value = string.IsNullOrEmpty(value) ? null : value;
                ArgValidator.Create(value, ContainerResource.PropertyNames.Id).IsValidId(this);
                this.SetPropertyValue(value, ContainerResource.PropertyNames.Id);
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
            get { return ContainerResource.OutputOrderValues; }
        }

        /// <summary>
        /// Gets the selector Id of the item.
        /// </summary>
        /// <example>The format of the selector path for <see cref="ContainerResource"/>s doesn't follow the
        /// "prefix=value" format, and is instead just "value". For example, the value for a <see cref="Segment"/> item
        /// might look like "segment1" where "segment1" is the Id. </example>
        public string SelectorId
        {
            get { return this.Id; }
        }

        /// <summary>
        /// Gets the full path of the item from the root document.
        /// </summary>
        public string SelectorPath
        {
            get { return this.BuildSelectorPath(); }
        }

        /// <summary>
        /// Gets or sets the source content to be translated.
        /// </summary>
        public Source Source
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
        /// Gets or sets the translated content.
        /// </summary>
        public Target Target
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

            if (this.Source != null)
            {
                XmlNameInfo name;

                name = new XmlNameInfo(NamespaceValues.Core, ElementNames.Source);
                result.Add(new ElementInfo(name, this.Source));
            }

            if (this.Target != null)
            {
                XmlNameInfo name;

                name = new XmlNameInfo(NamespaceValues.Core, ElementNames.Target);
                result.Add(new ElementInfo(name, this.Target));
            }

            return result;
        }

        /// <summary>
        /// Selects an item matching the selection query.
        /// </summary>
        /// <param name="path">The selection query.</param>
        /// <returns>The object that was selected from the query path, or null if no match was found.</returns>
        /// <example>The value of <paramref name="path"/> might look something like "#g=group1/f=file1/u=unit1/n=note1"
        /// which is a relative path from the current object, not a full path from the document root.</example>
        public ISelectable Select(string path)
        {
            ArgValidator.Create(path, "path").IsNotNull().StartsWith(Utilities.Constants.SelectorPathIndictator);
            return this.SelectElement(Utilities.RemoveSelectorIndicator(path));
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
            if (child.Element is Source)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.Source, this.Source);
                this.Source = (Source)child.Element;
                Utilities.SetParent(this.Source, this);
            }
            else if (child.Element is Target)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.Target, this.Target);
                this.Target = (Target)child.Element;
                Utilities.SetParent(this.Target, this);
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
            /// The name of the Id property.
            /// </summary>
            public const string Id = "Id";

            /// <summary>
            /// The name of the <see cref="Source"/> property.
            /// </summary>
            public const string Source = "Source";

            /// <summary>
            /// The name of the <see cref="Target"/> property.
            /// </summary>
            public const string Target = "Target";
        }
    }
}