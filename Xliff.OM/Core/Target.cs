namespace Localization.Xliff.OM.Core
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents text that was translated. This corresponds to a &lt;target> element in the
    /// XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;target [xml:lang=string]
    ///                [xml:space=(default|preserve)]
    ///                [order=uint] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#target">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="ResourceString"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class Target : ResourceString, IInheritanceInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Target"/> class.
        /// </summary>
        public Target()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Target"/> class.
        /// </summary>
        /// <param name="text">The translated text.</param>
        public Target(string text)
            : base(text)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
        }

        #region Properties
        /// <summary>
        /// Gets a value that is used as a prefix on the SelectorId for children.
        /// </summary>
        internal override string ChildSelectorIdPrefix
        {
            get { return Target.Constants.SelectorPrefix; }
        }

        /// <summary>
        /// Gets or sets the value indicating the position in the host container this text should be assigned.
        /// </summary>
        /// <remarks>If this value is not specified, the <see cref="Target"/> assumes the position as ordered in
        /// the host container.</remarks>
        [Converter(typeof(UIntConverter))]
        [SchemaEntity(AttributeNames.Order, Requirement.Optional)]
        public uint? Order
        {
            get { return (uint?)this.GetPropertyValue(Target.PropertyNames.Order); }
            set { this.SetPropertyValue(value, Target.PropertyNames.Order); }
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
            Debug.Assert(property == "Language", "Unsupported property");
            return new InheritanceInfo(this.LanguageInheritanceHandler);
        }

        /// <summary>
        /// Handler called when getting the custom Directionality value from a spanning code element. The value
        /// depends on the hierarchy of the ancestry.
        /// </summary>
        /// <param name="element">The element that the property is for.</param>
        /// <param name="property">The name of the property that is to be returned.</param>
        /// <returns>The custom value.</returns>
        private object LanguageInheritanceHandler(XliffElement element, string property)
        {
            object result;
            XliffDocument document;

            Debug.Assert(property == "Language", "Property name is not supported.");

            result = null;
            document = element.FindAncestor<XliffDocument>();
            if (document != null)
            {
                result = document.TargetLanguage;
            }

            return result;
        }
        #endregion IInheritanceInfoProvider Implementation

        /// <summary>
        /// This class contains constant values that are used in this class.
        /// </summary>
        private static class Constants
        {
            /// <summary>
            /// The selector path prefix for this element.
            /// </summary>
            public const string SelectorPrefix = "t";
        }

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Target.Order"/> property.
            /// </summary>
            public const string Order = "Order";
        }
    }
}