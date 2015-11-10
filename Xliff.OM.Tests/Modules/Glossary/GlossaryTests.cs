namespace Localization.Xliff.OM.Modules.Glossary.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization.Xliff.OM.Modules.Glossary;
    using Localization.Xliff.OM.Modules.Glossary.XmlNames;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="Glossary"/> class.
    /// </summary>
    [TestClass]
    public class GlossaryTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private Glossary element;

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
            this.element = new Glossary();
            this.provider = this.element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Glossary_IXliffDataProvider_GetXliffAttributes()
        {
            for (int i = 0; i < 1; i++)
            {
                IEnumerable<IAttributeDataProvider> attributes;

                switch (i)
                {
                    case 0:
                        Console.WriteLine("Test with default element.");
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
        public void Glossary_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;

            Console.WriteLine("Test with default children.");
            children = this.provider.GetXliffChildren().ToList();
            Assert.AreEqual(0, children.Count, "Incorrect number of children.");

            Console.WriteLine("Test with children.");
            this.element.Entries.Add(new GlossaryEntry("ge1"));
            this.element.Entries[0].Reference = "ref1";
            this.element.Entries.Add(new GlossaryEntry("ge2"));
            this.element.Entries[1].Reference = "ref2";
            children = this.provider.GetXliffChildren().ToList();
            Assert.AreEqual(2, children.Count, "Incorrect number of children.");
            TestUtilities.VerifyItems(this.element.Entries, children, ElementNames.GlossaryEntry);
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Glossary_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this.provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
