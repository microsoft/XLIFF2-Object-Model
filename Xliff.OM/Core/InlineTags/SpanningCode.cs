namespace Localization.Xliff.OM.Core
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a well-formed spanning original code. This corresponds to a &lt;pc> element
    /// in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;pc [canCopy=(yes|no)]
    ///            [canDelete=(yes|no)]
    ///            [canOverlap=(yes|no)]
    ///            [canReorder=(yes|no)]
    ///            [copyOf=string]
    ///            [dispEnd=string]
    ///            [dispStart=string]
    ///            [equivEnd=string]
    ///            [equivStart=string]
    ///            id=string
    ///            [dataRefEnd=string]
    ///            [dataRefStart=string]
    ///            [subFlowsEnd=string]
    ///            [subFlowsStart=string]
    ///            [subType=string]
    ///            [type=(fmt|ui|quote|link|image|other)]
    ///            [dir=(ltr|rtl|auto)] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#pc">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="CodeBase"/>
    /// <seealso cref="IResourceStringContentContainer"/>
    [SchemaChild(NamespaceValues.Core, ElementNames.CDataTag, typeof(CDataTag))]
    [SchemaChild(NamespaceValues.Core, ElementNames.CodePoint, typeof(CodePoint))]
    [SchemaChild(NamespaceValues.Core, ElementNames.CommentTag, typeof(CommentTag))]
    [SchemaChild(NamespaceValues.Core, ElementNames.MarkedSpan, typeof(MarkedSpan))]
    [SchemaChild(NamespaceValues.Core, ElementNames.MarkedSpanEnd, typeof(MarkedSpanEnd))]
    [SchemaChild(NamespaceValues.Core, ElementNames.MarkedSpanStart, typeof(MarkedSpanStart))]
    [SchemaChild(NamespaceValues.Core, ElementNames.PlainText, typeof(PlainText))]
    [SchemaChild(NamespaceValues.Core, ElementNames.ProcessingInstructionTag, typeof(ProcessingInstructionTag))]
    [SchemaChild(NamespaceValues.Core, ElementNames.SpanningCode, typeof(SpanningCode))]
    [SchemaChild(NamespaceValues.Core, ElementNames.SpanningCodeEnd, typeof(SpanningCodeEnd))]
    [SchemaChild(NamespaceValues.Core, ElementNames.SpanningCodeStart, typeof(SpanningCodeStart))]
    [SchemaChild(NamespaceValues.Core, ElementNames.StandaloneCode, typeof(StandaloneCode))]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class SpanningCode : CodeBase, IInheritanceInfoProvider, IResourceStringContentContainer
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SpanningCode"/> class.
        /// </summary>
        /// <param name="id">The Id of the marked span.</param>
        public SpanningCode(string id)
            : base(id)
        {
            this.Text = new ParentAttachedList<ResourceStringContent>(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanningCode"/> class.
        /// </summary>
        /// <param name="id">The Id of the marked span.</param>
        /// <param name="text">Plain text to add.</param>
        public SpanningCode(string id, string text)
            : this(id)
        {
            this.Text.Add(new PlainText(text));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanningCode"/> class.
        /// </summary>
        internal SpanningCode()
            : this(null)
        {
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the collection of references to <see cref="Data"/> elements. The key is a name associated with the
        /// reference, and the value is the actual reference.
        /// </summary>
        public override IDictionary<string, string> AllDataReferences
        {
            get
            {
                return new Dictionary<string, string>()
                        {
                            { SpanningCode.PropertyNames.DataReferenceEnd, this.DataReferenceEnd },
                            { SpanningCode.PropertyNames.DataReferenceStart, this.DataReferenceStart }
                        };
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the spanning code where this attribute is used can enclose
        /// partial spanning codes (ex. a start code without its corresponding end code, or an end code without
        /// its corresponding start code).
        /// </summary>
        [Converter(typeof(BoolConverter))]
        [DefaultValue(false)]
        [SchemaEntity(AttributeNames.CanOverlap, Requirement.Optional)]
        public bool CanOverlap
        {
            get { return (bool)this.GetPropertyValue(SpanningCode.PropertyNames.CanOverlap); }
            set { this.SetPropertyValue(value, SpanningCode.PropertyNames.CanOverlap); }
        }

        /// <summary>
        /// Gets or sets the Id of the <see cref="Data"/> element that contains the original data for the end
        /// marker for a given inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.DataReferenceEnd, Requirement.Optional)]
        public string DataReferenceEnd
        {
            get { return (string)this.GetPropertyValue(SpanningCode.PropertyNames.DataReferenceEnd); }
            set { this.SetPropertyValue(value, SpanningCode.PropertyNames.DataReferenceEnd); }
        }

        /// <summary>
        /// Gets or sets the Id of the <see cref="Data"/> element that contains the original data for the start
        /// marker of a given inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.DataReferenceStart, Requirement.Optional)]
        public string DataReferenceStart
        {
            get { return (string)this.GetPropertyValue(SpanningCode.PropertyNames.DataReferenceStart); }
            set { this.SetPropertyValue(value, SpanningCode.PropertyNames.DataReferenceStart); }
        }

        /// <summary>
        /// Gets or sets a value indicating the directionality of the content.
        /// </summary>
        [Converter(typeof(ContentDirectionalityConverter))]
        [DefaultValue(ContentDirectionality.Auto)]
        [InheritValue(Inheritance.Callback)]
        [SchemaEntity(AttributeNames.Directionality, Requirement.Optional)]
        public ContentDirectionality Directionality
        {
            get { return (ContentDirectionality)this.GetPropertyValue(SpanningCode.PropertyNames.Directionality); }
            set { this.SetPropertyValue(value, SpanningCode.PropertyNames.Directionality); }
        }

        /// <summary>
        /// Gets or sets an alternative user-friendly display representation of the original data of the end
        /// marker of an inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.DisplayTextEnd, Requirement.Optional)]
        public string DisplayTextEnd
        {
            get { return (string)this.GetPropertyValue(SpanningCode.PropertyNames.DisplayTextEnd); }
            set { this.SetPropertyValue(value, SpanningCode.PropertyNames.DisplayTextEnd); }
        }

        /// <summary>
        /// Gets or sets an alternative user-friendly display representation of the original data of the start
        /// marker of an inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.DisplayTextStart, Requirement.Optional)]
        public string DisplayTextStart
        {
            get { return (string)this.GetPropertyValue(SpanningCode.PropertyNames.DisplayTextStart); }
            set { this.SetPropertyValue(value, SpanningCode.PropertyNames.DisplayTextStart); }
        }

        /// <summary>
        /// Gets or sets a plain text representation of the original data of the end marker of an inline code
        /// that can be used when generating a plain text representation of the content.
        /// </summary>
        [DefaultValue("")]
        [SchemaEntity(AttributeNames.EquivalentTextEnd, Requirement.Optional)]
        public string EquivalentTextEnd
        {
            get { return (string)this.GetPropertyValue(SpanningCode.PropertyNames.EquivalentTextEnd); }
            set { this.SetPropertyValue(value, SpanningCode.PropertyNames.EquivalentTextEnd); }
        }

        /// <summary>
        /// Gets or sets a plain text representation of the original data of the start marker of an inline code
        /// that can be used when generating a plain text representation of the content.
        /// </summary>
        [DefaultValue("")]
        [SchemaEntity(AttributeNames.EquivalentTextStart, Requirement.Optional)]
        public string EquivalentTextStart
        {
            get { return (string)this.GetPropertyValue(SpanningCode.PropertyNames.EquivalentTextStart); }
            set { this.SetPropertyValue(value, SpanningCode.PropertyNames.EquivalentTextStart); }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.Text.Count > 0); }
        }

        /// <summary>
        /// Gets a value indicating whether the element contains subflow values.
        /// </summary>
        public override bool HasSubFlows
        {
            get { return !string.IsNullOrWhiteSpace(this.SubFlowsEnd) || !string.IsNullOrWhiteSpace(this.SubFlowsStart); }
        }

        /// <summary>
        /// Gets or sets a list of Ids corresponding to the <see cref="Unit"/> elements that contain the sub-flows
        /// for the end marker of a given inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.SubFlowsEnd, Requirement.Optional)]
        public string SubFlowsEnd
        {
            get { return (string)this.GetPropertyValue(SpanningCode.PropertyNames.SubFlowsEnd); }
            set { this.SetPropertyValue(value, SpanningCode.PropertyNames.SubFlowsEnd); }
        }

        /// <summary>
        /// Gets or sets a list of Ids corresponding to the <see cref="Unit"/> elements that contain the sub-flows
        /// for the start marker of a given inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.SubFlowsStart, Requirement.Optional)]
        public string SubFlowsStart
        {
            get { return (string)this.GetPropertyValue(SpanningCode.PropertyNames.SubFlowsStart); }
            set { this.SetPropertyValue(value, SpanningCode.PropertyNames.SubFlowsStart); }
        }

        /// <summary>
        /// Gets a value indicating whether the object has references to <see cref="Data"/> elements.
        /// </summary>
        public override bool SupportsDataReferences
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the elements that describe the text.
        /// </summary>
        public IList<ResourceStringContent> Text { get; private set; }
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
            this.AddChildElementsToList(this.Text, ref result);

            return result;
        }

        /// <summary>
        /// Method called to provide custom inheritance information. This is typically used when the inheritance
        /// depends on runtime information.
        /// </summary>
        /// <param name="property">The name of the property being retrieved.</param>
        /// <returns>The value of the property.</returns>
        InheritanceInfo IInheritanceInfoProvider.GetInheritanceInfo(string property)
        {
            Debug.Assert(property == "Directionality", "Unsupported property");
            return new InheritanceInfo(Utilities.SpanningCodeInheritanceHandler);
        }

        /// <summary>
        /// Sets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="text">The text to set.</param>
        protected override void SetInnerText(string text)
        {
            this.Text.Add(new PlainText(text));
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
            if (child.Element is ResourceStringContent)
            {
                this.Text.Add((ResourceStringContent)child.Element);
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
            /// The name of the <see cref="SpanningCode.CanOverlap"/> property.
            /// </summary>
            public const string CanOverlap = "CanOverlap";

            /// <summary>
            /// The name of the <see cref="SpanningCode.DataReferenceEnd"/> property.
            /// </summary>
            public const string DataReferenceEnd = "DataReferenceEnd";

            /// <summary>
            /// The name of the <see cref="SpanningCode.DataReferenceStart"/> property.
            /// </summary>
            public const string DataReferenceStart = "DataReferenceStart";

            /// <summary>
            /// The name of the <see cref="SpanningCode.Directionality"/> property.
            /// </summary>
            public const string Directionality = "Directionality";

            /// <summary>
            /// The name of the <see cref="SpanningCode.DisplayTextEnd"/> property.
            /// </summary>
            public const string DisplayTextEnd = "DisplayTextEnd";

            /// <summary>
            /// The name of the <see cref="SpanningCode.DisplayTextStart"/> property.
            /// </summary>
            public const string DisplayTextStart = "DisplayTextStart";

            /// <summary>
            /// The name of the <see cref="SpanningCode.EquivalentTextEnd"/> property.
            /// </summary>
            public const string EquivalentTextEnd = "EquivalentTextEnd";

            /// <summary>
            /// The name of the <see cref="SpanningCode.EquivalentTextStart"/> property.
            /// </summary>
            public const string EquivalentTextStart = "EquivalentTextStart";

            /// <summary>
            /// The name of the <see cref="SpanningCode.SubFlowsEnd"/> property.
            /// </summary>
            public const string SubFlowsEnd = "SubFlowsEnd";

            /// <summary>
            /// The name of the <see cref="SpanningCode.SubFlowsStart"/> property.
            /// </summary>
            public const string SubFlowsStart = "SubFlowsStart";
        }
    }
}
