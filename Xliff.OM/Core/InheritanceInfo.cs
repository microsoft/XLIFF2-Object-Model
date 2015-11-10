namespace Localization.Xliff.OM
{
    using System;

    /// <summary>
    /// This class contains information about how values are inherited.
    /// </summary>
    internal class InheritanceInfo
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InheritanceInfo"/> class.
        /// </summary>
        /// <param name="ancestorType">The type of the ancestor to inherit from if the property doesn't have
        /// a value.</param>
        /// <param name="propertyName">The name of the property to inherit from the ancestor.</param>
        public InheritanceInfo(Type ancestorType, string propertyName)
        {
            this.AncestorInheritanceType = ancestorType;
            this.AncestorPropertyName = propertyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InheritanceInfo"/> class.
        /// </summary>
        /// <param name="handler">A method to call to get a custom value.</param>
        public InheritanceInfo(InheritanceHandlerDelegate handler)
        {
            this.InheritanceHandler = handler;
        }
        #endregion Constructors

        /// <summary>
        /// The delegate used for getting custom values.
        /// </summary>
        /// <param name="element">The element the property is being returned for.</param>
        /// <param name="property">The name of the property whose value to return.</param>
        /// <returns>The custom value.</returns>
        public delegate object InheritanceHandlerDelegate(XliffElement element, string property);

        #region Properties
        /// <summary>
        /// Gets the type of the ancestor to inherit from if the property doesn't have a value.
        /// </summary>
        public Type AncestorInheritanceType { get; private set; }

        /// <summary>
        /// Gets the name of the property to inherit from the ancestor.
        /// </summary>
        public string AncestorPropertyName { get; private set; }

        /// <summary>
        /// Gets a method to call to get a custom value.
        /// </summary>
        public InheritanceHandlerDelegate InheritanceHandler { get; private set; }
        #endregion Properties
    }
}
