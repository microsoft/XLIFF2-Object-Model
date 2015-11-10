namespace Localization.Xliff.OM.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Converts a SubFormatStyle value to an XLIFF string, and back.
    /// </summary>
    /// <seealso cref="IValueConverter"/>
    internal class SubFormatStyleConverter : IValueConverter
    {
        #region Constants
        /// <summary>
        /// Character representing a backslash.
        /// </summary>
        private const char BackslashChar = '\\';

        /// <summary>
        /// Character representing a comma.
        /// </summary>
        private const char CommaChar = ',';

        /// <summary>
        /// String representing a backslash.
        /// </summary>
        private const string BackslashString = @"\";

        /// <summary>
        /// String representing a comma.
        /// </summary>
        private const string CommaString = @",";

        /// <summary>
        /// String representing an escaped backslash.
        /// </summary>
        private const string EscapedBackslash = @"\\";

        /// <summary>
        /// String representing an escaped Comma.
        /// </summary>
        private const string EscapedComma = @"\,";
        #endregion Constants

        /// <summary>
        /// Converts a dictionary whose key and value are strings to a string where a comma delimits the key from the
        /// value, and a backslash delimits entries.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The string representation of the value that is used by XLIFF.</returns>
        public string Convert(object value)
        {
            IDictionary<string, string> styles;
            StringBuilder builder;

            ArgValidator.Create(value, "value").IsNotNull().IsOfType(typeof(IDictionary<string, string>));

            styles = (IDictionary<string, string>)value;
            builder = new StringBuilder();

            foreach (KeyValuePair<string, string> pair in styles)
            {
                string entry;
                string entryKey;
                string entryValue;

                try
                {
                    XmlConvert.VerifyName(pair.Key);
                }
                catch (Exception e)
                {
                    string message;

                    message = string.Format(Properties.Resources.SubFormatStyleConverter_InvalidKey_Format, pair.Key);
                    throw new FormatException(message, e);
                }

                entryKey = pair.Key.Replace(
                                            SubFormatStyleConverter.BackslashString,
                                            SubFormatStyleConverter.EscapedBackslash)
                                   .Replace(
                                            SubFormatStyleConverter.CommaString,
                                            SubFormatStyleConverter.EscapedComma);

                entryValue = pair.Value.Replace(
                                                SubFormatStyleConverter.BackslashString,
                                                SubFormatStyleConverter.EscapedBackslash)
                                       .Replace(
                                                SubFormatStyleConverter.CommaString,
                                                SubFormatStyleConverter.EscapedComma);

                entry = string.Join(SubFormatStyleConverter.CommaString, entryKey, entryValue);
                builder.Append(entry + SubFormatStyleConverter.BackslashString);
            }

            // Remove the trailing backslash.
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 1, 1);
            }

            return builder.Length > 0 ? builder.ToString() : null;
        }

        /// <summary>
        /// Converts an XLIFF string to a dictionary whose key and value are strings.
        /// </summary>
        /// <param name="value">The XLIFF string to convert.</param>
        /// <returns>The native type of the data.</returns>
        public object ConvertBack(string value)
        {
            IDictionary<string, string> result;
            int start;
            int length;
            string key;

            ArgValidator.Create(value, "value").IsNotWhitespaceOnly();

            result = new Dictionary<string, string>();

            start = 0;
            length = 0;
            key = null;
            for (int i = 0; i < value.Length; i++)
            {
                char c;

                c = value[i];
                if (c == SubFormatStyleConverter.CommaChar)
                {
                    // Delimiting the key from the value.
                    if (key == null)
                    {
                        try
                        {
                            key = value.Substring(start, length);
                            XmlConvert.VerifyName(key);
                        }
                        catch (Exception e)
                        {
                            string message;

                            message = string.Format(Properties.Resources.SubFormatStyleConverter_InvalidKey_Format, key);
                            throw new FormatException(message, e);
                        }

                        start = i + 1;
                        length = 0;
                    }
                    else
                    {
                        string message;

                        message = string.Format(Properties.Resources.SubFormatStyleConverter_CommaInValue_Format, key);
                        throw new FormatException(message);
                    }
                }
                else if (c == SubFormatStyleConverter.BackslashChar)
                {
                    if ((i < (value.Length - 1)) &&
                        ((string.Format("{0}{1}", c, value[i + 1]) == SubFormatStyleConverter.EscapedBackslash) ||
                         (string.Format("{0}{1}", c, value[i + 1]) == SubFormatStyleConverter.EscapedComma)))
                    {
                        i++;
                        length += 2;
                    }
                    else if (key != null)
                    {
                        // Delimiting entries. Overwrite duplicates.
                        result[key] = value.Substring(start, length);
                        key = null;
                        start = i + 1;
                        length = 0;
                    }
                    else
                    {
                        throw new FormatException(Properties.Resources.SubFormatStyleConverter_BackslashInKey);
                    }
                }
                else
                {
                    length++;
                }
            }

            if (length > 0)
            {
                if (key == null)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.SubFormatStyleConverter_KeyWithoutValue_Format,
                                            value.Substring(start, length));
                    throw new FormatException(message);
                }
                else
                {
                    // Overwrite duplicates.
                    result[key] = value.Substring(start, length);
                }
            }

            return result;
        }
    }
}
