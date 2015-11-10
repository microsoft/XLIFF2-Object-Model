namespace Localization.Xliff.OM.Core.Tests
{
    using System;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="AttributeData"/> class.
    /// </summary>
    [TestClass()]
    public class AttributeDataTests
    {
        #region Test Methods
        /// <summary>
        /// Test for the constructors.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void AttributeData_Ctor()
        {
            AttributeData data;

            Console.WriteLine("Test with optional value and no default value.");
            data = new AttributeData("test", new XmlNameInfo("name"), true);
            Assert.IsNotNull(data.InheritanceList, "InheritanceList is not null.");
            Assert.AreEqual(0, data.InheritanceList.Count, "InheritanceList count is incorrect.");
            Assert.IsNull(data.ConverterTypeName, "ConverterTypeName is not null.");
            Assert.IsFalse(data.HasValue, "HasValue is incorrect.");
            Assert.IsFalse(data.InheritValue, "InheritValue is incorrect.");
            Assert.IsFalse(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.IsTrue(data.IsOptional, "IsOptional is incorrect.");
            Assert.IsNull(data.Value, "Value is incorrect.");
            Assert.AreEqual("name", data.LocalName, "XmlName is incorrect.");

            Console.WriteLine("Test with required value and no default value.");
            data = new AttributeData("test", new XmlNameInfo("name"), false);
            Assert.IsNotNull(data.InheritanceList, "InheritanceList is not null.");
            Assert.AreEqual(0, data.InheritanceList.Count, "InheritanceList count is incorrect.");
            Assert.IsNull(data.ConverterTypeName, "ConverterTypeName is not null.");
            Assert.IsFalse(data.HasValue, "HasValue is incorrect.");
            Assert.IsFalse(data.InheritValue, "InheritValue is incorrect.");
            Assert.IsFalse(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.IsFalse(data.IsOptional, "IsOptional is incorrect.");
            Assert.IsNull(data.Value, "Value is incorrect.");
            Assert.AreEqual("name", data.LocalName, "XmlName is incorrect.");

            Console.WriteLine("Test with optional value and default value.");
            data = new AttributeData("test", new XmlNameInfo("name"), true, 1);
            Assert.IsNotNull(data.InheritanceList, "InheritanceList is not null.");
            Assert.AreEqual(0, data.InheritanceList.Count, "InheritanceList count is incorrect.");
            Assert.IsNull(data.ConverterTypeName, "ConverterTypeName is not null.");
            Assert.IsFalse(data.HasValue, "HasValue is incorrect.");
            Assert.IsFalse(data.InheritValue, "InheritValue is incorrect.");
            Assert.IsTrue(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.IsTrue(data.IsOptional, "IsOptional is incorrect.");
            Assert.AreEqual(1, data.Value, "Value is incorrect.");
            Assert.AreEqual("name", data.LocalName, "XmlName is incorrect.");

            Console.WriteLine("Test with required value and default value.");
            data = new AttributeData("test", new XmlNameInfo("name"), false, 1);
            Assert.IsNotNull(data.InheritanceList, "InheritanceList is not null.");
            Assert.AreEqual(0, data.InheritanceList.Count, "InheritanceList count is incorrect.");
            Assert.IsNull(data.ConverterTypeName, "ConverterTypeName is not null.");
            Assert.IsTrue(data.HasValue, "HasValue is incorrect.");
            Assert.IsFalse(data.InheritValue, "InheritValue is incorrect.");
            Assert.IsTrue(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.IsFalse(data.IsOptional, "IsOptional is incorrect.");
            Assert.AreEqual(1, data.Value, "Value is incorrect.");
            Assert.AreEqual("name", data.LocalName, "XmlName is incorrect.");
        }

        /// <summary>
        /// Test for GetStringValue method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void AttributeData_GetStringValue()
        {
            AttributeData data;

            data = new AttributeData("test", new XmlNameInfo("name"), true);

            Console.WriteLine("Test with null value.");
            Assert.IsNull(data.GetStringValue(), "Value is not null.");

            Console.WriteLine("Test with string value.");
            data.Value = "string";
            Assert.AreEqual("string", data.GetStringValue(), "Value is incorrect.");

            Console.WriteLine("Test with int value.");
            data.Value = 1;
            Assert.AreEqual("1", data.GetStringValue(), "Value is incorrect.");

            Console.WriteLine("Test with converter.");
            data.ConverterTypeName = typeof(HexConverter).FullName;
            data.Value = 10;
            Assert.AreEqual("000A", data.GetStringValue(), "Value is incorrect.");
        }

        /// <summary>
        /// Test for SetValue method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void AttributeData_SetValue()
        {
            AttributeData data;

            Console.WriteLine("Test with no converter.");
            data = new AttributeData("test", new XmlNameInfo("name"), true);
            data.SetValue("string");
            Assert.IsTrue(data.HasValue, "HasValue is incorrect.");
            Assert.AreEqual("string", data.Value, "Value is incorrect.");

            Console.WriteLine("Test with converter.");
            data = new AttributeData("test", new XmlNameInfo("name"), true);
            data.ConverterTypeName = typeof(HexConverter).FullName;
            data.SetValue("A");
            Assert.IsTrue(data.HasValue, "HasValue is incorrect.");
            Assert.AreEqual(10, data.Value, "Value is incorrect.");
        }

        /// <summary>
        /// Test for Value property.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void AttributeData_Value()
        {
            AttributeData data;

            Console.WriteLine("Test with no default value.");
            data = new AttributeData("test", new XmlNameInfo("name"), true);
            data.Value = 1;
            Assert.IsTrue(data.HasValue, "HasValue is incorrect.");
            Assert.IsFalse(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.AreEqual(1, data.Value, "Value is incorrect.");

            Console.WriteLine("Test with default value.");
            data = new AttributeData("test", new XmlNameInfo("name"), true, "default");
            data.Value = 1;
            Assert.IsTrue(data.HasValue, "HasValue is incorrect.");
            Assert.IsFalse(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.AreEqual(1, data.Value, "Value is incorrect.");

            Console.WriteLine("Test with value equal to default value.");
            data = new AttributeData("test", new XmlNameInfo("name"), true, 1);
            data.Value = 1;
            Assert.IsTrue(data.HasValue, "HasValue is incorrect.");
            Assert.IsTrue(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.AreEqual(1, data.Value, "Value is incorrect.");
        }
        #endregion Test Methods
    }
}
