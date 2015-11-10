namespace Localization.Xliff.OM.Modules.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.ChangeTracking.XmlNames;

    /// <summary>
    /// This class represents a container for a specific revision associated with a sibling element, or a child of a
    /// sibling element, to the change track module within the scope of the enclosing element. This corresponds to an
    /// &lt;item> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;item property=string ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#item">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    public class Item : XliffElement, IExtensible
    {
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="property">The type of revision data.</param>
        public Item(string property)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.Property = property;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        internal Item()
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
        /// Gets a value indicating whether the provider has text.
        /// </summary>
        protected override bool HasInnerText
        {
            get { return !string.IsNullOrEmpty(this.Text); }
        }

        /// <summary>
        /// Gets or sets the type of revision data.
        /// </summary>
        [SchemaEntity(AttributeNames.Property, Requirement.Required)]
        public string Property
        {
            get { return (string)this.GetPropertyValue(Item.PropertyNames.Property); }
            set { this.SetPropertyValue(value, Item.PropertyNames.Property); }
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

        /// <summary>
        /// Gets or sets the revision item text.
        /// </summary>
        public string Text { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Gets the text associated with this <see cref="Item"/>.
        /// </summary>
        /// <returns>The text within the <see cref="Item"/>.</returns>
        protected override string GetInnerText()
        {
            return this.Text;
        }

        /// <summary>
        /// Sets the text associated with this <see cref="Item"/>.
        /// </summary>
        /// <param name="text">The text to set.</param>
        protected override void SetInnerText(string text)
        {
            this.Text = text;
        }
        #endregion Methods

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Item.Property"/> property.
            /// </summary>
            public const string Property = "Property";
        }
    }
}
