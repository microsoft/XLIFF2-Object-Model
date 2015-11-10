namespace Localization.Xliff.OM.Converters.Tests
{
    using System;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="TranslationStateConverter"/> class.
    /// </summary>
    [TestClass()]
    public class TranslationStateConverterTests
    {
        /// <summary>
        /// Test the Convert method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void TranslationStateConverter_Convert()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new TranslationStateConverter();
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
            // Test with Final.
            //

            actualValue = converter.Convert(TranslationState.Final);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("final", actualValue, "Converted value is incorrect.");

            //
            // Test with Initial.
            //

            actualValue = converter.Convert(TranslationState.Initial);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("initial", actualValue, "Converted value is incorrect.");

            //
            // Test with Reviewed.
            //

            actualValue = converter.Convert(TranslationState.Reviewed);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("reviewed", actualValue, "Converted value is incorrect.");

            //
            // Test with Translated.
            //

            actualValue = converter.Convert(TranslationState.Translated);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("translated", actualValue, "Converted value is incorrect.");
        }

        /// <summary>
        /// Test the ConvertBack method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void TranslationStateConverter_ConvertBack()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new TranslationStateConverter();
            expectedType = typeof(TranslationState);

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
            // Test with final.
            //

            actualValue = converter.ConvertBack("final");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(TranslationState.Final, actualValue, "Converted value is incorrect.");

            //
            // Test with initial.
            //

            actualValue = converter.ConvertBack("initial");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(TranslationState.Initial, actualValue, "Converted value is incorrect.");

            //
            // Test with reviewed.
            //

            actualValue = converter.ConvertBack("reviewed");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(TranslationState.Reviewed, actualValue, "Converted value is incorrect.");

            //
            // Test with translated.
            //

            actualValue = converter.ConvertBack("translated");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(TranslationState.Translated, actualValue, "Converted value is incorrect.");

            //
            // Test with upper case.
            //

            try
            {
                converter.ConvertBack("FINAL");
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (NotSupportedException)
            {
            }
        }
    }
}
