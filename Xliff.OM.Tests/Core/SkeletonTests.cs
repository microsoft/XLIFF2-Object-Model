namespace Localization.Xliff.OM.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="Skeleton"/> class.
    /// </summary>
    [TestClass()]
    public class SkeletonTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private Skeleton _element;

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
            this._element = new Skeleton();
            this._provider = this._element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Skeleton_IXliffDataProvider_GetXliffAttributes()
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
                        this._element.HRef = Guid.NewGuid().ToString();
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this._provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(1, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this._element.HRef,
                                attributes.First((a) => a.LocalName == AttributeNames.HRef).Value,
                                "HRef is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Skeleton_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;

            Console.WriteLine("Test with a no children.");
            children = this._provider.GetXliffChildren().ToList();
            Assert.IsTrue(((children == null) || (children.Count == 0)), "Children is not null.");
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Skeleton_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this._provider.GetXliffText(), "Text is not null.");

            Console.WriteLine("Test with text.");
            this._element.NonTranslatableText = Guid.NewGuid().ToString();
            Assert.AreEqual(this._element.NonTranslatableText, this._provider.GetXliffText(), "Text is incorrect.");
        }
        #endregion Test Methods
    }
}
