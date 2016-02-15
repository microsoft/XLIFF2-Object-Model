namespace Localization.Xliff.OM.Validators.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Exceptions;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.ChangeTracking;
    using Localization.Xliff.OM.Modules.FormatStyle;
    using Localization.Xliff.OM.Modules.Glossary;
    using Localization.Xliff.OM.Modules.Metadata;
    using Localization.Xliff.OM.Modules.ResourceData;
    using Localization.Xliff.OM.Modules.SizeRestriction;
    using Localization.Xliff.OM.Modules.TranslationCandidates;
    using Localization.Xliff.OM.Modules.Validation;
    using Localization.Xliff.OM.Serialization;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="StandardValidator"/> class.
    /// </summary>
    [DeploymentItem(TestUtilities.TestDataDirectory, TestUtilities.TestDataDirectory)]
    [TestClass()]
    public class StandardValidatorTests
    {
        #region Member Variables
        /// <summary>
        /// The document used for validation.
        /// </summary>
        private XliffDocument _document;

        /// <summary>
        /// The reader used to read the document stream.
        /// </summary>
        private static XliffReader _reader;

        /// <summary>
        /// The stream that stores the document contents.
        /// </summary>
        private static MemoryStream _stream;

        /// <summary>
        /// The validator under test.
        /// </summary>
        private static IXliffValidator _validator;
        #endregion Member Variables

        /// <summary>
        /// This method is called once before all tests execute to initialize the class.
        /// </summary>
        /// <param name="context">The content under which the tests are running.</param>
        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            string path;

            StandardValidatorTests._stream = new MemoryStream();

            path = Path.Combine(Environment.CurrentDirectory, TestUtilities.TestDataDirectory, "ValidDocument.xlf");
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyTo(StandardValidatorTests._stream);
            }

            StandardValidatorTests._reader = new XliffReader();
            StandardValidatorTests._validator = new StandardValidator();
        }

        /// <summary>
        /// Initializes the test class before every test method is executed.
        /// </summary>
        [TestInitialize()]
        public void TestInitialize()
        {
            this._document = new XliffDocument();
        }

        #region Test Methods
        /// <summary>
        /// Tests the validator for <see cref="ChangeTrack"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_ChangeTrack_ChangeTrack()
        {
            ChangeTrack change;
            Item item;
            Revision revision;
            RevisionsContainer container;

            change = new ChangeTrack();

            Console.WriteLine("Test that ChangeTrack has at least on RevisionsContainer.");
            this.DeserializeDocument();
            this._document.Files[0].Changes = change;
            this.VerifyValidationException(ValidationError.ChangeTrackMissingRevisions);

            Console.WriteLine("Test with a valid change track.");
            item = new Item();
            item.Property = "property";
            revision = new Revision();
            revision.Items.Add(item);
            container = new RevisionsContainer();
            container.AppliesTo = "appliesTo";
            container.Revisions.Add(revision);
            change.Revisions.Add(container);
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="Item"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_ChangeTrack_Item()
        {
            ChangeTrack change;
            Item item;
            Revision revision;
            RevisionsContainer container;

            item = new Item();
            revision = new Revision();
            revision.Items.Add(item);
            container = new RevisionsContainer();
            container.AppliesTo = "appliesTo";
            container.Revisions.Add(revision);
            change = new ChangeTrack();
            change.Revisions.Add(container);

            this.DeserializeDocument();
            this._document.Files[0].Changes = change;

            Console.WriteLine("Test with null property.");
            item.Property = null;
            this.VerifyValidationException(ValidationError.ItemPropertyNull);

            Console.WriteLine("Test with empty property.");
            item.Property = null;
            this.VerifyValidationException(ValidationError.ItemPropertyNull);

            Console.WriteLine("Test with whitespace only property.");
            item.Property = "   ";
            this.VerifyValidationException(ValidationError.ItemPropertyNull);

            Console.WriteLine("Test with non-empty property.");
            item.Property = "property";
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="Revision"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_ChangeTrack_Revision()
        {
            ChangeTrack change;
            Item item;
            Revision revision;
            RevisionsContainer container;

            item = new Item();
            item.Property = "property";
            revision = new Revision();
            container = new RevisionsContainer();
            container.AppliesTo = "appliesTo";
            container.Revisions.Add(revision);
            change = new ChangeTrack();
            change.Revisions.Add(container);

            this.DeserializeDocument();
            this._document.Files[0].Changes = change;

            Console.WriteLine("Test with no items.");
            this.VerifyValidationException(ValidationError.RevisionMissingItems);

            Console.WriteLine("Test with invalid version.");
            revision.Items.Add(item);
            revision.Version = "!!@";
            this.VerifyValidationException(ValidationError.RevisionVersionNotNMToken);

            Console.WriteLine("Test with valid data.");
            revision.Version = "version";
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="RevisionsContainer"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_ChangeTrack_RevisionsContainer()
        {
            ChangeTrack change;
            Item item;
            Revision revision;
            RevisionsContainer container;

            item = new Item();
            item.Property = "property";
            revision = new Revision();
            revision.Items.Add(item);
            container = new RevisionsContainer();
            change = new ChangeTrack();
            change.Revisions.Add(container);

            this.DeserializeDocument();
            this._document.Files[0].Changes = change;

            Console.WriteLine("Test with no revisions.");
            container.AppliesTo = "appliesTo";
            container.Reference = "reference";
            container.CurrentVersion = "version";
            this.VerifyValidationException(ValidationError.RevisionsContainerMissingRevisions);
            container.Revisions.Add(revision);

            Console.WriteLine("Test with invalid appliesTo.");
            container.AppliesTo = "!!@";
            this.VerifyValidationException(ValidationError.RevisionsContainerAppliesToNotNMToken);
            container.AppliesTo = "appliesTo";

            Console.WriteLine("Test with invalid reference.");
            container.Reference = "!!@";
            this.VerifyValidationException(ValidationError.RevisionsContainerReferenceNotNMToken);
            container.Reference = "reference";

            Console.WriteLine("Test with invalid currentVersion.");
            container.CurrentVersion = "!!@";
            this.VerifyValidationException(ValidationError.RevisionsContainerCurrentVersionNotNMToken);
            container.CurrentVersion = "version";

            Console.WriteLine("Test with valid data.");
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="Data"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Data()
        {
            Unit unit1;
            Unit unit2;

            Console.WriteLine("Test with duplicate id.");
            this.DeserializeDocument();
            unit1 = ((Group)(this._document.Files[0].Containers[0])).Containers[0] as Unit;
            unit1.OriginalData.DataElements[1].Id = unit1.OriginalData.DataElements[0].Id;
            this.VerifyValidationException(ValidationError.ElementIdDuplicate);

            Console.WriteLine("Test with duplicate id across units is ok.");
            this.DeserializeDocument();
            unit1 = ((Group)(this._document.Files[0].Containers[0])).Containers[0] as Unit;
            unit2 = ((Group)(this._document.Files[0].Containers[0])).Containers[1] as Unit;
            unit2.OriginalData.DataElements[0].Id = unit1.OriginalData.DataElements[0].Id;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with invalid space.");
            this.DeserializeDocument();
            unit1 = ((Group)(this._document.Files[0].Containers[0])).Containers[0] as Unit;

            try
            {
                unit1.OriginalData.DataElements[0].Space = Preservation.Default;
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        /// <summary>
        /// Tests the validator for <see cref="Document"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Document()
        {
            Console.WriteLine("Test with no files.");
            this.DeserializeDocument();
            this._document.Files.Clear();
            this.VerifyValidationException(ValidationError.DocumentMissingFile);

            Console.WriteLine("Test with null version.");
            this.DeserializeDocument();
            this._document.Version = null;
            this.VerifyValidationException(ValidationError.DocumentVersionNull);

            Console.WriteLine("Test with empty version.");
            this.DeserializeDocument();
            this._document.Version = String.Empty;
            this.VerifyValidationException(ValidationError.DocumentVersionNull);

            Console.WriteLine("Test with null source language.");
            this.DeserializeDocument();
            this._document.SourceLanguage = null;
            this.VerifyValidationException(ValidationError.DocumentSourceLangNull);

            Console.WriteLine("Test with empty source language.");
            this.DeserializeDocument();
            this._document.SourceLanguage = String.Empty;
            this.VerifyValidationException(ValidationError.DocumentSourceLangNull);

            Console.WriteLine("Test with null target language and targets.");
            this.DeserializeDocument();
            this._document.TargetLanguage = null;
            this.VerifyValidationException(ValidationError.TargetLangMismatch);

            Console.WriteLine("Test with empty target language and targets.");
            this.DeserializeDocument();
            this._document.TargetLanguage = String.Empty;
            this.VerifyValidationException(ValidationError.DocumentTargetLangInvalid);

            Console.WriteLine("Test with targets and no target language.");
            this.DeserializeDocument();
            foreach (ContainerResource container in this._document.CollapseChildren<ContainerResource>())
            {
                container.Target = new Target();
                container.Target.Space = container.Source.Space;
            }

            this._document.TargetLanguage = null;
            this.VerifyValidationException(ValidationError.DocumentMissingTargetLang);

            Console.WriteLine("Test with targets and empty target language.");
            this.DeserializeDocument();
            foreach (ContainerResource container in this._document.CollapseChildren<ContainerResource>())
            {
                container.Target = new Target();
                container.Target.Space = container.Source.Space;
            }

            this._document.TargetLanguage = null;
            this.VerifyValidationException(ValidationError.DocumentMissingTargetLang);

            Console.WriteLine("Test with target language and no targets.");
            this.DeserializeDocument();
            foreach (ContainerResource container in this._document.CollapseChildren<ContainerResource>())
            {
                container.Target = null;
            }

            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator editing hints around CanCopy.
        /// </summary>
        [TestCategory(TestUtilities.UnitTestCategory)]
        [TestMethod()]
        public void StandardValidator_EditingHintsCanCopy()
        {
            MarkedSpan mrk;
            Segment segment;
            SpanningCode sourceCode;
            SpanningCode targetCode;
            Unit unit;

            Console.WriteLine("Test with copy from source.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();

            sourceCode = new SpanningCode("pc1");
            sourceCode.CanCopy = false;
            segment.Source.Text.Add(sourceCode);

            mrk = new MarkedSpan("mrk1");
            segment.Target.Text.Add(mrk);

            targetCode = new SpanningCode("pc2");
            targetCode.CopyOf = "pc1";
            targetCode.DataReferenceEnd = null;
            targetCode.DataReferenceStart = null;
            mrk.Text.Add(targetCode);

            this.VerifyValidationException(ValidationError.CodeBaseTargetCopyOfTypeMismatchOrNotCanCopy);

            Console.WriteLine("Test with copy from target.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();

            sourceCode = new SpanningCode("pc1");
            sourceCode.CanCopy = false;
            segment.Target.Text.Add(sourceCode);

            mrk = new MarkedSpan("mrk1");
            segment.Target.Text.Add(mrk);

            targetCode = new SpanningCode("pc2");
            targetCode.CopyOf = "pc1";
            mrk.Text.Add(targetCode);

            this.VerifyValidationException(ValidationError.CodeBaseTargetCopyOfTypeMismatchOrNotCanCopy);
        }

        /// <summary>
        /// Tests the validator editing hints around CanDelete.
        /// </summary>
        [TestCategory(TestUtilities.UnitTestCategory)]
        [TestMethod()]
        public void StandardValidator_EditingHintsCanDelete()
        {
            MarkedSpan mrk;
            Segment segment;
            SpanningCode sourceCode;
            SpanningCodeStart startCode;
            Unit unit;

            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();

            sourceCode = new SpanningCode("pc1");
            sourceCode.CanDelete = false;
            segment.Source.Text.Add(sourceCode);

            mrk = new MarkedSpan("mrk1");
            segment.Target.Text.Add(mrk);

            this.VerifyValidationException(ValidationError.CodeBaseTagDeleted);

            // Add a start tag to the target. Since the Id doesn't match the pc, validation still fails.
            startCode = new SpanningCodeStart("sc1");
            segment.Target.Text.Add(startCode);
            this.VerifyValidationException(ValidationError.CodeBaseTagDeleted);

            // Name the sc tag the same as the pc tag. Validation fails because the sc tag is isolated.
            startCode.Id = "pc1";
            startCode.Isolated = true;
            this.VerifyValidationException(ValidationError.ContainerResourceTypesWithSameIdMismatch);

            // Add the ec and make the sc not isolated. Now validation should pass because sc and pc are equivalent.
            startCode.Isolated = false;
            segment.Target.Text.Add(new SpanningCodeEnd() { StartReference = startCode.Id });
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator editing hints around CanReorder.
        /// </summary>
        [TestCategory(TestUtilities.UnitTestCategory)]
        [TestMethod()]
        public void StandardValidator_EditingHintsCanReorder()
        {
            MarkedSpan mrk;
            Segment segment;
            SpanningCode span;
            Unit unit;

            Console.WriteLine("Test with no at start in source.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment.Source.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Target.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.Yes });
            this.VerifyValidationException(ValidationError.CodeBaseSequenceStartsWithCanReorderNo);

            Console.WriteLine("Test with no at start in target.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment.Source.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.Yes });
            segment.Target.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            this.VerifyValidationException(ValidationError.CodeBaseSequenceStartsWithCanReorderNo);

            Console.WriteLine("Test with 1 element in sequence.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment.Source.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            segment.Target.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.Yes });
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with missing element in sequence.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment.Source.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            mrk = new MarkedSpan("mrk1");
            segment.Source.Text.Add(mrk);
            mrk.Text.Add(new StandaloneCode("ph1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Target.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            this.VerifyValidationException(ValidationError.CodeBaseTagDeleted);

            Console.WriteLine("Test with reordered sequence.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment.Source.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            mrk = new MarkedSpan("mrk1");
            segment.Source.Text.Add(mrk);
            mrk.Text.Add(new StandaloneCode("ph1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Target.Text.Add(new StandaloneCode("ph1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            segment.Target.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            this.VerifyValidationException(ValidationError.CodeBaseSequenceNotFound);

            Console.WriteLine("Test with added element in sequence.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment.Source.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            mrk = new MarkedSpan("mrk1");
            segment.Source.Text.Add(mrk);
            mrk.Text.Add(new StandaloneCode("ph1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Target.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            segment.Target.Text.Add(new StandaloneCode("ph1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Target.Text.Add(new SpanningCodeStart("sc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            this.VerifyValidationException(ValidationError.CodeBaseSequenceMismatch);

            Console.WriteLine("Test with missing sequence.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment.Source.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            mrk = new MarkedSpan("mrk1");
            segment.Source.Text.Add(mrk);
            mrk.Text.Add(new StandaloneCode("ph1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            this.VerifyValidationException(ValidationError.CodeBaseTagDeleted);

            Console.WriteLine("Test with CanCopy = true.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment.Source.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            mrk = new MarkedSpan("mrk1");
            segment.Source.Text.Add(mrk);
            mrk.Text.Add(new StandaloneCode("ph1") { CanCopy = true, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Target.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            segment.Target.Text.Add(new StandaloneCode("ph1") { CanCopy = true, CanDelete = false, CanReorder = CanReorderValue.No });
            this.VerifyValidationException(ValidationError.CodeBaseMismatchedCanReorderCopyDelete);

            Console.WriteLine("Test with CanDelete = true.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment.Source.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            mrk = new MarkedSpan("mrk1");
            segment.Source.Text.Add(mrk);
            mrk.Text.Add(new StandaloneCode("ph1") { CanCopy = false, CanDelete = true, CanReorder = CanReorderValue.No });
            segment.Target.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            segment.Target.Text.Add(new StandaloneCode("ph1") { CanCopy = false, CanDelete = true, CanReorder = CanReorderValue.No });
            this.VerifyValidationException(ValidationError.CodeBaseMismatchedCanReorderCopyDelete);

            Console.WriteLine("Test with multiple sequence where sequences are reordered.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment.Source.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            mrk = new MarkedSpan("mrk1");
            segment.Source.Text.Add(mrk);
            mrk.Text.Add(new StandaloneCode("ph1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Source.Text.Add(new SpanningCode("pc2") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            mrk = new MarkedSpan("mrk2");
            segment.Source.Text.Add(mrk);
            mrk.Text.Add(new StandaloneCode("ph2") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            mrk.Text.Add(new StandaloneCode("ph3") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Source.Text.Add(new SpanningCodeStart("sc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No, Isolated = true });
            segment.Source.Text.Add(new SpanningCodeEnd("ec1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No, Isolated = true });
            segment.Source.Text.Add(new SpanningCodeEnd("ec2") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.Yes, Isolated = true });
            segment.Target.Text.Add(new StandaloneCode("ph2") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            segment.Target.Text.Add(new StandaloneCode("ph3") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Target.Text.Add(new SpanningCodeStart("sc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No, Isolated = true });
            segment.Target.Text.Add(new SpanningCodeEnd("ec1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No, Isolated = true });
            segment.Target.Text.Add(new SpanningCodeEnd("ec2") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.Yes, Isolated = true });
            segment.Target.Text.Add(new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            segment.Target.Text.Add(new StandaloneCode("ph1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Target.Text.Add(new SpanningCode("pc2") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with invalid source hierarchy.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            span = new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.Yes };
            span.Text.Add(new SpanningCode("pc2") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            segment.Source.Text.Add(span);
            segment.Source.Text.Add(new SpanningCode("pc3") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Target = null;
            this.VerifyValidationException(ValidationError.CodeBaseInvalidSourceSequenceHierarchy);

            Console.WriteLine("Test with invalid target hierarchy.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            span = new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.Yes };
            span.Text.Add(new SpanningCode("pc2") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            span.Text.Add(new SpanningCode("pc3") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Source.Text.Add(span);
            span = new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.Yes };
            span.Text.Add(new SpanningCode("pc2") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo });
            segment.Target.Text.Add(span);
            segment.Target.Text.Add(new SpanningCode("pc3") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            this.VerifyValidationException(ValidationError.CodeBaseInvalidTargetSequenceHierarchy);

            Console.WriteLine("Test with mismatched hierarchy.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            span = new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo };
            span.Text.Add(new SpanningCode("pc2") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            span.Text.Add(new SpanningCode("pc3") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Source.Text.Add(span);
            span = new SpanningCode("pc1") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.FirstNo };
            span.Text.Add(new SpanningCode("pc2") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            segment.Target.Text.Add(span);
            segment.Target.Text.Add(new SpanningCode("pc3") { CanCopy = false, CanDelete = false, CanReorder = CanReorderValue.No });
            this.VerifyValidationException(ValidationError.CodeBaseSequenceHierarchyMismatch);
        }

        /// <summary>
        /// Tests the validator for <see cref="File"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_File()
        {
            Console.WriteLine("Test with no units or groups.");
            this.DeserializeDocument();
            this._document.Files[0].Containers.Clear();
            this.VerifyValidationException(ValidationError.FileMissingContainer);

            Console.WriteLine("Test with null id.");
            this.DeserializeDocument();
            this._document.Files[0].Id = null;
            this.VerifyValidationException(ValidationError.FileIdNull);

            Console.WriteLine("Test with empty id.");
            this.DeserializeDocument();
            this._document.Files[0].Id = null;
            this.VerifyValidationException(ValidationError.FileIdNull);

            Console.WriteLine("Test with duplicate id.");
            this.DeserializeDocument();
            this._document.Files[0].Id = this._document.Files[1].Id;
            this.VerifyValidationException(ValidationError.ElementIdDuplicate);
        }

        /// <summary>
        /// Tests the validator for Format Style module attribute constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_FormatStyle_Attributes()
        {
            Core.File file;
            Segment segment;
            SpanningCodeEnd end;
            SpanningCodeStart start;
            Unit unit;

            segment = new Segment("sx");
            segment.Source = new Source();
            unit = new Unit("ux");
            unit.Resources.Add(segment);

            file = new Core.File("fx");
            file.Containers.Add(unit);

            this.DeserializeDocument();
            this._document.Files.Add(file);

            start = new SpanningCodeStart();
            start.Id = "sc1";
            segment.Source.Text.Add(start);

            end = new SpanningCodeEnd();
            end.Isolated = false;
            end.FormatStyle = FormatStyleValue.Anchor;
            end.StartReference = start.Id;
            segment.Source.Text.Add(end);

            Console.WriteLine("Test fs with ec not isolated.");
            this.VerifyValidationException(ValidationError.FormatStyleWithSpanEndNotIsolated);
            end.Id = "ec1";
            end.Isolated = true;
            end.StartReference = null;
            start.Isolated = true;

            Console.WriteLine("Test subfs without fs.");
            end.FormatStyle = null;
            end.SubFormatStyle.Add("key", "value");
            this.VerifyValidationException(ValidationError.FormatStyleSubFormatWithoutFormat);

            Console.WriteLine("Test with valid data.");
            end.FormatStyle = FormatStyleValue.Anchor;
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="Glossary"/> constraints.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Glossary()
        {
            Glossary glossary;

            Console.WriteLine("Test with no entries.");
            this.DeserializeDocument();
            glossary = this._document.CollapseChildren<Glossary>()[0];
            glossary.Entries.Clear();
            this.VerifyValidationException(ValidationError.GlossaryMissingEntry);

            Console.WriteLine("Test duplicate entry Id.");
            this.DeserializeDocument();
            glossary = this._document.CollapseChildren<Glossary>()[0];
            glossary.Entries[0].Id = glossary.Entries[1].Id;
            this.VerifyValidationException(ValidationError.GlossaryEntryIdDuplicate);

            Console.WriteLine("Test invalid entry reference.");
            this.DeserializeDocument();
            glossary = this._document.CollapseChildren<Glossary>()[0];
            glossary.Entries[0].Reference = "bogus";
            this.VerifyValidationException(ValidationError.IriInvalid);

            Console.WriteLine("Test entry with no translation or definition.");
            this.DeserializeDocument();
            glossary = this._document.CollapseChildren<Glossary>()[0];
            glossary.Entries[0].Definition = null;
            glossary.Entries[0].Translations.Clear();
            this.VerifyValidationException(ValidationError.GlossaryEntryChildrenMissing);

            Console.WriteLine("Test duplicate translation Id.");
            this.DeserializeDocument();
            glossary = this._document.CollapseChildren<Glossary>()[0];
            glossary.Entries[0].Translations[0].Id = glossary.Entries[1].Translations[0].Id;
            this.VerifyValidationException(ValidationError.GlossaryTranslationIdDuplicate);

            Console.WriteLine("Test translation entry reference.");
            this.DeserializeDocument();
            glossary = this._document.CollapseChildren<Glossary>()[0];
            glossary.Entries[0].Translations[0].Reference = "bogus";
            this.VerifyValidationException(ValidationError.IriInvalid);

            Console.WriteLine("Test duplicate Id between glossary entry and translation.");
            this.DeserializeDocument();
            glossary = this._document.CollapseChildren<Glossary>()[0];
            glossary.Entries[0].Id = glossary.Entries[0].Translations[0].Id;
            this.VerifyValidationException(ValidationError.GlossaryTranslationIdDuplicate);
        }

        /// <summary>
        /// Tests the validator for <see cref="Group"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Group()
        {
            Console.WriteLine("Test with null id.");
            this.DeserializeDocument();
            this._document.Files[0].Containers.First(c => c is Group).Id = null;
            this.VerifyValidationException(ValidationError.TranslationContainerIdNull);

            Console.WriteLine("Test with empty id.");
            this.DeserializeDocument();
            this._document.Files[0].Containers.First(c => c is Group).Id = String.Empty;
            this.VerifyValidationException(ValidationError.TranslationContainerIdNull);

            Console.WriteLine("Test with duplicate id.");
            this.DeserializeDocument();
            foreach (Group group in this._document.CollapseChildren<Group>())
            {
                group.Id = "groupid";
            }

            this.VerifyValidationException(ValidationError.ElementIdDuplicate);

            Console.WriteLine("Test with type not like prefix:value.");
            this.DeserializeDocument();
            this._document.Files[0].Containers.First(c => c is Group).Type = "a";
            this.VerifyValidationException(ValidationError.PrefixValueMissingColon);

            Console.WriteLine("Test with type using xlf:value.");
            this.DeserializeDocument();
            this._document.Files[0].Containers.First(c => c is Group).Type = "Xlf:value";
            this.VerifyValidationException(ValidationError.PrefixValueInvalid);

            Console.WriteLine("Test with type using prefix:value:value is ok.");
            this.DeserializeDocument();
            this._document.Files[0].Containers.First(c => c is Group).Type = "prefix:value:value";
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="Ignorable"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Ignorable()
        {
            Unit unit;

            Console.WriteLine("Test with null source.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            unit.Resources.Where(c => c is Ignorable).Cast<Ignorable>().First().Source = null;
            this.VerifyValidationException(ValidationError.SourceNull);

            Console.WriteLine("Test with duplicate id by segment.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            unit.Resources.Where(c => c is Segment).Cast<Segment>().First().Id = "ignorableId";
            unit.Resources.Where(c => c is Ignorable).Cast<Ignorable>().First().Id = "ignorableId";
            this.VerifyValidationException(ValidationError.ElementIdDuplicate);

            Console.WriteLine("Test with duplicate id by ignorable.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            foreach (Ignorable ignorable in unit.Resources.Where(c => c is Ignorable).Cast<Ignorable>())
            {
                ignorable.Id = "ignorableId";
            }

            this.VerifyValidationException(ValidationError.ElementIdDuplicate);
        }

        /// <summary>
        /// Tests the validator for <see cref="MarkedSpan"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_MarkedSpan()
        {
            MarkedSpan span;
            Segment segment;
            Unit unit;

            span = new MarkedSpan();

            Console.WriteLine("Test with null Id.");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            this.VerifyValidationException(ValidationError.MarkedSpanIdNull);

            Console.WriteLine("Test with empty Id.");
            span.Id = String.Empty;
            this.VerifyValidationException(ValidationError.MarkedSpanIdNull);

            Console.WriteLine("Test with duplicate Id.");
            segment.Id = "duplicateId";
            span.Id = segment.Id;
            this.VerifyValidationException(ValidationError.ElementIdDuplicate);

            Console.WriteLine("Test with item on target not matching source.");
            span.Id = "newSpanId";
            segment.Target.Text.Add(new MarkedSpan("bogus"));
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with type like prefix:value.");
            span = new MarkedSpan("spanId");
            span.Value = "comment";
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.Type = "my:type";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with type not like prefix:value.");
            span = new MarkedSpan("spanId");
            span.Value = "comment";
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.Type = "a";
            this.VerifyValidationException(ValidationError.MarkedSpanInvalidType);

            Console.WriteLine("Test with type using comment.");
            span.Type = MarkedSpanTypes.Comment;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with type using generic.");
            span.Type = MarkedSpanTypes.Generic;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with type using term.");
            span.Type = MarkedSpanTypes.Term;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with null type.");
            span.Type = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test a relative path to a note.");
            span.Reference = "#" + unit.Notes[0].SelectorId;
            span.Type = MarkedSpanTypes.Comment;
            span.Value = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test a full path to a note.");
            span.Reference = unit.Notes[0].SelectorPath;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test a full path to a note outside the unit.");
            span.Reference = ((Unit)this._document.Files[1].Containers.First(c => c is Unit)).Notes[0].SelectorPath;
            this.VerifyValidationException(ValidationError.MarkedSpanInvalidReference);
        }

        /// <summary>
        /// Tests the validator for <see cref="MarkedSpanEnd"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_MarkedSpanEnd()
        {
            MarkedSpanEnd span;
            MarkedSpanStart startSpan;
            Segment segment;
            Unit unit;

            span = new MarkedSpanEnd();

            Console.WriteLine("Test with StartReference matching sm.");
            span = new MarkedSpanEnd("spanId");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            startSpan = new MarkedSpanStart("smId");
            startSpan.Type = "term";
            segment.Source.Text.Add(startSpan);
            segment.Source.Text.Add(span);
            span.StartReference = "smId";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with StartReference not matching sm.");
            span.StartReference = "bogus";
            this.VerifyValidationException(ValidationError.TagStartRefInvalid);

            Console.WriteLine("Test with StartReference is null.");
            span.StartReference = null;
            this.VerifyValidationException(ValidationError.MarkedSpanEndStartRefNull);

            Console.WriteLine("Test with StartReference is empty.");
            span.StartReference = String.Empty;
            this.VerifyValidationException(ValidationError.MarkedSpanEndStartRefNull);
        }

        /// <summary>
        /// Tests the validator for <see cref="MarkedSpanStart"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_MarkedStart()
        {
            MarkedSpanStart span;
            Segment segment;
            Unit unit;

            span = new MarkedSpanStart();

            Console.WriteLine("Test with null Id.");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            this.VerifyValidationException(ValidationError.MarkedSpanStartIdNull);

            Console.WriteLine("Test with empty Id.");
            span.Id = String.Empty;
            this.VerifyValidationException(ValidationError.MarkedSpanStartIdNull);

            Console.WriteLine("Test with duplicate Id.");
            segment.Id = "duplicateId";
            span.Id = segment.Id;
            this.VerifyValidationException(ValidationError.ElementIdDuplicate);

            Console.WriteLine("Test with item on target not matching source.");
            span.Id = "newSpanId";
            segment.Target.Text.Add(new MarkedSpan("bogus"));
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with type like prefix:value.");
            span = new MarkedSpanStart("spanId");
            span.Value = "comment";
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.Type = "my:type";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with type not like prefix:value.");
            span = new MarkedSpanStart("spanId");
            span.Value = "comment";
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.Type = "a";
            this.VerifyValidationException(ValidationError.MarkedSpanStartInvalidType);

            Console.WriteLine("Test with type using comment.");
            span.Type = MarkedSpanTypes.Comment;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with type using generic.");
            span.Type = MarkedSpanTypes.Generic;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with type using term.");
            span.Type = MarkedSpanTypes.Term;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with null type.");
            span.Type = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test a relative path to a note.");
            span.Reference = "#" + unit.Notes[0].SelectorId;
            span.Type = MarkedSpanTypes.Comment;
            span.Value = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test a full path to a note.");
            span.Reference = unit.Notes[0].SelectorPath;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test a full path to a note outside the unit.");
            span.Reference = ((Unit)this._document.Files[1].Containers.First(c => c is Unit)).Notes[0].SelectorPath;
            this.VerifyValidationException(ValidationError.MarkedSpanStartInvalidReference);
        }

        /// <summary>
        /// Tests the validator for <see cref="Match"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Match()
        {
            Match match;
            Unit unit;

            Console.WriteLine("Test with no source.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].Source = null;
            this.VerifyValidationException(ValidationError.SourceNull);

            Console.WriteLine("Test with no target.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].Target = null;
            this.VerifyValidationException(ValidationError.MatchTargetNull);

            Console.WriteLine("Test with match quality less than 0.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].MatchQuality = -1;
            this.VerifyValidationException(ValidationError.MatchQualityNotInRange);

            Console.WriteLine("Test with match quality equal to 0.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].MatchQuality = 0;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with match quality equal to 100.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].MatchQuality = 100;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with match quality greater than 100.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].MatchQuality = 101;
            this.VerifyValidationException(ValidationError.MatchQualityNotInRange);

            Console.WriteLine("Test with match suitability less than 0.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].MatchSuitability = -1;
            this.VerifyValidationException(ValidationError.MatchSuitabilityNotInRange);

            Console.WriteLine("Test with match suitability equal to 0.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].MatchSuitability = 0;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with match suitability equal to 100.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].MatchSuitability = 100;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with match suitability greater than 100.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].MatchSuitability = 101;
            this.VerifyValidationException(ValidationError.MatchSuitabilityNotInRange);

            Console.WriteLine("Test with similarity less than 0.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].Similarity = -1;
            this.VerifyValidationException(ValidationError.MatchSimilarityNotInRange);

            Console.WriteLine("Test with similarity equal to 0.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].Similarity = 0;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with similarity equal to 100.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].Similarity = 100;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with similarity greater than 100.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].Similarity = 101;
            this.VerifyValidationException(ValidationError.MatchSimilarityNotInRange);

            Console.WriteLine("Test with ref is null.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].SourceReference = null;
            this.VerifyValidationException(ValidationError.MatchSourceRefNull);

            Console.WriteLine("Test with ref is invalid.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].SourceReference = "invalid";
            this.VerifyValidationException(ValidationError.MatchMissingSourceRef);

            Console.WriteLine("Test with SubType is invalid.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].SubType = "invalid";
            this.VerifyValidationException(ValidationError.PrefixValueMissingColon);

            Console.WriteLine("Test with SubType using xlf:value.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].SubType = "Xlf:value";
            this.VerifyValidationException(ValidationError.PrefixValueInvalid);

            Console.WriteLine("Test with SubType using prefix:value:value is ok.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches[0].SubType = "prefix:value:value";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with duplicate id.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            match = unit.Matches[0];
            match.Id = "mid";
            match.Parent = null;
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.Matches.Add(match);
            unit.Matches[0].Id = match.Id;
            this.VerifyValidationException(ValidationError.ElementIdDuplicate);
            match.Id = Guid.NewGuid().ToString();
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="Match"/> constraints that reference original data.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_MatchWithOriginalData()
        {
            Match match;
            StandaloneCode code;
            Unit unit;

            Console.WriteLine("Test with valid original data reference.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            match = new Match(Utilities.MakeIri("s1"));
            match.OriginalData = new OriginalData();
            match.OriginalData.DataElements.Add(new Data("xd1"));
            match.Source = new Source();
            code = new StandaloneCode("c1");
            code.DataReference = "xd1";
            match.Source.Text.Add(code);
            match.Target = new Target();
            unit.Matches.Add(match);
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with invalid original data reference.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            match = new Match(Utilities.MakeIri("s1"));
            match.OriginalData = new OriginalData();
            match.OriginalData.DataElements.Add(new Data("xd1"));
            match.Source = new Source();
            code = new StandaloneCode("c1");
            code.DataReference = "zd1";
            match.Source.Text.Add(code);
            match.Target = new Target();
            unit.Matches.Add(match);
            this.VerifyValidationException(ValidationError.StandaloneCodeDataRefInvalid);

            Console.WriteLine("Test with original data reference not on match, but on unit.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            unit.OriginalData = new OriginalData();
            unit.OriginalData.DataElements.Add(new Data("xd1"));
            match = new Match(Utilities.MakeIri("s1"));
            match.Source = new Source();
            code = new StandaloneCode("c1");
            code.DataReference = "xd1";
            match.Source.Text.Add(code);
            match.Target = new Target();
            unit.Matches.Add(match);
            this.VerifyValidationException(ValidationError.SpanningCodeInvalidDataRefEnd);
        }

        /// <summary>
        /// Tests the validator for <see cref="Meta"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Meta()
        {
            Meta meta;
            Unit unit;

            Console.WriteLine("Test with valid metadata.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            Assert.IsNotNull(unit, "Unit was not found.");
            unit.Matches[0].Metadata = this.CreateMetadataContainer(false);
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with type is null.");
            meta = unit.Matches[0].Metadata.Groups[0].Containers[0] as Meta;
            meta.Type = null;
            this.VerifyValidationException(ValidationError.MetaTypeNull);
        }

        /// <summary>
        /// Tests the validator for <see cref="MetadataContainer"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_MetadataContainer()
        {
            MetadataContainer container;
            Unit unit;

            Console.WriteLine("Test with no groups.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            Assert.IsNotNull(unit, "Unit was not found.");
            unit.Matches[0].Metadata = this.CreateMetadataContainer(false);
            unit.Matches[0].Metadata.Groups.Clear();
            this.VerifyValidationException(ValidationError.MetadataMissingGroup);

            Console.WriteLine("Test with duplicate id.");
            unit.Matches[0].Metadata = this.CreateMetadataContainer(false);
            container = this.CreateMetadataContainer(false);
            container.Groups[0].Parent = null;
            unit.Matches[0].Metadata.Groups.Add(container.Groups[0]);
            unit.Matches[0].Metadata.Groups[1].Id = unit.Matches[0].Metadata.Id;
            this.VerifyValidationException(ValidationError.MetadataIdDuplicate);
        }

        /// <summary>
        /// Tests the validator for <see cref="MetaGroup"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_MetaGroup()
        {
            MetadataContainer container;
            MetadataContainer container2;
            Unit unit;

            Console.WriteLine("Test with no groups.");
            this.DeserializeDocument();
            unit = this._document.Select("#/f=f2/u=u10") as Unit;
            Assert.IsNotNull(unit, "Unit was not found.");
            unit.Matches[0].Metadata = this.CreateMetadataContainer(false);
            unit.Matches[0].Metadata.Groups[0].Containers.Clear();
            this.VerifyValidationException(ValidationError.MetaGroupMissingContainer);

            Console.WriteLine("Test with duplicate id.");
            unit.Matches[0].Metadata = this.CreateMetadataContainer(false);
            container = this.CreateMetadataContainer(false);
            container.Groups[0].Parent = null;
            unit.Matches[0].Metadata.Groups.Add(container.Groups[0]);
            this.VerifyValidationException(ValidationError.MetadataIdDuplicate);

            Console.WriteLine("Test more than one group with null id's passes validation.");
            unit.Matches[0].Metadata = this.CreateMetadataContainer(true);
            container = this.CreateMetadataContainer(true);
            container.Groups[0].Parent = null;
            unit.Matches[0].Metadata.Groups.Add(container.Groups[0]);
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test top level group with null Id with two child groups with duplicate id's fails validation.");
            unit.Matches[0].Metadata = this.CreateMetadataContainer(true);
            container = this.CreateMetadataContainer(false);
            container.Groups[0].Parent = null;
            unit.Matches[0].Metadata.Groups.Add(container.Groups[0]);
            container2 = this.CreateMetadataContainer(false);
            container2.Groups[0].Parent = null;
            unit.Matches[0].Metadata.Groups.Add(container2.Groups[0]);
            this.VerifyValidationException(ValidationError.MetadataIdDuplicate);

            Console.WriteLine("Test Metadata with null Id with two child groups with duplicate id's fails validation.");
            unit.Matches[0].Metadata = this.CreateMetadataContainer(false);
            unit.Matches[0].Metadata.Id = null;
            container = this.CreateMetadataContainer(false);
            container.Groups[0].Parent = null;
            unit.Matches[0].Metadata.Groups.Add(container.Groups[0]);
            this.VerifyValidationException(ValidationError.MetadataIdDuplicate);
        }

        /// <summary>
        /// Tests the validator for <see cref="Note"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Note()
        {
            Console.WriteLine("Test with low priority.");
            this.DeserializeDocument();
            this._document.Files[0].Notes[0].Priority = 0;
            this.VerifyValidationException(ValidationError.NoteInvalidPriority);

            Console.WriteLine("Test with high priority.");
            this.DeserializeDocument();
            this._document.Files[0].Notes[0].Priority = 11;
            this.VerifyValidationException(ValidationError.NoteInvalidPriority);

            Console.WriteLine("Test with priority 1 is ok.");
            this.DeserializeDocument();
            this._document.Files[0].Notes[0].Priority = 1;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with priority 10 is ok.");
            this.DeserializeDocument();
            this._document.Files[0].Notes[0].Priority = 1;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with duplicate id.");
            this.DeserializeDocument();
            this._document.Files[0].Notes[0].Id = "note1";
            this._document.Files[0].Notes[1].Id = this._document.Files[0].Notes[0].Id;
            this.VerifyValidationException(ValidationError.ElementIdDuplicate);

            Console.WriteLine("Test with duplicate id across files is ok.");
            this.DeserializeDocument();
            this._document.Files[0].Notes[0].Id = "note1";
            this._document.Files[1].Notes[0].Id = this._document.Files[0].Notes[0].Id;
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="Reference"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_ResourceData_Reference()
        {
            IExtensible extensible;
            Reference reference;
            ResourceData data;
            ResourceItem item;

            reference = new Reference();
            extensible = reference;

            item = new ResourceItem();
            item.Context = true;
            item.Id = null;
            item.MimeType = "mime";
            item.References.Add(reference);
            data = new ResourceData();
            data.ResourceItems.Add(item);

            this.DeserializeDocument();
            this._document.Files[0].ResourceData = data;

            Console.WriteLine("Test with null href.");
            reference.HRef = null;
            this.VerifyValidationException(ValidationError.ReferenceHRefNotSpecified);
            reference.HRef = "href";

            Console.WriteLine("Test with invalid language.");
            reference.Language = "--";
            this.VerifyValidationException(ValidationError.ReferenceLangInvalid);
        }

        /// <summary>
        /// Tests the validator for <see cref="ResourceData"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_ResourceData_ResourceData()
        {
            Reference reference;
            ResourceData data;
            ResourceItem item;
            ResourceItemRef itemRef;
            ResourceItemSource source;
            ResourceItemTarget target;

            item = new ResourceItem();
            data = new ResourceData();

            this.DeserializeDocument();
            this._document.Files[0].ResourceData = data;

            Console.WriteLine("Test with no items or item references.");
            this.VerifyValidationException(ValidationError.ResourceDataMissingItems);

            reference = new Reference();
            reference.HRef = "href";

            source = new ResourceItemSource();
            source.HRef = "href";

            target = new ResourceItemTarget();
            target.HRef = "href";

            itemRef = new ResourceItemRef();
            itemRef.Reference = item.Id;
            data.ResourceItemReferences.Add(itemRef);

            item.Context = true;
            item.Id = null;
            item.MimeType = "mime";
            item.References.Add(reference);
            item.Source = source;
            item.Target = target;
            data.ResourceItems.Add(item);

            Console.WriteLine("Test with valid data.");
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="ResourceItem"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_ResourceData_ResourceItem()
        {
            Reference reference;
            ResourceData data;
            ResourceItem item;
            ResourceItemRef itemRef;
            ResourceItemSource source;

            source = new ResourceItemSource();
            source.HRef = "href";

            item = new ResourceItem();
            item.Context = true;
            item.Id = null;
            item.MimeType = "mime";
            data = new ResourceData();
            data.ResourceItems.Add(item);

            this.DeserializeDocument();
            this._document.Files[0].ResourceData = data;

            Console.WriteLine("Test with no source, target or reference.");
            this.VerifyValidationException(ValidationError.ResourceItemMissingChildren);
            item.Source = source;

            Console.WriteLine("Test with invalid MimeType.");
            item.MimeType = "  ";
            this.VerifyValidationException(ValidationError.MimeTypeNotSpecified);
            item.MimeType = "mime";

            Console.WriteLine("Test with invalid Id.");
            item.Id = "!!@";
            this.VerifyValidationException(ValidationError.ResourceItemIdNotNMToken);
            item.Id = "id";

            Console.WriteLine("Test with empty source an no mime type.");
            item.MimeType = null;
            this.VerifyValidationException(ValidationError.MimeTypeNotSpecified);
            item.MimeType = "mime";

            Console.WriteLine("Test with duplicate Id.");
            itemRef = new ResourceItemRef();
            itemRef.Id = item.Id;
            itemRef.Reference = item.Id;
            data.ResourceItemReferences.Add(itemRef);
            this.VerifyValidationException(ValidationError.ResourceItemRefIdDuplicate);

            Console.WriteLine("Test with missing source when Context is false.");
            data.ResourceItemReferences.Clear();
            reference = new Reference();
            reference.HRef = "href";
            item.References.Add(reference);
            item.Context = false;
            item.Source = null;
            this.VerifyValidationException(ValidationError.ResourceItemSourceMissingWithNoContext);
        }

        /// <summary>
        /// Tests the validator for <see cref="ResourceItemRef"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_ResourceData_ResourceItemRef()
        {
            ResourceData data;
            ResourceItem item;
            ResourceItemRef itemRef1;
            ResourceItemRef itemRef2;
            ResourceItemSource source;

            source = new ResourceItemSource();
            source.HRef = "href";

            item = new ResourceItem();
            item.Context = true;
            item.Id = "itemid";
            item.MimeType = "mime";
            item.Source = source;
            data = new ResourceData();
            data.ResourceItems.Add(item);

            itemRef1 = new ResourceItemRef();
            itemRef1.Reference = item.Id;
            data.ResourceItemReferences.Add(itemRef1);

            itemRef2 = new ResourceItemRef();
            itemRef2.Reference = item.Id;
            data.ResourceItemReferences.Add(itemRef2);

            this.DeserializeDocument();
            this._document.Files[0].ResourceData = data;

            Console.WriteLine("Test with invalid Id.");
            itemRef1.Id = "!!@";
            this.VerifyValidationException(ValidationError.ResourceItemRefIdNotNMToken);
            itemRef1.Id = "id";

            Console.WriteLine("Test with duplicate Id.");
            itemRef2.Id = itemRef1.Id;
            this.VerifyValidationException(ValidationError.ResourceItemRefIdDuplicate);
            itemRef2.Id = "id2";

            Console.WriteLine("Test with null reference.");
            itemRef2.Reference = null;
            this.VerifyValidationException(ValidationError.ResourceItemRefInvalidReference);

            Console.WriteLine("Test with invalid reference.");
            itemRef2.Reference = "bogus";
            this.VerifyValidationException(ValidationError.ResourceItemRefInvalidReference);
        }

        /// <summary>
        /// Tests the validator for <see cref="ResourceItemSource"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_ResourceData_ResourceItemSource()
        {
            IExtensible extensible;
            ResourceData data;
            ResourceItem item;
            ResourceItemSource source;

            source = new ResourceItemSource();
            source.HRef = "href";

            extensible = source;

            item = new ResourceItem();
            item.Context = true;
            item.Id = "itemid";
            item.MimeType = "mime";
            item.Source = source;
            data = new ResourceData();
            data.ResourceItems.Add(item);

            this.DeserializeDocument();
            this._document.Files[0].ResourceData = data;

            Console.WriteLine("Test with null href and empty source.");
            source.HRef = null;
            this.VerifyValidationException(ValidationError.ResourceItemReferenceBaseHRefAndSubject);

            Console.WriteLine("Test with non-null href and non-empty source.");
            extensible.Extensions.Add(new GenericExtension("ext"));
            extensible.Extensions[0].AddChild(new ElementInfo(new XmlNameInfo("name"), new GenericElement()));
            source.HRef = "href";
            this.VerifyValidationException(ValidationError.ResourceItemReferenceBaseHRefAndSubject);
            extensible.Extensions.Clear();

            Console.WriteLine("Test with invalid language.");
            source.Language = "--";
            this.VerifyValidationException(ValidationError.ResourceItemSourceLangInvalid);

            Console.WriteLine("Test with language not matching document source language.");
            source.Language = "fr-zh";
            this.VerifyValidationException(ValidationError.ResourceItemSourceLangMismatch);
        }

        /// <summary>
        /// Tests the validator for <see cref="ResourceItemTarget"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_ResourceData_ResourceItemTarget()
        {
            IExtensible extensible;
            ResourceData data;
            ResourceItem item;
            ResourceItemTarget target;

            target = new ResourceItemTarget();
            target.HRef = "href";

            extensible = target;

            item = new ResourceItem();
            item.Context = true;
            item.Id = "itemid";
            item.MimeType = "mime";
            item.Target = target;
            data = new ResourceData();
            data.ResourceItems.Add(item);

            this.DeserializeDocument();
            this._document.Files[0].ResourceData = data;

            Console.WriteLine("Test with null href and empty source.");
            target.HRef = null;
            this.VerifyValidationException(ValidationError.ResourceItemReferenceBaseHRefAndSubject);

            Console.WriteLine("Test with non-null href and non-empty source.");
            extensible.Extensions.Add(new GenericExtension("ext"));
            extensible.Extensions[0].AddChild(new ElementInfo(new XmlNameInfo("name"), new GenericElement()));
            target.HRef = "href";
            this.VerifyValidationException(ValidationError.ResourceItemReferenceBaseHRefAndSubject);
            extensible.Extensions.Clear();

            Console.WriteLine("Test with invalid language.");
            target.Language = "--";
            this.VerifyValidationException(ValidationError.ResourceItemTargetLangInvalid);

            Console.WriteLine("Test with language not matching document target language.");
            target.Language = "fr-zh";
            this.VerifyValidationException(ValidationError.ResourceItemTargetLangMismatch);
        }

        /// <summary>
        /// Tests the validator for <see cref="Segment"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Segment()
        {
            Unit unit;

            Console.WriteLine("Test with null source.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            unit.Resources.Where(c => c is Segment).Cast<Segment>().First().Source = null;
            this.VerifyValidationException(ValidationError.SourceNull);

            Console.WriteLine("Test with substate not like prefix:value.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            unit.Resources.Where(c => c is Segment).Cast<Segment>().First().SubState = "a";
            this.VerifyValidationException(ValidationError.PrefixValueMissingColon);

            Console.WriteLine("Test with state using xlf:value.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            unit.Resources.Where(c => c is Segment).Cast<Segment>().First().SubState = "xlf:value";
            this.VerifyValidationException(ValidationError.PrefixValueInvalid);

            Console.WriteLine("Test with state using prefix:value:value is ok.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            unit.Resources.Where(c => c is Segment).Cast<Segment>().First().SubState = "prefix:value:value";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with duplicate id by ignorable.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            unit.Resources.Where(c => c is Segment).Cast<Segment>().First().Id = "segmentId";
            unit.Resources.Where(c => c is Ignorable).Cast<Ignorable>().First().Id = "segmentId";
            this.VerifyValidationException(ValidationError.ElementIdDuplicate);

            Console.WriteLine("Test with duplicate id by segment.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            foreach (Segment segment in unit.Resources.Where(c => c is Segment).Cast<Segment>())
            {
                segment.Id = "segmentId";
            }

            this.VerifyValidationException(ValidationError.ElementIdDuplicate);
        }

        /// <summary>
        /// Tests the validator for Size and Length Restriction module attribute constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_SizeRestriction_Attributes()
        {
            this.DeserializeDocument();
            this._document.Files[0].SizeInfo = "sizeInfo";
            this._document.Files[0].SizeInfoReference = "sizeInfoRef";
            Console.WriteLine("Test with sizeInfo and sizeInfoRef.");
            this.VerifyValidationException(ValidationError.SameSizeInfoAndSizeInfoReferencePresence);

            Console.WriteLine("Test with only sizeInfo.");
            this._document.Files[0].SizeInfo = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with only sizeInfoRef.");
            this._document.Files[0].SizeInfo = "sizeInfo";
            this._document.Files[0].SizeInfoReference = null;
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="ProfileData"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_SizeRestriction_ProfileData()
        {
            ProfileData data;

            data = new ProfileData();

            this.DeserializeDocument();
            this._document.Files[0].ProfileData = data;

            Console.WriteLine("Test with null profile.");
            data.Profile = null;
            this.VerifyValidationException(ValidationError.ProfileDataProfileNull);

            Console.WriteLine("Test with valid data.");
            data.Profile = "profile";
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="Skeleton"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Skeleton()
        {
            Console.WriteLine("Test with href and body.");
            this.DeserializeDocument();
            this._document.Files[0].Skeleton.HRef = "href";
            this._document.Files[0].Skeleton.NonTranslatableText = "text";
            this.VerifyValidationException(ValidationError.SkeletonHasHRefAndTextOrNeither);

            Console.WriteLine("Test with empty href and body.");
            this.DeserializeDocument();
            this._document.Files[0].Skeleton.HRef = String.Empty;
            this._document.Files[0].Skeleton.NonTranslatableText = "text";
            this.VerifyValidationException(ValidationError.SkeletonHRefEmpty);

            Console.WriteLine("Test with href and empty body.");
            this.DeserializeDocument();
            this._document.Files[0].Skeleton.HRef = "href";
            this._document.Files[0].Skeleton.NonTranslatableText = String.Empty;
            this.VerifyValidationException(ValidationError.SkeletonHasHRefAndTextOrNeither);

            Console.WriteLine("Test with empty href and empty body.");
            this.DeserializeDocument();
            this._document.Files[0].Skeleton.HRef = String.Empty;
            this._document.Files[0].Skeleton.NonTranslatableText = String.Empty;
            this.VerifyValidationException(ValidationError.SkeletonHRefEmpty);

            Console.WriteLine("Test with null href and null body.");
            this.DeserializeDocument();
            this._document.Files[0].Skeleton.HRef = null;
            this._document.Files[0].Skeleton.NonTranslatableText = null;
            this.VerifyValidationException(ValidationError.SkeletonHasHRefAndTextOrNeither);
        }

        /// <summary>
        /// Tests the validator for <see cref="SpanningCode"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_SpanningCode()
        {
            SpanningCode span;
            Segment segment;
            Unit unit;

            span = new SpanningCode();

            Console.WriteLine("Test with null Id.");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            this.VerifyValidationException(ValidationError.CodeBaseIdNull);

            Console.WriteLine("Test with empty Id.");
            span.Id = String.Empty;
            this.VerifyValidationException(ValidationError.CodeBaseIdNull);

            Console.WriteLine("Test with duplicate Id.");
            segment.Id = "duplicateId";
            span.Id = segment.Id;
            this.VerifyValidationException(ValidationError.ElementIdDuplicate);

            Console.WriteLine("Test with item on target not matching source.");
            span.Id = "newSpanId";
            segment.Target.Text.Add(new MarkedSpan("bogus"));
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with DataReferenceEnd matching data.");
            span = new SpanningCode("spanId");
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.DataReferenceEnd = "dataId";
            span.DataReferenceStart = "dataId";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with DataReferenceEnd not matching data.");
            span.DataReferenceEnd = "bogus";
            this.VerifyValidationException(ValidationError.SpanningCodeInvalidDataRefEnd);

            Console.WriteLine("Test with DataReferenceEnd is null.");
            span.DataReferenceEnd = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with DataReferenceEnd is empty.");
            span.DataReferenceEnd = String.Empty;
            this.VerifyValidationException(ValidationError.SpanningCodeInvalidDataRefEnd);

            span.DataReferenceEnd = "dataId";

            Console.WriteLine("Test with DataReferenceStart not matching data.");
            span.DataReferenceStart = "bogus";
            this.VerifyValidationException(ValidationError.SpanningCodeInvalidDataRefStart);

            Console.WriteLine("Test with DataReferenceStart is null.");
            span.DataReferenceStart = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with DataReferenceStart is empty.");
            span.DataReferenceStart = String.Empty;
            this.VerifyValidationException(ValidationError.SpanningCodeInvalidDataRefStart);

            Console.WriteLine("Test with SubFlowsEnd is null.");
            span = new SpanningCode("spanId");
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.DataReferenceEnd = "dataId";
            span.DataReferenceStart = "dataId";
            span.SubFlowsEnd = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlowsEnd is empty.");
            span.SubFlowsEnd = String.Empty;
            this.VerifyValidationException(ValidationError.SpanningCodeInvalidSubFlowsEnd);

            Console.WriteLine("Test with SubFlowsEnd matching a unit.");
            span.SubFlowsEnd = "u1";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlowsEnd matching multiple units.");
            span.SubFlowsEnd = "u1 u2 u3";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlowsEnd multiple units with one not matching.");
            span.SubFlowsEnd = "u1 u200 u3";
            this.VerifyValidationException(ValidationError.SpanningCodeInvalidSubFlowsEnd);

            Console.WriteLine("Test with SubFlowsStart is null.");
            span = new SpanningCode("spanId");
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.DataReferenceEnd = "dataId";
            span.DataReferenceStart = "dataId";
            span.SubFlowsStart = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlowsStart is empty.");
            span.SubFlowsStart = String.Empty;
            this.VerifyValidationException(ValidationError.SpanningCodeInvalidSubFlowsStart);

            Console.WriteLine("Test with SubFlowsStart matching a unit.");
            span.SubFlowsStart = "u1";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlowsStart matching multiple units.");
            span.SubFlowsStart = "u1 u2 u3";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlowsStart multiple units with one not matching.");
            span.SubFlowsStart = "u1 u200 u3";
            this.VerifyValidationException(ValidationError.SpanningCodeInvalidSubFlowsStart);

            Console.WriteLine("Test with SubType not like prefix:value.");
            span = new SpanningCode("spanId");
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.DataReferenceEnd = "dataId";
            span.DataReferenceStart = "dataId";
            span.SubType = "a";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubType using xlf:value.");
            span.SubType = "Xlf:value";
            this.VerifyValidationException(ValidationError.CodeBaseSubTypeInvalid);

            Console.WriteLine("Test with SubType using prefix:value:value is ok.");
            span.SubType = "prefix:value:value";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubType set and Type not.");
            span.SubType = "prefix:value";
            span.Type = null;
            this.VerifyValidationException(ValidationError.CodeBaseTypeNotSpecified);

            Console.WriteLine("Test with SubType is null.");
            span.SubType = null;
            span.Type = CodeType.Formatting;
            StandardValidatorTests._validator.Validate(this._document);

            foreach (string name in new string[] { "lb", "pb", "b", "i", "u" })
            {
                Console.WriteLine("Test with SubType set to {0} and Type set to fmt.", name);
                span.SubType = "xlf:" + name;
                span.Type = CodeType.Formatting;
                StandardValidatorTests._validator.Validate(this._document);

                Console.WriteLine("Test with SubType set to {0} and Type not set to fmt.", name);
                span.Type = CodeType.Image;
                this.VerifyValidationException(ValidationError.CodeBaseSubTypeMismatchFormatting);
            }

            Console.WriteLine("Test with SubType set to var and Type set to var.");
            span.SubType = "xlf:var";
            span.Type = CodeType.UserInterface;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubType set to var and Type not set to var.");
            span.Type = CodeType.Image;
            this.VerifyValidationException(ValidationError.CodeBaseSubTypeMismatchUserInterface);
        }

        /// <summary>
        /// Tests the validator for <see cref="SpanningCodeEnd"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_SpanningCodeEnd()
        {
            SpanningCodeEnd span;
            SpanningCodeStart startSpan;
            Segment segment;
            Unit unit;

            span = new SpanningCodeEnd();
            span.Isolated = true;
            span.Id = "ec1";

            Console.WriteLine("Test with null Id.");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with empty Id.");
            span.Id = String.Empty;
            this.VerifyValidationException(ValidationError.SpanningCodeEndIdNull);

            Console.WriteLine("Test with duplicate Id.");
            segment.Id = "duplicateId";
            span.Id = segment.Id;
            this.VerifyValidationException(ValidationError.ElementIdDuplicate);

            Console.WriteLine("Test with startRef and Id.");
            span.Id = "newSpanId";
            span.StartReference = "sc1";
            span.Isolated = false;
            this.VerifyValidationException(ValidationError.SpanningCodeEndStartRefAndIdSpecified);

            Console.WriteLine("Test with item on target not matching source.");
            span.Id = "newSpanId";
            span.Isolated = true;
            span.StartReference = null;
            segment.Target.Text.Add(new MarkedSpan("bogus"));
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with DataReference matching data.");
            span = new SpanningCodeEnd("spanId");
            span.Isolated = true;
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.DataReference = "dataId";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with DataReference not matching data.");
            span.DataReference = "bogus";
            this.VerifyValidationException(ValidationError.SpanningCodeEndInvalidDataRef);

            Console.WriteLine("Test with DataReference is null.");
            span.DataReference = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with DataReference is empty.");
            span.DataReference = String.Empty;
            this.VerifyValidationException(ValidationError.SpanningCodeEndInvalidDataRef);

            span.DataReference = "dataId";

            Console.WriteLine("Test with StartReference matching sc.");
            span = new SpanningCodeEnd("spanId");
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            startSpan = new SpanningCodeStart("scId");
            startSpan.DataReference = "dataId";
            startSpan.Type = CodeType.Formatting;
            segment.Source.Text.Add(startSpan);
            segment.Source.Text.Add(span);
            span.DataReference = "dataId";
            span.Id = null;
            span.StartReference = "scId";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with StartReference not matching sc.");
            span.StartReference = "bogus";
            this.VerifyValidationException(ValidationError.TagStartRefInvalid);

            Console.WriteLine("Test with StartReference is null.");
            startSpan.Isolated = true;
            span.Id = "ec1";
            span.Isolated = true;
            span.StartReference = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with StartReference is empty.");
            span.Id = null;
            span.Isolated = false;
            span.StartReference = String.Empty;
            this.VerifyValidationException(ValidationError.TagStartRefInvalid);

            Console.WriteLine("Test with SubFlows is null.");
            span = new SpanningCodeEnd("spanId");
            span.Isolated = true;
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.DataReference = "dataId";
            span.SubFlows = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlows is empty.");
            span.SubFlows = String.Empty;
            this.VerifyValidationException(ValidationError.SpanningCodeEndSubFlowsInvalid);

            Console.WriteLine("Test with SubFlows matching a unit.");
            span.SubFlows = "u1";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlows matching multiple units.");
            span.SubFlows = "u1 u2 u3";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlows multiple units with one not matching.");
            span.SubFlows = "u1 u200 u3";
            this.VerifyValidationException(ValidationError.SpanningCodeEndSubFlowsInvalid);

            Console.WriteLine("Test with SubType not like prefix:value.");
            span = new SpanningCodeEnd("spanId");
            span.Isolated = true;
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.DataReference = "dataId";
            span.SubType = "a";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubType using xlf:value.");
            span.SubType = "Xlf:value";
            this.VerifyValidationException(ValidationError.CodeBaseSubTypeInvalid);

            Console.WriteLine("Test with SubType using prefix:value:value is ok.");
            span.SubType = "prefix:value:value";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubType set and Type not.");
            span.SubType = "prefix:value";
            span.Type = null;
            this.VerifyValidationException(ValidationError.CodeBaseTypeNotSpecified);

            Console.WriteLine("Test with SubType is null.");
            span.SubType = null;
            span.Type = CodeType.Formatting;
            StandardValidatorTests._validator.Validate(this._document);

            foreach (string name in new string[] { "lb", "pb", "b", "i", "u" })
            {
                Console.WriteLine("Test with SubType set to {0} and Type set to fmt.", name);
                span.SubType = "xlf:" + name;
                span.Type = CodeType.Formatting;
                StandardValidatorTests._validator.Validate(this._document);

                Console.WriteLine("Test with SubType set to {0} and Type not set to fmt.", name);
                span.Type = CodeType.Image;
                this.VerifyValidationException(ValidationError.CodeBaseSubTypeMismatchFormatting);
            }

            Console.WriteLine("Test with SubType set to var and Type set to var.");
            span.SubType = "xlf:var";
            span.Type = CodeType.UserInterface;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubType set to var and Type not set to var.");
            span.Type = CodeType.Image;
            this.VerifyValidationException(ValidationError.CodeBaseSubTypeMismatchUserInterface);
        }

        /// <summary>
        /// Tests the validator for <see cref="SpanningCodeStart"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_SpanningCodeStart()
        {
            // canOverlap defaults to yes

            SpanningCodeStart span;
            Segment segment;
            Unit unit;

            span = new SpanningCodeStart();

            Console.WriteLine("Test with null Id.");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            this.VerifyValidationException(ValidationError.CodeBaseIdNull);

            Console.WriteLine("Test with empty Id.");
            span.Id = String.Empty;
            this.VerifyValidationException(ValidationError.CodeBaseIdNull);

            Console.WriteLine("Test with duplicate Id.");
            segment.Id = "duplicateId";
            span.Id = segment.Id;
            this.VerifyValidationException(ValidationError.ElementIdDuplicate);

            Console.WriteLine("Test with item on target not matching source.");
            span.Id = "newSpanId";
            span.Isolated = true;
            segment.Target.Text.Add(new MarkedSpan("bogus"));
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with DataReference matching data.");
            span = new SpanningCodeStart("spanId");
            span.Isolated = true;
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.DataReference = "dataId";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with DataReference not matching data.");
            span.DataReference = "bogus";
            this.VerifyValidationException(ValidationError.SpanningCodeStartDataRefInvalid);

            Console.WriteLine("Test with DataReference is null.");
            span.DataReference = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with DataReferenceEnd is empty.");
            span.DataReference = String.Empty;
            this.VerifyValidationException(ValidationError.SpanningCodeStartDataRefInvalid);

            span.DataReference = "dataId";

            Console.WriteLine("Test with SubFlows is null.");
            span = new SpanningCodeStart("spanId");
            span.Isolated = true;
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.DataReference = "dataId";
            span.SubFlows = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlows is empty.");
            span.SubFlows = String.Empty;
            this.VerifyValidationException(ValidationError.SpanningCodeStartSubflowsInvalid);

            Console.WriteLine("Test with SubFlows matching a unit.");
            span.SubFlows = "u1";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlows matching multiple units.");
            span.SubFlows = "u1 u2 u3";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlows multiple units with one not matching.");
            span.SubFlows = "u1 u200 u3";
            this.VerifyValidationException(ValidationError.SpanningCodeStartSubflowsInvalid);

            Console.WriteLine("Test with SubType not like prefix:value.");
            span = new SpanningCodeStart("spanId");
            span.Isolated = true;
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.DataReference = "dataId";
            span.SubType = "a";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubType using xlf:value.");
            span.SubType = "Xlf:value";
            this.VerifyValidationException(ValidationError.CodeBaseSubTypeInvalid);

            Console.WriteLine("Test with SubType using prefix:value:value is ok.");
            span.SubType = "prefix:value:value";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubType set and Type not.");
            span.SubType = "prefix:value";
            span.Type = null;
            this.VerifyValidationException(ValidationError.CodeBaseTypeNotSpecified);

            Console.WriteLine("Test with SubType is null.");
            span.SubType = null;
            span.Type = CodeType.Formatting;
            StandardValidatorTests._validator.Validate(this._document);

            foreach (string name in new string[] { "lb", "pb", "b", "i", "u" })
            {
                Console.WriteLine("Test with SubType set to {0} and Type set to fmt.", name);
                span.SubType = "xlf:" + name;
                span.Type = CodeType.Formatting;
                StandardValidatorTests._validator.Validate(this._document);

                Console.WriteLine("Test with SubType set to {0} and Type not set to fmt.", name);
                span.Type = CodeType.Image;
                this.VerifyValidationException(ValidationError.CodeBaseSubTypeMismatchFormatting);
            }

            Console.WriteLine("Test with SubType set to var and Type set to var.");
            span.SubType = "xlf:var";
            span.Type = CodeType.UserInterface;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubType set to var and Type not set to var.");
            span.Type = CodeType.Image;
            this.VerifyValidationException(ValidationError.CodeBaseSubTypeMismatchUserInterface);

            Console.WriteLine("Test with Type is null.");
            span.SubType = null;
            span.SubType = null;
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="StandaloneCode"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_StandaloneCode()
        {
            StandaloneCode span;
            Segment segment;
            Unit unit;

            span = new StandaloneCode();

            Console.WriteLine("Test with null Id.");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            this.VerifyValidationException(ValidationError.CodeBaseIdNull);

            Console.WriteLine("Test with empty Id.");
            span.Id = String.Empty;
            this.VerifyValidationException(ValidationError.CodeBaseIdNull);

            Console.WriteLine("Test with duplicate Id.");
            segment.Id = "duplicateId";
            span.Id = segment.Id;
            this.VerifyValidationException(ValidationError.ElementIdDuplicate);

            Console.WriteLine("Test with item on target not matching source.");
            span.Id = "newSpanId";
            segment.Target.Text.Add(new MarkedSpan("bogus"));
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with DataReference matching data.");
            span = new StandaloneCode("spanId");
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.DataReference = "dataId";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with DataReference not matching data.");
            span.DataReference = "bogus";
            this.VerifyValidationException(ValidationError.StandaloneCodeDataRefInvalid);

            Console.WriteLine("Test with DataReference is null.");
            span.DataReference = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with DataReference is empty.");
            span.DataReference = String.Empty;
            this.VerifyValidationException(ValidationError.StandaloneCodeDataRefInvalid);

            span.DataReference = "dataId";

            Console.WriteLine("Test with SubFlows is null.");
            span = new StandaloneCode("spanId");
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.DataReference = "dataId";
            span.SubFlows = null;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlows is empty.");
            span.SubFlows = String.Empty;
            this.VerifyValidationException(ValidationError.StandaloneCodeSubflowsInvalid);

            Console.WriteLine("Test with SubFlows matching a unit.");
            span.SubFlows = "u1";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlows matching multiple units.");
            span.SubFlows = "u1 u2 u3";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubFlows multiple units with one not matching.");
            span.SubFlows = "u1 u200 u3";
            this.VerifyValidationException(ValidationError.StandaloneCodeSubflowsInvalid);

            Console.WriteLine("Test with SubType not like prefix:value.");
            span = new StandaloneCode("spanId");
            span.Type = CodeType.Formatting;
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("dataId", "text");
            segment = (Segment)unit.Resources.First(r => r is Segment);
            segment.Source.Text.Add(span);
            span.DataReference = "dataId";
            span.SubType = "a";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubType using xlf:value.");
            span.SubType = "Xlf:value";
            this.VerifyValidationException(ValidationError.CodeBaseSubTypeInvalid);

            Console.WriteLine("Test with SubType using prefix:value:value is ok.");
            span.SubType = "prefix:value:value";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubType set and Type not.");
            span.SubType = "prefix:value";
            span.Type = null;
            this.VerifyValidationException(ValidationError.CodeBaseTypeNotSpecified);

            Console.WriteLine("Test with SubType is null.");
            span.SubType = null;
            span.Type = CodeType.Formatting;
            StandardValidatorTests._validator.Validate(this._document);

            foreach (string name in new string[] { "lb", "pb", "b", "i", "u" })
            {
                Console.WriteLine("Test with SubType set to {0} and Type set to fmt.", name);
                span.SubType = "xlf:" + name;
                span.Type = CodeType.Formatting;
                StandardValidatorTests._validator.Validate(this._document);

                Console.WriteLine("Test with SubType set to {0} and Type not set to fmt.", name);
                span.Type = CodeType.Image;
                this.VerifyValidationException(ValidationError.CodeBaseSubTypeMismatchFormatting);
            }

            Console.WriteLine("Test with SubType set to var and Type set to var.");
            span.SubType = "xlf:var";
            span.Type = CodeType.UserInterface;
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SubType set to var and Type not set to var.");
            span.Type = CodeType.Image;
            this.VerifyValidationException(ValidationError.CodeBaseSubTypeMismatchUserInterface);

            Console.WriteLine("Test with Type is null.");
            span.SubType = null;
            span.SubType = null;
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="Target"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Target()
        {
            Segment segment1;
            Segment segment2;
            Unit unit;

            Console.WriteLine("Test with order == 0.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment1 = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment1.Target.Order = 0;
            this.VerifyValidationException(ValidationError.TargetOrderInvalid);

            Console.WriteLine("Test with order > N.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment1 = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment1.Target.Order = (uint)unit.Resources.Where(c => c is ContainerResource).Count() + 1;
            this.VerifyValidationException(ValidationError.TargetOrderInvalid);

            Console.WriteLine("Test with order == N is ok.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment1 = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment1.Target.Order = (uint)unit.Resources.Where(c => c is ContainerResource).Count();
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with duplicate order.");
            this.DeserializeDocument();
            unit = this._document.Files[0].Containers.Where(c => c is Unit).First() as Unit;
            segment1 = unit.Resources.Where(c => c is Segment).Cast<Segment>().First();
            segment2 = unit.Resources.Where(c => (c is Segment) && (c != segment1)).Cast<Segment>().First();
            segment1.Target.Order = 1;
            segment2.Target.Order = segment1.Target.Order;
            Assert.AreNotEqual(segment1, segment2, "Segments should not be the same.");
            this.VerifyValidationException(ValidationError.TargetOrderDuplicate);
        }

        /// <summary>
        /// Tests that the validator verifies that inline tags with the same Id are the same or equivalent between the
        /// source and target.
        /// </summary>
        [TestCategory(TestUtilities.UnitTestCategory)]
        [TestMethod]
        public void StandardValidator_TargetAndSoureTypesMatch()
        {
            MarkedSpan host;
            Segment segment;
            Unit unit;

            Console.WriteLine("Test with MarkedSpan types with same Id.");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            host = new MarkedSpan("hostSource");
            segment.Source.Text.Add(host);
            host.Text.Add(new MarkedSpan("testId"));
            host = new MarkedSpan("hostTarget");
            segment.Target.Text.Add(host);
            host.Text.Add(new MarkedSpan("testId"));
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with MarkedSpanStart types with same Id.");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            host = new MarkedSpan("hostSource");
            segment.Source.Text.Add(host);
            host.Text.Add(new MarkedSpanStart("testId"));
            host = new MarkedSpan("hostTarget");
            segment.Target.Text.Add(host);
            host.Text.Add(new MarkedSpan("testId"));
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SpanningCode types with same Id.");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            host = new MarkedSpan("hostSource");
            segment.Source.Text.Add(host);
            host.Text.Add(new SpanningCode("testId"));
            host = new MarkedSpan("hostTarget");
            segment.Target.Text.Add(host);
            host.Text.Add(new SpanningCode("testId"));
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with SpanningCodeStart types with same Id.");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            host = new MarkedSpan("hostSource");
            segment.Source.Text.Add(host);
            host.Text.Add(new SpanningCodeStart("testId") { Isolated = false });
            host.Text.Add(new SpanningCodeEnd() { StartReference = "testId" });
            host = new MarkedSpan("hostTarget");
            segment.Target.Text.Add(host);
            host.Text.Add(new SpanningCode("testId"));
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with StandaloneCode types with same Id.");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            host = new MarkedSpan("hostSource");
            segment.Source.Text.Add(host);
            host.Text.Add(new StandaloneCode("testId"));
            host = new MarkedSpan("hostTarget");
            segment.Target.Text.Add(host);
            host.Text.Add(new StandaloneCode("testId"));
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with different types (MarkedSpan, SpanningCode) with same Id.");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            host = new MarkedSpan("hostSource");
            segment.Source.Text.Add(host);
            host.Text.Add(new MarkedSpan("testId"));
            host = new MarkedSpan("hostTarget");
            segment.Target.Text.Add(host);
            host.Text.Add(new SpanningCode("testId"));
            this.VerifyValidationException(ValidationError.ContainerResourceTypesWithSameIdMismatch);

            Console.WriteLine("Test with different types (SpanningCode, StandaloneCode) with same Id.");
            this.DeserializeDocument();
            unit = (Unit)this._document.Files[0].Containers.First(c => c is Unit);
            segment = (Segment)unit.Resources.First(r => r is Segment);
            host = new MarkedSpan("hostSource");
            segment.Source.Text.Add(host);
            host.Text.Add(new SpanningCode("testId"));
            host = new MarkedSpan("hostTarget");
            segment.Target.Text.Add(host);
            host.Text.Add(new StandaloneCode("testId"));
            this.VerifyValidationException(ValidationError.ContainerResourceTypesWithSameIdMismatch);
        }

        /// <summary>
        /// Tests the validator for <see cref="Unit"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Unit()
        {
            uint numRemoved;

            Console.WriteLine("Test with null id.");
            this.DeserializeDocument();
            this._document.Files[0].Containers.First(c => c is Unit).Id = null;
            this.VerifyValidationException(ValidationError.TranslationContainerIdNull);

            Console.WriteLine("Test with empty id.");
            this.DeserializeDocument();
            this._document.Files[0].Containers.First(c => c is Unit).Id = String.Empty;
            this.VerifyValidationException(ValidationError.TranslationContainerIdNull);

            Console.WriteLine("Test with duplicate id.");
            this.DeserializeDocument();
            foreach (Unit unit in this._document.CollapseChildren<Unit>())
            {
                unit.Id = "unit";
            }

            this.VerifyValidationException(ValidationError.ElementIdDuplicate);

            Console.WriteLine("Test with type not like prefix:value.");
            this.DeserializeDocument();
            this._document.Files[0].Containers.First(c => c is Unit).Type = "a";
            this.VerifyValidationException(ValidationError.PrefixValueMissingColon);

            Console.WriteLine("Test with type using xlf:value.");
            this.DeserializeDocument();
            this._document.Files[0].Containers.First(c => c is Unit).Type = "Xlf:value";
            this.VerifyValidationException(ValidationError.PrefixValueInvalid);

            Console.WriteLine("Test with type using prefix:value:value is ok.");
            this.DeserializeDocument();
            this._document.Files[0].Containers.First(c => c is Unit).Type = "prefix:value:value";
            StandardValidatorTests._validator.Validate(this._document);

            Console.WriteLine("Test with no ignorables or segments.");
            this.DeserializeDocument();
            this._document.Files[0].Containers.Where(c => c is Unit).Cast<Unit>().First().Resources.Clear();
            this.VerifyValidationException(ValidationError.UnitMissingResource);

            Console.WriteLine("Test with no segments.");
            this.DeserializeDocument();
            foreach (Unit unit in this._document.Files[0].Containers.Where(c => c is Unit).Cast<Unit>())
            {
                for (int i = unit.Resources.Count - 1; i >= 0; i--)
                {
                    if (unit.Resources[i] is Segment)
                    {
                        unit.Resources.RemoveAt(i);
                    }
                }
            }

            this.VerifyValidationException(ValidationError.UnitMissingSegment);

            Console.WriteLine("Test with no ignorables is ok.");
            numRemoved = 0;
            this.DeserializeDocument();
            foreach (Unit unit in this._document.Files[0].Containers.Where(c => c is Unit).Cast<Unit>())
            {
                for (int i = unit.Resources.Count - 1; i >= 0; i--)
                {
                    if (unit.Resources[i] is Ignorable)
                    {
                        unit.Resources.RemoveAt(i);
                        numRemoved++;
                    }
                }
            }

            // Account for the missing targets.
            foreach (Unit unit in this._document.Files[0].Containers.Where(c => c is Unit).Cast<Unit>())
            {
                foreach (Target target in unit.CollapseChildren<Target>())
                {
                    if (target.Order.HasValue)
                    {
                        target.Order -= numRemoved;
                    }
                }
            }

            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Tests the validator for <see cref="Rule"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Validation_Rule()
        {
            Rule rule;
            Validation validation;

            rule = new Rule();
            validation = new Validation();
            validation.Rules.Add(rule);

            this.DeserializeDocument();
            this._document.Files[0].ValidationRules = validation;

            Console.WriteLine("Test with occurs < 1.");
            rule.Occurs = 0;
            this.VerifyValidationException(ValidationError.RuleInvalidOccurs);
            rule.Occurs = 1;

            Console.WriteLine("Test with no isPresent, isNotPresent, startsWtih, endsWith, or custom rule.");
            rule.ExistsInSource = false;
            rule.IsPresent = null;
            rule.IsNotPresent = null;
            rule.StartsWith = null;
            rule.EndsWith = null;
            this.VerifyValidationException(ValidationError.RuleInvalidDefinition);
            rule.IsNotPresent = "text";

            Console.WriteLine("Test with existsInSource and no isPresent, startsWtih, or endsWith.");
            rule.ExistsInSource = true;
            this.VerifyValidationException(ValidationError.RuleInvalidExistsInSource);
        }

        /// <summary>
        /// Tests the validator for <see cref="Validation"/> constraints.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_Validation_Validation()
        {
            Rule rule;
            Validation validation;

            validation = new Validation();

            this.DeserializeDocument();
            this._document.Files[0].ValidationRules = validation;

            Console.WriteLine("Test with no rules.");
            validation.Rules.Clear();
            this.VerifyValidationException(ValidationError.ValidationMissingRules);

            Console.WriteLine("Test with valid data.");
            rule = new Rule();
            rule.IsNotPresent = "text";
            validation.Rules.Add(rule);
            StandardValidatorTests._validator.Validate(this._document);
        }

        /// <summary>
        /// Verify the test document is valid because this is used by other tests and is assumed to valid.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void StandardValidator_ValidDocument()
        {
            this.DeserializeDocument();
            StandardValidatorTests._validator.Validate(this._document);
        }
        #endregion Test Methods

        #region Helper Methods
        /// <summary>
        /// Creates a generic <see cref="MetadataContainer"/> with metadata within it.
        /// </summary>
        /// <param name="nullGroupId">True indicates that the <see cref="MetadataGroup"/> should be created with a
        /// null Id. False indicates that the <see cref="MetadataGroup"/> should have a valid Id.</param>
        /// <returns></returns>
        private MetadataContainer CreateMetadataContainer(bool nullGroupId)
        {
            Meta meta;
            MetadataContainer container;

            container = new MetadataContainer();
            container.Id = "m1";
            container.Groups.Add(new MetaGroup());
            if (!nullGroupId)
            {
                container.Groups[0].Id = "g1";
            }
            meta = new Meta();
            meta.Type = "type";
            container.Groups[0].Containers.Add(meta);

            return container;
        }

        /// <summary>
        /// Deserializes the document stored in the internal stream and stores the result in the internal document.
        /// </summary>
        private void DeserializeDocument()
        {
            StandardValidatorTests._stream.Seek(0, SeekOrigin.Begin);
            this._document = StandardValidatorTests._reader.Deserialize(StandardValidatorTests._stream);
        }

        /// <summary>
        /// Invokes the validator and catches a <see cref="ValidationException"/> as a valid pass for the test.
        /// If the exception isn't thrown then a failure is raised.
        /// </summary>
        /// <param name="error">The expected error number.</param>
        private void VerifyValidationException(int error)
        {
            try
            {
                StandardValidatorTests._validator.Validate(this._document);
                Assert.Fail("Expected ValidationException to be thrown.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual((int)error, e.ErrorNumber, "Error number is incorrect.");
                Console.WriteLine(e);
            }
        }
        #endregion Helper Methods
    }
}
