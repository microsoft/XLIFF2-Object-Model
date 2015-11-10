namespace Localization.Xliff.OM.Core
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Core.XmlNames;

    /// <summary>
    /// This class represents a start of a spanning original code. This corresponds to a &lt;sc> element in the XLIFF
    /// 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;sc [canCopy=(yes|no)]
    ///            [canDelete=(yes|no)]
    ///            [canOverlap=(yes|no)]
    ///            [canReorder=(yes|no)]
    ///            [copyOf=string]
    ///            [dataRef=string]
    ///            [dir=(ltr|rtl|auto)]
    ///            [disp=string]
    ///            [equiv=string]
    ///            id=string
    ///            [isolated=(yes|no)]
    ///            [subFlows=string]
    ///            [subType=string]
    ///            [type=(fmt|ui|quote|link|image|other)] .../>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#sc">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="CodeBase"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class SpanningCodeStart : CodeBase, IInheritanceInfoProvider
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SpanningCodeStart"/> class.
        /// </summary>
        /// <param name="id">The Id of the marked span.</param>
        public SpanningCodeStart(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanningCodeStart"/> class.
        /// </summary>
        internal SpanningCodeStart()
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
                            { SpanningCodeStart.PropertyNames.DataReference, this.DataReference }
                        };
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the spanning code where this attribute is used can enclose
        /// partial spanning codes (ex. a start code without its corresponding end code, or an end code without
        /// its corresponding start code).
        /// </summary>
        [Converter(typeof(BoolConverter))]
        [DefaultValue(true)]
        [SchemaEntity(AttributeNames.CanOverlap, Requirement.Optional)]
        public bool CanOverlap
        {
            get { return (bool)this.GetPropertyValue(SpanningCodeStart.PropertyNames.CanOverlap); }
            set { this.SetPropertyValue(value, SpanningCodeStart.PropertyNames.CanOverlap); }
        }

        /// <summary>
        /// Gets or sets the Id of the <see cref="Data"/> element that contains the original data for a given
        /// inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.DataReference, Requirement.Optional)]
        public string DataReference
        {
            get { return (string)this.GetPropertyValue(SpanningCodeStart.PropertyNames.DataReference); }
            set { this.SetPropertyValue(value, SpanningCodeStart.PropertyNames.DataReference); }
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
            get { return (ContentDirectionality)this.GetPropertyValue(SpanningCodeStart.PropertyNames.Directionality); }
            set { this.SetPropertyValue(value, SpanningCodeStart.PropertyNames.Directionality); }
        }

        /// <summary>
        /// Gets or sets an alternative user-friendly display representation of the original data of the inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.DisplayText, Requirement.Optional)]
        public string DisplayText
        {
            get { return (string)this.GetPropertyValue(SpanningCodeStart.PropertyNames.DisplayText); }
            set { this.SetPropertyValue(value, SpanningCodeStart.PropertyNames.DisplayText); }
        }

        /// <summary>
        /// Gets or sets a plain text representation of the original data of the inline code that can be used
        /// when generating a plain text representation of the content.
        /// </summary>
        [DefaultValue("")]
        [SchemaEntity(AttributeNames.EquivalentText, Requirement.Optional)]
        public string EquivalentText
        {
            get { return (string)this.GetPropertyValue(SpanningCodeStart.PropertyNames.EquivalentText); }
            set { this.SetPropertyValue(value, SpanningCodeStart.PropertyNames.EquivalentText); }
        }

        /// <summary>
        /// Gets a value indicating whether the element contains subflow values.
        /// </summary>
        public override bool HasSubFlows
        {
            get { return !string.IsNullOrWhiteSpace(this.SubFlows); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the start or end marker of a spanning inline code is not in
        /// the same <see cref="Unit"/> as its corresponding end or start code.
        /// </summary>
        [DefaultValue(false)]
        [Converter(typeof(BoolConverter))]
        [SchemaEntity(AttributeNames.Isolated, Requirement.Optional)]
        public bool Isolated
        {
            get { return (bool)this.GetPropertyValue(SpanningCodeStart.PropertyNames.Isolated); }
            set { this.SetPropertyValue(value, SpanningCodeStart.PropertyNames.Isolated); }
        }

        /// <summary>
        /// Gets or sets holds a list of Ids corresponding to the <see cref="Unit"/> elements that contain the
        /// sub-flows for a given inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.SubFlows, Requirement.Optional)]
        public string SubFlows
        {
            get { return (string)this.GetPropertyValue(SpanningCodeStart.PropertyNames.SubFlows); }
            set { this.SetPropertyValue(value, SpanningCodeStart.PropertyNames.SubFlows); }
        }

        /// <summary>
        /// Gets a value indicating whether the object has references to <see cref="Data"/> elements.
        /// </summary>
        public override bool SupportsDataReferences
        {
            get { return true; }
        }
        #endregion Properties

        #region IInheritanceInfoProvider Implementation
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
        #endregion IInheritanceInfoProvider Implementation

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="SpanningCodeStart.CanOverlap"/> property.
            /// </summary>
            public const string CanOverlap = "CanOverlap";

            /// <summary>
            /// The name of the <see cref="SpanningCodeStart.DataReference"/> property.
            /// </summary>
            public const string DataReference = "DataReference";

            /// <summary>
            /// The name of the <see cref="SpanningCodeStart.Directionality"/> property.
            /// </summary>
            public const string Directionality = "Directionality";

            /// <summary>
            /// The name of the <see cref="SpanningCodeStart.DisplayText"/> property.
            /// </summary>
            public const string DisplayText = "DisplayText";

            /// <summary>
            /// The name of the <see cref="SpanningCodeStart.EquivalentText"/> property.
            /// </summary>
            public const string EquivalentText = "EquivalentText";

            /// <summary>
            /// The name of the <see cref="SpanningCodeStart.Isolated"/> property.
            /// </summary>
            public const string Isolated = "Isolated";

            /// <summary>
            /// The name of the <see cref="SpanningCodeStart.SubFlows"/> property.
            /// </summary>
            public const string SubFlows = "SubFlows";
        }
    }
}
