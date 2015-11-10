namespace Localization.Xliff.OM.Tests
{
    using System;
    using System.Collections.Generic;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="ParentAttachedList{TData}"/> class.
    /// </summary>
    [TestClass()]
    public class ParentAttachedListTests
    {
        private ParentAttachedList<XliffDocument> _list;
        private static readonly XliffDocument _parent = new XliffDocument();

        /// <summary>
        /// Initializes the test class before every test method is executed.
        /// </summary>
        [TestInitialize()]
        public void ParentAttachedList_Initialize()
        {
            this._list = new ParentAttachedList<XliffDocument>(ParentAttachedListTests._parent);
        }

        /// <summary>
        /// Tests the Add method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ParentAttachedList_Add()
        {
            XliffDocument item;

            //
            // Test with null.
            //

            try
            {
                this._list.Add(null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test with valid item1.
            //

            item = new XliffDocument();
            this._list.Add(item);
            Assert.IsNotNull(item.Parent, "Parent was not set.");
            Assert.AreEqual(ParentAttachedListTests._parent, item.Parent, "Parent was not set correctly.");

            //
            // Test with parent already set.
            //

            Assert.IsNotNull(item.Parent, "Parent should be set.");

            try
            {
                this._list.Add(item);
                Assert.Fail("Expected ElementReuseException to be thrown.");
            }
            catch (ElementReuseException)
            {
            }
        }

        /// <summary>
        /// Tests the Clear method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ParentAttachedList_Clear()
        {
            XliffDocument item;

            //
            // Test with empty list.
            //

            Assert.AreEqual(0, this._list.Count, "List is not empty.");
            this._list.Clear();
            Assert.AreEqual(0, this._list.Count, "List is not empty.");

            //
            // Test with valid item1.
            //

            item = new XliffDocument();
            this._list.Add(item);
            Assert.IsNotNull(item.Parent, "Parent was not set.");
            this._list.Clear();
            Assert.AreEqual(0, this._list.Count, "List is not empty.");
            Assert.IsNull(item.Parent, "Parent was not reset.");
        }

        /// <summary>
        /// Tests the Contains method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ParentAttachedList_Contains()
        {
            XliffDocument item;
            bool actualValue;

            //
            // Test with empty list.
            //

            actualValue = this._list.Contains(new XliffDocument());
            Assert.IsFalse(actualValue, "Contains returned wrong result.");

            //
            // Test with valid item1.
            //

            item = new XliffDocument();
            this._list.Add(item);
            actualValue = this._list.Contains(new XliffDocument());
            Assert.IsFalse(actualValue, "Contains returned wrong result.");
            actualValue = this._list.Contains(item);
            Assert.IsTrue(actualValue, "Contains didn't find the item.");
        }

        /// <summary>
        /// Tests the CopyTo method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ParentAttachedList_CopyTo()
        {
            XliffDocument item;
            XliffDocument[] array;

            array = new XliffDocument[3];

            //
            // Test with null array.
            //

            try
            {
                this._list.CopyTo(null, 0);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test with invalid index.
            //

            try
            {
                this._list.CopyTo(array, array.Length + 1);
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (ArgumentException)
            {
            }

            //
            // Test with empty list.
            //

            array[0] = null;
            this._list.CopyTo(array, 0);
            Assert.IsNull(array[0], "Array was updated.");

            //
            // Test with valid item1.
            //

            item = new XliffDocument();
            this._list.Add(item);
            this._list.CopyTo(array, 0);
            Assert.IsNotNull(array[0], "Array was not updated.");
            Assert.AreSame(item, array[0], "List was not copied.");

            //
            // Test with valid items.
            //

            item = new XliffDocument();
            this._list.Add(item);
            this._list.CopyTo(array, 1);
            Assert.IsNotNull(array[0], "Array[0] was not updated.");
            Assert.IsNotNull(array[1], "Array[1] was not updated.");
            Assert.IsNotNull(array[2], "Array[2] was not updated.");
            Assert.AreSame(array[0], array[1], "List was not copied at index 1.");
            Assert.AreSame(item, array[2], "Full list was not copied.");
        }

        /// <summary>
        /// Tests the Count property.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ParentAttachedList_Count()
        {
            XliffDocument item;

            //
            // Test with empty list.
            //

            Assert.AreEqual(0, this._list.Count);

            //
            // Test with valid items.
            //

            item = new XliffDocument();
            this._list.Add(item);
            Assert.AreEqual(1, this._list.Count);
            item = new XliffDocument();
            this._list.Add(item);
            item = new XliffDocument();
            this._list.Add(item);
            Assert.AreEqual(3, this._list.Count);
        }

        /// <summary>
        /// Tests the GetEnumerator method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ParentAttachedList_GetEnumerator()
        {
            IEnumerator<XliffDocument> enumerator;
            XliffDocument item;

            //
            // Test with empty list.
            //

            enumerator = this._list.GetEnumerator();
            Assert.IsNotNull(enumerator, "Enumerator is null.");
            Assert.IsFalse(enumerator.MoveNext(), "Enumerator should be empty.");

            //
            // Test with multiple items.
            //

            item = new XliffDocument();
            this._list.Add(item);
            item = new XliffDocument();
            this._list.Add(item);
            enumerator = this._list.GetEnumerator();
            Assert.IsNotNull(enumerator, "Enumerator is null.");
            Assert.IsTrue(enumerator.MoveNext(), "Enumerator should not be empty.");
            Assert.IsTrue(enumerator.MoveNext(), "Enumerator should not be empty.");
            Assert.AreSame(item, enumerator.Current, "Enumerator item is incorrect.");
            Assert.IsFalse(enumerator.MoveNext(), "Enumerator should be empty.");
        }

        /// <summary>
        /// Tests the indexer.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ParentAttachedList_Indexer()
        {
            XliffDocument item1;
            XliffDocument item2;

            //
            // Test get with invalid index.
            //

            try
            {
                item1 = this._list[0];
                Assert.Fail("Expected ArgumentOutOfRangeException to be thrown.");
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            //
            // Test set with invalid index.
            //

            item1 = new XliffDocument();

            try
            {
                this._list[0] = item1;
                Assert.Fail("Expected ArgumentOutOfRangeException to be thrown.");
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            this._list.Add(item1);

            //
            // Test set with null.
            //

            try
            {
                this._list[0] = null;
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test set with parent already set.
            //

            Assert.IsNotNull(item1.Parent, "Parent should be set.");

            try
            {
                this._list[0] = item1;
                Assert.Fail("Expected ElementReuseException to be thrown.");
            }
            catch (ElementReuseException)
            {
            }

            //
            // Test set by overwriting item1.
            //

            item2 = new XliffDocument();
            this._list[0] = item2;
            Assert.IsNull(item1.Parent, "Parent should be reset.");
            Assert.IsNotNull(item2.Parent, "Parent should be set.");

            //
            // Test that get works.
            //

            Assert.AreSame(item2, this._list[0], "Get didn't return the correct item.");
        }

        /// <summary>
        /// Tests the IndexOf method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ParentAttachedList_IndexOf()
        {
            XliffDocument item;

            //
            // Test with null.
            //

            try
            {
                this._list.IndexOf(null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test with empty list.
            //

            Assert.AreEqual(-1, this._list.IndexOf(new XliffDocument()), "IndexOf returned invalid index.");

            //
            // Test with invalid item1.
            //

            item = new XliffDocument();
            this._list.Add(item);
            Assert.AreEqual(-1, this._list.IndexOf(new XliffDocument()), "IndexOf returned invalid index.");

            //
            // Test with valid item1.
            //

            item = new XliffDocument();
            this._list.Add(item);
            Assert.AreEqual(1, this._list.IndexOf(item), "IndexOf returned invalid index.");
        }

        /// <summary>
        /// Tests the Insert method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ParentAttachedList_Insert()
        {
            XliffDocument item1;
            XliffDocument item2;

            //
            // Test with null.
            //

            try
            {
                this._list.Insert(0, null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test with invalid index.
            //

            try
            {
                this._list.Insert(10, new XliffDocument());
                Assert.Fail("Expected ArgumentOutOfRangeException to be thrown.");
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            //
            // Test with parent already set.
            //

            item1 = new XliffDocument();
            item1.Parent = ParentAttachedListTests._parent;
            try
            {
                this._list.Insert(0, item1);
                Assert.Fail("Expected ElementReuseException to be thrown.");
            }
            catch (ElementReuseException)
            {
            }

            //
            // Test with valid item1.
            //

            item1 = new XliffDocument();
            this._list.Insert(0, item1);
            Assert.AreEqual(1, this._list.Count, "List wasn't updated.");
            Assert.AreSame(item1, this._list[0], "Item wasn't added to list.");
            Assert.IsNotNull(item1.Parent, "Parent wasn't set.");

            //
            // Overwrite an item1.
            //

            item2 = new XliffDocument();
            this._list.Insert(0, item2);
            Assert.AreEqual(2, this._list.Count, "List wasn't updated.");
            Assert.AreSame(item2, this._list[0], "Item wasn't added to list.");
            Assert.IsNotNull(item2.Parent, "Parent wasn't set.");
            Assert.IsNotNull(item1.Parent, "Parent was modified.");
        }

        /// <summary>
        /// Tests the Remove method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ParentAttachedList_Remove()
        {
            XliffDocument item1;
            XliffDocument item2;

            //
            // Test with null.
            //

            try
            {
                this._list.Remove(null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }

            //
            // Test with empty list.
            //

            Assert.IsFalse(this._list.Remove(new XliffDocument()), "Remove returned incorrect result.");

            //
            // Test with invalid item1.
            //

            item1 = new XliffDocument();
            this._list.Add(item1);
            Assert.IsFalse(this._list.Remove(new XliffDocument()), "Remove returned incorrect result.");
            Assert.AreEqual(1, this._list.Count, "Count is incorrect.");

            //
            // Test with valid item1.
            //

            item2 = new XliffDocument();
            this._list.Add(item2);
            Assert.IsTrue(this._list.Remove(item1), "Remove returned incorrect result.");
            Assert.AreEqual(1, this._list.Count, "Count is incorrect.");
            Assert.AreSame(this._list[0], item2, "List was incorrectly modified.");
            Assert.IsNull(item1.Parent, "Parent was not reset.");
            Assert.IsNotNull(item2.Parent, "Parent was reset incorrectly.");
        }

        /// <summary>
        /// Tests the RemoveAt method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void ParentAttachedList_RemoveAt()
        {
            XliffDocument item1;
            XliffDocument item2;

            //
            // Test with empty list.
            //

            try
            {
                this._list.RemoveAt(0);
                Assert.Fail("Expected ArgumentOutOfRangeException to be thrown.");
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            //
            // Test with negative value.
            //

            item1 = new XliffDocument();
            this._list.Add(item1);

            try
            {
                this._list.RemoveAt(-1);
                Assert.Fail("Expected ArgumentOutOfRangeException to be thrown.");
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            //
            // Test with invalid index.
            //

            try
            {
                this._list.RemoveAt(this._list.Count);
                Assert.Fail("Expected ArgumentOutOfRangeException to be thrown.");
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            //
            // Test with valid index.
            //

            item2 = new XliffDocument();
            this._list.Add(item2);
            this._list.RemoveAt(0);
            Assert.AreEqual(1, this._list.Count, "Count is incorrect.");
            Assert.AreSame(this._list[0], item2, "List was incorrectly modified.");
            Assert.IsNull(item1.Parent, "Parent was not reset.");
            Assert.IsNotNull(item2.Parent, "Parent was reset incorrectly.");
        }
    }
}
