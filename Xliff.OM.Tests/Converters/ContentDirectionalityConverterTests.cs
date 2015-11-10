namespace Localization.Xliff.OM.Converters.Tests
{
    using System;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="ContentDirectionalityConverter"/> class.
    /// </summary>
    [TestClass()]
    public class ContentDirectionalityConverterTests
    {
        /// <summary>
        /// Test the Convert method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ContentDirectionalityConverter_Convert()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new ContentDirectionalityConverter();
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
            // Test with Auto.
            //

            actualValue = converter.Convert(ContentDirectionality.Auto);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("auto", actualValue, "Converted value is incorrect.");

            //
            // Test with LTR.
            //

            actualValue = converter.Convert(ContentDirectionality.LTR);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("ltr", actualValue, "Converted value is incorrect.");

            //
            // Test with RTL.
            //

            actualValue = converter.Convert(ContentDirectionality.RTL);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("rtl", actualValue, "Converted value is incorrect.");
        }

        /// <summary>
        /// Test the ConvertBack method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ContentDirectionalityConverter_ConvertBack()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new ContentDirectionalityConverter();
            expectedType = typeof(ContentDirectionality);

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
            // Test with auto.
            //

            actualValue = converter.ConvertBack("auto");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(ContentDirectionality.Auto, actualValue, "Converted value is incorrect.");

            //
            // Test with ltr.
            //

            actualValue = converter.ConvertBack("ltr");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(ContentDirectionality.LTR, actualValue, "Converted value is incorrect.");

            //
            // Test with rtl.
            //

            actualValue = converter.ConvertBack("rtl");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(ContentDirectionality.RTL, actualValue, "Converted value is incorrect.");

            //
            // Test with upper case.
            //

            try
            {
                converter.ConvertBack("AUTO");
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (NotSupportedException)
            {
            }
        }
    }
}
