namespace Localization.Xliff.OM.Modules.TranslationCandidates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.Metadata;
    using Localization.Xliff.OM.Modules.TranslationCandidates.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a potential translation suggested for a part of the <see cref="Source"/> content of
    /// an enclosing <see cref="Unit"/> element. This corresponds to a &lt;mtc:match> element in the XLIFF 2.0
    /// specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;mtc:match [id=string]
    ///                   [matchQuality=float]
    ///                   [matchSuitability=float]
    ///                   [origin=string]
    ///                   [ref=string]
    ///                   [reference=bool]
    ///                   [similarity=float]
    ///                   [subType=string]
    ///                   [type=(am|mt|icm|idm|tb|tm|other)] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#match">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    /// <seealso cref="IMetadataStorage"/>
    /// <seealso cref="ISelectable"/>
    [SchemaChild(NamespaceValues.Core, ElementNames.OriginalData, typeof(OriginalData))]
    [SchemaChild(NamespaceValues.Core, ElementNames.Source, typeof(Source))]
    [SchemaChild(NamespaceValues.Core, ElementNames.Target, typeof(Target))]
    [SchemaChild(
                 NamespacePrefixes.MetadataModule,
                 NamespaceValues.MetadataModule,
                 ElementNames.Metadata,
                 typeof(MetadataContainer))]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class Match : XliffElement, IContainerResource, IExtensible, IMetadataStorage, ISelectable
    {
        #region Member Variables
        /// <summary>
        /// The order in which to write data to a file so the output conforms to a defined schema.
        /// </summary>
        private static readonly IEnumerable<OutputItem> OutputOrderValues;

        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// The container for metadata associated with the enclosing element.
        /// </summary>
        private MetadataContainer metadata;

        /// <summary>
        /// The original data for inline codes.
        /// </summary>
        private OriginalData originalData;

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
        /// Initializes static members of the <see cref="Match"/> class.
        /// </summary>
        static Match()
        {
            Match.OutputOrderValues = new[] 
                { 
                    new OutputItem(OutputItemType.Child, typeof(MetadataContainer), 1),
                    new OutputItem(OutputItemType.Child, typeof(OriginalData), 2),
                    new OutputItem(OutputItemType.Child, typeof(Source), 3),
                    new OutputItem(OutputItemType.Child, typeof(Target), 4),
                    new OutputItem(OutputItemType.Extension, null, 5)
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Match"/> class.
        /// </summary>
        /// <param name="sourceReference">The reference to to a span of source text within the same <see cref="Unit"/>,
        /// to which the translation candidate is relevant.</param>
        public Match(string sourceReference)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.SourceReference = sourceReference;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Match"/> class.
        /// </summary>
        internal Match()
            : this(null)
        {
        }

        #region Properties
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
            get
            {
                return base.HasChildren ||
                       (this.Metadata != null) ||
                       (this.OriginalData != null) ||
                       (this.Source != null) ||
                       (this.target != null);
            }
        }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        bool IExtensible.HasExtensions
        {
            get { return this.extensions.IsValueCreated && (this.extensions.Value.Count > 0); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Target"/> child of the <see cref="Match"/> element
        /// contains a translation into a reference language rather than into the target language. For example,
        /// a German translation can be used as reference by a Luxembourgish translator.
        /// </summary>
        [Converter(typeof(BoolConverter))]
        [DefaultValue(false)]
        [SchemaEntity(AttributeNames.Reference, Requirement.Required)]
        public bool HasReferenceTranslation
        {
            get { return (bool)this.GetPropertyValue(Match.PropertyNames.HasReferenceTranslation); }
            set { this.SetPropertyValue(value, Match.PropertyNames.HasReferenceTranslation); }
        }

        /// <summary>
        /// Gets or sets the Id of element.
        /// </summary>
        [SchemaEntity(AttributeNames.Id, Requirement.Optional)]
        public string Id
        {
            get
            {
                return (string)this.GetPropertyValue(Match.PropertyNames.Id);
            }

            set
            {
                value = string.IsNullOrEmpty(value) ? null : value;
                ArgValidator.Create(value, Match.PropertyNames.Id).IsValidId(this);
                this.SetPropertyValue(value, Match.PropertyNames.Id);
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
        /// Gets or sets a value indicating the quality of the <see cref="Target"/> child of a <see cref="Match"/>
        /// element based on an external benchmark or metric.
        /// </summary>
        [Converter(typeof(FloatConverter))]
        [SchemaEntity(AttributeNames.MatchQuality, Requirement.Optional)]
        public float? MatchQuality
        {
            get { return (float?)this.GetPropertyValue(Match.PropertyNames.MatchQuality); }
            set { this.SetPropertyValue(value, Match.PropertyNames.MatchQuality); }
        }

        /// <summary>
        /// Gets or sets a value indicating the general suitability and relevance of its <see cref="Match"/> element
        /// based on various external benchmarks or metrics pertaining to both the <see cref="Source"/> and the
        /// <see cref="Target"/> children of the <see cref="Match"/>.
        /// </summary>
        [Converter(typeof(FloatConverter))]
        [SchemaEntity(AttributeNames.MatchSuitability, Requirement.Optional)]
        public float? MatchSuitability
        {
            get { return (float?)this.GetPropertyValue(Match.PropertyNames.MatchSuitability); }
            set { this.SetPropertyValue(value, Match.PropertyNames.MatchSuitability); }
        }

        /// <summary>
        /// Gets or sets the container for metadata associated with the enclosing <see cref="Match"/>.
        /// </summary>
        public MetadataContainer Metadata
        {
            get
            {
                return this.metadata;
            }

            set
            {
                ArgValidator.ParentIsNull(value);
                Utilities.SetParent(this.metadata, null);
                Utilities.SetParent(value, this);
                this.metadata = value;
            }
        }

        /// <summary>
        /// Gets or sets the Name of a tool, system, or repository that generated a <see cref="Match"/> element.
        /// </summary>
        [SchemaEntity(AttributeNames.Origin, Requirement.Optional)]
        public string Origin
        {
            get { return (string)this.GetPropertyValue(Match.PropertyNames.Origin); }
            set { this.SetPropertyValue(value, Match.PropertyNames.Origin); }
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
            get { return Match.OutputOrderValues; }
        }

        /// <summary>
        /// Gets the selector Id of the item.
        /// </summary>
        /// <example>For a <see cref="Match"/> item, this value might look like "mtc=match1" where "match1" is the Id.
        /// </example>
        public string SelectorId
        {
            get { return Utilities.CreateSelectorId(Match.Constants.SelectorPrefix, this.Id); }
        }

        /// <summary>
        /// Gets the full path of the item from the root document.
        /// </summary>
        public string SelectorPath
        {
            get { return this.BuildSelectorPath(); }
        }

        /// <summary>
        /// Gets or sets a value indicating the similarity level between the content of the <see cref="Source"/>
        /// child of a <see cref="Match"/> element and the translatable text being matched.
        /// </summary>
        [Converter(typeof(FloatConverter))]
        [SchemaEntity(AttributeNames.Similarity, Requirement.Optional)]
        public float? Similarity
        {
            get { return (float?)this.GetPropertyValue(Match.PropertyNames.Similarity); }
            set { this.SetPropertyValue(value, Match.PropertyNames.Similarity); }
        }

        /// <summary>
        /// Gets or sets the portion of the text to be translated.
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
        /// Gets or sets the reference to to a span of source text within the same <see cref="Unit"/>, to which
        /// the translation candidate is relevant.
        /// </summary>
        [SchemaEntity(AttributeNames.SourceReference, Requirement.Required)]
        public string SourceReference
        {
            get { return (string)this.GetPropertyValue(Match.PropertyNames.SourceReference); }
            set { this.SetPropertyValue(value, Match.PropertyNames.SourceReference); }
        }

        /// <summary>
        /// Gets or sets the secondary level type of a <see cref="Match"/> element. This property is set in conjunction
        /// with Type.
        /// </summary>
        [SchemaEntity(AttributeNames.SubType, Requirement.Optional)]
        public string SubType
        {
            get { return (string)this.GetPropertyValue(Match.PropertyNames.SubType); }
            set { this.SetPropertyValue(value, Match.PropertyNames.SubType); }
        }

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
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the translation of the sibling <see cref="Source"/> element.
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

        /// <summary>
        /// Gets or sets the type of a <see cref="Match"/> element, it gives the value providing additional
        /// information on how the match was generated or qualifying further the relevance of the match. The list
        /// of pre-defined values is general and user-specific information can be added using the SubType property.
        /// </summary>
        [Converter(typeof(MatchTypeConverter))]
        [DefaultValue(MatchType.TranslationMemory)]
        [ExplicitOutputDependency(Match.PropertyNames.SubType)]
        [SchemaEntity(AttributeNames.Type, Requirement.Optional)]
        public MatchType Type
        {
            get { return (MatchType)this.GetPropertyValue(Match.PropertyNames.Type); }
            set { this.SetPropertyValue(value, Match.PropertyNames.Type); }
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

            if (this.Metadata != null)
            {
                XmlNameInfo name;

                name = new XmlNameInfo(
                                       NamespacePrefixes.MetadataModule,
                                       NamespaceValues.MetadataModule,
                                       ElementNames.Metadata);
                result.Add(new ElementInfo(name, this.Metadata));
            }

            if (this.OriginalData != null)
            {
                ElementInfo child;
                XmlNameInfo name;

                name = new XmlNameInfo(NamespaceValues.Core, ElementNames.OriginalData);
                child = new ElementInfo(name, this.OriginalData);
                result.Add(child);
            }

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
        /// Stores the <see cref="XliffElement"/> as a child of this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>True if the child was stored, otherwise false.</returns>
        protected override bool StoreChild(ElementInfo child)
        {
            bool result;

            result = true;
            if (child.Element is MetadataContainer)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.Metadata, this.Metadata);
                this.Metadata = (MetadataContainer)child.Element;
            }
            else if (child.Element is OriginalData)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.OriginalData, this.OriginalData);
                this.OriginalData = (OriginalData)child.Element;
            }
            else if (child.Element is Source)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.Source, this.Source);
                this.Source = (Source)child.Element;
            }
            else if (child.Element is Target)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.Target, this.Target);
                this.Target = (Target)child.Element;
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
            public const string SelectorPrefix = "mtc";
        }

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Match.HasReferenceTranslation"/> property.
            /// </summary>
            public const string HasReferenceTranslation = "HasReferenceTranslation";

            /// <summary>
            /// The name of the <see cref="Match.Id"/> property.
            /// </summary>
            public const string Id = "Id";

            /// <summary>
            /// The name of the <see cref="Match.MatchQuality"/> property.
            /// </summary>
            public const string MatchQuality = "MatchQuality";

            /// <summary>
            /// The name of the <see cref="Match.MatchSuitability"/> property.
            /// </summary>
            public const string MatchSuitability = "MatchSuitability";

            /// <summary>
            /// The name of the <see cref="Match.Metadata"/> property.
            /// </summary>
            public const string Metadata = "Metadata";

            /// <summary>
            /// The name of the <see cref="Match.Origin"/> property.
            /// </summary>
            public const string Origin = "Origin";

            /// <summary>
            /// The name of the <see cref="Match.OriginalData"/> property.
            /// </summary>
            public const string OriginalData = "OriginalData";

            /// <summary>
            /// The name of the <see cref="Match.Similarity"/> property.
            /// </summary>
            public const string Similarity = "Similarity";

            /// <summary>
            /// The name of the <see cref="Match.Source"/> property.
            /// </summary>
            public const string Source = "Source";

            /// <summary>
            /// The name of the <see cref="Match.SourceReference"/> property.
            /// </summary>
            public const string SourceReference = "SourceReference";

            /// <summary>
            /// The name of the <see cref="Match.SubType"/> property.
            /// </summary>
            public const string SubType = "SubType";

            /// <summary>
            /// The name of the <see cref="Match.Target"/> property.
            /// </summary>
            public const string Target = "Target";

            /// <summary>
            /// The name of the <see cref="Match.Type"/> property.
            /// </summary>
            public const string Type = "Type";
        }
    }
}
