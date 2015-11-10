namespace Localization.Xliff.OM.Converters
{
    using Localization.Xliff.OM.Core;

    /// <summary>
    /// Converts a <see cref="CanReorderValue"/> value to an XLIFF string, and back.
    /// </summary>
    /// <seealso cref="ConverterBase{TData}"/>
    internal class CanReorderValueConverter : ConverterBase<CanReorderValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CanReorderValueConverter"/> class.
        /// </summary>
        public CanReorderValueConverter()
        {
            this.Map[CanReorderValue.FirstNo] = "firstNo";
            this.Map[CanReorderValue.No] = "no";
            this.Map[CanReorderValue.Yes] = "yes";
        }
    }
}
