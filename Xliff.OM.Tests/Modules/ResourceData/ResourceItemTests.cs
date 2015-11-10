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
    /// This class tests the <see cref="ResourceItem"/> class.
    /// </summary>
    [TestClass]
    public class ResourceItemTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private ResourceItem element;

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
            this.element = new ResourceItem();
            this.provider = this.element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ResourceItem_IXliffDataProvider_GetXliffAttributes()
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
                        this.element.Context = !this.element.Context;
                        this.element.Id = "id";
                        this.element.MimeType = "mimetype";
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this.provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(3, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this.element.Context,
                                attributes.First((a) => a.LocalName == AttributeNames.Context).Value,
                                "Context is incorrect.");

                Assert.AreEqual(this.element.Id,
                                attributes.First((a) => a.LocalName == AttributeNames.Id).Value,
                                "Id is incorrect.");

                Assert.AreEqual(this.element.MimeType,
                                attributes.First((a) => a.LocalName == AttributeNames.MimeType).Value,
                                "MimeType is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ResourceItem_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;
            
            Console.WriteLine("Test with a no children.");
            children = this.provider.GetXliffChildren().ToList();
            Assert.IsTrue(((children == null) || (children.Count() == 0)), "Children is not null.");

            Console.WriteLine("Test with children.");
            this.element.Source = new ResourceItemSource();
            this.element.Target = new ResourceItemTarget();
            this.element.References.Add(new Reference());
            this.element.References.Add(new Reference());
            children = this.provider.GetXliffChildren().ToList();
            Assert.AreEqual(4, children.Count, "Incorrect number of children.");
            TestUtilities.VerifyItems<ResourceItemSource>(this.element.Source, children, ElementNames.Source);
            TestUtilities.VerifyItems<ResourceItemTarget>(this.element.Target, children, ElementNames.Target);
            TestUtilities.VerifyItems<Reference>(this.element.References, children, ElementNames.Reference);

            Console.WriteLine("Test with a children removed.");
            this.element.References.RemoveAt(0);
            this.element.Target = null;
            children = this.provider.GetXliffChildren().ToList();
            Assert.AreEqual(2, children.Count(), "Incorrect number of children.");
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ResourceItem_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this.provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
