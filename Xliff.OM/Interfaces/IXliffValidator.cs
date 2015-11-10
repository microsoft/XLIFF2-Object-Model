namespace Localization.Xliff.OM.Validators
{
    using Localization.Xliff.OM.Core;

    /// <summary>
    /// This interface is used to indicate an object can validate XLIFF content.
    /// </summary>
    public interface IXliffValidator
    {
        /// <summary>
        /// Validates the given document structure and contents.
        /// </summary>
        /// <param name="document">The document to validate.</param>
        void Validate(XliffDocument document);
    }
}
