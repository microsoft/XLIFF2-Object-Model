namespace Localization.Xliff.OM.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents the root of an XLIFF 2.0 document. The document contains various containers that
    /// store resources and translatable and translated text. This corresponds to a &lt;xliff> element
    /// in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;xliff version=string
    ///               srcLang=string
    ///               [trgLang=string]
    ///               xml:space=(default|preserve) ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#xliff">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    /// <seealso cref="ISelectable"/>
    /// <seealso cref="ISelectNavigable"/>
    [SchemaChild(NamespaceValues.Core, ElementNames.File, typeof(File))]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class XliffDocument : XliffElement, IExtensible, ISelectable, ISelectNavigable
    {
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="XliffDocument"/> class.
        /// </summary>
        /// <param name="sourceLanguage">The language of the source text to translate.</param>
        public XliffDocument(string sourceLanguage)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
            this.extensions = new Lazy<List<IExtension>>();
            this.Files = new ParentAttachedList<File>(this);
            this.SourceLanguage = sourceLanguage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XliffDocument"/> class.
        /// </summary>
        internal XliffDocument()
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
        /// Gets the list of <see cref="File"/>s that contain the resources to translate.
        /// </summary>
        public IList<File> Files { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.Files.Count > 0); }
        }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        bool IExtensible.HasExtensions
        {
            get { return this.extensions.IsValueCreated && (this.extensions.Value.Count > 0); }
        }

        /// <summary>
        /// Gets or sets the Id of the item.
        /// </summary>
        /// <remarks>This property always returns null.</remarks>
        public string Id
        {
            get { return null; }
            set { throw new NotSupportedException(Properties.Resources.XliffDocument_SetIdNotSupported); }
        }

        /// <summary>
        /// Gets a value indicating whether the element represents a leaf fragment in a selector path. If so, the
        /// selector path shouldn't contain any other fragments after this fragment.
        /// </summary>
        public bool IsLeafFragment
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the selector Id of the item.
        /// </summary>
        public string SelectorId
        {
            get { return XliffDocument.Constants.SelectorPrefix.ToString(); }
        }

        /// <summary>
        /// Gets the full path of the item from the root document.
        /// </summary>
        public string SelectorPath
        {
            get { return this.BuildSelectorPath(); }
        }

        /// <summary>
        /// Gets or sets the language of the source text to be translated.
        /// </summary>
        [SchemaEntity(AttributeNames.SourceLanguage, Requirement.Required)]
        public string SourceLanguage
        {
            get { return (string)this.GetPropertyValue(XliffDocument.PropertyNames.SourceLanguage); }
            set { this.SetPropertyValue(value, XliffDocument.PropertyNames.SourceLanguage); }
        }

        /// <summary>
        /// Gets or sets the language of the target text that was translated.
        /// </summary>
        [SchemaEntity(AttributeNames.TargetLanguage, Requirement.Optional)]
        public string TargetLanguage
        {
            get { return (string)this.GetPropertyValue(XliffDocument.PropertyNames.TargetLanguage); }
            set { this.SetPropertyValue(value, XliffDocument.PropertyNames.TargetLanguage); }
        }

        /// <summary>
        /// Gets or sets the version of the XLIFF document.
        /// </summary>
        [DefaultValue(AttributeValues.Version)]
        [SchemaEntity(AttributeNames.Version, Requirement.Required)]
        public string Version
        {
            get { return (string)this.GetPropertyValue(XliffDocument.PropertyNames.Version); }
            set { this.SetPropertyValue(value, XliffDocument.PropertyNames.Version); }
        }

        /// <summary>
        /// Gets or sets a value indicating how to handle whitespace.
        /// </summary>
        [Converter(typeof(PreservationConverter))]
        [DefaultValue(Preservation.Default)]
        [SchemaEntity(NamespacePrefixes.Xml, null, AttributeNames.SpacePreservation, Requirement.Optional)]
        public Preservation Space
        {
            get { return (Preservation)this.GetPropertyValue(XliffDocument.PropertyNames.Space); }
            set { this.SetPropertyValue(value, XliffDocument.PropertyNames.Space); }
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

        #region Methods
        /// <summary>
        /// Gets the child <see cref="XliffElement"/>s contained within this object.
        /// </summary>
        /// <returns>A list of child elements stored as a pair consisting of the XLIFF Name for the child and
        /// the child itself.</returns>
        protected override List<ElementInfo> GetChildren()
        {
            List<ElementInfo> result;

            result = null;
            this.AddChildElementsToList(this.Files, ref result);

            return result;
        }

        /// <summary>
        /// Selects an item matching the selection query.
        /// </summary>
        /// <param name="path">The selection query.</param>
        /// <returns>The object that was selected from the query path, or null if no match was found.</returns>
        /// <example>The value of <paramref name="path"/> might look something like "#g=group1/f=file1/u=unit1/n=note1"
        /// which is a full path from the document root.</example>
        public ISelectable Select(string path)
        {
            string selectionPath;

            ArgValidator.Create(path, "path").IsNotNull().StartsWith(Utilities.Constants.SelectorPathIndictator);

            selectionPath = Utilities.RemoveSelectorIndicator(path);
            if ((selectionPath.Length > 0) && (selectionPath[0] == Utilities.Constants.SelectorPathSeparator))
            {
                // Strip the leading '/'.
                selectionPath = selectionPath.Substring(1);
            }

            return this.SelectElement(selectionPath);
        }

        /// <summary>
        /// Stores the <see cref="XliffElement"/> as a child of this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>True if the child was stored, otherwise false.</returns>
        protected override bool StoreChild(ElementInfo child)
        {
            bool result;

            result = true;
            if (child.Element is File)
            {
                this.Files.Add((File)child.Element);
            }
            else
            {
                result = base.StoreChild(child);
            }

            return result;
        }
        #endregion Methods

        /// <summary>
        /// This class contains constant values that are used in this class.
        /// </summary>
        private static class Constants
        {
            /// <summary>
            /// The selector path prefix for this element.
            /// </summary>
            public const char SelectorPrefix = '#';
        }

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="XliffDocument.Space"/> property.
            /// </summary>
            public const string Space = "Space";

            /// <summary>
            /// The name of the <see cref="XliffDocument.SourceLanguage"/> property.
            /// </summary>
            public const string SourceLanguage = "SourceLanguage";

            /// <summary>
            /// The name of the <see cref="XliffDocument.TargetLanguage"/> property.
            /// </summary>
            public const string TargetLanguage = "TargetLanguage";

            /// <summary>
            /// The name of the <see cref="XliffDocument.Version"/> property.
            /// </summary>
            public const string Version = "Version";
        }
    }
}