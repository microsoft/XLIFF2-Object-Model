namespace Localization.Xliff.OM.Extensibility.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Exceptions;
    using Localization.Xliff.OM.Modules.ChangeTracking;
    using Localization.Xliff.OM.Modules.FormatStyle;
    using Localization.Xliff.OM.Modules.Glossary;
    using Localization.Xliff.OM.Modules.Metadata;
    using Localization.Xliff.OM.Modules.ResourceData;
    using Localization.Xliff.OM.Modules.SizeRestriction;
    using Localization.Xliff.OM.Modules.TranslationCandidates;
    using Localization.Xliff.OM.Modules.Validation;
    using Localization.Xliff.OM.Serialization;
    using Localization.Xliff.OM.Serialization.Tests;
    using Localization.Xliff.OM.Tests;
    using Localization.Xliff.OM.XmlNames;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using IO = System.IO;

    /// <summary>
    /// This class tests extensibility functionaly.
    /// </summary>
    [DeploymentItem(TestUtilities.TestDataDirectory, TestUtilities.TestDataDirectory)]
    [TestClass()]
    public class ExtensibilityTests
    {
        /// <summary>
        /// The name of the default extension handler.
        /// </summary>
        private const string DefaultExtensionName = "GenericExtensionHandler";

        /// <summary>
        /// The name of the first test namespace.
        /// </summary>
        private const string Namespace1 = "namespace1";

        /// <summary>
        /// The name of the second test namespace.
        /// </summary>
        private const string Namespace2 = "namespace2";

        /// <summary>
        /// The name of the first test Xml prefix.
        /// </summary>
        private const string Prefix1 = "pre1";

        /// <summary>
        /// The name of the second test Xml prefix.
        /// </summary>
        private const string Prefix2 = "pre2";

        /// <summary>
        /// The Id to use for created element Ids.
        /// </summary>
        private Dictionary<Type, int> ids = new Dictionary<Type, int>();

        #region Test Methods
        /// <summary>
        /// This method tests that extensions are deserialized correctly when there is a registered handler but the
        /// handler can't handle the data stored in the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Extensibility_DeserializeWithInvalidRegisteredExtensionHandlers()
        {
            XliffDocument document;
            XliffDataList expectedChildren;
            List<IExtensionAttribute> expectedAttributes;
            Dictionary<string, IExtensionHandler> handlers;
            IExtensible extensible;
            XliffData data1;
            XliffData data2;
            XliffData data3;

            handlers = new Dictionary<string, IExtensionHandler>();
            handlers["bogus"] = new CustomHandler("bogus", "bogus");
            document = TestUtilities.Deserialize(TestData.DocumentWithExtensions, true, handlers);

            Console.WriteLine("Verifying document extensions.");
            extensible = (IExtensible)document;
            Assert.IsNotNull(extensible.Extensions, "Extensions should not be null");
            Assert.AreEqual(1, extensible.Extensions.Count, "Extensions count is incorrect.");

            Console.WriteLine("  Verifying Extensions[0].");
            Assert.AreEqual(ExtensibilityTests.DefaultExtensionName, extensible.Extensions[0].Name, "Extension name is incorrect.");
            expectedAttributes = new List<IExtensionAttribute>();
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 1"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 2"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute1", "attribute 3"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute2", "attribute 4"));
            this.VerifyAttributes(expectedAttributes, extensible.Extensions[0]);
            this.VerifyExtensionChildren("    ", null, extensible.Extensions[0]);

            Console.WriteLine("Verifying file extensions.");
            extensible = (IExtensible)document.Files[0];
            Assert.IsNotNull(extensible.Extensions, "Extensions should not be null");
            Assert.AreEqual(1, extensible.Extensions.Count, "Extensions count is incorrect.");

            Console.WriteLine("  Verifying Extensions[0].");
            Assert.AreEqual(ExtensibilityTests.DefaultExtensionName, extensible.Extensions[0].Name, "Extension name is incorrect.");
            expectedAttributes = new List<IExtensionAttribute>();
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 5"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 6"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute1", "attribute 7"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute2", "attribute 8"));
            this.VerifyAttributes(expectedAttributes, extensible.Extensions[0]);

            expectedChildren = new XliffDataList();
            // Child 0.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1", typeof(GenericElement));
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute1", "attribute 9");
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 10");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element2", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace1, "attribute3", "attribute 11");
            data3 = data2.Children.AddChild(NamespaceValues.Core, "source", typeof(Source));
            data3.AddAttribute(NamespacePrefixes.Xml, null, "lang", "de-de", true);
            data3.AddAttribute(NamespacePrefixes.Xml, null, "space", Preservation.Preserve, true);

            // Child 1.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1", typeof(GenericElement));
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute1", "attribute 12");
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 13");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element2", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace1, "attribute3", "attribute 14");
            data2.Text = "text 1";

            // Child 2.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1", typeof(GenericElement));
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute1", "attribute 15");
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 16");
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute1", "attribute 17");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 18");
            data2.Text = "text 3";

            // Child 3.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1", typeof(GenericElement));
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute1", "attribute 19");
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute2", "attribute 20");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element2", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace2, "attribute3", "attribute 21");
            data3 = data2.Children.AddChild(NamespaceValues.Core, "target", typeof(Target));
            data3.AddAttribute(null, "order", null, false);
            data3.AddAttribute(NamespacePrefixes.Xml, null, "lang", null, false);
            data3.AddAttribute(NamespacePrefixes.Xml, null, "space", Preservation.Default, false);

            // Child 4.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1", typeof(GenericElement));
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute1", "attribute 22");
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute2", "attribute 23");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element2", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace2, "attribute3", "attribute 24");
            data2.Text = "text 2";

            this.VerifyExtensionChildren("    ", expectedChildren, extensible.Extensions[0]);

            Console.WriteLine("Verifying unit extensions.");
            extensible = (IExtensible)document.Files[0].Containers[0];
            Assert.IsFalse(extensible.HasExtensions, "HasExtensions is incorrect.");
        }

        /// <summary>
        /// This method tests that extensions are deserialized correctly when there are no handlers registered for the
        /// data in the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Extensibility_DeserializeWithNoRegisteredExtensionHandlers()
        {
            XliffDocument document;
            XliffDataList expectedChildren;
            List<IExtensionAttribute> expectedAttributes;
            IExtensible extensible;
            XliffData data1;
            XliffData data2;
            XliffData data3;

            document = TestUtilities.Deserialize(TestData.DocumentWithExtensions, true, null);

            Console.WriteLine("Verifying document extensions.");
            extensible = (IExtensible)document;
            Assert.IsNotNull(extensible.Extensions, "Extensions should not be null");
            Assert.AreEqual(1, extensible.Extensions.Count, "Extensions count is incorrect.");

            Console.WriteLine("  Verifying Extensions[0].");
            Assert.AreEqual(ExtensibilityTests.DefaultExtensionName, extensible.Extensions[0].Name, "Extension name is incorrect.");
            expectedAttributes = new List<IExtensionAttribute>();
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 1"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 2"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute1", "attribute 3"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute2", "attribute 4"));
            this.VerifyAttributes(expectedAttributes, extensible.Extensions[0]);
            this.VerifyExtensionChildren("    ", null, extensible.Extensions[0]);

            Console.WriteLine("Verifying file extensions.");
            extensible = (IExtensible)document.Files[0];
            Assert.IsNotNull(extensible.Extensions, "Extensions should not be null");
            Assert.AreEqual(1, extensible.Extensions.Count, "Extensions count is incorrect.");

            Console.WriteLine("  Verifying Extensions[0].");
            Assert.AreEqual(ExtensibilityTests.DefaultExtensionName, extensible.Extensions[0].Name, "Extension name is incorrect.");
            expectedAttributes = new List<IExtensionAttribute>();
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 5"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 6"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute1", "attribute 7"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute2", "attribute 8"));
            this.VerifyAttributes(expectedAttributes, extensible.Extensions[0]);

            expectedChildren = new XliffDataList();
            // Child 0.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1", typeof(GenericElement));
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute1", "attribute 9");
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 10");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element2", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace1, "attribute3", "attribute 11");
            data3 = data2.Children.AddChild(NamespaceValues.Core, "source", typeof(Source));
            data3.AddAttribute(NamespacePrefixes.Xml, null, "lang", "de-de", true);
            data3.AddAttribute(NamespacePrefixes.Xml, null, "space", Preservation.Preserve, true);

            // Child 1.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1", typeof(GenericElement));
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute1", "attribute 12");
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 13");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element2", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace1, "attribute3", "attribute 14");
            data2.Text = "text 1";

            // Child 2.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1", typeof(GenericElement));
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute1", "attribute 15");
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 16");
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute1", "attribute 17");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 18");
            data2.Text = "text 3";

            // Child 3.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1", typeof(GenericElement));
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute1", "attribute 19");
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute2", "attribute 20");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element2", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace2, "attribute3", "attribute 21");
            data3 = data2.Children.AddChild(NamespaceValues.Core, "target", typeof(Target));
            data3.AddAttribute(null, "order", null, false);
            data3.AddAttribute(NamespacePrefixes.Xml, null, "lang", null, false);
            data3.AddAttribute(NamespacePrefixes.Xml, null, "space", Preservation.Default, false);

            // Child 4.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1", typeof(GenericElement));
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute1", "attribute 22");
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute2", "attribute 23");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element2", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace2, "attribute3", "attribute 24");
            data2.Text = "text 2";

            this.VerifyExtensionChildren("    ", expectedChildren, extensible.Extensions[0]);

            Console.WriteLine("Verifying unit extensions.");
            extensible = (IExtensible)document.Files[0].Containers[0];
            Assert.IsFalse(extensible.HasExtensions, "HasExtensions is incorrect.");
        }

        /// <summary>
        /// This method tests that extensions are deserialized correctly when there there is one handler registered
        /// that can handle some of the data stored in the document, but not all of it.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Extensibility_DeserializeWithOneRegisteredExtensionHandlers()
        {
            CustomExtension extension;
            XliffDocument document;
            XliffDataList expectedChildren;
            List<IExtensionAttribute> expectedAttributes;
            Dictionary<string, IExtensionHandler> handlers;
            IExtensible extensible;
            XliffData data1;
            XliffData data2;
            XliffData data3;

            handlers = new Dictionary<string, IExtensionHandler>();
            handlers[ExtensibilityTests.Namespace1] = new CustomHandler(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1);
            document = TestUtilities.Deserialize(TestData.DocumentWithExtensions, true, handlers);

            Console.WriteLine("Verifying document extensions.");
            extensible = (IExtensible)document;
            Assert.IsNotNull(extensible.Extensions, "Extensions should not be null");
            Assert.AreEqual(2, extensible.Extensions.Count, "Extensions count is incorrect.");

            Console.WriteLine("  Verifying Extensions[0].");
            Assert.AreEqual(ExtensibilityTests.Namespace1, extensible.Extensions[0].Name, "Extension name is incorrect.");
            expectedAttributes = new List<IExtensionAttribute>();
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 1"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 2"));
            this.VerifyAttributes(expectedAttributes, extensible.Extensions[0]);
            this.VerifyExtensionChildren("    ", null, extensible.Extensions[0]);

            Console.WriteLine("  Verifying Extensions[1].");
            Assert.AreEqual(ExtensibilityTests.DefaultExtensionName, extensible.Extensions[1].Name, "Extension name is incorrect.");
            expectedAttributes = new List<IExtensionAttribute>();
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute1", "attribute 3"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute2", "attribute 4"));
            this.VerifyAttributes(expectedAttributes, extensible.Extensions[1]);
            this.VerifyExtensionChildren("    ", null, extensible.Extensions[1]);

            Console.WriteLine("Verifying file extensions.");
            extensible = (IExtensible)document.Files[0];
            Assert.IsNotNull(extensible.Extensions, "Extensions should not be null");
            Assert.AreEqual(2, extensible.Extensions.Count, "Extensions count is incorrect.");

            Console.WriteLine("  Verifying Extensions[0].");
            Assert.IsInstanceOfType(extensible.Extensions[0], typeof(CustomExtension), "Extension[0] type is incorrect.");
            Assert.AreEqual(ExtensibilityTests.Namespace1, extensible.Extensions[0].Name, "Extension name is incorrect.");
            expectedAttributes = new List<IExtensionAttribute>();
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 5"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 6"));
            this.VerifyAttributes(expectedAttributes, extensible.Extensions[0]);

            expectedChildren = new XliffDataList();
            // Child 0.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1", typeof(CustomElement1));
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute1", "attribute 9");
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 10");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element2", typeof(CustomElement2));
            data2.AddAttribute(ExtensibilityTests.Namespace1, "attribute3", "attribute 11");
            data3 = data2.Children.AddChild(NamespaceValues.Core, "source", typeof(GenericElement));
            data3.AddAttribute(null, "http://www.w3.org/XML/1998/namespace", "lang", "de-de", true);
            data3.AddAttribute(null, "http://www.w3.org/XML/1998/namespace", "space", "preserve", true);

            // Child 1.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1", typeof(CustomElement1));
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute1", "attribute 12");
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 13");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element2", typeof(CustomElement2));
            data2.AddAttribute(ExtensibilityTests.Namespace1, "attribute3", "attribute 14");
            data2.Text = "text 1";

            // Child 2.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1", typeof(CustomElement1));
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute1", "attribute 15");
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 16");
            // Attribute22 is in a different namespace than CustomElement1 so it is discarded.
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 18");
            data2.Text = "text 3";

            this.VerifyExtensionChildren("    ", expectedChildren, extensible.Extensions[0]);

            // Verify the attribute got set.
            extension = (CustomExtension)extensible.Extensions[0];
            Assert.AreEqual(
                            "attribute 9",
                            ((CustomElement1)extension.GetChildren().ToList()[0].Element).Attribute1,
                            "Attribute1 is incorrect.");

            Console.WriteLine("  Verifying Extensions[1].");
            Assert.AreEqual(ExtensibilityTests.DefaultExtensionName, extensible.Extensions[1].Name, "Extension name is incorrect.");
            expectedAttributes = new List<IExtensionAttribute>();
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute1", "attribute 7"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute2", "attribute 8"));
            this.VerifyAttributes(expectedAttributes, extensible.Extensions[1]);

            expectedChildren = new XliffDataList();
            // Child 0.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1", typeof(GenericElement));
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute1", "attribute 19");
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute2", "attribute 20");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element2", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace2, "attribute3", "attribute 21");
            data3 = data2.Children.AddChild(NamespaceValues.Core, "target", typeof(Target));
            data3.AddAttribute(null, "order", null, false);
            data3.AddAttribute(NamespacePrefixes.Xml, null, "lang", null, false);
            data3.AddAttribute(NamespacePrefixes.Xml, null, "space", Preservation.Default, false);

            // Child 1.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1", typeof(GenericElement));
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute1", "attribute 22");
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute2", "attribute 23");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element2", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace2, "attribute3", "attribute 24");
            data2.Text = "text 2";

            this.VerifyExtensionChildren("    ", expectedChildren, extensible.Extensions[1]);
            Assert.IsInstanceOfType(extensible.Extensions[0], typeof(CustomExtension), "Extension[0] type is incorrect.");
            Assert.IsInstanceOfType(extensible.Extensions[1], typeof(GenericExtension), "Extension[1] type is incorrect.");

            Console.WriteLine("Verifying unit extensions.");
            extensible = (IExtensible)document.Files[0].Containers[0];
            Assert.IsFalse(extensible.HasExtensions, "HasExtensions is incorrect.");
        }

        /// <summary>
        /// This method tests that extensions are deserialized correctly when there there are two handlers registered
        /// that can handle all of the data stored in the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Extensibility_DeserializeWithTwoRegisteredExtensionHandlers()
        {
            CustomExtension extension;
            XliffDocument document;
            XliffDataList expectedChildren;
            List<IExtensionAttribute> expectedAttributes;
            Dictionary<string, IExtensionHandler> handlers;
            IExtensible extensible;
            XliffData data1;
            XliffData data2;
            XliffData data3;

            handlers = new Dictionary<string, IExtensionHandler>();
            handlers[ExtensibilityTests.Namespace1] = new CustomHandler(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1);
            handlers[ExtensibilityTests.Namespace2] = new CustomHandler(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2);
            document = TestUtilities.Deserialize(TestData.DocumentWithExtensions, true, handlers);

            Console.WriteLine("Verifying document extensions.");
            extensible = (IExtensible)document;
            Assert.IsNotNull(extensible.Extensions, "Extensions should not be null");
            Assert.AreEqual(2, extensible.Extensions.Count, "Extensions count is incorrect.");

            Console.WriteLine("  Verifying Extensions[0].");
            Assert.IsInstanceOfType(extensible.Extensions[0], typeof(CustomExtension), "Extension[0] type is incorrect.");
            Assert.AreEqual(ExtensibilityTests.Namespace1, extensible.Extensions[0].Name, "Extension name is incorrect.");
            expectedAttributes = new List<IExtensionAttribute>();
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 1"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 2"));
            this.VerifyAttributes(expectedAttributes, extensible.Extensions[0]);
            this.VerifyExtensionChildren("    ", null, extensible.Extensions[0]);

            Console.WriteLine("  Verifying Extensions[1].");
            Assert.IsInstanceOfType(extensible.Extensions[1], typeof(CustomExtension), "Extension[1] type is incorrect.");
            Assert.AreEqual(ExtensibilityTests.Namespace2, extensible.Extensions[1].Name, "Extension name is incorrect.");
            expectedAttributes = new List<IExtensionAttribute>();
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute1", "attribute 3"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute2", "attribute 4"));
            this.VerifyAttributes(expectedAttributes, extensible.Extensions[1]);
            this.VerifyExtensionChildren("    ", null, extensible.Extensions[1]);

            Console.WriteLine("Verifying file extensions.");
            extensible = (IExtensible)document.Files[0];
            Assert.IsNotNull(extensible.Extensions, "Extensions should not be null");
            Assert.AreEqual(2, extensible.Extensions.Count, "Extensions count is incorrect.");

            Console.WriteLine("  Verifying Extensions[0].");
            Assert.AreEqual(ExtensibilityTests.Namespace1, extensible.Extensions[0].Name, "Extension name is incorrect.");
            expectedAttributes = new List<IExtensionAttribute>();
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 5"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 6"));
            this.VerifyAttributes(expectedAttributes, extensible.Extensions[0]);

            expectedChildren = new XliffDataList();
            // Child 0.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1", typeof(CustomElement1));
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute1", "attribute 9");
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 10");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element2", typeof(CustomElement2));
            data2.AddAttribute(ExtensibilityTests.Namespace1, "attribute3", "attribute 11");
            data3 = data2.Children.AddChild(NamespaceValues.Core, "source", typeof(GenericElement));
            data3.AddAttribute(null, "http://www.w3.org/XML/1998/namespace", "lang", "de-de", true);
            data3.AddAttribute(null, "http://www.w3.org/XML/1998/namespace", "space", "preserve", true);

            // Child 1.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1", typeof(CustomElement1));
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute1", "attribute 12");
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 13");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element2", typeof(CustomElement2));
            data2.AddAttribute(ExtensibilityTests.Namespace1, "attribute3", "attribute 14");
            data2.Text = "text 1";

            // Child 2.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1", typeof(CustomElement1));
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute1", "attribute 15");
            data1.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 16");
            // Attribute22 is in a different namespace than CustomElement1 so it is discarded.
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1", typeof(GenericElement));
            data2.AddAttribute(ExtensibilityTests.Namespace1, "attribute2", "attribute 18");
            data2.Text = "text 3";

            this.VerifyExtensionChildren("    ", expectedChildren, extensible.Extensions[0]);

            // Verify the attribute got set.
            extension = (CustomExtension)extensible.Extensions[0];
            Assert.AreEqual(
                            "attribute 9",
                            ((CustomElement1)extension.GetChildren().ToList()[0].Element).Attribute1,
                            "Attribute1 is incorrect.");

            Console.WriteLine("  Verifying Extensions[1].");
            Assert.AreEqual(ExtensibilityTests.Namespace2, extensible.Extensions[1].Name, "Extension name is incorrect.");
            expectedAttributes = new List<IExtensionAttribute>();
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute1", "attribute 7"));
            expectedAttributes.Add(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace2, "attribute2", "attribute 8"));
            this.VerifyAttributes(expectedAttributes, extensible.Extensions[1]);

            expectedChildren = new XliffDataList();
            // Child 0.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1", typeof(CustomElement1));
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute1", "attribute 19");
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute2", "attribute 20");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element2", typeof(CustomElement2));
            data2.AddAttribute(ExtensibilityTests.Namespace2, "attribute3", "attribute 21");
            data3 = data2.Children.AddChild(NamespaceValues.Core, "target", typeof(GenericElement));

            // Child 1.
            data1 = expectedChildren.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1", typeof(CustomElement1));
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute1", "attribute 22");
            data1.AddAttribute(ExtensibilityTests.Namespace2, "attribute2", "attribute 23");
            data2 = data1.Children.AddChild(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element2", typeof(CustomElement2));
            data2.AddAttribute(ExtensibilityTests.Namespace2, "attribute3", "attribute 24");
            data2.Text = "text 2";

            this.VerifyExtensionChildren("    ", expectedChildren, extensible.Extensions[1]);

            Console.WriteLine("Verifying unit extensions.");
            extensible = (IExtensible)document.Files[0].Containers[0];
            Assert.IsFalse(extensible.HasExtensions, "HasExtensions is incorrect.");
        }

        /// <summary>
        /// This method tests that all elements and attributes including extensions are serialized and deserialized in
        /// a roundtrip fashion.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Extensibility_FullDocumentRoundtrip()
        {
            XliffDocument document;
            string documentContents;
            string fileContents;
            int mismatchIndex;
            
            this.ids.Clear();

            document = this.CreateFullyLoadedDocument();
            documentContents = TestUtilities.GetDocumentContents(document, string.Empty);
            fileContents = TestUtilities.GetFileContents(TestData.DocumentWithEverything);
            mismatchIndex = -1;
            for (int i = 0; i < fileContents.Length; i++)
            {
                if ((mismatchIndex < 0) && (fileContents[i] != documentContents[i]))
                {
                    mismatchIndex = i;
                    break;
                }
            }

            if (mismatchIndex >= 0)
            {
                Console.WriteLine("Expected Output:");
                Console.WriteLine(fileContents);
                Console.WriteLine();
                Console.WriteLine("Actual Output:");
                Console.WriteLine(documentContents);
                Console.WriteLine();
            }

            Assert.IsTrue(fileContents == documentContents,
                          "Document contents are incorrect starting at index {0} (neighbor chars are '{1}' vs. '{2}').",
                          mismatchIndex,
                          (mismatchIndex >= 10) ? fileContents.Substring(mismatchIndex - 10, 20) : "[see output]",
                          (mismatchIndex >= 10) ? documentContents.Substring(mismatchIndex - 10, 20) : "[see output]");

            Console.WriteLine("Serializing and deserializing document");
            using (IO.MemoryStream stream = new IO.MemoryStream())
            {
                XliffReader reader;
                XliffWriter writer;

                writer = new XliffWriter();
                writer.Serialize(stream, document);

                stream.Seek(0, IO.SeekOrigin.Begin);
                reader = new XliffReader();
                document = reader.Deserialize(stream);

                Assert.AreEqual(
                              TestUtilities.GetFileContents(TestData.DocumentWithEverything),
                              TestUtilities.GetDocumentContents(document, string.Empty),
                              "Document contents are incorrect.");
            }
        }

        /// <summary>
        /// This method tests that extensions are serialized and deserialized in a roundtrip fashion.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Extensibility_Roundtrip()
        {
            Dictionary<string, IExtensionHandler> handlers;
            XliffDocument document;

            handlers = new Dictionary<string, IExtensionHandler>();
            handlers[ExtensibilityTests.Namespace1] = new CustomHandler(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1);
            handlers[ExtensibilityTests.Namespace2] = new CustomHandler(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2);
            document = TestUtilities.Deserialize(TestData.DocumentWithExtensions, true, handlers);

            Assert.AreEqual(
                           TestUtilities.GetFileContents(TestData.DocumentWithExtensions),
                           TestUtilities.GetDocumentContents(document, "    "),
                           "Document contents are incorrect.");

        }

        /// <summary>
        /// This method tests that extensions are serialized correctly when there there are multiple extensions registered
        /// on various parts of the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Extensibility_SerializeWithRegisteredExtensions()
        {
            GenericElement child1;
            GenericElement child2;
            XliffDocument document;
            IExtensible extensible;
            IExtension extension;
            Segment segment;
            Source source;
            Unit unit;

            // Document extensions.
            document = new XliffDocument("en-us");
            extensible = document;
            extension = new GenericExtension("extension");
            extensible.Extensions.Add(extension);
            extension.AddAttribute(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 1"));
            extension.AddAttribute(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 2"));
            extension = new GenericExtension("extension");
            extensible.Extensions.Add(extension);
            extension.AddAttribute(new GenericExtensionAttribute(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "attribute1", "attribute 3"));
            extension.AddAttribute(new GenericExtensionAttribute(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "attribute2", "attribute 4"));

            // File extensions.
            document.Files.Add(new File("f1"));
            extensible = document.Files[0];
            extension = new GenericExtension("extension");
            extensible.Extensions.Add(extension);
            extension.AddAttribute(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 5"));
            extension.AddAttribute(new GenericExtensionAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 6"));
            extension = new GenericExtension("extension");
            extensible.Extensions.Add(extension);
            extension.AddAttribute(new GenericExtensionAttribute(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "attribute1", "attribute 7"));
            extension.AddAttribute(new GenericExtensionAttribute(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "attribute2", "attribute 8"));

            // Child 0.
            child1 = new GenericElement();
            child1.SetAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 9");
            child1.SetAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 10");
            extension.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1"), child1));
            child2 = new GenericElement();
            child2.SetAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute3", "attribute 11");
            child1.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element2"), child2));
            source = new Source();
            source.Language = "de-de";
            source.Space = Preservation.Preserve;
            child2.AddChild(new ElementInfo(new XmlNameInfo("source"), source));

            // Child 1.
            child1 = new GenericElement();
            child1.SetAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 12");
            child1.SetAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 13");
            extension.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1"), child1));
            child2 = new GenericElement();
            child2.SetAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute3", "attribute 14");
            child1.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Namespace1, "element2"), child2));
            child2.AddChild(new ElementInfo(new XmlNameInfo((string)null), new PlainText("text 1")));

            // Child 2.
            child1 = new GenericElement();
            child1.SetAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 15");
            child1.SetAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 16");
            child1.SetAttribute(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "attribute1", "attribute 17");
            extension.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "element1"), child1));
            child2 = new GenericElement();
            child2.SetAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute2", "attribute 18");
            child1.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Namespace2, "element1"), child2));
            child2.AddChild(new ElementInfo(new XmlNameInfo((string)null), new PlainText("text 3")));

            // Child 3.
            child1 = new GenericElement();
            child1.SetAttribute(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "attribute1", "attribute 19");
            child1.SetAttribute(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "attribute2", "attribute 20");
            extension.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1"), child1));
            child2 = new GenericElement();
            child2.SetAttribute(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "attribute3", "attribute 21");
            child1.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Namespace2, "element2"), child2));
            child2.AddChild(new ElementInfo(new XmlNameInfo("target"), new Target()));

            // Child 4.
            child1 = new GenericElement();
            child1.SetAttribute(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "attribute1", "attribute 22");
            child1.SetAttribute(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "attribute2", "attribute 23");
            extension.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "element1"), child1));
            child2 = new GenericElement();
            child2.SetAttribute(ExtensibilityTests.Prefix2, ExtensibilityTests.Namespace2, "attribute3", "attribute 24");
            child1.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Namespace2, "element2"), child2));
            child2.AddChild(new ElementInfo(new XmlNameInfo((string)null), new PlainText("text 2")));

            // Unit information.
            unit = new Unit("u1");
            document.Files[0].Containers.Add(unit);
            extensible = unit;

            extension = new GenericExtension("extension");
            extensible.Extensions.Add(extension);

            // Segment information.
            segment = new Segment("s1");
            segment.Source = new Source();
            segment.State = TranslationState.Initial;
            unit.Resources.Add(segment);

            Assert.AreEqual(
                            TestUtilities.GetFileContents(TestData.DocumentWithExtensions),
                            TestUtilities.GetDocumentContents(document, "    "),
                            "Document contents are incorrect.");
        }

        /// <summary>
        /// This method tests that serialization fails when trying to write out attribute extension information with
        /// invalid Xml prefix and namespace values.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Extensibility_WithInvalidAttributePrefix()
        {
            TestAttribute attribute;
            XliffDocument document;
            IExtensible extensible;
            IExtension extension;
            Segment segment;
            Unit unit;

            attribute = new TestAttribute(ExtensibilityTests.Namespace1, "attribute1", "attribute 1");

            document = new XliffDocument("en-us");
            extensible = document;
            extension = new GenericExtension("extension");
            extensible.Extensions.Add(extension);
            extension.AddAttribute(attribute);

            document.Files.Add(new File("f1"));

            // Unit information.
            unit = new Unit("u1");
            document.Files[0].Containers.Add(unit);
            extensible = unit;

            // Segment information.
            segment = new Segment("s1");
            segment.Source = new Source();
            segment.State = TranslationState.Initial;
            unit.Resources.Add(segment);

            Console.WriteLine("Test with null prefix.");
            try
            {
                attribute.Prefix = null;
                TestUtilities.GetDocumentContents(document, "    ");
                Assert.Fail("Expected InvalidXmlSpecifierException to be thrown.");
            }
            catch (InvalidXmlSpecifierException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(ArgumentNullException), "Exception is incorrect.");
            }

            Console.WriteLine("Test with invalid prefix.");
            try
            {
                attribute.Prefix = "a:b";
                TestUtilities.GetDocumentContents(document, "    ");
                Assert.Fail("Expected InvalidXmlSpecifierException to be thrown.");
            }
            catch (InvalidXmlSpecifierException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(XmlException), "Exception is incorrect.");
            }

            Console.WriteLine("Test with null namespace.");
            try
            {
                attribute.Namespace = null;
                attribute.Prefix = ExtensibilityTests.Prefix1;
                TestUtilities.GetDocumentContents(document, "    ");
                Assert.Fail("Expected InvalidXmlSpecifierException to be thrown.");
            }
            catch (InvalidXmlSpecifierException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(ArgumentNullException), "Exception is incorrect.");
            }

            Console.WriteLine("Test with null local name.");
            try
            {
                attribute.Namespace = ExtensibilityTests.Namespace1;
                attribute.LocalName = null;
                TestUtilities.GetDocumentContents(document, "    ");
                Assert.Fail("Expected InvalidXmlSpecifierException to be thrown.");
            }
            catch (InvalidXmlSpecifierException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(ArgumentNullException), "Exception is incorrect.");
            }

            Console.WriteLine("Test with differing namespace.");
            try
            {
                attribute.Prefix = ExtensibilityTests.Prefix1;
                attribute.LocalName = "name";
                extension.AddAttribute(new TestAttribute(attribute.Prefix, "test", "attr2", "value"));

                TestUtilities.GetDocumentContents(document, "    ");
                Assert.Fail("Expected InvalidXmlSpecifierException to be thrown.");
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// This method tests that serialization fails when trying to write out element extension information with
        /// invalid Xml prefix and namespace values.
        /// </summary>
        [TestMethod]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void Extensibility_WithInvalidElementPrefix()
        {
            XliffDocument document;
            IExtensible extensible;
            IExtension extension;
            Segment segment;
            Unit unit;

            document = new XliffDocument("en-us");

            document.Files.Add(new File("f1"));
            extensible = document.Files[0];

            // Unit information.
            unit = new Unit("u1");
            document.Files[0].Containers.Add(unit);
            extensible = unit;

            // Segment information.
            segment = new Segment("s1");
            segment.Source = new Source();
            segment.State = TranslationState.Initial;
            unit.Resources.Add(segment);

            Console.WriteLine("Test with null prefix.");
            try
            {
                extensible.Extensions.Clear();
                extension = new GenericExtension("extension");
                extensible.Extensions.Add(extension);
                extension.AddAttribute(new TestAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 1"));
                extension.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Namespace1, "name"), new Source()));

                TestUtilities.GetDocumentContents(document, "    ");
                Assert.Fail("Expected InvalidXmlSpecifierException to be thrown.");
            }
            catch (InvalidXmlSpecifierException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(ArgumentNullException), "Exception is incorrect.");
            }

            Console.WriteLine("Test with invalid prefix.");
            try
            {
                extensible.Extensions.Clear();
                extension = new GenericExtension("extension");
                extensible.Extensions.Add(extension);
                extension.AddAttribute(new TestAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 1"));
                extension.AddChild(new ElementInfo(new XmlNameInfo("a:b", ExtensibilityTests.Namespace1, "name"), new Source()));

                TestUtilities.GetDocumentContents(document, "    ");
                Assert.Fail("Expected InvalidXmlSpecifierException to be thrown.");
            }
            catch (InvalidXmlSpecifierException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(XmlException), "Exception is incorrect.");
            }

            Console.WriteLine("Test with null namespace.");
            try
            {
                extensible.Extensions.Clear();
                extension = new GenericExtension("extension");
                extensible.Extensions.Add(extension);
                extension.AddAttribute(new TestAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 1"));
                extension.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Prefix1, null, "name"), new Source()));

                TestUtilities.GetDocumentContents(document, "    ");
                Assert.Fail("Expected InvalidXmlSpecifierException to be thrown.");
            }
            catch (InvalidXmlSpecifierException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(ArgumentNullException), "Exception is incorrect.");
            }

            Console.WriteLine("Test with null local name.");
            try
            {
                extensible.Extensions.Clear();
                extension = new GenericExtension("extension");
                extensible.Extensions.Add(extension);
                extension.AddAttribute(new TestAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 1"));
                extension.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, null), new Source()));

                TestUtilities.GetDocumentContents(document, "    ");
                Assert.Fail("Expected InvalidXmlSpecifierException to be thrown.");
            }
            catch (InvalidXmlSpecifierException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(ArgumentNullException), "Exception is incorrect.");
            }

            Console.WriteLine("Test with differing namespace.");
            try
            {
                extensible.Extensions.Clear();
                extension = new GenericExtension("extension");
                extensible.Extensions.Add(extension);
                extension.AddAttribute(new TestAttribute(ExtensibilityTests.Prefix1, ExtensibilityTests.Namespace1, "attribute1", "attribute 1"));
                extension.AddChild(new ElementInfo(new XmlNameInfo(ExtensibilityTests.Prefix1, "namespace", "name"), new Source()));

                TestUtilities.GetDocumentContents(document, "    ");
                Assert.Fail("Expected InvalidXmlSpecifierException to be thrown.");
            }
            catch (InvalidOperationException)
            {
            }
        }
        #endregion Test Methods

        #region Helper Methods
        /// <summary>
        /// Creates an extension with every element and attribute and adds it to the container.
        /// </summary>
        /// <param name="extensible">The container to add the extension to.</param>
        private void AddExtension(IExtensible extensible)
        {
            GenericExtension extension;

            extension = new GenericExtension("extension");

            if (extensible.SupportsAttributeExtensions)
            {
                extension.AddAttribute(new GenericExtensionAttribute("ext", "ext:namespace", "attr2", "value"));
            }

            if (extensible.SupportsElementExtensions)
            {
                GenericElement element;

                element = new GenericElement();
                element.SetAttribute("ext", "ext:namespace", "attr1", "value");
                extension.AddChild(new ElementInfo(new XmlNameInfo("ext", "ext:namespace", "extelement"), element));
            }

            extensible.Extensions.Add(extension);
        }

        /// <summary>
        /// Creates a note with every element and attribute and adds it to the container.
        /// </summary>
        /// <param name="container">The container to add the note to.</param>
        /// <returns>The added note.</returns>
        private Note AddNote(INoteContainer container)
        {
            Note result;

            result = container.AddNote("note");
            result.AppliesTo = TranslationSubject.Source;
            result.Category = "category";
            result.FormatStyle = FormatStyleValue.Anchor;
            result.Id = "n" + this.GetNextId(typeof(Note));
            result.Priority = 1;
            result.SubFormatStyle.Add("key1", "value1");
            result.SubFormatStyle.Add("key2", "value2");

            return result;
        }

        /// <summary>
        /// Clones the existing Id dictionary.
        /// </summary>
        /// <returns>A clone of the Id dictionary.</returns>
        private Dictionary<Type, int> CloneIds()
        {
            Dictionary<Type, int> clone;

            clone = new Dictionary<Type, int>();
            foreach (KeyValuePair<Type, int> pair in this.ids)
            {
                clone[pair.Key] = pair.Value;
            }

            return clone;
        }

        /// <summary>
        /// Creates a <see cref="ChangeTrack"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="ChangeTrack"/>.</returns>
        private ChangeTrack CreateChangeTrackingModule_ChangeTrack()
        {
            ChangeTrack result;

            result = new ChangeTrack();
            result.Revisions.Add(this.CreateChangeTrackingModule_RevisionsContainer());

            return result;
        }

        /// <summary>
        /// Creates a <see cref="Item"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="Item"/>.</returns>
        private Item CreateChangeTrackModule_Item()
        {
            Item result;

            result = new Item();
            result.Property = "content";
            result.Text = "text";

            this.AddExtension(result);

            return result;
        }

        /// <summary>
        /// Creates a <see cref="Revision"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="Revision"/>.</returns>
        private Revision CreateChangeTrackModule_Revision()
        {
            Revision reult;

            reult = new Revision();
            reult.Author = "author";
            reult.ChangeDate = new DateTime(2015, 1, 2, 3, 4, 5);
            reult.Version = "ver1";

            reult.Items.Add(this.CreateChangeTrackModule_Item());
            this.AddExtension(reult);

            return reult;
        }

        /// <summary>
        /// Creates a <see cref="RevisionsContainer"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="RevisionsContainer"/>.</returns>
        private RevisionsContainer CreateChangeTrackingModule_RevisionsContainer()
        {
            RevisionsContainer result;

            result = new RevisionsContainer();
            result.AppliesTo = "source";
            result.Reference = "1";
            result.CurrentVersion = "ver1";

            result.Revisions.Add(this.CreateChangeTrackModule_Revision());
            this.AddExtension(result);

            return result;
        }

        /// <summary>
        /// Creates a <see cref="SpanningCodeEnd"/> with every element and attribute.
        /// </summary>
        /// <param name="copyOf">The Id of the element to copy, or null.</param>
        /// <param name="startRef">The reference to the start tag.</param>
        /// <returns>The created <see cref="SpanningCodeEnd"/>.</returns>
        private SpanningCodeEnd CreateEc(string copyOf, string startRef)
        {
            SpanningCodeEnd result;

            result = new SpanningCodeEnd("ec" + this.GetNextId(typeof(SpanningCodeEnd)));
            result.CanCopy = true;
            result.CanDelete = true;
            result.CanOverlap = true;
            result.CanReorder = CanReorderValue.Yes;
            result.CopyOf = copyOf;
            result.DataReference = null;
            result.Directionality = ContentDirectionality.Auto;
            result.DisplayText = "display";
            result.EquivalentText = "equiv";
            result.Isolated = (startRef == null);
            result.StartReference = startRef;
            result.SubFlows = "u1";
            result.SubType = "pre:subtype";
            result.Type = CodeType.Formatting;

            if (startRef == null)
            {
                result.EquivalentStorage = "storage";
                result.FormatStyle = FormatStyleValue.Anchor;
                result.SizeInfo = "size";
                result.SubFormatStyle.Add("key1", "value1");
                result.SubFormatStyle.Add("key2", "value2");
            }

            return result;
        }

        /// <summary>
        /// Creates a <see cref="MarkedSpanEnd"/> with every element and attribute.
        /// </summary>
        /// <param name="startRef">The reference to the start tag.</param>
        /// <returns>The created <see cref="MarkedSpanEnd"/>.</returns>
        private MarkedSpanEnd CreateEm(string startRef)
        {
            MarkedSpanEnd result;

            result = new MarkedSpanEnd(startRef);

            return result;
        }

        /// <summary>
        /// Creates a file with every element and attribute.
        /// </summary>
        /// <returns>The created file.</returns>
        private File CreateFile()
        {
            File result;
            int id;

            result = new File("f" + this.GetNextId(typeof(File)));
            result.CanResegment = true;
            result.FormatStyle = FormatStyleValue.Anchor;
            result.Original = "original";
            result.SizeInfo = "size";
            result.SizeRestriction = "restriction";
            result.SourceDirectionality = ContentDirectionality.Auto;
            result.Space = Preservation.Default;
            result.StorageRestriction = "restriction";
            result.SubFormatStyle.Add("key1", "value1");
            result.SubFormatStyle.Add("key2", "value2");
            result.TargetDirectionality = ContentDirectionality.Auto;
            result.Translate = true;

            // Reset all ids, because most ids must be unique within the file (or unit).
            id = this.ids[typeof(File)];
            this.ids.Clear();
            this.ids[typeof(File)] = id;

            result.Skeleton = this.CreateSkeleton(false);
            this.AddExtension(result);
            this.AddNote(result);
            result.Containers.Add(this.CreateGroup(true));
            result.Containers.Add(this.CreateUnit());

            result.Changes = this.CreateChangeTrackingModule_ChangeTrack();
            result.Metadata = this.CreateMetadata();
            result.ProfileData = this.CreateSizeRestrictionModule_ProfileData();
            result.ResourceData = this.CreateResourceDataModule_ResourceData();
            result.RestrictionProfiles = this.CreateSizeRestrictionModule_Profiles();
            result.ValidationRules = this.CreateValidationModule_Validation();

            return result;
        }

        /// <summary>
        /// Creates a document with every element and attribute.
        /// </summary>
        /// <returns>The created document.</returns>
        private XliffDocument CreateFullyLoadedDocument()
        {
            XliffDocument result;

            result = new XliffDocument("en-us");
            result.TargetLanguage = "de-de";
            result.Space = Preservation.Default;
            // version is automatic

            result.Files.Add(this.CreateFile());

            return result;
        }

        /// <summary>
        /// Creates a glossary with every element and attribute.
        /// </summary>
        /// <param name="segmentId">The Id of the segment containing the glossary.</param>
        /// <returns>The created glossary.</returns>
        private Glossary CreateGlossary(string segmentId)
        {
            Glossary result;

            result = new Glossary();
            result.Entries.Add(this.CreateGlossaryEntry(segmentId));

            return result;
        }

        /// <summary>
        /// Creates a glossary definition with every element and attribute.
        /// </summary>
        /// <returns>The created glossary definition.</returns>
        private Definition CreateGlossaryDefinition()
        {
            Definition result;

            result = new Definition();
            result.Source = "definitionSource";
            result.Text = "definition text";

            return result;
        }

        /// <summary>
        /// Creates a glossary entry with every element and attribute.
        /// </summary>
        /// <param name="segmentId">The Id of the segment containing the glossary.</param>
        /// <returns>The created glossary entry.</returns>
        private GlossaryEntry CreateGlossaryEntry(string segmentId)
        {
            GlossaryEntry result;

            result = new GlossaryEntry();
            result.Definition = this.CreateGlossaryDefinition();
            result.Id = "entry" + this.GetNextId(typeof(Definition));
            result.Reference = Utilities.MakeIri(segmentId);
            result.Term.Source = "termSource";
            result.Term.Text = "term text";
            result.Translations.Add(this.CreateGlossaryTranslation(segmentId));

            return result;
        }

        /// <summary>
        /// Creates a glossary translation with every element and attribute.
        /// </summary>
        /// <param name="segmentId">The Id of the segment containing the glossary.</param>
        /// <returns>The created glossary translation.</returns>
        private Translation CreateGlossaryTranslation(string segmentId)
        {
            Translation result;

            result = new Translation();
            result.Id = "trans" + this.GetNextId(typeof(Translation));
            result.Reference = Utilities.MakeIri(segmentId);
            result.Source = "translationSource";
            result.Text = "translation text";

            return result;
        }

        /// <summary>
        /// Creates a group with every element and attribute.
        /// </summary>
        /// <returns>The created group.</returns>
        private Group CreateGroup(bool nest)
        {
            Group result;

            result = new Group("g" + this.GetNextId(typeof(Group)));
            result.Name = "group";
            result.CanResegment = true;
            result.FormatStyle = FormatStyleValue.Anchor;
            result.SizeInfoReference = "sizeref";
            result.SizeRestriction = "restriction";
            result.SourceDirectionality = ContentDirectionality.LTR;
            result.Space = Preservation.Default;
            result.StorageRestriction = "restriction";
            result.SubFormatStyle.Add("key1", "value1");
            result.SubFormatStyle.Add("key2", "value2");
            result.TargetDirectionality = ContentDirectionality.RTL;
            result.Translate = false;
            result.Type = "pre:type";

            this.AddExtension(result);
            this.AddNote(result);
            if (nest)
            {
                result.Containers.Add(this.CreateGroup(false));
            }

            result.Containers.Add(this.CreateUnit());
            result.Changes = this.CreateChangeTrackingModule_ChangeTrack();
            result.Metadata = this.CreateMetadata();
            result.ProfileData = this.CreateSizeRestrictionModule_ProfileData();
            result.ValidationRules = this.CreateValidationModule_Validation();

            return result;
        }

        /// <summary>
        /// Creates an ignorable with every element and attribute.
        /// </summary>
        /// <param name="noteId">The Id of a note to reference.</param>
        /// <returns>The created ignorable.</returns>
        private Ignorable CreateIgnorable(string noteId)
        {
            Dictionary<Type, int> clone;
            Ignorable result;

            result = new Ignorable("i" + this.GetNextId(typeof(Ignorable)));

            // Clone the Ids so the Target uses the same Ids.
            clone = this.CloneIds();
            result.Source = this.CreateSource(noteId);

            this.ids = clone;
            result.Target = this.CreateTarget(noteId);

            return result;
        }

        /// <summary>
        /// Creates a match with every element and attribute.
        /// </summary>
        /// <param name="noteId">The Id of a note to reference.</param>
        /// <returns>The created match.</returns>
        private Match CreateMatch(string noteId)
        {
            Dictionary<Type, int> clone;
            Match result;

            this.ids[typeof(Data)] = 0;

            result = new Match(Utilities.MakeIri("s1"));
            result.Id = "match" + this.GetNextId(typeof(Match));
            result.MatchQuality = 1;
            result.MatchSuitability = 1;
            result.Origin = "origin";
            result.HasReferenceTranslation = true;
            result.Similarity = 1;
            result.SubType = "pre:subtype";
            result.Type = MatchType.AssembledMatch;

            result.Metadata = this.CreateMetadata();
            result.OriginalData = this.CreateOriginalData();

            // Clone the Ids so the Target uses the same Ids.
            clone = this.CloneIds();
            result.Source = this.CreateSource(noteId);

            this.ids = clone;
            result.Target = this.CreateTarget(noteId);
            this.AddExtension(result);

            return result;
        }

        /// <summary>
        /// Creates metadata with every element and attribute.
        /// </summary>
        /// <returns>The created metadata.</returns>
        private Meta CreateMeta()
        {
            Meta result;

            result = new Meta("text");
            result.Type = "pre:type";

            return result;
        }

        /// <summary>
        /// Creates a metadata container with every element and attribute.
        /// </summary>
        /// <returns>The created metadata container.</returns>
        private MetadataContainer CreateMetadata()
        {
            MetadataContainer result;

            result = new MetadataContainer();
            result.Id = "metadata" + this.GetNextId(typeof(MetadataContainer));

            result.Groups.Add(this.CreateMetaGroup(true));

            return result;
        }

        /// <summary>
        /// Creates a metadata group with every element and attribute.
        /// </summary>
        /// <returns>The created metadata group.</returns>
        private MetaGroup CreateMetaGroup(bool nest)
        {
            MetaGroup result;

            result = new MetaGroup();
            result.Id = "metagroup" + this.GetNextId(typeof(MetaGroup));
            result.Category = "category";
            result.AppliesTo = MetaGroupSubject.Ignorable;

            if (nest)
            {
                result.Containers.Add(this.CreateMetaGroup(false));
            }

            result.Containers.Add(this.CreateMeta());

            return result;
        }

        /// <summary>
        /// Creates a <see cref="MarkedSpan"/> with every element and attribute.
        /// </summary>
        /// <param name="noteId">The Id of a note to reference.</param>
        /// <returns>The created <see cref="MarkedSpan"/>.</returns>
        private MarkedSpan CreateMrk(string noteId)
        {
            MarkedSpan result;

            result = new MarkedSpan("mrk" + this.GetNextId(typeof(MarkedSpan)));
            result.FormatStyle = FormatStyleValue.Anchor;
            result.Reference = "#n=" + noteId;
            result.SizeRestriction = "restriction";
            result.StorageRestriction = "restriction";
            result.SubFormatStyle.Add("key1", "value1");
            result.SubFormatStyle.Add("key2", "value2");
            result.Translate = true;
            result.Type = "pre:type";
            result.Value = "value";

            return result;
        }

        /// <summary>
        /// Creates an original data with every element and attribute.
        /// </summary>
        /// <returns>The created original data.</returns>
        private OriginalData CreateOriginalData()
        {
            OriginalData result;
            Data data;

            result = new OriginalData();
            data = result.AddData("d" + this.GetNextId(typeof(Data)), "data");
            data.Directionality = ContentDirectionality.Auto;
            data.Space = Preservation.Preserve;

            data.Text.Add(new PlainText("text"));
            data.Text.Add(new CodePoint(24));
            data.Text.Add(new PlainText("text"));
            data.Text.Add(new CodePoint(25));

            return result;
        }

        /// <summary>
        /// Creates a <see cref="SpanningCode"/> with every element and attribute.
        /// </summary>
        /// <param name="copyOf">The Id of the element to copy, or null.</param>
        /// <returns>The created <see cref="SpanningCode"/>.</returns>
        private SpanningCode CreatePc(string copyOf)
        {
            SpanningCode result;

            result = new SpanningCode("pc" + this.GetNextId(typeof(SpanningCode)));
            result.CanCopy = true;
            result.CanDelete = true;
            result.CanOverlap = true;
            result.CanReorder = CanReorderValue.Yes;
            result.CopyOf = copyOf;
            result.DataReferenceEnd = null;
            result.DataReferenceStart = null;
            result.Directionality = ContentDirectionality.Auto;
            result.DisplayTextEnd = "end";
            result.DisplayTextStart = "start";
            result.EquivalentStorage = "storage";
            result.EquivalentTextEnd = "equivend";
            result.EquivalentTextStart = "equivstart";
            result.FormatStyle = FormatStyleValue.Anchor;
            result.SizeInfoReference = "sizeref";
            result.SizeRestriction = "restriction";
            result.StorageRestriction = "restriction";
            result.SubFlowsEnd = "u1";
            result.SubFlowsStart = "u1";
            result.SubFormatStyle.Add("key1", "value1");
            result.SubFormatStyle.Add("key2", "value2");
            result.SubType = "pre:subtype";
            result.Text.Add(new PlainText("text"));
            result.Type = CodeType.Formatting;

            return result;
        }

        /// <summary>
        /// Creates a <see cref="StandaloneCode"/> with every element and attribute.
        /// </summary>
        /// <param name="copyOf">The Id of the element to copy, or null.</param>
        /// <returns>The created <see cref="StandaloneCode"/>.</returns>
        private StandaloneCode CreatePh(string copyOf)
        {
            StandaloneCode result;

            result = new StandaloneCode("ph" + this.GetNextId(typeof(StandaloneCode)));
            result.CanCopy = true;
            result.CanDelete = false;
            result.CanReorder = CanReorderValue.Yes;
            result.CopyOf = copyOf;
            result.DataReference = null;
            result.DisplayText = "display";
            result.EquivalentStorage = "storage";
            result.EquivalentText = "equiv";
            result.FormatStyle = FormatStyleValue.Anchor;
            result.SizeInfo = "size";
            result.SubFlows = "u1";
            result.SubFormatStyle.Add("key1", "value1");
            result.SubFormatStyle.Add("key2", "value2");
            result.Type = CodeType.Image;

            return result;
        }

        /// <summary>
        /// Creates a <see cref="Reference"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="Reference"/>.</returns>
        private Reference CreateResourceDataModule_Reference()
        {
            Reference result;

            result = new Reference();
            result.HRef = "resource";
            result.Language = "de-de";

            this.AddExtension(result);

            return result;
        }

        /// <summary>
        /// Creates a <see cref="ResourceData"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="ResourceData"/>.</returns>
        private ResourceData CreateResourceDataModule_ResourceData()
        {
            ResourceData result;
            ResourceItem item;

            result = new ResourceData();
            item = this.CreateResourceDataModule_ResourceItem();
            result.ResourceItems.Add(item);
            result.ResourceItemReferences.Add(this.CreateResourceDataModule_ResourceItemRef(item.Id));

            return result;
        }

        /// <summary>
        /// Creates a <see cref="ResourceItem"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="ResourceItem"/>.</returns>
        private ResourceItem CreateResourceDataModule_ResourceItem()
        {
            ResourceItem result;

            result = new ResourceItem();
            result.Context = true;
            result.Id = "ri" + this.GetNextId(typeof(ResourceItem));
            result.MimeType = "mime";

            result.References.Add(this.CreateResourceDataModule_Reference());
            result.Source = this.CreateResourceDataModule_ResourceItemSource();
            result.Target = this.CreateResourceDataModule_ResourceItemTarget();

            this.AddExtension(result);

            return result;
        }

        /// <summary>
        /// Creates a <see cref="ResourceItemRef"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="ResourceItemRef"/>.</returns>
        private ResourceItemRef CreateResourceDataModule_ResourceItemRef(string reference)
        {
            ResourceItemRef result;

            result = new ResourceItemRef();
            result.Id = "rif" + this.GetNextId(typeof(ResourceItemRef));
            result.Reference = reference;

            this.AddExtension(result);

            return result;
        }

        /// <summary>
        /// Creates a <see cref="ResourceItemSource"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="ResourceItemSource"/>.</returns>
        private ResourceItemSource CreateResourceDataModule_ResourceItemSource()
        {
            ResourceItemSource result;

            result = new ResourceItemSource();
            result.HRef = "resource";
            result.Language = "en-us";

            return result;
        }

        /// <summary>
        /// Creates a <see cref="ResourceItemTarget"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="ResourceItemTarget"/>.</returns>
        private ResourceItemTarget CreateResourceDataModule_ResourceItemTarget()
        {
            ResourceItemTarget result;

            result = new ResourceItemTarget();
            result.HRef = "resource";
            result.Language = "de-de";

            return result;
        }

        /// <summary>
        /// Creates a <see cref="SpanningCodeStart"/> with every element and attribute.
        /// </summary>
        /// <param name="copyOf">The Id of the element to copy, or null.</param>
        /// <returns>The created <see cref="SpanningCodeStart"/>.</returns>
        private SpanningCodeStart CreateSc(string copyOf)
        {
            SpanningCodeStart result;

            result = new SpanningCodeStart("sc" + this.GetNextId(typeof(SpanningCodeStart)));
            result.CanCopy = true;
            result.CanDelete = true;
            result.CanOverlap = true;
            result.CanReorder = CanReorderValue.Yes;
            result.CopyOf = copyOf;
            result.DataReference = null;
            result.Directionality = ContentDirectionality.Auto;
            result.DisplayText = "display";
            result.EquivalentStorage = "storage";
            result.EquivalentText = "equiv";
            result.FormatStyle = FormatStyleValue.Anchor;
            result.Isolated = false;
            result.SizeInfoReference = "sizeref";
            result.SizeRestriction = "restriction";
            result.StorageRestriction = "restriction";
            result.SubFlows = "u1";
            result.SubFormatStyle.Add("key1", "value1");
            result.SubFormatStyle.Add("key2", "value2");
            result.SubType = "pre:subtype";
            result.Type = CodeType.Formatting;

            return result;
        }

        /// <summary>
        /// Creates a segment with every element and attribute.
        /// </summary>
        /// <param name="noteId">The Id of a note to reference.</param>
        /// <returns>The created segment.</returns>
        private Segment CreateSegment(string noteId)
        {
            Dictionary<Type, int> clone;
            Segment result;

            result = new Segment("s" + this.GetNextId(typeof(Segment)));
            result.CanResegment = false;
            result.State = TranslationState.Initial;
            result.SubState = "pre:substate";

            // Clone the Ids so the Target uses the same Ids.
            clone = this.CloneIds();
            result.Source = this.CreateSource(noteId);

            this.ids = clone;
            result.Target = this.CreateTarget(noteId);

            return result;
        }

        /// <summary>
        /// Creates a <see cref="Normalization"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="Normalization"/>.</returns>
        private Normalization CreateSizeRestrictionModule_Normalization()
        {
            Normalization result;

            result = new Normalization();
            result.General = Modules.NormalizationValue.NFC;
            result.Storage = Modules.NormalizationValue.NFC;

            return result;
        }

        /// <summary>
        /// Creates a <see cref="ProfileData"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="ProfileData"/>.</returns>
        private ProfileData CreateSizeRestrictionModule_ProfileData()
        {
            ProfileData result;

            result = new ProfileData();
            result.Profile = "profile";

            this.AddExtension(result);

            return result;
        }

        /// <summary>
        /// Creates a <see cref="Profiles"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="Profiles"/>.</returns>
        private Profiles CreateSizeRestrictionModule_Profiles()
        {
            Profiles result;

            result = new Profiles();
            result.GeneralProfile = "profile";
            result.StorageProfile = "profile";

            result.Normalization = this.CreateSizeRestrictionModule_Normalization();

            this.AddExtension(result);

            return result;
        }

        /// <summary>
        /// Creates a skeleton with every element and attribute.
        /// </summary>
        /// <param name="useHRef">True to use the HRef value, false to use the Text value.</param>
        /// <returns>The created skeleton.</returns>
        private Skeleton CreateSkeleton(bool useHRef)
        {
            Skeleton result;

            result = new Skeleton();
            if (useHRef)
            {
                result.HRef = "href";
            }
            else
            {
                result.NonTranslatableText = "text";
                this.AddExtension(result);
            }


            return result;
        }

        /// <summary>
        /// Creates a <see cref="MaredSpanStart"/> with every element and attribute.
        /// </summary>
        /// <param name="noteId">The Id of a note to reference.</param>
        /// <returns>The created <see cref="MaredSpanStart"/>.</returns>
        private MarkedSpanStart CreateSm(string noteId)
        {
            MarkedSpanStart result;

            result = new MarkedSpanStart("sm" + this.GetNextId(typeof(MarkedSpanStart)));
            result.FormatStyle = FormatStyleValue.Anchor;
            result.Reference = "#n=" + noteId;
            result.SizeRestriction = "restriction";
            result.StorageRestriction = "restriction";
            result.SubFormatStyle.Add("key1", "value1");
            result.SubFormatStyle.Add("key2", "value2");
            result.Translate = true;
            result.Type = "pre:type";
            result.Value = "value";

            return result;
        }

        /// <summary>
        /// Creates a source with every element and attribute.
        /// </summary>
        /// <param name="noteId">The Id of a note to reference.</param>
        /// <returns>The created source.</returns>
        private Source CreateSource(string noteId)
        {
            IIdentifiable identifiable1;
            IIdentifiable identifiable2;
            Source result;

            result = new Source();
            result.Text.Add(new CodePoint(24));
            identifiable1 = this.CreatePh(null);
            result.Text.Add((ResourceStringContent)identifiable1);
            result.Text.Add(this.CreatePh(identifiable1.Id));
            identifiable1 = this.CreatePc(null);
            result.Text.Add((ResourceStringContent)identifiable1);
            result.Text.Add(this.CreatePc(identifiable1.Id));

            identifiable1 = this.CreateSc(null);
            result.Text.Add((ResourceStringContent)identifiable1);
            identifiable2 = this.CreateEc(null, identifiable1.Id);
            result.Text.Add((ResourceStringContent)identifiable2);
            result.Text.Add(this.CreateEc(null, null));

            identifiable1 = this.CreateSc(identifiable1.Id);
            result.Text.Add((ResourceStringContent)identifiable1);
            result.Text.Add(this.CreateEc(identifiable2.Id, identifiable1.Id));

            result.Text.Add(new PlainText("text"));
            result.Text.Add(new CDataTag("cdata text"));
            result.Text.Add(new CommentTag("comment text"));
            result.Text.Add(new ProcessingInstructionTag("instruction", "instruction text"));
            result.Text.Add(this.CreateMrk(noteId));

            identifiable1 = this.CreateSm(noteId);
            result.Text.Add((ResourceStringContent)identifiable1);
            result.Text.Add(this.CreateEm(identifiable1.Id));

            return result;
        }

        /// <summary>
        /// Creates a target with every element and attribute.
        /// </summary>
        /// <param name="noteId">The Id of a note to reference.</param>
        /// <returns>The created target.</returns>
        private Target CreateTarget(string noteId)
        {
            IIdentifiable identifiable1;
            IIdentifiable identifiable2;
            Target result;

            result = new Target();
            result.Language = "de-de";
            result.Space = Preservation.Preserve;
            result.Order = 1;
            result.Text.Add(new CodePoint(24));
            identifiable1 = this.CreatePh(null);
            result.Text.Add((ResourceStringContent)identifiable1);
            result.Text.Add(this.CreatePh(identifiable1.Id));
            identifiable1 = this.CreatePc(null);
            result.Text.Add((ResourceStringContent)identifiable1);
            result.Text.Add(this.CreatePc(identifiable1.Id));

            identifiable1 = this.CreateSc(null);
            result.Text.Add((ResourceStringContent)identifiable1);
            identifiable2 = this.CreateEc(null, identifiable1.Id);
            result.Text.Add((ResourceStringContent)identifiable2);
            result.Text.Add(this.CreateEc(null, null));

            identifiable1 = this.CreateSc(identifiable1.Id);
            result.Text.Add((ResourceStringContent)identifiable1);
            result.Text.Add(this.CreateEc(identifiable2.Id, identifiable1.Id));

            result.Text.Add(new PlainText("text"));
            result.Text.Add(new CDataTag("cdata text"));
            result.Text.Add(new CommentTag("comment text"));
            result.Text.Add(new ProcessingInstructionTag("instruction", "instruction text"));
            result.Text.Add(this.CreateMrk(noteId));

            identifiable1 = this.CreateSm(noteId);
            result.Text.Add((ResourceStringContent)identifiable1);
            result.Text.Add(this.CreateEm(identifiable1.Id));

            return result;
        }

        /// <summary>
        /// Creates a unit with every element and attribute.
        /// </summary>
        /// <returns>The created unit.</returns>
        private Unit CreateUnit()
        {
            Unit result;
            Ignorable ignorable;
            Note note;
            Segment segment;

            this.ids[typeof(Data)] = 0;

            result = new Unit("u" + this.GetNextId(typeof(Unit)));
            result.CanResegment = true;
            result.FormatStyle = FormatStyleValue.Anchor;
            result.Name = "unit";
            result.SizeInfoReference = "sizeref";
            result.SizeRestriction = "restriction";
            result.SourceDirectionality = ContentDirectionality.RTL;
            result.Space = Preservation.Preserve;
            result.StorageRestriction = "restriction";
            result.SubFormatStyle.Add("key1", "value1");
            result.SubFormatStyle.Add("key2", "value2");
            result.TargetDirectionality = ContentDirectionality.LTR;
            result.Translate = false;
            result.Type = "pre:type";

            this.AddExtension(result);
            note = this.AddNote(result);
            result.OriginalData = this.CreateOriginalData();
            segment = this.CreateSegment(note.Id);
            result.Resources.Add(segment);
            ignorable = this.CreateIgnorable(note.Id);
            ignorable.Target.Order = 2;
            result.Resources.Add(ignorable);

            result.Changes = this.CreateChangeTrackingModule_ChangeTrack();
            result.Glossary = this.CreateGlossary(segment.Id);
            result.Matches.Add(this.CreateMatch(note.Id));
            result.Matches[0].SourceReference = Utilities.MakeIri(segment.Id);
            result.Metadata = this.CreateMetadata();
            result.ProfileData = this.CreateSizeRestrictionModule_ProfileData();
            result.ResourceData = this.CreateResourceDataModule_ResourceData();
            result.ValidationRules = this.CreateValidationModule_Validation();

            return result;
        }

        /// <summary>
        /// Creates a <see cref="Rule"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="Rule"/>.</returns>
        private Rule CreateValidationModule_Rule()
        {
            Rule result;

            result = new Rule();
            result.IsPresent = "ispresent";
            result.ExistsInSource = true;
            result.CaseSensitive = true;
            result.Normalization = Modules.NormalizationValue.NFC;
            result.Disabled = false;
            result.Occurs = 1;

            return result;
        }

        /// <summary>
        /// Creates a <see cref="Validation"/> with every element and attribute.
        /// </summary>
        /// <returns>The created <see cref="Validation"/>.</returns>
        private Validation CreateValidationModule_Validation()
        {
            Validation result;

            result = new Validation();
            result.Rules.Add(this.CreateValidationModule_Rule());

            this.AddExtension(result);

            return result;
        }

        /// <summary>
        /// Gets the next Id to use for an element.
        /// </summary>
        /// <param name="type">The type of element whose Id to get.</param>
        /// <returns>The Id to use with the element.</returns>
        private int GetNextId(Type type)
        {
            int result;

            this.ids.TryGetValue(type, out result);
            result++;
            this.ids[type] = result;

            return result;
        }

        /// <summary>
        /// Compares a list of attributes and outputs the names of those that differe between the lists.
        /// </summary>
        /// <param name="indent">Indent level to use when writing log information.</param>
        /// <param name="expected">The list of attributes that are expected to be present.</param>
        /// <param name="actual">The list of actual attributes.</param>
        private void LogMismatchedAttributes(string indent, List<AttributeData> expected, List<IAttributeDataProvider> actual)
        {
            StringBuilder builder;
            List<string> actualNames;
            List<string> expectedNames;

            actualNames = new List<string>();
            foreach (IAttributeDataProvider data in actual)
            {
                actualNames.Add(string.Join(":", data.Prefix, data.Namespace, data.LocalName));
            }

            expectedNames = new List<string>();
            foreach (AttributeData data in expected)
            {
                expectedNames.Add(string.Join(":", data.Prefix, data.Namespace, data.LocalName));
            }

            builder = new StringBuilder();
            foreach (string name in expectedNames)
            {
                if (!actualNames.Contains(name))
                {
                    builder.AppendLine(indent + "  " + name);
                }
            }

            if (builder.Length > 0)
            {
                Console.WriteLine(indent + "Missing Attributes:");
                Console.WriteLine(builder);
            }

            builder.Clear();
            foreach (string name in actualNames)
            {
                if (!expectedNames.Contains(name))
                {
                    builder.AppendLine(indent + "  " + name);
                }
            }

            if (builder.Length > 0)
            {
                Console.WriteLine(indent + "Extra Attributes:");
                Console.WriteLine(builder);
            }
        }

        /// <summary>
        /// Verifies that the attribute data match what is expected. Asserts will be raised for any differences.
        /// </summary>
        /// <param name="indent">Indent level to use when writing log information.</param>
        /// <param name="expected">The list of attributes that are expected to be present.</param>
        /// <param name="actual">The list of actual attributes.</param>
        private void VerifyAttributeData(string indent, List<AttributeData> expected, IEnumerable<IAttributeDataProvider> actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, "Attribute list is not null.");
            }
            else
            {
                List<IAttributeDataProvider> actualAttributes;

                Assert.IsNotNull(actual, "Attribute list is null.");
                actualAttributes = actual.ToList();

                if (expected.Count != actualAttributes.Count)
                {
                    this.LogMismatchedAttributes(indent + "  ", expected, actualAttributes);
                    Assert.AreEqual(expected.Count, actualAttributes.Count, "Count is incorrect.");
                }

                for (int i = 0; i < expected.Count; i++)
                {
                    Console.WriteLine(indent + "Verifying attribute {0} ({1}={2})", i, actualAttributes[i].LocalName, actualAttributes[i].Value);
                    Assert.AreEqual(expected[i].HasValue, actualAttributes[i].HasValue, "HasValue is incorrect.");
                    Assert.AreEqual(expected[i].LocalName, actualAttributes[i].LocalName, "LocalName is incorrect.");
                    Assert.AreEqual(expected[i].Namespace, actualAttributes[i].Namespace, "Namespace is incorrect.");
                    Assert.AreEqual(expected[i].Prefix, actualAttributes[i].Prefix, "Prefix is incorrect.");
                    Assert.AreEqual(expected[i].Value, actualAttributes[i].Value, "Value is incorrect.");
                }
            }
        }

        /// <summary>
        /// Verifies that the attributes stored in an extension match what is expected. Asserts will be raised for any
        /// differences.
        /// </summary>
        /// <param name="expected">The attributes that are expected to be present.</param>
        /// <param name="actual">The extension that contains the actual attributes.</param>
        private void VerifyAttributes(List<IExtensionAttribute> expected, IExtension actual)
        {
            if (expected == null)
            {
                Assert.IsFalse(actual.HasAttributes, "HasAttributes is incorrect.");
                Assert.IsNull(actual.GetAttributes(), "Attributes should be null.");
            }
            else
            {
                IEnumerable<IExtensionAttribute> attributes;
                IList<IExtensionAttribute> attributesList;

                attributes = actual.GetAttributes();

                Assert.IsTrue(actual.HasAttributes, "HasAttributes is incorrect.");
                Assert.IsNotNull(attributes, "Attributes should not be null.");
                attributesList = attributes.ToList();
                Assert.AreEqual(expected.Count, attributesList.Count, "Attributes Count is incorrect.");

                for (int i = 0; i < expected.Count; i++)
                {
                    Assert.AreEqual(expected[i].LocalName, attributesList[i].LocalName, "Attribute[{0}] Name is incorrect.", i);
                    Assert.AreEqual(expected[i].Namespace, attributesList[i].Namespace, "Attribute[{0}] Namespace is incorrect.", i);
                    Assert.AreEqual(expected[i].Value, attributesList[i].Value, "Attribute[{0}] Value is incorrect.", i);
                }
            }
        }

        /// <summary>
        /// Verifies that the children match what is expected. Asserts will be raised for any differences.
        /// </summary>
        /// <param name="indent">Indent level to use when writing log information.</param>
        /// <param name="expected">The list of children that are expected to be present.</param>
        /// <param name="actual">The list of actual children.</param>
        private void VerifyChildren(string indent, List<XliffData> expected, IEnumerable<ElementInfo> actual)
        {
            if ((expected == null) || (expected.Count == 0))
            {
                Assert.IsTrue((actual == null) || (actual.Count() == 0), "Child list is not null.");
            }
            else
            {
                List<ElementInfo> childrenList;

                Assert.IsNotNull(actual, "Child list is null.");
                childrenList = actual.ToList();

                Assert.AreEqual(expected.Count, childrenList.Count, "Children Count is incorrect.");
                for (int i = 0; i < expected.Count; i++)
                {
                    Console.WriteLine(indent + "Verifying child {0} ({1})", i, childrenList[i].Element.GetType().Name);
                    Assert.AreEqual(expected[i].LocalName, childrenList[i].LocalName, "LocalName is incorrect.", i);
                    Assert.AreEqual(expected[i].Namespace, childrenList[i].Namespace, "Namespace is incorrect.", i);
                    Assert.AreEqual(expected[i].Prefix, childrenList[i].Prefix, "Prefix is incorrect.", i);
                    this.VerifyData(indent, expected[i], childrenList[i].Element);
                }
            }
        }

        /// <summary>
        /// Verifies that the data stored in an extension match what is expected. Asserts will be raised for any
        /// differences.
        /// </summary>
        /// <param name="indent">Indent level to use when writing log information.</param>
        /// <param name="expected">The data that are expected to be present.</param>
        /// <param name="actual">The element that contains the actual data.</param>
        private void VerifyData(string indent, XliffData expected, IXliffDataProvider actual)
        {
            Assert.AreEqual(expected.Type, actual.GetType(), "Type is incorrect.");
            Assert.AreEqual(expected.HasChildren, actual.HasXliffChildren, "HasXliffChildren is incorrect.");
            Assert.AreEqual(expected.HasText, actual.HasXliffText, "HasXliffText is incorrect.");

            indent += "  ";
            this.VerifyAttributeData(indent, expected.Attributes, actual.GetXliffAttributes());
            this.VerifyChildren(indent, expected.Children, actual.GetXliffChildren());
            Assert.AreEqual(expected.Text, actual.GetXliffText());
        }

        /// <summary>
        /// Verifies that the children stored in an extension match what is expected. Asserts will be raised for any
        /// differences.
        /// </summary>
        /// <param name="indent">Indent level to use when writing log information.</param>
        /// <param name="expected">The children that are expected to be present.</param>
        /// <param name="actual">The extension that contains the actual children.</param>
        private void VerifyExtensionChildren(string indent, List<XliffData> expected, IExtension actual)
        {
            if (expected == null)
            {
                Assert.IsFalse(actual.HasChildren, "HasChildren is incorrect.");
            }
            else
            {
                Assert.IsTrue(actual.HasChildren, "HasChildren is incorrect.");
                this.VerifyChildren(indent, expected, actual.GetChildren());
            }
        }
        #endregion Helper Methods

        /// <summary>
        /// This class is used as an extension attribute.
        /// </summary>
        private class TestAttribute : IExtensionAttribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TestAttribute"/> class.
            /// </summary>
            /// <param name="prefix">The Xml prefix of the member.</param>
            /// <param name="ns">The namespace of the member.</param>
            /// <param name="localName">The local name of the member.</param>
            /// <param name="value">The value of the attribute</param>
            public TestAttribute(string prefix, string ns, string localName, string value)
            {
                this.Namespace = ns;
                this.LocalName = localName;
                this.Value = value;
                this.Prefix = prefix;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="TestAttribute"/> class.
            /// </summary>
            /// <param name="ns">The namespace of the member.</param>
            /// <param name="localName">The local name of the member.</param>
            /// <param name="value">The value of the attribute</param>
            public TestAttribute(string ns, string localName, string value)
                : this(null, ns, localName, value)
            {
            }

            /// <summary>
            /// Gets the local name of the member.
            /// </summary>
            public string LocalName { get; set; }

            /// <summary>
            /// Gets the namespace of the member.
            /// </summary>
            public string Namespace { get; set; }

            /// <summary>
            /// Gets the Xml prefix of the member.
            /// </summary>
            public string Prefix { get; set; }

            /// <summary>
            /// Gets or sets the value of the attribute.
            /// </summary>
            public string Value { get; set; }
        }
    }
}