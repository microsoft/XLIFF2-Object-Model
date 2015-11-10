namespace Localization.Xliff.OM.Modules.ChangeTracking.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization.Xliff.OM.Modules.ChangeTracking;
    using Localization.Xliff.OM.Modules.ChangeTracking.XmlNames;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="Revision"/> class.
    /// </summary>
    [TestClass]
    public class RevisionTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private Revision element;

        /// <summary>
        /// The provider to the element data.
        /// </summary>
        private IXliffDataProvider provider;

        /// <summary>
        /// Initializes the test class before every test method is executed.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.element = new Revision();
            this.provider = this.element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Revision_IXliffDataProvider_GetXliffAttributes()
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
                        this.element.Author = "author";
                        this.element.ChangeDate = DateTime.Now;
                        this.element.Version = "version";
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this.provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(3, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this.element.Author,
                                attributes.First((a) => a.LocalName == AttributeNames.Author).Value,
                                "Author is incorrect.");

                Assert.AreEqual(this.element.ChangeDate,
                                attributes.First((a) => a.LocalName == AttributeNames.DateTime).Value,
                                "ChangeDate is incorrect.");

                Assert.AreEqual(this.element.Version,
                                attributes.First((a) => a.LocalName == AttributeNames.Version).Value,
                                "Version is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Revision_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;
            
            Console.WriteLine("Test with a no children.");
            Assert.IsNull(this.provider.GetXliffChildren(), "Children is not null.");

            Console.WriteLine("Test with children.");
            this.element.Items.Add(new Item());
            this.element.Items.Add(new Item());
            children = this.provider.GetXliffChildren().ToList();
            Assert.AreEqual(2, children.Count, "Incorrect number of children.");
            TestUtilities.VerifyItems<Item>(this.element.Items, children, ElementNames.Item);

            Console.WriteLine("Test with a children removed.");
            this.element.Items.RemoveAt(0);
            children = this.provider.GetXliffChildren().ToList();
            Assert.AreEqual(1, children.Count(), "Incorrect number of children.");
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Revision_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this.provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
