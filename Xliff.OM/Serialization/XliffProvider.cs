namespace Localization.Xliff.OM.Serialization
{
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class is used as a container for the XLIFF document during serialization.
    /// </summary>
    /// <seealso cref="XliffElement"/>
    [SchemaChild(NamespaceValues.Core, ElementNames.Root, typeof(XliffDocument))]
    internal sealed class XliffProvider : XliffElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XliffProvider"/> class.
        /// </summary>
        public XliffProvider()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
        }

        /// <summary>
        /// Gets or sets the XLIFF document.
        /// </summary>
        public XliffDocument Document { get; set; }

        /// <summary>
        /// Stores the <see cref="XliffElement"/> as a child of this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>True if the child was stored, otherwise false.</returns>
        protected override bool StoreChild(ElementInfo child)
        {
            bool result;

            result = true;
            if (child.Element is XliffDocument)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.Document, this.Document);
                this.Document = (XliffDocument)child.Element;
            }
            else
            {
                result = base.StoreChild(child);
            }

            return result;
        }

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="XliffProvider.Document"/> property.
            /// </summary>
            public const string Document = "Document";
        }
    }
}
