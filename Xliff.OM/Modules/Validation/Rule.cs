namespace Localization.Xliff.OM.Modules.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.Validation.XmlNames;

    /// <summary>
    /// This class represents a specific rule and constraint to apply to the target text of the enclosing element. This
    /// corresponds to a &lt;rule> element in the
    /// XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;rule [caseSensitive=(yes|no)]
    ///              [disabled=(yes|no)]
    ///              [endsWith=string]
    ///              [existsInSource=(yes|no)]
    ///              [isNotPresent=string]
    ///              [isPresent=string]
    ///              [normalization=(none|nfc|nfd)]
    ///              [occurs=number]
    ///              [startsWith=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#rule">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    [SuppressMessage(
                     "StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class Rule : XliffElement, IExtensible
    {
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rule"/> class.
        /// </summary>
        public Rule()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
        }

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the test defined within the rule is case sensitive (true) or not
        /// (false).
        /// </summary>
        [Converter(typeof(BoolConverter))]
        [DefaultValue(true)]
        [SchemaEntity(AttributeNames.CaseSensitive, Requirement.Optional)]
        public bool CaseSensitive
        {
            get { return (bool)this.GetPropertyValue(Rule.PropertyNames.CaseSensitive); }
            set { this.SetPropertyValue(value, Rule.PropertyNames.CaseSensitive); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the rule must (false) or must not (true) be applied within the
        /// scope of its enclosing element.
        /// </summary>
        [Converter(typeof(BoolConverter))]
        [DefaultValue(false)]
        [SchemaEntity(AttributeNames.Disabled, Requirement.Optional)]
        public bool Disabled
        {
            get { return (bool)this.GetPropertyValue(Rule.PropertyNames.Disabled); }
            set { this.SetPropertyValue(value, Rule.PropertyNames.Disabled); }
        }

        /// <summary>
        /// Gets or sets a string that indicates a text must end with the specified value.
        /// </summary>
        [SchemaEntity(AttributeNames.EndsWith, Requirement.Optional)]
        public string EndsWith
        {
            get { return (string)this.GetPropertyValue(Rule.PropertyNames.EndsWith); }
            set { this.SetPropertyValue(value, Rule.PropertyNames.EndsWith); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the condition must be satisfied in both the source and target. This
        /// value is only valid when used in conjunction with one of IsPresent, StartsWith, or EndsWith.
        /// </summary>
        [Converter(typeof(BoolConverter))]
        [DefaultValue(false)]
        [SchemaEntity(AttributeNames.ExistsInSource, Requirement.Optional)]
        public bool ExistsInSource
        {
            get { return (bool)this.GetPropertyValue(Rule.PropertyNames.ExistsInSource); }
            set { this.SetPropertyValue(value, Rule.PropertyNames.ExistsInSource); }
        }

        /// <summary>
        /// Gets the list of registered extensions on the object.
        /// </summary>
        IList<IExtension> IExtensible.Extensions
        {
            get { return this.extensions.Value; }
        }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        bool IExtensible.HasExtensions
        {
            get { return this.extensions.IsValueCreated && (this.extensions.Value.Count > 0); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a string must not be present in the target text.
        /// </summary>
        [SchemaEntity(AttributeNames.IsNotPresent, Requirement.Optional)]
        public string IsNotPresent
        {
            get { return (string)this.GetPropertyValue(Rule.PropertyNames.IsNotPresent); }
            set { this.SetPropertyValue(value, Rule.PropertyNames.IsNotPresent); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a string must be present in the target text.
        /// </summary>
        [SchemaEntity(AttributeNames.IsPresent, Requirement.Optional)]
        public string IsPresent
        {
            get { return (string)this.GetPropertyValue(Rule.PropertyNames.IsPresent); }
            set { this.SetPropertyValue(value, Rule.PropertyNames.IsPresent); }
        }

        /// <summary>
        /// Gets or sets the normalization form to apply when validating a rule.
        /// </summary>
        [Converter(typeof(NormalizationValueConverter))]
        [DefaultValue(NormalizationValue.NFC)]
        [SchemaEntity(AttributeNames.Normalization, Requirement.Optional)]
        public NormalizationValue Normalization
        {
            get { return (NormalizationValue)this.GetPropertyValue(Rule.PropertyNames.Normalization); }
            set { this.SetPropertyValue(value, Rule.PropertyNames.Normalization); }
        }

        /// <summary>
        /// Gets or sets the exact number of times a string must be present in the target text. When this value is false,
        /// then the string must be present in the target text at least once. This value is only used in conjunction
        /// with IsPresent.
        /// </summary>
        [Converter(typeof(UIntConverter))]
        [SchemaEntity(AttributeNames.Occurs, Requirement.Optional)]
        public uint? Occurs
        {
            get { return (uint?)this.GetPropertyValue(Rule.PropertyNames.Occurs); }
            set { this.SetPropertyValue(value, Rule.PropertyNames.Occurs); }
        }

        /// <summary>
        /// Gets or sets a string that indicates a text must start with the specified value.
        /// </summary>
        [SchemaEntity(AttributeNames.StartsWith, Requirement.Optional)]
        public string StartsWith
        {
            get { return (string)this.GetPropertyValue(Rule.PropertyNames.StartsWith); }
            set { this.SetPropertyValue(value, Rule.PropertyNames.StartsWith); }
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
            get { return false; }
        }
        #endregion Properties

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Rule.CaseSensitive"/> property.
            /// </summary>
            public const string CaseSensitive = "CaseSensitive";

            /// <summary>
            /// The name of the <see cref="Rule.Disabled"/> property.
            /// </summary>
            public const string Disabled = "Disabled";

            /// <summary>
            /// The name of the <see cref="Rule.EndsWith"/> property.
            /// </summary>
            public const string EndsWith = "EndsWith";

            /// <summary>
            /// The name of the <see cref="Rule.ExistsInSource"/> property.
            /// </summary>
            public const string ExistsInSource = "ExistsInSource";

            /// <summary>
            /// The name of the <see cref="Rule.IsNotPresent"/> property.
            /// </summary>
            public const string IsNotPresent = "IsNotPresent";

            /// <summary>
            /// The name of the <see cref="Rule.IsPresent"/> property.
            /// </summary>
            public const string IsPresent = "IsPresent";

            /// <summary>
            /// The name of the <see cref="Rule.Normalization"/> property.
            /// </summary>
            public const string Normalization = "Normalization";

            /// <summary>
            /// The name of the <see cref="Rule.Occurs"/> property.
            /// </summary>
            public const string Occurs = "Occurs";

            /// <summary>
            /// The name of the <see cref="Rule.StartsWith"/> property.
            /// </summary>
            public const string StartsWith = "StartsWith";
        }
    }
}
