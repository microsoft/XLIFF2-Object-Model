namespace Localization.Xliff.OM.Modules.Validation
{
    using System;
    using System.Collections.Generic;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.Validation.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents a parent container for a list of rules and constraints to apply to the target text of the
    /// enclosing element. This corresponds to a &lt;validation> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like:
    ///     &lt;validation>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#validation">XLIFF
    /// specification</a> for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IExtensible"/>
    [SchemaChild(
                 NamespacePrefixes.ValidationModule,
                 NamespaceValues.ValidationModule,
                 ElementNames.Rule,
                 typeof(Rule))]
    public class Validation : XliffElement, IExtensible
    {
        /// <summary>
        /// The list of extensions that store custom data.
        /// </summary>
        private readonly Lazy<List<IExtension>> extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Validation"/> class.
        /// </summary>
        public Validation()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.extensions = new Lazy<List<IExtension>>();
            this.Rules = new ParentAttachedList<Rule>(this);
        }

        #region Properties
        /// <summary>
        /// Gets the list of registered extensions on the object.
        /// </summary>
        IList<IExtension> IExtensible.Extensions
        {
            get { return this.extensions.Value; }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.Rules.Count > 0); }
        }

        /// <summary>
        /// Gets a value indicating whether extensions are registered on the object.
        /// </summary>
        bool IExtensible.HasExtensions
        {
            get { return this.extensions.IsValueCreated && (this.extensions.Value.Count > 0); }
        }

        /// <summary>
        /// Gets a list of rules and constraints to apply to the target text of the enclosing element.
        /// </summary>
        public IList<Rule> Rules { get; private set; }

        /// <summary>
        /// Gets a value indicating whether attribute extensions are supported by the object.
        /// </summary>
        bool IExtensible.SupportsAttributeExtensions
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether element extensions are supported by the object.
        /// </summary>
        bool IExtensible.SupportsElementExtensions
        {
            get { return false; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Gets the child <see cref="XliffElement"/>s contained within this object.
        /// </summary>
        /// <returns>A list of child elements stored as a pair consisting of the XLIFF Name for the child and
        /// the child itself.</returns>
        protected override List<ElementInfo> GetChildren()
        {
            List<ElementInfo> result;

            result = null;
            this.AddChildElementsToList(this.Rules, ref result);

            return result;
        }

        /// <summary>
        /// Stores the <see cref="XliffElement"/> as a child of this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>True if the child was stored, otherwise false.</returns>
        protected override bool StoreChild(ElementInfo child)
        {
            bool result;

            result = true;
            if (child.Element is Rule)
            {
                this.Rules.Add((Rule)child.Element);
            }
            else
            {
                result = base.StoreChild(child);
            }

            return result;
        }
        #endregion Methods
    }
}
