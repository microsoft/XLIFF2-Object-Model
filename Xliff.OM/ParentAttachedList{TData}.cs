namespace Localization.Xliff.OM
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// This class is used to associate <see cref="XliffElement"/>s with owners of lists that contain them.
    /// </summary>
    /// <typeparam name="TData">The type of data stored in the list.</typeparam>
    /// <remarks>
    /// This list should check that the items aren't re-used.
    /// ex. doc.Files.Add(foo), doc.Files.Add(foo)
    /// ex. doc.Files.Unit.Segments.Add(foo), doc.Files.Group[0].Unit.Segments.Add(foo).</remarks>
    /// <seealso cref="XliffElement"/>
    internal class ParentAttachedList<TData> : IList<TData> where TData : XliffElement
    {
        #region Member Variables
        /// <summary>
        /// The list that stores actual data.
        /// </summary>
        private readonly List<TData> list;

        /// <summary>
        /// The object that owns the list. All items stored in the list will be attached to the parent.
        /// </summary>
        private XliffElement parent;
        #endregion Member Variables

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentAttachedList{TData}"/> class.
        /// </summary>
        /// <param name="parent">The object that acts as the parent of all items in the list.</param>
        public ParentAttachedList(XliffElement parent)
        {
            Debug.Assert(parent != null, "Parent should not be null.");

            this.list = new List<TData>();
            this.parent = parent;
        }

        #region Properties
        /// <summary>
        /// Gets the number of elements contained in the list.
        /// </summary>
        public int Count
        {
            get { return this.list.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the list is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return ((IList<TData>)this.list).IsReadOnly; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public TData this[int index]
        {
            get
            {
                return this.list[index];
            }

            set
            {
                ArgValidator.Create(value, "value").IsNotNull();
                ArgValidator.ParentIsNull(value);
                this.list[index].Parent = null;
                this.list[index] = value;
                this.list[index].Parent = this.parent;
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="item">The object to add to the list.</param>
        public void Add(TData item)
        {
            ArgValidator.Create(item, "item").IsNotNull();
            ArgValidator.ParentIsNull(item);
            this.list.Add(item);
            item.Parent = this.parent;
        }

        /// <summary>
        /// Removes all items from the list.
        /// </summary>
        public void Clear()
        {
            foreach (TData item in this.list)
            {
                item.Parent = null;
            }

            this.list.Clear();
        }

        /// <summary>
        /// Determines whether the list contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the list.</param>
        /// <returns>True if item is found in the list, otherwise false.</returns>
        public bool Contains(TData item)
        {
            return this.list.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the list to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from
        /// the list. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(TData[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<TData> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <summary>
        /// Determines the index of a specific item in the list.
        /// </summary>
        /// <param name="item">The object to locate in the list.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(TData item)
        {
            ArgValidator.Create(item, "item").IsNotNull();
            return this.list.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the list.</param>
        public void Insert(int index, TData item)
        {
            ArgValidator.Create(item, "item").IsNotNull();
            ArgValidator.ParentIsNull(item);
            this.list.Insert(index, item);
            item.Parent = this.parent;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the list.
        /// </summary>
        /// <param name="item">The object to remove from the list.</param>
        /// <returns>True if item was successfully removed from the list, otherwise false. This method also
        /// returns false if item is not found in the list.</returns>
        public bool Remove(TData item)
        {
            bool result;

            ArgValidator.Create(item, "item").IsNotNull();

            result = this.list.Remove(item);
            if (result)
            {
                item.Parent = null;
            }

            return result;
        }

        /// <summary>
        /// Removes the list item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            if ((index >= 0) && (this.list.Count > index))
            {
                this.list[index].Parent = null;
            }

            this.list.RemoveAt(index);
        }        
        #endregion Methods

        #region IEnumerable Implementation
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)this.list).GetEnumerator();
        }
        #endregion IEnumerable Implementation
    }
}
