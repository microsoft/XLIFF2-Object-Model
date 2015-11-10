namespace Localization.Xliff.OM.Modules.Validation.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization.Xliff.OM.Modules.Validation;
    using Localization.Xliff.OM.Modules.Validation.XmlNames;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="Validation"/> class.
    /// </summary>
    [TestClass]
    public class ValidationTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private Validation element;

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
            this.element = new Validation();
            this.provider = this.element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Validation_IXliffDataProvider_GetXliffAttributes()
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
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this.provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(0, attributes.Count(), "Number of attributes is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Validation_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;
            
            Console.WriteLine("Test with a no children.");
            Assert.IsNull(this.provider.GetXliffChildren(), "Children is not null.");

            Console.WriteLine("Test with children.");
            this.element.Rules.Add(new Rule());
            this.element.Rules.Add(new Rule());
            children = this.provider.GetXliffChildren().ToList();
            Assert.AreEqual(2, children.Count, "Incorrect number of children.");
            TestUtilities.VerifyItems<Rule>(this.element.Rules, children, ElementNames.Rule);

            Console.WriteLine("Test with a children removed.");
            this.element.Rules.RemoveAt(0);
            children = this.provider.GetXliffChildren().ToList();
            Assert.AreEqual(1, children.Count(), "Incorrect number of children.");
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Validation_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this.provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
