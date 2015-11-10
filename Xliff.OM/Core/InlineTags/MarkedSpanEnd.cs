namespace Localization.Xliff.OM.Core
{
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents an end marker of an annotation where the spanning marker cannot be used to
    /// indicate text is well-formed. This corresponds to a &lt;em> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;em startRef=string .../>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#em">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="ResourceStringContent"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class MarkedSpanEnd : ResourceStringContent
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkedSpanEnd"/> class.
        /// </summary>
        /// <param name="reference">The Id of the <see cref="SpanningCodeStart"/> element or the
        /// <see cref="MarkedSpanStart"/> element a given <see cref="SpanningCodeEnd"/> element or
        /// <see cref="MarkedSpanEnd"/> element corresponds.</param>
        public MarkedSpanEnd(string reference)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.StartReference = reference;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkedSpanEnd"/> class.
        /// </summary>
        internal MarkedSpanEnd()
            : this(null)
        {
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the Id of the <see cref="SpanningCodeStart"/> element or the <see cref="MarkedSpanStart"/>
        /// element a given <see cref="SpanningCodeEnd"/> element or <see cref="MarkedSpanEnd"/> element corresponds.
        /// </summary>
        [SchemaEntity(AttributeNames.StartReference, Requirement.Optional)]
        public string StartReference
        {
            get { return (string)this.GetPropertyValue(MarkedSpanEnd.PropertyNames.StartReference); }
            set { this.SetPropertyValue(value, MarkedSpanEnd.PropertyNames.StartReference); }
        }
        #endregion Properties

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="MarkedSpanEnd.StartReference"/> property.
            /// </summary>
            public const string StartReference = "StartReference";
        }
    }
}
