namespace Localization.Xliff.OM.Converters
{
    /// <summary>
    /// Converts a float to an string, and back.
    /// </summary>
    /// <seealso cref="IValueConverter"/>
    internal class FloatConverter : IValueConverter
    {
        #region IValueConverter Implementation
        /// <summary>
        /// Converts a float to a string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The string representation of the value that is used by XLIFF.</returns>
        string IValueConverter.Convert(object value)
        {
            ArgValidator.Create(value, "value").IsNotNull().IsOfType(typeof(float));

            return value.ToString();
        }

        /// <summary>
        /// Converts a string to a float.
        /// </summary>
        /// <param name="value">The XLIFF string to convert.</param>
        /// <returns>The float value of the string.</returns>
        object IValueConverter.ConvertBack(string value)
        {
            ArgValidator.Create(value, "value").IsNotNullOrWhitespace();

            return float.Parse(value);
        }
        #endregion IValueConverter Implementation
    }
}
