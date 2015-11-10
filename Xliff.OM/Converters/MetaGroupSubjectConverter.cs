namespace Localization.Xliff.OM.Converters
{
    using Localization.Xliff.OM.Modules.Metadata;

    /// <summary>
    /// Converts a <see cref="MetaGroupSubject"/> value to an XLIFF string, and back.
    /// </summary>
    /// <seealso cref="ConverterBase{TData}"/>
    internal class MetaGroupSubjectConverter : ConverterBase<MetaGroupSubject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaGroupSubjectConverter"/> class.
        /// </summary>
        public MetaGroupSubjectConverter()
        {
            this.Map[MetaGroupSubject.Ignorable] = "ignorable";
            this.Map[MetaGroupSubject.Source] = "source";
            this.Map[MetaGroupSubject.Target] = "target";
        }
    }
}
