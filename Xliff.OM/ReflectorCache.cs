namespace Localization.Xliff.OM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;
    using Localization.Xliff.OM.Attributes;

    /// <summary>
    /// This class is a cache for reflection operations so they aren't repeated unnecessarily.
    /// </summary>
    internal sealed class ReflectorCache
    {
        #region Static Variables
        /// <summary>
        /// The singleton instance of this class.
        /// </summary>
        private static readonly ReflectorCache ObjectInstance;

        /// <summary>
        /// Mapping from Xml name to <see cref="XliffElement"/> type that can be used to create an instance of the
        /// <see cref="XliffElement"/> from the Xml information.
        /// </summary>
        private readonly Lazy<Dictionary<string, Type>> allXliffElementTypes;

        /// <summary>
        /// Mapping from a type to the default constructor information.
        /// </summary>
        private readonly Dictionary<Type, ConstructorInfo> defaultConstructors;

        /// <summary>
        /// The list of attributes that map to properties in a class.
        /// </summary>
        private readonly Dictionary<Type, IEnumerable<SchemaAttributes>> schemaAttributes;

        /// <summary>
        /// Mapping from type to a list of types (representing children) and their corresponding XLIFF element names.
        /// </summary>
        private readonly Dictionary<Type, IDictionary<Type, XmlNameInfo>> schemaChildren;

        /// <summary>
        /// Mapping from type to a singleton instance.
        /// </summary>
        private readonly Dictionary<string, object> singletons;

        /// <summary>
        /// Mapping from type to a list of properties.
        /// </summary>
        private readonly Dictionary<Type, IEnumerable<PropertyInfo>> typeProperties;
        #endregion Static Variables

        /// <summary>
        /// Initializes static members of the <see cref="ReflectorCache"/> class.
        /// </summary>
        static ReflectorCache()
        {
            ReflectorCache.ObjectInstance = new ReflectorCache();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ReflectorCache"/> class from being created.
        /// </summary>
        private ReflectorCache()
        {
            this.allXliffElementTypes = new Lazy<Dictionary<string, Type>>();
            this.defaultConstructors = new Dictionary<Type, ConstructorInfo>();
            this.schemaAttributes = new Dictionary<Type, IEnumerable<SchemaAttributes>>();
            this.schemaChildren = new Dictionary<Type, IDictionary<Type, XmlNameInfo>>();
            this.singletons = new Dictionary<string, object>();
            this.typeProperties = new Dictionary<Type, IEnumerable<PropertyInfo>>();
        }

        #region Properties
        /// <summary>
        /// Gets the singleton instance of this class.
        /// </summary>
        public static ReflectorCache Instance
        {
            get { return ReflectorCache.ObjectInstance; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates an <see cref="XliffElement"/> that corresponds to the given Xml namespace and name.
        /// </summary>
        /// <param name="name">The Xml namespace and name that represents the element to create.</param>
        /// <returns>The corresponding <see cref="XliffElement"/> or null if one wasn't found.</returns>
        public XliffElement CreateElement(XmlNameInfo name)
        {
            Dictionary<string, Type> typeMap;
            XliffElement result;
            Type elementType;
            string key;

            typeMap = this.GetAllXliffElementTypes();
            key = ReflectorCache.MakeAllXliffElementTypesKey(name);
            if (typeMap.TryGetValue(key, out elementType))
            {
                result = (XliffElement)Reflector.InvokeDefaultConstructor(elementType);
            }
            else
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Gets the default constructor on a type.
        /// </summary>
        /// <param name="type">Type type to reflect on.</param>
        /// <returns>The default constructor, or null if not found.</returns>
        public ConstructorInfo GetDefaultConstructor(Type type)
        {
            lock (this.defaultConstructors)
            {
                ConstructorInfo ctor;

                if (!this.defaultConstructors.TryGetValue(type, out ctor))
                {
                    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

                    foreach (ConstructorInfo info in type.GetConstructors(Flags))
                    {
                        if (info.GetParameters().Length == 0)
                        {
                            ctor = info;
                            break;
                        }
                    }

                    this.defaultConstructors[type] = ctor;
                }

                return ctor;
            }
        }

        /// <summary>
        /// Gets all the properties on the specified type.
        /// </summary>
        /// <param name="type">The type to reflect on.</param>
        /// <returns>The list of properties on the type.</returns>
        public IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            lock (this.typeProperties)
            {
                IEnumerable<PropertyInfo> list;

                if (!this.typeProperties.TryGetValue(type, out list))
                {
                    list = type.GetProperties();
                    this.typeProperties[type] = list;
                }

                return list;
            }
        }

        /// <summary>
        /// Enumerates the properties in the specified type and stores the attributes associated with each property
        /// in a <see cref="SchemaAttributes"/> object.
        /// </summary>
        /// <param name="type">The type to reflect on.</param>
        /// <returns>An enumeration of attributes associated with each property in the <paramref name="type"/>.</returns>
        public IEnumerable<SchemaAttributes> GetSchemaAttributes(Type type)
        {
            lock (this.schemaAttributes)
            {
                IEnumerable<SchemaAttributes> result;

                if (!this.schemaAttributes.TryGetValue(type, out result))
                {
                    List<SchemaAttributes> list;

                    list = new List<SchemaAttributes>();
                    result = list;
                    this.schemaAttributes[type] = result;

                    foreach (PropertyInfo info in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        object[] attributes;
                        ConverterAttribute converter;
                        DefaultValueAttribute defaultValue;
                        HasValueIndicatorAttribute hasValueIndicator;
                        SchemaAttributes listEntry;
                        SchemaEntityAttribute schema;
                        List<InheritValueAttribute> inheritanceList;
                        List<string> dependencyList;

                        converter = null;
                        defaultValue = null;
                        dependencyList = new List<string>();
                        hasValueIndicator = null;
                        inheritanceList = new List<InheritValueAttribute>();
                        schema = null;

                        attributes = info.GetCustomAttributes(true);
                        foreach (object attribute in attributes)
                        {
                            Type attributeType;

                            attributeType = attribute.GetType();

                            if (attributeType == typeof(ConverterAttribute))
                            {
                                converter = (ConverterAttribute)attribute;
                            }
                            else if (attributeType == typeof(DefaultValueAttribute))
                            {
                                defaultValue = (DefaultValueAttribute)attribute;
                            }
                            else if (attributeType == typeof(HasValueIndicatorAttribute))
                            {
                                hasValueIndicator = (HasValueIndicatorAttribute)attribute;
                            }
                            else if (attributeType == typeof(SchemaEntityAttribute))
                            {
                                schema = (SchemaEntityAttribute)attribute;
                            }
                            else if (attributeType == typeof(InheritValueAttribute))
                            {
                                inheritanceList.Add((InheritValueAttribute)attribute);
                            }
                            else if (attributeType == typeof(ExplicitOutputDependencyAttribute))
                            {
                                dependencyList.Add(((ExplicitOutputDependencyAttribute)attribute).Property);
                            }
                        }

                        listEntry = new SchemaAttributes(
                                                         info.Name,
                                                         converter,
                                                         defaultValue,
                                                         hasValueIndicator,
                                                         schema,
                                                         inheritanceList,
                                                         dependencyList);
                        list.Add(listEntry);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Returns the set of children types by looking for <see cref="SchemaChildAttribute"/> attributes and
        /// returning the type and XLIFF Name.
        /// </summary>
        /// <param name="type">The type of object to reflect on.</param>
        /// <returns>A dictionary whose type is the type of class a child represents, and the value is the XLIFF
        /// Name associated with that class.</returns>
        public IDictionary<Type, XmlNameInfo> GetSchemaChildren(Type type)
        {
            lock (this.schemaChildren)
            {
                IDictionary<Type, XmlNameInfo> result;

                if (!this.schemaChildren.TryGetValue(type, out result))
                {
                    object[] attributes;

                    result = new Dictionary<Type, XmlNameInfo>();
                    this.schemaChildren[type] = result;

                    attributes = type.GetCustomAttributes(typeof(SchemaChildAttribute), true);
                    foreach (object attribute in attributes)
                    {
                        SchemaChildAttribute child;

                        child = (SchemaChildAttribute)attribute;
                        result.Add(child.ChildType, child.Name);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the instance of a type as if it were a singleton using the activator.
        /// </summary>
        /// <param name="typeName">The type of the object to get.</param>
        /// <returns>An instance of the type.</returns>
        public object GetSingletonViaActivator(string typeName)
        {
            lock (this.singletons)
            {
                object instance;

                if (!this.singletons.TryGetValue(typeName, out instance))
                {
                    Type type;

                    type = Type.GetType(typeName);
                    instance = Activator.CreateInstance(type);
                    this.singletons[typeName] = instance;
                }

                return instance;
            }
        }
        #endregion Methods

        /// <summary>
        /// Computes a key to use for the allXliffElementTypes dictionary.
        /// </summary>
        /// <param name="name">The Xml name information about the item whose key to compute.</param>
        /// <returns>A key that can be used to reference an element in this.allXliffElementTypes.</returns>
        private static string MakeAllXliffElementTypesKey(XmlNameInfo name)
        {
            return string.Join("<", name.Namespace, name.LocalName);
        }

        /// <summary>
        /// Gets all the registered <see cref="XliffElement"/>s using reflection on the types in this assembly.
        /// </summary>
        /// <returns>A dictionary whose key is a combination of the Xml namespace and name that the element is
        /// serialized under. The value is the type of the element.</returns>
        private Dictionary<string, Type> GetAllXliffElementTypes()
        {
            if (!this.allXliffElementTypes.IsValueCreated)
            {
                lock (this.allXliffElementTypes)
                {
                    if (!this.allXliffElementTypes.IsValueCreated)
                    {
                        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                        {
                            IDictionary<Type, XmlNameInfo> childrenTypes;

                            childrenTypes = Reflector.GetSchemaChildren(type);
                            foreach (KeyValuePair<Type, XmlNameInfo> pair in childrenTypes)
                            {
                                Type existingType;
                                string key;

                                key = ReflectorCache.MakeAllXliffElementTypesKey(pair.Value);

                                // This assumes that the names of Xliff elements are the same for a given type. For
                                // instance a Foo element is called xyz in the Xliff and we don't have a Bar element
                                // also called xyz.
                                if (this.allXliffElementTypes.Value.TryGetValue(key, out existingType))
                                {
                                    Debug.Assert(pair.Key == existingType, "The type for an Xml name is inconsistent.");
                                }
                                else
                                {
                                    this.allXliffElementTypes.Value.Add(key, pair.Key);
                                }
                            }
                        }
                    }
                }
            }

            return this.allXliffElementTypes.Value;
        }
    }
}
