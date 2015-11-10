namespace Localization.Xliff.OM.Extensibility
{
    using System.Collections.Generic;

    /// <summary>
    /// This interface defines the data associated with an extension.
    /// </summary>
    public interface IExtensionData
    {
        /// <summary>
        /// Gets a value indicating whether the data contains attribute members.
        /// </summary>
        bool HasAttributes { get; }

        /// <summary>
        /// Gets a value indicating whether the data contains element or text members.
        /// </summary>
        bool HasChildren { get; }
    
        /// <summary>
        /// Adds an attribute member to the extension data.
        /// </summary>
        /// <param name="attribute">The attribute to add.</param>
        void AddAttribute(IExtensionAttribute attribute);

        /// <summary>
        /// Adds an element or text member to the extension data.
        /// </summary>
        /// <param name="child">The child to add.</param>
        void AddChild(ElementInfo child);

        /// <summary>
        /// Gets the attribute members.
        /// </summary>
        /// <returns>An enumeration of attribute members.</returns>
        IEnumerable<IExtensionAttribute> GetAttributes();

        /// <summary>
        /// Gets the element and text members.
        /// </summary>
        /// <returns>An enumeration of element and text members.</returns>
        IEnumerable<ElementInfo> GetChildren();
    }
}
