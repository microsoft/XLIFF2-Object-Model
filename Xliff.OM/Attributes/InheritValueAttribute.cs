namespace Localization.Xliff.OM.Attributes
{
    using System;

    /// <summary>
    /// This class is used to indicate a property has a value that may be inherited from an ancestor.
    /// </summary>
    /// <seealso cref="Attribute"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class InheritValueAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InheritValueAttribute"/> class.
        /// </summary>
        /// <param name="inheritance">The type of inheritance.</param>
        public InheritValueAttribute(Inheritance inheritance)
            : this(inheritance, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InheritValueAttribute"/> class.
        /// </summary>
        /// <param name="inheritance">The type of inheritance.</param>
        /// <param name="ancestorType">The type of the ancestor to inherit from.</param>
        public InheritValueAttribute(Inheritance inheritance, Type ancestorType)
            : this(inheritance, ancestorType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InheritValueAttribute"/> class.
        /// </summary>
        /// <param name="inheritance">The type of inheritance.</param>
        /// <param name="ancestorType">The type of the ancestor to inherit from.</param>
        /// <param name="propertyName">The name of the property to get from the ancestor.</param>
        public InheritValueAttribute(Inheritance inheritance, Type ancestorType, string propertyName)
        {
            if (inheritance == Inheritance.AncestorType)
            {
                ArgValidator.Create(ancestorType, "ancestorType").IsNotNull();
            }

            this.AncestorPropertyName = propertyName;
            this.AncestorType = ancestorType;
            this.Inheritance = inheritance;
        }

        #region Properties
        /// <summary>
        /// Gets the Name of the property to inherit from the ancestor.
        /// </summary>
        public string AncestorPropertyName { get; private set; }

        /// <summary>
        /// Gets the type of the ancestor to inherit from if the property doesn't have a value.
        /// </summary>
        public Type AncestorType { get; private set; }

        /// <summary>
        /// Gets the type of inheritance to apply if the property doesn't have a value.
        /// </summary>
        public Inheritance Inheritance { get; private set; }
        #endregion Properties
    }
}
