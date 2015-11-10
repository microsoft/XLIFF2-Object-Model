namespace Localization.Xliff.OM.Converters.Tests
{
    using System;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="UIntConverterTests"/> class.
    /// </summary>
    [TestClass()]
    public class UIntConverterTests
    {
        /// <summary>
        /// Test the Convert method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void UIntConverter_Convert()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new UIntConverter();
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

            actualValue = converter.Convert(0u);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("0", actualValue, "Converted value is incorrect.");

            //
            // Test with negative value.
            //

            try
            {
                actualValue = converter.Convert(-1);
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (ArgumentException)
            {
            }

            //
            // Test with positive value.
            //

            actualValue = converter.Convert(1024u);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("1024", actualValue, "Converted value is incorrect.");
        }

        /// <summary>
        /// Test the ConvertBack method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void UIntConverter_ConvertBack()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new UIntConverter();
            expectedType = typeof(uint);

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
                converter.ConvertBack("999999999999999");
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
            Assert.AreEqual(0u, actualValue, "Converted value is incorrect.");

            //
            // Test with positive value.
            //

            actualValue = converter.ConvertBack("1024");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(1024u, actualValue, "Converted value is incorrect.");

            //
            // Test with negative value.
            //

            try
            {
                actualValue = converter.ConvertBack("-1024");
                Assert.Fail("Expected OverflowException to be thrown.");
            }
            catch (OverflowException)
            {
            }
        }
    }
}
