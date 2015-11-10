namespace Localization.Xliff.OM.Modules.ResourceData
{
    using System;
    using System.Collections.Generic;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.ResourceData.XmlNames;

    /// <summary>
    /// This class represents a reference to an associated <see cref="ResourceItem"/> element located at the 
    /// <see cref="Localization.Xliff.OM.Core.File"/> level. This corresponds to a &lt;resourceItemRef> element
    /// in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;resourceItemRef [id=string]
    ///                         ref=string ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#res_resourceItemRef">
    /// XLIFF specification</a> for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    /// <seealso cref="ISelectable"/>
    public class ResourceItemRef : XliffElement, IExtensible, ISelectable
    {
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceItemRef"/> class.
        /// </summary>
        /// <param name="reference">A reference to an associated <see cref="ResourceItem"/> element located at the
        /// <see cref="Localization.Xliff.OM.Core.File"/> level.</param>
        public ResourceItemRef(string reference)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.Reference = reference;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceItemRef"/> class.
        /// </summary>
        internal ResourceItemRef()
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
        /// Gets or sets the Id of the resource item reference.
        /// </summary>
        [SchemaEntity(AttributeNames.Id, Requirement.Optional)]
        public string Id
        {
            get { return (string)this.GetPropertyValue(ResourceItemRef.PropertyNames.Id); }
            set { this.SetPropertyValue(value, ResourceItemRef.PropertyNames.Id); }
        }

        /// <summary>
        /// Gets a value indicating whether the element represents a leaf fragment in a selector path. If so, the
        /// selector path shouldn't contain any other fragments after this fragment.
        /// </summary>
        public bool IsLeafFragment
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets a reference to an associated <see cref="ResourceItem"/> element located at the
        /// <see cref="Localization.Xliff.OM.Core.File"/> level.
        /// </summary>
        [SchemaEntity(AttributeNames.ReferenceAbbreviated, Requirement.Required)]
        public string Reference
        {
            get { return (string)this.GetPropertyValue(ResourceItemRef.PropertyNames.Reference); }
            set { this.SetPropertyValue(value, ResourceItemRef.PropertyNames.Reference); }
        }

        /// <summary>
        /// Gets the selector Id of the item.
        /// </summary>
        /// <example>For a <see cref="ResourceItemRef"/> item, this value might look like "res=ref1" where "ref1" is the Id.
        /// </example>
        public string SelectorId
        {
            get { return Utilities.CreateSelectorId(ResourceItemRef.Constants.SelectorPrefix, this.Id); }
        }

        /// <summary>
        /// Gets the full path of the item from the root document.
        /// </summary>
        public string SelectorPath
        {
            get { return this.BuildSelectorPath(); }
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
        /// This class contains constant values that are used in this class.
        /// </summary>
        private static class Constants
        {
            /// <summary>
            /// The selector path prefix for this element.
            /// </summary>
            public const string SelectorPrefix = "res";
        }

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="ResourceItemRef.Id"/> property.
            /// </summary>
            public const string Id = "Id";

            /// <summary>
            /// The name of the <see cref="ResourceItemRef.Reference"/> property.
            /// </summary>
            public const string Reference = "Reference";
        }
    }
}
