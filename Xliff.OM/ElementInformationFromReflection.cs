namespace Localization.Xliff.OM
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class uses reflection to retrieve <see cref="XliffElement"/> child and attribute information.
    /// </summary>
    /// <seealso cref="IElementInformation"/>
    internal class ElementInformationFromReflection : IElementInformation
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="ElementInformationFromReflection"/> class from being created.
        /// </summary>
        private ElementInformationFromReflection()
        {
        }

        /// <summary>
        /// Gets the mapping of friendly name to attribute data.
        /// </summary>
        public IDictionary<string, AttributeData> AttributeMap { get; private set; }

        /// <summary>
        /// Gets the mapping of type to Xml information for all children of the element.
        /// </summary>
        public IDictionary<Type, XmlNameInfo> ChildMap { get; private set; }

        /// <summary>
        /// Creates an instance of this class and populates its members using reflection on the specified
        /// <see cref="XliffElement"/>
        /// </summary>
        /// <param name="element">The element to reflect upon.</param>
        /// <returns>An instance of this class with information about the elements children and attributes.</returns>
        public static IElementInformation Create(XliffElement element)
        {
            ElementInformationFromReflection result;

            result = new ElementInformationFromReflection();
            result.ChildMap = Reflector.GetSchemaChildren(element.GetType());
            result.AttributeMap = Reflector.GetSchemaAttributes(element.GetType(), element as IInheritanceInfoProvider, element as IOutputResolver);

            return result;
        }
    }
}
