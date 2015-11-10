namespace Localization.Xliff.OM.Modules.Validation.Tests
{
    using Localization.Xliff.OM.Modules.Validation;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the default values for all Validation module classes.
    /// </summary>
    [TestClass()]
    public class ValidationDefaultValueTests
    {
        #region Test Methods
        /// <summary>
        /// Tests that defaults for <see cref="Rule"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ValidationDefaultValue_Rule()
        {
            Rule rule;

            rule = new Rule();
            Assert.IsTrue(rule.CaseSensitive, "CaseSensitive is incorrect.");
            Assert.IsFalse(rule.Disabled, "Disabled is incorrect.");
            Assert.IsNull(rule.EndsWith, "EndsWith is incorrect.");
            Assert.IsFalse(rule.ExistsInSource, "ExistsInSource is incorrect.");
            Assert.IsNull(rule.IsNotPresent, "IsNotPresent is incorrect.");
            Assert.IsNull(rule.IsPresent, "IsPresent is incorrect.");
            Assert.AreEqual(NormalizationValue.NFC, rule.Normalization, "Normalization is incorrect.");
            Assert.IsNull(rule.Occurs, "Occurs is incorrect.");
            Assert.IsNull(rule.StartsWith, "StartsWith is incorrect.");
        }

        /// <summary>
        /// Tests that defaults for <see cref="Validation"/> are correct.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ValidationDefaultValue_Validation()
        {
            Validation validation;

            validation = new Validation();
            Assert.AreEqual(0, validation.Rules.Count, "Rule count is incorrect.");
        }
        #endregion Test Methods
    }
}
