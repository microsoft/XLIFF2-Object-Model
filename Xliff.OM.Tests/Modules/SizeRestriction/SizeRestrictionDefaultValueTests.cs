namespace Localization.Xliff.OM.Modules.SizeRestriction.Tests
{
    using Localization.Xliff.OM.Modules.SizeRestriction;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the default values for all Size and Length Restriction module classes.
    /// </summary>
    [TestClass()]
    public class SizeRestrictionDefaultValueTests
    {
        #region Test Methods
        /// <summary>
        /// Tests that defaults for <see cref="Normalization"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void SizeRestrictionDefaultValue_Normalization()
        {
            Normalization normalization;

            normalization = new Normalization();
            Assert.AreEqual(NormalizationValue.None, normalization.General, "General is incorrect.");
            Assert.AreEqual(NormalizationValue.None, normalization.Storage, "Storageis incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="ProfileData"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void SizeRestrictionDefaultValue_ProfileData()
        {
            ProfileData data;

            data = new ProfileData();
            Assert.IsNull(data.Profile, "Profile is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Profiles"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void SizeRestrictionDefaultValue_Profiles()
        {
            Profiles profiles;

            profiles = new Profiles();
            Assert.AreEqual(string.Empty, profiles.GeneralProfile, "GeneralProfile is incorrect.");
            Assert.IsNull(profiles.Normalization, "Normalization is incorrect.");
            Assert.AreEqual(string.Empty, profiles.StorageProfile, "StorageProfile is incorrect.");
        }
        #endregion Test Methods
    }
}
