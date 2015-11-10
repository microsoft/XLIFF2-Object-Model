namespace Localization.Xliff.OM.Extensibility.Tests
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class is used to store data that represents children of <see cref="XliffElement"/>s.
    /// </summary>
    internal class XliffData : XmlNameInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XliffData"/> class.
        /// </summary>
        /// <param name="prefix">The Xml prefix associated with the element.</param>
        /// <param name="ns">The namespace associated with the element.</param>
        /// <param name="localName">The local name associated with the element.</param>
        /// <param name="type">The type of the element represented by this class.</param>
        public XliffData(string prefix, string ns, string localName, Type type)
            : base(prefix, ns, localName)
        {
            this.Attributes = new List<AttributeData>();
            this.Children = new XliffDataList();
            this.Type = type;
        }

        /// <summary>
        /// Gets the list of attributes associated with the element.
        /// </summary>
        public List<AttributeData> Attributes { get; private set; }

        /// <summary>
        /// Gets the list of children associated with the element.
        /// </summary>
        public XliffDataList Children { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the element contains children.
        /// </summary>
        public bool HasChildren
        {
            get { return (this.Children != null) && (this.Children.Count > 0); }
        }

        /// <summary>
        /// Gets a value indicating whether the element contains inner text.
        /// </summary>
        public bool HasText
        {
            get { return this.Text != null; }
        }

        /// <summary>
        /// Gets or sets the inner text associated with the element.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets the type associated with this class.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Adds an attribute.
        /// </summary>
        /// <param name="ns">The namespace of the attribute.</param>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public void AddAttribute(string ns, string localName, object value)
        {
            this.AddAttribute(ns, localName, value, true);
        }

        /// <summary>
        /// Adds an attribute.
        /// </summary>
        /// <param name="ns">The namespace of the attribute.</param>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <param name="hasValue">True if the attribute has a value, otherwise false.</param>
        public AttributeData AddAttribute(string ns, string localName, object value, bool hasValue)
        {
            return this.AddAttribute(null, ns, localName, value, hasValue);
        }

        /// <summary>
        /// Adds an attribute.
        /// </summary>
        /// <param name="prefix">The Xml prefix of the attribute.</param>
        /// <param name="ns">The namespace of the attribute.</param>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <param name="hasValue">True if the attribute has a value, otherwise false.</param>
        public AttributeData AddAttribute(string prefix, string ns, string localName, object value, bool hasValue)
        {
            AttributeData data;
            XmlNameInfo name;

            name = new XmlNameInfo(prefix, ns, localName);
            data = new AttributeData(this.Type.Name, name, false, value, hasValue);
            this.Attributes.Add(data);

            return data;
        }
    }
}
