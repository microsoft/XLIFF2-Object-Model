namespace Localization.Xliff.OM.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// This interface indicates that an object can have notes associated with it.
    /// </summary>
    public interface INoteContainer
    {
        /// <summary>
        /// Gets a value indicating whether the object contains notes.
        /// </summary>
        bool HasNotes { get; }

        /// <summary>
        /// Gets the list of notes associated with the object.
        /// </summary>
        IList<Note> Notes { get; }

        /// <summary>
        /// Adds a note with the specified text.
        /// </summary>
        /// <param name="note">The text of the note to add.</param>
        /// <returns>The newly created note.</returns>
        Note AddNote(string note);
    }
}
