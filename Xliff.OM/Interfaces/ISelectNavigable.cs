namespace Localization.Xliff.OM
{
    /// <summary>
    /// This interface indicates that an object can be used to navigate a selection query. The object may be a
    /// selection target (implements <see cref="ISelectable"/>) or it contains children that are selection targets.
    /// </summary>
    public interface ISelectNavigable
    {
        /// <summary>
        /// Selects an item matching the selection query.
        /// </summary>
        /// <param name="path">The selection query.</param>
        /// <returns>The object that was selected from the query path, or null if no match was found.</returns>
        /// <example>The value of <paramref name="path"/> might look something like "#g=group1/f=file1/u=unit1/n=note1"
        /// which is a relative path from the current object, not a full path from the document root.</example>
        ISelectable Select(string path);
    }
}
