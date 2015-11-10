namespace Localization.Xliff.OM.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Exceptions;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Validators;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class writes XLIFF 2.0 documents.
    /// </summary>
    public class XliffWriter
    {
        #region Member Variables
        /// <summary>
        /// The settings describing how to write content.
        /// </summary>
        private readonly XliffWriterSettings settings;

        /// <summary>
        /// The document to serialize.
        /// </summary>
        private XliffDocument document;

        /// <summary>
        /// The stream to write to.
        /// </summary>
        private XmlWriter writer;
        #endregion Member Variables

        /// <summary>
        /// Initializes a new instance of the <see cref="XliffWriter"/> class using default settings.
        /// </summary>
        public XliffWriter()
            : this(new XliffWriterSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XliffWriter"/> class.
        /// </summary>
        /// <param name="settings">The settings describing how to write content.</param>
        public XliffWriter(XliffWriterSettings settings)
        {
            ArgValidator.Create(settings, "settings").IsNotNull();

            this.settings = settings;
        }

        #region Static Methods
        /// <summary>
        /// Gets the children from a provider grouped by the type specified by the provider.
        /// </summary>
        /// <param name="provider">The provider that contains the children.</param>
        /// <returns>A dictionary whose key is the type of the objects contained in the value. The value is a list of
        /// objects that should be output for the type. Objects that have no corresponding type will be grouped with a
        /// created key of type object.</returns>
        private static Dictionary<Type, List<ElementInfo>> GetProviderChildrenGroupedByType(IXliffDataProvider provider)
        {
            IEnumerable<ElementInfo> children;
            IEnumerable<OutputItem> order;
            Dictionary<Type, List<ElementInfo>> result;
            List<ElementInfo> unorderedItems;

            result = new Dictionary<Type, List<ElementInfo>>();
            order = provider.XliffOutputOrder;
            unorderedItems = new List<ElementInfo>();

            children = provider.GetXliffChildren();
            if (children != null)
            {
                // Create a list for each type so all items of the same type are grouped together.
                foreach (OutputItem item in order)
                {
                    if (item.ItemType == OutputItemType.Child)
                    {
                        result[item.ChildType] = new List<ElementInfo>();
                    }
                }

                // Add each child to the appropriate list.
                foreach (ElementInfo child in children)
                {
                    List<ElementInfo> list;

                    if (result.TryGetValue(child.Element.GetType(), out list))
                    {
                        list.Add(child);
                    }
                    else
                    {
                        bool added;

                        added = false;

                        // The class may derive from another so try checking the runtime type.
                        foreach (Type key in result.Keys)
                        {
                            if (key.IsInstanceOfType(child.Element))
                            {
                                result[key].Add(child);
                                added = true;
                                break;
                            }
                        }

                        if (!added)
                        {
                            // Order doesn't matter so append it.
                            unorderedItems.Add(child);
                        }
                    }
                }
            }

            // Merge all the unordered items to a generic group. If object is already specified as a key by the 
            // provider then there won't be any unordered items because all items will at least be instances of objects.
            if (unorderedItems.Count > 0)
            {
                result[typeof(object)] = unorderedItems;
            }

            return result;
        }
        #endregion Static Methods

        #region Methods
        /// <summary>
        /// Adds a value to a dictionary if an entry for it doesn't already exist. If the key for the value already
        /// exists and the value differs, an exception is thrown.
        /// </summary>
        /// <param name="dictionary">The dictionary to update.</param>
        /// <param name="prefix">The key of the entry to add.</param>
        /// <param name="ns">The value to add.</param>
        private void AddNamespaceOrThrowIfMismatch(Dictionary<string, string> dictionary, string prefix, string ns)
        {
            // The prefix may be null for native xliff elements.
            if (prefix != null)
            {
                string dictValue;

                if (dictionary.TryGetValue(prefix, out dictValue))
                {
                    if (dictValue != ns)
                    {
                        string message;

                        message = string.Format(Properties.Resources.XliffWriter_PrefixInUse_Format, prefix, dictValue);
                        throw new InvalidOperationException(message);
                    }
                }
                else
                {
                    dictionary.Add(prefix, ns);
                }
            }
        }

        /// <summary>
        /// Gets the set of namespace attributes to associate with the document based on the contents of the document.
        /// </summary>
        /// <returns>A dictionary whose key is the namespace Name and the value is the namespace value that is to
        /// be set when creating the document element.</returns>
        /// <example>
        /// An example dictionary:
        /// { MTC => URN:OASIS:NAMES:TC:XLIFF:MATCHES:2.0 }
        /// { MDA => URN:OASIS:NAMES:TC:XLIFF:METADATA:2.0 }.
        /// </example>
        private Dictionary<string, string> GetNamespaceAttributes()
        {
            Dictionary<string, string> result;
            Dictionary<string, string> namespaces;

            namespaces = new Dictionary<string, string>();
            this.GetXmlElementNamespaces(this.document, namespaces);
            this.GetXmlElementNamespaces(((IXliffDataProvider)this.document).GetXliffChildren(), namespaces, false);

            result = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in namespaces)
            {
                switch (pair.Value)
                {
                    case NamespaceValues.GlossaryModule:
                        result[NamespacePrefixes.GlossaryModule] = NamespaceValues.GlossaryModule;
                        break;

                    case NamespaceValues.TranslationCandidatesModule:
                        result[NamespacePrefixes.TranslationCandidatesModule] = NamespaceValues.TranslationCandidatesModule;
                        break;

                    case NamespaceValues.MetadataModule:
                        result[NamespacePrefixes.MetadataModule] = NamespaceValues.MetadataModule;
                        break;

                    case NamespaceValues.Core:
                        // Default namespace that doesn't need to be included.
                        break;

                    default:
                        result[pair.Key] = pair.Value;
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Enumerates all children and all descendants storing the set of unique Xml namespaces used for serializing
        /// the elements.
        /// </summary>
        /// <param name="children">The children to enumerate over.</param>
        /// <param name="namespaces">The set of unique namespaces.</param>
        /// <param name="requirePrefixAndNamespace">If true, each child must have a prefix and namespace defined. An
        /// exception will be thrown if a child does not meet this criteria. If false, the prefix and namespace of
        /// children are not checked.</param>
        private void GetXmlElementNamespaces(
                                             IEnumerable<ElementInfo> children,
                                             Dictionary<string, string> namespaces,
                                             bool requirePrefixAndNamespace)
        {
            if (children != null)
            {
                foreach (ElementInfo info in children)
                {
                    // Root elements in the extensions must be in custom namespaces with custom prefixes.
                    // PlainText elements are output as text so won't have names.
                    if (requirePrefixAndNamespace)
                    {
                        this.ValidatePrefixAndNamespace(info.Prefix, info.Namespace, info.LocalName, false);
                    }
                    else if (!(info.Element is CDataTag) &&
                             !(info.Element is CommentTag) &&
                             !(info.Element is PlainText) &&
                             !(info.Element is ProcessingInstructionTag))
                    {
                        this.ValidatePrefixAndNamespace(null, null, info.LocalName, true);
                    }

                    this.AddNamespaceOrThrowIfMismatch(namespaces, info.Prefix, info.Namespace);
                    this.GetXmlElementNamespaces(info.Element as IExtensible, namespaces);
                    this.GetXmlElementNamespaces(((IXliffDataProvider)info.Element).GetXliffChildren(), namespaces, false);
                }
            }
        }

        /// <summary>
        /// Enumerates all extensions and their descendants storing the set of unique Xml namespaces used for serializing
        /// the elements.
        /// </summary>
        /// <param name="extensible">The extensible object that may contain extensions to enumerate over. This value
        /// may be null.</param>
        /// <param name="namespaces">The set of unique namespaces.</param>
        private void GetXmlElementNamespaces(IExtensible extensible, Dictionary<string, string> namespaces)
        {
            if ((extensible != null) && extensible.HasExtensions)
            {
                foreach (IExtension extension in extensible.Extensions)
                {
                    if (extension.HasAttributes)
                    {
                        foreach (IExtensionAttribute attribute in extension.GetAttributes())
                        {
                            // Custom attributes must have a prefix and namespace.
                            this.ValidatePrefixAndNamespace(
                                                            attribute.Prefix,
                                                            attribute.Namespace,
                                                            attribute.LocalName,
                                                            false);

                            this.AddNamespaceOrThrowIfMismatch(namespaces, attribute.Prefix, attribute.Namespace);
                        }
                    }

                    if (extension.HasChildren)
                    {
                        this.GetXmlElementNamespaces(extension.GetChildren(), namespaces, true);
                    }
                }
            }
        }

        /// <summary>
        /// Serializes an <see cref="XliffDocument"/> to a stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="document">The document to write.</param>
        public void Serialize(Stream stream, XliffDocument document)
        {
            XmlWriterSettings settings;

            ArgValidator.Create(stream, "stream").IsNotNull();
            ArgValidator.Create(document, "document").IsNotNull();

            settings = new XmlWriterSettings();
            settings.Indent = this.settings.Indent;
            if (settings.Indent && (this.settings.IndentChars != null))
            {
                settings.IndentChars = this.settings.IndentChars;
            }

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                this.Serialize(writer, document);
            }
        }

        /// <summary>
        /// Serializes an <see cref="XliffDocument"/> to an Xml stream.
        /// </summary>
        /// <param name="writer">The Xml stream to write to.</param>
        /// <param name="document">The document to write.</param>
        private void Serialize(XmlWriter writer, XliffDocument document)
        {
            // Validate the document before serializing.
            foreach (IXliffValidator validator in this.settings.Validators)
            {
                validator.Validate(document);
            }

            this.document = document;
            this.writer = writer;

            try
            {
                this.SerializeImpl();
            }
            finally
            {
                this.document = null;
                this.writer = null;
            }
        }

        /// <summary>
        /// Serializes attributes to the current Xml element.
        /// </summary>
        /// <param name="provider">The data provider that contains attributes to serialize.</param>
        private void SerializeAttributesImpl(IXliffDataProvider provider)
        {
            if (provider != null)
            {
                IEnumerable<IAttributeDataProvider> attributes;

                attributes = provider.GetXliffAttributes();
                if (attributes != null)
                {
                    foreach (IAttributeDataProvider attribute in attributes)
                    {
                        bool shouldWrite;

                        shouldWrite = this.ShouldWriteAttributeString(attribute);
                        if (!shouldWrite)
                        {
                            foreach (IAttributeDataProvider dependency in attribute.ExplicitOutputDependencies)
                            {
                                shouldWrite = this.ShouldWriteAttributeString(dependency);
                                if (shouldWrite)
                                {
                                    break;
                                }
                            }
                        }

                        if (shouldWrite)
                        {
                            this.WriteAttributeString(attribute);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Serializes the children of an element.
        /// </summary>
        /// <param name="children">The children to serialize.</param>
        private void SerializeChildImpl(IEnumerable<ElementInfo> children)
        {
            foreach (ElementInfo pair in children)
            {
                if (pair.Element is CDataTag)
                {
                    this.writer.WriteCData(((CDataTag)pair.Element).Text);
                }
                else if (pair.Element is CommentTag)
                {
                    this.writer.WriteComment(((CommentTag)pair.Element).Text);
                }
                else if (pair.Element is ProcessingInstructionTag)
                {
                    ProcessingInstructionTag instruction;

                    instruction = (ProcessingInstructionTag)pair.Element;
                    this.writer.WriteProcessingInstruction(instruction.Name, instruction.Text);
                }
                else
                {
                    this.SerializeImpl(pair.LocalName, pair.Namespace, pair.Prefix, pair.Element, null);
                }
            }
        }

        /// <summary>
        /// Serializes an element into a stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="element">The element to serialize.</param>
        /// <param name="serializeInnerContent">True to serialize inner content. If false only the opening and closing
        /// tags will be serialized along with attributes.</param>
        public static void SerializeElement(Stream stream, ResourceStringContent element, bool serializeInnerContent)
        {
            IXliffDataProvider provider;
            IEnumerable<ElementInfo> children;
            XliffWriter writer;
            XmlWriterSettings settings;

            Debug.Assert(element != null, "The element should not be null.");
            Debug.Assert(element.Parent != null, "The element parent should not be null.");
            Debug.Assert(stream != null, "The stream should not be null.");

            settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            writer = new XliffWriter();
            writer.writer = XmlWriter.Create(stream, settings);
            writer.writer.WriteStartDocument();

            provider = element.Parent;
            children = provider.GetXliffChildren();
            foreach (ElementInfo pair in children)
            {
                if (pair.Element == element)
                {
                    // Don't use the namespace or prefix otherwise they'll be output for fragments. The namespaces
                    // should be written to the root xliff node so they'll be ok for the final document.
                    writer.SerializeImpl(
                                         pair.LocalName,
                                         null,
                                         null,
                                         pair.Element,
                                         null,
                                         serializeInnerContent);
                    break;
                }
            }

            writer.writer.WriteEndDocument();
            writer.writer.Flush();
        }

        /// <summary>
        /// Serializes the extension information of attributes of the specified extension data.
        /// </summary>
        /// <param name="data">The extension data that may contain attributes to serialize.</param>
        private void SerializeExtensionAttributes(IExtensionData data)
        {
            if ((data != null) && data.HasAttributes)
            {
                foreach (IExtensionAttribute attribute in data.GetAttributes())
                {
                    this.writer.WriteAttributeString(attribute.LocalName, attribute.Namespace, attribute.Value);
                }
            }
        }

        /// <summary>
        /// Serializes the extension information of children of the specified extension data.
        /// </summary>
        /// <param name="data">The extension data that may contain children to serialize.</param>
        private void SerializeExtensionChildren(IExtensionData data)
        {
            if ((data != null) && data.HasChildren)
            {
                foreach (ElementInfo child in data.GetChildren())
                {
                    this.SerializeImpl(child.LocalName, child.Namespace, null, child.Element, null);
                }
            }
        }

        /// <summary>
        /// Serializes extension information on an extensible object.
        /// </summary>
        /// <param name="extensible">The extensible object that may contain extensions.</param>
        /// <param name="serializeAttributes">If true, attributes extensions will be serialized, otherwise they'll be skipped.</param>
        /// <param name="serializeChildren">If true, element extensions will be serialized, otherwise they'll be skipped.</param>
        private void SerializeExtensions(IExtensible extensible, bool serializeAttributes, bool serializeChildren)
        {
            if (this.settings.IncludeExtensions && (extensible != null))
            {
                foreach (IExtension extension in extensible.Extensions)
                {
                    if (serializeAttributes && extension.HasAttributes)
                    {
                        if (extensible.SupportsAttributeExtensions)
                        {
                            this.SerializeExtensionAttributes(extension);
                        }
                        else
                        {
                            string message;

                            message = string.Format(
                                            Properties.Resources.XliffWriter_ExtensionTypeNotSupported_Format,
                                            extensible.GetType().Name,
                                            "Attribute");
                            throw new NotSupportedException(message);
                        }
                    }

                    if (serializeChildren && extension.HasChildren)
                    {
                        if (extensible.SupportsElementExtensions)
                        {
                            this.SerializeExtensionChildren(extension);
                        }
                        else
                        {
                            string message;

                            message = string.Format(
                                            Properties.Resources.XliffWriter_ExtensionTypeNotSupported_Format,
                                            extensible.GetType().Name,
                                            "Element");
                            throw new NotSupportedException(message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Serializes the stored document to the stored stream.
        /// </summary>
        private void SerializeImpl()
        {
            Dictionary<string, string> extraNamespace;

            extraNamespace = this.GetNamespaceAttributes();
            this.writer.WriteStartDocument();
            this.SerializeImpl(
                               ElementNames.Root,
                               NamespaceValues.Core,
                               null,
                               this.document,
                               extraNamespace);
            this.writer.WriteEndDocument();
            this.writer.Flush();
        }

        /// <summary>
        /// Serializes an <see cref="XliffElement"/> to the stream using the specified Xml element Name.
        /// </summary>
        /// <param name="name">The name of the XmlElement to create.</param>
        /// <param name="element">The content to serialize.</param>
        private void SerializeImpl(string name, XliffElement element)
        {
            this.SerializeImpl(name, null, null, element, null);
        }

        /// <summary>
        /// Serializes an <see cref="XliffElement"/> to the stream using the specified Xml element Name and namespace.
        /// The method creates an element and writes attributes if necessary, then writes out text followed by notes
        /// if the element supports notes. It then uses recursion to do the same for children.
        /// </summary>
        /// <param name="name">The name of the XmlElement to create.</param>
        /// <param name="namespaceName">The namespace of the XmlElement to create.</param>
        /// <param name="prefix">The prefix Name of the XmlElement to create.</param>
        /// <param name="provider">The content to serialize.</param>
        /// <param name="extraNamespace">A set of namespaces to configure on the element. The key is the name
        /// of the namespace and the value is the URI of the namespace.</param>
        private void SerializeImpl(
                                   string name,
                                   string namespaceName,
                                   string prefix,
                                   IXliffDataProvider provider,
                                   Dictionary<string, string> extraNamespace)
        {
            this.SerializeImpl(
                               name,
                               namespaceName,
                               prefix,
                               provider,
                               extraNamespace,
                               true);
        }

        /// <summary>
        /// Serializes an <see cref="XliffElement"/> to the stream using the specified Xml element Name and namespace.
        /// The method creates an element and writes attributes if necessary, then writes out text followed by notes
        /// if the element supports notes. It then uses recursion to do the same for children.
        /// </summary>
        /// <param name="name">The name of the XmlElement to create.</param>
        /// <param name="namespaceName">The namespace of the XmlElement to create.</param>
        /// <param name="prefix">The prefix Name of the XmlElement to create.</param>
        /// <param name="provider">The content to serialize.</param>
        /// <param name="extraNamespace">A set of namespaces to configure on the element. The key is the name
        /// of the namespace and the value is the URI of the namespace.</param>
        /// <param name="serializeInnerContent">True to serialize inner content. If false only the opening and closing
        /// tags will be serialized along with attributes.</param>
        private void SerializeImpl(
                                   string name,
                                   string namespaceName,
                                   string prefix,
                                   IXliffDataProvider provider,
                                   Dictionary<string, string> extraNamespace,
                                   bool serializeInnerContent)
        {
            // The name may be null for elements that are just inline elements for others. For example, the
            // PlainText class represents text within a Source node, but doesn't actually require an Xml element
            // so it won't have a Name.
            if (name != null)
            {
                this.writer.WriteStartElement(prefix, name, namespaceName);

                if (extraNamespace != null)
                {
                    foreach (string key in extraNamespace.Keys)
                    {
                        this.writer.WriteAttributeString(
                                                          NamespacePrefixes.XmlNamespace,
                                                          key,
                                                          string.Empty,
                                                          extraNamespace[key]);
                    }
                }

                this.SerializeAttributesImpl(provider);
                this.SerializeExtensions(provider as IExtensible, true, false);
            }

            if (serializeInnerContent)
            {
                Comparison<OutputItem> comparer;
                List<OutputItem> order;
                Dictionary<Type, List<ElementInfo>> children;
                List<ElementInfo> unorderedChildren;
                bool serializedChildAsObject;
                bool serializedText;
                bool serializedExtensions;

                serializedChildAsObject = false;
                serializedText = false;
                serializedExtensions = false;

                children = XliffWriter.GetProviderChildrenGroupedByType(provider);

                comparer = delegate(OutputItem item1, OutputItem item2)
                {
                    int result;

                    if (item1.GroupOrdinal == item2.GroupOrdinal)
                    {
                        result = 0;
                    }
                    else if (item1.GroupOrdinal < item2.GroupOrdinal)
                    {
                        result = -1;
                    }
                    else
                    {
                        result = 1;
                    }

                    return result;
                };

                order = new List<OutputItem>(provider.XliffOutputOrder);
                order.Sort(comparer);

                foreach (OutputItem item in order)
                {
                    switch (item.ItemType)
                    {
                        case OutputItemType.Child:
                            this.SerializeChildImpl(children[item.ChildType]);

                            // Object was specified by the provider so don't treat it as the unordered list.
                            serializedChildAsObject |= item.ChildType == typeof(object);
                            break;

                        case OutputItemType.Extension:
                            this.SerializeExtensions(provider as IExtensible, false, true);
                            serializedExtensions = true;
                            break;

                        case OutputItemType.Text:
                            this.SerializeTextImpl(provider);
                            serializedText = true;
                            break;

                        default:
                            Debug.Assert(false, item.ItemType.ToString() + " is not handled.");
                            break;
                    }
                }

                // Serialize everything that hasn't been serialized yet. Order doesn't matter at this point.
                if (!serializedText)
                {
                    this.SerializeTextImpl(provider);
                }

                if (!serializedChildAsObject && children.TryGetValue(typeof(object), out unorderedChildren))
                {
                    this.SerializeChildImpl(unorderedChildren);
                }

                if (!serializedExtensions)
                {
                    this.SerializeExtensions(provider as IExtensible, false, true);
                }
            }

            if (name != null)
            {
                if (provider.HasXliffChildren || provider.HasXliffText)
                {
                    this.writer.WriteFullEndElement();
                }
                else
                {
                    this.writer.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// Serializes the text of a provider.
        /// </summary>
        /// <param name="provider">The provider that contains the text to serialize.</param>
        private void SerializeTextImpl(IXliffDataProvider provider)
        {
            string text;

            text = provider.GetXliffText();
            if (text != null)
            {
                this.writer.WriteString(text);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified attribute should be written in the output.
        /// </summary>
        /// <param name="value">The attribute to check.</param>
        /// <returns>True if the attribute should be written based on the current settings, false indicates the
        /// attribute does not need to be written.</returns>
        private bool ShouldWriteAttributeString(IAttributeDataProvider value)
        {
            bool result;

            result = false;
            if (!value.IsOptional)
            {
                // Value is required so it must be written.
                result = true;
            }
            else if ((this.settings.Detail == OutputDetail.Full) && !object.Equals(value.GetStringValue(), null))
            {
                // Full detail so write everything that isn't null (has no value).
                result = true;
            }
            else if ((this.settings.Detail == OutputDetail.Explicit) && value.HasValue)
            {
                // Explicit detail so only write things that have values explicitly set.
                result = true;
            }
            else if ((this.settings.Detail == OutputDetail.Minimal) && value.HasValue)
            {
                if (!value.IsDefaultValue)
                {
                    // The value is different than the default.
                    result = true;
                }
                else
                {
                    string inheritedValue;

                    if (value.TryGetInheritedStringValue(out inheritedValue) &&
                        (value.GetStringValue() != inheritedValue))
                    {
                        // The value is the same as the default value but is different than an ancestor's value so it
                        // needs to be written otherwise the inherited value will override the default.
                        return true;
                    }

                    // else there is no inherited value so the default can be used, or the value is the same as the
                    // default value and an ancestor's value so the value can just be taken as the default or whatever
                    // is written by the ancestor.
                }
            }

            return result;
        }

        /// <summary>
        /// Validates that the Xml prefix, namespace, and local name are not null and are valid Xml values. If any
        /// value is invalid an exception is thrown.
        /// </summary>
        /// <param name="prefix">The prefix to validate.</param>
        /// <param name="ns">The namespace to validate.</param>
        /// <param name="localName">The local name of the element to validate.</param>
        /// <param name="checkLocalOnly">If true, only the <paramref name="localName"/> is validated. If false, the
        /// <paramref name="prefix"/>, <paramref name="ns"/>, and <paramref name="localName"/> are validated.</param>
        /// <exception cref="InvalidXmlSpecifierException">This exception is thrown if the prefix, namespace, or local
        /// name are invalid according to Xml standards.</exception>
        private void ValidatePrefixAndNamespace(string prefix, string ns, string localName, bool checkLocalOnly)
        {
            try
            {
                if (!checkLocalOnly)
                {
                    XmlConvert.VerifyNCName(prefix);
                    XmlConvert.VerifyName(ns);
                }

                XmlConvert.VerifyName(localName);
            }
            catch (Exception e)
            {
                string message;

                message = string.Format(
                                        Properties.Resources.XmlWriter_InvalidXmlSpecifier_Format,
                                        localName);
                throw new InvalidXmlSpecifierException(message, e);
            }
        }

        /// <summary>
        /// Writes out an attribute to the current XmlElement.
        /// </summary>
        /// <param name="value">The value of the attribute.</param>
        /// <remarks>The value is written depending on the OutputDetail of the Settings and whether the
        /// attribute value is set.</remarks>
        private void WriteAttributeString(IAttributeDataProvider value)
        {
            this.writer.WriteAttributeString(value.Prefix, value.LocalName, value.Namespace, value.GetStringValue());
        }
        #endregion Methods
    }
}
