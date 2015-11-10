namespace Localization.Xliff.OM.Tests.Attributes
{
    using System;
    using Localization.Xliff.OM.Attributes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="SchemaEntityAttribute"/> class.
    /// </summary>
    [TestClass()]
    public class SchemaEntityAttributeTests
    {
        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void SchemaEntityAttribute_Ctor()
        {
            SchemaEntityAttribute instance;

            //
            // Test with null name.
            //

            try
            {
                instance = new SchemaEntityAttribute(null, Requirement.Optional);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test with empty name.
            //

            try
            {
                instance = new SchemaEntityAttribute(String.Empty, Requirement.Optional);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test with whitespace name.
            //

            try
            {
                instance = new SchemaEntityAttribute("\t\n ", Requirement.Optional);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test with string.
            //

            instance = new SchemaEntityAttribute("name", Requirement.Required);
            Assert.AreEqual(new XmlNameInfo(null, null, "name"), instance.Name, "Name was not set properly.");
            Assert.AreEqual(Requirement.Required, instance.Requirement, "Requirement was not set properly.");
        }
    }
}
