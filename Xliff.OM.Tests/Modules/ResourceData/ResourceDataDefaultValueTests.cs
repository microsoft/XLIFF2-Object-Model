namespace Localization.Xliff.OM.Modules.ResourceData.Tests
{
    using Localization.Xliff.OM.Modules.ResourceData;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the default values for all ResourceData module classes.
    /// </summary>
    [TestClass()]
    public class ResourceDataDefaultValueTests
    {
        #region Test Methods
        /// <summary>
        /// Tests that defaults for <see cref="Reference"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ResourceDataDefaultValue_Reference()
        {
            Reference reference;

            reference = new Reference();
            Assert.IsNull(reference.HRef, "HRef is incorrect.");
            Assert.IsNull(reference.Language, "Language is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="ResourceData"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ResourceDataDefaultValue_ResourceData()
        {
            ResourceData resource;

            resource = new ResourceData();
            Assert.AreEqual(0, resource.ResourceItemReferences.Count, "ResourceItemReferences count is incorrect.");
            Assert.AreEqual(0, resource.ResourceItems.Count, "ResourceItems count is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="ResourceItem"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ResourceDataDefaultValue_ResourceItem()
        {
            ResourceItem resource;

            resource = new ResourceItem();
            Assert.IsTrue(resource.Context, "Context is incorrect.");
            Assert.IsNull(resource.Id, "Id is incorrect.");
            Assert.IsNull(resource.MimeType, "MimeType is incorrect.");
            Assert.AreEqual(0, resource.References.Count, "References count is incorrect.");
            Assert.IsNull(resource.Source, "Source is incorrect.");
            Assert.IsNull(resource.Target, "Target is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="ResourceItemRef"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ResourceDataDefaultValue_ResourceItemRef()
        {
            ResourceItemRef resource;

            resource = new ResourceItemRef();
            Assert.IsNull(resource.Id, "Id is incorrect.");
            Assert.IsNull(resource.Reference, "Reference is incorrect.");
        }
        
        /// <summary>
        /// Tests that defaults for <see cref="ResourceItemSource"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ResourceDataDefaultValue_ResourceItemSource()
        {
            ResourceItemSource resource;

            resource = new ResourceItemSource();
            Assert.IsNull(resource.HRef, "HRef is incorrect.");
            Assert.IsNull(resource.Language, "Language is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="ResourceItemTarget"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ResourceDataDefaultValue_ResourceItemTarget()
        {
            ResourceItemTarget resource;

            resource = new ResourceItemTarget();
            Assert.IsNull(resource.HRef, "HRef is incorrect.");
            Assert.IsNull(resource.Language, "Language is incorrect.");
        }
        #endregion Test Methods
    }
}
