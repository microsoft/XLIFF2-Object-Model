namespace Localization.Xliff.OM.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.XmlNames;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="Reflector"/> class.
    /// </summary>
    [TestClass]
    public class ReflectorTests
    {
        #region Test Classes
        /// <summary>
        /// This class is used to test when there are no <see cref="SchemaChildAttribute"/> attributes.
        /// </summary>
        private class ClassWithNoAttributes
        {
        }

        /// <summary>
        /// This class is used to test when there is one <see cref="SchemaChildAttribute"/> attribute.
        /// </summary>
        [SchemaChildAttribute(NamespaceValues.Core, "name", typeof(string))]
        private class ClassWithOneAttribute
        {
        }

        /// <summary>
        /// This class is used to test when there are multiple <see cref="SchemaChildAttribute"/> attributes.
        /// The attributes have different types but the same names.
        /// </summary>
        [SchemaChildAttribute(NamespaceValues.Core, "name", typeof(string))]
        [SchemaChildAttribute(NamespaceValues.Core, "name", typeof(int))]
        private class ClassWithMultipleAttributes
        {
        }

        /// <summary>
        /// This class is used to test when there are duplicate <see cref="SchemaChildAttribute"/> attributes.
        /// The attributes have the same type but different names.
        /// </summary>
        [SchemaChildAttribute(NamespaceValues.Core, "name1", typeof(string))]
        [SchemaChildAttribute(NamespaceValues.Core, "name2", typeof(string))]
        private class ClassWithDuplicateAttributes
        {
        }

        /// <summary>
        /// This class is used to test when parsing properties with duplicate <see cref="SchemaEntityAttribute"/>
        /// attributes. The attributes have the same name.
        /// </summary>
        private class ClassWithDuplicateSchemaAttributes
        {
            [SchemaEntityAttribute("name", Xliff.OM.Requirement.Optional)]
            public string Property1 { get; set; }

            [SchemaEntityAttribute("name", Xliff.OM.Requirement.Optional)]
            public string Property2 { get; set; }
        }

        /// <summary>
        /// This class is used to test when parsing properties with <see cref="SchemaEntityAttribute"/> attributes.
        /// </summary>
        private class ClassWithSchemaAttributes
        {
            // Protected (non-public) property.
            [SchemaEntityAttribute("name1", Xliff.OM.Requirement.Optional)]
            protected string Property1 { get; set; }

            // Entity only attribute.
            [SchemaEntityAttribute("name2", Xliff.OM.Requirement.Optional)]
            public string Property2 { get; set; }

            // Converter and Entity attribute.
            [Converter(typeof(string))]
            [SchemaEntityAttribute("name3", Xliff.OM.Requirement.Optional)]
            public string Property3 { get; set; }

            // Default and Entity attribute.
            [DefaultValue("hello")]
            [SchemaEntityAttribute("name4", Xliff.OM.Requirement.Optional)]
            public string Property4 { get; set; }

            // Converter, Default, and Entity attribute.
            [Converter(typeof(string))]
            [DefaultValue("hello")]
            [SchemaEntityAttribute("name5", Xliff.OM.Requirement.Optional)]
            public string Property5 { get; set; }

            // Converter only attribute.
            [Converter(typeof(string))]
            public string Property6 { get; set; }

            // Default only attribute.
            [DefaultValue("hello")]
            public string Property7 { get; set; }

            // Another Converter, Default, and Entity attribute.
            [Converter(typeof(string))]
            [DefaultValue("hello")]
            [SchemaEntityAttribute("name8", Xliff.OM.Requirement.Optional)]
            public string Property8 { get; set; }
        }

        /// <summary>
        /// This class is used to test when parsing properties with no <see cref="SchemaEntityAttribute"/> attributes.
        /// </summary>
        private class ClassWithNoSchemaAttributes
        {
        }
        #endregion Test Classes

        /// <summary>
        /// Tests the GetSchemaAttributes method.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Reflector_GetSchemaAttributes()
        {
            AttributeData data;
            IDictionary<string, AttributeData> result;
            string xmlName;

            //
            // Test with no attributes.
            //

            Console.WriteLine("Test with no attributes.");
            result = Reflector.GetSchemaAttributes(typeof(ClassWithNoSchemaAttributes), null);
            Assert.IsNotNull(result, "No results were returned.");
            Assert.AreEqual(0, result.Count, "Count of results is incorrect.");

            //
            // Test the following:
            //  + protected property
            //  + converter and schema attributes
            //  + default and schema attributes
            //  + multiple converter, default, and schema attributes
            //  + converter only attribute
            //  + default only attribute
            //  + schema only attribute
            //

            Console.WriteLine("Test with various attributes.");
            result = Reflector.GetSchemaAttributes(typeof(ClassWithSchemaAttributes), null);
            Assert.IsNotNull(result, "No results were returned.");
            Assert.AreEqual(5, result.Count, "Count of results is incorrect.");

            xmlName = "name2";
            Console.WriteLine("Validating item {0}.", xmlName);
            data = result["Property2"];
            Assert.IsNull(data.ConverterTypeName, "ConverterTypeName is incorrect.");
            Assert.IsFalse(data.HasValue, "HasValue is incorrect.");
            Assert.IsFalse(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.IsTrue(data.IsOptional, "IsOptional is incorrect.");
            Assert.IsNull(data.Value, "Value is incorrect.");
            Assert.AreEqual(xmlName, data.LocalName, "XmlName is incorrect.");

            xmlName = "name3";
            Console.WriteLine("Validating item {0}.", xmlName);
            data = result["Property3"];
            Assert.AreEqual(typeof(string).FullName, data.ConverterTypeName, "ConverterTypeName is incorrect.");
            Assert.IsFalse(data.HasValue, "HasValue is incorrect.");
            Assert.IsFalse(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.IsTrue(data.IsOptional, "IsOptional is incorrect.");
            Assert.IsNull(data.Value, "Value is incorrect.");
            Assert.AreEqual(xmlName, data.LocalName, "XmlName is incorrect.");

            xmlName = "name4";
            Console.WriteLine("Validating item {0}.", xmlName);
            data = result["Property4"];
            Assert.IsNull(data.ConverterTypeName, "ConverterTypeName is incorrect.");
            Assert.IsFalse(data.HasValue, "HasValue is incorrect.");
            Assert.IsTrue(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.IsTrue(data.IsOptional, "IsOptional is incorrect.");
            Assert.AreEqual("hello", data.Value, "Value is incorrect.");
            Assert.AreEqual(xmlName, data.LocalName, "XmlName is incorrect.");

            xmlName = "name5";
            Console.WriteLine("Validating item {0}.", xmlName);
            data = result["Property5"];
            Assert.AreEqual(typeof(string).FullName, data.ConverterTypeName, "ConverterTypeName is incorrect.");
            Assert.IsFalse(data.HasValue, "HasValue is incorrect.");
            Assert.IsTrue(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.IsTrue(data.IsOptional, "IsOptional is incorrect.");
            Assert.AreEqual("hello", data.Value, "Value is incorrect.");
            Assert.AreEqual(xmlName, data.LocalName, "XmlName is incorrect.");

            xmlName = "name8";
            Console.WriteLine("Validating item {0}.", xmlName);
            data = result["Property8"];
            Assert.AreEqual(typeof(string).FullName, data.ConverterTypeName, "ConverterTypeName is incorrect.");
            Assert.IsFalse(data.HasValue, "HasValue is incorrect.");
            Assert.IsTrue(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.IsTrue(data.IsOptional, "IsOptional is incorrect.");
            Assert.AreEqual("hello", data.Value, "Value is incorrect.");
            Assert.AreEqual(xmlName, data.LocalName, "XmlName is incorrect.");

            //
            // Test with duplicate attributes.
            //

            Console.WriteLine("Test with duplicate attributes.");
            result = Reflector.GetSchemaAttributes(typeof(ClassWithDuplicateSchemaAttributes), null);

            xmlName = "name";
            Console.WriteLine("Validating item {0}.", xmlName);
            data = result["Property1"];
            Assert.IsNull(data.ConverterTypeName, "ConverterTypeName is incorrect.");
            Assert.IsFalse(data.HasValue, "HasValue is incorrect.");
            Assert.IsFalse(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.IsTrue(data.IsOptional, "IsOptional is incorrect.");
            Assert.IsNull(data.Value, "Value is incorrect.");
            Assert.AreEqual(xmlName, data.LocalName, "XmlName is incorrect.");

            xmlName = "name";
            Console.WriteLine("Validating item {0}.", xmlName);
            data = result["Property2"];
            Assert.IsNull(data.ConverterTypeName, "ConverterTypeName is incorrect.");
            Assert.IsFalse(data.HasValue, "HasValue is incorrect.");
            Assert.IsFalse(data.IsDefaultValue, "IsDefaultValue is incorrect.");
            Assert.IsTrue(data.IsOptional, "IsOptional is incorrect.");
            Assert.IsNull(data.Value, "Value is incorrect.");
            Assert.AreEqual(xmlName, data.LocalName, "XmlName is incorrect.");
        }

        /// <summary>
        /// Tests the GetSchemaChildren method.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Reflector_GetSchemaChildren()
        {
            IDictionary<Type, XmlNameInfo> result;

            //
            // Test with no attributes.
            //

            Console.WriteLine("Test with no attributes.");
            result = Reflector.GetSchemaChildren(typeof(ClassWithNoAttributes));
            Assert.IsNotNull(result, "No results were returned.");
            Assert.AreEqual(0, result.Count, "Count of results is incorrect.");

            //
            // Test with one attribute.
            //

            Console.WriteLine("Test with one attribute.");
            result = Reflector.GetSchemaChildren(typeof(ClassWithOneAttribute));
            Assert.IsNotNull(result, "No results were returned.");
            Assert.AreEqual(1, result.Count, "Count of results is incorrect.");
            Assert.IsTrue(result.ContainsKey(typeof(string)), "String type was not found.");
            Assert.AreEqual(new XmlNameInfo(null, NamespaceValues.Core, "name"), result[typeof(string)], "Name does not match.");

            //
            // Test with multiple attributes.
            //

            Console.WriteLine("Test with multiple attributes.");
            result = Reflector.GetSchemaChildren(typeof(ClassWithMultipleAttributes));
            Assert.IsNotNull(result, "No results were returned.");
            Assert.AreEqual(2, result.Count, "Count of results is incorrect.");
            Assert.IsTrue(result.ContainsKey(typeof(string)), "String type was not found.");
            Assert.AreEqual(new XmlNameInfo(null, NamespaceValues.Core, "name"), result[typeof(string)], "Name does not match string type.");
            Assert.IsTrue(result.ContainsKey(typeof(int)), "Int type was not found.");
            Assert.AreEqual(new XmlNameInfo(null, NamespaceValues.Core, "name"), result[typeof(int)], "Name does not match int type.");

            //
            // Test with duplicate attributes.
            //

            Console.WriteLine("Test with duplicate attributes.");
            try
            {
                result = Reflector.GetSchemaChildren(typeof(ClassWithDuplicateAttributes));
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }
    }
}
