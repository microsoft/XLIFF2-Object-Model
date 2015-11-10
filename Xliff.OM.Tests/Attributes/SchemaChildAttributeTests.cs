namespace Localization.Xliff.OM.Tests.Attributes
{
    using System;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.XmlNames;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="SchemaChildAttribute"/> class.
    /// </summary>
    [TestClass()]
    public class SchemaChildAttributeTests
    {
        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void SchemaChildAttribute_Ctor()
        {
            SchemaChildAttribute instance;

            //
            // Test with empty name.
            //

            try
            {
                instance = new SchemaChildAttribute(NamespaceValues.Core, String.Empty, typeof(bool));
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
                instance = new SchemaChildAttribute(NamespaceValues.Core, "\t\n  ", typeof(bool));
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test with null type.
            //

            try
            {
                instance = new SchemaChildAttribute(NamespaceValues.Core, "name", null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test with null name.
            //

            instance = new SchemaChildAttribute(NamespaceValues.Core, null, typeof(bool));
            Assert.AreEqual(new XmlNameInfo(null, NamespaceValues.Core, null), instance.Name, "Name was not set properly.");
            Assert.AreEqual(typeof(bool), instance.ChildType, "ChildTypewas not set properly.");

            //
            // Test with valid values.
            //

            instance = new SchemaChildAttribute(NamespaceValues.Core, "name", typeof(bool));
            Assert.AreEqual(new XmlNameInfo(null, NamespaceValues.Core, "name"), instance.Name, "Name was not set properly.");
            Assert.AreEqual(typeof(bool), instance.ChildType, "ChildTypewas not set properly.");
        }
    }
}
