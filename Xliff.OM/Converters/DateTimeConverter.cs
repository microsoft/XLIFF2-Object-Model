namespace Localization.Xliff.OM.Converters
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Converts a DateTime value to an XLIFF string, and back.
    /// </summary>
    /// <seealso cref="IValueConverter"/>
    internal class DateTimeConverter : IValueConverter
    {
        #region IValueConverter Implementation
        /// <summary>
        /// Converts a DateTime to a string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The string representation of the value that is used by XLIFF.</returns>
        string IValueConverter.Convert(object value)
        {
            ArgValidator.Create(value, "value").IsNotNull().IsOfType(typeof(DateTime));

            return ((DateTime)value).ToString("O");
        }

        /// <summary>
        /// Converts a string to a DateTime.
        /// </summary>
        /// <param name="value">The XLIFF string to convert.</param>
        /// <returns>The integer value of the string.</returns>
        object IValueConverter.ConvertBack(string value)
        {
            ArgValidator.Create(value, "value").IsNotNullOrWhitespace();

            return DateTime.Parse(value, null, DateTimeStyles.RoundtripKind);
        }
        #endregion IValueConverter Implementation
    }
}
