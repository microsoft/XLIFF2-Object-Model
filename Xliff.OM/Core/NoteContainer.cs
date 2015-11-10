namespace Localization.Xliff.OM.Core
{
    using System.Collections.Generic;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class is used as a container of <see cref="Note"/>s.
    /// </summary>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="INoteContainer"/>
    /// <seealso cref="ISelectNavigable"/>
    [SchemaChild(NamespaceValues.Core, ElementNames.Note, typeof(Note))]
    internal class NoteContainer : XliffElement, INoteContainer, ISelectNavigable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoteContainer"/> class.
        /// </summary>
        public NoteContainer()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.Notes = new ParentAttachedList<Note>(this);
        }

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || this.HasNotes; }
        }

        /// <summary>
        /// Gets a value indicating whether the object contains notes.
        /// </summary>
        public bool HasNotes
        {
            get { return (this.Notes != null) && (this.Notes.Count > 0); }
        }

        /// <summary>
        /// Gets the list of notes associated with the object.
        /// </summary>
        public IList<Note> Notes { get; private set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Adds a note with the specified text.
        /// </summary>
        /// <param name="note">The text of the note to add.</param>
        /// <returns>The newly created note.</returns>
        public Note AddNote(string note)
        {
            Note newNote;

            newNote = new Note(note);
            this.Notes.Add(newNote);

            return newNote;
        }

        /// <summary>
        /// Gets the child <see cref="XliffElement"/>s contained within this object.
        /// </summary>
        /// <returns>A list of child elements stored as a pair consisting of the XLIFF Name for the child and
        /// the child itself.</returns>
        protected override List<ElementInfo> GetChildren()
        {
            List<ElementInfo> result;

            result = null;
            this.AddChildElementsToList(this.Notes, ref result);

            return result;
        }

        /// <summary>
        /// Selects an item matching the selection query.
        /// </summary>
        /// <param name="path">The selection query.</param>
        /// <returns>The object that was selected from the query path, or null if no match was found.</returns>
        /// <example>The value of <paramref name="path"/> might look something like "#g=group1/f=file1/u=unit1/n=note1"
        /// which is a relative path from the current object, not a full path from the document root.</example>
        public ISelectable Select(string path)
        {
            ArgValidator.Create(path, "path").IsNotNull().StartsWith(Utilities.Constants.SelectorPathIndictator);
            return this.SelectElement(Utilities.RemoveSelectorIndicator(path));
        }

        /// <summary>
        /// Stores the <see cref="XliffElement"/> as a child of this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>True if the child was stored, otherwise false.</returns>
        protected override bool StoreChild(ElementInfo child)
        {
            bool result;

            result = true;
            if (child.Element is Note)
            {
                this.Notes.Add((Note)child.Element);
            }
            else
            {
                result = base.StoreChild(child);
            }

            return result;
        }
        #endregion Methods
    }
}
