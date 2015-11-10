namespace Localization.Xliff.OM.Core
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Core.XmlNames;

    /// <summary>
    /// This class represents a standalone code inline tag. This corresponds to a &lt;ph> element in the XLIFF 2.0
    /// specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;ph [canCopy=(yes|no)]
    ///            [canDelete=(yes|no)]
    ///            [canReorder=(yes|no)]
    ///            [copyOf=string]
    ///            [disp=string]
    ///            [equiv=string]
    ///            id=string
    ///            [dataRef=string]
    ///            [subFlows=string]
    ///            [subType=string]
    ///            [type=(fmt|ui|quote|link|image|other)] .../>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#ph">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="CodeBase"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class StandaloneCode : CodeBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StandaloneCode"/> class.
        /// </summary>
        /// <param name="id">The Id of the marked span.</param>
        public StandaloneCode(string id)
            : base(id)
        {
            this.EnableAttribute(StandaloneCode.PropertyNames.SizeRestriction, false);
            this.EnableAttribute(StandaloneCode.PropertyNames.StorageRestriction, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandaloneCode"/> class.
        /// </summary>
        internal StandaloneCode()
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
                            { StandaloneCode.PropertyNames.DataReference, this.DataReference }
                        };
            }
        }

        /// <summary>
        /// Gets or sets the Id of the <see cref="Data"/> element that contains the original data for a given
        /// inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.DataReference, Requirement.Optional)]
        public string DataReference
        {
            get { return (string)this.GetPropertyValue(StandaloneCode.PropertyNames.DataReference); }
            set { this.SetPropertyValue(value, StandaloneCode.PropertyNames.DataReference); }
        }

        /// <summary>
        /// Gets or sets an alternative user-friendly display representation of the original data of the end
        /// marker of an inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.DisplayText, Requirement.Optional)]
        public string DisplayText
        {
            get { return (string)this.GetPropertyValue(StandaloneCode.PropertyNames.DisplayText); }
            set { this.SetPropertyValue(value, StandaloneCode.PropertyNames.DisplayText); }
        }

        /// <summary>
        /// Gets or sets a plain text representation of the original data of the inline code that can be used
        /// when generating a plain text representation of the content.
        /// </summary>
        [DefaultValue("")]
        [SchemaEntity(AttributeNames.EquivalentText, Requirement.Optional)]
        public string EquivalentText
        {
            get { return (string)this.GetPropertyValue(StandaloneCode.PropertyNames.EquivalentText); }
            set { this.SetPropertyValue(value, StandaloneCode.PropertyNames.EquivalentText); }
        }

        /// <summary>
        /// Gets a value indicating whether the element contains subflow values.
        /// </summary>
        public override bool HasSubFlows
        {
            get { return !string.IsNullOrWhiteSpace(this.SubFlows); }
        }

        /// <summary>
        /// Gets or sets holds a list of Ids corresponding to the <see cref="Unit"/> elements that contain the
        /// sub-flows for a given inline code.
        /// </summary>
        [SchemaEntity(AttributeNames.SubFlows, Requirement.Optional)]
        public string SubFlows
        {
            get { return (string)this.GetPropertyValue(StandaloneCode.PropertyNames.SubFlows); }
            set { this.SetPropertyValue(value, StandaloneCode.PropertyNames.SubFlows); }
        }

        /// <summary>
        /// Gets a value indicating whether the object has references to <see cref="Data"/> elements.
        /// </summary>
        public override bool SupportsDataReferences
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the object supports the SizeRestriction property.
        /// </summary>
        public override bool SupportsSizeRestriction
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the object supports the StorageRestriction property.
        /// </summary>
        public override bool SupportsStorageRestriction
        {
            get { return false; }
        }
        #endregion Properties

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="StandaloneCode.DataReference"/> property.
            /// </summary>
            public const string DataReference = "DataReference";

            /// <summary>
            /// The name of the <see cref="StandaloneCode.DisplayText"/> property.
            /// </summary>
            public const string DisplayText = "DisplayText";

            /// <summary>
            /// The name of the <see cref="StandaloneCode.EquivalentText"/> property.
            /// </summary>
            public const string EquivalentText = "EquivalentText";

            /// <summary>
            /// The name of the <see cref="CodeBase.SizeRestriction"/> property.
            /// </summary>
            public const string SizeRestriction = "SizeRestriction";

            /// <summary>
            /// The name of the <see cref="CodeBase.StorageRestriction"/> property.
            /// </summary>
            public const string StorageRestriction = "StorageRestriction";

            /// <summary>
            /// The name of the <see cref="StandaloneCode.SubFlows"/> property.
            /// </summary>
            public const string SubFlows = "SubFlows";
        }
    }
}
