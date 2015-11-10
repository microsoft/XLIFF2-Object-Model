namespace Localization.Xliff.OM.Converters
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Converts an integer to an hexidecimal string, and back.
    /// </summary>
    /// <seealso cref="IValueConverter"/>
    internal class HexConverter : IValueConverter
    {
        #region IValueConverter Implementation
        /// <summary>
        /// Converts an integer to a hexidecimal string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The string representation of the value that is used by XLIFF.</returns>
        string IValueConverter.Convert(object value)
        {
            ArgValidator.Create(value, "value").IsNotNull().IsOfType(typeof(int));

            return string.Format("{0:X}", value).PadLeft(4, '0');
        }

        /// <summary>
        /// Converts a hexidecimal string to an integer.
        /// </summary>
        /// <param name="value">The XLIFF string to convert.</param>
        /// <returns>The integer value of the string.</returns>
        object IValueConverter.ConvertBack(string value)
        {
            ArgValidator.Create(value, "value").IsNotNullOrWhitespace();

            return int.Parse(value, NumberStyles.HexNumber);
        }
        #endregion IValueConverter Implementation
    }
}
