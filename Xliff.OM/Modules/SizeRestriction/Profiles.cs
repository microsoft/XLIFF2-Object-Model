namespace Localization.Xliff.OM.Modules.SizeRestriction
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.SizeRestriction.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class selects the restriction profiles to use in the document. If no storage or general profile is
    /// specified the default values (empty) of those elements will disable restriction checking in the file. This
    /// corresponds to a &lt;profiles> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;profiles [generalProfile=string]
    ///                  [storageProfile=string]>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#slr_profiles">XLIFF
    /// specification</a> for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    [SchemaChild(
                 NamespacePrefixes.SizeRestrictionModule,
                 NamespaceValues.SizeRestrictionModule,
                 ElementNames.Normalization,
                 typeof(Normalization))]
    public class Profiles : XliffElement, IExtensible
    {
        #region Member Variables
        /// <summary>
        /// The order in which to write data to a file so the output conforms to a defined schema.
        /// </summary>
        private static readonly IEnumerable<OutputItem> OutputOrderValues;

        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// The normalization form to apply to storage and size restrictions defined in the standard
        /// profiles.
        /// </summary>
        private Normalization normalization;
        #endregion Member Variables

        /// <summary>
        /// Initializes static members of the <see cref="Profiles"/> class.
        /// </summary>
        static Profiles()
        {
            Profiles.OutputOrderValues = new[] 
                { 
                    new OutputItem(OutputItemType.Child, typeof(Normalization), 1),
                    new OutputItem(OutputItemType.Extension, null, 2)
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Profiles"/> class.
        /// </summary>
        public Profiles()
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
        /// Gets or sets the name of a restriction profile to use while checking the general size restrictions. Empty
        /// string means that no restrictions apply.
        /// </summary>
        [DefaultValue("")]
        [SchemaEntity(AttributeNames.GeneralProfile, Requirement.Optional)]
        public string GeneralProfile
        {
            get { return (string)this.GetPropertyValue(Profiles.PropertyNames.GeneralProfile); }
            set { this.SetPropertyValue(value, Profiles.PropertyNames.GeneralProfile); }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.Normalization != null); }
        }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        bool IExtensible.HasExtensions
        {
            get { return this.extensions.IsValueCreated && (this.extensions.Value.Count > 0); }
        }

        /// <summary>
        /// Gets or sets the normalization form to apply to storage and size restrictions defined in the standard
        /// profiles.
        /// </summary>
        public Normalization Normalization
        {
            get
            {
                return this.normalization;
            }

            set
            {
                ArgValidator.ParentIsNull(value);
                Utilities.SetParent(this.normalization, null);
                Utilities.SetParent(value, this);
                this.normalization = value;
            }
        }

        /// <summary>
        /// Gets the order in which to write data to a file so the output conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be output in order to be compliant. This method is
        /// used during serialization to ensure the elements and text are output in the order specified by that schema.
        /// </summary>
        protected override IEnumerable<OutputItem> OutputOrder
        {
            get { return Profiles.OutputOrderValues; }
        }

        /// <summary>
        /// Gets or sets the name of a restriction profile to use while checking the storage size restrictions. Empty
        /// string means that no restrictions apply.
        /// </summary>
        [DefaultValue("")]
        [SchemaEntity(AttributeNames.StorageProfile, Requirement.Optional)]
        public string StorageProfile
        {
            get { return (string)this.GetPropertyValue(Profiles.PropertyNames.StorageProfile); }
            set { this.SetPropertyValue(value, Profiles.PropertyNames.StorageProfile); }
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
        /// Gets the child <see cref="XliffElement"/>s contained within this object.
        /// </summary>
        /// <returns>A list of child elements stored as a pair consisting of the XLIFF Name for the child and
        /// the child itself.</returns>
        protected override List<ElementInfo> GetChildren()
        {
            List<ElementInfo> result;

            result = new List<ElementInfo>();

            if (this.Normalization != null)
            {
                XmlNameInfo name;

                name = new XmlNameInfo(NamespaceValues.SizeRestrictionModule, ElementNames.Normalization);
                result.Add(new ElementInfo(name, this.Normalization));
            }

            return result;
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
            if (child.Element is Normalization)
            {
                Utilities.ThrowIfPropertyNotNull(this, PropertyNames.Normalization, this.Normalization);
                this.Normalization = (Normalization)child.Element;
                Utilities.SetParent(this.Normalization, this);
            }
            else
            {
                result = base.StoreChild(child);
            }

            return result;
        }
        #endregion Methods

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Profiles.GeneralProfile"/> property.
            /// </summary>
            public const string GeneralProfile = "GeneralProfile";

            /// <summary>
            /// The name of the <see cref="Profiles.Normalization"/> property.
            /// </summary>
            public const string Normalization = "Normalization";

            /// <summary>
            /// The name of the <see cref="Profiles.StorageProfile"/> property.
            /// </summary>
            public const string StorageProfile = "StorageProfile";
        }
    }
}
