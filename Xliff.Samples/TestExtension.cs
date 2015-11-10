namespace Localization.Xliff.Samples
{
    using System.Collections.Generic;
    using Localization.Xliff.OM;
    using Localization.Xliff.OM.Extensibility;

    /// <summary>
    /// This class stores test extension information that is registered on extensible objects.
    /// </summary>
    public class TestExtension : IExtension
    {
        /// <summary>
        /// The name associated with this handler.
        /// </summary>
        public const string ExtensionName = "TestExtensionHandler";

        /// <summary>
        /// The list of attribute members.
        /// </summary>
        private List<IExtensionAttribute> attributes;

        /// <summary>
        /// The list of element and text data members.
        /// </summary>
        private List<ElementInfo> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExtension"/> class.
        /// </summary>
        public TestExtension()
        {
            this.attributes = new List<IExtensionAttribute>();
            this.children = new List<ElementInfo>();
        }

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the data contains attribute members.
        /// </summary>
        public bool HasAttributes
        {
            get { return this.attributes.Count > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the data contains element or text members.
        /// </summary>
        public bool HasChildren
        {
            get { return this.children.Count > 0; }
        }

        /// <summary>
        /// Gets the name of the extension.
        /// </summary>
        public string Name
        {
            get { return TestExtension.ExtensionName; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Adds an attribute member to the extension data.
        /// </summary>
        /// <param name="attribute">The attribute to add.</param>
        public void AddAttribute(IExtensionAttribute attribute)
        {
            this.attributes.Add(attribute);
        }

        /// <summary>
        /// Adds an element or text member to the extension data.
        /// </summary>
        /// <param name="child">The child to add.</param>
        public void AddChild(ElementInfo child)
        {
            this.children.Add(child);
        }

        /// <summary>
        /// Gets the attribute members.
        /// </summary>
        /// <returns>An enumeration of attribute members.</returns>
        public IEnumerable<IExtensionAttribute> GetAttributes()
        {
            return this.attributes;
        }

        /// <summary>
        /// Gets the element and text members.
        /// </summary>
        /// <returns>An enumeration of element and text members.</returns>
        public IEnumerable<ElementInfo> GetChildren()
        {
            return this.children;
        }
        #endregion Methods
    }
}
