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
    /// This class tests the <see cref="StandaloneCode"/> class.
    /// </summary>
    [TestClass()]
    public class StandaloneCodeTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private StandaloneCode _element;

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
            this._element = new StandaloneCode();
            this._provider = this._element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandaloneCode_IXliffDataProvider_GetXliffAttributes()
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
                        this._element.CanCopy = false;
                        this._element.CanDelete = false;
                        this._element.CanReorder = CanReorderValue.No;
                        this._element.CopyOf = "copy";
                        this._element.DataReference = "end";
                        this._element.DisplayText = "end";
                        this._element.EquivalentText = "end";
                        this._element.EquivalentStorage = "storage";
                        this._element.FormatStyle = FormatStyleValue.Anchor;
                        this._element.Id = "id";
                        this._element.SizeInfo = "sizeinfo";
                        this._element.SizeInfoReference = "sizeref";
                        this._element.SubFlows = "start";
                        this._element.SubFormatStyle.Add("format", "style");
                        this._element.SubType = "subtype";
                        this._element.Type = CodeType.Quote;
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this._provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(16, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this._element.CanCopy,
                                attributes.First((a) => a.LocalName == AttributeNames.CanCopy).Value,
                                "CanCopy is incorrect.");
                Assert.AreEqual(this._element.CanDelete,
                                attributes.First((a) => a.LocalName == AttributeNames.CanDelete).Value,
                                "CanDelete is incorrect.");
                Assert.AreEqual(this._element.CanReorder,
                                attributes.First((a) => a.LocalName == AttributeNames.CanReorder).Value,
                                "CanReorder is incorrect.");
                Assert.AreEqual(this._element.CopyOf,
                                attributes.First((a) => a.LocalName == AttributeNames.CopyOf).Value,
                                "CopyOf is incorrect.");
                Assert.AreEqual(this._element.DataReference,
                                attributes.First((a) => a.LocalName == AttributeNames.DataReference).Value,
                                "DataReference is incorrect.");
                Assert.AreEqual(this._element.DisplayText,
                                attributes.First((a) => a.LocalName == AttributeNames.DisplayText).Value,
                                "DisplayText is incorrect.");
                Assert.AreEqual(this._element.EquivalentStorage,
                                attributes.First((a) => a.LocalName == SizeXmlNames.AttributeNames.EquivalentStorage).Value,
                                "EquivalentStorage is incorrect.");
                Assert.AreEqual(this._element.EquivalentText,
                                attributes.First((a) => a.LocalName == AttributeNames.EquivalentText).Value,
                                "EquivalentText is incorrect.");
                Assert.AreEqual(this._element.FormatStyle,
                                attributes.First((a) => a.LocalName == FsXmlNames.AttributeNames.FormatStyle).Value,
                                "FormatStyle is incorrect.");
                Assert.AreEqual(this._element.Id,
                                attributes.First((a) => a.LocalName == AttributeNames.Id).Value,
                                "Id is incorrect.");
                Assert.AreEqual(this._element.SizeInfo,
                                attributes.First((a) => a.LocalName == SizeXmlNames.AttributeNames.SizeInfo).Value,
                                "SizeInfo is incorrect.");
                Assert.AreEqual(this._element.SizeInfoReference,
                                attributes.First((a) => a.LocalName == SizeXmlNames.AttributeNames.SizeInfoReference).Value,
                                "SizeInfoReference is incorrect.");
                Assert.AreEqual(this._element.SubFlows,
                                attributes.First((a) => a.LocalName == AttributeNames.SubFlows).Value,
                                "SubFlows is incorrect.");
                refEquals = object.ReferenceEquals(this._element.SubFormatStyle,
                                                   attributes.First((a) => a.LocalName == FsXmlNames.AttributeNames.SubFormatStyle).Value);
                Assert.IsTrue(refEquals, "SubFormatStyle is incorrect.");
                Assert.AreEqual(this._element.SubType,
                                attributes.First((a) => a.LocalName == AttributeNames.SubType).Value,
                                "SubType is incorrect.");
                Assert.AreEqual(this._element.Type,
                                attributes.First((a) => a.LocalName == AttributeNames.Type).Value,
                                "Type is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandaloneCode_IXliffDataProvider_GetXliffChildren()
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
        public void StandaloneCode_IXliffDataProvider_GetXliffText()
        {
            Assert.IsNull(this._provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
