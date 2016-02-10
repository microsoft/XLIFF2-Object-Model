namespace Localization.Xliff.OM.Attributes
{
    using System;

    /// <summary>
    /// This class is used to indicate a property must be output when another property is output.
    /// </summary>
    /// <seealso cref="Attribute"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ExplicitOutputDependencyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplicitOutputDependencyAttribute"/> class.
        /// </summary>
        /// <remarks>This constructor is used to indicate that the hosting type implements an IOutputResolver that
        /// should be used to determine whether the property should be output explicitly.</remarks>
        public ExplicitOutputDependencyAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplicitOutputDependencyAttribute"/> class.
        /// </summary>
        /// <param name="property">The name of the property the attributed property depends on.</param>
        public ExplicitOutputDependencyAttribute(string property)
        {
            ArgValidator.Create(property, "property").IsNotNull();

            this.Property = property;
        }

        #region Properties
        /// <summary>
        /// Gets the name of the property the attributed property depends on.
        /// </summary>
        public string Property { get; private set; }
        #endregion Properties
    }
}
