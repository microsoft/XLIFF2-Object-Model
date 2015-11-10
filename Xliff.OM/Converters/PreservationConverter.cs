namespace Localization.Xliff.OM.Converters
{
    /// <summary>
    /// Converts a <see cref="Preservation"/> value to an XLIFF string, and back.
    /// </summary>
    /// <seealso cref="ConverterBase{TData}"/>
    internal class PreservationConverter : ConverterBase<Preservation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreservationConverter"/> class.
        /// </summary>
        public PreservationConverter()
        {
            this.Map[Preservation.Default] = "default";
            this.Map[Preservation.Preserve] = "preserve";
        }
    }
}
