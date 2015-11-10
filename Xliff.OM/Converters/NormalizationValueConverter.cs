namespace Localization.Xliff.OM.Converters
{
    using Localization.Xliff.OM.Modules;

    /// <summary>
    /// Converts a <see cref="NormalizationValue"/> value to an XLIFF string, and back.
    /// </summary>
    /// <seealso cref="ConverterBase{TData}"/>
    internal class NormalizationValueConverter : ConverterBase<NormalizationValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NormalizationValueConverter"/> class.
        /// </summary>
        public NormalizationValueConverter()
        {
            this.Map[NormalizationValue.NFC] = "nfc";
            this.Map[NormalizationValue.NFD] = "nfd";
            this.Map[NormalizationValue.None] = "none";
        }
    }
}
