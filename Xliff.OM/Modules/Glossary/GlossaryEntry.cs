namespace Localization.Xliff.OM.Modules.Glossary
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.Glossary.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents glossary entry. This corresponds to a &lt;gls:glossEntry> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;gls:glossEntry [id=string]
    ///                        [ref=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#glossentry">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    /// <seealso cref="ISelectable"/>
    [SchemaChild(
                 NamespacePrefixes.GlossaryModule,
                 NamespaceValues.GlossaryModule,
                 Modules.Glossary.XmlNames.ElementNames.Definition,
                 typeof(Definition))]
    [SchemaChild(
                 NamespacePrefixes.GlossaryModule,
                 NamespaceValues.GlossaryModule,
                 Modules.Glossary.XmlNames.ElementNames.Term,
                 typeof(Term))]
    [SchemaChild(
                 NamespacePrefixes.GlossaryModule,
                 NamespaceValues.GlossaryModule,
                 Modules.Glossary.XmlNames.ElementNames.Translation,
                 typeof(Translation))]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class GlossaryEntry : XliffElement, IExtensible, ISelectable
    {
        #region Member Variables
        /// <summary>
        /// The order in which to write data to a file so the output conforms to a defined schema.
        /// </summary>
        private static readonly IEnumerable<OutputItem> OutputOrderValues;

        /// <summary>
        /// Indicates whether a term was already stored to avoid storing multiple instances. The term is required so
        /// a default instance is created and can be overridden during deserialization.
        /// </summary>
        private bool storedTerm;

        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// Definition in plain text for the term.
        /// </summary>
        private Definition definition;

        /// <summary>
        /// A term in the glossary, expressed in the source language of the enclosing xliff element.
        /// </summary>
        private Term term;
        #endregion Member Variables

        /// <summary>
        /// Initializes static members of the <see cref="GlossaryEntry"/> class.
        /// </summary>
        static GlossaryEntry()
        {
            GlossaryEntry.OutputOrderValues = new[] 
                { 
                    new OutputItem(OutputItemType.Child, typeof(Term), 1),
                    new OutputItem(OutputItemType.Child, typeof(Translation), 2),
                    new OutputItem(OutputItemType.Child, typeof(Definition), 3),
                    new OutputItem(OutputItemType.Extension, null, 4)
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlossaryEntry"/> class.
        /// </summary>
        public GlossaryEntry()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlossaryEntry"/> class.
        /// </summary>
        /// <param name="id">The Id of the entry.</param>
        public GlossaryEntry(string id)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.Id = id;
            this.Term = new Term();
            this.Translations = new ParentAttachedList<Translation>(this);

            // The default .ctor is called during deserialization and that is the only case where term can be overridden.
            this.storedTerm = id != null;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the definition in plain text for the term stored in the sibling term element.
        /// </summary>
        public Definition Definition
        {
            get
            {
                return this.definition;
            }

            set
            {
                ArgValidator.ParentIsNull(value);
                Utilities.SetParent(this.definition, null);
                Utilities.SetParent(value, this);
                this.definition = value;
            }
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
            get
            {
                return base.HasChildren ||
                       (this.Term != null) ||
                       (this.Translations.Count > 0) ||
                       (this.Definition != null);
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
        /// Gets or sets the Id of the entry.
        /// </summary>
        [SchemaEntity(AttributeNames.Id, Requirement.Optional)]
        public string Id
        {
            get
            {
                return (string)this.GetPropertyValue(GlossaryEntry.PropertyNames.Id);
            }

            set
            {
                value = string.IsNullOrEmpty(value) ? null : value;
                ArgValidator.Create(value, GlossaryEntry.PropertyNames.Id).IsValidId(this);
                this.SetPropertyValue(value, GlossaryEntry.PropertyNames.Id);
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
            get { return GlossaryEntry.OutputOrderValues; }
        }

        /// <summary>
        /// Gets or sets reference that points to a span of source or target text within the same unit, to which the
        /// glossary entry is relevant.
        /// </summary>
        [SchemaEntity(AttributeNames.ReferenceAbbreviated, Requirement.Optional)]
        public string Reference
        {
            get { return (string)this.GetPropertyValue(GlossaryEntry.PropertyNames.Reference); }
            set { this.SetPropertyValue(value, GlossaryEntry.PropertyNames.Reference); }
        }

        /// <summary>
        /// Gets the selector Id of the item.
        /// </summary>
        /// <example>For a <see cref="GlossaryEntry"/> item, this value might look like "gls=entry1" where "entry1" is the Id.
        /// </example>
        public string SelectorId
        {
            get { return Utilities.CreateSelectorId(GlossaryEntry.Constants.SelectorPrefix, this.Id); }
        }

        /// <summary>
        /// Gets the full path of the item from the root document.
        /// </summary>
        public string SelectorPath
        {
            get { return this.BuildSelectorPath(); }
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
        /// Gets the term in the glossary, expressed in the source language of the enclosing xliff element.
        /// </summary>
        public Term Term
        {
            get
            {
                return this.term;
            }

            private set
            {
                ArgValidator.ParentIsNull(value);
                Utilities.SetParent(this.term, null);
                Utilities.SetParent(value, this);
                this.term = value;
            }
        }

        /// <summary>
        /// Gets the list of translations for the sibling term element expressed in the target language of the enclosing
        /// xliff element. Multiple translations can be specified as synonyms.
        /// </summary>
        public IList<Translation> Translations { get; private set; }
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

            if (this.Term != null)
            {
                XmlNameInfo name;

                name = new XmlNameInfo(
                                       NamespacePrefixes.GlossaryModule,
                                       NamespaceValues.GlossaryModule,
                                       Modules.Glossary.XmlNames.ElementNames.Term);
                result.Add(new ElementInfo(name, this.Term));
            }

            this.AddChildElementsToList(this.Translations, ref result);

            if (this.Definition != null)
            {
                XmlNameInfo name;

                name = new XmlNameInfo(
                                       NamespacePrefixes.GlossaryModule,
                                       NamespaceValues.GlossaryModule,
                                       Modules.Glossary.XmlNames.ElementNames.Definition);
                result.Add(new ElementInfo(name, this.Definition));
            }

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
            if (child.Element is Definition)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.Definition, this.Definition);
                this.Definition = (Definition)child.Element;
            }
            else if (child.Element is Term)
            {
                if (this.storedTerm)
                {
                    Utilities.ThrowIfPropertyNotNull(this, PropertyNames.Term, this.Term);
                }

                this.Term = (Term)child.Element;
            }
            else if (child.Element is Translation)
            {
                this.Translations.Add((Translation)child.Element);
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
            public const string SelectorPrefix = "gls";
        }

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="GlossaryEntry.Definition"/> property.
            /// </summary>
            public const string Definition = "Definition";

            /// <summary>
            /// The name of the <see cref="GlossaryEntry.Id"/> property.
            /// </summary>
            public const string Id = "Id";

            /// <summary>
            /// The name of the <see cref="GlossaryEntry.Reference"/> property.
            /// </summary>
            public const string Reference = "Reference";

            /// <summary>
            /// The name of the <see cref="GlossaryEntry.Term"/> property.
            /// </summary>
            public const string Term = "Term";
        }
    }
}
