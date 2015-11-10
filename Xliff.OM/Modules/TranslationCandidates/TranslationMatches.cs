namespace Localization.Xliff.OM.Modules.TranslationCandidates
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Modules.TranslationCandidates.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a collection of matches retrieved from any leveraging system (i.e. MT, TM, etc). This
    /// corresponds to a &lt;mtc:matches> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;mtc:matches ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#matches">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    [SchemaChild(NamespacePrefixes.TranslationCandidatesModule, NamespaceValues.TranslationCandidatesModule, ElementNames.Match, typeof(Match))]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class TranslationMatches : XliffElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationMatches"/> class.
        /// </summary>
        public TranslationMatches()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.Matches = new ParentAttachedList<Match>(this);
        }

        /// <summary>
        /// Gets the list of potential translation suggestions for a part of the source content.
        /// </summary>
        public IList<Match> Matches { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.Matches.Count > 0); }
        }

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
            this.AddChildElementsToList(this.Matches, ref result);

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
            if (child.Element is Match)
            {
                this.Matches.Add((Match)child.Element);
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
