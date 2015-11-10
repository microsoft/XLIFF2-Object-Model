namespace Localization.Xliff.OM.Modules.FormatStyle
{
    using System.Collections.Generic;

    /// <summary>
    /// This interface provides information for generating a quick at a glance HTML preview of XLIFF content using a
    /// predefined set of simple HTML formatting elements.
    /// </summary>
    public interface IFormatStyleAttributes
    {
        /// <summary>
        /// Gets or sets basic formatting information using a predefined subset of HTML formatting elements. It enables
        /// the generation of HTML pages or snippets for preview and review purposes.
        /// </summary>
        FormatStyleValue? FormatStyle { get; set; }

        /// <summary>
        /// Gets extra HTML attributes to use along with the HTML element declared in the FormatStyle attribute. The
        /// key is the HTML attribute name and the value is the value for that attribute.
        /// </summary>
        IDictionary<string, string> SubFormatStyle { get; }
    }
}
