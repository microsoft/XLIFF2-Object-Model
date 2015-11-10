namespace Localization.Xliff.OM.Attributes
{
    using System;

    /// <summary>
    /// This class is used to define a converter type that can convert data from the native type to a string
    /// (and back) that is used with the XLIFF document.
    /// </summary>
    /// <seealso cref="Attribute"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConverterAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterAttribute"/> class.
        /// </summary>
        /// <param name="type">The type of the converter class.</param>
        public ConverterAttribute(Type type)
        {
            ArgValidator.Create(type, "type").IsNotNull();

            this.TypeName = type.FullName;
        }

        #region Properties
        /// <summary>
        /// Gets the type Name of the converter class.
        /// </summary>
        public string TypeName { get; private set; }
        #endregion Properties
    }
}
