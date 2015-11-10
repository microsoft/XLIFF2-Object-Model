namespace Localization.Xliff.OM.Modules.ResourceData
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// This class represents a reference to the localized counterpart of the sibling <see cref="ResourceItemSource"/>
    /// element. This corresponds to a &lt;target> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;target [href=string]
    ///                [xml:lang=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#res_target">XLIFF
    /// specification</a> for more details.
    /// </remarks>
    /// <seealso cref="ResourceItemReferenceBase"/>
    [SuppressMessage(
                     "StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class ResourceItemTarget : ResourceItemReferenceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceItemTarget"/> class.
        /// </summary>
        public ResourceItemTarget()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
        }
    }
}
