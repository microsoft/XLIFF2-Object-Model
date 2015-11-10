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
    /// This class tests the <see cref="File"/> class.
    /// </summary>
    [TestClass()]
    public class FileTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private File _element;

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
            this._element = new File();
            this._provider = this._element;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void File_IXliffDataProvider_GetXliffAttributes()
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
                        this._element.Original = Guid.NewGuid().ToString();
                        this._element.SizeInfo = "sizeinfo";
                        this._element.SizeInfoReference = "sizeref";
                        this._element.SizeRestriction = "sizeRestriction";
                        this._element.SourceDirectionality = ContentDirectionality.RTL;
                        this._element.Space = Preservation.Preserve;
                        this._element.StorageRestriction = "storageRestriction";
                        this._element.SubFormatStyle.Add("format", "style");
                        this._element.TargetDirectionality = ContentDirectionality.RTL;
                        this._element.Translate = !this._element.Translate;
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this._provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(13, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this._element.CanResegment,
                                attributes.First((a) => a.LocalName == AttributeNames.CanResegment).Value,
                                "CanResegment is incorrect.");
                Assert.AreEqual(this._element.FormatStyle,
                                attributes.First((a) => a.LocalName == FsXmlNames.AttributeNames.FormatStyle).Value,
                                "FormatStyle is incorrect.");
                Assert.AreEqual(this._element.Id,
                                attributes.First((a) => a.LocalName == AttributeNames.Id).Value,
                                "Id is incorrect.");
                Assert.AreEqual(this._element.Original,
                                attributes.First((a) => a.LocalName == AttributeNames.Original).Value,
                                "Original is incorrect.");
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
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void File_IXliffDataProvider_GetXliffChildren()
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
            this._element.Containers.Add(new Group());
            this._element.Containers.Add(new Unit());
            this._element.Skeleton = new Skeleton();
            children = this._provider.GetXliffChildren().ToList();
            Assert.AreEqual(4, children.Count, "Incorrect number of children.");
            TestUtilities.VerifyItems<Group>(this._element.Containers, children, ElementNames.Group);
            TestUtilities.VerifyItems<Skeleton>(this._element.Skeleton, children, ElementNames.Skeleton);
            TestUtilities.VerifyItems<Unit>(this._element.Containers, children, ElementNames.Unit);
            TestUtilities.VerifyNoteContainerItems(this._element.Notes, children, ElementNames.Notes);

            Console.WriteLine("Test with a children removed.");
            this._element.Notes.Clear();
            this._element.Containers.RemoveAt(1);
            children = this._provider.GetXliffChildren().ToList();
            Assert.AreEqual(2, children.Count, "Incorrect number of children.");
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void File_IXliffDataProvider_GetXliffText()
        {
            Assert.IsNull(this._provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
