namespace Localization.Xliff.OM.Extensibility
{
    /// <summary>
    /// This interface defines a handler to storing non-native XLIFF elements and attributes as extension data.
    /// </summary>
    public interface IExtensionHandler
    {
        /// <summary>
        /// Creates an extension member that stores the attribute data that can later be stored in an extension.
        /// </summary>
        /// <param name="nameInfo">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>An extension member that stores the attribute data.</returns>
        IExtensionAttribute CreateAttribute(IExtensionNameInfo nameInfo, string value);

        /// <summary>
        /// Creates an extension member that stores the element data that can later be stored in an extension.
        /// </summary>
        /// <param name="nameInfo">The name of the element.</param>
        /// <returns>An extension member that stores the element data.</returns>
        XliffElement CreateElement(IExtensionNameInfo nameInfo);

        /// <summary>
        /// Stores the attribute in an extension.
        /// </summary>
        /// <param name="extensible">The object that is being extended.</param>
        /// <param name="attribute">The attribute to store.</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        bool StoreAttribute(IExtensible extensible, IExtensionAttribute attribute);

        /// <summary>
        /// Stores the element in an extension.
        /// </summary>
        /// <param name="extensible">The object that is being extended.</param>
        /// <param name="element">The element to store.</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        bool StoreElement(IExtensible extensible, ElementInfo element);
    }
}
