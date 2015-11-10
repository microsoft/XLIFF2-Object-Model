namespace Localization.Xliff.OM.Core
{
    using System.Collections.Generic;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class is used as a container of <see cref="Data"/>s. This corresponds to a &lt;originalData>
    /// element in the XLIFF 2.0 specification.
    /// </summary>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#originaldata">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="XliffElement"/>
    /// <seealso cref="ISelectNavigable"/>
    [SchemaChild(NamespaceValues.Core, ElementNames.Data, typeof(Data))]
    public class OriginalData : XliffElement, ISelectNavigable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OriginalData"/> class.
        /// </summary>
        public OriginalData()
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));

            this.DataElements = new ParentAttachedList<Data>(this);
        }

        #region Properties
        /// <summary>
        /// Gets the list of data associated with the object.
        /// </summary>
        public IList<Data> DataElements { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected override bool HasChildren
        {
            get { return base.HasChildren || this.HasData; }
        }

        /// <summary>
        /// Gets a value indicating whether the object contains data.
        /// </summary>
        public bool HasData
        {
            get { return (this.DataElements != null) && (this.DataElements.Count > 0); }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Adds data with the specified text.
        /// </summary>
        /// <param name="id">The Id of the data element to add.</param>
        /// <param name="text">The text of the data to add.</param>
        /// <returns>The newly created data.</returns>
        public Data AddData(string id, string text)
        {
            Data newData;

            newData = new Data(id, text);
            this.DataElements.Add(newData);

            return newData;
        }

        /// <summary>
        /// Selects an item matching the selection query.
        /// </summary>
        /// <param name="path">The selection query.</param>
        /// <returns>The object that was selected from the query path, or null if no match was found.</returns>
        /// <example>The value of <paramref name="path"/> might look something like "#g=group1/f=file1/u=unit1/d=data1"
        /// which is a relative path from the current object, not a full path from the document root.</example>
        public ISelectable Select(string path)
        {
            ArgValidator.Create(path, "path").IsNotNull().StartsWith(Utilities.Constants.SelectorPathIndictator);
            return this.SelectElement(Utilities.RemoveSelectorIndicator(path));
        }

        /// <summary>
        /// Gets the child <see cref="XliffElement"/>s contained within this object.
        /// </summary>
        /// <returns>A list of child elements stored as a pair consisting of the XLIFF Name for the child and
        /// the child itself.</returns>
        protected override List<ElementInfo> GetChildren()
        {
            List<ElementInfo> result;

            result = null;
            this.AddChildElementsToList(this.DataElements, ref result);

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
            if (child.Element is Data)
            {
                this.DataElements.Add((Data)child.Element);
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
