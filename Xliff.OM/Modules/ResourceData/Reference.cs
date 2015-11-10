namespace Localization.Xliff.OM.Modules.ResourceData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.ResourceData.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a reference to contextual data relating to the sibling <see cref="ResourceItemSource"/>
    /// and <see cref="ResourceItemTarget"/> elements, such as a German screenshot for a Luxembourgish translator.
    /// This corresponds to a &lt;reference> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;reference href=string
    ///                   [xml:lang=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#res_reference">XLIFF
    /// specification</a> for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    [SuppressMessage(
                     "StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class Reference : XliffElement, IExtensible
    {
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference"/> class.
        /// </summary>
        /// <param name="href">An IRI referencing an external resource.</param>
        public Reference(string href)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.HRef = href;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference"/> class.
        /// </summary>
        internal Reference()
            : this(null)
        {
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
        [SchemaEntity(AttributeNames.HRef, Requirement.Required)]
        public string HRef
        {
            get { return (string)this.GetPropertyValue(Reference.PropertyNames.HRef); }
            set { this.SetPropertyValue(value, Reference.PropertyNames.HRef); }
        }

        /// <summary>
        /// Gets or sets the language variant of the text of a given element.
        /// </summary>
        [SchemaEntity(NamespacePrefixes.Xml, null, AttributeNames.Language, Requirement.Optional)]
        public string Language
        {
            get { return (string)this.GetPropertyValue(Reference.PropertyNames.Language); }
            set { this.SetPropertyValue(value, Reference.PropertyNames.Language); }
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
            get { return false; }
        }
        #endregion Properties

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Reference.HRef"/> property.
            /// </summary>
            public const string HRef = "HRef";

            /// <summary>
            /// The name of the <see cref="Reference.Language"/> property.
            /// </summary>
            public const string Language = "Language";
        }
    }
}
