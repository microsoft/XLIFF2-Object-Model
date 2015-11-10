namespace Localization.Xliff.OM.Modules.ResourceData
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// This class represents reference to the actual resource data that is either intended for modification, or to be
    /// used as contextual reference during translation. This corresponds to a &lt;source> element in the XLIFF 2.0
    /// specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;source [href=string]
    ///                [xml:lang=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#res_source">XLIFF
    /// specification</a> for more details.
    /// </remarks>
    /// <seealso cref="ResourceItemReferenceBase"/>
    [SuppressMessage(
                     "StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class ResourceItemSource : ResourceItemReferenceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceItemSource"/> class.
        /// </summary>
        public ResourceItemSource()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
        }
    }
}
