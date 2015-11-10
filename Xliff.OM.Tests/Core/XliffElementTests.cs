namespace Localization.Xliff.OM.Core.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Localization.Xliff.OM.Attributes;
    using Localization.Xliff.OM.Converters;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Extensibility.Tests;
    using Localization.Xliff.OM.Tests;
    using Localization.Xliff.OM.XmlNames;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the <see cref="XliffElement"/> class.
    /// </summary>
    [TestClass()]
    public class XliffElementTests
    {
        #region Test Classes
        /// <summary>
        /// This is a test class used within the tests for value inheritance testing.
        /// </summary>
        private class TestAncestorXliffElement : XliffElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TestAncestorXliffElement"/> class.
            /// </summary>
            public TestAncestorXliffElement()
            {
                this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
            }

            /// <summary>
            /// Gets or sets a value that is used to verify property access with inheritance from an ancestor
            /// using the same property name.
            /// </summary>
            [Converter(typeof(HexConverter))]
            [DefaultValue(1000)]
            [SchemaEntity("AncestorInheritedProperty", Requirement.Optional)]
            public int AncestorInheritedProperty
            {
                get { return (int)this.GetPropertyValue("AncestorInheritedProperty"); }
                set { this.SetPropertyValue(value, "AncestorInheritedProperty"); }
            }

            /// <summary>
            /// Gets or sets a value that is used to verify property access with inheritance from an ancestor.
            /// </summary>
            [Converter(typeof(HexConverter))]
            [DefaultValue(2000)]
            [SchemaEntity("Property", Requirement.Optional)]
            public int Property
            {
                get { return (int)this.GetPropertyValue("Property"); }
                set { this.SetPropertyValue(value, "Property"); }
            }
        }

        /// <summary>
        /// This is a test class used within the tests.
        /// </summary>
        [SchemaChild(NamespaceValues.Core, ElementNames.File, typeof(File))]
        [SchemaChild(NamespaceValues.Core, ElementNames.Source, typeof(Source))]
        private class TestXliffElement : XliffElement, IInheritanceInfoProvider
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TestXliffElement"/> class.
            /// </summary>
            public TestXliffElement()
            {
                this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
            }

            #region Properties
            /// <summary>
            /// Gets or sets a value that is used to verify property access with inheritance from an ancestor
            /// that is not the parent.
            /// </summary>
            [Converter(typeof(HexConverter))]
            [DefaultValue(100)]
            [InheritValue(Inheritance.AncestorType, typeof(TestAncestorXliffElement))]
            [SchemaEntity("AncestorInheritedProperty", Requirement.Optional)]
            public int AncestorInheritedProperty
            {
                get { return (int)this.GetPropertyValue("AncestorInheritedProperty"); }
                set { this.SetPropertyValue(value, "AncestorInheritedProperty"); }
            }

            /// <summary>
            /// Gets or sets a value that is used to verify property access with inheritance from an ancestor
            /// that is not the parent.
            /// </summary>
            [Converter(typeof(HexConverter))]
            [DefaultValue(100)]
            [InheritValue(Inheritance.AncestorType, typeof(TestAncestorXliffElement), "Property")]
            [SchemaEntity("AncestorInheritedPropertyDifferentName", Requirement.Optional)]
            public int AncestorInheritedPropertyDifferentName
            {
                get { return (int)this.GetPropertyValue("AncestorInheritedPropertyDifferentName"); }
                set { this.SetPropertyValue(value, "AncestorInheritedPropertyDifferentName"); }
            }

            [Converter(typeof(HexConverter))]
            [DefaultValue(100)]
            [InheritValue(Inheritance.Callback)]
            [SchemaEntity("CallbackInheritedProperty", Requirement.Optional)]
            public int CallbackInheritedProperty
            {
                get { return (int)this.GetPropertyValue("NotInheritedProperty"); }
                set { this.SetPropertyValue(value, "NotInheritedProperty"); }
            }

            /// <summary>
            /// Gets or sets the document that is used to test.
            /// </summary>
            public XliffDocument Document { get; set; }

            /// <summary>
            /// Gets or sets a value that is used to verify property access with inheritance from the parent.
            /// </summary>
            [Converter(typeof(HexConverter))]
            [DefaultValue(100)]
            [InheritValue(Inheritance.Parent)]
            [SchemaEntity("ParentInheritedProperty", Requirement.Optional)]
            public int ParentInheritedProperty
            {
                get { return (int)this.GetPropertyValue("ParentInheritedProperty"); }
                set { this.SetPropertyValue(value, "ParentInheritedProperty"); }
            }

            /// <summary>
            /// Gets or sets a value that is used to verify property access.
            /// </summary>
            [Converter(typeof(HexConverter))]
            [DefaultValue(100)]
            [SchemaEntity("Property", Requirement.Optional)]
            public int Property
            {
                get { return (int)this.GetPropertyValue("Property"); }
                set { this.SetPropertyValue(value, "Property"); }
            }
            #endregion Properties

            /// <summary>
            /// Adds a list of <see cref="XliffElement"/>s to another list as a key-value pair. The key is the name
            /// to assign to the elements, and the value is the element. The name of the object comes from the children
            /// types defined on the object.
            /// </summary>
            /// <param name="elements">The elements to add to the list. This should be a list of
            /// <see cref="XliffElement"/> derived objects.</param>
            /// <param name="list">The list to add the elements to. If this value is null, the list will be
            /// created.</param>
            public void CallAddChildElementsToList(IEnumerable elements, ref List<ElementInfo> list)
            {
                this.AddChildElementsToList(elements, ref list);
            }

            /// <summary>
            /// Gets the value of a property.
            /// </summary>
            /// <param name="property">The name of the property to get.</param>
            /// <returns>The value of the attribute in its native type.</returns>
            public object CallGetPropertyValue(string property)
            {
                return this.GetPropertyValue(property);
            }

            /// <summary>
            /// Selects an <see cref="XliffElement"/> matching the selection query.
            /// </summary>
            /// <param name="path">The selection query that is relative to the current object.</param>
            /// <returns>The object that was selected from the query path, or null if no match was found.</returns>
            /// <example>The value of <paramref name="path"/> might look something like
            /// "g=group1/f=file1/u=unit1/n=note1"
            /// which is a relative path from the current object, not a full path from the document root.</example>
            public ISelectable CallSelectElement(string path)
            {
                return this.SelectElement(path);
            }

            /// <summary>
            /// Gets the value of a property.
            /// </summary>
            /// <param name="value">The value to set.</param>
            /// <param name="property">The name of the property to set.</param>
            public void CallSetPropertyValue(object value, string property)
            {
                this.SetPropertyValue(value, property);
            }

            /// <summary>
            /// Gets the child <see cref="XliffElement"/>s contained within this object.
            /// </summary>
            /// <returns>A list of child elements stored as a pair consisting of the XLIFF name for the child and
            /// the child itself.</returns>
            protected override List<ElementInfo> GetChildren()
            {
                ElementInfo child;

                child = new ElementInfo(new XmlNameInfo("doc"), this.Document);

                return new List<ElementInfo>() { child };
            }

            #region IInheritanceInfoProvider Implementation
            /// <summary>
            /// Method called to provide custom inheritance information. This is typically used when the inheritance
            /// depends on runtime information.
            /// </summary>
            /// <param name="property">The name of the property being retrieved.</param>
            /// <returns>The value of the property.</returns>
            InheritanceInfo IInheritanceInfoProvider.GetInheritanceInfo(string property)
            {
                Assert.AreEqual(property, "CallbackInheritedProperty", "Property is incorrect.");
                return new InheritanceInfo((e, p) => 40);
            }
            #endregion IInheritanceInfoProvider Implementation
        }
        #endregion Test Classes

        #region Test Methods
        /// <summary>
        /// Tests the Test AddChildElementsToList method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_AddChildElementsToList()
        {
            TestXliffElement element;
            List<ElementInfo> list;
            List<XliffElement> children;

            element = new TestXliffElement();
            children = new List<XliffElement>();

            Console.WriteLine("Test with null.");
            list = null;
            element.CallAddChildElementsToList(null, ref list);
            Assert.IsNull(list, "List is incorrect.");

            Console.WriteLine("Test with empty enumeration.");
            list = null;
            element.CallAddChildElementsToList(new XliffElement[] { }, ref list);
            Assert.IsNull(list, "List is incorrect.");

            Console.WriteLine("Test with invalid enumeration.");
            children.Add(new Target());
            list = null;
            try
            {
                element.CallAddChildElementsToList(children, ref list);
                Assert.Fail("Expected KeyNotFoundException to be thrown.");
            }
            catch (KeyNotFoundException)
            {
            }

            Console.WriteLine("Test with valid enumeration.");
            children.Clear();
            children.Add(new File());
            children.Add(new File());
            children.Add(new Source());
            list = null;
            element.CallAddChildElementsToList(children, ref list);
            Assert.AreEqual(3, list.Count, "List count is incorrect.");
            Assert.AreEqual(ElementNames.File, list[0].LocalName, "LocalName[0] is incorrect.");
            Assert.AreEqual(ElementNames.File, list[1].LocalName, "LocalName[1] is incorrect.");
            Assert.AreEqual(ElementNames.Source, list[2].LocalName, "LocalName[2] is incorrect.");
            Assert.AreEqual(children[0], list[0].Element, "Element[0] is incorrect.");
            Assert.AreEqual(children[1], list[1].Element, "Element[1] is incorrect.");
            Assert.AreEqual(children[2], list[2].Element, "Element[2] is incorrect.");

            Console.WriteLine("Test with full list.");
            list = new List<ElementInfo>();
            list.Add(new ElementInfo(new XmlNameInfo("name"), new TestXliffElement()));
            children.Clear();
            children.Add(new File());
            element.CallAddChildElementsToList(children, ref list);
            Assert.AreEqual(2, list.Count, "List count is incorrect.");
            Assert.AreEqual("name", list[0].LocalName, "LocalName[0] is incorrect.");
            Assert.AreEqual(ElementNames.File, list[1].LocalName, "Key[1] is incorrect.");
            Assert.AreEqual(children[0], list[1].Element, "Value[1] is incorrect.");
        }

        /// <summary>
        /// Tests the CollapseChildren method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_CollapseChildren()
        {
            List<PlainText> textList;
            List<ResourceStringContent> tagList;
            List<Source> sourceList;
            List<Target> targetList;
            List<XliffElement> elementList;
            PlainText text;
            Segment segment;
            StandaloneCode code;
            Unit unit;
            XliffDocument document;

            code = new StandaloneCode();
            text = new PlainText();
            segment = new Segment();
            segment.Source = new Source();
            segment.Source.Text.Add(text);
            segment.Source.Text.Add(code);

            unit = new Unit();
            unit.Resources.Add(segment);

            document = new XliffDocument();
            document.Files.Add(new File());
            document.Files[0].Containers.Add(unit);

            elementList = document.CollapseChildren<XliffElement>();
            Assert.AreEqual(6, elementList.Count, "Element count is incorrect.");
            Assert.IsTrue(elementList.Contains(code), "StandaloneCode not found in list");
            Assert.IsFalse(elementList.Contains(document), "Document not found in list");
            Assert.IsTrue(elementList.Contains(document.Files[0]), "File not found in list");
            Assert.IsTrue(elementList.Contains(segment), "Segment not found in list");
            Assert.IsTrue(elementList.Contains(segment.Source), "Source not found in list");
            Assert.IsTrue(elementList.Contains(text), "PlainText not found in list");
            Assert.IsTrue(elementList.Contains(unit), "Unit not found in list");

            sourceList = document.CollapseChildren<Source>();
            Assert.AreEqual(1, sourceList.Count, "Element count is incorrect.");
            Assert.IsTrue(sourceList.Contains(segment.Source), "Source not found in list");

            tagList = document.CollapseChildren<ResourceStringContent>();
            Assert.AreEqual(2, tagList.Count, "Element count is incorrect.");
            Assert.IsTrue(tagList.Contains(code), "StandaloneCode not found in list");
            Assert.IsTrue(tagList.Contains(text), "PlainText not found in list");

            targetList = document.CollapseChildren<Target>();
            Assert.AreEqual(0, targetList.Count, "Element count is incorrect.");

            textList = document.CollapseChildren<PlainText>();
            Assert.AreEqual(1, textList.Count, "Element count is incorrect.");
            Assert.IsTrue(textList.Contains(text), "PlainText not found in list");
        }

        /// <summary>
        /// Tests the CollapseChildren method using a <see cref="CollapseScope"/>.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_CollapseChildrenWithScope()
        {
            IExtensible extensible;
            CustomExtension extension1;
            CustomExtension extension2;
            CustomElement1 element1;
            CustomElement2 element2;
            List<XliffElement> elementList;
            List<Source> sourceList;
            PlainText text;
            Segment segment;
            Source source;
            StandaloneCode code;
            Unit unit;
            XliffDocument document;

            code = new StandaloneCode();
            text = new PlainText();
            segment = new Segment();
            segment.Source = new Source();
            segment.Source.Text.Add(text);
            segment.Source.Text.Add(code);

            unit = new Unit();
            unit.Resources.Add(segment);

            document = new XliffDocument();
            document.Files.Add(new File());
            document.Files[0].Containers.Add(unit);

            extensible = document.Files[0];

            extension1 = new CustomExtension("namespace");
            element1 = new CustomElement1("pre1", "namespace");
            extension1.AddChild(new ElementInfo(new XmlNameInfo("localname"), element1));
            extensible.Extensions.Add(extension1);

            extension2 = new CustomExtension("namespace");
            element2 = new CustomElement2("pre1", "namespace");
            extension2.AddChild(new ElementInfo(new XmlNameInfo("localname"), element2));
            source = new Source();
            extension2.AddChild(new ElementInfo(new XmlNameInfo("localname"), source));
            extensible.Extensions.Add(extension2);

            Console.WriteLine("Test with none.");
            elementList = document.CollapseChildren<XliffElement>(CollapseScope.None);
            Assert.AreEqual(0, elementList.Count, "Element count is incorrect.");

            Console.WriteLine("Test with current element.");
            elementList = document.CollapseChildren<XliffElement>(CollapseScope.CurrentElement);
            Assert.AreEqual(1, elementList.Count, "Element count is incorrect.");
            Assert.IsTrue(elementList.Contains(document), "Document not found in list");

            Console.WriteLine("Test with default.");
            elementList = document.CollapseChildren<XliffElement>(CollapseScope.Default);
            Assert.AreEqual(6, elementList.Count, "Element count is incorrect.");
            Assert.IsTrue(elementList.Contains(code), "StandaloneCode not found in list");
            Assert.IsFalse(elementList.Contains(document), "Document not found in list");
            Assert.IsTrue(elementList.Contains(document.Files[0]), "File not found in list");
            Assert.IsTrue(elementList.Contains(segment), "Segment not found in list");
            Assert.IsTrue(elementList.Contains(segment.Source), "Source not found in list");
            Assert.IsTrue(elementList.Contains(text), "PlainText not found in list");
            Assert.IsTrue(elementList.Contains(unit), "Unit not found in list");

            Console.WriteLine("Test with default with current element.");
            elementList = document.CollapseChildren<XliffElement>(CollapseScope.Default | CollapseScope.CurrentElement);
            Assert.AreEqual(7, elementList.Count, "Element count is incorrect.");
            Assert.IsTrue(elementList.Contains(document), "Document not found in list");
            Assert.IsTrue(elementList.Contains(code), "StandaloneCode not found in list");
            Assert.IsTrue(elementList.Contains(document.Files[0]), "File not found in list");
            Assert.IsTrue(elementList.Contains(segment), "Segment not found in list");
            Assert.IsTrue(elementList.Contains(segment.Source), "Source not found in list");
            Assert.IsTrue(elementList.Contains(text), "PlainText not found in list");
            Assert.IsTrue(elementList.Contains(unit), "Unit not found in list");

            elementList = document.CollapseChildren<XliffElement>(CollapseScope.Extensions | CollapseScope.CoreElements | CollapseScope.AllDescendants);
            Assert.AreEqual(9, elementList.Count, "Element count is incorrect.");
            Assert.IsTrue(elementList.Contains(code), "StandaloneCode not found in list");
            Assert.IsTrue(elementList.Contains(document.Files[0]), "File not found in list");
            Assert.IsTrue(elementList.Contains(segment), "Segment not found in list");
            Assert.IsTrue(elementList.Contains(segment.Source), "Source not found in list");
            Assert.IsTrue(elementList.Contains(text), "PlainText not found in list");
            Assert.IsTrue(elementList.Contains(unit), "Unit not found in list");
            Assert.IsTrue(elementList.Contains(source), "Source not found in list");
            Assert.IsTrue(elementList.Contains(element1), "Element1 not found in list");
            Assert.IsTrue(elementList.Contains(element2), "Element2 not found in list");

            sourceList = document.CollapseChildren<Source>(CollapseScope.Extensions | CollapseScope.CoreElements | CollapseScope.AllDescendants);
            Assert.AreEqual(2, sourceList.Count, "Element count is incorrect.");
            Assert.IsTrue(sourceList.Contains(segment.Source), "Source not found in list");
            Assert.IsTrue(sourceList.Contains(source), "Source not found in list");

            elementList = document.CollapseChildren<XliffElement>(CollapseScope.All);
            Assert.AreEqual(10, elementList.Count, "Element count is incorrect.");
            Assert.IsTrue(elementList.Contains(document), "Document not found in list");
            Assert.IsTrue(elementList.Contains(code), "StandaloneCode not found in list");
            Assert.IsTrue(elementList.Contains(document.Files[0]), "File not found in list");
            Assert.IsTrue(elementList.Contains(segment), "Segment not found in list");
            Assert.IsTrue(elementList.Contains(segment.Source), "Source not found in list");
            Assert.IsTrue(elementList.Contains(text), "PlainText not found in list");
            Assert.IsTrue(elementList.Contains(unit), "Unit not found in list");
            Assert.IsTrue(elementList.Contains(source), "Source not found in list");
            Assert.IsTrue(elementList.Contains(element1), "Element1 not found in list");
            Assert.IsTrue(elementList.Contains(element2), "Element2 not found in list");

            sourceList = document.CollapseChildren<Source>(CollapseScope.All);
            Assert.AreEqual(2, sourceList.Count, "Element count is incorrect.");
            Assert.IsTrue(sourceList.Contains(source), "Source not found in list");
            Assert.IsTrue(sourceList.Contains(segment.Source), "Source not found in list");

            Console.WriteLine("Test with extensions with core elements for Source.");
            sourceList = document.CollapseChildren<Source>(CollapseScope.Extensions | CollapseScope.AllDescendants);
            Assert.AreEqual(1, sourceList.Count, "Element count is incorrect.");
            Assert.IsTrue(sourceList.Contains(source), "Source not found in list");

            Console.WriteLine("Test with extensions without core elements.");
            elementList = document.CollapseChildren<XliffElement>(CollapseScope.Extensions | CollapseScope.AllDescendants);
            Assert.AreEqual(3, elementList.Count, "Element count is incorrect.");
            Assert.IsTrue(elementList.Contains(element1), "Element1 not found in list");
            Assert.IsTrue(elementList.Contains(element2), "Element2 not found in list");
            Assert.IsTrue(elementList.Contains(source), "Source not found in list");

            elementList = document.CollapseChildren<XliffElement>(CollapseScope.TopLevelDescendants);
            Assert.AreEqual(0, elementList.Count, "Element count is incorrect.");

            elementList = document.CollapseChildren<XliffElement>(CollapseScope.TopLevelDescendants | CollapseScope.CoreElements);
            Assert.AreEqual(1, elementList.Count, "Element count is incorrect.");
            Assert.IsTrue(elementList.Contains(document.Files[0]), "File not found in list");
        }

        /// <summary>
        /// Tests the GetPropertyValue and SetPropertyValue methods.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_GetSetPropertyValue()
        {
            TestAncestorXliffElement ancestor;
            TestXliffElement element;
            TestXliffElement parent;
            const string propertyName = "Property";

            element = new TestXliffElement();

            Console.WriteLine("Test with default value.");
            Assert.AreEqual(100, element.CallGetPropertyValue(propertyName), "Value is incorrect.");

            Console.WriteLine("Test with new value.");
            element.CallSetPropertyValue(200, propertyName);
            Assert.AreEqual(200, element.CallGetPropertyValue(propertyName), "Value is incorrect.");

            Console.WriteLine("Test with parent inherited value.");
            parent = new TestXliffElement();
            parent.ParentInheritedProperty = 302;
            element.Parent = parent;
            Assert.AreEqual(parent.ParentInheritedProperty,
                            element.CallGetPropertyValue("ParentInheritedProperty"),
                            "Value is incorrect.");

            Console.WriteLine("Test with ancestor inherited value.");
            parent = new TestXliffElement();
            element.Parent = parent;
            ancestor = new TestAncestorXliffElement();
            ancestor.AncestorInheritedProperty = 500;
            parent.Parent = ancestor;
            Assert.AreEqual(ancestor.AncestorInheritedProperty,
                            element.CallGetPropertyValue("AncestorInheritedProperty"),
                            "Value is incorrect.");

            Console.WriteLine("Test with ancestor inherited value with a different name.");
            parent = new TestXliffElement();
            element.Parent = parent;
            ancestor = new TestAncestorXliffElement();
            ancestor.Property = 600;
            parent.Parent = ancestor;
            Assert.AreEqual(ancestor.Property,
                            element.CallGetPropertyValue("AncestorInheritedPropertyDifferentName"),
                            "Value is incorrect.");

            Console.WriteLine("Test with callback inherited value.");
            Assert.AreEqual(40,
                            element.CallGetPropertyValue("CallbackInheritedProperty"),
                            "Value is incorrect.");
        }

        /// <summary>
        /// Tests the Parent property.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_Parent()
        {
            TestXliffElement element;
            TestXliffElement parent;

            Console.WriteLine("Test default value.");
            element = new TestXliffElement();
            Assert.IsNull(element.Parent, "Parent is not null.");

            Console.WriteLine("Test with parent.");
            parent = new TestXliffElement();
            element.Parent = parent;
            Assert.AreEqual(parent, element.Parent, "Parent was not set.");
        }

        /// <summary>
        /// Tests the SelectableAncestor property.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_SelectableAncestor()
        {
            TestXliffElement element;
            TestXliffElement parent;

            Console.WriteLine("Test default value.");
            element = new TestXliffElement();
            Assert.IsNull(element.SelectableAncestor, "SelectableAncestor is not null.");

            Console.WriteLine("Test with parent that is not selectable.");
            parent = new TestXliffElement();
            element.Parent = parent;
            Assert.IsNull(element.SelectableAncestor, "SelectableAncestor is not null.");

            Console.WriteLine("Test with parent that is selectable.");
            parent.Parent = new File();
            Assert.AreEqual(parent.Parent, element.SelectableAncestor, "SelectableAncestor is incorrect.");
        }

        /// <summary>
        /// Tests the SelectElement method.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_SelectElement()
        {
            File file;
            Group group;
            Unit unit;
            Segment segment;
            Ignorable ignorable;
            TestXliffElement element;

            element = new TestXliffElement();
            element.Document = new XliffDocument();

            Console.WriteLine("Test with no items.");
            Assert.IsNull(element.CallSelectElement("#/file"));

            Console.WriteLine("Test with file items.");
            file = new File("file");
            element.Document.Files.Add(file);
            Assert.AreEqual(file, element.CallSelectElement("#/f=file"));

            Console.WriteLine("Test with relative path to file item.");
            Assert.AreEqual(file, element.Document.Select("#f=file"));

            Console.WriteLine("Test with many items.");
            group = new Group("group");
            file.Containers.Add(new Group());
            file.Containers.Add(group);

            unit = new Unit("unit");
            group.Containers.Add(unit);
            group.Containers.Add(new Group());
            group.Containers.Add(new Unit());
            Assert.AreEqual(unit, element.CallSelectElement("#/f=file/g=group/u=unit"));

            Console.WriteLine("Test with segment.");
            segment = new Segment("segment");
            unit.Resources.Add(segment);
            Assert.AreEqual(segment, element.CallSelectElement("#/f=file/g=group/u=unit/segment"));

            Console.WriteLine("Test with relative path to segment.");
            Assert.AreEqual(segment, group.Select("#u=unit/segment"));

            Console.WriteLine("Test with invalid relative path to segment.");
            try
            {
                group.Select("u=unit/segment");
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (ArgumentException)
            {
            }

            Console.WriteLine("Test with ignorable.");
            ignorable = new Ignorable("ignorable");
            unit.Resources.Add(ignorable);
            Assert.AreEqual(ignorable, element.CallSelectElement("#/f=file/g=group/u=unit/ignorable"));

            Console.WriteLine("Test with data.");
            unit.OriginalData = new OriginalData();
            unit.OriginalData.AddData("data", "text");
            Assert.AreEqual(unit.OriginalData.DataElements[0],
                            element.CallSelectElement("#/f=file/g=group/u=unit/d=data"));

            Console.WriteLine("Test with note.");
            file.AddNote("note").Id = "note";
            Assert.AreEqual(file.Notes[0], element.CallSelectElement("#/f=file/n=note"));

            //
            // This test should still work even though it's an invalid document.
            //

            Console.WriteLine("Test with duplicate names.");
            element.Document.Files.Add(new File("file"));
            Assert.AreEqual(file, element.CallSelectElement("#/f=file"), "Found wrong item.");

            Console.WriteLine("Test with bogus names.");
            Assert.IsNull(element.CallSelectElement("#/f=foo"), "Item should not have been found.");

            Console.WriteLine("Test with bogus path.");
            Assert.IsNull(element.CallSelectElement("#/blah"), "Item should not have been found.");

            Console.WriteLine("Test with empty string.");
            // This returns null because SelectElement is called directly which doesn't search for the leading '#'.
            Assert.IsNull(element.CallSelectElement(String.Empty));

            Console.WriteLine("Test with empty path.");
            Assert.IsNull(element.CallSelectElement("/"), "Item should not have been found.");

            Console.WriteLine("Test with inline element.");
            segment.Source = new Source();
            segment.Source.Text.Add(new MarkedSpan("ms1"));
            Assert.AreEqual(segment.Source.Text[0],
                            element.CallSelectElement("#/f=file/g=group/u=unit/ms1"),
                            "Item should have been found.");

            Console.WriteLine("Test with inline element.");
            segment.Target = new Target();
            segment.Target.Text.Add(new MarkedSpan("ms2"));
            Assert.AreEqual(segment.Target.Text[0],
                            element.CallSelectElement("#/f=file/g=group/u=unit/t=ms2"),
                            "Item should have been found.");

            Console.WriteLine("Test with relative path to inline element from file.");
            Assert.AreEqual(segment.Target.Text[0],
                            file.Select("#g=group/u=unit/t=ms2"),
                            "Item should have been found.");

            Console.WriteLine("Test with relative path to inline element from unit.");
            Assert.AreEqual(segment.Target.Text[0],
                            unit.Select("#t=ms2"),
                            "Item should have been found.");
        }

        /// <summary>
        /// Tests getting the SelectorPath for core elements.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_SelectorPath()
        {
            MarkedSpan span;
            Group group;
            Unit unit;
            Segment segment;
            XliffDocument document;

            span = new MarkedSpan("mrk1");

            segment = new Segment();
            segment.Source = new Source();
            segment.Source.Text.Add(span);

            unit = new Unit("u1");
            unit.Resources.Add(segment);

            group = new Group("g1");
            group.Containers.Add(unit);

            document = new XliffDocument();
            document.Files.Add(new File("f1"));
            document.Files[0].Containers.Add(group);

            Console.WriteLine("Test for document.");
            Assert.AreEqual("#", document.SelectorPath, "SelectorPath is incorrect.");

            Console.WriteLine("Test for file.");
            Assert.AreEqual("#/f=f1", document.Files[0].SelectorPath, "SelectorPath is incorrect.");

            Console.WriteLine("Test for group.");
            Assert.AreEqual("#/f=f1/g=g1", group.SelectorPath, "SelectorPath is incorrect.");

            Console.WriteLine("Test for unit.");
            Assert.AreEqual("#/f=f1/g=g1/u=u1", unit.SelectorPath, "SelectorPath is incorrect.");
        }

        /// <summary>
        /// Tests getting the SelectorPath for inline tags within Source elements.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_SelectorPath_Source()
        {
            MarkedSpan span;
            Group group;
            Unit unit;
            Segment segment;
            XliffDocument document;

            span = new MarkedSpan("mrk1");

            segment = new Segment();
            segment.Source = new Source();
            segment.Source.Text.Add(span);

            unit = new Unit("u1");
            unit.Resources.Add(segment);

            group = new Group("g1");
            group.Containers.Add(unit);

            document = new XliffDocument();
            document.Files.Add(new File("f1"));
            document.Files[0].Containers.Add(group);

            Console.WriteLine("Test with segment without Id.");
            Assert.AreEqual("#/f=f1/g=g1/u=u1/mrk1", span.SelectorPath, "SelectorPath is incorrect.");

            Console.WriteLine("Test with all items with Id.");
            segment.Id = "s1";
            Assert.AreEqual("#/f=f1/g=g1/u=u1/mrk1", span.SelectorPath, "SelectorPath is incorrect.");

            Console.WriteLine("Test with tag without Id.");
            span.Id = null;
            Assert.AreEqual("#/f=f1/g=g1/u=u1/s1", span.SelectorPath, "SelectorPath is incorrect.");

            Console.WriteLine("Test with tag and segment without Id.");
            segment.Id = null;
            span.Id = null;
            Assert.AreEqual("#/f=f1/g=g1/u=u1", span.SelectorPath, "SelectorPath is incorrect.");
        }

        /// <summary>
        /// Tests getting the SelectorPath for inline tags within Target elements.
        /// </summary>
        [TestMethod()]
        [TestCategory(TestUtilities.UnitTestCategory)]
        public void XliffElement_SelectorPath_Target()
        {
            MarkedSpan span;
            Group group;
            Unit unit;
            Segment segment;
            XliffDocument document;

            span = new MarkedSpan("mrk1");

            segment = new Segment();
            segment.Target = new Target();
            segment.Target.Text.Add(span);

            unit = new Unit("u1");
            unit.Resources.Add(segment);

            group = new Group("g1");
            group.Containers.Add(unit);

            document = new XliffDocument();
            document.Files.Add(new File("f1"));
            document.Files[0].Containers.Add(group);

            Console.WriteLine("Test with segment without Id.");
            Assert.AreEqual("#/f=f1/g=g1/u=u1/t=mrk1", span.SelectorPath, "SelectorPath is incorrect.");

            Console.WriteLine("Test with all items with Id.");
            segment.Id = "s1";
            Assert.AreEqual("#/f=f1/g=g1/u=u1/t=mrk1", span.SelectorPath, "SelectorPath is incorrect.");

            Console.WriteLine("Test with tag without Id.");
            span.Id = null;
            Assert.AreEqual("#/f=f1/g=g1/u=u1/s1", span.SelectorPath, "SelectorPath is incorrect.");

            Console.WriteLine("Test with tag and segment without Id.");
            segment.Id = null;
            span.Id = null;
            Assert.AreEqual("#/f=f1/g=g1/u=u1", span.SelectorPath, "SelectorPath is incorrect.");
        }


        #endregion Test Methods
    }
}
