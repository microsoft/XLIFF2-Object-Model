namespace Localization.Xliff.OM.Core
{
    /// <summary>
    /// Defines the state of translation for content.
    /// </summary>
    public enum TranslationState
    {
        /// <summary>
        /// The content is in its initial state.
        /// </summary>
        Initial,

        /// <summary>
        /// The content has been translated.
        /// </summary>
        Translated,

        /// <summary>
        /// The content has been reviewed.
        /// </summary>
        Reviewed,

        /// <summary>
        /// The content is finalized and ready to be used.
        /// </summary>
        Final,

        /// <summary>
        /// None of the previous.
        /// </summary>
        NotApplicable
    }
}