namespace Localization.Xliff.OM.XmlNames
{
    /// <summary>
    /// This class contains values used in Xml formatting.
    /// </summary>
    public static class Xml
    {
        /// <summary>
        /// The beginning of a CDATA fragment.
        /// </summary>
        public const string CDATABeginTag = "<![CDATA[";

        /// <summary>
        /// The ending of a CDATA fragment.
        /// </summary>
        public const string CDATAEndTag = "]]>";

        /// <summary>
        /// The beginning of a comment fragment.
        /// </summary>
        public const string CommentBeginTag = "<!--";

        /// <summary>
        /// The ending of a comment fragment.
        /// </summary>
        public const string CommentEndTag = "-->";

        /// <summary>
        /// The beginning of a processing instruction fragment.
        /// </summary>
        public const string ProcessingInstructionBeginTag = "<?";

        /// <summary>
        /// The ending of a processing instruction fragment.
        /// </summary>
        public const string ProcessingInstructionEndTag = "?>";

        /// <summary>
        /// The closing Xml fragment for an empty element.
        /// </summary>
        public const string XmlCloseTagEndStandalone = "/>";

        /// <summary>
        /// The closing Xml fragment for a non-empty element.
        /// </summary>
        public const string XmlCloseTagBeginSpanning = "</";

        /// <summary>
        /// The beginning of an opening Xml fragment.
        /// </summary>
        public const string XmlBeginTag = "<";

        /// <summary>
        /// The end of an opening Xml fragment.
        /// </summary>
        public const string XmlEndTag = ">";
    }
}
