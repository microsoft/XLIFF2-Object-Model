namespace Localization.Xliff.OM.Modules.SizeRestriction
{
    /// <summary>
    /// This interface provides a mechanism to annotate the XLIFF content with information on storage and general size
    /// restrictions.
    /// </summary>
    public interface ISizeRestrictionAttributes
    {
        /// <summary>
        /// Gets or sets a means to specify how much storage space an inline element will use in the native format.
        /// This size contribution is then added to the size contributed by the textual parts. Interpretation of the
        /// value is dependent on the selected <see cref="Profiles.StorageProfile"/>. It must represent the equivalent
        /// storage size represented by the inline element.
        /// </summary>
        string EquivalentStorage { get; set; }

        /// <summary>
        /// Gets or sets profile specific information to inline elements so that size information can be decoupled from
        /// the native format or represented when the native data is not available in the XLIFF document.
        /// Interpretation of the value is dependent on selected <see cref="Profiles.GeneralProfile"/>. It must
        /// represent information related to how the element it is attached to contributes to the size of the text or
        /// entity in which it occurs or represents.
        /// </summary>
        string SizeInfo { get; set; }

        /// <summary>
        /// Gets or sets a reference to data that provide the same information that could be otherwise put in a sizeInfo
        /// attribute. The reference must point to an element in a <see cref="ProfileData"/> element that is a sibling
        /// to the element this attribute is attached to or a sibling to one of its ancestors. 
        /// </summary>
        string SizeInfoReference { get; set; }

        /// <summary>
        /// Gets or sets the size restriction to apply to the collection descendants of the element it is defined on.
        /// Interpretation of the value is dependent on the selected <see cref="Profiles.GeneralProfile"/>. It must
        /// represent the restriction to apply to the indicated sub part of the document.
        /// </summary>
        string SizeRestriction { get; set; }

        /// <summary>
        /// Gets or sets the storage restriction to apply to the collection descendants of the element it is defined on.
        /// Interpretation of the value is dependent on the selected <see cref="Profiles.StorageProfile"/>. It must
        /// represent the restriction to apply to the indicated sub part of the document.
        /// </summary>
        string StorageRestriction { get; set; }

        /// <summary>
        /// Gets a value indicating whether the object supports the EquivalentStorage property.
        /// </summary>
        bool SupportsEquivalentStorage { get; }

        /// <summary>
        /// Gets a value indicating whether the object supports the SizeInfo property.
        /// </summary>
        bool SupportsSizeInfo { get; }

        /// <summary>
        /// Gets a value indicating whether the object supports the SizeInfoReference property.
        /// </summary>
        bool SupportsSizeInfoReference { get; }

        /// <summary>
        /// Gets a value indicating whether the object supports the SizeRestriction property.
        /// </summary>
        bool SupportsSizeRestriction { get; }

        /// <summary>
        /// Gets a value indicating whether the object supports the StorageRestriction property.
        /// </summary>
        bool SupportsStorageRestriction { get; }
    }
}
