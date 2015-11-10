namespace Localization.Xliff.OM.Modules.Metadata
{
    /// <summary>
    /// Defines the element to which the content of the <see cref="MetaGroup"/> applies.
    /// </summary>
    public enum MetaGroupSubject
    {
        /// <summary>
        /// <see cref="MetaGroup"/> applies to an <see cref="Ignorable"/>.
        /// </summary>
        Ignorable,

        /// <summary>
        /// <see cref="MetaGroup"/> applies to an <see cref="Source"/>.
        /// </summary>
        Source,

        /// <summary>
        /// <see cref="MetaGroup"/> applies to an <see cref="Target"/>.
        /// </summary>
        Target
    }
}
