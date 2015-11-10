namespace Localization.Xliff.OM.Modules.ChangeTracking
{
    using System.Collections.Generic;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Modules.ChangeTracking.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a Parent container for change tracking information associated with a sibling element, or
    /// a child of a sibling element, to the change track module within the scope of the enclosing element. This
    /// corresponds to a &lt;changeTrack> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;changeTrack appliesTo=string
    ///                     [currentVersion=string]
    ///                     [ref=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#changeTrack">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    [SchemaChild(
                 NamespacePrefixes.ChangeTrackingModule,
                 NamespaceValues.ChangeTrackingModule,
                 ElementNames.Revisions,
                 typeof(RevisionsContainer))]
    public class ChangeTrack : XliffElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTrack"/> class.
        /// </summary>
        public ChangeTrack()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.Revisions = new ParentAttachedList<RevisionsContainer>(this);
        }

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.Revisions.Count > 0); }
        }

        /// <summary>
        /// Gets the list of change revisions.
        /// </summary>
        public IList<RevisionsContainer> Revisions { get; private set; }
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
        /// Stores the <see cref="XliffElement"/> as a child of this <see cref="ChangeTrack"/>.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>True if the child was stored, otherwise false.</returns>
        protected override bool StoreChild(ElementInfo child)
        {
            bool result;

            result = true;
            if (child.Element is RevisionsContainer)
            {
                this.Revisions.Add((RevisionsContainer)child.Element);
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
