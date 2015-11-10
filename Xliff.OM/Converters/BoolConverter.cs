namespace Localization.Xliff.OM.Converters
{
    /// <summary>
    /// Converts a boolean value to an XLIFF string, and back.
    /// </summary>
    /// <seealso cref="ConverterBase{TData}"/>
    internal class BoolConverter : ConverterBase<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolConverter"/> class.
        /// </summary>
        public BoolConverter()
        {
            this.Map[true] = "yes";
            this.Map[false] = "no";
        }
    }
}
