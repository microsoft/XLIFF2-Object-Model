namespace Localization.Xliff.OM.Serialization.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Modules;
    using Localization.Xliff.OM.Modules.ChangeTracking;
    using Localization.Xliff.OM.Modules.Metadata;
    using Localization.Xliff.OM.Modules.ResourceData;
    using Localization.Xliff.OM.Modules.SizeRestriction;
    using Localization.Xliff.OM.Modules.Validation;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// This class tests the <see cref="XliffReader"/> class.
    /// </summary>
    [DeploymentItem(TestUtilities.TestDataDirectory, TestUtilities.TestDataDirectory)]
    [TestClass()]
    public class XliffReaderTests
    {
        #region Member Variables
        /// <summary>
        /// The document used for validation.
        /// </summary>
        private XliffDocument _document;

        /// <summary>
        /// The serializer used to deserialize the document from the stream.
        /// </summary>
        private XliffReader _reader;
        #endregion Member Variables

        /// <summary>
        /// Initializes the test class before every test method is executed.
        /// </summary>
        [TestInitialize()]
        public void TestInitialize()
        {
            XliffReaderSettings settings;

            this._document = new XliffDocument();

            settings = new XliffReaderSettings();
            settings.Validators.Clear();
            this._reader = new XliffReader(settings);
        }

        #region Test Methods
        /// <summary>
        /// Tests that an <see cref="ChangeTrack"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_ChangeTrack()
        {
            ChangeTrack change;
            Item item;
            RevisionsContainer container;
            Revision revision;

            this.Deserialize(TestData.FileWithChangeTrack);

            change = this._document.Files[0].Changes;
            Assert.IsNotNull(change, "Changes is incorrect.");
            Assert.IsNotNull(change.Revisions, "Revisions is null.");
            Assert.AreEqual(1, change.Revisions.Count, "Revisions count is incorrect.");

            container = change.Revisions[0];
            Assert.AreEqual("source", container.AppliesTo, "Container AppliesTo is incorrect.");
            Assert.AreEqual("1", container.Reference, "Container Referenceis incorrect.");
            Assert.AreEqual("ver1", container.CurrentVersion, "Container CurrentVersion is incorrect.");
            Assert.AreEqual(1, container.Revisions.Count, "Container Revisions.Count is incorrect.");

            revision = container.Revisions[0];
            Assert.AreEqual("author", revision.Author, "Revision Author is incorrect.");
            Assert.AreEqual(new DateTime(2015, 1, 2, 3, 4, 5), revision.ChangeDate, "Revision ChangeDate is incorrect.");
            Assert.AreEqual("ver1", revision.Version, "Revision Version is incorrect.");
            Assert.IsNotNull(revision.Items, "Revisions Items is null.");
            Assert.AreEqual(1, revision.Items.Count, "Revisions Items count is incorrect.");

            item = revision.Items[0];
            Assert.AreEqual("content", item.Property, "Item Property is incorrect.");
            Assert.AreEqual("text", item.Text, "Item Text is incorrect.");
        }

        /// <summary>
        /// Tests the various Deserialize methods.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_Deserialize()
        {
            XliffDocument document;
            string path;

            this._document.SourceLanguage = "en-us";
            this._document.Space = Preservation.Preserve;
            this._document.TargetLanguage = "de-de";
            this._document.Version = "version";

            Console.WriteLine("Test with stream.");
            path = System.IO.Path.Combine(Environment.CurrentDirectory, TestUtilities.TestDataDirectory, "DocumentWithValidValues.xlf");
            using (System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                document = this._reader.Deserialize(stream);
                Assert.AreEqual(0, this._document.Files.Count, "File count is incorrect.");
                Assert.IsNull(this._document.Id, "Id is incorrect.");
                Assert.AreEqual("en-us", this._document.SourceLanguage, "SourceLanguage is incorrect.");
                Assert.AreEqual(Preservation.Preserve, this._document.Space, "Space is incorrect.");
                Assert.AreEqual("de-de", this._document.TargetLanguage, "TargetLanguage is incorrect.");
                Assert.AreEqual("version", this._document.Version, "Version is incorrect.");
            }
        }

        /// <summary>
        /// Tests that an <see cref="XliffDocument"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_Document()
        {
            Console.WriteLine("Test with default values.");
            this.Deserialize(TestData.DocumentWithDefaultValues);
            Assert.AreEqual(0, this._document.Files.Count, "File count is incorrect.");
            Assert.IsNull(this._document.Id, "Id is incorrect.");
            Assert.AreEqual(String.Empty, this._document.SourceLanguage, "SourceLanguage is incorrect.");
            Assert.AreEqual(Preservation.Default, this._document.Space, "Space is incorrect.");
            Assert.IsNull(this._document.TargetLanguage, "TargetLanguage is incorrect.");
            Assert.AreEqual("2.0", this._document.Version, "Version is incorrect.");

            Console.WriteLine("Test with empty values.");
            this.Deserialize(TestData.DocumentWithEmptyValues);
            Assert.AreEqual(0, this._document.Files.Count, "File count is incorrect.");
            Assert.IsNull(this._document.Id, "Id is incorrect.");
            Assert.AreEqual(String.Empty, this._document.SourceLanguage, "SourceLanguage is incorrect.");
            Assert.AreEqual(Preservation.Default, this._document.Space, "Space is incorrect.");
            Assert.AreEqual(String.Empty, this._document.TargetLanguage, "TargetLanguage is incorrect.");
            Assert.AreEqual("2.0", this._document.Version, "Version is incorrect.");

            Console.WriteLine("Test with valid values.");
            this.Deserialize(TestData.DocumentWithValidValues);
            Assert.AreEqual(0, this._document.Files.Count, "File count is incorrect.");
            Assert.IsNull(this._document.Id, "Id is incorrect.");
            Assert.AreEqual("en-us", this._document.SourceLanguage, "SourceLanguage is incorrect.");
            Assert.AreEqual(Preservation.Preserve, this._document.Space, "Space is incorrect.");
            Assert.AreEqual("de-de", this._document.TargetLanguage, "TargetLanguage is incorrect.");
            Assert.AreEqual("version", this._document.Version, "Version is incorrect.");
        }

        /// <summary>
        /// Tests that an <see cref="XliffDocument"/> deserializes correctly when the document uses a namespace prefix
        /// for xliff elements rather than using a default namespace.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_DocumentWithXmlPrefix()
        {
            Segment segment;
            Unit unit;

            this.Deserialize(TestData.DocumentWithXmlPrefix);
            Assert.AreEqual(1, this._document.Files.Count, "File count is incorrect.");
            Assert.AreEqual(1, this._document.Files[0].Containers.Count, "File containers count is incorrect.");
            Assert.IsInstanceOfType(this._document.Files[0].Containers[0], typeof(Unit), "Containers[0] type is incorrect.");
            unit = this._document.Files[0].Containers[0] as Unit;
            Assert.AreEqual(1, unit.Resources.Count, "Unit resources count is incorrect.");
            Assert.IsInstanceOfType(unit.Resources[0], typeof(Segment), "Unit resources[0] type is incorrect.");
            segment = unit.Resources[0] as Segment;
            Assert.AreEqual(1, segment.Source.Text.Count, "Source text count is incorrect.");
            Assert.IsInstanceOfType(segment.Source.Text[0], typeof(PlainText), "Text[0] type is incorrect.");
            Assert.AreEqual("text", ((PlainText)segment.Source.Text[0]).Text, "Text[0].Text is incorrect.");
        }

        /// <summary>
        /// Tests that a <see cref="File"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_File()
        {
            Console.WriteLine("Test with default values.");
            this.Deserialize(TestData.FileWithDefaultValues);
            Assert.AreEqual(1, this._document.Files.Count, "File count is incorrect.");
            Assert.IsTrue(this._document.Files[0].CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual(0, this._document.Files[0].Containers.Count, "Containers count is incorrect.");
            Assert.IsFalse(this._document.Files[0].HasNotes, "HasNotes is incorrect.");
            Assert.AreEqual(String.Empty, this._document.Files[0].Id, "Id is incorrect.");
            Assert.IsNull(this._document.Files[0].Original, "Original is incorrect.");
            Assert.IsNull(this._document.Files[0].Skeleton, "Skeleton is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            this._document.Files[0].SourceDirectionality,
                            "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Default,
                            this._document.Files[0].Space,
                            "CanResegment is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            this._document.Files[0].TargetDirectionality,
                            "TargetDirectionality is incorrect.");
            Assert.IsTrue(this._document.Files[0].Translate, "Translate is incorrect.");

            Console.WriteLine("Test with empty values.");
            this.Deserialize(TestData.FileWithEmptyValues);
            this.Deserialize(TestData.FileWithDefaultValues);
            Assert.AreEqual(1, this._document.Files.Count, "File count is incorrect.");
            Assert.IsTrue(this._document.Files[0].CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual(0, this._document.Files[0].Containers.Count, "Containers count is incorrect.");
            Assert.IsFalse(this._document.Files[0].HasNotes, "HasNotes is incorrect.");
            Assert.AreEqual(String.Empty, this._document.Files[0].Id, "Id is incorrect.");
            Assert.IsNull(this._document.Files[0].Original, "Original is incorrect.");
            Assert.IsNull(this._document.Files[0].Skeleton, "Skeleton is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            this._document.Files[0].SourceDirectionality,
                            "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Default,
                            this._document.Files[0].Space,
                            "CanResegment is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            this._document.Files[0].TargetDirectionality,
                            "TargetDirectionality is incorrect.");
            Assert.IsTrue(this._document.Files[0].Translate, "Translate is incorrect.");

            Console.WriteLine("Test with valid values.");
            this.Deserialize(TestData.FileWithValidValues);
            Assert.AreEqual(1, this._document.Files.Count, "File count is incorrect.");
            Assert.IsTrue(this._document.Files[0].CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual(0, this._document.Files[0].Containers.Count, "Containers count is incorrect.");
            Assert.IsFalse(this._document.Files[0].HasNotes, "HasNotes is incorrect.");
            Assert.AreEqual("id", this._document.Files[0].Id, "Id is incorrect.");
            Assert.AreEqual("original", this._document.Files[0].Original, "Original is incorrect.");
            Assert.IsNull(this._document.Files[0].Skeleton, "Skeleton is incorrect.");
            Assert.AreEqual(ContentDirectionality.LTR,
                            this._document.Files[0].SourceDirectionality,
                            "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Default,
                            this._document.Files[0].Space,
                            "CanResegment is incorrect.");
            Assert.AreEqual(ContentDirectionality.RTL,
                            this._document.Files[0].TargetDirectionality,
                            "TargetDirectionality is incorrect.");
            Assert.IsFalse(this._document.Files[0].Translate, "Translate is incorrect.");
        }

        /// <summary>
        /// Tests that <see cref="Note"/>s in a <see cref="File"/> deserialize correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_FileNotes()
        {
            Console.WriteLine("Test with default values.");
            this.Deserialize(TestData.FileNoteWithDefaultValues);
            Assert.IsTrue(this._document.Files[0].HasNotes, "HasNotes is incorrect.");
            Assert.AreEqual(1, this._document.Files[0].Notes.Count, "Note count is incorrect.");
            Assert.IsNull(this._document.Files[0].Notes[0].AppliesTo, "AppliesTo is incorrect.");
            Assert.IsNull(this._document.Files[0].Notes[0].Category, "Category is incorrect.");
            Assert.IsNull(this._document.Files[0].Notes[0].Id, "Id is incorrect.");
            Assert.AreEqual(1, this._document.Files[0].Notes[0].Priority, "Priority is incorrect.");
            Assert.IsNull(this._document.Files[0].Notes[0].Text, "Text is incorrect.");

            Console.WriteLine("Test with empty values.");
            this.Deserialize(TestData.FileNoteWithEmptyValues);
            Assert.IsTrue(this._document.Files[0].HasNotes, "HasNotes is incorrect.");
            Assert.AreEqual(1, this._document.Files[0].Notes.Count, "Note count is incorrect.");
            Assert.IsNull(this._document.Files[0].Notes[0].AppliesTo, "AppliesTo is incorrect.");
            Assert.AreEqual(String.Empty,
                            this._document.Files[0].Notes[0].Category,
                            "Category is incorrect.");
            Assert.IsNull(this._document.Files[0].Notes[0].Id, "Id is incorrect.");
            Assert.AreEqual(1, this._document.Files[0].Notes[0].Priority, "Priority is incorrect.");
            Assert.IsNull(this._document.Files[0].Notes[0].Text, "Text is incorrect.");

            Console.WriteLine("Test with valid values.");
            this.Deserialize(TestData.FileNoteWithValidValues);
            Assert.IsTrue(this._document.Files[0].HasNotes, "HasNotes is incorrect.");
            Assert.AreEqual(1, this._document.Files[0].Notes.Count, "Note count is incorrect.");
            Assert.AreEqual(TranslationSubject.Target,
                            this._document.Files[0].Notes[0].AppliesTo,
                            "AppliesTo is incorrect.");
            Assert.AreEqual("category",
                            this._document.Files[0].Notes[0].Category,
                            "Category is incorrect.");
            Assert.AreEqual("id", this._document.Files[0].Notes[0].Id, "Id is incorrect.");
            Assert.AreEqual(100, this._document.Files[0].Notes[0].Priority, "Priority is incorrect.");
            Assert.AreEqual("text", this._document.Files[0].Notes[0].Text, "Text is incorrect.");
        }

        /// <summary>
        /// Tests that a full <see cref="XliffDocument"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_FullDocument()
        {
            File file;
            Group group;
            Ignorable ignorable;
            Note note;
            Segment segment;
            Skeleton skeleton;
            Unit unit;

            this.Deserialize(TestData.FullDocument);

            Console.WriteLine("Verifying XliffDocument.");
            Assert.AreEqual(2, this._document.Files.Count, "File count is incorrect.");
            Assert.AreEqual("en-us", this._document.SourceLanguage, "SourceLanguage is incorrect.");
            Assert.AreEqual(Preservation.Default, this._document.Space, "Space is incorrect.");
            Assert.AreEqual("de-de", this._document.TargetLanguage, "TargetLanguage is incorrect.");
            Assert.AreEqual("version", this._document.Version, "Version is incorrect.");

            Console.WriteLine("Verifying File[0].");
            file = this._document.Files[0];
            Assert.IsTrue(file.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual(2, file.Containers.Count, "Containers count is incorrect.");
            Assert.AreEqual("id", file.Id, "Id is incorrect.");
            Assert.AreEqual("original", file.Original, "Original is incorrect.");
            Assert.AreEqual(ContentDirectionality.LTR, file.SourceDirectionality, "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Preserve, file.Space, "Space is incorrect.");
            Assert.AreEqual(ContentDirectionality.RTL, file.TargetDirectionality, "TargetDirectionality is incorrect.");
            Assert.IsTrue(file.Translate, "Translate is incorrect.");

            Console.WriteLine("Verifying File[0].Group[0]");
            group = file.Containers[0] as Group;
            Assert.IsNotNull(group, "Group is null.");
            Assert.IsTrue(group.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual(0, group.Containers.Count, "Containers count is incorrect.");
            Assert.AreEqual("id", group.Id, "Id is incorrect.");
            Assert.AreEqual("name", group.Name, "Id is incorrect.");
            Assert.AreEqual(ContentDirectionality.RTL, group.SourceDirectionality, "Id is incorrect.");
            Assert.AreEqual(Preservation.Preserve, group.Space, "Id is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto, group.TargetDirectionality, "Id is incorrect.");
            Assert.IsTrue(group.Translate, "Id is incorrect.");
            Assert.AreEqual("type", group.Type, "Id is incorrect.");

            Console.WriteLine("Verifying File[0].Group[1]");
            group = file.Containers[1] as Group;
            Assert.IsNotNull(group, "Group is null.");
            Assert.IsFalse(group.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual(2, group.Containers.Count, "Containers count is incorrect.");
            Assert.AreEqual("id2", group.Id, "Id is incorrect.");
            Assert.AreEqual("name2", group.Name, "Id is incorrect.");
            Assert.AreEqual(ContentDirectionality.LTR, group.SourceDirectionality, "Id is incorrect.");
            Assert.AreEqual(Preservation.Default, group.Space, "Id is incorrect.");
            Assert.AreEqual(ContentDirectionality.RTL, group.TargetDirectionality, "Id is incorrect.");
            Assert.IsFalse(group.Translate, "Id is incorrect.");
            Assert.AreEqual("type2", group.Type, "Id is incorrect.");

            Console.WriteLine("Verifying File[0].Group[1].Unit[0]");
            unit = group.Containers[0] as Unit;
            Assert.IsNotNull(unit, "Unit is null.");
            Assert.IsFalse(unit.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual("id", unit.Id, "Id is incorrect.");
            Assert.AreEqual("name", unit.Name, "Name is incorrect.");
            Assert.AreEqual(1, unit.Notes.Count, "Notes count is incorrect.");
            Assert.AreEqual(ContentDirectionality.LTR, unit.SourceDirectionality, "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Default, unit.Space, "Space is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            unit.TargetDirectionality,
                            "TargetDirectionality is incorrect.");
            Assert.IsTrue(unit.Translate, "Translate is incorrect.");
            Assert.AreEqual("type", unit.Type, "Type is incorrect.");

            Console.WriteLine("Verifying File[0].Group[1].Unit[0].Notes[0]");
            Assert.IsNull(unit.Notes[0].AppliesTo, "AppliesTo is incorrect.");
            Assert.IsNull(unit.Notes[0].Category, "Category is incorrect.");
            Assert.IsNull(unit.Notes[0].Id, "Id is incorrect.");
            Assert.AreEqual(1, unit.Notes[0].Priority, "Priority is incorrect.");
            Assert.AreEqual("text", unit.Notes[0].Text, "Text is incorrect.");

            Console.WriteLine("Verifying File[0].Group[1].Unit[1]");
            unit = group.Containers[1] as Unit;
            Assert.IsNotNull(unit, "Unit is null.");
            Assert.IsFalse(unit.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual("id2", unit.Id, "Id is incorrect.");
            Assert.AreEqual("name2", unit.Name, "Name is incorrect.");
            Assert.AreEqual(1, unit.Notes.Count, "Notes count is incorrect.");
            Assert.AreEqual(ContentDirectionality.RTL, unit.SourceDirectionality, "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Preserve, unit.Space, "Space is incorrect.");
            Assert.AreEqual(ContentDirectionality.LTR, unit.TargetDirectionality, "TargetDirectionality is incorrect.");
            Assert.IsFalse(unit.Translate, "Translate is incorrect.");
            Assert.AreEqual("type2", unit.Type, "Type is incorrect.");

            Console.WriteLine("Verifying File[0].Group[1].Unit[1].Notes[0]");
            Assert.IsNotNull(unit.Notes[0], "Notes[0] is null.");
            Assert.IsNull(unit.Notes[0].AppliesTo, "AppliesTo is incorrect.");
            Assert.IsNull(unit.Notes[0].Category, "Category is incorrect.");
            Assert.IsNull(unit.Notes[0].Id, "Id is incorrect.");
            Assert.AreEqual(1, unit.Notes[0].Priority, "Priority is incorrect.");
            Assert.AreEqual("text2", unit.Notes[0].Text, "Text is incorrect.");

            Console.WriteLine("Verifying File[0].Group[1].Unit[1].Ignorable[0]");
            ignorable = unit.Resources[0] as Ignorable;
            Assert.IsNotNull(unit, "Unit is null.");
            Assert.AreEqual("id", ignorable.Id, "Id is incorrect.");
            Assert.IsNotNull(ignorable.Source, "Source is null.");
            Assert.AreEqual("en-us", ignorable.Source.Language, "Inherited Source.Language is incorrect.");
            Assert.AreEqual(Preservation.Preserve, ignorable.Source.Space, "Source.Space is incorrect.");
            Assert.AreEqual(1, ignorable.Source.Text.Count, "Source.Text count is incorrect.");
            Assert.IsInstanceOfType(ignorable.Source.Text[0], typeof(PlainText), "Source.Text is incorrect.");
            Assert.AreEqual("text", ((PlainText)ignorable.Source.Text[0]).Text, "Source.Text is incorrect.");
            Assert.IsNotNull(ignorable.Target, "Target is null.");
            Assert.AreEqual("de-de", ignorable.Target.Language, "Target.Language is incorrect.");
            Assert.IsNull(ignorable.Target.Order, "Target.Order is incorrect.");
            Assert.AreEqual(Preservation.Preserve, ignorable.Target.Space, "Target.Space is incorrect.");
            Assert.AreEqual(1, ignorable.Target.Text.Count, "Target.Text count is incorrect.");
            Assert.IsInstanceOfType(ignorable.Target.Text[0], typeof(PlainText), "Target.Text is incorrect.");
            Assert.AreEqual("text", ((PlainText)ignorable.Target.Text[0]).Text, "Target.Text is incorrect.");

            Console.WriteLine("Verifying File[0].Group[1].Unit[1].Segment[0]");
            segment = unit.Resources[1] as Segment;
            Assert.IsTrue(segment.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual("id", segment.Id, "Id is incorrect.");
            Assert.IsNotNull(segment.Source, "Source is null.");
            Assert.AreEqual("en-us", segment.Source.Language, "Source.Language is incorrect.");
            Assert.AreEqual(Preservation.Default, segment.Source.Space, "Source.Space is incorrect.");
            Assert.AreEqual(2, segment.Source.Text.Count, "Source.Text.Count is incorrect.");
            Assert.IsInstanceOfType(segment.Source.Text[0], typeof(CodePoint), "Source.Text[0] is incorrect.");
            Assert.AreEqual(1, ((CodePoint)segment.Source.Text[0]).Code, "CodePoint Code is incorrect.");
            Assert.IsInstanceOfType(segment.Source.Text[1], typeof(PlainText), "Source.Text[1] is incorrect.");
            Assert.AreEqual("text", ((PlainText)segment.Source.Text[1]).Text, "PlainText Text is incorrect.");
            Assert.AreEqual(TranslationState.Reviewed, segment.State, "State is incorrect.");
            Assert.AreEqual("substate", segment.SubState, "SubState is incorrect.");
            Assert.IsNotNull(segment.Target, "Target is null.");
            Assert.AreEqual("en-us", segment.Target.Language, "Target.Language is incorrect.");
            Assert.AreEqual(100u, segment.Target.Order, "Target.Order is incorrect.");
            Assert.AreEqual(Preservation.Default, segment.Target.Space, "Target.Space is incorrect.");
            Assert.AreEqual(2, segment.Target.Text.Count, "Target.Text.Count is incorrect.");
            Assert.IsInstanceOfType(segment.Target.Text[0], typeof(CodePoint), "Target.Text[0] is incorrect.");
            Assert.AreEqual(12, ((CodePoint)segment.Target.Text[0]).Code, "CodePoint Code is incorrect.");
            Assert.IsInstanceOfType(segment.Target.Text[1], typeof(PlainText), "Target.Text[1] is incorrect.");
            Assert.AreEqual("text2", ((PlainText)segment.Target.Text[1]).Text, "PlainText Text is incorrect.");

            Console.WriteLine("Verifying File.Notes[0]");
            note = file.Notes[0];
            Assert.IsNotNull(note, "Note is null.");
            Assert.AreEqual(TranslationSubject.Source, note.AppliesTo, "AppliesTo is incorrect.");
            Assert.AreEqual("category", note.Category, "Category is incorrect.");
            Assert.AreEqual("id", note.Id, "Id is incorrect.");
            Assert.AreEqual(2, note.Priority, "Priority is incorrect.");
            Assert.AreEqual("text", note.Text, "Text is incorrect.");

            Console.WriteLine("Verifying File.Notes[1]");
            note = file.Notes[1];
            Assert.IsNotNull(note, "Note is null.");
            Assert.IsNull(note.AppliesTo, "AppliesTo is incorrect.");
            Assert.IsNull(note.Category, "Category is incorrect.");
            Assert.IsNull(note.Id, "Id is incorrect.");
            Assert.AreEqual(1, note.Priority, "Priority is incorrect.");
            Assert.AreEqual("text2", note.Text, "Text is incorrect.");

            Console.WriteLine("Verifying File.Skeleton");
            skeleton = file.Skeleton;
            Assert.IsNotNull(skeleton, "Skeleton is null.");
            Assert.AreEqual("href", skeleton.HRef, "HRef is incorrect.");
            Assert.AreEqual("text", skeleton.NonTranslatableText, "NonTranslatableText is incorrect.");

            Console.WriteLine("Verifying File[1]");
            file = this._document.Files[1];
            Assert.IsNotNull(file, "File is null.");
            Assert.IsFalse(file.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual("id2", file.Id, "Id is incorrect.");
            Assert.AreEqual(1, file.Notes.Count, "Note count is incorrect.");
            Assert.IsNotNull(file.Notes[0], "Note is null.");
            Assert.IsNull(file.Notes[0].AppliesTo, "AppliesTo is incorrect.");
            Assert.IsNull(file.Notes[0].Category, "Category is incorrect.");
            Assert.IsNull(file.Notes[0].Id, "Id is incorrect.");
            Assert.AreEqual(1, file.Notes[0].Priority, "Priority is incorrect.");
            Assert.AreEqual("text", file.Notes[0].Text, "Text is incorrect.");
            Assert.AreEqual("original2", file.Original, "Original is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            file.SourceDirectionality,
                            "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Default, file.Space, "Space is incorrect.");
            Assert.AreEqual(ContentDirectionality.LTR, file.TargetDirectionality, "TargetDirectionality is incorrect.");
            Assert.IsFalse(file.Translate, "Translate is incorrect.");

            Console.WriteLine("Verifying File[1].Skeleton");
            skeleton = file.Skeleton;
            Assert.IsNotNull(skeleton, "Skeleton is null.");
            Assert.AreEqual("href", skeleton.HRef, "HRef is incorrect.");
            Assert.AreEqual("text", skeleton.NonTranslatableText, "NonTranslatableText is incorrect.");
        }

        /// <summary>
        /// Tests that a <see cref="Modules.Glossary.Glossary"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_Glossary()
        {
            Unit unit;

            this.Deserialize(TestData.GlossaryWithValidValues);
            unit = this._document.Files[0].Containers[0] as Unit;
            Assert.IsNotNull(unit.Glossary, "Glossary should not be null.");
            Assert.AreEqual(1, unit.Glossary.Entries.Count, "Entries count is incorrect.");

            Assert.IsNotNull(unit.Glossary.Entries[0].Definition, "Definition should not be null.");
            Assert.AreEqual("definitionSource", unit.Glossary.Entries[0].Definition.Source, "Definition.Source is incorrect.");
            Assert.AreEqual("definition text", unit.Glossary.Entries[0].Definition.Text, "Definition.Text is incorrect.");

            Assert.AreEqual("entry1", unit.Glossary.Entries[0].Id, "Entry Id is incorrect.");
            Assert.AreEqual("#m1", unit.Glossary.Entries[0].Reference, "Entry Reference is incorrect.");

            Assert.IsNotNull(unit.Glossary.Entries[0].Term, "Term should not be null.");
            Assert.AreEqual("termSource", unit.Glossary.Entries[0].Term.Source, "Term.Source is incorrect.");
            Assert.AreEqual("term text", unit.Glossary.Entries[0].Term.Text, "Term.Text is incorretc.");

            Assert.IsNotNull(unit.Glossary.Entries[0].Translations, "Translations should not be null.");
            Assert.AreEqual(1, unit.Glossary.Entries[0].Translations.Count, "Translations count is incorrect.");
            Assert.AreEqual("trans1", unit.Glossary.Entries[0].Translations[0].Id, "Translation.Id is incorrect.");
            Assert.AreEqual("#m1", unit.Glossary.Entries[0].Translations[0].Reference, "Translation.Reference is incorrect.");
            Assert.AreEqual("transSource", unit.Glossary.Entries[0].Translations[0].Source, "Translation.Source is incorrect.");
            Assert.AreEqual("translation text", unit.Glossary.Entries[0].Translations[0].Text, "Translation.Text is incorrect.");
        }

        /// <summary>
        /// Tests that a <see cref="Group"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_Group()
        {
            Group group;

            Console.WriteLine("Test with default values.");
            this.Deserialize(TestData.GroupWithDefaultValues);
            group = this._document.Files[0].Containers[0] as Group;
            Assert.IsNotNull(group, "Group is null.");
            Assert.IsTrue(group.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual(String.Empty, group.Id, "Id is incorrect.");
            Assert.IsNull(group.Name, "Name is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            group.SourceDirectionality,
                            "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Default, group.Space, "Space is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            group.TargetDirectionality,
                            "TargetDirectionality is incorrect.");
            Assert.IsTrue(group.Translate, "Translate is incorrect.");
            Assert.IsNull(group.Type, "Type is incorrect.");

            Console.WriteLine("Test with empty values.");
            this.Deserialize(TestData.GroupWithEmptyValues);
            group = this._document.Files[0].Containers[0] as Group;
            Assert.IsNotNull(group, "Group is null.");
            Assert.IsTrue(group.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual(String.Empty, group.Id, "Id is incorrect.");
            Assert.AreEqual(String.Empty, group.Name, "Name is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            group.SourceDirectionality,
                            "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Default, group.Space, "Space is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            group.TargetDirectionality,
                            "TargetDirectionality is incorrect.");
            Assert.IsTrue(group.Translate, "Translate is incorrect.");
            Assert.AreEqual(String.Empty, group.Type, "Type is incorrect.");

            Console.WriteLine("Test with valid values.");
            this.Deserialize(TestData.GroupWithValidValues);
            group = this._document.Files[0].Containers[0] as Group;
            Assert.IsNotNull(group, "Group is null.");
            Assert.IsFalse(group.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual("id", group.Id, "Id is incorrect.");
            Assert.AreEqual("name", group.Name, "Name is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            group.SourceDirectionality,
                            "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Default, group.Space, "Space is incorrect.");
            Assert.AreEqual(ContentDirectionality.LTR,
                            group.TargetDirectionality,
                            "TargetDirectionality is incorrect.");
            Assert.IsTrue(group.Translate, "Translate is incorrect.");
            Assert.AreEqual("type", group.Type, "Type is incorrect.");
        }

        /// <summary>
        /// Tests that an <see cref="Ignorable"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_Ignorable()
        {
            Ignorable ignorable;

            Console.WriteLine("Test with default values.");
            this.Deserialize(TestData.IgnorableWithDefaultValues);
            ignorable = ((Unit)this._document.Files[0].Containers[0]).Resources[0] as Ignorable;
            Assert.IsNotNull(ignorable, "Ignorable is null.");
            Assert.IsNull(ignorable.Id, "Id is incorrect.");
            Assert.IsNull(ignorable.Source, "Source is incorrect.");
            Assert.IsNull(ignorable.Target, "Targetis incorrect.");

            Console.WriteLine("Test with empty values.");
            this.Deserialize(TestData.IgnorableWithEmptyValues);
            ignorable = ((Unit)this._document.Files[0].Containers[0]).Resources[0] as Ignorable;
            Assert.IsNotNull(ignorable, "Ignorable is null.");
            Assert.IsNull(ignorable.Id, "Id is incorrect.");
            Assert.IsNull(ignorable.Source, "Source is incorrect.");
            Assert.IsNull(ignorable.Target, "Targetis incorrect.");

            Console.WriteLine("Test with valid values.");
            this.Deserialize(TestData.IgnorableWithValidValues);
            ignorable = ((Unit)this._document.Files[0].Containers[0]).Resources[0] as Ignorable;
            Assert.IsNotNull(ignorable, "Ignorable is null.");
            Assert.AreEqual("id", ignorable.Id, "Id is incorrect.");
            Assert.IsNull(ignorable.Source, "Source is incorrect.");
            Assert.IsNull(ignorable.Target, "Targetis incorrect.");
        }

        /// <summary>
        /// Test de-serializer with invalid arguments.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_InvalidDocuments()
        {
            Console.WriteLine("Test with null stream.");
            try
            {
                this._reader.Deserialize((System.IO.Stream)null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            Console.WriteLine("Test with empty stream.");
            try
            {
                this._reader.Deserialize(new System.IO.MemoryStream());
                Assert.Fail("Expected XmlException to be thrown.");
            }
            catch (XmlException)
            {
            }

            Console.WriteLine("Test with invalid file.");
            try
            {
                this.Deserialize(TestData.InvalidFile);
                Assert.Fail("Expected XmlException to be thrown.");
            }
            catch (XmlException)
            {
            }

            Console.WriteLine("Test with empty file.");
            try
            {
                this.Deserialize(TestData.EmptyFile);
                Assert.Fail("Expected XmlException to be thrown.");
            }
            catch (XmlException)
            {
            }
        }

        /// <summary>
        /// Tests that a <see cref="MarkedSpan"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_MarkedSpan()
        {
            List<MarkedSpan> spans;

            Console.WriteLine("Test with missing translate.");
            try
            {                
                this.Deserialize(TestData.MarkedSpanWithMissingTranslate);
                Assert.Fail("Expected FormatException to be thrown.");
            }
            catch (FormatException)
            {
            }

            Console.WriteLine("Test with valid values.");
            this.Deserialize(TestData.MarkedSpanWithValidValues);

            Console.WriteLine("Test with nested spans.");
            this.Deserialize(TestData.MarkedSpanWithNestedMarkedSpan);
            spans = this._document.Files[0].CollapseChildren<MarkedSpan>();            
            Assert.AreEqual("mrk1", spans[0].Id);
            Assert.AreEqual("comment", spans[0].Type);
            Assert.AreEqual("this is a comment", spans[0].Value);
            Assert.AreEqual(spans[0], spans[1].Parent); 
            Assert.AreEqual("mrk2", spans[1].Id);
            Assert.AreEqual("generic", spans[1].Type);
            Assert.AreEqual(null, spans[1].Value);
        }

        /// <summary>
        /// Tests that a <see cref="MarkedSpanStart"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_MarkedSpanStart()
        {
            try
            {
                this.Deserialize(TestData.MarkedSpanStartWithMissingTranslate);
                Assert.Fail("Expected FormatException to be thrown.");
            }
            catch (FormatException)
            {
            }

            this.Deserialize(TestData.MarkedSpanStartWithValidValues);
        }

        /// <summary>
        /// Tests that a <see cref="MetadataContainer"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_Metadata()
        {
            MetadataContainer metadata;
            this.Deserialize(TestData.UnitWithMetadata);

            metadata = this._document.Files[0].Metadata;
            Assert.IsNull(metadata, "Metadata is incorrect.");

            metadata = ((Unit)this._document.Files[0].Containers[0]).Metadata;
            Assert.AreEqual(1, metadata.Groups.Count, "Group count is incorrect.");
            Assert.AreEqual("category", metadata.Groups[0].Category, "Category is incorrect.");
            Assert.AreEqual(MetaGroupSubject.Source, metadata.Groups[0].AppliesTo, "AppliesTo is incorrect.");
            Assert.AreEqual(1, metadata.Groups[0].Containers.Count, "Containers count is incorrect.");
            Assert.IsInstanceOfType(metadata.Groups[0].Containers[0], typeof(Meta), "Container[0] type is incorrect.");
            Assert.AreEqual("text", ((Meta)metadata.Groups[0].Containers[0]).NonTranslatableText, "Text is incorrect.");
            Assert.AreEqual("type", ((Meta)metadata.Groups[0].Containers[0]).Type, "Text is incorrect.");
        }

        /// <summary>
        /// Tests that an <see cref="ProfileData"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_ProfileData()
        {
            ProfileData data;

            this.Deserialize(TestData.FileWithProfileData);

            data = this._document.Files[0].ProfileData;
            Assert.IsNotNull(data, "Data is null.");
            Assert.AreEqual("profile", data.Profile, "Profile is incorrect.");
        }

        /// <summary>
        /// Tests that an <see cref="Profiles"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_Profiles()
        {
            Normalization normalization;
            Profiles profiles;

            this.Deserialize(TestData.FileWithProfiles);

            profiles = this._document.Files[0].RestrictionProfiles;
            Assert.IsNotNull(profiles, "Profiles is null.");
            Assert.AreEqual("generalprofile", profiles.GeneralProfile, "GeneralProfile is incorrect.");
            Assert.AreEqual("storageprofile", profiles.StorageProfile, "StorageProfile is incorrect.");
            Assert.IsNotNull(profiles.Normalization, "Normalization is null.");

            normalization = profiles.Normalization;
            Assert.AreEqual(NormalizationValue.NFC, normalization.General, "Normalization General is incorrect.");
            Assert.AreEqual(NormalizationValue.NFC, normalization.Storage, "Normalization Storage is incorrect.");
        }

        /// <summary>
        /// Tests that an <see cref="ResourceData"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_ResourceData()
        {
            Reference reference;
            ResourceData data;
            ResourceItemRef itemRef;
            ResourceItem item;
            ResourceItemSource source;
            ResourceItemTarget target;

            this.Deserialize(TestData.FileWithResourceData);

            data = this._document.Files[0].ResourceData;
            Assert.IsNotNull(data, "ResourceData is null.");
            Assert.IsNotNull(data.ResourceItemReferences, "ResourceItemReferences is null.");
            Assert.IsNotNull(data.ResourceItems, "ResourceItems is null.");

            Assert.AreEqual(1, data.ResourceItemReferences.Count, "ResourceItemReferences count is incorrect.");
            itemRef = data.ResourceItemReferences[0];
            Assert.AreEqual("rif1", itemRef.Id, "Ref[0] Id is incorrect.");
            Assert.AreEqual("ri1", itemRef.Reference, "Ref[0] Reference is incorrect.");

            Assert.AreEqual(1, data.ResourceItems.Count, "ResourceItem count is incorrect.");
            item = data.ResourceItems[0];
            Assert.AreEqual(true, item.Context, "Item[0] Context is incorrect.");
            Assert.AreEqual("ri1", item.Id, "Item[0] Id is incorrect.");
            Assert.AreEqual("mime", item.MimeType, "Item[0] MimeType is incorrect.");
            Assert.IsNotNull(item.References, "Item[0] References is null.");
            Assert.IsNotNull(item.Source, "Item[0] Source is null.");
            Assert.IsNotNull(item.Target, "Item[0] Target is null.");

            Assert.AreEqual(1, item.References.Count, "Item[0] References count is incorrect.");
            reference = item.References[0];
            Assert.AreEqual("resource", reference.HRef, "Reference HRef is incorrect.");
            Assert.AreEqual("de-de", reference.Language, "Reference Language is incorrect.");

            source = item.Source;
            Assert.AreEqual("resource", source.HRef, "Source HRef is incorrect.");
            Assert.AreEqual("de-de", source.Language, "Source Language is incorrect.");

            target = item.Target;
            Assert.AreEqual("resource", target.HRef, "Target HRef is incorrect.");
            Assert.AreEqual("de-de", target.Language, "Target Language is incorrect.");
        }

        /// <summary>
        /// Tests that a <see cref="Segment"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_Segment()
        {
            Segment segment;

            Console.WriteLine("Test with default values.");
            this.Deserialize(TestData.SegmentWithDefaultValues);
            segment = ((Unit)this._document.Files[0].Containers[0]).Resources[0] as Segment;
            Assert.IsNotNull(segment, "Segment is null.");
            Assert.IsTrue(segment.CanResegment, "CanResegment is incorrect.");
            Assert.IsNull(segment.Id, "Id is incorrect.");
            Assert.IsNull(segment.Source, "Source is incorrect.");
            Assert.AreEqual(TranslationState.Initial, segment.State, "State is incorrect.");
            Assert.IsNull(segment.SubState, "SubState is incorrect.");
            Assert.IsNull(segment.Target, "Targetis incorrect.");

            Console.WriteLine("Test with empty values.");
            this.Deserialize(TestData.SegmentWithEmptyValues);
            segment = ((Unit)this._document.Files[0].Containers[0]).Resources[0] as Segment;
            Assert.IsTrue(segment.CanResegment, "CanResegment is incorrect.");
            Assert.IsNull(segment.Id, "Id is incorrect.");
            Assert.IsNull(segment.Source, "Source is incorrect.");
            Assert.AreEqual(TranslationState.Initial, segment.State, "State is incorrect.");
            Assert.AreEqual(String.Empty, segment.SubState, "SubState is incorrect.");
            Assert.IsNull(segment.Target, "Targetis incorrect.");

            Console.WriteLine("Test with valid values.");
            this.Deserialize(TestData.SegmentWithValidValues);
            segment = ((Unit)this._document.Files[0].Containers[0]).Resources[0] as Segment;
            Assert.IsTrue(segment.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual("id", segment.Id, "Id is incorrect.");
            Assert.IsNull(segment.Source, "Source is incorrect.");
            Assert.AreEqual(TranslationState.Reviewed, segment.State, "State is incorrect.");
            Assert.AreEqual("substate", segment.SubState, "SubState is incorrect.");
            Assert.IsNull(segment.Target, "Targetis incorrect.");
        }

        /// <summary>
        /// Tests that a <see cref="Skeleton"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_Skeleton()
        {
            Console.WriteLine("Test with default values.");
            this.Deserialize(TestData.SkeletonWithDefaultValues);
            Assert.IsNull(this._document.Files[0].Skeleton.HRef, "HRef is incorrect.");
            Assert.IsNull(this._document.Files[0].Skeleton.NonTranslatableText, "NonTranslatableText is incorrect.");

            Console.WriteLine("Test with empty values.");
            this.Deserialize(TestData.SkeletonWithEmptyValues);
            Assert.AreEqual(String.Empty, this._document.Files[0].Skeleton.HRef, "HRef is incorrect.");
            Assert.IsNull(this._document.Files[0].Skeleton.NonTranslatableText, "NonTranslatableText is incorrect.");

            Console.WriteLine("Test with empty values.");
            this.Deserialize(TestData.SkeletonWithValidValues);
            Assert.AreEqual("href", this._document.Files[0].Skeleton.HRef, "HRef is incorrect.");
            Assert.AreEqual("text",
                            this._document.Files[0].Skeleton.NonTranslatableText,
                            "NonTranslatableText is incorrect.");
        }

        /// <summary>
        /// Tests that a <see cref="Source"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_Source()
        {
            Source source;

            Console.WriteLine("Test with default values.");
            this.Deserialize(TestData.SourceWithDefaultValues);
            source = ((Unit)this._document.Files[0].Containers[0]).Resources[0].Source;
            Assert.AreEqual(this._document.SourceLanguage, source.Language, "Language is incorrect.");
            Assert.AreEqual(Preservation.Default, source.Space, "Space is incorrect.");

            Console.WriteLine("Test with empty values.");
            this.Deserialize(TestData.SourceWithEmptyValues);
            source = ((Unit)this._document.Files[0].Containers[0]).Resources[0].Source;
            Assert.AreEqual(String.Empty, source.Language, "Language is incorrect.");
            Assert.AreEqual(Preservation.Default, source.Space, "Space is incorrect.");

            Console.WriteLine("Test with valid values.");
            this.Deserialize(TestData.SourceWithValidValues);
            source = ((Unit)this._document.Files[0].Containers[0]).Resources[0].Source;
            Assert.AreEqual("en-us", source.Language, "Language is incorrect.");
            Assert.AreEqual(Preservation.Preserve, source.Space, "Space is incorrect.");
            Assert.AreEqual(3, source.Text.Count, "Text count is incorrect.");
            Assert.AreEqual(string.Empty, ((PlainText)source.Text[0]).Text.Trim(), "Text[0] is incorrect type.");
            Assert.IsInstanceOfType(source.Text[1], typeof(CodePoint), "Text[1] is incorrect type.");
            Assert.AreEqual(string.Empty, ((PlainText)source.Text[2]).Text.Trim(), "Text[2] is incorrect type.");
            Assert.AreEqual(1, ((CodePoint)source.Text[1]).Code, "Code is incorrect.");
        }

        /// <summary>
        /// Tests that a <see cref="Target"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_Target()
        {
            Target target;

            Console.WriteLine("Test with default values.");
            this.Deserialize(TestData.TargetWithDefaultValues);
            target = ((Unit)this._document.Files[0].Containers[0]).Resources[0].Target;
            Assert.IsNull(target.Language, "Language is incorrect.");
            Assert.AreEqual(Preservation.Default, target.Space, "Space is incorrect.");

            Console.WriteLine("Test with empty values.");
            this.Deserialize(TestData.TargetWithEmptyValues);
            target = ((Unit)this._document.Files[0].Containers[0]).Resources[0].Target;
            Assert.AreEqual(String.Empty, target.Language, "Language is incorrect.");
            Assert.AreEqual(Preservation.Default, target.Space, "Space is incorrect.");

            Console.WriteLine("Test with valid values.");
            this.Deserialize(TestData.TargetWithValidValues);
            target = ((Unit)this._document.Files[0].Containers[0]).Resources[0].Target;
            Assert.AreEqual("en-us", target.Language, "Language is incorrect.");
            Assert.AreEqual(100u, target.Order, "Order is incorrect.");
            Assert.AreEqual(Preservation.Preserve, target.Space, "Space is incorrect.");
            Assert.AreEqual(3, target.Text.Count, "Text count is incorrect.");
            Assert.AreEqual(string.Empty, ((PlainText)target.Text[0]).Text.Trim(), "Text[0] is incorrect type.");
            Assert.IsInstanceOfType(target.Text[1], typeof(CodePoint), "Text[1] is incorrect type.");
            Assert.AreEqual(string.Empty, ((PlainText)target.Text[2]).Text.Trim(), "Text[2] is incorrect type.");
            Assert.AreEqual(1, ((CodePoint)target.Text[1]).Code, "Code is incorrect.");
        }

        /// <summary>
        /// Tests that a <see cref="Unit"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_Unit()
        {
            Unit unit;

            Console.WriteLine("Test with default values.");
            this.Deserialize(TestData.UnitWithDefaultValues);
            unit = this._document.Files[0].Containers[0] as Unit;
            Assert.IsNotNull(unit, "Unit is null.");
            Assert.IsTrue(unit.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual(String.Empty, unit.Id, "Id is incorrect.");
            Assert.IsNull(unit.Name, "Name is incorrect.");
            Assert.AreEqual(0, unit.Resources.Count, "Resources count is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            unit.SourceDirectionality,
                            "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Default, unit.Space, "Space is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            unit.TargetDirectionality,
                            "TargetDirectionality is incorrect.");
            Assert.IsTrue(unit.Translate, "Translate is incorrect.");
            Assert.IsNull(unit.Type, "Type is incorrect.");

            Console.WriteLine("Test with empty values.");
            this.Deserialize(TestData.UnitWithEmptyValues);
            unit = this._document.Files[0].Containers[0] as Unit;
            Assert.IsNotNull(unit, "Unit is null.");
            Assert.IsTrue(unit.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual(String.Empty, unit.Id, "Id is incorrect.");
            Assert.AreEqual(String.Empty, unit.Name, "Name is incorrect.");
            Assert.AreEqual(0, unit.Resources.Count, "Resources count is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            unit.SourceDirectionality,
                            "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Default, unit.Space, "Space is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            unit.TargetDirectionality,
                            "TargetDirectionality is incorrect.");
            Assert.IsTrue(unit.Translate, "Translate is incorrect.");
            Assert.AreEqual(String.Empty, unit.Type, "Type is incorrect.");

            Console.WriteLine("Test with valid values.");
            this.Deserialize(TestData.UnitWithValidValues);
            unit = this._document.Files[0].Containers[0] as Unit;
            Assert.IsNotNull(unit, "Unit is null.");
            Assert.IsFalse(unit.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual("id", unit.Id, "Id is incorrect.");
            Assert.AreEqual("name", unit.Name, "Name is incorrect.");
            Assert.AreEqual(0, unit.Resources.Count, "Resources count is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            unit.SourceDirectionality,
                            "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Default, unit.Space, "Space is incorrect.");
            Assert.AreEqual(ContentDirectionality.LTR,
                            unit.TargetDirectionality,
                            "TargetDirectionality is incorrect.");
            Assert.IsTrue(unit.Translate, "Translate is incorrect.");
            Assert.AreEqual("type", unit.Type, "Type is incorrect.");
        }

        /// <summary>
        /// Tests that <see cref="Note"/>s in a <see cref="Unit"/> deserialize correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_UnitNotes()
        {
            Unit unit;

            Console.WriteLine("Test with default values.");
            this.Deserialize(TestData.UnitNoteWithDefaultValues);
            unit = this._document.Files[0].Containers[0] as Unit;
            Assert.IsNotNull(unit, "Unit is null.");
            Assert.IsTrue(unit.HasNotes, "HasNotes is incorrect.");
            Assert.IsNull(unit.Notes[0].AppliesTo, "AppliesTo is incorrect.");
            Assert.IsNull(unit.Notes[0].Category, "Category is incorrect.");
            Assert.IsNull(unit.Notes[0].Id, "Id is incorrect.");
            Assert.AreEqual(1, unit.Notes[0].Priority, "Priority is incorrect.");
            Assert.IsNull(unit.Notes[0].Text, "Text is incorrect.");

            Console.WriteLine("Test with empty values.");
            this.Deserialize(TestData.UnitNoteWithEmptyValues);
            unit = this._document.Files[0].Containers[0] as Unit;
            Assert.IsNotNull(unit, "Unit is null.");
            Assert.IsTrue(unit.HasNotes, "HasNotes is incorrect.");
            Assert.IsNull(unit.Notes[0].AppliesTo, "AppliesTo is incorrect.");
            Assert.AreEqual(String.Empty, unit.Notes[0].Category, "Category is incorrect.");
            Assert.IsNull(unit.Notes[0].Id, "Id is incorrect.");
            Assert.AreEqual(1, unit.Notes[0].Priority, "Priority is incorrect.");
            Assert.IsNull(unit.Notes[0].Text, "Text is incorrect.");

            Console.WriteLine("Test with valid values.");
            this.Deserialize(TestData.UnitNoteWithValidValues);
            unit = this._document.Files[0].Containers[0] as Unit;
            Assert.IsNotNull(unit, "Unit is null.");
            Assert.IsTrue(unit.HasNotes, "HasNotes is incorrect.");
            Assert.AreEqual(TranslationSubject.Target, unit.Notes[0].AppliesTo, "AppliesTo is incorrect.");
            Assert.AreEqual("category", unit.Notes[0].Category, "Category is incorrect.");
            Assert.AreEqual("id", unit.Notes[0].Id, "Id is incorrect.");
            Assert.AreEqual(100, unit.Notes[0].Priority, "Priority is incorrect.");
            Assert.AreEqual("text", unit.Notes[0].Text, "Text is incorrect.");
        }

        /// <summary>
        /// Tests that an <see cref="Validation"/> deserializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffReader_Validation()
        {
            Rule rule;
            Validation validation;

            this.Deserialize(TestData.FileWithValidation);

            validation = this._document.Files[0].ValidationRules;
            Assert.IsNotNull(validation, "Validation is null.");
            Assert.IsNotNull(validation.Rules, "Rules is null.");
            Assert.AreEqual(4, validation.Rules.Count, "Rules count is incorrect.");

            rule = validation.Rules[0];
            Assert.AreEqual(true, rule.CaseSensitive, "Rules[0] CaseSensitive is incorrect.");
            Assert.AreEqual(false, rule.Disabled, "Rules[0] Disabled is incorrect.");
            Assert.AreEqual("endswith", rule.EndsWith, "Rules[0] EndsWith is incorrect.");
            Assert.AreEqual(true, rule.ExistsInSource, "Rules[0] ExistsInSource is incorrect.");
            Assert.IsNull(rule.IsNotPresent, "Rules[0] IsNotPresent is not null.");
            Assert.IsNull(rule.IsPresent, "Rules[0] IsPresent is not null.");
            Assert.AreEqual(NormalizationValue.NFC, rule.Normalization, "Rules[0] Normalization is incorrect.");
            Assert.AreEqual(2u, rule.Occurs, "Rules[0] Occurs is incorrect.");
            Assert.IsNull(rule.StartsWith, "Rules[0] is not null.");

            rule = validation.Rules[1];
            Assert.AreEqual(true, rule.CaseSensitive, "Rules[1] CaseSensitive is incorrect.");
            Assert.AreEqual(false, rule.Disabled, "Rules[1] Disabled is incorrect.");
            Assert.IsNull(rule.EndsWith, "Rules[1] EndsWith is not null.");
            Assert.AreEqual(true, rule.ExistsInSource, "Rules[1] ExistsInSource is incorrect.");
            Assert.AreEqual("isnotpresent", rule.IsNotPresent, "Rules[1] IsNotPresent is incorrect.");
            Assert.IsNull(rule.IsPresent, "Rules[1] IsPresent is not null.");
            Assert.AreEqual(NormalizationValue.NFC, rule.Normalization, "Rules[1] Normalization is incorrect.");
            Assert.AreEqual(2u, rule.Occurs, "Rules[1] Occurs is incorrect.");
            Assert.IsNull(rule.StartsWith, "Rules[1] is not null.");

            rule = validation.Rules[2];
            Assert.AreEqual(true, rule.CaseSensitive, "Rules[2] CaseSensitive is incorrect.");
            Assert.AreEqual(false, rule.Disabled, "Rules[2] Disabled is incorrect.");
            Assert.IsNull(rule.EndsWith, "Rules[2] EndsWith is not null.");
            Assert.AreEqual(true, rule.ExistsInSource, "Rules[2] ExistsInSource is incorrect.");
            Assert.IsNull(rule.IsNotPresent, "Rules[2] IsNotPresent is not null.");
            Assert.AreEqual("ispresent", rule.IsPresent, "Rules[2] IsPresent is incorrect.");
            Assert.AreEqual(NormalizationValue.NFC, rule.Normalization, "Rules[2] Normalization is incorrect.");
            Assert.AreEqual(2u, rule.Occurs, "Rules[2] Occurs is incorrect.");
            Assert.IsNull(rule.StartsWith, "Rules[2] is not null.");

            rule = validation.Rules[3];
            Assert.AreEqual(true, rule.CaseSensitive, "Rules[3] CaseSensitive is incorrect.");
            Assert.AreEqual(false, rule.Disabled, "Rules[3] Disabled is incorrect.");
            Assert.IsNull(rule.EndsWith, "Rules[2] EndsWith is not null.");
            Assert.AreEqual(true, rule.ExistsInSource, "Rules[3] ExistsInSource is incorrect.");
            Assert.IsNull(rule.IsNotPresent, "Rules[3] IsNotPresent is not null.");
            Assert.IsNull(rule.IsPresent, "Rules[3] IsPresent is not null.");
            Assert.AreEqual(NormalizationValue.NFC, rule.Normalization, "Rules[3] Normalization is incorrect.");
            Assert.AreEqual(2u, rule.Occurs, "Rules[3] Occurs is incorrect.");
            Assert.AreEqual("startswith", rule.StartsWith, "Rules[3] is incorrect.");
        }
        #endregion Test Methods

        #region Helper Methods
        /// <summary>
        /// Deserializes a file and stores the resulting <see cref="XliffDocument"/> internally.
        /// </summary>
        /// <param name="data">The identifier of the document to deserialize.</param>
        private void Deserialize(TestData data)
        {
            string path;

            path = System.IO.Path.Combine(Environment.CurrentDirectory, TestUtilities.TestDataDirectory, data.ToString() + ".xlf");
            using (System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                this._document = this._reader.Deserialize(stream);
            }

            Assert.IsNotNull(this._document, "Failed to deserialize.");
        }
        #endregion Helper Methods
    }
}
