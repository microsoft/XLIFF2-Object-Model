namespace Localization.Xliff.OM.Converters
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base class used by converters to convert values from native types to XLIFF strings and back.
    /// </summary>
    /// <typeparam name="TData">The native type of data that is being converted to XLIFF strings.</typeparam>
    /// <seealso cref="IValueConverter"/>
    internal abstract class ConverterBase<TData> : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterBase{TData}"/> class.
        /// </summary>
        public ConverterBase()
        {
            this.Map = new Dictionary<TData, string>();
        }

        #region Properties
        /// <summary>
        /// Gets a dictionary that maps the native type values to strings.
        /// </summary>
        protected Dictionary<TData, string> Map { get; private set; }
        #endregion Properties

        #region IValueConverter Implementation
        /// <summary>
        /// Converts a value from a native type to a string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The string representation of the value that is used by XLIFF.</returns>
        public string Convert(object value)
        {
            string result;

            ArgValidator.Create(value, "value").IsNotNull().IsOfType(typeof(TData));

            if (!this.Map.TryGetValue((TData)value, out result))
            {
                string message;

                message = string.Format(
                                        Properties.Resources.ConverterBase_ValueNotImplemented_Format,
                                        value,
                                        this.GetType().Name);
                throw new NotImplementedException(message);
            }

            return result;
        }

        /// <summary>
        /// Converts an XLIFF string to a native type.
        /// </summary>
        /// <param name="value">The XLIFF string to convert.</param>
        /// <returns>The native type of the data.</returns>
        public object ConvertBack(string value)
        {
            bool found;
            TData result;

            found = false;
            result = default(TData);

            foreach (TData key in this.Map.Keys)
            {
                if (this.Map[key] == value)
                {
                    found = true;
                    result = key;
                    break;
                }
            }

            if (!found)
            {
                string message;

                message = string.Format(
                                        Properties.Resources.ConverterBase_ValueNotImplemented_Format,
                                        value,
                                        this.GetType().Name);
                throw new NotSupportedException(message);
            }

            return result;
        }
        #endregion IValueConverter Implementation
    }
}
