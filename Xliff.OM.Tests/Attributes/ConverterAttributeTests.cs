namespace Localization.Xliff.OM.Tests.Attributes
{
    using System;
    using Localization.Xliff.OM.Attributes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="ConverterAttribute"/> class.
    /// </summary>
    [TestClass()]
    public class ConverterAttributeTests
    {
        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ConverterAttribute_Ctor()
        {
            ConverterAttribute instance;

            //
            // Test with null type.
            //

            try
            {
                instance = new ConverterAttribute(null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test with valid type.
            //

            instance = new ConverterAttribute(typeof(bool));
            Assert.AreEqual(typeof(bool).FullName, instance.TypeName, "Name was not set properly.");
        }
    }
}
