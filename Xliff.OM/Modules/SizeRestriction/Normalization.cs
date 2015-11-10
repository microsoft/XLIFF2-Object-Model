namespace Localization.Xliff.OM.Modules.SizeRestriction
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Modules.SizeRestriction.XmlNames;

    /// <summary>
    /// This class is used to hold the attributes specifying the normalization form to apply to storage and size
    /// restrictions defined in the standard profiles. This corresponds to a &lt;normalization> element in the
    /// XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;normalization [general=(none|nfc|nfd)]
    ///                       [storage=(none|nfc|nfd)]>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#slr_normalization">XLIFF
    /// specification</a> for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    [SuppressMessage(
                     "StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class Normalization : XliffElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Normalization"/> class.
        /// </summary>
        public Normalization()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
        }

        #region Properties
        /// <summary>
        /// Gets or sets the normalization form to apply for general size restrictions.
        /// </summary>
        [Converter(typeof(NormalizationValueConverter))]
        [DefaultValue(NormalizationValue.None)]
        [SchemaEntity(AttributeNames.General, Requirement.Optional)]
        public NormalizationValue General
        {
            get { return (NormalizationValue)this.GetPropertyValue(Normalization.PropertyNames.General); }
            set { this.SetPropertyValue(value, Normalization.PropertyNames.General); }
        }

        /// <summary>
        /// Gets or sets the normalization form to apply for storage size restrictions.
        /// </summary>
        [Converter(typeof(NormalizationValueConverter))]
        [DefaultValue(NormalizationValue.None)]
        [SchemaEntity(AttributeNames.Storage, Requirement.Optional)]
        public NormalizationValue Storage
        {
            get { return (NormalizationValue)this.GetPropertyValue(Normalization.PropertyNames.Storage); }
            set { this.SetPropertyValue(value, Normalization.PropertyNames.Storage); }
        }
        #endregion Properties

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Normalization.General"/> property.
            /// </summary>
            public const string General = "General";

            /// <summary>
            /// The name of the <see cref="Normalization.Storage"/> property.
            /// </summary>
            public const string Storage = "Storage";
        }
    }
}
