namespace Localization.Xliff.OM.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Extensibility;

    /// <summary>
    /// This class is a container for non-translatable content. Modifiers and enrichers must not change this content.
    /// This corresponds to a &lt;skeleton> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: 
    ///     &lt;skeleton [href=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#skeleton">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class Skeleton : XliffElement, IExtensible
    {
        /// <summary>
        /// The order in which to write data to a file so the output conforms to a defined schema.
        /// </summary>
        private static readonly IEnumerable<OutputItem> OutputOrderValues;

        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// Initializes static members of the <see cref="Skeleton"/> class.
        /// </summary>
        static Skeleton()
        {
            Skeleton.OutputOrderValues = new[] 
                { 
                    new OutputItem(OutputItemType.Text, null, 1),
                    new OutputItem(OutputItemType.Extension, null, 1)
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Skeleton"/> class.
        /// </summary>
        public Skeleton()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
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
        /// Gets a value indicating whether the provider has text.
        /// </summary>
        protected override bool HasInnerText
        {
            get { return !string.IsNullOrEmpty(this.NonTranslatableText); }
        }

        /// <summary>
        /// Gets or sets the reference to the location of an external skeleton file.
        /// </summary>
        [SchemaEntity(AttributeNames.HRef, Requirement.Optional)]
        public string HRef
        {
            get { return (string)this.GetPropertyValue(Skeleton.PropertyNames.HRef); }
            set { this.SetPropertyValue(value, Skeleton.PropertyNames.HRef); }
        }

        /// <summary>
        /// Gets or sets the text that is not to be translated.
        /// </summary>
        public string NonTranslatableText { get; set; }

        /// <summary>
        /// Gets the order in which to write data to a file so the output conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be output in order to be compliant. This method is
        /// used during serialization to ensure the elements and text are output in the order specified by that schema.
        /// </summary>
        protected override IEnumerable<OutputItem> OutputOrder
        {
            get { return Skeleton.OutputOrderValues; }
        }

        /// <summary>
        /// Gets a value indicating whether attribute extensions are supported by the object.
        /// </summary>
        bool IExtensible.SupportsAttributeExtensions
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether element extensions are supported by the object.
        /// </summary>
        bool IExtensible.SupportsElementExtensions
        {
            get { return true; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Gets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <returns>The text within the <see cref="XliffElement"/>.</returns>
        protected override string GetInnerText()
        {
            return this.NonTranslatableText;
        }

        /// <summary>
        /// Sets the text associated with the object.
        /// </summary>
        /// <param name="text">The text to set.</param>
        protected override void SetInnerText(string text)
        {
            this.NonTranslatableText = text;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return this.NonTranslatableText;
        }
        #endregion Methods

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Skeleton.HRef"/> property.
            /// </summary>
            public const string HRef = "HRef";
        }
    }
}