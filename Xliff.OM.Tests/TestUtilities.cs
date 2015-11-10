namespace Localization.Xliff.OM.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Serialization;
    using Localization.Xliff.OM.Serialization.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class contains utility methods for tests.
    /// </summary>
    internal static class TestUtilities
    {
        /// <summary>
        /// Test category for identifying unit tests.
        /// </summary>
        public const string UnitTestCategory = "Unit Test";

        /// <summary>
        /// The relative directory where test data files are located.
        /// </summary>
        public const string TestDataDirectory = "TestData.Xliff.OM.Tests";

        /// <summary>
        /// Deserializes a file and stores the resulting <see cref="XliffDocument"/> internally.
        /// </summary>
        /// <param name="data">The identifier of the document to deserialize.</param>
        /// <param name="validate">If true the document will be validated, otherwise it won't be validated.</param>
        /// <param name="handlers">The extension handlers to register.</param>
        /// <returns>The deserialized document.</returns>
        public static XliffDocument Deserialize(TestData data, bool validate, Dictionary<string, IExtensionHandler> handlers)
        {
            XliffDocument result;
            XliffReader reader;
            string path;

            if (validate)
            {
                reader = new XliffReader();
            }
            else
            {
                XliffReaderSettings settings;

                settings = new XliffReaderSettings();
                settings.Validators.Clear();
                reader = new XliffReader(settings);
            }

            path = Path.Combine(Environment.CurrentDirectory, TestUtilities.TestDataDirectory, data.ToString() + ".xlf");
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                if (handlers != null)
                {
                    foreach (string ns in handlers.Keys)
                    {
                        reader.RegisterExtensionHandler(ns, handlers[ns]);
                    }
                }

                result = reader.Deserialize(stream);
            }

            Assert.IsNotNull(result, "Failed to deserialize.");

            return result;
        }
        
        /// <summary>
        /// Gets the contents of a document as a serialized string.
        /// </summary>
        /// <param name="document">The document whose contents to get.</param>
        /// <param name="indentChars">Characters to use to indent the serialized document for readability.</param>
        /// <returns>The string representation of the serialized document.</returns>
        public static string GetDocumentContents(XliffDocument document, string indentChars)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                StreamReader reader;
                XliffWriter writer;
                
                if (indentChars == null)
                {
                    writer = new XliffWriter();
                }
                else
                {
                    XliffWriterSettings settings;

                    settings = new XliffWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = (indentChars == string.Empty) ? "  " : indentChars;
                    
                    writer = new XliffWriter(settings);
                }

                writer.Serialize(stream, document);

                stream.Seek(0, SeekOrigin.Begin);
                reader = new StreamReader(stream);

                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Reads the contents of a file and returns it as a string.
        /// </summary>
        /// <param name="data">The enumeration value that represents the file.</param>
        /// <returns>The contents of the file.</returns>
        public static string GetFileContents(TestData data)
        {
            string path;

            path = Path.Combine(Environment.CurrentDirectory, TestUtilities.TestDataDirectory, data.ToString() + ".xlf");
            return TestUtilities.GetFileContents(path);
        }

        /// <summary>
        /// Reads the contents of a file and returns it as a string.
        /// </summary>
        /// <param name="path">The file to read.</param>
        /// <returns>The contents of the file.</returns>
        public static string GetFileContents(string path)
        {
            using (TextReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Verifies that the expected item is present and matches a set of items obtained from
        /// another target. The <typeparamref name="TItem"/> represents the type of items to compare from
        /// the enumerations. Only items of these types are compared. All other types are ignored.
        /// </summary>
        /// <typeparam name="TItem">The type of items to extract from <paramref name="expectedItem"/>
        /// and <paramref name="actualItems"/>.</typeparam>
        /// <param name="expectedItem">The expected item.</param>
        /// <param name="actualItems">The items to compare against the expected.</param>
        /// <param name="name">The XLIFF name the keys of the <paramref name="actualItems"/> should match.</param>
        public static void VerifyItems<TItem>(XliffElement expectedItem,
                                              IEnumerable<ElementInfo> actualItems,
                                              string name) where TItem : XliffElement
        {
            TestUtilities.VerifyItems<TItem>(new XliffElement[] { expectedItem }, actualItems, name);
        }

        /// <summary>
        /// Verifies that the expected items are all present and match a set of items obtained from
        /// another target. The <typeparamref name="TItem"/> represents the type of items to compare from
        /// the enumerations. Only items of these types are compared. All other types are ignored.
        /// </summary>
        /// <typeparam name="TItem">The type of items to extract from <paramref name="expectedItem"/>
        /// and <paramref name="actualItems"/>.</typeparam>
        /// <param name="expectedItems">The expected items.</param>
        /// <param name="actualItems">The items to compare against the expected.</param>
        /// <param name="name">The XLIFF name the keys of the <paramref name="actualItems"/> should match.</param>
        public static void VerifyItems<TItem>(IEnumerable<XliffElement> expectedItems,
                                              IEnumerable<ElementInfo> actualItems,
                                              string name) where TItem : XliffElement
        {
            TestUtilities.VerifyItems(expectedItems.OfType<TItem>(),
                                 actualItems.Where(e => e.Element.GetType() == typeof(TItem)).ToList(),
                                 name);
        }

        /// <summary>
        /// Verifies that the expected items are all present and match a set of items obtained from
        /// another target.
        /// </summary>
        /// <param name="expectedItems">The expected items.</param>
        /// <param name="actualItems">The items to compare against the expected.</param>
        /// <param name="name">The XLIFF name the keys of the <paramref name="actualItems"/> should match.</param>
        public static void VerifyItems(IEnumerable<XliffElement> expectedItems,
                                       IList<ElementInfo> actualItems,
                                       string name)
        {
            int index;

            Assert.IsNotNull(actualItems, "items is null.");
            Assert.IsTrue(actualItems.Count > 0, "No items were returned.");

            index = 0;
            foreach (object expecteditem in expectedItems)
            {
                XliffElement item;

                Console.WriteLine("Verifying item {0}", index);
                item = actualItems[index].Element;
                Assert.IsNotNull(item, "item is not a File ({1}).", item.GetType().Name);
                Assert.AreEqual(name, actualItems[index].LocalName, "LocalName is incorrect.");
                Assert.AreSame(expecteditem, item, "item doesn't match File element.");

                index++;
            }

            Assert.AreEqual(index, actualItems.Count, "Number of items is incorrect.");
        }

        /// <summary>
        /// Verifies that the expected items are all present and match a set of items obtained from
        /// another target. The <paramref name="expectedItem"/> are expected to be of type <see cref="Note"/> and
        /// the type stored in <paramref name="actualItems"/> is a <see cref="NoteContainer"/>. The container
        /// notes are then compared with the <paramref name="expectedItem"/> items.
        /// </summary>
        /// <param name="expectedItems">The expected items.</param>
        /// <param name="actualItems">The items to compare against the expected.</param>
        /// <param name="name">The XLIFF name the keys of the <paramref name="actualItems"/> should match.</param>
        public static void VerifyNoteContainerItems(IEnumerable<Note> expectedItems,
                                                    IList<ElementInfo> actualItems,
                                                    string name)
        {
            int index;

            Assert.IsNotNull(actualItems, "items is null.");
            actualItems = actualItems.Where(e => e.Element.GetType() == typeof(NoteContainer)).ToList();

            Assert.AreEqual(1, actualItems.Count, "item count is incorrect.");
            Assert.AreEqual(name, actualItems[0].LocalName, "LocalName is incorrect.");
            Assert.AreEqual(typeof(NoteContainer), actualItems[0].Element.GetType(), "item is not a NoteContainer.");

            index = 0;
            foreach (object expecteditem in expectedItems)
            {
                NoteContainer item;

                Console.WriteLine("Verifying item {0}", index);
                item = actualItems[0].Element as NoteContainer;
                Assert.IsNotNull(item, "item is not a File ({1}).", item.GetType().Name);
                Assert.AreSame(expecteditem, item.Notes[index], "item doesn't match File element.");

                index++;
            }
        }
    }
}
