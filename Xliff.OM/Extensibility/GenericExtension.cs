namespace Localization.Xliff.OM.Extensibility
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class stores extension information that is registered on extensible objects.
    /// </summary>
    /// <seealso cref="IExtension"/>
    public class GenericExtension : IExtension
    {
        /// <summary>
        /// The list of attribute members.
        /// </summary>
        private Lazy<List<IExtensionAttribute>> attributes;

        /// <summary>
        /// The list of element and text data members.
        /// </summary>
        private Lazy<List<ElementInfo>> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericExtension"/> class.
        /// </summary>
        /// <param name="name">The name of the extension.</param>
        public GenericExtension(string name)
        {
            ArgValidator.Create(name, "name").IsNotNullOrWhitespace();

            this.attributes = new Lazy<List<IExtensionAttribute>>();
            this.children = new Lazy<List<ElementInfo>>();
            this.Name = name;
        }

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the data contains attribute members.
        /// </summary>
        public bool HasAttributes
        {
            get { return this.attributes.IsValueCreated && (this.attributes.Value.Count > 0); }
        }

        /// <summary>
        /// Gets a value indicating whether the data contains element or text members.
        /// </summary>
        public bool HasChildren
        {
            get { return this.children.IsValueCreated && (this.children.Value.Count > 0); }
        }

        /// <summary>
        /// Gets the name of the extension.
        /// </summary>
        public string Name { get; private set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Adds an attribute member to the extension data.
        /// </summary>
        /// <param name="attribute">The attribute to add.</param>
        public void AddAttribute(IExtensionAttribute attribute)
        {
            ArgValidator.Create(attribute, "attribute").IsNotNull();

            this.attributes.Value.Add(attribute);
        }

        /// <summary>
        /// Adds an element or text member to the extension data.
        /// </summary>
        /// <param name="child">The child to add.</param>
        public void AddChild(ElementInfo child)
        {
            ArgValidator.Create(child, "child").IsNotNull();

            this.children.Value.Add(child);
        }

        /// <summary>
        /// Gets the attribute members.
        /// </summary>
        /// <returns>An enumeration of attribute members.</returns>
        public IEnumerable<IExtensionAttribute> GetAttributes()
        {
            return this.attributes.Value;
        }

        /// <summary>
        /// Gets the element and text members.
        /// </summary>
        /// <returns>An enumeration of element and text members.</returns>
        public IEnumerable<ElementInfo> GetChildren()
        {
            return this.children.Value;
        }
        #endregion Methods
    }
}
