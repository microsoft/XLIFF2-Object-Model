namespace Localization.Xliff.OM.Modules.TranslationCandidates.Tests
{
    using Localization.Xliff.OM.Modules.TranslationCandidates;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the default values for all Translation Candidates module classes.
    /// </summary>
    [TestClass()]
    public class TranslationCandidatesDefaultValueTests
    {
        #region Test Methods
        /// <summary>
        /// Tests that defaults for <see cref="Match"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void TranslationCandidatesDefaultValue_Match()
        {
            Match match;

            match = new Match();
            Assert.IsFalse(match.HasReferenceTranslation, "HasReferenceTranslation is incorrect.");
            Assert.IsNull(match.Id, "Id is incorrect.");
            Assert.IsNull(match.MatchQuality, "MatchQuality is incorrect.");
            Assert.IsNull(match.MatchSuitability, "MatchSuitability is incorrect.");
            Assert.IsNull(match.Metadata, "Metadata is incorrect.");
            Assert.IsNull(match.Origin, "Origin is incorrect.");
            Assert.IsNull(match.OriginalData, "OriginalData is incorrect.");
            Assert.IsNull(match.Similarity, "Similarity is incorrect.");
            Assert.IsNull(match.Source, "Source is incorrect.");
            Assert.IsNull(match.SourceReference, "SourceReference is incorrect.");
            Assert.IsNull(match.SubType, "SubType is incorrect.");
            Assert.IsNull(match.Target, "Target is incorrect.");
            Assert.AreEqual(MatchType.TranslationMemory, match.Type, "Type is incorrect.");

            match = new Match("sourcereference");
            Assert.IsFalse(match.HasReferenceTranslation, "HasReferenceTranslation is incorrect.");
            Assert.IsNull(match.Id, "Id is incorrect.");
            Assert.IsNull(match.MatchQuality, "MatchQuality is incorrect.");
            Assert.IsNull(match.MatchSuitability, "MatchSuitability is incorrect.");
            Assert.IsNull(match.Metadata, "Metadata is incorrect.");
            Assert.IsNull(match.Origin, "Origin is incorrect.");
            Assert.IsNull(match.OriginalData, "OriginalData is incorrect.");
            Assert.IsNull(match.Similarity, "Similarity is incorrect.");
            Assert.IsNull(match.Source, "Source is incorrect.");
            Assert.AreEqual("sourcereference", match.SourceReference, "SourceReference is incorrect.");
            Assert.IsNull(match.SubType, "SubType is incorrect.");
            Assert.IsNull(match.Target, "Target is incorrect.");
            Assert.AreEqual(MatchType.TranslationMemory, match.Type, "Type is incorrect.");
        }
        #endregion Test Methods
    }
}
