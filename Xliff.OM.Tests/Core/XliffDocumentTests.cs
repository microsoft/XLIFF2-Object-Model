namespace Localization.Xliff.OM.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="XliffDocument"/> class.
    /// </summary>
    [TestClass()]
    public class XliffDocumentTests
    {
        /// <summary>
        /// The document under test.
        /// </summary>
        private XliffDocument _element;

        /// <summary>
        /// The provider to the document data.
        /// </summary>
        private IXliffDataProvider _provider;

        /// <summary>
        /// Initializes the test class before every test method is executed.
        /// </summary>
        [TestInitialize()]
        public void TestInitialize()
        {
            this._element = new XliffDocument("source");
            this._provider = this._element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffDocument_IXliffDataProvider_GetXliffAttributes()
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
                        Console.WriteLine("Test with modified document.");
                        this._element.SourceLanguage = Guid.NewGuid().ToString();
                        this._element.TargetLanguage = Guid.NewGuid().ToString();
                        this._element.Version = Guid.NewGuid().ToString();
                        this._element.Space = Preservation.Preserve;
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                //
                // Test with a default element.
                //

                Console.WriteLine("Test with default element.");

                attributes = this._provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(4, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this._element.SourceLanguage,
                                attributes.First((a) => a.LocalName == AttributeNames.SourceLanguage).Value,
                                "SourceLanguage is incorrect.");
                Assert.AreEqual(this._element.TargetLanguage,
                                attributes.First((a) => a.LocalName == AttributeNames.TargetLanguage).Value,
                                "TargetLanguage is incorrect.");
                Assert.AreEqual(this._element.Version,
                                attributes.First((a) => a.LocalName == AttributeNames.Version).Value,
                                "Version is incorrect.");
                Assert.AreEqual(this._element.Space,
                                attributes.First((a) => a.LocalName == AttributeNames.SpacePreservation).Value,
                                "WhiteSpacePreservation is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffDocument_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;

            Console.WriteLine("Test with a no children.");
            Assert.IsNull(this._provider.GetXliffChildren(), "Children is not null.");

            Console.WriteLine("Test with a multiple children.");
            this._element.Files.Add(new File("file1"));
            this._element.Files.Add(new File("file2"));
            children = this._provider.GetXliffChildren().ToList();
            TestUtilities.VerifyItems(this._element.Files, children, ElementNames.File);

            Console.WriteLine("Test with a children removed.");
            this._element.Files.RemoveAt(0);
            children = this._provider.GetXliffChildren().ToList();
            TestUtilities.VerifyItems(this._element.Files, children, ElementNames.File);
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffDocument_IXliffDataProvider_GetXliffText()
        {
            Assert.IsNull(this._provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
