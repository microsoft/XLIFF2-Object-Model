namespace Localization.Xliff.OM
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Localization.Xliff.OM.Attributes;

    /// <summary>
    /// This class provides data that is gathered using reflection.
    /// </summary>
    internal static class Reflector
    {
        #region Static Variables
        /// <summary>
        /// An array of empty objects used in reflection.
        /// </summary>
        private static readonly object[] EmptyObjects = new object[] { };
        #endregion Static Variables

        #region Methods
        /// <summary>
        /// Creates an <see cref="XliffElement"/> that corresponds to the given Xml namespace and name.
        /// </summary>
        /// <param name="name">The Xml namespace and name that represents the element to create.</param>
        /// <returns>The corresponding <see cref="XliffElement"/> or null if one wasn't found.</returns>
        public static XliffElement CreateElement(XmlNameInfo name)
        {
            return ReflectorCache.Instance.CreateElement(name);
        }

        /// <summary>
        /// Gets all the properties on the specified type.
        /// </summary>
        /// <param name="type">The type to reflect on.</param>
        /// <returns>The list of properties on the type.</returns>
        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return ReflectorCache.Instance.GetProperties(type);
        }

        /// <summary>
        /// Returns the set of attributes by looking for <see cref="SchemaEntityAttribute"/> attributes and
        /// returning the CSharp property Name and attribute data.
        /// </summary>
        /// <param name="type">The type to reflect on.</param>
        /// <param name="provider">A provider that can return custom values when inheriting values.</param>
        /// <returns>A dictionary whose type is the CSharp property Name, and the value is an object that
        /// stores the attribute data.</returns>
        /// <param name="outputResolver">The resolver to use when the output of properties is determined at runtime.
        /// </param>
        public static IDictionary<string, AttributeData> GetSchemaAttributes(
                                                                             Type type,
                                                                             IInheritanceInfoProvider provider,
                                                                             IOutputResolver outputResolver)
        {
            Dictionary<string, AttributeData> result;
            Dictionary<string, IEnumerable<ExplicitOutputDependencyAttribute>> outputDependencyMap;
            IEnumerable<SchemaAttributes> attributes;

            result = new Dictionary<string, AttributeData>();
            outputDependencyMap = new Dictionary<string, IEnumerable<ExplicitOutputDependencyAttribute>>();

            attributes = ReflectorCache.Instance.GetSchemaAttributes(type);
            foreach (SchemaAttributes entry in attributes)
            {
                if (entry.Schema != null)
                {
                    AttributeData data;

                    if (entry.DefaultValue != null)
                    {
                        data = new AttributeData(
                                                 type.Name,
                                                 entry.Schema.Name,
                                                 entry.Schema.Requirement != Requirement.Required,
                                                 entry.DefaultValue.Value);
                    }
                    else
                    {
                        data = new AttributeData(
                                                 type.Name,
                                                 entry.Schema.Name,
                                                 entry.Schema.Requirement != Requirement.Required);
                    }

                    data.IgnoreNamespace = true;
                    result.Add(entry.Name, data);

                    data.ConverterTypeName = (entry.Converter != null) ? entry.Converter.TypeName : null;
                    if (entry.HasValueIndicator != null)
                    {
                        data.HasValueIndicator = Reflector.GetSingletonViaActivator(entry.HasValueIndicator.TypeName) as IHasValueIndicator;
                    }

                    foreach (InheritValueAttribute inheritance in entry.InheritanceList)
                    {
                        InheritanceInfo inheritanceInfo;

                        data.InheritValue = true;

                        if (inheritance.Inheritance == Inheritance.Callback)
                        {
                            Debug.Assert(provider != null, "IInheritanceProvider is null.");
                            inheritanceInfo = provider.GetInheritanceInfo(entry.Name);
                            Debug.Assert(inheritanceInfo != null, "inheritanceInfo is null.");
                        }
                        else
                        {
                            inheritanceInfo = new InheritanceInfo(
                                                                  inheritance.AncestorType,
                                                                  inheritance.AncestorPropertyName);
                        }

                        data.InheritanceList.Add(inheritanceInfo);
                    }

                    // Store the mapping of attribute name to the names of other attributes it depends on for output.
                    if (entry.ExplicitOutputDependencies != null)
                    {
                        outputDependencyMap[entry.Name] = entry.ExplicitOutputDependencies;
                    }
                }
            }

            // Now that all attributes have been stored, add the explicit output dependencies referencing the
            // attributes directly.
            foreach (KeyValuePair<string, IEnumerable<ExplicitOutputDependencyAttribute>> pair in outputDependencyMap)
            {
                foreach (ExplicitOutputDependencyAttribute attribute in pair.Value)
                {
                    if (attribute.Property == null)
                    {
                        result[pair.Key].OutputResolver = outputResolver;
                    }
                    else
                    {
                        result[pair.Key].ExplicitOutputDependencies.Add(attribute.Property, result[attribute.Property]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the set of children types by looking for <see cref="SchemaChildAttribute"/> attributes and
        /// returning the type and XLIFF Name.
        /// </summary>
        /// <param name="type">The type to reflect on.</param>
        /// <returns>A dictionary whose type is the type of class a child represents, and the value is the XLIFF
        /// Name associated with that class.</returns>
        public static IDictionary<Type, XmlNameInfo> GetSchemaChildren(Type type)
        {
            return ReflectorCache.Instance.GetSchemaChildren(type);
        }

        /// <summary>
        /// Gets the instance of a type as if it were a singleton using the activator.
        /// </summary>
        /// <param name="typeName">The type of the object to get.</param>
        /// <returns>An instance of the type.</returns>
        public static object GetSingletonViaActivator(string typeName)
        {
            return ReflectorCache.Instance.GetSingletonViaActivator(typeName);
        }

        /// <summary>
        /// Invokes the default constructor for a given type.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <returns>An instance of the specified type, created using the default constructor.</returns>
        public static object InvokeDefaultConstructor(Type type)
        {
            ConstructorInfo info;

            info = ReflectorCache.Instance.GetDefaultConstructor(type);
            return info.Invoke(Reflector.EmptyObjects);
        }
        #endregion Methods
    }
}
