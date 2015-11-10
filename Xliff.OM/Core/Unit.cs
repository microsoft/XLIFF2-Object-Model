namespace Localization.Xliff.OM.Core
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Modules.ChangeTracking;
    using Localization.Xliff.OM.Modules.Glossary;
    using Localization.Xliff.OM.Modules.Metadata;
    using Localization.Xliff.OM.Modules.ResourceData;
    using Localization.Xliff.OM.Modules.SizeRestriction;
    using Localization.Xliff.OM.Modules.TranslationCandidates;
    using Localization.Xliff.OM.Modules.Validation;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class is a container for containers that contain translatable text. This corresponds to a
    /// &lt;unit> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;unit id=string
    ///              [Name=string]
    ///              [canResegment=(yes|no)]
    ///              [translate=(yes|no)]
    ///              [srcDir=(ltr|rtl|auto)]
    ///              [trgDir=(ltr|rtl|auto)]
    ///              [type=string]
    ///              [xml:space=(default|preserve)] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#unit">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="TranslationContainer"/>
    [SchemaChild(
                 NamespacePrefixes.GlossaryModule,
                 NamespaceValues.GlossaryModule,
                 Modules.Glossary.XmlNames.ElementNames.Glossary,
                 typeof(Glossary))]
    [SchemaChild(NamespaceValues.Core, ElementNames.Ignorable, typeof(Ignorable))]
    [SchemaChild(NamespaceValues.Core, ElementNames.Notes, typeof(NoteContainer))]
    [SchemaChild(NamespaceValues.Core, ElementNames.OriginalData, typeof(OriginalData))]
    [SchemaChild(
                 NamespacePrefixes.ResourceDataModule,
                 NamespaceValues.ResourceDataModule,
                 Modules.ResourceData.XmlNames.ElementNames.ResourceData,
                 typeof(ResourceData))]
    [SchemaChild(NamespaceValues.Core, ElementNames.Segment, typeof(Segment))]
    [SchemaChild(
                 NamespacePrefixes.TranslationCandidatesModule,
                 NamespaceValues.TranslationCandidatesModule,
                 Modules.TranslationCandidates.XmlNames.ElementNames.Matches,
                 typeof(TranslationMatches))]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class Unit : TranslationContainer
    {
        #region Member Variables
        /// <summary>
        /// The order in which to write data to a file so the output conforms to a defined schema.
        /// </summary>
        private static readonly IEnumerable<OutputItem> OutputOrderValues;

        /// <summary>
        /// Indicates whether matches were already stored to avoid storing multiple instances. The matches container is
        /// not exposed directly so it starts out empty (not null).
        /// </summary>
        private bool storedMatches;

        /// <summary>
        /// The container for a list of glossary terms.
        /// </summary>
        private Glossary glossary;

        /// <summary>
        /// The original data for inline codes.
        /// </summary>
        private OriginalData originalData;

        /// <summary>
        /// The container for resource data associated with the enclosing element.
        /// </summary>
        private ResourceData resourceData;

        /// <summary>
        /// The collection of matches retrieved from any leveraging system (MT, TM, etc.).
        /// </summary>
        private TranslationMatches translationCandidates;
        #endregion Member Variables

        /// <summary>
        /// Initializes static members of the <see cref="Unit"/> class.
        /// </summary>
        static Unit()
        {
            Unit.OutputOrderValues = new[] 
                { 
                    new OutputItem(OutputItemType.Child, typeof(ChangeTrack), 1),
                    new OutputItem(OutputItemType.Child, typeof(Glossary), 1),
                    new OutputItem(OutputItemType.Child, typeof(MetadataContainer), 1),
                    new OutputItem(OutputItemType.Child, typeof(ProfileData), 1),
                    new OutputItem(OutputItemType.Child, typeof(ResourceData), 1),
                    new OutputItem(OutputItemType.Child, typeof(TranslationMatches), 1),
                    new OutputItem(OutputItemType.Child, typeof(Validation), 1),
                    new OutputItem(OutputItemType.Extension, null, 1),
                    new OutputItem(OutputItemType.Child, typeof(NoteContainer), 2),
                    new OutputItem(OutputItemType.Child, typeof(OriginalData), 3),
                    new OutputItem(OutputItemType.Child, typeof(ContainerResource), 4)
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Unit"/> class.
        /// </summary>
        /// <param name="id">The Id of the unit.</param>
        public Unit(string id)
            : this(id, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Unit"/> class.
        /// </summary>
        internal Unit()
            : this(null, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Unit"/> class.
        /// </summary>
        /// <param name="id">The Id of the unit.</param>
        /// <param name="internalCtor">True if this internal constructor was called that is used for deserialization,
        /// false if the public one was called.
        /// </param>
        private Unit(string id, bool internalCtor)
            : base(id, internalCtor)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.translationCandidates = new TranslationMatches();
            Utilities.SetParent(this.translationCandidates, this);

            // DEV NOTE: Order of segments must be maintained during serialization and deserialization so make sure
            // the order can be preserved. Using a type of list maintains order, but if you change this.Resources
            // to another type (like dictionary), make sure you have a way to maintain order.
            this.Resources = new ParentAttachedList<ContainerResource>(this);

            // The default .ctor is called during deserialization and that is the only case where matches can be overridden.
            this.storedMatches = !internalCtor;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the container for a list of glossary terms.
        /// </summary>
        public Glossary Glossary
        {
            get
            {
                return this.glossary;
            }

            set
            {
                ArgValidator.ParentIsNull(value);
                Utilities.SetParent(this.glossary, null);
                Utilities.SetParent(value, this);
                this.glossary = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get
            {
                return base.HasChildren ||
                       this.HasMatches ||
                       (this.OriginalData != null) ||
                       (this.ResourceData != null) ||
                       (this.Resources.Count > 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the object contains translation matches.
        /// </summary>
        public bool HasMatches
        {
            get { return this.translationCandidates.Matches.Count > 0; }
        }

        /// <summary>
        /// Gets the collection of matches retrieved from any leveraging system (MT, TM, etc.).
        /// </summary>
        public IList<Match> Matches
        {
            get { return this.translationCandidates.Matches; }
        }

        /// <summary>
        /// Gets or sets the original data for inline codes.
        /// </summary>
        public OriginalData OriginalData
        {
            get
            {
                return this.originalData;
            }

            set
            {
                ArgValidator.ParentIsNull(value);
                Utilities.SetParent(this.originalData, null);
                Utilities.SetParent(value, this);
                this.originalData = value;
            }
        }

        /// <summary>
        /// Gets the order in which to write data to a file so the output conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be output in order to be compliant. This method is
        /// used during serialization to ensure the elements and text are output in the order specified by that schema.
        /// </summary>
        protected override IEnumerable<OutputItem> OutputOrder
        {
            get { return Unit.OutputOrderValues; }
        }

        /// <summary>
        /// Gets or sets the container container for resource data associated with the enclosing Unit.
        /// </summary>
        public ResourceData ResourceData
        {
            get
            {
                return this.resourceData;
            }

            set
            {
                ArgValidator.ParentIsNull(value);
                Utilities.SetParent(this.resourceData, null);
                Utilities.SetParent(value, this);
                this.resourceData = value;
            }
        }

        /// <summary>
        /// Gets the list of containers that contain resources to translate.
        /// </summary>
        public IList<ContainerResource> Resources { get; private set; }

        /// <summary>
        /// Gets the selector Id of the item.
        /// </summary>
        /// <example>For a <see cref="Unit"/> item, this value might look like "u=unit1" where "unit1" is the Id.
        /// </example>
        public override string SelectorId
        {
            get { return Utilities.CreateSelectorId(Unit.Constants.SelectorPrefix, this.Id); }
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

            if (this.Glossary != null)
            {
                XmlNameInfo name;

                name = new XmlNameInfo(
                                       NamespacePrefixes.GlossaryModule,
                                       NamespaceValues.GlossaryModule,
                                       Modules.Glossary.XmlNames.ElementNames.Glossary);
                result.Add(new ElementInfo(name, this.Glossary));
            }

            if (this.HasMatches)
            {
                XmlNameInfo name;

                name = new XmlNameInfo(
                                       NamespacePrefixes.TranslationCandidatesModule,
                                       NamespaceValues.TranslationCandidatesModule,
                                       Modules.TranslationCandidates.XmlNames.ElementNames.Matches);
                result.Add(new ElementInfo(name, this.translationCandidates));
            }

            if (this.OriginalData != null)
            {
                ElementInfo child;
                XmlNameInfo name;

                name = new XmlNameInfo(NamespaceValues.Core, ElementNames.OriginalData);
                child = new ElementInfo(name, this.OriginalData);
                result.Add(child);
            }

            if (this.ResourceData != null)
            {
                XmlNameInfo name;

                name = new XmlNameInfo(
                                       NamespacePrefixes.ResourceDataModule,
                                       NamespaceValues.ResourceDataModule,
                                       Modules.ResourceData.XmlNames.ElementNames.ResourceData);
                result.Add(new ElementInfo(name, this.ResourceData));
            }

            // DEV NOTE: Order of segments must be maintained during serialization and deserialization so make sure
            // the order can be preserved. Using a type of list maintains order, but if you change this.Resources
            // to another type (like dictionary), make sure you have a way to maintain order.
            this.AddChildElementsToList(this.Resources, ref result);

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
            if (child.Element is ContainerResource)
            {
                // DEV NOTE: Order of segments must be maintained during serialization and deserialization so make sure
                // the order can be preserved. Using a type of list maintains order, but if you change this.Resources
                // to another type (like dictionary), make sure you have a way to maintain order.
                this.Resources.Add((ContainerResource)child.Element);
            }
            else if (child.Element is Glossary)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.Glossary, this.Glossary);
                this.Glossary = (Glossary)child.Element;
            }
            else if (child.Element is OriginalData)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.OriginalData, this.OriginalData);
                this.OriginalData = (OriginalData)child.Element;
            }
            else if (child.Element is TranslationMatches)
            {
                if (this.storedMatches)
                {
                    Utilities.ThrowIfPropertyNotNull(this, "Matches", this.translationCandidates);
                }

                this.translationCandidates = (TranslationMatches)child.Element;
                Utilities.SetParent(this.translationCandidates, this);
                this.storedMatches = true;
            }
            else if (child.Element is ResourceData)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.ResourceData, this.ResourceData);
                this.ResourceData = (ResourceData)child.Element;
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
            public const string SelectorPrefix = "u";
        }

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Unit.Glossary"/> property.
            /// </summary>
            public const string Glossary = "Glossary";

            /// <summary>
            /// The name of the <see cref="Unit.OriginalData"/> property.
            /// </summary>
            public const string OriginalData = "OriginalData";

            /// <summary>
            /// The name of the <see cref="Unit.ResourceData"/> property.
            /// </summary>
            public const string ResourceData = "ResourceData";
        }
    }
}