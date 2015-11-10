namespace Localization.Xliff.OM.Modules.SizeRestriction
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.SizeRestriction.XmlNames;

    /// <summary>
    /// This class is a container for data needed by the specified profile to check the part of the XLIFF document that
    /// is a sibling or descendant of a sibling of this element. It is not used by the default profiles. This
    /// corresponds to a &lt;data> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;data ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#slr_data">XLIFF
    /// specification</a> for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    [SuppressMessage(
                     "StyleCop.CSharp.DocumentationRules",
                     "SA1650:ElementDocumentationMustBeSpelledCorrectly",
                     Justification = "Example contains literals described in the specification.")]
    public class ProfileData : XliffElement, IExtensible
    {
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileData"/> class.
        /// </summary>
        /// <param name="profile">The normalization form to apply for general size restrictions.</param>
        public ProfileData(string profile)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.Profile = profile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileData"/> class.
        /// </summary>
        internal ProfileData()
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
        /// Gets or sets the normalization form to apply for general size restrictions.
        /// </summary>
        [SchemaEntity(AttributeNames.Profile, Requirement.Required)]
        public string Profile
        {
            get { return (string)this.GetPropertyValue(ProfileData.PropertyNames.Profile); }
            set { this.SetPropertyValue(value, ProfileData.PropertyNames.Profile); }
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
            /// The name of the <see cref="ProfileData.Profile"/> property.
            /// </summary>
            public const string Profile = "Profile";
        }
    }
}
