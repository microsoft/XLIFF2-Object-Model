namespace Localization.Xliff.OM.Modules.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.ChangeTracking.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a container for specific revisions associated with a sibling element, or a child of a
    /// sibling element, to the change track module within the scope of the enclosing element. This corresponds to a
    /// &lt;revision> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;revision [author=string]
    ///                  [datetime=string]
    ///                  [version=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#revision">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    [SchemaChild(
                 NamespacePrefixes.ChangeTrackingModule,
                 NamespaceValues.ChangeTrackingModule,
                 ElementNames.Item,
                 typeof(Item))]
    [SuppressMessage(
                     "StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class Revision : XliffElement, IExtensible
    {
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Revision"/> class.
        /// </summary>
        public Revision()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.Items = new ParentAttachedList<Item>(this);
        }

        #region Properties
        /// <summary>
        /// Gets or sets the user or agent that created or modified the referenced element or its attributes.
        /// </summary>
        [SchemaEntity(AttributeNames.Author, Requirement.Optional)]
        public string Author
        {
            get { return (string)this.GetPropertyValue(Revision.PropertyNames.Author); }
            set { this.SetPropertyValue(value, Revision.PropertyNames.Author); }
        }

        /// <summary>
        /// Gets or sets the date and time the referenced element or its attributes were created or modified.
        /// </summary>
        [Converter(typeof(DateTimeConverter))]
        [SchemaEntity(AttributeNames.DateTime, Requirement.Optional)]
        public DateTime? ChangeDate
        {
            get { return (DateTime?)this.GetPropertyValue(Revision.PropertyNames.ChangeDate); }
            set { this.SetPropertyValue(value, Revision.PropertyNames.ChangeDate); }
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
            get { return base.HasChildren || (this.Items.Count > 0); }
        }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        bool IExtensible.HasExtensions
        {
            get { return this.extensions.IsValueCreated && (this.extensions.Value.Count > 0); }
        }

        /// <summary>
        /// Gets the list of revision items that changed.
        /// </summary>
        public IList<Item> Items { get; private set; }

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
        /// Gets or sets the version of the referenced element or its attributes that were created or modified.
        /// </summary>
        [SchemaEntity(AttributeNames.Version, Requirement.Optional)]
        public string Version
        {
            get { return (string)this.GetPropertyValue(Revision.PropertyNames.Version); }
            set { this.SetPropertyValue(value, Revision.PropertyNames.Version); }
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
            this.AddChildElementsToList(this.Items, ref result);

            return result;
        }

        /// <summary>
        /// Stores the <see cref="XliffElement"/> as a child of this <see cref="Revision"/>.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>True if the child was stored, otherwise false.</returns>
        protected override bool StoreChild(ElementInfo child)
        {
            bool result;

            result = true;
            if (child.Element is Item)
            {
                this.Items.Add((Item)child.Element);
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
            /// The name of the <see cref="Revision.Author"/> property.
            /// </summary>
            public const string Author = "Author";

            /// <summary>
            /// The name of the <see cref="Revision.ChangeDate"/> property.
            /// </summary>
            public const string ChangeDate = "ChangeDate";

            /// <summary>
            /// The name of the <see cref="Revision.Version"/> property.
            /// </summary>
            public const string Version = "Version";
        }
    }
}
