namespace Localization.Xliff.OM.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Exceptions;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents an XLIFF code point. This corresponds to a &lt;cp> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: &lt;cp hex="0010"/></example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#cp">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <see cref="ResourceStringContent"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class CodePoint : ResourceStringContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodePoint"/> class.
        /// </summary>
        /// <param name="code">The code point.</param>
        public CodePoint(int code)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.Code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodePoint"/> class.
        /// </summary>
        internal CodePoint()
            : this(0)
        {
        }

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating the code point.
        /// </summary>
        [Converter(typeof(HexConverter))]
        [SchemaEntity(AttributeNames.Hex, Requirement.Required)]
        public int Code
        {
            get
            {
                return (int)this.GetPropertyValue(CodePoint.PropertyNames.Code);
            }

            set 
            {
                // Code point must be a valid Unicode code point.
                if (value < 0 || value > 0x10FFFF)
                {
                    throw new ArgumentOutOfRangeException(
                        string.Format(Properties.Resources.CodePoint_OutOfRange, value));
                }
                
                // Code point must not be a valid XML character.
                if (Utilities.IsValidXmlCodePoint(value))
                {
                    throw new ArgumentException(
                        string.Format(Properties.Resources.CodePoint_InvalidCode, value));
                }

                this.SetPropertyValue(value, CodePoint.PropertyNames.Code);
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            CodePoint point;

            point = obj as CodePoint;
            return (point != null) && (point.Code == this.Code);
        }

        /// <summary>
        /// Gets as a hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return this.Code.GetHashCode();
        }
        #endregion Methods

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the Code property.
            /// </summary>
            public const string Code = "Code";
        }
    }
}