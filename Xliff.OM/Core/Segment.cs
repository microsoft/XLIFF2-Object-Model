namespace Localization.Xliff.OM.Core
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class is a container that contains source text and translated text. This corresponds to a
    /// &lt;segment> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;segment [id=string]
    ///                 [canResegment=(yes|no)]
    ///                 [state=(initial|translated|reviewed|final)]
    ///                 [subState=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#segment">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="ContainerResource"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                         "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                         Justification = "Example contains literals described in the specification.")]
    public class Segment : ContainerResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Segment"/> class.
        /// </summary>
        /// <param name="id">The Id of the segment.</param>
        public Segment(string id)
            : base(id)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Segment"/> class.
        /// </summary>
        /// <param name="id">The Id of the segment.</param>
        /// <param name="state">The translation state of the segment.</param>
        public Segment(string id, TranslationState state)
            : base(id)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.State = state;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Segment"/> class.
        /// </summary>
        public Segment()
            : this(null)
        {
        }

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the source content can be segmented.
        /// </summary>
        [Converter(typeof(BoolConverter))]
        [DefaultValue(true)]
        [InheritValue(Inheritance.Parent)]
        [SchemaEntity(AttributeNames.CanResegment, Requirement.Optional)]
        public bool CanResegment
        {
            get { return (bool)this.GetPropertyValue(Segment.PropertyNames.CanResegment); }
            set { this.SetPropertyValue(value, Segment.PropertyNames.CanResegment); }
        }

        /// <summary>
        /// Gets or sets the translation state of the resources within the segment.
        /// </summary>
        [Converter(typeof(TranslationStateConverter))]
        [DefaultValue(TranslationState.Initial)]
        [ExplicitOutputDependency(Segment.PropertyNames.SubState)]
        [SchemaEntity(AttributeNames.State, Requirement.Optional)]
        public TranslationState State
        {
            get { return (TranslationState)this.GetPropertyValue(Segment.PropertyNames.State); }
            set { this.SetPropertyValue(value, Segment.PropertyNames.State); }
        }

        /// <summary>
        /// Gets or sets the sub-state of the resources within the segment.
        /// </summary>
        /// <remarks>If the value is not null, the format of the value must be "prefix:value" where prefix must
        /// not be "xlf" and prefix and value must not be empty strings.</remarks>
        [SchemaEntity(AttributeNames.SubState, Requirement.Optional)]
        public string SubState
        {
            get { return (string)this.GetPropertyValue(Segment.PropertyNames.SubState); }
            set { this.SetPropertyValue(value, Segment.PropertyNames.SubState); }
        }
        #endregion Properties

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Segment.CanResegment"/> property.
            /// </summary>
            public const string CanResegment = "CanResegment";

            /// <summary>
            /// The name of the <see cref="Segment.State"/> property.
            /// </summary>
            public const string State = "State";

            /// <summary>
            /// The name of the <see cref="Segment.SubState"/> property.
            /// </summary>
            public const string SubState = "SubState";
        }
    }
}