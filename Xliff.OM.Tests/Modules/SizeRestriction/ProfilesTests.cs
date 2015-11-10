namespace Localization.Xliff.OM.Modules.SizeRestriction.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization.Xliff.OM.Modules.SizeRestriction;
    using Localization.Xliff.OM.Modules.SizeRestriction.XmlNames;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="Profiles"/> class.
    /// </summary>
    [TestClass]
    public class ProfilesTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private Profiles element;

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
            this.element = new Profiles();
            this.provider = this.element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Profiles_IXliffDataProvider_GetXliffAttributes()
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
                        this.element.GeneralProfile = "generalprofile";
                        this.element.StorageProfile = "storageprofile";
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this.provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(2, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this.element.GeneralProfile,
                                attributes.First((a) => a.LocalName == AttributeNames.GeneralProfile).Value,
                                "GeneralProfile is incorrect.");

                Assert.AreEqual(this.element.StorageProfile,
                                attributes.First((a) => a.LocalName == AttributeNames.StorageProfile).Value,
                                "StorageProfile is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Profiles_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;
            
            Console.WriteLine("Test with a no children.");
            children = this.provider.GetXliffChildren().ToList();
            Assert.IsTrue(((children == null) || (children.Count() == 0)), "Children is not null.");

            Console.WriteLine("Test with children.");
            this.element.Normalization = new Normalization();
            children = this.provider.GetXliffChildren().ToList();
            Assert.AreEqual(1, children.Count, "Incorrect number of children.");
            TestUtilities.VerifyItems<Normalization>(this.element.Normalization, children, ElementNames.Normalization);

            Console.WriteLine("Test with a children removed.");
            this.element.Normalization = null;
            children = this.provider.GetXliffChildren().ToList();
            Assert.AreEqual(0, children.Count(), "Incorrect number of children.");
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Profiles_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this.provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
