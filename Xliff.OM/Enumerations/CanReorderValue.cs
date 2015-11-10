namespace Localization.Xliff.OM.Core
{
    /// <summary>
    /// Defines the values that indicate whether or not the inline tag can be re-ordered.
    /// </summary>
    public enum CanReorderValue
    {
        /// <summary>
        /// Indicates the inline tag is the first element of a sequence that cannot be re-ordered.
        /// </summary>
        FirstNo,

        /// <summary>
        /// Indicates the inline tag cannot be re-ordered.
        /// </summary>
        No,

        /// <summary>
        /// Indicates the inline tag can be re-ordered.
        /// </summary>
        Yes
    }
}
