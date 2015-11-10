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
    /// This class tests the <see cref="GlossaryEntry"/> class.
    /// </summary>
    [TestClass]
    public class GlossaryEntryTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private GlossaryEntry element;

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
            this.element = new GlossaryEntry();
            this.provider = this.element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void GlossaryEntry_IXliffDataProvider_GetXliffAttributes()
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
        public void GlossaryEntry_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;

            Console.WriteLine("Test with default children.");
            this.element.Term.Source = "source";
            this.element.Term.Text = "text";
            children = this.provider.GetXliffChildren().ToList();
            Assert.AreEqual(1, children.Count, "Incorrect number of children.");
            TestUtilities.VerifyItems<Term>(this.element.Term, children, ElementNames.Term);

            Console.WriteLine("Test with children.");
            this.element.Definition = new Definition();
            this.element.Definition.Source = "source";
            this.element.Definition.Text = "text";
            this.element.Translations.Add(new Translation("t1"));
            this.element.Translations[0].Reference = "ref1";
            this.element.Translations[0].Source = "source1";
            this.element.Translations[0].Text = "text1";
            this.element.Translations.Add(new Translation("t2"));
            this.element.Translations[1].Reference = "ref2";
            this.element.Translations[1].Source = "source2";
            this.element.Translations[1].Text = "text2";
            children = this.provider.GetXliffChildren().ToList();
            Assert.AreEqual(4, children.Count(), "Incorrect number of children.");
            TestUtilities.VerifyItems<Definition>(this.element.Definition, children, ElementNames.Definition);
            TestUtilities.VerifyItems<Term>(this.element.Term, children, ElementNames.Term);
            TestUtilities.VerifyItems<Translation>(this.element.Translations, children, ElementNames.Translation);
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the element.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void GlossaryEntry_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this.provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
