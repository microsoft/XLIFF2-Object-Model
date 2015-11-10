namespace Localization.Xliff.Samples
{
    using System.Linq;
    using Localization.Xliff.OM;
    using Localization.Xliff.OM.Extensibility;

    /// <summary>
    /// This class represents a test extension handler that stores extension information.
    /// </summary>
    public class TestExtensionHandler : IExtensionHandler
    {
        /// <summary>
        /// Creates an extension member that stores the attribute data that can later be stored in an extension.
        /// </summary>
        /// <param name="nameInfo">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>An extension member that stores the attribute data.</returns>
        public IExtensionAttribute CreateAttribute(IExtensionNameInfo nameInfo, string value)
        {
            return new TestAttribute(nameInfo, value);
        }

        /// <summary>
        /// Creates an extension member that stores the element data that can later be stored in an extension.
        /// </summary>
        /// <param name="nameInfo">The name of the element.</param>
        /// <returns>An extension member that stores the element data.</returns>
        public XliffElement CreateElement(IExtensionNameInfo nameInfo)
        {
            return new TestElement();
        }

        /// <summary>
        /// Stores the attribute in an extension.
        /// </summary>
        /// <param name="extensible">The object that is being extended.</param>
        /// <param name="attribute">The attribute to store.</param>
        /// <returns>This method always returns true.</returns>
        public bool StoreAttribute(IExtensible extensible, IExtensionAttribute attribute)
        {
            TestExtension extension;

            extension = extensible.Extensions.FirstOrDefault((e) => e.Name == TestExtension.ExtensionName) as TestExtension;
            if (extension == null)
            {
                extension = new TestExtension();
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
            TestExtension extension;

            extension = extensible.Extensions.FirstOrDefault((e) => e.Name == TestExtension.ExtensionName) as TestExtension;
            if (extension == null)
            {
                extension = new TestExtension();
                extensible.Extensions.Add(extension);
            }

            extension.AddChild(element);

            return true;
        }
    }
}
