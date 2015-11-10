namespace Localization.Xliff.OM.Converters.Tests
{
    using System;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="HexConverter"/> class.
    /// </summary>
    [TestClass()]
    public class HexConverterTests
    {
        /// <summary>
        /// Test the Convert method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void HexConverter_Convert()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new HexConverter();
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
            // Test with 0.
            //

            actualValue = converter.Convert(0);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("0000", actualValue, "Converted value is incorrect.");

            //
            // Test with negative value.
            //

            actualValue = converter.Convert(-1);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("FFFFFFFF", actualValue, "Converted value is incorrect.");

            //
            // Test with positive value.
            //

            actualValue = converter.Convert(1024);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("0400", actualValue, "Converted value is incorrect.");
        }

        /// <summary>
        /// Test the ConvertBack method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void HexConverter_ConvertBack()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new HexConverter();
            expectedType = typeof(int);

            //
            // Test with null.
            //

            try
            {
                converter.ConvertBack(null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test with incorrect value.
            //

            try
            {
                converter.ConvertBack("true");
                Assert.Fail("Expected FormatException to be thrown.");
            }
            catch (FormatException)
            {
            }

            try
            {
                converter.ConvertBack("FFFFFFFFFFFFF");
                Assert.Fail("Expected OverflowException to be thrown.");
            }
            catch (OverflowException)
            {
            }

            //
            // Test with 0.
            //

            actualValue = converter.ConvertBack("0");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(0, actualValue, "Converted value is incorrect.");

            //
            // Test with lower case value.
            //

            actualValue = converter.ConvertBack("abc");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(0xABC, actualValue, "Converted value is incorrect.");

            //
            // Test with upper case value.
            //

            actualValue = converter.ConvertBack("F00D");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(0xF00D, actualValue, "Converted value is incorrect.");
        }
    }
}
