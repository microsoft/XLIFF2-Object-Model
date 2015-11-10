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
    /// This class tests the <see cref="SpanningCodeStart"/> class.
    /// </summary>
    [TestClass()]
    public class SpanningCodeStartTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private SpanningCodeStart _element;

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
            this._element = new SpanningCodeStart();
            this._provider = this._element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the default values for <see cref="SpanningCodeStart"/>.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void SpanningCodeStart_Defaults()
        {
            Segment segment;
            SpanningCodeStart span;
            SpanningCode spanParent;
            Unit unit;

            unit = new Unit();
            unit.SourceDirectionality = ContentDirectionality.Auto;
            unit.TargetDirectionality = ContentDirectionality.Auto;

            segment = new Segment();
            unit.Resources.Add(segment);

            segment.Source = new Source();
            segment.Target = new Target();

            Console.WriteLine("Test with SpanningCode as parent.");
            span = new SpanningCodeStart();
            spanParent = new SpanningCode();
            spanParent.Directionality = ContentDirectionality.RTL;
            spanParent.Text.Add(span);
            segment.Target.Text.Add(spanParent);
            Assert.AreEqual(spanParent.Directionality, span.Directionality, "Directionality is incorrect.");

            Console.WriteLine("Test with Source as parent.");
            unit.SourceDirectionality = ContentDirectionality.LTR;
            unit.TargetDirectionality = ContentDirectionality.Auto;
            span = new SpanningCodeStart();
            segment.Source.Text.Add(span);
            Assert.AreEqual(unit.SourceDirectionality, span.Directionality, "Directionality is incorrect.");

            Console.WriteLine("Test with Target as parent.");
            unit.SourceDirectionality = ContentDirectionality.Auto;
            unit.TargetDirectionality = ContentDirectionality.LTR;
            span = new SpanningCodeStart();
            segment.Target.Text.Add(span);
            Assert.AreEqual(unit.TargetDirectionality, span.Directionality, "Directionality is incorrect.");
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void SpanningCodeStart_IXliffDataProvider_GetXliffAttributes()
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
                        this._element.CanOverlap = false;
                        this._element.CanReorder = CanReorderValue.No;
                        this._element.CopyOf = "copy";
                        this._element.DataReference = "end";
                        this._element.Directionality = ContentDirectionality.RTL;
                        this._element.DisplayText = "end";
                        this._element.EquivalentStorage = "storage";
                        this._element.EquivalentText = "end";
                        this._element.FormatStyle = FormatStyleValue.Anchor;
                        this._element.Id = "id";
                        this._element.Isolated = false;
                        this._element.SizeInfo = "sizeinfo";
                        this._element.SizeInfoReference = "sizeref";
                        this._element.SizeRestriction = "sizeRestriction";
                        this._element.StorageRestriction = "storageRestriction";
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
                Assert.AreEqual(21, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this._element.CanCopy,
                                attributes.First((a) => a.LocalName == AttributeNames.CanCopy).Value,
                                "CanCopy is incorrect.");
                Assert.AreEqual(this._element.CanDelete,
                                attributes.First((a) => a.LocalName == AttributeNames.CanDelete).Value,
                                "CanDelete is incorrect.");
                Assert.AreEqual(this._element.CanOverlap,
                                attributes.First((a) => a.LocalName == AttributeNames.CanOverlap).Value,
                                "CanOverlap is incorrect.");
                Assert.AreEqual(this._element.CanReorder,
                                attributes.First((a) => a.LocalName == AttributeNames.CanReorder).Value,
                                "CanReorder is incorrect.");
                Assert.AreEqual(this._element.CopyOf,
                                attributes.First((a) => a.LocalName == AttributeNames.CopyOf).Value,
                                "CopyOf is incorrect.");
                Assert.AreEqual(this._element.DataReference,
                                attributes.First((a) => a.LocalName == AttributeNames.DataReference).Value,
                                "DataReference is incorrect.");
                Assert.AreEqual(this._element.Directionality,
                                attributes.First((a) => a.LocalName == AttributeNames.Directionality).Value,
                                "Directionality is incorrect.");
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
                Assert.AreEqual(this._element.Isolated,
                                attributes.First((a) => a.LocalName == AttributeNames.Isolated).Value,
                                "Isolated is incorrect.");
                Assert.AreEqual(this._element.SizeInfo,
                                attributes.First((a) => a.LocalName == SizeXmlNames.AttributeNames.SizeInfo).Value,
                                "SizeInfo is incorrect.");
                Assert.AreEqual(this._element.SizeInfoReference,
                                attributes.First((a) => a.LocalName == SizeXmlNames.AttributeNames.SizeInfoReference).Value,
                                "SizeInfoReference is incorrect.");
                Assert.AreEqual(this._element.SizeRestriction,
                                attributes.First((a) => a.LocalName == SizeXmlNames.AttributeNames.SizeRestriction).Value,
                                "SizeRestriction is incorrect.");
                Assert.AreEqual(this._element.StorageRestriction,
                                attributes.First((a) => a.LocalName == SizeXmlNames.AttributeNames.StorageRestriction).Value,
                                "StorageRestriction is incorrect.");
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
        public void SpanningCodeStart_IXliffDataProvider_GetXliffChildren()
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
        public void SpanningCodeStart_IXliffDataProvider_GetXliffText()
        {
            Assert.IsNull(this._provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
