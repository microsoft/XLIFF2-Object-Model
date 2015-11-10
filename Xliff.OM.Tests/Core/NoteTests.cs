namespace Localization.Xliff.OM.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Modules.FormatStyle;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FsXmlNames = Localization.Xliff.OM.Modules.FormatStyle.XmlNames;

    /// <summary>
    /// This class tests the <see cref="Note"/> class.
    /// </summary>
    [TestClass()]
    public class NoteTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private Note _element;

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
            this._element = new Note();
            this._provider = this._element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Note_IXliffDataProvider_GetXliffAttributes()
        {
            for (int i = 0; i < 2; i++)
            {
                IEnumerable<IAttributeDataProvider> attributes;
                bool refEquals;

                switch (i)
                {
                    case 0:
                        Console.WriteLine("Test with default element.");
                        break;

                    case 1:
                        Console.WriteLine("Test with modified element.");
                        this._element.AppliesTo = TranslationSubject.Target;
                        this._element.Category = Guid.NewGuid().ToString();
                        this._element.FormatStyle = FormatStyleValue.Anchor;
                        this._element.Id = Guid.NewGuid().ToString();
                        this._element.Priority = 3;
                        this._element.SubFormatStyle.Add("format", "style");
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this._provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(6, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this._element.AppliesTo,
                                attributes.First((a) => a.LocalName == AttributeNames.AppliesTo).Value,
                                "AppliesTo is incorrect.");
                Assert.AreEqual(this._element.Category,
                                attributes.First((a) => a.LocalName == AttributeNames.Category).Value,
                                "Category is incorrect.");
                Assert.AreEqual(this._element.FormatStyle,
                                attributes.First((a) => a.LocalName == FsXmlNames.AttributeNames.FormatStyle).Value,
                                "FormatStyle is incorrect.");
                Assert.AreEqual(this._element.Id,
                                attributes.First((a) => a.LocalName == AttributeNames.Id).Value,
                                "Id is incorrect.");
                Assert.AreEqual(this._element.Priority,
                                attributes.First((a) => a.LocalName == AttributeNames.Priority).Value,
                                "Priority is incorrect.");
                refEquals = object.ReferenceEquals(this._element.SubFormatStyle,
                                                   attributes.First((a) => a.LocalName == FsXmlNames.AttributeNames.SubFormatStyle).Value);
                Assert.IsTrue(refEquals, "SubFormatStyle is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Note_IXliffDataProvider_GetXliffChildren()
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
        public void Note_IXliffDataProvider_GetXliffText()
        {
            Console.WriteLine("Test with no text.");
            Assert.IsNull(this._provider.GetXliffText(), "Text is not null.");

            Console.WriteLine("Test with text.");
            this._element.Text = Guid.NewGuid().ToString();
            Assert.AreEqual(this._element.Text, this._provider.GetXliffText(), "Text is incorrect.");
        }
        #endregion Test Methods
    }
}
