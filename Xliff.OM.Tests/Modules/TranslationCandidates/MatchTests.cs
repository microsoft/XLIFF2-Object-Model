namespace Localization.Xliff.OM.Modules.TranslationCandidates.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Modules.Metadata;
    using Localization.Xliff.OM.Modules.TranslationCandidates;
    using Localization.Xliff.OM.Modules.TranslationCandidates.XmlNames;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="Match"/> class.
    /// </summary>
    [TestClass()]
    public class MatchTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private Match _element;

        /// <summary>
        /// The provider to the element data.
        /// </summary>
        private IXliffDataProvider _provider;

        /// <summary>
        /// Initializes the test class before every test method is executed.
        /// </summary>
        [TestInitialize()]
        public void TestInitialize()
        {
            this._element = new Match();
            this._provider = this._element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Match_IXliffDataProvider_GetXliffAttributes()
        {
            for (int i = 0; i < 2; i++)
            {
                IEnumerable<IAttributeDataProvider> attributes;

                switch (i)
                {
                    case 0:
                        Console.WriteLine("Test with default element.");
                        break;

                    case 1:
                        Console.WriteLine("Test with modified element.");
                        this._element.HasReferenceTranslation = true;
                        this._element.Id = "id";
                        this._element.MatchQuality = 40;
                        this._element.MatchSuitability = 38;
                        this._element.Metadata = new MetadataContainer();
                        this._element.Origin = "origin";
                        this._element.Similarity = 32;
                        this._element.Source = new Source();
                        this._element.SourceReference = Utilities.MakeIri("ref");
                        this._element.SubType = "subtype";
                        this._element.Target = new Target();
                        this._element.Type = MatchType.TermBase;
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this._provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(9, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this._element.HasReferenceTranslation,
                                attributes.First((a) => a.LocalName == AttributeNames.Reference).Value,
                                "HasReferenceTranslation is incorrect.");
                Assert.AreEqual(this._element.Id,
                                attributes.First((a) => a.LocalName == AttributeNames.Id).Value,
                                "Id is incorrect.");
                Assert.AreEqual(this._element.MatchQuality,
                                attributes.First((a) => a.LocalName == AttributeNames.MatchQuality).Value,
                                "MatchQuality is incorrect.");
                Assert.AreEqual(this._element.MatchSuitability,
                                attributes.First((a) => a.LocalName == AttributeNames.MatchSuitability).Value,
                                "MatchSuitability is incorrect.");
                Assert.AreEqual(this._element.Origin,
                                attributes.First((a) => a.LocalName == AttributeNames.Origin).Value,
                                "Origin is incorrect.");
                Assert.AreEqual(this._element.Similarity,
                                attributes.First((a) => a.LocalName == AttributeNames.Similarity).Value,
                                "Similarity is incorrect.");
                Assert.AreEqual(this._element.SourceReference,
                                attributes.First((a) => a.LocalName == AttributeNames.SourceReference).Value,
                                "SourceReference is incorrect.");
                Assert.AreEqual(this._element.SubType,
                                attributes.First((a) => a.LocalName == AttributeNames.SubType).Value,
                                "SubType is incorrect.");
                Assert.AreEqual(this._element.Type,
                                attributes.First((a) => a.LocalName == AttributeNames.Type).Value,
                                "Type is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Match_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;
            string prefix;
            string value;

            Utilities.SplitPrefixValue(ElementNames.Metadata, ':', out prefix, out value);

            Console.WriteLine("Test with a no children.");
            children = this._provider.GetXliffChildren().ToList();
            Assert.IsTrue(((children == null) || (children.Count == 0)), "Children is not null.");

            Console.WriteLine("Test with children.");
            this._element.Metadata = new MetadataContainer();
            this._element.Source = new Source();
            this._element.Target = new Target();
            children = this._provider.GetXliffChildren().ToList();
            Assert.AreEqual(3, children.Count, "Incorrect number of children.");
            TestUtilities.VerifyItems<Source>(this._element.Source, children, ElementNames.Source);
            TestUtilities.VerifyItems<Target>(this._element.Target, children, ElementNames.Target);
            TestUtilities.VerifyItems<MetadataContainer>(this._element.Metadata, children, value);
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Match_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this._provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
