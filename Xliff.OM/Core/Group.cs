namespace Localization.Xliff.OM.Core
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Modules.ChangeTracking;
    using Localization.Xliff.OM.Modules.Metadata;
    using Localization.Xliff.OM.Modules.SizeRestriction;
    using Localization.Xliff.OM.Modules.Validation;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a grouping of content within an XLIFF document. This corresponds to a &lt;group>
    /// element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;group id=string
    ///               [Name=string]
    ///               [canResegment=(yes|no)]
    ///               [translate=(yes|no)]
    ///               [srcDir=(ltr|rtl|auto)]
    ///               [trgDir=(ltr|rtl|auto)]
    ///               [type=string]
    ///               [xml:space=(default|preserve)] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#group">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="TranslationContainer"/>
    [SchemaChild(NamespaceValues.Core, ElementNames.Group, typeof(Group))]
    [SchemaChild(NamespaceValues.Core, ElementNames.Notes, typeof(NoteContainer))]
    [SchemaChild(NamespaceValues.Core, ElementNames.Unit, typeof(Unit))]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class Group : TranslationContainer
    {
        /// <summary>
        /// The order in which to write data to a file so the output conforms to a defined schema.
        /// </summary>
        private static readonly IEnumerable<OutputItem> OutputOrderValues;

        /// <summary>
        /// Initializes static members of the <see cref="Group"/> class.
        /// </summary>
        static Group()
        {
            Group.OutputOrderValues = new[] 
                { 
                    new OutputItem(OutputItemType.Child, typeof(ChangeTrack), 1),
                    new OutputItem(OutputItemType.Child, typeof(MetadataContainer), 1),
                    new OutputItem(OutputItemType.Child, typeof(ProfileData), 1),
                    new OutputItem(OutputItemType.Child, typeof(Validation), 1),
                    new OutputItem(OutputItemType.Extension, null, 1),
                    new OutputItem(OutputItemType.Child, typeof(NoteContainer), 2),
                    new OutputItem(OutputItemType.Child, typeof(TranslationContainer), 3),
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        /// <param name="id">The Id of the group.</param>
        public Group(string id)
            : this(id, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        internal Group()
            : this(null, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        /// <param name="id">The Id of the group.</param>
        /// <param name="internalCtor">True if this internal constructor was called that is used for deserialization,
        /// false if the public one was called.
        /// </param>
        private Group(string id, bool internalCtor)
            : base(id, internalCtor)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.Containers = new ParentAttachedList<TranslationContainer>(this);
        }

        #region Properties
        /// <summary>
        /// Gets the list of containers contained within this object.
        /// </summary>
        public IList<TranslationContainer> Containers { get; private set; }

        /// <summary>
        /// Gets the selector Id of the item.
        /// </summary>
        /// <example>For a <see cref="Group"/> item, this value might look like "g=group1" where "group1" is the Id.
        /// </example>
        public override string SelectorId
        {
            get { return Utilities.CreateSelectorId(Group.Constants.SelectorPrefix, this.Id); }
        }

        /// <summary>
        /// Gets the order in which to write data to a file so the output conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be output in order to be compliant. This method is
        /// used during serialization to ensure the elements and text are output in the order specified by that schema.
        /// </summary>
        protected override IEnumerable<OutputItem> OutputOrder
        {
            get { return Group.OutputOrderValues; }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.Containers.Count > 0); }
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
            if (child.Element is TranslationContainer)
            {
                this.Containers.Add((TranslationContainer)child.Element);
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
            public const string SelectorPrefix = "g";
        }
    }
}