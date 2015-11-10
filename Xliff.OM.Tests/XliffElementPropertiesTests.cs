namespace Localization.Xliff.OM.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Modules;
    using Localization.Xliff.OM.Modules.ChangeTracking;
    using Localization.Xliff.OM.Modules.FormatStyle;
    using Localization.Xliff.OM.Modules.Glossary;
    using Localization.Xliff.OM.Modules.Metadata;
    using Localization.Xliff.OM.Modules.ResourceData;
    using Localization.Xliff.OM.Modules.SizeRestriction;
    using Localization.Xliff.OM.Modules.TranslationCandidates;
    using Localization.Xliff.OM.Modules.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the way properties for <see cref="XliffElement"/> derived types.
    /// </summary>
    [TestClass()]
    public class XliffElementPropertiesTests
    {
        #region Member Variables
        /// <summary>
        /// The name of the DLL that contains the XLIFF object model being tested.
        /// </summary>
        private const string XLIFF_DLL_NAME = @"Xliff.OM.dll";

        /// <summary>
        /// An array of empty <see cref="Type"/> objects used in reflection.
        /// </summary>
        private static readonly Type[] _emptyTypes = new Type[] { };

        /// <summary>
        /// An array of empty objects used in reflection.
        /// </summary>
        private static readonly object[] _emptyObjects = new object[] { };
        #endregion Member Variables

        #region Test Methods
        /// <summary>
        /// Verifies that the getter and setter work.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_SchemaProperties()
        {
            List<Tuple<Type, PropertyInfo>> propertyList;
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            propertyList = XliffElementPropertiesTests.GetProperties(true);
            foreach (Tuple<Type, PropertyInfo> tuple in propertyList)
            {
                ConstructorInfo ctor;
                object instance;
                object oldValue;
                object newValue;
                bool notSupported;

                if (!XliffElementPropertiesTests.HasSetter(tuple.Item2))
                {
                    continue;
                }

                Console.WriteLine(tuple.Item1.Name + "." + tuple.Item2.Name);

                ctor = tuple.Item1.GetConstructor(flags, null, XliffElementPropertiesTests._emptyTypes, null);
                instance = ctor.Invoke(XliffElementPropertiesTests._emptyObjects);
                newValue = null;

                try
                {
                    oldValue = tuple.Item2.GetValue(instance);
                    newValue = XliffElementPropertiesTests.GetNewValue(tuple.Item2.PropertyType, oldValue);

                    Assert.AreNotEqual(newValue, oldValue, "The new value and old value are the same.");
                    notSupported = false;
                }
                catch (TargetInvocationException e)
                {
                    // If the property is not supported a NotSupportedException is thrown and is wrapped by a
                    // TargetInvocationException by .NET. Verify that is the case here. If it's not a
                    // NotSupportedException then something else happened and the test should fail.
                    Assert.IsInstanceOfType(e.InnerException, typeof(NotSupportedException));
                    notSupported = true;
                }

                try
                {
                    tuple.Item2.SetValue(instance, newValue);

                    oldValue = tuple.Item2.GetValue(instance);
                    Assert.AreEqual(newValue, oldValue, "The setter didn't update the property.");
                    Assert.IsFalse(notSupported, "The getter was not supported but the setter is.");
                }
                catch (TargetInvocationException e)
                {
                    // If the property is not supported a NotSupportedException is thrown and is wrapped by a
                    // TargetInvocationException by .NET. Verify that is the case here. If it's not a
                    // NotSupportedException then something else happened and the test should fail.
                    if ((instance is Data) && (tuple.Item2.Name == "Space"))
                    {
                        Assert.IsInstanceOfType(e.InnerException, typeof(ArgumentException));
                    }
                    else
                    {
                        Assert.IsInstanceOfType(e.InnerException, typeof(NotSupportedException));
                    }
                }
            }
        }

        /// <summary>
        /// Verifies that the underlying attribute is in sync with the getter and setter.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_SchemaPropertyAttributes()
        {
            List<Tuple<Type, PropertyInfo>> propertyList;
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            propertyList = XliffElementPropertiesTests.GetSchemaEntityProperties(true);
            foreach (Tuple<Type, PropertyInfo> tuple in propertyList)
            {
                ConstructorInfo ctor;
                Dictionary<string, AttributeData> attributes;
                object instance;
                object value;

                Console.WriteLine(tuple.Item1.Name + "." + tuple.Item2.Name);

                ctor = tuple.Item1.GetConstructor(flags, null, XliffElementPropertiesTests._emptyTypes, null);
                instance = ctor.Invoke(XliffElementPropertiesTests._emptyObjects);

                try
                {
                    value = tuple.Item2.GetValue(instance);
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException is NotSupportedException)
                    {
                        Console.WriteLine("Skipping test because the property is not supported by this object.");
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }
            
                //
                // This logic is not ideal because it uses reflection to get at a private member, but we really want
                // to verify that the public property and the internal property are mapped correctly. We could make
                // the internal attributes public or internal, but then other methods would be able to access the
                // field and that is not ideal because nothing else should know about this logic.
                //

                attributes = (Dictionary<string, AttributeData>)typeof(XliffElement)
                                                                .GetField("attributes", flags)
                                                                .GetValue(instance);
                Assert.AreEqual(value, attributes[tuple.Item2.Name].Value);

                value = XliffElementPropertiesTests.GetNewValue(tuple.Item2.PropertyType, value);

                // SubFormatStyle doesn't have a setter.
                if ((!(instance is Data) || (tuple.Item2.Name != "Space")) && (tuple.Item2.Name != "SubFormatStyle"))
                {
                    tuple.Item2.SetValue(instance, value);
                    Assert.AreEqual(value, attributes[tuple.Item2.Name].Value);
                }
            }
        }

        /// <summary>
        /// Verifies that all elements have default constructors so they can be created via reflection during
        /// deserialization.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_XliffElementsDefaultCtor()
        {
            foreach (Type type in XliffElementPropertiesTests.GetXliffElementTypes())
            {
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                ConstructorInfo ctor;
                object instance;

                if (type.IsAbstract)
                {
                    continue;
                }

                Console.WriteLine(type.FullName);

                ctor = type.GetConstructor(flags, null, XliffElementPropertiesTests._emptyTypes, null);
                Assert.IsNotNull(ctor);

                instance = ctor.Invoke(XliffElementPropertiesTests._emptyObjects);
                Assert.IsNotNull(instance);
            }
        }

        /// <summary>
        /// Verifies that all schema type properties have both getter and setter.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_XliffElementsProperties()
        {
            List<Tuple<Type, PropertyInfo>> propertyList;

            propertyList = XliffElementPropertiesTests.GetSchemaEntityProperties(true);
            foreach (Tuple<Type, PropertyInfo> tuple in propertyList)
            {
                MethodInfo[] methods;
                bool result;

                Console.WriteLine(tuple.Item1.Name + "." + tuple.Item2.Name);

                //
                // Verify the property has both get and set accessors.
                //

                methods = tuple.Item2.GetAccessors(false);

                Assert.IsNotNull(methods, "No accessors found.");
                if (tuple.Item2.PropertyType == typeof(IDictionary<string, string>))
                {
                    Assert.AreEqual(1, methods.Length, "Accessor count mismatch.");

                    result = methods[0].Name.StartsWith("get_");
                }
                else
                {
                    Assert.AreEqual(2, methods.Length, "Accessor count mismatch.");

                    result = (methods[0].Name.StartsWith("get_") && methods[1].Name.StartsWith("set_")) ||
                             (methods[0].Name.StartsWith("set_") && methods[1].Name.StartsWith("get_"));
                }

                Assert.IsTrue(result, "Property does not contain get and set accessors.");
            }
        }
        #endregion Test Methods

        #region Helper Methods
        /// <summary>
        /// Gets a value for a specific type that is used to construct a document.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="currentValue">The existing value of the same type, used to make sure a duplicate value
        /// isn't returned.</param>
        /// <returns>An object of the specified type that is used to create a document.</returns>
        private static object GetNewValue(Type type, object currentValue)
        {
            object newValue;
            bool skipValueCheck;

            skipValueCheck = false;

            Dictionary<Type, object> values = new Dictionary<Type, object>()
            {
                { typeof(CanReorderValue), CanReorderValue.No },
                { typeof(ChangeTrack), new ChangeTrack() },
                { typeof(CodeType?), CodeType.UserInterface },
                { typeof(ContentDirectionality), ContentDirectionality.LTR },
                { typeof(Data), new Data() },
                { typeof(DateTime?), DateTime.Now },
                { typeof(Definition), new Definition() },
                { typeof(float?), 43f },
                { typeof (FormatStyleValue?), FormatStyleValue.BiDiOverride },
                { typeof(Glossary), new Glossary() },
                { typeof(GlossaryEntry), new GlossaryEntry() },
                { typeof(IList<TranslationContainer>), new List<TranslationContainer>() },
                { typeof(int), 3 },
                { typeof(int?), 32 },
                { typeof(MatchType), MatchType.InContextMatch },
                { typeof(MetadataContainer), new MetadataContainer() },
                { typeof(MetaGroupSubject?), MetaGroupSubject.Ignorable },
                { typeof(Normalization), new Normalization()},
                { typeof(NormalizationValue), NormalizationValue.NFD},
                { typeof(OriginalData), new OriginalData() },
                { typeof(ProfileData), new ProfileData() },
                { typeof(Profiles), new Profiles() },
                { typeof(ResourceData), new ResourceData() },
                { typeof(ResourceItem), new ResourceItem() },
                { typeof(ResourceItemRef), new ResourceItemRef() },
                { typeof(ResourceItemSource), new ResourceItemSource() },
                { typeof(ResourceItemTarget), new ResourceItemTarget() },
                { typeof(Skeleton), new Skeleton() },
                { typeof(Source), new Source() },
                { typeof(string), "hello" },
                { typeof(Target), new Target() },
                { typeof(Term), new Term() },
                { typeof(TranslationState), TranslationState.Reviewed },
                { typeof(TranslationSubject?), TranslationSubject.Target },
                { typeof(uint?), 32u },
                { typeof(Validation), new Validation() },
                { typeof(XliffDocument), new XliffDocument() }
            };

            if (!values.TryGetValue(type, out newValue))
            {
                if (type == typeof(bool))
                {
                    newValue = !(bool)currentValue;
                }
                else if (type == typeof(IDictionary<string, string>))
                {
                    IDictionary<string, string> value;

                    value = (IDictionary<string, string>)currentValue;
                    value.Add("newkey", "newvalue");
                    newValue = currentValue;
                    skipValueCheck = true;
                }
                else if (type == typeof(Preservation))
                {
                    newValue = Preservation.Preserve;
                    if ((Preservation)currentValue == Preservation.Preserve)
                    {
                        newValue = Preservation.Default;
                    }
                }
                else
                {
                    Assert.Fail("PropertyType {0} is not covered in the test.", type);
                }
            }

            if (!skipValueCheck)
            {
                Assert.AreNotEqual(currentValue, newValue, "The new value is the same as the current value.");
            }

            return newValue;
        }

        /// <summary>
        /// Gets a list of properties across all  types, optionally excluding abstract types.
        /// </summary>
        /// <param name="skipAbstractTypes">True to ignore abstract types, false to include them.</param>
        /// <returns>A list of types and their associated properties. All types are classes that derive from
        /// <see cref="XliffElement"/>.</returns>
        private static List<Tuple<Type, PropertyInfo>> GetProperties(bool skipAbstractTypes)
        {
            List<Tuple<Type, PropertyInfo>> result;

            result = new List<Tuple<Type, PropertyInfo>>();

            foreach (Type type in XliffElementPropertiesTests.GetXliffElementTypes())
            {
                IEnumerable<PropertyInfo> propertyList;

                if (skipAbstractTypes && type.IsAbstract)
                {
                    continue;
                }

                propertyList = type.GetProperties();
                foreach (PropertyInfo property in propertyList)
                {
                    result.Add(new Tuple<Type, PropertyInfo>(type, property));
                }
            }

            //
            // Make sure this method worked correctly.
            //

            Assert.IsTrue(result.Count > 0, "GetSchemaEntityProperties failed to return items.");

            return result;
        }

        /// <summary>
        /// Gets a list of properties across all <see cref="XliffElement"/> types that are attributed with
        /// <see cref="SchemaEntityAttribute"/>, optionally excluding abstract types.
        /// </summary>
        /// <param name="skipAbstractTypes">True to ignore abstract types, false to include them.</param>
        /// <returns>A list of types and their associated properties. All types are classes that derive from
        /// <see cref="XliffElement"/>.</returns>
        private static List<Tuple<Type, PropertyInfo>> GetSchemaEntityProperties(bool skipAbstractTypes)
        {
            List<Tuple<Type, PropertyInfo>> result;

            result = new List<Tuple<Type, PropertyInfo>>();

            foreach (Type type in XliffElementPropertiesTests.GetXliffElementTypes())
            {
                IEnumerable<PropertyInfo> propertyList;

                if (skipAbstractTypes && type.IsAbstract)
                {
                    continue;
                }

                propertyList = XliffElementPropertiesTests.GetSchemaEntityProperties(type);
                foreach (PropertyInfo property in propertyList)
                {
                    result.Add(new Tuple<Type, PropertyInfo>(type, property));
                }
            }

            //
            // Make sure this method worked correctly.
            //

            Assert.IsTrue(result.Count > 0, "GetSchemaEntityProperties failed to return items.");

            return result;
        }

        /// <summary>
        /// Gets a list of properties on the specified type that are attributed with
        /// <see cref="SchemaEntityAttribute"/>.
        /// </summary>
        /// <param name="type">The type that contains properties to return.</param>
        /// <returns>A list of properties that are attributed with <see cref="SchemaEntityAttribute"/>.</returns>
        private static List<PropertyInfo> GetSchemaEntityProperties(Type type)
        {
            List<PropertyInfo> result;

            result = new List<PropertyInfo>();
            foreach (PropertyInfo property in type.GetProperties())
            {
                bool hasAttributes;

                hasAttributes = property.GetCustomAttributes(typeof(SchemaEntityAttribute), false).Any();
                if (hasAttributes)
                {
                    result.Add(property);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a list of the classes that derive from <see cref="XliffElement"/>.
        /// </summary>
        /// <returns>The list of types.</returns>
        private static List<Type> GetXliffElementTypes()
        {
            Assembly assembly;
            List<Type> result;

            result = new List<Type>();

            assembly = Assembly.LoadFrom(XliffElementPropertiesTests.XLIFF_DLL_NAME);
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(XliffElement)))
                {
                    result.Add(type);
                }
            }

            //
            // Make sure this method worked correctly.
            //

            Assert.IsTrue(result.Count > 0, "GetSchemaEntityProperties failed to return items.");

            return result;
        }

        /// <summary>
        /// Determines whether a given property has a setter accessor.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>True if the property has a setter accessor, otherwise false.</returns>
        private static bool HasSetter(PropertyInfo property)
        {
            MethodInfo[] methods;
            bool result;

            result = false;

            methods = property.GetAccessors(false);
            foreach (MethodInfo info in methods)
            {
                if (info.Name.StartsWith("set_"))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
        #endregion Helper Methods
    }
}
