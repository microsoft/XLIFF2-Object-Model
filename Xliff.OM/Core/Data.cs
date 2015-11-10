namespace Localization.Xliff.OM.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents the original data of an inline code. This corresponds to a &lt;data> element in the
    /// XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: &lt;data>text&lt;/data></example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#data">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="IResourceStringContentContainer"/>
    /// <seealso cref="ISelectable"/>
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
    public class Data : XliffElement, IResourceStringContentContainer, ISelectable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Data"/> class.
        /// </summary>
        public Data()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Data"/> class.
        /// </summary>
        /// <param name="id">The Id of the data.</param>
        public Data(string id)
            : this(id, (string)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Data"/> class.
        /// </summary>
        /// <param name="id">The Id of the data.</param>
        /// <param name="contents">The original text to assign to Text.</param>
        public Data(string id, IEnumerable<ResourceStringContent> contents)
            : this(id, (string)null)
        {
            if (contents != null)
            {
                foreach (ResourceStringContent data in contents)
                {
                    this.Text.Add(data);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Data"/> class.
        /// </summary>
        /// <param name="id">The Id of the data.</param>
        /// <param name="text">The non-translatable text.</param>
        public Data(string id, string text)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.Id = id;
            this.Text = new ParentAttachedList<ResourceStringContent>(this);

            if (text != null)
            {
                this.Text.Add(new PlainText(text));
            }
        }

        #region Properties
        /// <summary>
        /// Gets or sets the directionality of the content.
        /// </summary>
        [Converter(typeof(ContentDirectionalityConverter))]
        [DefaultValue(ContentDirectionality.Auto)]
        [SchemaEntity(AttributeNames.Directionality, Requirement.Optional)]
        public ContentDirectionality Directionality
        {
            get { return (ContentDirectionality)this.GetPropertyValue(Data.PropertyNames.Directionality); }
            set { this.SetPropertyValue(value, Data.PropertyNames.Directionality); }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || (this.Text.Count > 0); }
        }

        /// <summary>
        /// Gets or sets the Id of the data.
        /// </summary>
        [SchemaEntity(AttributeNames.Id, Requirement.Required)]
        public string Id
        {
            get
            {
                return (string)this.GetPropertyValue(Data.PropertyNames.Id);
            }

            set
            {
                value = string.IsNullOrEmpty(value) ? null : value;
                ArgValidator.Create(value, Data.PropertyNames.Id).IsValidId(this);
                this.SetPropertyValue(value, Data.PropertyNames.Id);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the element represents a leaf fragment in a selector path. If so, the
        /// selector path shouldn't contain any other fragments after this fragment.
        /// </summary>
        public bool IsLeafFragment
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the selector Id of the item.
        /// </summary>
        /// <example>For a <see cref="Data"/> item, this value might look like "d=data1" where "data1" is the Id.
        /// </example>
        public string SelectorId
        {
            get { return Utilities.CreateSelectorId(Data.Constants.SelectorPrefix, this.Id); }
        }

        /// <summary>
        /// Gets the full path of the item from the root document.
        /// </summary>
        public string SelectorPath
        {
            get { return this.BuildSelectorPath(); }
        }

        /// <summary>
        /// Gets or sets a value indicating how to handle whitespace.
        /// </summary>
        [Converter(typeof(PreservationConverter))]
        [DefaultValue(Preservation.Preserve)]
        [SchemaEntity(NamespacePrefixes.Xml, null, AttributeNames.SpacePreservation, Requirement.Optional)]
        public Preservation Space
        {
            get
            {
                return (Preservation)this.GetPropertyValue(Data.PropertyNames.Space);
            }

            set
            {
                if (value != Preservation.Preserve)
                {
                    throw new ArgumentException(Properties.Resources.Data_SpaceRestriction, "value");
                }

                this.SetPropertyValue(value, Data.PropertyNames.Space);
            }
        }

        /// <summary>
        /// Gets the original text.
        /// </summary>
        public IList<ResourceStringContent> Text { get; private set; }
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
        /// This class contains constant values that are used in this class.
        /// </summary>
        public static class Constants
        {
            /// <summary>
            /// The selector path prefix for this element.
            /// </summary>
            public const string SelectorPrefix = "d";
        }

        /// <summary>
        /// This class contains the names of properties in this class.
        /// </summary>
        private static class PropertyNames
        {
            /// <summary>
            /// The name of the <see cref="Data.Directionality"/> property.
            /// </summary>
            public const string Directionality = "Directionality";

            /// <summary>
            /// The name of the <see cref="Data.Id"/> property.
            /// </summary>
            public const string Id = "Id";

            /// <summary>
            /// The name of the <see cref="Data.Space"/> property.
            /// </summary>
            public const string Space = "Space";
        }
    }
}
