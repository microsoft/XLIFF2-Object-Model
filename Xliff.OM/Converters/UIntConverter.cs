namespace Localization.Xliff.OM.Converters
{
    /// <summary>
    /// Converts an unsigned integer to a string, and back.
    /// </summary>
    /// <seealso cref="IValueConverter"/>
    internal class UIntConverter : IValueConverter
    {
        #region IValueConverter Implementation
        /// <summary>
        /// Converts an unsigned integer to a string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The string representation of the value that is used by XLIFF.</returns>
        string IValueConverter.Convert(object value)
        {
            ArgValidator.Create(value, "value").IsNotNull().IsOfType(typeof(uint));

            return value.ToString();
        }

        /// <summary>
        /// Converts a string to an unsigned integer.
        /// </summary>
        /// <param name="value">The XLIFF string to convert.</param>
        /// <returns>The unsigned integer value of the string.</returns>
        object IValueConverter.ConvertBack(string value)
        {
            ArgValidator.Create(value, "value").IsNotNullOrWhitespace();

            return uint.Parse(value);
        }
        #endregion IValueConverter Implementation
    }
}
