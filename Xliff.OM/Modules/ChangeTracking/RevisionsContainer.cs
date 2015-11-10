namespace Localization.Xliff.OM.Modules.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.ChangeTracking.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a container for logical groups of revisions associated with a sibling element, or a child
    /// of a sibling element, to the change track module within the scope of the enclosing element. This corresponds to
    /// a &lt;revisions> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;revisions appliesTo=string
    ///                   [ref=string]
    ///                   [currentVersion=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#revisions">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    [SchemaChild(
                 NamespacePrefixes.ChangeTrackingModule,
                 NamespaceValues.ChangeTrackingModule,
                 ElementNames.Revision,
                 typeof(Revision))]
    public class RevisionsContainer : XliffElement, IExtensible
    {
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RevisionsContainer"/> class.
        /// </summary>
        /// <param name="appliesTo">The specific XLIFF element which is a sibling, or a child of a sibling element, to
        /// the change track module within the scope of the enclosing element.</param>
        public RevisionsContainer(string appliesTo)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.AppliesTo = appliesTo;
            this.Revisions = new ParentAttachedList<Revision>(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RevisionsContainer"/> class.
        /// </summary>
        internal RevisionsContainer()
            : this(null)
        {
        }

        #region Properties
        /// <summary>
        /// Gets or sets the specific XLIFF element which is a sibling, or a child of a sibling element, to the change
        /// track module within the scope of the enclosing element.
        /// </summary>
        [SchemaEntity(AttributeNames.AppliesTo, Requirement.Required)]
        public string AppliesTo
        {
            get { return (string)this.GetPropertyValue(RevisionsContainer.PropertyNames.AppliesTo); }
            set { this.SetPropertyValue(value, RevisionsContainer.PropertyNames.AppliesTo); }
        }

        /// <summary>
        /// Gets or sets a reference to the most current version of a revision.
        /// </summary>
        [SchemaEntity(AttributeNames.CurrentVersion, Requirement.Optional)]
        public string CurrentVersion
        {
            get { return (string)this.GetPropertyValue(RevisionsContainer.PropertyNames.CurrentVersion); }
            set { this.SetPropertyValue(value, RevisionsContainer.PropertyNames.CurrentVersion); }
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
            get { return base.HasChildren || (this.Revisions.Count > 0); }
        }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        bool IExtensible.HasExtensions
        {
            get { return this.extensions.IsValueCreated && (this.extensions.Value.Count > 0); }
        }

        /// <summary>
        /// Gets or sets a reference to a single instance of an element that has multiple instances within the
        /// enclosing element.
        /// </summary>
        [SchemaEntity(AttributeNames.ReferenceAbbreviated, Requirement.Optional)]
        public string Reference
        {
            get { return (string)this.GetPropertyValue(RevisionsContainer.PropertyNames.Reference); }
            set { this.SetPropertyValue(value, RevisionsContainer.PropertyNames.Reference); }
        }

        /// <summary>
        /// Gets the list of revisions grouped by this container.
        /// </summary>
        public IList<Revision> Revisions { get; private set; }

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
            this.AddChildElementsToList(this.Revisions, ref result);

            return result;
        }

        /// <summary>
        /// Stores the <see cref="XliffElement"/> as a child of this <see cref="RevisionsContainer"/>.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>True if the child was stored, otherwise false.</returns>
        protected override bool StoreChild(ElementInfo child)
        {
            bool result;

            result = true;
            if (child.Element is Revision)
            {
                this.Revisions.Add((Revision)child.Element);
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
            /// The name of the <see cref="RevisionsContainer.AppliesTo"/> property.
            /// </summary>
            public const string AppliesTo = "AppliesTo";

            /// <summary>
            /// The name of the <see cref="RevisionsContainer.CurrentVersion"/> property.
            /// </summary>
            public const string CurrentVersion = "CurrentVersion";

            /// <summary>
            /// The name of the <see cref="RevisionsContainer.Reference"/> property.
            /// </summary>
            public const string Reference = "Reference";
        }
    }
}
