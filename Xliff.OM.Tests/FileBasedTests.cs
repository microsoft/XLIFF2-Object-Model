namespace Localization.Xliff.OM.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Exceptions;
    using Localization.Xliff.OM.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the default values for all <see cref="XliffElement"/> classes.
    /// </summary>
    [DeploymentItem(FileBasedTests.DataDrivenTestFile)]
    [DeploymentItem(FileBasedTests.TestFileSourceDirectory, FileBasedTests.TestFileSourceDirectory)]
    [TestClass()]
    public class FileBasedTests
    {
        private const string DataDrivenDataSource = "Microsoft.VisualStudio.TestTools.DataSource.XML";
        private const string DataDrivenTestFile = "FileBasedTests.xml";
        private const string TestEntryElementName = "Row";
        private const string TestEntryErrorAttribute = "errorNumber";
        private const string TestEntryPathAttribute = "path";
        private const string TestEntryResultAttribute = "expectedResult";
        private const string TestEntryXPath = "/TestData/Row";
        private const string TestFileSourceDirectory = "Oasis_TestFiles";
        private const string XliffFileExtension = "*.xlf";

        public TestContext TestContext { get; set; }

        #region Test Methods
        /// <summary>
        /// Tests that defaults for <see cref="Data"/> are correct.
        /// </summary>
        [DataSource(
                    FileBasedTests.DataDrivenDataSource,
                    FileBasedTests.DataDrivenTestFile,
                    FileBasedTests.TestEntryElementName,
                    DataAccessMethod.Sequential)]       
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Xliff_DataDriven()
        {
            string path;
            ExpectedResult expectedResult;
            int errorNumber;

            errorNumber = -1;
            path = (string)this.TestContext.DataRow[FileBasedTests.TestEntryPathAttribute];
            expectedResult = (ExpectedResult)Enum.Parse(
                                                        typeof(ExpectedResult),
                                                        (string)this.TestContext.DataRow[FileBasedTests.TestEntryResultAttribute]);
            if (expectedResult == ExpectedResult.ValidationError)
            {
                errorNumber = int.Parse((string)this.TestContext.DataRow[FileBasedTests.TestEntryErrorAttribute]);
            }
            
            Trace.WriteLine("Path: " + path);
            Trace.WriteLine("ExpectedResult: " + expectedResult);
            Trace.WriteLine("ErrorNumber: " + errorNumber);

            try
            {
                Assert.IsTrue(System.IO.File.Exists(path), "File '{0}' doesn't exist.", path);
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    XliffDocument document;
                    XliffReader reader;

                    reader = new XliffReader();
                    document = reader.Deserialize(stream);
                }

                Assert.AreEqual(expectedResult, ExpectedResult.Success, "Expected result is incorrect.");
            }
            catch (FormatException)
            {
                if (expectedResult != ExpectedResult.BadFormat)
                {
                    throw;
                }
            }
            catch (InvalidOperationException)
            {
                if (expectedResult != ExpectedResult.InvalidOperation)
                {
                    throw;
                }
            }
            catch (NotSupportedException)
            {
                if (expectedResult != ExpectedResult.NotSupported)
                {
                    throw;
                }
            }
            catch (ValidationException e)
            {
                if (expectedResult != ExpectedResult.ValidationError)
                {
                    throw;
                }

                Assert.AreEqual(errorNumber, e.ErrorNumber, "ErrorNumber is incorrect.");
            }
            catch (XmlException)
            {
                if (expectedResult != ExpectedResult.ReadError)
                {
                    throw;
                }
            }
        }

        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Xliff_TestFile()
        {
            IEnumerable<string> files;
            HashSet<string> testPaths;
            XmlDocument doc;

            testPaths = new HashSet<string>();

            doc = new XmlDocument();
            doc.Load(FileBasedTests.DataDrivenTestFile);
            foreach (XmlNode node in doc.SelectNodes(FileBasedTests.TestEntryXPath))
            {
                testPaths.Add(node.Attributes[FileBasedTests.TestEntryPathAttribute].Value.ToLower());
            }

            // Validate that there is something to test.
            Assert.IsTrue(testPaths.Count > 0, "No files found in test data file.");

            // Validate that all files in the test directory are represented in the test data file. This ensures that
            // test files are not added and the test data file is not updated.
            files = Directory.EnumerateFiles(
                                             FileBasedTests.TestFileSourceDirectory,
                                             FileBasedTests.XliffFileExtension,
                                             SearchOption.AllDirectories);
            foreach (string path in files)
            {
                string lowerPath;

                lowerPath = path.ToLower();
                Assert.IsTrue(
                              testPaths.Contains(lowerPath),
                              "File '{0}' is not listed in the test data file.",
                              lowerPath);
            }
        }
        #endregion Test Methods

        private enum ExpectedResult
        {
            BadFormat,
            InvalidOperation,
            NotSupported,
            ReadError,
            Success,
            ValidationError
        }
    }
}
