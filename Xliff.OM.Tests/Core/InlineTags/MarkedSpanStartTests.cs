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
    using SizeXmlNames = Localization.Xliff.OM.Modules.SizeRestriction.XmlNames;

    /// <summary>
    /// This class tests the <see cref="MarkedSpanStart"/> class.
    /// </summary>
    [TestClass()]
    public class MarkedSpanStartTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private MarkedSpanStart _element;

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
            this._element = new MarkedSpanStart();
            this._provider = this._element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the default values for <see cref="MarkedSpanStart"/>.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void MarkedSpanStart_Defaults()
        {
            MarkedSpan spanParent;
            Unit unitParent;

            Console.WriteLine("Test with element child of unit with false Translate.");
            unitParent = new Unit();
            unitParent.Translate = false;
            this._element.Parent = unitParent;

            Assert.AreEqual(this._element.Translate,
                            unitParent.Translate,
                            "Translate is incorrect.");

            Console.WriteLine("Test with element child of unit with true Translate.");
            unitParent.Translate = true;

            Assert.AreEqual(this._element.Translate,
                            unitParent.Translate,
                            "Translate is incorrect.");

            Console.WriteLine("Test with element child of mrk with false Translate.");
            spanParent = new MarkedSpan();
            spanParent.Translate = false;
            spanParent.Parent = unitParent;
            this._element.Parent = spanParent;

            Assert.AreEqual(this._element.Translate,
                            spanParent.Translate,
                            "Translate is incorrect.");

            Console.WriteLine("Test with element child of mrk with true Translate.");
            spanParent.Translate = true;

            Assert.AreEqual(this._element.Translate,
                            spanParent.Translate,
                            "Translate is incorrect.");
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void MarkedSpanStart_IXliffDataProvider_GetXliffAttributes()
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
                        this._element.FormatStyle = FormatStyleValue.Anchor;
                        this._element.Id = "id";
                        this._element.Reference = "ref";
                        this._element.SizeRestriction = "sizeRestriction";
                        this._element.StorageRestriction = "storageRestriction";
                        this._element.SubFormatStyle.Add("format", "style");
                        this._element.Translate = false;
                        this._element.Type = "type";
                        this._element.Value = "value";
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this._provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(9, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this._element.Id,
                                attributes.First((a) => a.LocalName == AttributeNames.Id).Value,
                                "Id is incorrect.");
                Assert.AreEqual(this._element.FormatStyle,
                                attributes.First((a) => a.LocalName == FsXmlNames.AttributeNames.FormatStyle).Value,
                                "FormatStyle is incorrect.");
                Assert.AreEqual(this._element.Reference,
                                attributes.First((a) => a.LocalName == AttributeNames.ReferenceAbbreviated).Value,
                                "Reference is incorrect.");
                Assert.AreEqual(this._element.SizeRestriction,
                                attributes.First((a) => a.LocalName == SizeXmlNames.AttributeNames.SizeRestriction).Value,
                                "SizeRestriction is incorrect.");
                Assert.AreEqual(this._element.StorageRestriction,
                                attributes.First((a) => a.LocalName == SizeXmlNames.AttributeNames.StorageRestriction).Value,
                                "StorageRestriction is incorrect.");
                refEquals = object.ReferenceEquals(this._element.SubFormatStyle,
                                                   attributes.First((a) => a.LocalName == FsXmlNames.AttributeNames.SubFormatStyle).Value);
                Assert.IsTrue(refEquals, "SubFormatStyle is incorrect.");
                Assert.AreEqual(this._element.Translate,
                                attributes.First((a) => a.LocalName == AttributeNames.Translate).Value,
                                "Translate is incorrect.");
                Assert.AreEqual(this._element.Type,
                                attributes.First((a) => a.LocalName == AttributeNames.Type).Value,
                                "Type is incorrect.");
                Assert.AreEqual(this._element.Value,
                                attributes.First((a) => a.LocalName == AttributeNames.Value).Value,
                                "Value is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void MarkedSpanStart_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;

            Console.WriteLine("Test with a no files.");
            children = this._provider.GetXliffChildren().ToList();
            Assert.IsTrue(((children == null) || (children.Count() == 0)), "Children is not null.");
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void MarkedSpanStart_IXliffDataProvider_GetXliffText()
        {
            Assert.IsNull(this._provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
