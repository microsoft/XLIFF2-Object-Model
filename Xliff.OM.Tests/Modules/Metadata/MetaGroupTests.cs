namespace Localization.Xliff.OM.Modules.Metadata.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization.Xliff.OM.Modules.Metadata.XmlNames;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="MetaGroup"/> class.
    /// </summary>
    [TestClass()]
    public class MetaGroupTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private MetaGroup _element;

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
            this._element = new MetaGroup();
            this._provider = this._element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void MetaGroup_IXliffDataProvider_GetXliffAttributes()
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
                        this._element.AppliesTo = MetaGroupSubject.Ignorable;
                        this._element.Category = "category";
                        this._element.Containers.Add(new Meta());
                        this._element.Id = "id";
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this._provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(3, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this._element.AppliesTo,
                                attributes.First((a) => a.LocalName == AttributeNames.AppliesTo).Value,
                                "AppliesTo is incorrect.");
                Assert.AreEqual(this._element.Category,
                                attributes.First((a) => a.LocalName == AttributeNames.Category).Value,
                                "Category is incorrect.");
                Assert.AreEqual(this._element.Id,
                                attributes.First((a) => a.LocalName == AttributeNames.Id).Value,
                                "Id is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void MetaGroup_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;
            string prefix;
            string value;

            Utilities.SplitPrefixValue(ElementNames.Meta, ':', out prefix, out value);

            Console.WriteLine("Test with a no children.");
            children = this._provider.GetXliffChildren().ToList();
            Assert.IsTrue(((children == null) || (children.Count == 0)), "Children is not null.");

            Console.WriteLine("Test with children.");
            this._element.Containers.Add(new Meta());
            children = this._provider.GetXliffChildren().ToList();
            Assert.AreEqual(1, children.Count, "Incorrect number of children.");
            TestUtilities.VerifyItems<Meta>(this._element.Containers, children, value);
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void MetaGroup_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this._provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
