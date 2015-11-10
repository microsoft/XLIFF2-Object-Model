namespace Localization.Xliff.OM.Modules.Glossary
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Modules.Glossary.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a container for a list of glossary terms. This corresponds to a &lt;gls:glossary> element
    /// in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;gls:glossary ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#glossary">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    [SchemaChild(
                 NamespacePrefixes.GlossaryModule,
                 NamespaceValues.GlossaryModule,
                 ElementNames.GlossaryEntry,
                 typeof(GlossaryEntry))]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class Glossary : XliffElement
    {
        /// <summary>
        /// The order in which to write data to a file so the output conforms to a defined schema.
        /// </summary>
        private static readonly IEnumerable<OutputItem> OutputOrderValues;

        /// <summary>
        /// Initializes static members of the <see cref="Glossary"/> class.
        /// </summary>
        static Glossary()
        {
            Glossary.OutputOrderValues = new[] 
                { 
                    new OutputItem(OutputItemType.Child, typeof(GlossaryEntry), 1)
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Glossary"/> class.
        /// </summary>
        public Glossary()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.Entries = new ParentAttachedList<GlossaryEntry>(this);
        }

        #region Properties
        /// <summary>
        /// Gets the list of glossary entries contained within this object.
        /// </summary>
        public IList<GlossaryEntry> Entries { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.Entries.Count > 0); }
        }

        /// <summary>
        /// Gets the order in which to write data to a file so the output conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be output in order to be compliant. This method is
        /// used during serialization to ensure the elements and text are output in the order specified by that schema.
        /// </summary>
        protected override IEnumerable<OutputItem> OutputOrder
        {
            get { return Glossary.OutputOrderValues; }
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
            this.AddChildElementsToList(this.Entries, ref result);

            return result;
        }

        /// <summary>
        /// Stores the defined element as a child of this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>True if the child was stored, otherwise false.</returns>
        protected override bool StoreChild(ElementInfo child)
        {
            bool result;

            result = true;
            if (child.Element is GlossaryEntry)
            {
                this.Entries.Add((GlossaryEntry)child.Element);
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
