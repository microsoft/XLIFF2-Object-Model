namespace Localization.Xliff.OM.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// This interface indicates that an object is a container for <see cref="ResourceStringContent"/>.
    /// </summary>
    public interface IResourceStringContentContainer
    {
        /// <summary>
        /// Gets the original text.
        /// </summary>
        IList<ResourceStringContent> Text { get; }
    }
}
