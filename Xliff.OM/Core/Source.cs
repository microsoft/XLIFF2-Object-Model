namespace Localization.Xliff.OM.Core
{
    using System.Diagnostics;

    /// <summary>
    /// This class represents source text that is to be translated. This corresponds to a &lt;source> element
    /// in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;source [xml:lang=string]
    ///                [xml:space=(default|preserve)] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#source">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="ResourceString"/>
    public class Source : ResourceString, IInheritanceInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Source"/> class.
        /// </summary>
        public Source()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Source"/> class.
        /// </summary>
        /// <param name="text">The text to translate.</param>
        public Source(string text)
            : base(text)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
        }

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
                result = document.SourceLanguage;
            }

            return result;
        }
        #endregion IInheritanceInfoProvider Implementation
    }
}