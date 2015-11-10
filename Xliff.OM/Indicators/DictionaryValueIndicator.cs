namespace Localization.Xliff.OM.Indicators
{
    using System.Collections.Generic;

    /// <summary>
    /// This class is used to determine if a dictionary has any values.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="IHasValueIndicator"/>
    internal class DictionaryValueIndicator<TKey, TValue> : IHasValueIndicator
    {
        /// <summary>
        /// This method returns true of the specified value is a dictionary and that dictionary has at least one
        /// item in it.
        /// </summary>
        /// <param name="value">The value to inspect.</param>
        /// <returns>True if the value is a dictionary with at least one item, otherwise false.</returns>
        public bool HasValue(object value)
        {
            IDictionary<TKey, TValue> dictionary;

            dictionary = value as IDictionary<TKey, TValue>;
            return (dictionary != null) && (dictionary.Count > 0);
        }
    }
}
