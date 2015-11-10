namespace Localization.Xliff.OM.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="Segment"/> class.
    /// </summary>
    [TestClass()]
    public class SegmentTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private Segment _element;

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
            this._element = new Segment();
            this._provider = this._element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Segment_IXliffDataProvider_GetXliffAttributes()
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
                        this._element.CanResegment = !this._element.CanResegment;
                        this._element.Id = Guid.NewGuid().ToString();
                        this._element.State = TranslationState.Reviewed;
                        this._element.SubState = Guid.NewGuid().ToString();
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this._provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(4, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this._element.CanResegment,
                                attributes.First((a) => a.LocalName == AttributeNames.CanResegment).Value,
                                "CanResegment is incorrect.");
                Assert.AreEqual(this._element.Id,
                                attributes.First((a) => a.LocalName == AttributeNames.Id).Value,
                                "Id is incorrect.");
                Assert.AreEqual(this._element.State,
                                attributes.First((a) => a.LocalName == AttributeNames.State).Value,
                                "State is incorrect.");
                Assert.AreEqual(this._element.SubState,
                                attributes.First((a) => a.LocalName == AttributeNames.SubState).Value,
                                "SubState is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Segment_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;

            Console.WriteLine("Test with a no children.");
            children = this._provider.GetXliffChildren().ToList();
            Assert.IsTrue(((children == null) || (children.Count() == 0)), "Children is not null.");

            Console.WriteLine("Test with children.");
            this._element.Source = new Source();
            this._element.Target = new Target();
            children = this._provider.GetXliffChildren().ToList();
            Assert.AreEqual(2, children.Count(), "Incorrect number of children.");
            TestUtilities.VerifyItems<Source>(this._element.Source, children, ElementNames.Source);
            TestUtilities.VerifyItems<Target>(this._element.Target, children, ElementNames.Target);
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Segment_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this._provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
