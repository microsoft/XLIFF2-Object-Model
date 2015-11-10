namespace Localization.Xliff.OM.Validators.Tests
{
    using System.Collections.Generic;
    using System.Reflection;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="ValidationErrors"/> class.
    /// </summary>
    [TestClass()]
    public class ValidationErrorTests
    {
        /// <summary>
        /// Tests that fields are sequential and not duplicated.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ValidationErrors_Fields()
        {
            List<int> list;
            int nextValue;
            int numFields;

            list = new List<int>();
            numFields = 0;

            foreach (FieldInfo field in typeof(ValidationError).GetFields(BindingFlags.Static | BindingFlags.NonPublic))
            {
                // Don't store the special values.
                if ((field.Name != "BaseValue") && (field.Name != "LastUsedOffset"))
                {
                    list.Add((int)field.GetValue(null));
                    numFields++;
                }
            }

            Assert.AreNotEqual(0, numFields, "No fields were tested.");
            list.Sort();

            // Test that values are sequential and aren't duplicated.
            nextValue = ValidationError.BaseValue + 1;
            foreach (int value in list)
            {
                Assert.AreEqual(nextValue, value, "Value was not expected.");
                nextValue++;
            }

            Assert.AreEqual(nextValue - 1,
                            ValidationError.BaseValue + ValidationError.LastUsedOffset,
                            "LastUsedOffset is incorrect.");
        }
    }
}
