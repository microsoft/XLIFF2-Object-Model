namespace Localization.Xliff.OM.Converters
{
    /// <summary>
    /// This interface defines converters that convert from native types to XLIFF strings.
    /// </summary>
    internal interface IValueConverter
    {
        /// <summary>
        /// Converts a value from a native type to a string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The string representation of the value that is used by XLIFF.</returns>
        string Convert(object value);

        /// <summary>
        /// Converts an XLIFF string to a native type.
        /// </summary>
        /// <param name="value">The XLIFF string to convert.</param>
        /// <returns>The native type of the data.</returns>
        object ConvertBack(string value);
    }
}
