namespace Localization.Xliff.OM
{
    /// <summary>
    /// This interface is used for XliffElements to indicate (at runtime) whether properties need to be written to the
    /// XLIFF file.
    /// </summary>
    internal interface IOutputResolver
    {
        /// <summary>
        /// Returns a value indicating whether the specified property is required to be output to the XLIFF file.
        /// </summary>
        /// <param name="property">The property being inquired about whether it needs to be output.</param>
        /// <returns>True if the property needs to be written to the XLIFF file, false if the property output is
        /// optional.
        /// </returns>
        bool IsOutputRequired(string property);
    }
}
