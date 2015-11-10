namespace Localization.Xliff.OM.Extensibility
{
    using System.Collections.Generic;

    /// <summary>
    /// This interface indicates that a class is extensible and can store custom attributes, elements, and text.
    /// </summary>
    public interface IExtensible
    {
        /// <summary>
        /// Gets the list of registered extensions on the object.
        /// </summary>
        IList<IExtension> Extensions { get; }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        bool HasExtensions { get; }

        /// <summary>
        /// Gets a value indictating whether attribute extensions are supported by the object.
        /// </summary>
        bool SupportsAttributeExtensions { get; }

        /// <summary>
        /// Gets a value indictating whether element extensions are supported by the object.
        /// </summary>
        bool SupportsElementExtensions { get; }
    }
}
