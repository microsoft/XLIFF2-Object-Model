namespace Localization.Xliff.OM
{
    /// <summary>
    /// Defines the result of trying to set an attribute value.
    /// </summary>
    internal enum SetAttributeResult
    {
        /// <summary>
        /// The specified attribute is invalid.
        /// </summary>
        InvalidAttribute,

        /// <summary>
        /// The specified attribute is not valid for a core or module element but may be stored as an extension.
        /// </summary>
        PossibleExtension,

        /// <summary>
        /// The name is recognized and reserved for other purposes than as an attribute.
        /// </summary>
        ReservedName,

        /// <summary>
        /// The attribute was updated successfully.
        /// </summary>
        Success
    }
}
