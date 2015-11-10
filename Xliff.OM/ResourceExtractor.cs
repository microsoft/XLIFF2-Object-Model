namespace Localization.Xliff.OM
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class extracts resources stored within the assembly. This class can be used to extract schemas for Xliff
    /// core and modules.
    /// </summary>
    public static class ResourceExtractor
    {
        /// <summary>
        /// The namespace under which all schema resources reside under.
        /// </summary>
        private const string BaseSchemaNamespace = "Localization.Xliff.OM.Resources.";

        /// <summary>
        /// The name of the schema for the change tracking module.
        /// </summary>
        public const string ChangeTrackingSchema = ResourceExtractor.BaseSchemaNamespace + "change_tracking.xsd";

        /// <summary>
        /// The name of the schema for the Xliff core.
        /// </summary>
        public const string CoreSchema = ResourceExtractor.BaseSchemaNamespace + "xliff_core_2.0.xsd";

        /// <summary>
        /// The name of the schema for the format style module.
        /// </summary>
        public const string FormatStyleSchema = ResourceExtractor.BaseSchemaNamespace + "fs.xsd";

        /// <summary>
        /// The name of the schema for the glossary module.
        /// </summary>
        public const string GlossarySchema = ResourceExtractor.BaseSchemaNamespace + "glossary.xsd";

        /// <summary>
        /// The name of the schema for the translation candidates module.
        /// </summary>
        public const string TranslationCandidatesSchema = ResourceExtractor.BaseSchemaNamespace + "matches.xsd";

        /// <summary>
        /// The name of the schema for the metadata module.
        /// </summary>
        public const string MetadataSchema = ResourceExtractor.BaseSchemaNamespace + "metadata.xsd";

        /// <summary>
        /// The name of the schema for the resource data module.
        /// </summary>
        public const string ResourceDataSchema = ResourceExtractor.BaseSchemaNamespace + "resource_data.xsd";

        /// <summary>
        /// The name of the schema for the size and length restriction module.
        /// </summary>
        public const string SizeRestrictionSchema = ResourceExtractor.BaseSchemaNamespace + "size_restriction.xsd";

        /// <summary>
        /// The name of the schema for the validation module.
        /// </summary>
        public const string ValidationSchema = ResourceExtractor.BaseSchemaNamespace + "validation.xsd";

        /// <summary>
        /// The name of the schema for the xml namespace.
        /// </summary>
        public const string XmlSchema = ResourceExtractor.BaseSchemaNamespace + "xml.xsd";

        /// <summary>
        /// Gets the resource names and namespaces for known schemas.
        /// </summary>
        /// <returns>A dictionary whose key is the resource name for a schema, and the value is the registered
        /// namespace associated with that schema.</returns>
        public static IDictionary<string, string> GetSchemaResourceNames()
        {
            return new Dictionary<string, string>()
            {
                { ResourceExtractor.ChangeTrackingSchema, NamespaceValues.ChangeTrackingModule },
                { ResourceExtractor.CoreSchema, NamespaceValues.Core },
                { ResourceExtractor.FormatStyleSchema, NamespaceValues.FormatStyleModule },
                { ResourceExtractor.GlossarySchema, NamespaceValues.GlossaryModule },
                { ResourceExtractor.TranslationCandidatesSchema, NamespaceValues.TranslationCandidatesModule },
                { ResourceExtractor.MetadataSchema, NamespaceValues.MetadataModule },
                { ResourceExtractor.ResourceDataSchema, NamespaceValues.ResourceDataModule },
                { ResourceExtractor.SizeRestrictionSchema, NamespaceValues.SizeRestrictionModule },
                { ResourceExtractor.ValidationSchema, NamespaceValues.ValidationModule },
                { ResourceExtractor.XmlSchema, string.Empty }
            };
        }

        /// <summary>
        /// Gets the specified resource from this assembly. 
        /// </summary>
        /// <param name="name">The case-sensitive name of the manifest resource being requested</param>
        /// <returns>The resource or null if a resource with the specified name is not present.</returns>
        public static Stream GetResource(string name)
        {
            Assembly assembly;

            ArgValidator.Create(name, "name").IsNotNull();

            assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(name);
        }
    }
}
