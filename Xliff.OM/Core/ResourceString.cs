namespace Localization.Xliff.OM.Core
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents the text within translatable or translated content. (namely <see cref="Source"/> and
    /// <see cref="Target"/>).
    /// </summary>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IResourceStringContentContainer"/>
    /// <seealso cref="ISelectNavigable"/>
    [SchemaChild(NamespaceValues.Core, ElementNames.CDataTag, typeof(CDataTag))]
    [SchemaChild(NamespaceValues.Core, ElementNames.CodePoint, typeof(CodePoint))]
    [SchemaChild(NamespaceValues.Core, ElementNames.CommentTag, typeof(CommentTag))]
    [SchemaChild(NamespaceValues.Core, ElementNames.MarkedSpan, typeof(MarkedSpan))]
    [SchemaChild(NamespaceValues.Core, ElementNames.MarkedSpanEnd, typeof(MarkedSpanEnd))]
    [SchemaChild(NamespaceValues.Core, ElementNames.MarkedSpanStart, typeof(MarkedSpanStart))]
    [SchemaChild(NamespaceValues.Core, ElementNames.PlainText, typeof(PlainText))]
    [SchemaChild(NamespaceValues.Core, ElementNames.ProcessingInstructionTag, typeof(ProcessingInstructionTag))]
    [SchemaChild(NamespaceValues.Core, ElementNames.SpanningCode, typeof(SpanningCode))]
    [SchemaChild(NamespaceValues.Core, ElementNames.SpanningCodeEnd, typeof(SpanningCodeEnd))]
    [SchemaChild(NamespaceValues.Core, ElementNames.SpanningCodeStart, typeof(SpanningCodeStart))]
    [SchemaChild(NamespaceValues.Core, ElementNames.StandaloneCode, typeof(StandaloneCode))]
    public abstract class ResourceString : XliffElement, IResourceStringContentContainer, ISelectNavigable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceString"/> class.
        /// </summary>
        protected ResourceString()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.Text = new ParentAttachedList<ResourceStringContent>(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceString"/> class.
        /// </summary>
        /// <param name="text">Plain text to add.</param>
        protected ResourceString(string text)
            : this()
        {
            this.Text.Add(new PlainText(text));
        }

        #region Properties
        /// <summary>
        /// Gets a value that is used as a prefix on the SelectorId for children.
        /// </summary>
        internal virtual string ChildSelectorIdPrefix
        {
            get { return null; }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.Text.Count > 0); }
        }

        /// <summary>
        /// Gets or sets the language the text is written in.
        /// </summary>
        [InheritValue(Inheritance.Callback)]
        [SchemaEntity(NamespacePrefixes.Xml, null, AttributeNames.Language, Requirement.Optional)]
        public string Language
        {
            get { return (string)this.GetPropertyValue(ResourceString.PropertyNames.Language); }
            set { this.SetPropertyValue(value, ResourceString.PropertyNames.Language); }
        }

        /// <summary>
        /// Gets the elements that describe the text.
        /// </summary>
        public IList<ResourceStringContent> Text { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating how to handle whitespace.
        /// </summary>
        [Converter(typeof(PreservationConverter))]
        [DefaultValue(Preservation.Default)]
        [InheritValue(Inheritance.Parent)]
        [SchemaEntity(NamespacePrefixes.Xml, null, AttributeNames.SpacePreservation, Requirement.Optional)]
        public Preservation Space
        {
            get { return (Preservation)this.GetPropertyValue(ResourceString.PropertyNames.Space); }
            set { this.SetPropertyValue(value, ResourceString.PropertyNames.Space); }
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
            this.AddChildElementsToList(this.Text, ref result);

            return result;
        }

        /// <summary>
        /// Selects an item matching the selection query.
        /// </summary>
        /// <param name="path">The selection query.</param>
        /// <returns>The object that was selected from the query path, or null if no match was found.</returns>
        /// <example>The value of <paramref name="path"/> might look something like "#g=group1/f=file1/u=unit1/n=note1"
        /// which is a relative path from the current object, not a full path from the document root.</example>
        public ISelectable Select(string path)
        {
            ArgValidator.Create(path, "path").IsNotNull().StartsWith(Utilities.Constants.SelectorPathIndictator);
            return this.SelectElement(Utilities.RemoveSelectorIndicator(path));
        }

        /// <summary>
        /// Sets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="text">The text to set.</param>
        protected override void SetInnerText(string text)
        {
            this.Text.Add(new PlainText(text));
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
            if (child.Element is ResourceStringContent)
            {
                this.Text.Add((ResourceStringContent)child.Element);
            }
            else
            {
                result = base.StoreChild(child);
            }

            return result;
        }
        #endregion Methods

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="ResourceString.Language"/> property.
            /// </summary>
            public const string Language = "Language";

            /// <summary>
            /// The name of the <see cref="ResourceString.Space"/> property.
            /// </summary>
            public const string Space = "Space";
        }
    }
}
