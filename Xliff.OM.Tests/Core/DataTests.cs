namespace Localization.Xliff.OM.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="Data"/> class.
    /// </summary>
    [TestClass()]
    public class DataTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private Data _element;

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
            this._element = new Data();
            this._provider = this._element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Data_IXliffDataProvider_GetXliffAttributes()
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
                        this._element.Directionality = ContentDirectionality.RTL;
                        this._element.Id = "id";
                        this._element.Space = Preservation.Preserve;
                        this._element.Text.Add(new PlainText("text"));
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this._provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(3, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this._element.Directionality,
                                attributes.First((a) => a.LocalName == AttributeNames.Directionality).Value,
                                "Directionality is incorrect.");
                Assert.AreEqual(this._element.Id,
                                attributes.First((a) => a.LocalName == AttributeNames.Id).Value,
                                "Id is incorrect.");
                Assert.AreEqual(this._element.Space,
                                attributes.First((a) => a.LocalName == AttributeNames.SpacePreservation).Value,
                                "Space is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Data_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;

            Console.WriteLine("Test with a no children.");
            Assert.IsNull(this._provider.GetXliffChildren(), "Children is not null.");

            Console.WriteLine("Test with children.");
            this._element.Text.Add(new CodePoint());
            this._element.Text.Add(new PlainText());
            children = this._provider.GetXliffChildren().ToList();
            Assert.AreEqual(2, children.Count(), "Incorrect number of children.");
            TestUtilities.VerifyItems<CodePoint>(this._element.Text, children, ElementNames.CodePoint);
            TestUtilities.VerifyItems<PlainText>(this._element.Text, children, ElementNames.PlainText);

            Console.WriteLine("Test with a children removed.");
            this._element.Text.RemoveAt(0);
            children = this._provider.GetXliffChildren().ToList();
            Assert.AreEqual(1, children.Count(), "Incorrect number of children.");
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Data_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this._provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
