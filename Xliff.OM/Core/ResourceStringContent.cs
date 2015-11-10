namespace Localization.Xliff.OM.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// This is a base class used to group elements that describe resource text (namely inline tags and plain text).
    /// </summary>
    /// <seealso cref="XliffElement"/>
    public abstract class ResourceStringContent : XliffElement
    {
        /// <summary>
        /// Gets the collection of references to <see cref="Data"/> elements. The key is a name associated with the
        /// reference, and the value is the actual reference.
        /// </summary>
        public virtual IDictionary<string, string> AllDataReferences
        {
            get { return null; }
        }

        /// <summary>
        /// Gets a value indicating whether the object has references to <see cref="Data"/> elements.
        /// </summary>
        public virtual bool SupportsDataReferences
        {
            get { return false; }
        }
    }
}
