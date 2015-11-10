namespace Localization.Xliff.OM.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="CodePoint"/> class.
    /// </summary>
    [TestClass()]
    public class CodePointTests
    {
        /// <summary>
        /// The element under test.
        /// </summary>
        private CodePoint _element;

        /// <summary>
        /// The provider to the element data.
        /// </summary>
        private IXliffDataProvider _provider;

        /// <summary>
        /// Initializes the test class before every test method is executed.
        /// </summary>
        [TestInitialize()]
        public void TestInitialize()
        {
            this._element = new CodePoint();
            this._provider = this._element;
        }

        /// <summary>
        /// Tests code point validation with an invalid Unicode.
        /// </summary>
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void CodePoint_InvalidCode_NotUnicode_Positive()
        {
            this._element.Code = 0x11FFFF;
        }

        /// <summary>
        /// Tests code point validation with an invalid Unicode.
        /// </summary>
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void CodePoint_InvalidCode_NotUnicode_Negative()
        {
            this._element.Code = -3;
        }

        /// <summary>
        /// Tests code point validation with valid XML character.
        /// </summary>
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void CodePoint_InvalidCode_RegularChar()
        {
            // Go through the constructor as well.
            CodePoint cp = new CodePoint((int)'a');
        }

        /// <summary>
        /// Tests code point validation with valid XML character.
        /// </summary>
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void CodePoint_InvalidCode_Extended()
        {
            this._element.Code = 0xFFFD;
        }

        /// <summary>
        /// Tests code point validation with valid XML character.
        /// </summary>
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void CodePoint_InvalidCode_Surrogate()
        {
            // Surrogate pairs are valid to include in the XML.
            // They do not need to be encoded.
            this._element.Code = 0x10148;
        }

        #region Test Methods
        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffAttributes"/> method for the element.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void CodePoint_IXliffDataProvider_GetXliffAttributes()
        {
            for (int i = 0; i < 4; i++)
            {
                IEnumerable<IAttributeDataProvider> attributes;

                switch (i)
                {
                    case 0:
                        Console.WriteLine("Test with default element.");
                        break;

                    case 1:
                        Console.WriteLine("Test with modified element.");
                        this._element.Code = 5;
                        break;

                    case 2:
                        Console.WriteLine("Test with extended character.");
                        this._element.Code = 0xFFFF;
                        break;

                    case 3:
                        Console.WriteLine("Test with half-surrogate.");
                        this._element.Code = 0xD801;
                        break;

                    default:
                        Assert.Fail("Iteration not expected.");
                        break;
                }

                attributes = this._provider.GetXliffAttributes();
                Assert.IsNotNull(attributes, "Attributes are null.");
                Assert.AreEqual(1, attributes.Count(), "Number of attributes is incorrect.");

                Assert.AreEqual(this._element.Code,
                                attributes.First((a) => a.LocalName == AttributeNames.Hex).Value,
                                "Code is incorrect.");
            }
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffChildren"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void CodePoint_IXliffDataProvider_GetXliffChildren()
        {
            List<ElementInfo> children;

            Console.WriteLine("Test with a no files.");
            children = this._provider.GetXliffChildren().ToList();
            Assert.IsTrue(((children == null) || (children.Count() == 0)), "Children is not null.");
        }

        /// <summary>
        /// Tests the <see cref="IXliffDataProvider.GetXliffText"/> method for the document.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void CodePoint_IXliffDataProvider_GetXliffText()
        {
            Assert.IsNull(this._provider.GetXliffText(), "Text is not null.");
        }
        #endregion Test Methods
    }
}
