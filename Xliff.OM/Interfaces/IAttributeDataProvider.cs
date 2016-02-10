namespace Localization.Xliff.OM
{
    using System.Collections.Generic;

    /// <summary>
    /// This interface provides access to attribute data for the purpose of serialization and deserialization.
    /// </summary>
    internal interface IAttributeDataProvider
    {
        #region Properties
        /// <summary>
        /// Gets the list of attributes that, when written, must be accompanied by this attribute.
        /// </summary>
        List<IAttributeDataProvider> ExplicitOutputDependencies { get; }

        /// <summary>
        /// Gets a value indicating whether the attribute has a value.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Gets a value indicating whether the attribute value is the default value.
        /// </summary>
        bool IsDefaultValue { get; }

        /// <summary>
        /// Gets a value indicating whether the attribute is optional.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        /// Gets the local name of the Xml fragment used to read/write the attribute.
        /// </summary>
        string LocalName { get; }

        /// <summary>
        /// Gets the namespace of the Xml fragment used to read/write the attribute.
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Gets a resolver that determines whether properties need to be written to an XLIFF file.
        /// </summary>
        IOutputResolver OutputResolver { get; }

        /// <summary>
        /// Gets the prefix of the Xml fragment used to read/write the attribute.
        /// </summary>
        string Prefix { get; }

        /// <summary>
        /// Gets the raw value of the attribute.
        /// </summary>
        object Value { get; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Gets the string representation of the attribute.
        /// </summary>
        /// <returns>The string representation of the attribute.</returns>
        string GetStringValue();

        /// <summary>
        /// Gets the string representation of the attribute if it were to be inherited. The actual value of the
        /// attribute is ignored and only the inherited value is returned.
        /// </summary>
        /// <param name="value">The inherited value of the attribute.</param>
        /// <returns>True if an inherited value exists and is returned, false if there is nothing to inherit from.
        /// </returns>
        bool TryGetInheritedStringValue(out string value);
        #endregion Methods
    }
}
