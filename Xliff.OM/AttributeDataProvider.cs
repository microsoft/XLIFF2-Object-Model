namespace Localization.Xliff.OM
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// This class provides access to attribute data for the purpose of serialization and deserialization.
    /// </summary>
    /// <seealso cref="XmlNameInfo"/>
    /// <seealso cref="IAttributeDataProvider"/>
    internal class AttributeDataProvider : XmlNameInfo, IAttributeDataProvider
    {
        #region Member Variables
        /// <summary>
        /// The underlying attribute data.
        /// </summary>
        private readonly AttributeData data;

        /// <summary>
        /// The element that hosts the attribute.
        /// </summary>
        private readonly XliffElement host;

        /// <summary>
        /// The name of the property associated with the attribute.
        /// </summary>
        private readonly string property;
        #endregion Member Variables

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDataProvider"/> class.
        /// </summary>
        /// <param name="host">The element that hosts the attribute.</param>
        /// <param name="property">The name of the property associated with the attribute.</param>
        /// <param name="data">The raw attribute data being referenced by this class.</param>
        internal AttributeDataProvider(XliffElement host, string property, AttributeData data)
            : base(data)
        {
            Debug.Assert(host != null, "Host cannot be null.");
            Debug.Assert(data != null, "Data cannot be null.");
            Debug.Assert(!string.IsNullOrEmpty(property), "Property cannot be null.");

            this.data = data;
            this.host = host;
            this.property = property;
        }

        #region Properties
        /// <summary>
        /// Gets the list of attributes that, when written, must be accompanied by this attribute.
        /// </summary>
        public List<IAttributeDataProvider> ExplicitOutputDependencies
        {
            get
            {
                List<IAttributeDataProvider> result;

                result = new List<IAttributeDataProvider>();
                foreach (KeyValuePair<string, AttributeData> data in this.data.ExplicitOutputDependencies)
                {
                    result.Add(new AttributeDataProvider(this.host, data.Key, data.Value));
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a resolver that determines whether properties need to be written to an XLIFF file.
        /// </summary>
        public IOutputResolver OutputResolver
        {
            get { return this.data.OutputResolver; }
        }

        /// <summary>
        /// Gets a value indicating whether the attribute has a value.
        /// </summary>
        public bool HasValue
        {
            get { return this.data.HasValue; }
        }

        /// <summary>
        /// Gets a value indicating whether the attribute value is the default value.
        /// </summary>
        public bool IsDefaultValue
        {
            get { return this.data.IsDefaultValue; }
        }

        /// <summary>
        /// Gets a value indicating whether the attribute is optional.
        /// </summary>
        public bool IsOptional
        {
            get { return this.data.IsOptional; }
        }

        /// <summary>
        /// Gets the raw value of the attribute.
        /// </summary>
        public object Value
        {
            get { return this.data.Value; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Gets the string representation of the attribute.
        /// </summary>
        /// <returns>The string representation of the attribute.</returns>
        public string GetStringValue()
        {
            return this.data.GetStringValue();
        }

        /// <summary>
        /// Gets the string representation of the attribute if it were to be inherited. The actual value of the
        /// attribute is ignored and only the inherited value is returned.
        /// </summary>
        /// <param name="value">The inherited value of the attribute.</param>
        /// <returns>True if an inherited value exists and is returned, false if there is nothing to inherit from.
        /// </returns>
        public bool TryGetInheritedStringValue(out string value)
        {
            bool result;
            object propertyValue;

            result = this.host.GetPropertyValue(this.property, true, true, out propertyValue);
            value = propertyValue as string;

            return result;
        }
        #endregion Methods
    }
}
