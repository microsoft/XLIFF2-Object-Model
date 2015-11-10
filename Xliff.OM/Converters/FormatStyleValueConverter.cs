namespace Localization.Xliff.OM.Converters
{
    using Localization.Xliff.OM.Modules.FormatStyle;

    /// <summary>
    /// Converts a <see cref="FormatStyleValue"/> value to an XLIFF string, and back.
    /// </summary>
    /// <seealso cref="ConverterBase{TData}"/>
    internal class FormatStyleValueConverter : ConverterBase<FormatStyleValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatStyleValueConverter"/> class.
        /// </summary>
        public FormatStyleValueConverter()
        {
            this.Map[FormatStyleValue.Anchor] = "a";
            this.Map[FormatStyleValue.BoldText] = "b";
            this.Map[FormatStyleValue.BiDiOverride] = "bdo";
            this.Map[FormatStyleValue.BigText] = "big";
            this.Map[FormatStyleValue.BlockQuote] = "blockquote";
            this.Map[FormatStyleValue.Body] = "body";
            this.Map[FormatStyleValue.LineBreak] = "br";
            this.Map[FormatStyleValue.Button] = "button";
            this.Map[FormatStyleValue.Caption] = "caption";
            this.Map[FormatStyleValue.Center] = "center";
            this.Map[FormatStyleValue.Citation] = "cite";
            this.Map[FormatStyleValue.Code] = "code";
            this.Map[FormatStyleValue.TableColumn] = "col";
            this.Map[FormatStyleValue.TableColumnGroup] = "colgroup";
            this.Map[FormatStyleValue.DefinitionDescription] = "dd";
            this.Map[FormatStyleValue.DeletedText] = "del";
            this.Map[FormatStyleValue.Div] = "div";
            this.Map[FormatStyleValue.DefinitionList] = "dl";
            this.Map[FormatStyleValue.DefinitionTerm] = "dt";
            this.Map[FormatStyleValue.Emphasis] = "em";
            this.Map[FormatStyleValue.Heading1] = "h1";
            this.Map[FormatStyleValue.Heading2] = "h2";
            this.Map[FormatStyleValue.Heading3] = "h3";
            this.Map[FormatStyleValue.Heading4] = "h4";
            this.Map[FormatStyleValue.Heading5] = "h5";
            this.Map[FormatStyleValue.Heading6] = "h6";
            this.Map[FormatStyleValue.Head] = "head";
            this.Map[FormatStyleValue.HorizontalRule] = "hr";
            this.Map[FormatStyleValue.Html] = "html";
            this.Map[FormatStyleValue.ItalicText] = "i";
            this.Map[FormatStyleValue.Image] = "img";
            this.Map[FormatStyleValue.LabelText] = "label";
            this.Map[FormatStyleValue.Legend] = "legend";
            this.Map[FormatStyleValue.ListItem] = "li";
            this.Map[FormatStyleValue.OrderedList] = "ol";
            this.Map[FormatStyleValue.Paragraph] = "p";
            this.Map[FormatStyleValue.PreformattedText] = "pre";
            this.Map[FormatStyleValue.Quotation] = "q";
            this.Map[FormatStyleValue.IncorrectText] = "s";
            this.Map[FormatStyleValue.Sample] = "samp";
            this.Map[FormatStyleValue.Select] = "select";
            this.Map[FormatStyleValue.SmallText] = "small";
            this.Map[FormatStyleValue.Span] = "span";
            this.Map[FormatStyleValue.StrikeThroughText] = "strike";
            this.Map[FormatStyleValue.Strong] = "strong";
            this.Map[FormatStyleValue.Subscript] = "sub";
            this.Map[FormatStyleValue.Superscript] = "sup";
            this.Map[FormatStyleValue.Table] = "table";
            this.Map[FormatStyleValue.TableBody] = "tbody";
            this.Map[FormatStyleValue.TableDataCell] = "td";
            this.Map[FormatStyleValue.TableFooter] = "tfoot";
            this.Map[FormatStyleValue.TableHeaderCell] = "th";
            this.Map[FormatStyleValue.TableHeader] = "thead";
            this.Map[FormatStyleValue.Title] = "title";
            this.Map[FormatStyleValue.TableRow] = "tr";
            this.Map[FormatStyleValue.Teletype] = "tt";
            this.Map[FormatStyleValue.UnderlinedText] = "u";
            this.Map[FormatStyleValue.UnorderedList] = "ul";
        }
    }
}
