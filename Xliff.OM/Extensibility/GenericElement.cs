namespace Localization.Xliff.OM.Extensibility
{
    using System;
    using System.Collections.Generic;
    using Localization.Xliff.OM;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a generic <see cref="XliffElement"/> that is used to store extensibility information
    /// that is not stored by registered extensions.
    /// </summary>
    public class GenericElement : XliffElement
    {
        /// <summary>
        /// The children contained by this object.
        /// </summary>
        private readonly Lazy<List<ElementInfo>> children;

        /// <summary>
        /// The text contained by this object.
        /// </summary>
        private string text;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericElement"/> class.
        /// </summary>
        public GenericElement()
        {
            this.children = new Lazy<List<ElementInfo>>();
        }

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this object contains children.
        /// </summary>
        protected override bool HasChildren
        {
            get
            {
                return this.children.IsValueCreated && (this.children.Value.Count > 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has text.
        /// </summary>
        protected override bool HasInnerText
        {
            get
            {
                return this.text != null;
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Adds an element to this object as a child.
        /// </summary>
        /// <param name="element">The object to add.</param>
        public void AddChild(ElementInfo element)
        {
            ArgValidator.Create(element, "element").IsNotNull();

            this.StoreChild(element);
        }

        /// <summary>
        /// Creates a new <see cref="XliffElement"/> depending on the XLIFF element Name.
        /// </summary>
        /// <param name="name">The XLIFF element Name.</param>
        /// <returns>An instance of a class associated with the specified XLIFF Name.</returns>
        public override XliffElement CreateElement(XmlNameInfo name)
        {
            return Reflector.CreateElement(name) ?? new GenericElement();
        }

        /// <summary>
        /// Gets the child <see cref="XliffElement"/>s contained within this object.
        /// </summary>
        /// <returns>A list of child elements stored as a pair consisting of the XLIFF Name for the child and
        /// the child itself.</returns>
        protected override List<ElementInfo> GetChildren()
        {
            return this.children.Value;
        }

        /// <summary>
        /// Gets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <returns>The text within the <see cref="XliffElement"/>.</returns>
        protected override string GetInnerText()
        {
            return this.text;
        }

        /// <summary>
        /// Sets the value of an attribute.
        /// </summary>
        /// <param name="prefix">The Xml prefix of the Xml attribute.</param>
        /// <param name="ns">The namespace of the Xml attribute.</param>
        /// <param name="localName">The local name of the Xml attribute.</param>
        /// <param name="value">The value to set.</param>
        public void SetAttribute(string prefix, string ns, string localName, string value)
        {
            ((IXliffDataConsumer)this).TrySetAttributeValue(new XmlNameInfo(prefix, ns, localName), value);
        }

        /// <summary>
        /// Sets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="text">The text to set.</param>
        protected override void SetInnerText(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Stores the <see cref="XliffElement"/> as a child of this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="info">The object to add.</param>
        /// <returns>True if the child was stored, otherwise false.</returns>
        protected override bool StoreChild(ElementInfo info)
        {
            ArgValidator.ParentIsNull(info.Element);
            Utilities.SetParent(info.Element, this);

            if (info.Namespace == null)
            {
                XmlNameInfo name;

                name = new XmlNameInfo(info.Prefix, NamespaceValues.Core, info.LocalName);
                info = new ElementInfo(name, info.Element);
            }

            this.children.Value.Add(info);

            return true;
        }

        /// <summary>
        /// Tries to set the value of an attribute.
        /// </summary>
        /// <param name="name">The XLIFF Name of the attribute.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>This method always returns true.</returns>
        protected override bool TrySetPropertyValue(XmlNameInfo name, string value)
        {
            XmlNameInfo nameWithoutPrefix;
            string key;

            key = string.Join("@@", name.Namespace, name.LocalName);
            this.RegisterAttribute(name.Namespace, name.LocalName, key, value);

            nameWithoutPrefix = new XmlNameInfo(name.Namespace, name.LocalName);
            this.SetPropertyValue(value, key);

            return true;
        }
        #endregion Methods
    }
}
