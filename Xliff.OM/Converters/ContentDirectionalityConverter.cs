namespace Localization.Xliff.OM.Converters
{
    using Localization.Xliff.OM.Core;

    /// <summary>
    /// Converts a <see cref="ContentDirectionality"/> value to an XLIFF string, and back.
    /// </summary>
    /// <seealso cref="ConverterBase{TData}"/>
    internal class ContentDirectionalityConverter : ConverterBase<ContentDirectionality>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentDirectionalityConverter"/> class.
        /// </summary>
        public ContentDirectionalityConverter()
        {
            this.Map[ContentDirectionality.Auto] = "auto";
            this.Map[ContentDirectionality.LTR] = "ltr";
            this.Map[ContentDirectionality.RTL] = "rtl";
        }
    }
}
