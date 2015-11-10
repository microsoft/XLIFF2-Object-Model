namespace Localization.Xliff.OM.Modules.ChangeTracking.Tests
{
    using Localization.Xliff.OM.Modules.ChangeTracking;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the default values for all Change Tracking module classes.
    /// </summary>
    [TestClass()]
    public class ChangeTrackingDefaultValueTests
    {
        #region Test Methods
        /// <summary>
        /// Tests that defaults for <see cref="ChangeTrack"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ChangeTrackingDefaultValue_ChangeTrack()
        {
            ChangeTrack change;

            change = new ChangeTrack();
            Assert.AreEqual(0, change.Revisions.Count, "Revisions count is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Item"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ChangeTrackingDefaultValue_Item()
        {
            Item item;

            item = new Item();
            Assert.IsNull(item.Property, "Property is incorrect.");
            Assert.IsNull(item.Text, "Text is not null.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="RevisionsContainer"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ChangeTrackingDefaultValue_RevisionsContainer()
        {
            RevisionsContainer container;

            container = new RevisionsContainer();
            Assert.IsNull(container.AppliesTo, "AppliesTo is incorrect.");
            Assert.IsNull(container.CurrentVersion, "CurrentVersion is incorrect.");
            Assert.IsNull(container.Reference, "Reference is incorrect.");
            Assert.AreEqual(0, container.Revisions.Count, "Revisions count is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Revision"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ChangeTrackingDefaultValue_Revision()
        {
            Revision revision;

            revision = new Revision();
            Assert.IsNull(revision.Author, "Author is incorrect.");
            Assert.IsNull(revision.ChangeDate, "ChangeDate is incorrect.");
            Assert.AreEqual(0, revision.Items.Count, "Items count is incorrect.");
            Assert.IsNull(revision.Version, "Version is incorrect.");
        }
        #endregion Test Methods
    }
}
