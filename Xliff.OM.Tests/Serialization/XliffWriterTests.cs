namespace Localization.Xliff.OM.Serialization.Tests
{
    using System;
    using System.Text;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Modules;
    using Localization.Xliff.OM.Modules.ChangeTracking;
    using Localization.Xliff.OM.Modules.Glossary;
    using Localization.Xliff.OM.Modules.Metadata;
    using Localization.Xliff.OM.Modules.ResourceData;
    using Localization.Xliff.OM.Modules.SizeRestriction;
    using Localization.Xliff.OM.Modules.Validation;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="XliffWriter"/> class.
    /// </summary>
    [DeploymentItem(TestUtilities.TestDataDirectory, TestUtilities.TestDataDirectory)]
    [TestClass()]
    public class XliffWriterTests : IDisposable
    {
        #region Member Variables
        /// <summary>
        /// The stream used when serializing the document.
        /// </summary>
        private System.IO.MemoryStream _stream;

        /// <summary>
        /// The document used for validation.
        /// </summary>
        private XliffDocument _document;

        /// <summary>
        /// The serializer used to serialize the document to the stream.
        /// </summary>
        private XliffWriter _writer;
        #endregion Member Variables

        /// <summary>
        /// Initializes the test class before every test method is executed.
        /// </summary>
        [TestInitialize()]
        public void TestInitialize()
        {
            XliffWriterSettings settings;

            settings = new XliffWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.Validators.Clear();

            this._document = new XliffDocument();
            this._stream = new System.IO.MemoryStream();
            this._writer = new XliffWriter(settings);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="XliffWriterTests"/> class.
        /// </summary>
        ~XliffWriterTests()
        {
            this.Dispose(false);
        }

        #region Test Methods
        /// <summary>
        /// Tests that a <see cref="ChangeTrack"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_ChangeTrack()
        {
            ChangeTrack change;
            Item item;
            RevisionsContainer container;
            Revision revision;
            string actualValue;

            change = new ChangeTrack();

            container = new RevisionsContainer();
            container.AppliesTo = "source";
            container.Reference = "1";
            container.CurrentVersion = "ver1";
            change.Revisions.Add(container);

            revision = new Revision();
            revision.Author = "author";
            revision.ChangeDate = new DateTime(2015, 1, 2, 3, 4, 5);
            revision.Version = "ver1";
            container.Revisions.Add(revision);

            item = new Item();
            item.Property = "content";
            item.Text = "text";
            revision.Items.Add(item);

            this._document.Files.Add(new File("f1"));
            this._document.Files[0].Changes = change;

            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.FileWithChangeTrack), actualValue);
        }

        /// <summary>
        /// Tests that a <see cref="Data"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Data()
        {
            Data data;
            Unit unit;
            string actualValue;

            data = new Data();
            unit = new Unit();
            unit.OriginalData = new OriginalData();
            unit.OriginalData.DataElements.Add(data);
            this._document.Files.Add(new File());
            this._document.Files[0].Containers.Add(unit);

            Console.WriteLine("Test with default values.");
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.DataWithDefaultValues), actualValue);

            Console.WriteLine("Test with empty values.");
            data.Id = String.Empty;
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.DataWithEmptyValues), actualValue);

            Console.WriteLine("Test with valid values.");
            data.Directionality = ContentDirectionality.RTL;
            data.Id = "id";
            data.Space = Preservation.Preserve;
            data.Text.Add(new CodePoint(3));
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.DataWithValidValues), actualValue);
        }

        /// <summary>
        /// Tests that an <see cref="XliffDocument"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Document()
        {
            string actualValue;

            Console.WriteLine("Test with null document.");
            try
            {
                this._writer.Serialize(this._stream, null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            Console.WriteLine("Test with default values.");
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.DocumentWithDefaultValues), actualValue);

            Console.WriteLine("Test with empty values.");
            this._document.SourceLanguage = String.Empty;
            this._document.TargetLanguage = String.Empty;
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.DocumentWithEmptyValues), actualValue);

            Console.WriteLine("Test with valid values.");
            this._document.SourceLanguage = "en-us";
            this._document.Space = Preservation.Preserve;
            this._document.TargetLanguage = "de-de";
            this._document.Version = "version";
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.DocumentWithValidValues), actualValue);
        }

        /// <summary>
        /// Tests that a <see cref="File"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_File()
        {
            File file;
            string actualValue;

            file = new File();
            this._document.Files.Add(file);

            Console.WriteLine("Test with default values.");
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.FileWithDefaultValues), actualValue);

            Console.WriteLine("Test with empty values.");
            file.Id = String.Empty;
            file.Original = String.Empty;
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.FileWithEmptyValues), actualValue);

            Console.WriteLine("Test with valid values.");
            file.CanResegment = true;
            file.Id = "id";
            file.Original = "original";
            file.Translate = false;
            file.SourceDirectionality = ContentDirectionality.LTR;
            file.TargetDirectionality = ContentDirectionality.RTL;
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.FileWithValidValues), actualValue);
        }

        /// <summary>
        /// Tests that <see cref="Note"/>s in a <see cref="File"/> serialize correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_FileNotes()
        {
            File file;
            string actualValue;

            file = new File();
            this._document.Files.Add(file);

            Console.WriteLine("Test with default values.");
            file.Notes.Add(new Note());
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.FileNoteWithDefaultValues), actualValue);

            Console.WriteLine("Test with empty values.");
            file.Notes[0].Category = String.Empty;
            file.Notes[0].Id = String.Empty;
            file.Notes[0].Text = String.Empty;
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.FileNoteWithEmptyValues), actualValue);

            Console.WriteLine("Test with valid values.");
            file.Notes[0].AppliesTo = TranslationSubject.Target;
            file.Notes[0].Category = "category";
            file.Notes[0].Id = "id";
            file.Notes[0].Priority = 100;
            file.Notes[0].Text = "text";
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.FileNoteWithValidValues), actualValue);
        }

        /// <summary>
        /// Tests that a full <see cref="XliffDocument"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_FullDocument()
        {
            File file;
            Group group1;
            Group group2;
            Ignorable ignorable;
            Note note;
            Segment segment;
            Skeleton skeleton;
            Unit unit;
            string actualValue;

            this._document.SourceLanguage = "en-us";
            this._document.Space = Preservation.Default;
            this._document.TargetLanguage = "de-de";
            this._document.Version = "version";

            file = new File();
            file.CanResegment = true;
            file.Id = "id";
            file.Original = "original";
            file.SourceDirectionality = ContentDirectionality.LTR;
            file.Space = Preservation.Preserve;
            file.TargetDirectionality = ContentDirectionality.RTL;
            file.Translate = true;
            this._document.Files.Add(file);

            group1 = new Group();
            group1.CanResegment = true;
            group1.Id = "id";
            group1.Name = "name";
            group1.SourceDirectionality = ContentDirectionality.RTL;
            group1.Space = Preservation.Preserve;
            group1.TargetDirectionality = ContentDirectionality.Auto;
            group1.Translate = true;
            group1.Type = "type";
            file.Containers.Add(group1);

            group2 = new Group();
            group2.CanResegment = false;
            group2.Id = "id2";
            group2.Name = "name2";
            group2.SourceDirectionality = ContentDirectionality.LTR;
            group2.Space = Preservation.Default;
            group2.TargetDirectionality = ContentDirectionality.RTL;
            group2.Translate = false;
            group2.Type = "type2";
            file.Containers.Add(group2);

            unit = new Unit();
            unit.CanResegment = false;
            unit.Id = "id";
            unit.Name = "name";
            unit.Notes.Add(new Note("text"));
            unit.SourceDirectionality = ContentDirectionality.LTR;
            unit.Space = Preservation.Default;
            unit.TargetDirectionality = ContentDirectionality.Auto;
            unit.Translate = true;
            unit.Type = "type";
            group2.Containers.Add(unit);

            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("id", "data1");
            unit.OriginalData.DataElements[0].Directionality = ContentDirectionality.RTL;
            unit.OriginalData.DataElements[0].Space = Preservation.Preserve;
            unit.OriginalData.DataElements[0].Text.Add(new CodePoint(1));

            unit = new Unit();
            unit.CanResegment = false;
            unit.Id = "id2";
            unit.Name = "name2";
            unit.Notes.Add(new Note("text2"));
            unit.SourceDirectionality = ContentDirectionality.RTL;
            unit.Space = Preservation.Preserve;
            unit.TargetDirectionality = ContentDirectionality.LTR;
            unit.Translate = false;
            unit.Type = "type2";
            group2.Containers.Add(unit);

            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("id1", "data1");
            unit.OriginalData.DataElements[0].Directionality = ContentDirectionality.LTR;
            unit.OriginalData.DataElements[0].Space = Preservation.Preserve;
            unit.OriginalData.DataElements[0].Text.Add(new CodePoint(2));
            unit.OriginalData.AddData("id2", "data2");
            unit.OriginalData.DataElements[1].Directionality = ContentDirectionality.RTL;
            unit.OriginalData.DataElements[1].Space = Preservation.Preserve;
            unit.OriginalData.DataElements[1].Text.Add(new CodePoint(1));

            ignorable = new Ignorable();
            ignorable.Id = "id";
            ignorable.Source = new Source("text");
            ignorable.Target = new Target("text");
            unit.Resources.Add(ignorable);

            segment = new Segment();
            segment.CanResegment = true;
            segment.Id = "id";
            segment.Source = new Source();
            segment.State = TranslationState.Reviewed;
            segment.SubState = "substate";
            segment.Target = new Target();
            segment.Source.Language = "en-us";
            segment.Source.Space = Preservation.Default;
            segment.Source.Text.Add(new CodePoint(1));
            segment.Source.Text.Add(new PlainText("text"));
            segment.Target.Language = "en-us";
            segment.Target.Order = 100;
            segment.Target.Space = Preservation.Default;
            segment.Target.Text.Add(new CodePoint(12));
            segment.Target.Text.Add(new PlainText("text2"));
            unit.Resources.Add(segment);

            note = new Note();
            note.AppliesTo = TranslationSubject.Source;
            note.Category = "category";
            note.Id = "id";
            note.Priority = 2;
            note.Text = "text";
            file.Notes.Add(note);

            file.Notes.Add(new Note("text2"));

            skeleton = new Skeleton();
            skeleton.HRef = "href";
            skeleton.NonTranslatableText = "text";
            file.Skeleton = skeleton;

            file = new File();
            file.CanResegment = false;
            file.Id = "id2";
            file.Notes.Add(new Note("text"));
            file.Original = "original2";
            file.SourceDirectionality = ContentDirectionality.Auto;
            file.Space = Preservation.Default;
            file.TargetDirectionality = ContentDirectionality.LTR;
            file.Translate = false;
            this._document.Files.Add(file);

            skeleton = new Skeleton();
            skeleton.HRef = "href";
            skeleton.NonTranslatableText = "text";
            file.Skeleton = skeleton;

            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.FullDocument), actualValue);
        }

        /// <summary>
        /// Tests that a <see cref="Modules.Glossary.Glossary"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Glossary()
        {
            MarkedSpan span;
            Segment segment;
            Unit unit;

            unit = new Unit("u1");
            unit.Glossary = new Glossary();
            unit.Glossary.Entries.Add(new GlossaryEntry());
            unit.Glossary.Entries[0].Id = "entry1";
            unit.Glossary.Entries[0].Reference = Utilities.MakeIri("m1");
            unit.Glossary.Entries[0].Definition = new Definition();
            unit.Glossary.Entries[0].Definition.Source = "definitionSource";
            unit.Glossary.Entries[0].Definition.Text = "definition text";
            unit.Glossary.Entries[0].Term.Source = "termSource";
            unit.Glossary.Entries[0].Term.Text = "term text";
            unit.Glossary.Entries[0].Translations.Add(new Translation());
            unit.Glossary.Entries[0].Translations[0].Id = "trans1";
            unit.Glossary.Entries[0].Translations[0].Reference = Utilities.MakeIri("m1");
            unit.Glossary.Entries[0].Translations[0].Source = "transSource";
            unit.Glossary.Entries[0].Translations[0].Text = "translation text";

            span = new MarkedSpan("m1");
            span.Type = "term";
            span.Text.Add(new PlainText("text"));

            segment = new Segment();
            segment.Source = new Source();
            segment.Source.Text.Add(span);
            segment.State = TranslationState.Initial;

            this._document.SourceLanguage = "en-us";
            this._document.Files.Add(new File("f1"));
            this._document.Files[0].Containers.Add(unit);
            unit.Resources.Add(segment);

            Assert.AreEqual(
                            TestUtilities.GetFileContents(TestData.GlossaryWithValidValues),
                            this.Serialize(),
                            "Serialized document is incorrect.");
        }

        /// <summary>
        /// Tests that a <see cref="Group"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Group()
        {
            File file;
            Group group;
            string actualValue;

            file = new File();
            this._document.Files.Add(file);

            group = new Group();
            file.Containers.Add(group);

            Console.WriteLine("Test with default values.");
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.GroupWithDefaultValues), actualValue);

            Console.WriteLine("Test with empty values.");
            group.Id = String.Empty;
            group.Name = String.Empty;
            group.Type = String.Empty;
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.GroupWithEmptyValues), actualValue);

            Console.WriteLine("Test with valid values.");
            group.CanResegment = false;
            group.Id = "id";
            group.Name = "name";
            group.SourceDirectionality = ContentDirectionality.Auto;
            group.Space = Preservation.Default;
            group.TargetDirectionality = ContentDirectionality.LTR;
            group.Translate = true;
            group.Type = "type";
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.GroupWithValidValues), actualValue);
        }

        /// <summary>
        /// Tests that an <see cref="Ignorable"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Ignorable()
        {
            File file;
            Ignorable ignorable;
            Unit unit;
            string actualValue;

            file = new File();
            this._document.Files.Add(file);

            unit = new Unit();
            file.Containers.Add(unit);

            ignorable = new Ignorable();
            unit.Resources.Add(ignorable);

            Console.WriteLine("Test with default values.");
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.IgnorableWithDefaultValues), actualValue);

            Console.WriteLine("Test with empty values.");
            ignorable.Id = String.Empty;
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.IgnorableWithEmptyValues), actualValue);

            Console.WriteLine("Test with valid values.");
            ignorable.Id = "id";
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.IgnorableWithValidValues), actualValue);
        }

        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Inheritance()
        {
            File file;
            Segment segment;
            Unit unit;
            string actualValue;
            
            file = new File("newfile");
            file.Translate = false;
            this._document.SourceLanguage = "en-us";
            this._document.Files.Add(file);

            unit = new Unit("newunit");
            // Inherit the value of Translate.
            file.Containers.Add(unit);

            segment = new Segment("newsegment");
            unit.Resources.Add(segment);

            segment.Source = new Source();
            segment.Source.Text.Add(new MarkedSpan("newmrk") { Translate = true });

            actualValue = this.Serialize(OutputDetail.Minimal);
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.InheritanceWithMinimalOutput), actualValue);
        }

        /// <summary>
        /// Test serializer with invalid arguments.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_InvalidDocuments()
        {
            Console.WriteLine("Test with null stream.");

            try
            {
                this._writer.Serialize((System.IO.Stream)null, this._document);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        /// <summary>
        /// Tests that a <see cref="MetadataContainer"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Metadata()
        {
            MetaGroup group;
            Unit unit;
            string actualValue;

            group = new MetaGroup();
            group.AppliesTo = MetaGroupSubject.Source;
            group.Category = "category";
            group.Containers.Add(new Meta("type", "text"));

            unit = new Unit("u1");
            unit.Metadata = new MetadataContainer();
            unit.Metadata.Groups.Add(group);
            this._document.Files.Add(new File("f1"));
            this._document.Files[0].Containers.Add(unit);

            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.UnitWithMetadata), actualValue);
        }

        /// <summary>
        /// Tests that a <see cref="OriginalData"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_OriginalData()
        {
            Unit unit;
            string actualValue;

            unit = new Unit();
            unit.OriginalData = new OriginalData();
            this._document.Files.Add(new File());
            this._document.Files[0].Containers.Add(unit);

            Console.WriteLine("Test with default values.");
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.OriginalDataWithDefaultValues), actualValue);

            Console.WriteLine("Test with valid values.");
            unit.OriginalData.AddData("id", "text");
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.OriginalDataWithValidValues), actualValue);
        }

        /// <summary>
        /// Tests that a <see cref="ProfileData"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_ProfileData()
        {
            ProfileData data;
            string actualValue;

            data = new ProfileData();
            data.Profile = "profile";

            this._document.Files.Add(new File("f1"));
            this._document.Files[0].ProfileData = data;

            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.FileWithProfileData), actualValue);
        }

        /// <summary>
        /// Tests that a <see cref="Profiles"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Profiles()
        {
            Profiles profiles;
            string actualValue;

            profiles = new Profiles();
            profiles.GeneralProfile = "generalprofile";
            profiles.StorageProfile = "storageprofile";

            profiles.Normalization = new Normalization();
            profiles.Normalization.General = NormalizationValue.NFC;
            profiles.Normalization.Storage = NormalizationValue.NFC;

            this._document.Files.Add(new File("f1"));
            this._document.Files[0].RestrictionProfiles = profiles;

            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.FileWithProfiles), actualValue);
        }

        /// <summary>
        /// Tests that a <see cref="ResourceData"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_ResourceData()
        {
            Reference reference;
            ResourceData data;
            ResourceItemRef itemRef;
            ResourceItem item;
            ResourceItemSource source;
            ResourceItemTarget target;
            string actualValue;

            data = new ResourceData();

            itemRef = new ResourceItemRef();
            itemRef.Id = "rif1";
            itemRef.Reference = "ri1";
            data.ResourceItemReferences.Add(itemRef);

            item = new ResourceItem();
            item.Context = true;
            item.Id = "ri1";
            item.MimeType = "mime";
            data.ResourceItems.Add(item);

            reference = new Reference();
            reference.HRef = "resource";
            reference.Language = "de-de";
            item.References.Add(reference);

            source = new ResourceItemSource();
            source.HRef = "resource";
            source.Language = "de-de";
            item.Source = source;

            target = new ResourceItemTarget();
            target.HRef = "resource";
            target.Language = "de-de";
            item.Target = target;

            this._document.Files.Add(new File("f1"));
            this._document.Files[0].ResourceData = data;

            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.FileWithResourceData), actualValue);
        }

        /// <summary>
        /// Tests that a <see cref="Segment"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Segment()
        {
            File file;
            Segment segment;
            Unit unit;
            string actualValue;

            file = new File();
            this._document.Files.Add(file);

            unit = new Unit();
            file.Containers.Add(unit);

            segment = new Segment();
            unit.Resources.Add(segment);

            Console.WriteLine("Test with default values. Also tests that State is not output because SubState is not output.");
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.SegmentWithDefaultValues), actualValue);

            Console.WriteLine("Test with empty values. Also tests that State is output because SubState is output.");
            segment.Id = String.Empty;
            segment.SubState = String.Empty;
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.SegmentWithEmptyValues), actualValue);

            Console.WriteLine("Test with valid values.");
            segment.CanResegment = true;
            segment.Id = "id";
            segment.State = TranslationState.Reviewed;
            segment.SubState = "substate";
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.SegmentWithValidValues), actualValue);
        }

        /// <summary>
        /// Tests the various Serialize methods.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Serialize()
        {
            string actualValue;

            this._document.SourceLanguage = "en-us";
            this._document.Space = Preservation.Preserve;
            this._document.TargetLanguage = "de-de";
            this._document.Version = "version";

            Console.WriteLine("Test with stream.");
            this._writer.Serialize(this._stream, this._document);
            actualValue = this.GetStreamContents();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.DocumentWithValidValues), actualValue);
        }

        /// <summary>
        /// Tests that a <see cref="Skeleton"/>serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Skeleton()
        {
            File file;
            string actualValue;

            file = new File();
            file.Skeleton = new Skeleton();
            this._document.Files.Add(file);

            Console.WriteLine("Test with default values.");
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.SkeletonWithDefaultValues), actualValue);

            Console.WriteLine("Test with empty values.");
            file.Skeleton.HRef = String.Empty;
            file.Skeleton.NonTranslatableText = String.Empty;
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.SkeletonWithEmptyValues), actualValue);

            Console.WriteLine("Test with empty values.");
            file.Skeleton.HRef = "href";
            file.Skeleton.NonTranslatableText = "text";
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.SkeletonWithValidValues), actualValue);
        }

        /// <summary>
        /// Tests that a <see cref="Source"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Source()
        {
            Source source;
            Unit unit;
            string actualValue;

            this._document.Files.Add(new File());
            unit = new Unit();
            this._document.Files[0].Containers.Add(unit);
            unit.Resources.Add(new Segment());
            source = new Source();
            unit.Resources[0].Source = source;

            Console.WriteLine("Test with default values.");
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.SourceWithDefaultValues), actualValue);

            Console.WriteLine("Test with empty values.");
            source.Language = String.Empty;
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.SourceWithEmptyValues), actualValue);

            Console.WriteLine("Test with valid values.");
            source.Language = "en-us";
            source.Space = Preservation.Preserve;
            source.Text.Add(new CodePoint(1));
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.SourceWithValidValues), actualValue);
        }

        /// <summary>
        /// Tests that a <see cref="Target"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Target()
        {
            Target target;
            Unit unit;
            string actualValue;

            this._document.Files.Add(new File());
            unit = new Unit();
            this._document.Files[0].Containers.Add(unit);
            unit.Resources.Add(new Segment());
            target = new Target();
            unit.Resources[0].Target = target;

            Console.WriteLine("Test with default values.");
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.TargetWithDefaultValues), actualValue);

            Console.WriteLine("Test with empty values.");
            target.Language = String.Empty;
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.TargetWithEmptyValues), actualValue);

            Console.WriteLine("Test with valid values.");
            target.Language = "en-us";
            target.Order = 100;
            target.Space = Preservation.Preserve;
            target.Text.Add(new CodePoint(1));
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.TargetWithValidValues), actualValue);
        }

        /// <summary>
        /// Tests that a <see cref="Unit"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Unit()
        {
            File file;
            Unit unit;
            string actualValue;

            file = new File();
            this._document.Files.Add(file);

            unit = new Unit();
            file.Containers.Add(unit);

            Console.WriteLine("Test with default values.");
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.UnitWithDefaultValues), actualValue);

            Console.WriteLine("Test with empty values.");
            unit.Id = String.Empty;
            unit.Name = String.Empty;
            unit.Type = String.Empty;
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.UnitWithEmptyValues), actualValue);

            Console.WriteLine("Test with valid values.");
            unit.CanResegment = false;
            unit.Id = "id";
            unit.Name = "name";
            unit.SourceDirectionality = ContentDirectionality.Auto;
            unit.Space = Preservation.Default;
            unit.TargetDirectionality = ContentDirectionality.LTR;
            unit.Translate = true;
            unit.Type = "type";
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.UnitWithValidValues), actualValue);
        }

        /// <summary>
        /// Tests that <see cref="Note"/>s in a <see cref="Unit"/> serialize correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_UnitNotes()
        {
            File file;
            Unit unit;
            string actualValue;

            file = new File();
            this._document.Files.Add(file);

            unit = new Unit();
            file.Containers.Add(unit);

            Console.WriteLine("Test with default values.");
            unit.Notes.Add(new Note());
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.UnitNoteWithDefaultValues), actualValue);

            Console.WriteLine("Test with empty values.");
            unit.Notes[0].Category = String.Empty;
            unit.Notes[0].Id = String.Empty;
            unit.Notes[0].Text = String.Empty;
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.UnitNoteWithEmptyValues), actualValue);

            Console.WriteLine("Test with valid values.");
            unit.Notes[0].AppliesTo = TranslationSubject.Target;
            unit.Notes[0].Category = "category";
            unit.Notes[0].Id = "id";
            unit.Notes[0].Priority = 100;
            unit.Notes[0].Text = "text";
            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.UnitNoteWithValidValues), actualValue);
        }

        /// <summary>
        /// Tests that a <see cref="Validation"/> serializes correctly.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffWriter_Validation()
        {
            Validation validation;
            Rule rule;
            string actualValue;

            validation = new Validation();

            rule = new Rule();
            rule.CaseSensitive = true;
            rule.Disabled = false;
            rule.EndsWith = "endswith";
            rule.ExistsInSource = true;
            rule.IsNotPresent = null;
            rule.IsPresent = null;
            rule.Normalization = NormalizationValue.NFC;
            rule.Occurs = 2;
            rule.StartsWith = null;
            validation.Rules.Add(rule);

            rule = new Rule();
            rule.CaseSensitive = true;
            rule.Disabled = false;
            rule.EndsWith = null;
            rule.ExistsInSource = true;
            rule.IsNotPresent = "isnotpresent";
            rule.IsPresent = null;
            rule.Normalization = NormalizationValue.NFC;
            rule.Occurs = 2;
            rule.StartsWith = null;
            validation.Rules.Add(rule);

            rule = new Rule();
            rule.CaseSensitive = true;
            rule.Disabled = false;
            rule.EndsWith = null;
            rule.ExistsInSource = true;
            rule.IsNotPresent = null;
            rule.IsPresent = "ispresent";
            rule.Normalization = NormalizationValue.NFC;
            rule.Occurs = 2;
            rule.StartsWith = null;
            validation.Rules.Add(rule);

            rule = new Rule();
            rule.CaseSensitive = true;
            rule.Disabled = false;
            rule.EndsWith = null;
            rule.ExistsInSource = true;
            rule.IsNotPresent = null;
            rule.IsPresent = null;
            rule.Normalization = NormalizationValue.NFC;
            rule.Occurs = 2;
            rule.StartsWith = "startswith";
            validation.Rules.Add(rule);

            this._document.Files.Add(new File("f1"));
            this._document.Files[0].ValidationRules = validation;

            actualValue = this.Serialize();
            Assert.AreEqual(TestUtilities.GetFileContents(TestData.FileWithValidation), actualValue);
        }
        #endregion Test Methods

        #region Helper Methods
        /// <summary>
        /// Gets the contents of the stream used to write the document. It also checks for the Xml header and
        /// asserts that it is correct and returns the stream without the header.
        /// </summary>
        /// <returns>The contents of the stream.</returns>
        private string GetStreamContents()
        {
            return XliffWriterTests.GetStreamContents(this._stream);
        }

        /// <summary>
        /// Gets the contents of the stream used to write the document. It also checks for the Xml header and
        /// asserts that it is correct and returns the stream without the header.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>The contents of the stream.</returns>
        private static string GetStreamContents(System.IO.Stream stream)
        {
            string result;

            stream.Seek(0, System.IO.SeekOrigin.Begin);
            using (System.IO.TextReader reader = new System.IO.StreamReader(stream, Encoding.UTF8, true, 1024, true))
            {
                const string xmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

                result = reader.ReadToEnd();
                Assert.IsTrue(result.StartsWith(xmlHeader), "Xml header is missing.");
            }

            return result;
        }

        /// <summary>
        /// Writes the internal document to the internal stream and returns the contents of the stream.
        /// </summary>
        /// <returns>The contents of the stream.</returns>
        private string Serialize()
        {
            this._stream.Seek(0, System.IO.SeekOrigin.Begin);
            this._writer.Serialize(this._stream, this._document);
            return this.GetStreamContents();
        }

        /// <summary>
        /// Writes the internal document to the internal stream and returns the contents of the stream using a
        /// specific output detail.
        /// </summary>
        /// <param name="detail">The output detail to use when writing.</param>
        /// <returns>The contents of the stream.</returns>
        private string Serialize(OutputDetail detail)
        {
            XliffWriterSettings settings;
            XliffWriter writer;

            settings = new XliffWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.Detail = detail;
            
            this._stream.Seek(0, System.IO.SeekOrigin.Begin);
            writer = new XliffWriter(settings);

            writer.Serialize(this._stream, this._document);
            return this.GetStreamContents();
        }
        #endregion Helper Methods

        #region IDisposable Implementation
        /// <summary>
        /// Disposes of the object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        /// <param name="disposing">True if Dispose was called, otherwise false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && (this._stream != null))
            {
                this._stream.Dispose();
                this._stream = null;
            }
        }
        #endregion IDisposable Implementation
    }
}
