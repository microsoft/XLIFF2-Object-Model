namespace Localization.Xliff.OM
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Localization.Xliff.OM.Converters;

    /// <summary>
    /// This class stores data about XLIFF attributes. This is used to store default values, current values, get
    /// XLIFF string values, etc. Its purpose is to make it easy to transform from the easy-to-use object model
    /// to the XLIFF file format.
    /// </summary>
    /// <seealso cref="XmlNameInfo"/>
    internal class AttributeData : XmlNameInfo
    {
        #region Member Variables
        /// <summary>
        /// The initial value that was given to an attribute.
        /// </summary>
        private object defaultValue;

        /// <summary>
        /// Indicates whether a default value was set.
        /// </summary>
        private bool hasDefaultValue;

        /// <summary>
        /// Indicates whether a value was set.
        /// </summary>
        private bool hasValue;

        /// <summary>
        /// The type name of the host that contains the attribute.
        /// </summary>
        private string hostTypeName;

        /// <summary>
        /// The native value that is exposed through the object model.
        /// </summary>
        private object value;
        #endregion Member Variables

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeData"/> class without a default value.
        /// </summary>
        /// <param name="hostTypeName">The type name of the host that contains the attribute.</param>
        /// <param name="name">The name of the XLIFF attribute.</param>
        /// <param name="isOptional">Indicates whether the attribute is optional in the XLIFF document.</param>
        public AttributeData(string hostTypeName, XmlNameInfo name, bool isOptional)
            : this(hostTypeName, name, isOptional, null, false)
        {
            this.hasDefaultValue = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeData"/> class.
        /// </summary>
        /// <param name="hostTypeName">The type name of the host that contains the attribute.</param>
        /// <param name="name">The name of the XLIFF attribute.</param>
        /// <param name="isOptional">Indicates whether the attribute is optional in the XLIFF document.</param>
        /// <param name="value">The default value of the attribute.</param>
        public AttributeData(string hostTypeName, XmlNameInfo name, bool isOptional, object value)
            : this(hostTypeName, name, isOptional, value, !isOptional)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeData"/> class.
        /// </summary>
        /// <param name="hostTypeName">The type name of the host that contains the attribute.</param>
        /// <param name="name">The name of the XLIFF attribute.</param>
        /// <param name="isOptional">Indicates whether the attribute is optional in the XLIFF document.</param>
        /// <param name="value">The default value of the attribute.</param>
        /// <param name="hasValue">A value indicating whether the attribute has a value.</param>
        public AttributeData(string hostTypeName, XmlNameInfo name, bool isOptional, object value, bool hasValue)
            : base(name)
        {
            this.ExplicitOutputDependencies = new Dictionary<string, AttributeData>();
            this.InheritanceList = new List<InheritanceInfo>();
            this.IsOptional = isOptional;
            this.IsSupported = true;
            this.Value = value;

            // This must go last because Value sets it.
            this.HasValue = hasValue;

            this.defaultValue = value;
            this.hasDefaultValue = true;
            this.hostTypeName = hostTypeName;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the type Name of the converter to use to convert from the attribute type to an XLIFF string.
        /// </summary>
        internal string ConverterTypeName { get; set; }

        /// <summary>
        /// Gets the list of attributes that, when written, must be accompanied by this attribute. The key is the
        /// name of the property on the object; the value is the attribute backing the property.
        /// </summary>
        public Dictionary<string, AttributeData> ExplicitOutputDependencies { get; private set; }

        /// <summary>
        /// Gets or sets a resolver that determines whether properties need to be written to an XLIFF file.
        /// </summary>
        public IOutputResolver OutputResolver { get; set; }

        /// <summary>
        /// Gets a value indicating whether the attribute has a value.
        /// </summary>
        public bool HasValue
        {
            get
            {
                return (this.HasValueIndicator != null) ? this.HasValueIndicator.HasValue(this.Value) : this.hasValue;
            }

            private set
            {
                this.hasValue = value;
            }
        }

        /// <summary>
        /// Gets or sets an object that can be called to indicate whether the value really has a value. This is useful for types
        /// like dictionaries where the dictionary itself doesn't change but the items in the dictionary do.
        /// </summary>
        internal IHasValueIndicator HasValueIndicator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore namespace information while storing the value for this attribute.
        /// </summary>
        internal bool IgnoreNamespace { get; set; }

        /// <summary>
        /// Gets the list of items that define how a value is inherited.
        /// </summary>
        public List<InheritanceInfo> InheritanceList { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property value can be inherited if one is not present.
        /// </summary>
        public bool InheritValue { get; set; }

        /// <summary>
        /// Gets a value indicating whether the attribute value is the default value.
        /// </summary>
        public bool IsDefaultValue
        {
            get { return this.hasDefaultValue && object.Equals(this.Value, this.defaultValue); }
        }

        /// <summary>
        /// Gets a value indicating whether the attribute is optional.
        /// </summary>
        public bool IsOptional { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is supported by the host object.
        /// </summary>
        public bool IsSupported { get; set; }

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        public object Value
        {
            get
            {
                this.VerifySupported();
                return this.value;
            }

            set
            {
                this.VerifySupported();
                this.value = value;
                this.HasValue = (this.value != null) || (this.defaultValue != null);
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Gets the converter associated with this attribute.
        /// </summary>
        /// <returns>An instance of the converter that converts from a type to an XLIFF string value.</returns>
        private IValueConverter GetConverter()
        {
            IValueConverter converter;

            converter = Reflector.GetSingletonViaActivator(this.ConverterTypeName) as IValueConverter;
            Debug.Assert(converter != null, "Converter doesn't implement IValueConverter");

            return converter;
        }

        /// <summary>
        /// Gets the string representation of the <see cref="Value"/>.
        /// </summary>
        /// <returns>The string representation of the value.</returns>
        /// <remarks>If a converter type is defined, the type is activated and invoked to convert from the
        /// native type to a string.</remarks>
        public string GetStringValue()
        {
            return this.GetStringValue(this.Value);
        }

        /// <summary>
        /// Gets the string representation of a value.
        /// </summary>
        /// <param name="value">The value whose string representation to get.</param>
        /// <returns>The string representation of the value.</returns>
        /// <remarks>If a converter type is defined, the type is activated and invoked to convert from the
        /// native type to a string.</remarks>
        public string GetStringValue(object value)
        {
            string result;

            if ((value == null) || (value.GetType() == typeof(string)))
            {
                result = value as string;
            }
            else if (this.ConverterTypeName != null)
            {
                result = this.GetConverter().Convert(value);
            }
            else
            {
                result = value.ToString();
            }

            return result;
        }

        /// <summary>
        /// Sets the value of the attribute from the XLIFF string value.
        /// </summary>
        /// <param name="value">The XLIFF value.</param>
        internal void SetValue(string value)
        {
            if (this.ConverterTypeName != null)
            {
                this.Value = this.GetConverter().ConvertBack(value);
            }
            else
            {
                this.Value = (string)value;
            }
        }

        /// <summary>
        /// Checks if the attribute is supported, and if not throws a NotSupportedException. If the attribute is
        /// supported, this method will just return.
        /// </summary>
        private void VerifySupported()
        {
            if (!this.IsSupported)
            {
                string message;

                if (string.IsNullOrEmpty(this.Prefix))
                {
                    message = this.LocalName;
                }
                else
                {
                    message = string.Join(":", this.Prefix, this.LocalName);
                }

                message = string.Format(
                                        Properties.Resources.XliffReader_InvalidAttributeName_Format,
                                        message,
                                        this.hostTypeName);
                throw new NotSupportedException(message);
            }
        }
        #endregion Methods
    }
}
