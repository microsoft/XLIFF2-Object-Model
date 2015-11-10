namespace Localization.Xliff.OM.Modules.Metadata.Tests
{
    using Localization.Xliff.OM.Modules.Metadata;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the default values for all Metadata module classes.
    /// </summary>
    [TestClass()]
    public class MetadataDefaultValueTests
    {
        #region Test Methods
        /// <summary>
        /// Tests that defaults for <see cref="MetadataContainer"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void MetadataDefaultValue_MetadataContainer()
        {
            MetadataContainer container;

            container = new MetadataContainer();
            Assert.AreEqual(0, container.Groups.Count, "Groups count is incorrect.");
            Assert.AreEqual(false, container.HasGroups, "HasGroups is incorrect.");
            Assert.IsNull(container.Id, "Id is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="MetaGroup"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void MetadataDefaultValue_MetaGroup()
        {
            MetaGroup group;

            group = new MetaGroup();
            Assert.IsNull(group.AppliesTo, "AppliesTo is incorrect.");
            Assert.IsNull(group.Category, "Category is incorrect.");
            Assert.AreEqual(0, group.Containers.Count, "Containers count is incorrect.");
            Assert.IsNull(group.Id, "Id is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Meta"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void MetadataDefaultValue_Meta()
        {
            Meta meta;

            meta = new Meta();
            Assert.IsNull(meta.NonTranslatableText, "NonTranslatableText is incorrect.");
            Assert.IsNull(meta.Type, "Type is incorrect.");

            meta = new Meta("text");
            Assert.AreEqual("text", meta.NonTranslatableText, "NonTranslatableText is incorrect.");
            Assert.IsNull(meta.Type, "Type is incorrect.");

            meta = new Meta("type", "text");
            Assert.AreEqual("text", meta.NonTranslatableText, "NonTranslatableText is incorrect.");
            Assert.AreEqual("type", meta.Type, "Type is incorrect.");
        }
        #endregion Test Methods
    }
}
