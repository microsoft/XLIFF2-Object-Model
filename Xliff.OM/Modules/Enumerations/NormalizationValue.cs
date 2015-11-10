namespace Localization.Xliff.OM.Modules
{
    /// <summary>
    /// Defines the values that indicate the unicode normalization form.
    /// </summary>
    /// <remarks>Refer to <a href="http://www.unicode.org/reports/tr15/">Unicode Normalization Forms</a> for more
    /// information.</remarks>
    public enum NormalizationValue
    {
        /// <summary>
        /// Indicates no normalization should be done.
        /// </summary>
        None,

        /// <summary>
        /// Indicates Normalization Form C (Canonical Decomposition, followed by Canonical Composition) must be used.
        /// </summary>
        NFC,

        /// <summary>
        /// Indicates Normalization Form D (Canonical Decomposition) must be used.
        /// </summary>
        NFD
    }
}
