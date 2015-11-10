namespace Localization.Xliff.OM.Extensibility.Tests
{
    using System.Collections.Generic;

    /// <summary>
    /// This class represents a custom element used by a custom extension.
    /// </summary>
    public class CustomElement1 : XliffElement, IExtensible
    {
        private readonly List<ElementInfo> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomElement1"/> class.
        /// </summary>
        /// <param name="prefix">The Xml prefix used when writing the data in this class to a file.</param>
        /// <param name="ns">The namespace of the data associated with this class.</param>
        public CustomElement1(string prefix, string ns)
        {
            this.Extensions = new List<IExtension>();
            this.Namespace = ns;
            this.Prefix = prefix;

            this.children = new List<ElementInfo>();
        }

        #region Properties
        /// <summary>
        /// Gets or sets a custom attribute.
        /// </summary>
        public string Attribute1
        {
            get { return (string)this.GetPropertyValue("Attribute1"); }
            set { this.SetPropertyValue(value, "Attribute1"); }
        }

        /// <summary>
        /// Gets or sets a custom element.
        /// </summary>
        public CustomElement1 Element1 { get; set; }

        /// <summary>
        /// Gets or sets a custom element.
        /// </summary>
        public CustomElement2 Element2 { get; set; }

        /// <summary>
        /// Gets the list of registered extensions on the object.
        /// </summary>
        public IList<IExtension> Extensions { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get
            {
                return (this.Element1 != null) || (this.Element2 != null) || (this.children.Count > 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        public bool HasExtensions
        {
            get { return this.Extensions.Count > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has text.
        /// </summary>
        protected override bool HasInnerText
        {
            get
            {
                return this.Text != null;
            }
        }

        /// <summary>
        /// Gets the namespace associated with this element.
        /// </summary>
        protected string Namespace { get; private set; }
        
        /// <summary>
        /// Gets the Xml prefix associated with this element.
        /// </summary>
        protected string Prefix { get; private set; }

        /// <summary>
        /// Gets a value indicating whether attribute extensions are supported by the object.
        /// </summary>
        public bool SupportsAttributeExtensions
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether element extensions are supported by the object.
        /// </summary>
        public bool SupportsElementExtensions
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the text associated with this element.
        /// </summary>
        public string Text { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new <see cref="XliffElement"/> depending on the XLIFF element Name.
        /// </summary>
        /// <param name="name">The XLIFF element Name.</param>
        /// <returns>An instance of a class associated with the specified XLIFF Name.</returns>
        public override XliffElement CreateElement(XmlNameInfo name)
        {
            XliffElement result;

            result = null;
            if ((name.Namespace == this.Namespace) && (name.LocalName == "element1"))
            {
                result = new CustomElement1(this.Prefix, this.Namespace);
                ((CustomElement1)result).Initialize();
            }
            else if ((name.Namespace == this.Namespace) && (name.LocalName == "element2"))
            {
                result = new CustomElement2(this.Prefix, this.Namespace);
                ((CustomElement2)result).Initialize();
            }
            else
            {
                return new GenericElement();
            }

            return result;
        }

        /// <summary>
        /// Gets the child <see cref="XliffElement"/>s contained within this object.
        /// </summary>
        /// <returns>A list of child elements stored as a pair consisting of the XLIFF Name for the child and
        /// the child itself.</returns>
        protected override List<ElementInfo> GetChildren()
        {
            List<ElementInfo> result;

            result = new List<ElementInfo>();
            if (this.Element1 != null)
            {
                result.Add(new ElementInfo(new XmlNameInfo(this.Prefix, this.Namespace, "element1"), this.Element1));
            }

            if (this.Element2 != null)
            {
                result.Add(new ElementInfo(new XmlNameInfo(this.Prefix, this.Namespace, "element2"), this.Element2));
            }

            result.AddRange(this.children);

            return result;
        }

        /// <summary>
        /// Gets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <returns>The text within the <see cref="XliffElement"/>.</returns>
        protected override string GetInnerText()
        {
            return this.Text;
        }

        /// <summary>
        /// Initializes the class by registering attributes.
        /// </summary>
        public virtual void Initialize()
        {
            this.RegisterAttribute(this.Namespace, "attribute1", "Attribute1", null);
            this.RegisterAttribute(this.Namespace, "attribute2", "Attribute2", null);
        }

        /// <summary>
        /// Sets the text associated with the object.
        /// </summary>
        /// <param name="text">The text to set.</param>
        protected override void SetInnerText(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// Stores the <see cref="XliffElement"/> as a child of this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>True if the child was stored, otherwise false.</returns>
        protected override bool StoreChild(ElementInfo child)
        {
            if (child.Element is CustomElement2)
            {
                this.Element2 = (CustomElement2)child.Element;
            }
            else if (child.Element is CustomElement1)
            {
                this.Element1 = (CustomElement1)child.Element;
            }
            else
            {
                this.children.Add(child);
            }

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
            bool result;

            result = false;
            if (name.Namespace == this.Namespace)
            {
                if (name.LocalName == "attribute1")
                {
                    this.Attribute1 = value;
                    result = true;
                }
                else
                {
                    this.SetPropertyValue(value, "Attribute2");
                    result = true;
                }
            }

            return result;
        }
        #endregion Methods
    }
}
