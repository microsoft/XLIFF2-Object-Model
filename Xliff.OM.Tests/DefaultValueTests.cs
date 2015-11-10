namespace Localization.Xliff.OM.Core.Tests
{
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the default values for all <see cref="XliffElement"/> classes.
    /// </summary>
    [TestClass()]
    public class DefaultValueTests
    {
        #region Test Methods
        /// <summary>
        /// Tests that defaults for <see cref="Data"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void DefaultValue_Data()
        {
            Data data;

            data = new Data();
            Assert.AreEqual(ContentDirectionality.Auto, data.Directionality, "Directionality is incorrect.");
            Assert.IsNull(data.Id, "Id is incorrect.");
            Assert.AreEqual(Preservation.Preserve, data.Space, "Space is incorrect.");
            Assert.IsNotNull(data.Text, "Text is null.");
            Assert.AreEqual(0, data.Text.Count, "Text count is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="XliffDocument"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void DefaultValue_Document()
        {
            XliffDocument document;

            document = new XliffDocument("en-us");
            Assert.AreEqual(0, document.Files.Count, "Files count is incorrect.");
            Assert.IsNull(document.Id, "Id is incorrect.");
            Assert.AreEqual("en-us", document.SourceLanguage, "SourceLanguage is incorrect.");
            Assert.AreEqual(Preservation.Default, document.Space, "Space is incorrect.");
            Assert.IsNull(document.TargetLanguage, "TargetLanguage is incorrect.");
            Assert.AreEqual("2.0", document.Version, "Version is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="File"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void DefaultValue_File()
        {
            File file;
            XliffDocument document;

            file = new File("id");
            document = new XliffDocument();
            document.Space = Preservation.Preserve;
            document.Files.Add(file);

            Assert.IsTrue(file.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual(0, file.Containers.Count, "Containers count is incorrect.");
            Assert.IsFalse(file.HasNotes, "HasNotes is incorrect.");
            Assert.AreEqual("id", file.Id, "Id is incorrect.");
            Assert.AreEqual(0, file.Notes.Count, "Notes count is incorrect.");
            Assert.IsNull(file.Original, "Original is incorrect.");
            Assert.IsNull(file.Skeleton, "Skeleton is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            file.SourceDirectionality,
                            "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Preserve, file.Space, "Space is incorrect");
            Assert.AreEqual(ContentDirectionality.Auto,
                            file.TargetDirectionality,
                            "TargetDirectionality is incorrect.");
            Assert.IsTrue(file.Translate, "Translate is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Group"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void DefaultValue_Group()
        {
            File file;
            Group group;

            file = new File();
            file.CanResegment = false;
            file.Space = Preservation.Preserve;
            group = new Group("id");
            file.Containers.Add(group);

            Assert.IsFalse(group.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual(0, group.Containers.Count, "Containers count is incorrect.");
            Assert.IsFalse(group.HasNotes, "HasNotes is incorrect.");
            Assert.AreEqual("id", group.Id, "Id is incorrect.");
            Assert.IsNull(group.Name, "Name is incorrect.");
            Assert.AreEqual(0, group.Notes.Count, "Notes count is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            group.SourceDirectionality,
                            "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Preserve, group.Space, "Space is incorrect");
            Assert.AreEqual(ContentDirectionality.Auto,
                            group.TargetDirectionality,
                            "TargetDirectionality is incorrect.");
            Assert.IsTrue(group.Translate, "Translate is incorrect.");
            Assert.IsNull(group.Type, "Type is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Ignorable"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void DefaultValue_Ignorable()
        {
            Ignorable ignorable;

            ignorable = new Ignorable("id");
            Assert.AreEqual("id", ignorable.Id, "Id is incorrect.");
            Assert.IsNull(ignorable.Source, "Source is incorrect.");
            Assert.IsNull(ignorable.Target, "Target is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Note"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void DefaultValue_Note()
        {
            Note note;

            note = new Note("text");
            Assert.IsNull(note.AppliesTo, "AppliesTo is incorrect.");
            Assert.IsNull(note.Category, "Category is incorrect.");
            Assert.IsNull(note.Id, "Id is incorrect.");
            Assert.AreEqual(1, note.Priority, "Priority is incorrect.");
            Assert.AreEqual("text", note.Text, "Text is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="OriginalData"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void DefaultValue_OriginalData()
        {
            OriginalData data;

            data = new OriginalData();
            Assert.IsNotNull(data.DataElements, "DataElements is null.");
            Assert.AreEqual(0, data.DataElements.Count, "DataElements count is incorrect.");
            Assert.IsFalse(data.HasData, "HasData is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Segment"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void DefaultValue_Segment()
        {
            Segment segment;
            Unit unit;

            unit = new Unit();
            unit.CanResegment = false;
            segment = new Segment("id");
            unit.Resources.Add(segment);

            Assert.IsFalse(segment.CanResegment, "CanResegment is incorrect.");
            Assert.AreEqual("id", segment.Id, "Id is incorrect.");
            Assert.IsNull(segment.Source, "Source is incorrect.");
            Assert.AreEqual(TranslationState.Initial, segment.State, "State is incorrect.");
            Assert.IsNull(segment.SubState, "SubState is incorrect.");
            Assert.IsNull(segment.Target, "Target is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Skeleton"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void DefaultValue_Skeleton()
        {
            Skeleton skeleton;

            skeleton = new Skeleton();
            Assert.IsNull(skeleton.HRef, "HRef is incorrect.");
            Assert.IsNull(skeleton.NonTranslatableText, "NonTranslatableText is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Source"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void DefaultValue_Source()
        {
            Unit unit;
            Source source;

            unit = new Unit();
            unit.Space = Preservation.Preserve;
            source = new Source();
            unit.Resources.Add(new Segment());
            unit.Resources[0].Source = source;

            Assert.IsNull(source.Language, "Language is incorrect.");
            Assert.AreEqual(Preservation.Preserve, source.Space, "Space is incorrect.");
            Assert.AreEqual(0, source.Text.Count, "Text count is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Target"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void DefaultValue_Target()
        {
            Unit unit;
            Target target;

            unit = new Unit();
            unit.Space = Preservation.Preserve;
            target = new Target();
            unit.Resources.Add(new Segment());
            unit.Resources[0].Target = target;

            Assert.IsNull(target.Language, "Language is incorrect.");
            Assert.IsNull(target.Order, "Order is incorrect.");
            Assert.AreEqual(Preservation.Preserve, target.Space, "Space is incorrect.");
            Assert.AreEqual(0, target.Text.Count, "Text count is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Unit"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void DefaultValue_Unit()
        {
            File file;
            Unit unit;

            file = new File();
            file.CanResegment = false;
            file.Space = Preservation.Preserve;
            unit = new Unit("id");
            file.Containers.Add(unit);

            Assert.IsFalse(unit.CanResegment, "CanResegment is incorrect.");
            Assert.IsFalse(unit.HasNotes, "HasNotes is incorrect.");
            Assert.AreEqual("id", unit.Id, "Id is incorrect.");
            Assert.IsNull(unit.Name, "Name is incorrect.");
            Assert.AreEqual(0, unit.Notes.Count, "Notes count is incorrect.");
            Assert.AreEqual(0, unit.Resources.Count, "Resources count is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto,
                            unit.SourceDirectionality,
                            "SourceDirectionality is incorrect.");
            Assert.AreEqual(Preservation.Preserve, unit.Space, "Space is incorrect");
            Assert.AreEqual(ContentDirectionality.Auto,
                            unit.TargetDirectionality,
                            "TargetDirectionality is incorrect.");
            Assert.IsTrue(unit.Translate, "Translate is incorrect.");
            Assert.IsNull(unit.Type, "Type is incorrect.");
        }
        #endregion Test Methods
    }
}
