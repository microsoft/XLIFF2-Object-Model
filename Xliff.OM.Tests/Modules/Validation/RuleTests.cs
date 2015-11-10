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
    public class RuleTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private Rule element;

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
            this.element = new Rule();
            this.provider = this.element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Rule_IXliffDataProvider_GetXliffAttributes()
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
                        this.element.CaseSensitive = !this.element.CaseSensitive;
                        this.element.Disabled = !this.element.Disabled;
                        this.element.EndsWith = "endswith";
                        this.element.ExistsInSource = !this.element.ExistsInSource;
                        this.element.IsNotPresent = "isnotpresent";
                        this.element.IsPresent = "ispresent";
                        this.element.Normalization = NormalizationValue.NFD;
                        this.element.Occurs = 10;
                        this.element.StartsWith = "startswith";
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this.provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(9, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this.element.CaseSensitive,
                                attributes.First((a) => a.LocalName == AttributeNames.CaseSensitive).Value,
                                "CaseSensitive is incorrect.");

                Assert.AreEqual(this.element.Disabled,
                                attributes.First((a) => a.LocalName == AttributeNames.Disabled).Value,
                                "Disabled is incorrect.");

                Assert.AreEqual(this.element.EndsWith,
                                attributes.First((a) => a.LocalName == AttributeNames.EndsWith).Value,
                                "EndsWith is incorrect.");

                Assert.AreEqual(this.element.ExistsInSource,
                                attributes.First((a) => a.LocalName == AttributeNames.ExistsInSource).Value,
                                "ExistsInSource is incorrect.");

                Assert.AreEqual(this.element.IsNotPresent,
                                attributes.First((a) => a.LocalName == AttributeNames.IsNotPresent).Value,
                                "IsNotPresent is incorrect.");

                Assert.AreEqual(this.element.IsPresent,
                                attributes.First((a) => a.LocalName == AttributeNames.IsPresent).Value,
                                "IsPresent is incorrect.");

                Assert.AreEqual(this.element.Normalization,
                                attributes.First((a) => a.LocalName == AttributeNames.Normalization).Value,
                                "Normalization is incorrect.");

                Assert.AreEqual(this.element.Occurs,
                                attributes.First((a) => a.LocalName == AttributeNames.Occurs).Value,
                                "Occurs is incorrect.");

                Assert.AreEqual(this.element.StartsWith,
                                attributes.First((a) => a.LocalName == AttributeNames.StartsWith).Value,
                                "StartsWith is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Rule_IXliffDataProvider_GetXliffChildren()
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
        public void Rule_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this.provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
