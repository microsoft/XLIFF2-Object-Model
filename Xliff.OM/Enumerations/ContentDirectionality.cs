namespace Localization.Xliff.OM.Core
{
    /// <summary>
    /// Defines the direction of text in content.
    /// </summary>
    public enum ContentDirectionality
    {
        /// <summary>
        /// Indicates that directionality is determined based on the first character of the string.
        /// </summary>
        Auto,

        /// <summary>
        /// Content reads left to right.
        /// </summary>
        LTR,

        /// <summary>
        /// Content reads right to left.
        /// </summary>
        RTL
    }
}