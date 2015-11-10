namespace Localization.Xliff.OM.Modules.ResourceData.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization.Xliff.OM.Modules.ResourceData;
    using Localization.Xliff.OM.Modules.ResourceData.XmlNames;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="ResourceItemRef"/> class.
    /// </summary>
    [TestClass]
    public class ResourceItemRefTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private ResourceItemRef element;

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
            this.element = new ResourceItemRef();
            this.provider = this.element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ResourceItemRef_IXliffDataProvider_GetXliffAttributes()
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
                        this.element.Id = "id";
                        this.element.Reference = "reference";
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this.provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(2, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this.element.Id,
                                attributes.First((a) => a.LocalName == AttributeNames.Id).Value,
                                "Id is incorrect.");

                Assert.AreEqual(this.element.Reference,
                                attributes.First((a) => a.LocalName == AttributeNames.ReferenceAbbreviated).Value,
                                "Reference is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ResourceItemRef_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;
            
            Console.WriteLine("Test with a no children.");
            children = this.provider.GetXliffChildren().ToList();
            Assert.IsTrue(((children == null) || (children.Count() == 0)), "Children is not null.");
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ResourceItemRef_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this.provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
