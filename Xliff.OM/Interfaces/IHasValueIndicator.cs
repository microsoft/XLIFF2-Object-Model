namespace Localization.Xliff.OM
{
    /// <summary>
    /// This interface defines indicators that inspect values and determines whether they have data or not. This is
    /// used to determine whether data is present and can be compared to a default value.
    /// </summary>
    internal interface IHasValueIndicator
    {
        /// <summary>
        /// This method returns true of the specified value has something that indicates it has data set by a user or
        /// code. It returns false if the value appears to not have any data set.
        /// </summary>
        /// <param name="value">The value to inspect.</param>
        /// <returns>True if the value contains set data, otherwise false.</returns>
        bool HasValue(object value);
    }
}
