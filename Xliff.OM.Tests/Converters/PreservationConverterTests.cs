namespace Localization.Xliff.OM.Converters.Tests
{
    using System;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="PreservationConverter"/> class.
    /// </summary>
    [TestClass()]
    public class PreservationConverterTests
    {
        /// <summary>
        /// Test the Convert method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void PreservationConverter_Convert()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new PreservationConverter();
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
            // Test with Default.
            //

            actualValue = converter.Convert(Preservation.Default);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("default", actualValue, "Converted value is incorrect.");

            //
            // Test with Preserve.
            //

            actualValue = converter.Convert(Preservation.Preserve);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("preserve", actualValue, "Converted value is incorrect.");
        }

        /// <summary>
        /// Test the ConvertBack method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void PreservationConverter_ConvertBack()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new PreservationConverter();
            expectedType = typeof(Preservation);

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
            // Test with default.
            //

            actualValue = converter.ConvertBack("default");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(Preservation.Default, actualValue, "Converted value is incorrect.");

            //
            // Test with preserve.
            //

            actualValue = converter.ConvertBack("preserve");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(Preservation.Preserve, actualValue, "Converted value is incorrect.");

            //
            // Test with upper case.
            //

            try
            {
                converter.ConvertBack("DEFAULT");
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (NotSupportedException)
            {
            }
        }
    }
}
