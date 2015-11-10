namespace Localization.Xliff.OM
{
    /// <summary>
    /// This interface indicates that an object can be selected using a selector query.
    /// </summary>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#fragid">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    public interface ISelectable : IIdentifiable
    {
        /// <summary>
        /// Gets the selector Id of the item.
        /// </summary>
        /// <example>For a <see cref="Localization.Xliff.OM.Core.File"/> item, this value might look like
        /// "f=file1" where "file1" is the Id.</example>
        string SelectorId { get; }

        /// <summary>
        /// Gets the full path of the item from the root document.
        /// </summary>
        string SelectorPath { get; }

        /// <summary>
        /// Gets a value indicating whether the element represents a leaf fragment in a selector path. If so, the
        /// selector path shouldn't contain any other fragments after this fragment.
        /// </summary>
        bool IsLeafFragment { get; }
    }
}
