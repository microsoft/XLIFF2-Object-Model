namespace Localization.Xliff.OM.Converters
{
    /// <summary>
    /// Converts an integer to an string, and back.
    /// </summary>
    /// <seealso cref="IValueConverter"/>
    internal class IntConverter : IValueConverter
    {
        #region IValueConverter Implementation
        /// <summary>
        /// Converts an integer to a string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The string representation of the value that is used by XLIFF.</returns>
        string IValueConverter.Convert(object value)
        {
            ArgValidator.Create(value, "value").IsNotNull().IsOfType(typeof(int));

            return value.ToString();
        }

        /// <summary>
        /// Converts a string to an integer.
        /// </summary>
        /// <param name="value">The XLIFF string to convert.</param>
        /// <returns>The integer value of the string.</returns>
        object IValueConverter.ConvertBack(string value)
        {
            ArgValidator.Create(value, "value").IsNotNullOrWhitespace();

            return int.Parse(value);
        }
        #endregion IValueConverter Implementation
    }
}
