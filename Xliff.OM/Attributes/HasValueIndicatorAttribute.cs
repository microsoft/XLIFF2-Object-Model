namespace Localization.Xliff.OM.Attributes
{
    using System;

    /// <summary>
    /// This class is used to indicate a property has a handler that should be used to determine if data was set.
    /// </summary>
    /// <seealso cref="Attribute"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class HasValueIndicatorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HasValueIndicatorAttribute"/> class.
        /// </summary>
        /// <param name="type">The type of the indicator class.</param>
        public HasValueIndicatorAttribute(Type type)
        {
            ArgValidator.Create(type, "type").IsNotNull();

            this.TypeName = type.FullName;
        }

        #region Properties
        /// <summary>
        /// Gets the type name of the indicator class.
        /// </summary>
        public string TypeName { get; private set; }
        #endregion Properties
    }
}
