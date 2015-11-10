namespace Localization.Xliff.OM.Converters
{
    using System;
    using Localization.Xliff.OM.Core;

    /// <summary>
    /// Converts a <see cref="CodeType"/> value to an XLIFF string, and back.
    /// </summary>
    /// <seealso cref="ConverterBase{TData}"/>
    internal class CodeTypeConverter : ConverterBase<CodeType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeTypeConverter"/> class.
        /// </summary>
        public CodeTypeConverter()
        {
            this.Map[CodeType.Formatting] = "fmt";
            this.Map[CodeType.Image] = "image";
            this.Map[CodeType.Link] = "link";
            this.Map[CodeType.Other] = "other";
            this.Map[CodeType.Quote] = "quote";
            this.Map[CodeType.UserInterface] = "ui";
        }
    }
}
