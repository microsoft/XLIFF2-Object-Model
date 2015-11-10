namespace Localization.Xliff.OM
{
    using System.Collections.Generic;

    /// <summary>
    /// This interface indicates that an object consumes XLIFF data for serialization.
    /// </summary>
    internal interface IXliffDataConsumer
    {
        /// <summary>
        /// Gets the order in which to read children from a file so the input conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be input in order to be compliant. This method is
        /// used during deserialization to ensure the elements and text are input in the order specified by that schema.
        /// </summary>
        IEnumerable<OutputItem> XliffInputOrder { get; }

        /// <summary>
        /// Creates a new <see cref="XliffElement"/> depending on the XLIFF element Name.
        /// </summary>
        /// <param name="name">The XLIFF element Name.</param>
        /// <returns>An instance of a class associated with the specified XLIFF Name.</returns>
        IXliffDataConsumer CreateXliffElement(XmlNameInfo name);

        /// <summary>
        /// Informs the implementing class to add the specified <see cref="XliffElement"/> to its structure.
        /// </summary>
        /// <param name="name">The name of the child to add.</param>
        /// <param name="child">The <see cref="IXliffDataConsumer"/> to add.</param>
        void AddXliffChild(XmlNameInfo name, IXliffDataConsumer child);

        /// <summary>
        /// Gets the list of attributes stored in this object. These correspond to XLIFF element attributes.
        /// </summary>
        /// <returns>A dictionary of attributes where the key is the XLIFF Name and the value is the full
        /// attribute details.</returns>
        IEnumerable<IAttributeDataProvider> GetXliffAttributes();

        /// <summary>
        /// Sets the text associated with the object.
        /// </summary>
        /// <param name="text">The text associated with the object.</param>
        void SetXliffText(string text);

        /// <summary>
        /// Tries to set the value of an attribute.
        /// </summary>
        /// <param name="name">The XLIFF Name of the attribute.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>A <see cref="SetAttributeResult"/> that indicates whether the attribute was set.</returns>
        SetAttributeResult TrySetAttributeValue(XmlNameInfo name, string value);
    }
}
