namespace Localization.Xliff.OM.Converters.Tests
{
    using System;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="TranslationSubjectConverter"/> class.
    /// </summary>
    [TestClass()]
    public class TranslationSubjectConverterTests
    {
        /// <summary>
        /// Test the Convert method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void TranslationSubjectConverter_Convert()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new TranslationSubjectConverter();
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
            // Test with Source.
            //

            actualValue = converter.Convert(TranslationSubject.Source);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("source", actualValue, "Converted value is incorrect.");

            //
            // Test with Target.
            //

            actualValue = converter.Convert(TranslationSubject.Target);
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual("target", actualValue, "Converted value is incorrect.");
        }

        /// <summary>
        /// Test the ConvertBack method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void TranslationSubjectConverter_ConvertBack()
        {
            IValueConverter converter;
            object actualValue;
            Type expectedType;

            converter = new TranslationSubjectConverter();
            expectedType = typeof(TranslationSubject);

            //
            // Test with empty string.
            //

            try
            {
                converter.ConvertBack(String.Empty);
                Assert.Fail("Expected NotSupportedException to be thrown.");
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
                Assert.Fail("Expected NotSupportedException to be thrown.");
            }
            catch (NotSupportedException)
            {
            }

            //
            // Test with null.
            //

            try
            {
                converter.ConvertBack(null);
                Assert.Fail("Expected NotSupportedException to be thrown.");
            }
            catch (NotSupportedException)
            {
            }

            //
            // Test with source.
            //

            actualValue = converter.ConvertBack("source");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(TranslationSubject.Source, actualValue, "Converted value is incorrect.");

            //
            // Test with target.
            //

            actualValue = converter.ConvertBack("target");
            Assert.IsNotNull(actualValue, "Converted value is null.");
            Assert.AreEqual(expectedType, actualValue.GetType(), "Type of converted value is incorrect.");
            Assert.AreEqual(TranslationSubject.Target, actualValue, "Converted value is incorrect.");

            //
            // Test with upper case.
            //

            try
            {
                converter.ConvertBack("TARGET");
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (NotSupportedException)
            {
            }
        }
    }
}
