namespace Localization.Xliff.OM
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This interface defines properties used to initialize an <see cref="XliffElement"/>.
    /// </summary>
    internal interface IElementInformation
    {
        /// <summary>
        /// Gets the mapping of friendly name to attribute data.
        /// </summary>
        IDictionary<string, AttributeData> AttributeMap { get; }

        /// <summary>
        /// Gets the mapping of type to Xml information for all children of the element.
        /// </summary>
        IDictionary<Type, XmlNameInfo> ChildMap { get; }
    }
}
