namespace Localization.Xliff.OM.Extensibility
{
    using System.Linq;

    /// <summary>
    /// This class represents a generic extension handler that stores extension information in a generic fashion.
    /// </summary>
    /// <seealso cref="IExtensionHandler"/>
    public class GenericExtensionHandler : IExtensionHandler
    {
        /// <summary>
        /// The name associated with this handler.
        /// </summary>
        private const string ExtensionName = "GenericExtensionHandler";

        /// <summary>
        /// Creates an extension member that stores the attribute data that can later be stored in an extension.
        /// </summary>
        /// <param name="nameInfo">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>An extension member that stores the attribute data.</returns>
        public IExtensionAttribute CreateAttribute(IExtensionNameInfo nameInfo, string value)
        {
            return new GenericExtensionAttribute(nameInfo, value);
        }

        /// <summary>
        /// Creates an extension member that stores the element data that can later be stored in an extension.
        /// </summary>
        /// <param name="nameInfo">The name of the element.</param>
        /// <returns>An extension member that stores the element data.</returns>
        public XliffElement CreateElement(IExtensionNameInfo nameInfo)
        {
            return new GenericElement();
        }

        /// <summary>
        /// Stores the attribute in an extension.
        /// </summary>
        /// <param name="extensible">The object that is being extended.</param>
        /// <param name="attribute">The attribute to store.</param>
        /// <returns>This method always returns true.</returns>
        public bool StoreAttribute(IExtensible extensible, IExtensionAttribute attribute)
        {
            GenericExtension extension;

            extension = extensible.Extensions.FirstOrDefault((e) => e.Name == GenericExtensionHandler.ExtensionName) as GenericExtension;
            if (extension == null)
            {
                extension = new GenericExtension(GenericExtensionHandler.ExtensionName);
                extensible.Extensions.Add(extension);
            }

            extension.AddAttribute(attribute);

            return true;
        }

        /// <summary>
        /// Stores the element in an extension.
        /// </summary>
        /// <param name="extensible">The object that is being extended.</param>
        /// <param name="element">The element to store.</param>
        /// <returns>This method always returns true.</returns>
        public bool StoreElement(IExtensible extensible, ElementInfo element)
        {
            GenericExtension extension;

            extension = extensible.Extensions.FirstOrDefault((e) => e.Name == GenericExtensionHandler.ExtensionName) as GenericExtension;
            if (extension == null)
            {
                extension = new GenericExtension(GenericExtensionHandler.ExtensionName);
                extensible.Extensions.Add(extension);
            }

            Utilities.SetParent(element.Element, extensible as XliffElement);
            extension.AddChild(element);

            return true;
        }
    }
}
