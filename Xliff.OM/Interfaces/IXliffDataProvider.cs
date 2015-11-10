namespace Localization.Xliff.OM
{
    using System.Collections.Generic;

    /// <summary>
    /// This interface indicates that an object provides XLIFF data for serialization.
    /// </summary>
    internal interface IXliffDataProvider
    {
        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        bool HasXliffChildren { get; }

        /// <summary>
        /// Gets a value indicating whether the provider has text.
        /// </summary>
        bool HasXliffText { get; }

        /// <summary>
        /// Gets the order in which to write children to a file so the output conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be output in order to be compliant. This method is
        /// used during serialization to ensure the elements and text are output in the order specified by that schema.
        /// </summary>
        IEnumerable<OutputItem> XliffOutputOrder { get; }

        /// <summary>
        /// Gets the list of attributes stored in this object. These correspond to XLIFF element attributes.
        /// </summary>
        /// <returns>A dictionary of attributes where the key is the XLIFF Name and the value is the full
        /// attribute details.</returns>
        IEnumerable<IAttributeDataProvider> GetXliffAttributes();

        /// <summary>
        /// Gets the child <see cref="XliffElement"/>s contained within this object.
        /// </summary>
        /// <returns>A list of child elements stored as a pair consisting of the XLIFF Name for the child and
        /// the child itself.</returns>
        IEnumerable<ElementInfo> GetXliffChildren();

        /// <summary>
        /// Gets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <returns>The text within the <see cref="XliffElement"/>.</returns>
        string GetXliffText();
    }
}
