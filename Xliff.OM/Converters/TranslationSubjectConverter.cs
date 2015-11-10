namespace Localization.Xliff.OM.Converters
{
    using System;
    using Localization.Xliff.OM.Core;

    /// <summary>
    /// Converts a <see cref="TranslationSubject"/> value to an XLIFF string, and back.
    /// </summary>
    /// <seealso cref="ConverterBase{TData}"/>
    internal class TranslationSubjectConverter : ConverterBase<TranslationSubject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationSubjectConverter"/> class.
        /// </summary>
        public TranslationSubjectConverter()
        {
            this.Map[TranslationSubject.Source] = "source";
            this.Map[TranslationSubject.Target] = "target";
        }
    }
}
