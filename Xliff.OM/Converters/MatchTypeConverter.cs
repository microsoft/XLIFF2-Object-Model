namespace Localization.Xliff.OM.Converters
{
    using Localization.Xliff.OM.Modules.TranslationCandidates;

    /// <summary>
    /// Converts a <see cref="MatchType"/> value to an XLIFF string, and back.
    /// </summary>
    /// <seealso cref="ConverterBase{TData}"/>
    internal class MatchTypeConverter : ConverterBase<MatchType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchTypeConverter"/> class.
        /// </summary>
        public MatchTypeConverter()
        {
            this.Map[MatchType.AssembledMatch] = "am";
            this.Map[MatchType.IdentifierMatch] = "idm";
            this.Map[MatchType.InContextMatch] = "icm";
            this.Map[MatchType.MachineTranslation] = "mt";
            this.Map[MatchType.Other] = "other";
            this.Map[MatchType.TermBase] = "tb";
            this.Map[MatchType.TranslationMemory] = "tm";
        }
    }
}
