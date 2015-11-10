namespace Localization.Xliff.OM.Modules.ResourceData
{
    using System;
    using System.Collections.Generic;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.ResourceData.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class is a base class for references to the actual resource data that is either intended for modification,
    /// or to be used as contextual reference during translation.
    /// </summary>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    public abstract class ResourceItemReferenceBase : XliffElement, IExtensible
    {
        #region Member Variables
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;
        #endregion Member Variables

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceItemReferenceBase"/> class.
        /// </summary>
        protected ResourceItemReferenceBase()
        {
            this.extensions = new Lazy<List<IExtension>>();
        }

        #region Properties
        /// <summary>
        /// Gets the list of registered extensions on the object.
        /// </summary>
        IList<IExtension> IExtensible.Extensions
        {
            get { return this.extensions.Value; }
        }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        bool IExtensible.HasExtensions
        {
            get { return this.extensions.IsValueCreated && (this.extensions.Value.Count > 0); }
        }

        /// <summary>
        /// Gets or sets an IRI referencing an external resource.
        /// </summary>
        [SchemaEntity(AttributeNames.HRef, Requirement.Optional)]
        public string HRef
        {
            get { return (string)this.GetPropertyValue(ResourceItemReferenceBase.PropertyNames.HRef); }
            set { this.SetPropertyValue(value, ResourceItemReferenceBase.PropertyNames.HRef); }
        }

        /// <summary>
        /// Gets or sets the language variant of the text of a given element.
        /// </summary>
        [SchemaEntity(NamespacePrefixes.Xml, null, AttributeNames.Language, Requirement.Optional)]
        public string Language
        {
            get { return (string)this.GetPropertyValue(ResourceItemReferenceBase.PropertyNames.Language); }
            set { this.SetPropertyValue(value, ResourceItemReferenceBase.PropertyNames.Language); }
        }

        /// <summary>
        /// Gets a value indicating whether attribute extensions are supported by the object.
        /// </summary>
        bool IExtensible.SupportsAttributeExtensions
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether element extensions are supported by the object.
        /// </summary>
        bool IExtensible.SupportsElementExtensions
        {
            get { return true; }
        }
        #endregion Properties

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="ResourceItemReferenceBase.HRef"/> property.
            /// </summary>
            public const string HRef = "HRef";

            /// <summary>
            /// The name of the <see cref="ResourceItemReferenceBase.Language"/> property.
            /// </summary>
            public const string Language = "Language";
        }
    }
}
