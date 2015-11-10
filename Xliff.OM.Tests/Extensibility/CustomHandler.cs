namespace Localization.Xliff.OM.Extensibility.Tests
{
    using System.Linq;

    /// <summary>
    /// This class represents a custom handler that manages extensions.
    /// </summary>
    internal class CustomHandler : IExtensionHandler
    {
        /// <summary>
        /// The namespace associated with the handler.
        /// </summary>
        private readonly string ns;

        /// <summary>
        /// The Xml prefix of the data associated with the handler.
        /// </summary>
        private string prefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHandler"/> class.
        /// </summary>
        /// <param name="prefix">The Xml prefix of the data associated with this handler.</param>
        /// <param name="ns">The namespace associated with this handler.</param>
        public CustomHandler(string prefix, string ns)
        {
            this.ns = ns;
            this.prefix = prefix;
        }

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
            XliffElement result;

            result = null;
            if ((nameInfo.Namespace == this.ns) && (nameInfo.LocalName == "element1"))
            {
                result = new CustomElement1(this.prefix, this.ns);
                ((CustomElement1)result).Initialize();
            }
            else if ((nameInfo.Namespace == this.ns) && (nameInfo.LocalName == "element2"))
            {
                result = new CustomElement2(this.prefix, this.ns);
                ((CustomElement2)result).Initialize();
            }
            else
            {
                result = new GenericElement();
            }

            return result;
        }

        /// <summary>
        /// Stores the attribute in an extension.
        /// </summary>
        /// <param name="extensible">The object that is being extended.</param>
        /// <param name="attribute">The attribute to store.</param>
        /// <returns>This method always returns true.</returns>
        public bool StoreAttribute(IExtensible extensible, IExtensionAttribute attribute)
        {
            CustomExtension extension;

            extension = extensible.Extensions.FirstOrDefault((e) => e.Name == this.ns) as CustomExtension;
            if (extension == null)
            {
                extension = new CustomExtension(this.ns);
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
            CustomExtension extension;

            extension = extensible.Extensions.FirstOrDefault((e) => e.Name == this.ns) as CustomExtension;
            if (extension == null)
            {
                extension = new CustomExtension(this.ns);
                extensible.Extensions.Add(extension);
            }

            extension.AddChild(element);

            return true;
        }
    }
}
