namespace Localization.Xliff.OM.Extensibility.Tests
{
    /// <summary>
    /// This class represents a custom element used by a custom extension.
    /// </summary>
    public class CustomElement2 : CustomElement1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomElement2"/> class.
        /// </summary>
        /// <param name="prefix">The Xml prefix used when writing the data in this class to a file.</param>
        /// <param name="ns">The namespace of the data associated with this class.</param>
        public CustomElement2(string prefix, string ns)
            : base(prefix, ns)
        {
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
                this.SetPropertyValue(value, "Attribute3");
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Initializes the class by registering attributes.
        /// </summary>
        public override void Initialize()
        {
            this.RegisterAttribute(this.Namespace, "attribute3", "Attribute3", null);
        }
    }
}
