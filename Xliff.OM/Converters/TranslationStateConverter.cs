namespace Localization.Xliff.OM.Converters
{
    using Localization.Xliff.OM.Core;

    /// <summary>
    /// Converts a <see cref="TranslationState"/> value to an XLIFF string, and back.
    /// </summary>
    /// <seealso cref="ConverterBase{TData}"/>
    internal class TranslationStateConverter : ConverterBase<TranslationState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationStateConverter"/> class.
        /// </summary>
        public TranslationStateConverter()
        {
            this.Map[TranslationState.Final] = "final";
            this.Map[TranslationState.Initial] = "initial";
            this.Map[TranslationState.Reviewed] = "reviewed";
            this.Map[TranslationState.Translated] = "translated";
        }
    }
}
