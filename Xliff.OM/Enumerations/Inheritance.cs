namespace Localization.Xliff.OM
{
    /// <summary>
    /// Defines how values are inherited.
    /// </summary>
    public enum Inheritance
    {
        /// <summary>
        /// The value should be obtained from calling a callback method.
        /// </summary>
        Callback,

        /// <summary>
        /// The value is inherited from the parent.
        /// </summary>
        Parent,

        /// <summary>
        /// The value is inherited from the first ancestor matching a specific type.
        /// </summary>
        AncestorType
    }
}
