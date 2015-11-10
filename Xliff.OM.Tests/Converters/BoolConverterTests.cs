namespace Localization.Xliff.OM.Converters.Tests
{
    using System;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="BoolConverter"/> class.
    /// </summary>
    [TestClass()]
    public class BoolConverterTests
    {
        /// <summary>
        /// Test the Convert method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void BoolConverter_Convert()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new BoolConverter();
            expectedType = typeof(string);

            //
            // Test with null.
            //

            try
            {
                converter.Convert(null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test with incorrect type.
            //

            try
            {
                converter.Convert("true");
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (ArgumentException)
            {
            }

            //
            // Test with true.
            //

            actualValue = converter.Convert(true);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("yes", actualValue, "Converted value is incorrect.");

            //
            // Test with false.
            //

            actualValue = converter.Convert(false);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("no", actualValue, "Converted value is incorrect.");
        }

        /// <summary>
        /// Test the ConvertBack method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void BoolConverter_ConvertBack()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new BoolConverter();
            expectedType = typeof(bool);

            //
            // Test with null.
            //

            try
            {
                converter.ConvertBack(null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (NotSupportedException)
            {
            }

            //
            // Test with incorrect value.
            //

            try
            {
                converter.ConvertBack("true");
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (NotSupportedException)
            {
            }

            //
            // Test with true.
            //

            actualValue = converter.ConvertBack("yes");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(true, actualValue, "Converted value is incorrect.");

            //
            // Test with false.
            //

            actualValue = converter.ConvertBack("no");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(false, actualValue, "Converted value is incorrect.");
        }
    }
}
