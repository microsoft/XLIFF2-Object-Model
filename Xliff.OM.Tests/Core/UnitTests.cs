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
    /// This class tests the <see cref="Unit"/> class.
    /// </summary>
    [TestClass()]
    public class UnitTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private Unit _element;

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
            this._element = new Unit();
            this._provider = this._element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Unit_IXliffDataProvider_GetXliffAttributes()
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
                        this._element.CanResegment = !this._element.CanResegment;
                        this._element.FormatStyle = FormatStyleValue.Anchor;
                        this._element.Id = Guid.NewGuid().ToString();
                        this._element.Name = Guid.NewGuid().ToString();
                        this._element.SizeInfo = "sizeinfo";
                        this._element.SizeInfoReference = "sizeref";
                        this._element.SizeRestriction = "sizeRestriction";
                        this._element.SourceDirectionality = ContentDirectionality.RTL;
                        this._element.Space = Preservation.Preserve;
                        this._element.StorageRestriction = "storageRestriction";
                        this._element.SubFormatStyle.Add("format", "style");
                        this._element.TargetDirectionality = ContentDirectionality.RTL;
                        this._element.Translate = !this._element.Translate;
                        this._element.Type = Guid.NewGuid().ToString();
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this._provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(14, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this._element.CanResegment,
                                attributes.First((a) => a.LocalName == AttributeNames.CanResegment).Value,
                                "CanResegment is incorrect.");
                Assert.AreEqual(this._element.FormatStyle,
                                attributes.First((a) => a.LocalName == FsXmlNames.AttributeNames.FormatStyle).Value,
                                "FormatStyle is incorrect.");
                Assert.AreEqual(this._element.Id,
                                attributes.First((a) => a.LocalName == AttributeNames.Id).Value,
                                "Id is incorrect.");
                Assert.AreEqual(this._element.Name,
                                attributes.First((a) => a.LocalName == AttributeNames.Name).Value,
                                "Name is incorrect.");
                Assert.AreEqual(this._element.SizeInfo,
                                attributes.First((a) => a.LocalName == SizeXmlNames.AttributeNames.SizeInfo).Value,
                                "SizeInfo is incorrect.");
                Assert.AreEqual(this._element.SizeInfoReference,
                                attributes.First((a) => a.LocalName == SizeXmlNames.AttributeNames.SizeInfoReference).Value,
                                "SizeInfoReference is incorrect.");
                Assert.AreEqual(this._element.SizeRestriction,
                                attributes.First((a) => a.LocalName == SizeXmlNames.AttributeNames.SizeRestriction).Value,
                                "SizeRestriction is incorrect.");
                Assert.AreEqual(this._element.SourceDirectionality,
                                attributes.First((a) => a.LocalName == AttributeNames.SourceDirectionality).Value,
                                "SourceDirectionality is incorrect.");
                Assert.AreEqual(this._element.Space,
                                attributes.First((a) => a.LocalName == AttributeNames.SpacePreservation).Value,
                                "Space is incorrect.");
                Assert.AreEqual(this._element.StorageRestriction,
                                attributes.First((a) => a.LocalName == SizeXmlNames.AttributeNames.StorageRestriction).Value,
                                "StorageRestriction is incorrect.");
                refEquals = object.ReferenceEquals(this._element.SubFormatStyle,
                                                   attributes.First((a) => a.LocalName == FsXmlNames.AttributeNames.SubFormatStyle).Value);
                Assert.IsTrue(refEquals, "SubFormatStyle is incorrect.");
                Assert.AreEqual(this._element.TargetDirectionality,
                                attributes.First((a) => a.LocalName == AttributeNames.TargetDirectionality).Value,
                                "TargetDirectionality is incorrect.");
                Assert.AreEqual(this._element.Translate,
                                attributes.First((a) => a.LocalName == AttributeNames.Translate).Value,
                                "Translate is incorrect.");
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
        public void Unit_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;

            Console.WriteLine("Test with a no children.");
            children = this._provider.GetXliffChildren().ToList();
            Assert.IsTrue(((children == null) || (children.Count == 0)), "Children is not null.");

            Console.WriteLine("Test with notes.");
            this._element.AddNote("note");
            this._element.AddNote("note");
            children = this._provider.GetXliffChildren().ToList();
            Assert.AreEqual(1, children.Count, "Incorrect number of children.");
            TestUtilities.VerifyNoteContainerItems(this._element.Notes, children, ElementNames.Notes);

            Console.WriteLine("Test with children.");
            this._element.Resources.Add(new Ignorable());
            this._element.Resources.Add(new Segment());
            children = this._provider.GetXliffChildren().ToList();
            Assert.AreEqual(3, children.Count, "Incorrect number of children.");
            TestUtilities.VerifyItems<Ignorable>(this._element.Resources, children, ElementNames.Ignorable);
            TestUtilities.VerifyItems<Segment>(this._element.Resources, children, ElementNames.Segment);
            TestUtilities.VerifyNoteContainerItems(this._element.Notes, children, ElementNames.Notes);

            Console.WriteLine("Test with a children removed.");
            this._element.Notes.Clear();
            this._element.Resources.RemoveAt(1);
            children = this._provider.GetXliffChildren().ToList();
            Assert.AreEqual(1, children.Count, "Incorrect number of children.");
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Unit_IXliffDataProvider_GetXliffText()
        {
            Assert.IsNull(this._provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
