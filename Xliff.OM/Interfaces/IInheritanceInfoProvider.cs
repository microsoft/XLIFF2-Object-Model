namespace Localization.Xliff.OM
{
    /// <summary>
    /// This interface indicates that an object provides information on how to inherit property values.
    /// </summary>
    internal interface IInheritanceInfoProvider
    {
        /// <summary>
        /// Method called to provide custom inheritance information. This is typically used when the inheritance
        /// depends on runtime information.
        /// </summary>
        /// <param name="property">The name of the property being retrieved.</param>
        /// <returns>The value of the property.</returns>
        InheritanceInfo GetInheritanceInfo(string property);
    }
}
