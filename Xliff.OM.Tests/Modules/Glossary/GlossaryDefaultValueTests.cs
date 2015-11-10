namespace Localization.Xliff.OM.Modules.ChangeTracking.Tests
{
    using Localization.Xliff.OM.Modules.Glossary;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the default values for all Glossary module classes.
    /// </summary>
    [TestClass()]
    public class GlossaryDefaultValueTests
    {
        #region Test Methods
        /// <summary>
        /// Tests that defaults for <see cref="Definition"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void GlossaryDefaultValue_Definition()
        {
            Definition definition;

            definition = new Definition();
            Assert.IsNull(definition.Source, "Source is incorrect.");
            Assert.IsNull(definition.Text, "Text is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="GlossaryEntry"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void GlossaryDefaultValue_GlossaryEntry()
        {
            GlossaryEntry entry;

            entry = new GlossaryEntry();
            Assert.IsNull(entry.Definition, "Definition is incorrect.");
            Assert.IsNull(entry.Id, "Id is incorrect.");
            Assert.IsNull(entry.Reference, "Reference is incorrect.");
            Assert.IsNotNull(entry.Term, "Term is incorrect.");
            Assert.AreEqual(0, entry.Translations.Count, "Translations count is incorrect.");

            entry = new GlossaryEntry("id");
            Assert.IsNull(entry.Definition, "Definition is incorrect.");
            Assert.AreEqual("id", entry.Id, "Id is incorrect.");
            Assert.IsNull(entry.Reference, "Reference is incorrect.");
            Assert.IsNotNull(entry.Term, "Term is incorrect.");
            Assert.AreEqual(0, entry.Translations.Count, "Translations count is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Glossary"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void GlossaryDefaultValue_Glossary()
        {
            Glossary glossary;

            glossary = new Glossary();
            Assert.AreEqual(0, glossary.Entries.Count, "Entries count is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Term"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void GlossaryDefaultValue_Term()
        {
            Term term;

            term = new Term();
            Assert.IsNull(term.Source, "Source is incorrect.");
            Assert.IsNull(term.Text, "Text is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Translation"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void GlossaryDefaultValue_Translation()
        {
            Translation translation;

            translation = new Translation("id");
            Assert.AreEqual("id", translation.Id, "Id is incorrect.");
            Assert.IsNull(translation.Reference, "Reference is incorrect.");
            Assert.IsNull(translation.Source, "Source is incorrect.");
            Assert.IsNull(translation.Text, "Text is incorrect.");
        }
        #endregion Test Methods
    }
}
