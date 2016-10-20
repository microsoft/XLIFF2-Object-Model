namespace Localization.Xliff.OM.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Exceptions;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Validators;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class reads XLIFF 2.0 documents.
    /// </summary>
    public class XliffReader
    {
        /// <summary>
        /// The key in handlers that identifies the default handler that stores any unclaimed attributes and elements
        /// in extensible objects.
        /// </summary>
        #region Member Variables
        private const string DefaultHandlerKey = "*";

        /// <summary>
        /// The list of handlers to call when reading elements and attributes that aren't native XLIFF.
        /// </summary>
        private readonly Lazy<Dictionary<string, IExtensionHandler>> handlers;

        /// <summary>
        /// The settings describing how to read content.
        /// </summary>
        private readonly XliffReaderSettings settings;

        /// <summary>
        /// The history of elements as they're being read.
        /// </summary>
        private readonly Stack<ElementState> elementStack;

        /// <summary>
        /// The current element being read.
        /// </summary>
        private ElementState currentElementState;

        /// <summary>
        /// The stream to read from.
        /// </summary>
        private XmlReader reader;
        #endregion Member Variables

        /// <summary>
        /// Initializes a new instance of the <see cref="XliffReader"/> class using default settings.
        /// </summary>
        public XliffReader()
            : this(new XliffReaderSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XliffReader"/> class.
        /// </summary>
        /// <param name="settings">Settings that determine how to read content.</param>
        public XliffReader(XliffReaderSettings settings)
        {
            GenericExtensionHandler defaultHandler;

            ArgValidator.Create(settings, "settings").IsNotNull();

            this.elementStack = new Stack<ElementState>();
            this.handlers = new Lazy<Dictionary<string, IExtensionHandler>>();
            this.settings = settings;

            defaultHandler = new GenericExtensionHandler();
            this.RegisterExtensionHandler(XliffReader.DefaultHandlerKey, defaultHandler);
        }

        #region Methods
        /// <summary>
        /// Deserializes XLIFF content from a stream.
        /// </summary>
        /// <param name="stream">The stream to deserialize.</param>
        /// <returns>The <see cref="XliffDocument"/> that defines the root of the document.</returns>
        public XliffDocument Deserialize(Stream stream)
        {
            ArgValidator.Create(stream, "stream").IsNotNull();

            using (XmlReader reader = XmlReader.Create(stream))
            {
                return this.Deserialize(reader);
            }
        }

        /// <summary>
        /// Deserializes XLIFF content from an Xml stream.
        /// </summary>
        /// <param name="reader">The Xml stream to deserialize.</param>
        /// <returns>The <see cref="XliffDocument"/> that defines the root of the document.</returns>
        private XliffDocument Deserialize(XmlReader reader)
        {
            XliffDocument result;

            this.currentElementState = null;
            this.elementStack.Clear();
            this.reader = reader;

            result = this.DeserializeImpl();

            // Validate the document after serializing.
            foreach (IXliffValidator validator in this.settings.Validators)
            {
                validator.Validate(result);
            }

            return result;
        }

        /// <summary>
        /// Deserializes the attributes at the reader's current position and stores them in the
        /// <paramref name="currentElement"/>.
        /// </summary>
        /// <param name="currentElement">The element associated with the reader's current position.</param>
        private void DeserializeAttributes(IXliffDataConsumer currentElement)
        {
            if (this.reader.MoveToFirstAttribute())
            {
                IExtensible extensible;

                extensible = currentElement as IExtensible;
                do
                {
                    SetAttributeResult setResult;
                    XmlNameInfo name;

                    name = new XmlNameInfo(this.reader.Prefix, this.reader.NamespaceURI, this.reader.LocalName);
                    setResult = currentElement.TrySetAttributeValue(name, this.reader.Value);
                    if ((setResult == SetAttributeResult.InvalidAttribute) ||
                        ((setResult == SetAttributeResult.PossibleExtension) &&
                            (this.StoreAttributeExtension(extensible) == SetAttributeResult.InvalidAttribute)))
                    {
                        string message;

                        if (string.IsNullOrEmpty(name.Prefix))
                        {
                            message = name.LocalName;
                        }
                        else
                        {
                            message = string.Join(":", name.Prefix, name.LocalName);
                        }

                        message = string.Format(
                                                Properties.Resources.XliffReader_InvalidAttributeName_Format,
                                                message,
                                                currentElement.GetType().Name);
                        throw new NotSupportedException(message);
                    }
                }
                while (this.reader.MoveToNextAttribute());
            }
        }

        /// <summary>
        /// Deserializes the element at the reader's current position as a new <see cref="XliffElement"/>.
        /// </summary>
        private void DeserializeElement()
        {
            IXliffDataConsumer newElement;
            XmlNameInfo name;
            bool handlerCreated;
            bool isEmpty;

            handlerCreated = false;

            // Create a new element based on the XLIFF Name. Some elements don't necessarily have XliffElements
            // (ex. Notes) so false is returned.
            name = new XmlNameInfo(this.reader.Prefix, this.reader.NamespaceURI, this.reader.LocalName);
            newElement = this.currentElementState.Consumer.CreateXliffElement(name);
            if (newElement == null)
            {
                // Don't store native XLIFF elements as extensions.
                if (this.settings.IncludeExtensions &&
                    !Utilities.IsCoreNamespace(this.reader.NamespaceURI) &&
                    !Utilities.IsModuleNamespace(this.reader.NamespaceURI))
                {
                    IExtensible extensible;

                    extensible = this.currentElementState.Consumer as IExtensible;
                    if ((extensible != null) && extensible.SupportsElementExtensions && this.handlers.IsValueCreated)
                    {
                        IExtensionHandler handler;

                        if (this.handlers.Value.TryGetValue(this.reader.NamespaceURI, out handler) ||
                            this.handlers.Value.TryGetValue(XliffReader.DefaultHandlerKey, out handler))
                        {
                            ExtensionNameInfo extensionName;
                            XliffElement createdElement;

                            extensionName = new ExtensionNameInfo(name.Prefix, name.Namespace, name.LocalName);
                            createdElement = handler.CreateElement(extensionName);
                            handler.StoreElement(extensible, new ElementInfo(name, createdElement));

                            newElement = createdElement;
                            handlerCreated = true;
                        }
                        else
                        {
                            Debug.Assert(false, "Default handler was not found.");
                        }
                    }
                }

                if (newElement == null)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.XliffElement_UnknownElement_Format,
                                            this.reader.Name,
                                            this.currentElementState.Consumer.GetType().Name);
                    throw new FormatException(message);
                }
                else
                {
                    int ordinal;

                    ordinal = this.currentElementState.GetOrdinal(OutputItemType.Extension, typeof(IExtension));
                    if (ordinal < this.currentElementState.LastOrdinalRead)
                    {
                        string message;

                        message = string.Format(
                                                Properties.Resources.XliffElement_ElementOutOfOrder_Format,
                                                this.currentElementState.Consumer.GetType().Name,
                                                newElement.GetType().Name,
                                                name.Namespace,
                                                name.LocalName);
                        throw new FormatException(message);
                    }

                    this.currentElementState.LastOrdinalRead = ordinal;
                }
            }
            else
            {
                int ordinal;

                ordinal = this.currentElementState.GetOrdinal(OutputItemType.Child, newElement.GetType());
                if (ordinal < this.currentElementState.LastOrdinalRead)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.XliffElement_ElementOutOfOrder_Format,
                                            this.currentElementState.Consumer.GetType().Name,
                                            newElement.GetType().Name,
                                            name.Namespace,
                                            name.LocalName);
                    throw new FormatException(message);
                }

                this.currentElementState.LastOrdinalRead = ordinal;
            }

            // Check if empty element before reading attributes because reading attributes moves the reader.
            isEmpty = this.reader.IsEmptyElement;

            Debug.Assert(newElement != null, "newElement is null.");
            this.DeserializeAttributes(newElement);

            // Extensions are not stored directly as children.
            if (!handlerCreated)
            {
                this.currentElementState.Consumer.AddXliffChild(name, newElement);
            }

            if (!isEmpty)
            {
                this.elementStack.Push(this.currentElementState);
                this.currentElementState = new ElementState(newElement);
            }
            else
            {
                this.ValidateElementState(new ElementState(newElement));
            }
        }

        /// <summary>
        /// Implementation for de-serializing XLIFF content from an Xml stream.
        /// </summary>
        /// <returns>The <see cref="XliffDocument"/> that defines the root of the document.</returns>
        private XliffDocument DeserializeImpl()
        {
            XliffProvider provider;

            provider = new XliffProvider();
            this.DeserializeXmlContent(provider);

            return provider.Document;
        }

        /// <summary>
        /// Deserializes text into XLIFF elements under the specified source element.
        /// </summary>
        /// <param name="source">The element to deserialize content to.</param>
        /// <param name="text">The text to deserialize.</param>
        public static void DeserializeText(XliffElement source, string text)
        {
            XliffReader reader;

            reader = new XliffReader();
            reader.currentElementState = new ElementState(source);

            using (MemoryStream stream = new MemoryStream())
            {
                // Don't wrap this in using because it may dispose of the stream which is one too many.
                TextWriter writer;

                writer = new StreamWriter(stream);
                writer.Write(text);

                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                reader.reader = XmlReader.Create(stream);
                reader.DeserializeXmlContent(source);
            }
        }

        /// <summary>
        /// Deserializes the Xml content in the reader to the specified element.
        /// </summary>
        /// <param name="root">The element to insert deserialized XLIFF elements to.</param>
        private void DeserializeXmlContent(XliffElement root)
        {
            this.currentElementState = new ElementState(root);

            this.reader.MoveToContent();

            do
            {
                switch (this.reader.NodeType)
                {
                    case XmlNodeType.CDATA:
                        if (this.currentElementState.Consumer is IResourceStringContentContainer)
                        {
                            this.currentElementState.Consumer.AddXliffChild(
                                                              new XmlNameInfo((string)null),
                                                              new CDataTag(this.reader.Value));
                        }

                        break;

                    case XmlNodeType.Comment:
                        if (this.currentElementState.Consumer is IResourceStringContentContainer)
                        {
                            this.currentElementState.Consumer.AddXliffChild(
                                                              new XmlNameInfo((string)null),
                                                              new CommentTag(this.reader.Value));
                        }

                        break;

                    case XmlNodeType.Element:
                        {
                            ElementState currentState;

                            currentState = this.currentElementState;
                            this.DeserializeElement();

                            if (object.ReferenceEquals(this.currentElementState, currentState))
                            {
                                // The element won't have any children so validate it.
                                this.ValidateElementState(this.currentElementState);
                            }
                        }

                        break;

                    case XmlNodeType.EndElement:
                        this.ValidateElementState(this.currentElementState);
                        this.currentElementState = this.elementStack.Pop();
                        break;

                    case XmlNodeType.Text:
                        {
                            int ordinal;

                            ordinal = this.currentElementState.GetOrdinal(OutputItemType.Text, typeof(string));
                            if (ordinal < this.currentElementState.LastOrdinalRead)
                            {
                                string message;

                                message = string.Format(
                                                        Properties.Resources.XliffElement_ElementOutOfOrder_Format,
                                                        this.currentElementState.Consumer.GetType().Name,
                                                        typeof(string).Name,
                                                        this.reader.NamespaceURI,
                                                        this.reader.LocalName);
                                throw new FormatException(message);
                            }

                            this.currentElementState.Consumer.SetXliffText(this.reader.Value);
                        }

                        break;

                    case XmlNodeType.ProcessingInstruction:
                        if (this.currentElementState.Consumer is IResourceStringContentContainer)
                        {
                            this.currentElementState.Consumer.AddXliffChild(
                                                              new XmlNameInfo((string)null),
                                                              new ProcessingInstructionTag(this.reader.Name, this.reader.Value));
                        }

                        break;

                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        // SignificantWhitespace is used when xml:space=preserve is added to an XmlElement and the
                        // whitespace wasn't added by the Xml writer.
                        if (this.currentElementState.Consumer is IResourceStringContentContainer)
                        {
                            IResourceStringContentContainer container;

                            container = (IResourceStringContentContainer)this.currentElementState.Consumer;
                            container.Text.Add(new PlainText(this.reader.Value));
                        }

                        break;
                }
            }
            while (this.reader.Read());
        }

        /// <summary>
        /// Registers a handler to call when reading non-native XLIFF elements and attributes to store extension data.
        /// </summary>
        /// <param name="ns">The namespace of items handled by the handler.</param>
        /// <param name="handler">The handler to add.</param>
        public void RegisterExtensionHandler(string ns, IExtensionHandler handler)
        {
            ArgValidator.Create(ns, "ns").IsNotNullOrWhitespace();
            ArgValidator.Create(handler, "handler").IsNotNull();

            if (this.handlers.Value.ContainsKey(ns))
            {
                string message;

                message = string.Format(Properties.Resources.XliffReader_ExtensionAlreadyRegistered_Format, ns);
                throw new ExtensionAlreadyRegisteredException(message);
            }

            this.handlers.Value.Add(ns, handler);
        }

        /// <summary>
        /// Stores an attribute in an extension. If no appropriate extension handler was found, a general purpose
        /// one will be created.
        /// </summary>
        /// <param name="extensible">The object that contains the extensions.</param>
        /// <returns>A result indicating whether the attribute was stored.</returns>
        private SetAttributeResult StoreAttributeExtension(IExtensible extensible)
        {
            SetAttributeResult result;

            result = SetAttributeResult.InvalidAttribute;
            if (this.settings.IncludeExtensions)
            {
                if (this.reader.Prefix == NamespacePrefixes.XmlNamespace)
                {
                    result = SetAttributeResult.ReservedName;
                }
                else if ((extensible != null) && extensible.SupportsAttributeExtensions)
                {
                    ExtensionNameInfo info;
                    IExtensionHandler handler;

                    info = new ExtensionNameInfo(this.reader.Prefix, this.reader.NamespaceURI, this.reader.LocalName);
                    if (this.handlers.Value.TryGetValue(this.reader.NamespaceURI, out handler) ||
                        this.handlers.Value.TryGetValue(XliffReader.DefaultHandlerKey, out handler))
                    {
                        IExtensionAttribute attribute;

                        attribute = handler.CreateAttribute(info, this.reader.Value);
                        if (handler.StoreAttribute(extensible, attribute))
                        {
                            result = SetAttributeResult.Success;
                        }
                    }
                    else
                    {
                        Debug.Assert(false, "Default handler was not found.");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Removes a handler to call when reading non-native XLIFF elements and attributes to store extension data.
        /// </summary>
        /// <param name="ns">The namespace of the handler to remove.</param>
        public void UnregisterExtensionHandler(string ns)
        {
            if (this.handlers.IsValueCreated)
            {
                this.handlers.Value.Remove(ns);
            }
        }

        /// <summary>
        /// Validates the specified element state for correctness.
        /// </summary>
        /// <param name="state">The state to validate.</param>
        /// <remarks>This method throws a <see cref="FormatException"/> if the state is malformed.</remarks>
        private void ValidateElementState(ElementState state)
        {
            if (state.Consumer is NoteContainer)
            {
                NoteContainer container;

                // Notes must contain at least one Note.
                container = (NoteContainer)state.Consumer;
                if (!container.HasNotes)
                {
                    string message;
                    XliffElement element;

                    element = container as XliffElement;
                    message = string.Format(
                                            Properties.Resources.XliffReader_INoteContainerMissingNote_Format,
                                            (element == null) ? string.Empty : element.SelectableAncestor.SelectorPath);
                    throw new FormatException(message);
                }
            }
            else if (state.Consumer is OriginalData)
            {
                OriginalData container;

                // OriginalData must contain at least one Data.
                container = (OriginalData)state.Consumer;
                if (!container.HasData)
                {
                    string message;
                    XliffElement element;

                    element = container as XliffElement;
                    message = string.Format(
                                            Properties.Resources.XliffReader_OriginalDataMissingData,
                                            (element == null) ? string.Empty : element.SelectableAncestor.SelectorPath);
                    throw new FormatException(message);
                }
            }

            // Verify all properties that are required by others are specified.
            foreach (IAttributeDataProvider attribute in state.Consumer.GetXliffAttributes())
            {
                if (!attribute.HasValue)
                {
                    foreach (IAttributeDataProvider dependant in attribute.ExplicitOutputDependencies)
                    {
                        if (dependant.HasValue)
                        {
                            string message;

                            message = string.Format(
                                            Properties.Resources.StandardValidator_ExplicitDependencyMissing_Format,
                                            dependant.LocalName,
                                            attribute.LocalName);
                            throw new FormatException(message);
                        }
                    }

                    if (attribute.OutputResolver != null)
                    {
                        if (attribute.OutputResolver.IsOutputRequired(attribute.LocalName))
                        {
                            string message;

                            message = string.Format(
                                            Properties.Resources.StandardValidator_OutputResolverFailed_Format,
                                            attribute.LocalName);
                            throw new FormatException(message);
                        }
                    }
                }
            }
        }
        #endregion Methods

        /// <summary>
        /// This class contains state information for the element currently being parsed.
        /// </summary>
        internal class ElementState
        {
            /// <summary>
            /// A dictionary whose string is a key identifying an element type and the value is the ordered index of
            /// the element with respect to other siblings within the parent element.
            /// </summary>
            private readonly Dictionary<string, int> inputOrderMap;

            /// <summary>
            /// Initializes a new instance of the <see cref="ElementState"/> class.
            /// </summary>
            /// <param name="consumer">The consumer that is consuming the file contents.</param>
            internal ElementState(IXliffDataConsumer consumer)
            {
                this.Consumer = consumer;
                this.LastOrdinalRead = -1;

                this.inputOrderMap = this.BuildInputOrderDictionary();
            }

            /// <summary>
            /// Gets the consumer that is consuming the file contents.
            /// </summary>
            internal IXliffDataConsumer Consumer { get; private set; }

            /// <summary>
            /// Gets or sets the ordinal of the last element read.
            /// </summary>
            internal int LastOrdinalRead { get; set; }

            /// <summary>
            /// Gets the ordinal corresponding to an element or text being read from the XmlReader.
            /// </summary>
            /// <param name="itemType">The output type of the element to look up.</param>
            /// <param name="type">The type of the element to look up.</param>
            /// <returns>The ordinal index that indicates where the element resides relative to its siblings. If an
            /// index isn't found, then the ordinal is assumed to be after all other specified siblings.</returns>
            internal int GetOrdinal(OutputItemType itemType, Type type)
            {
                int temp;

                if (!this.inputOrderMap.TryGetValue(itemType + ":" + type.ToString(), out temp))
                {
                    // Assume any order is valid for the remaining elements.
                    temp = this.inputOrderMap.Count;
                }

                return temp;
            }

            /// <summary>
            /// Builds the dictionary mapping an element type to its ordinal index relative to other siblings within
            /// the parent.
            /// </summary>
            /// <returns>The dictionary of ordinal indexes.</returns>
            private Dictionary<string, int> BuildInputOrderDictionary()
            {
                Dictionary<string, int> result;
                IEnumerable<OutputItem> inputOrder;
                int index;

                result = new Dictionary<string, int>();

                index = 0;
                inputOrder = this.Consumer.XliffInputOrder;
                foreach (OutputItem item in inputOrder)
                {
                    string key;

                    switch (item.ItemType)
                    {
                        case OutputItemType.Child:
                            key = ElementState.MakeKey(item.ItemType, item.ChildType);
                            break;

                        case OutputItemType.Extension:
                            key = ElementState.MakeKey(item.ItemType, typeof(IExtension));
                            break;

                        case OutputItemType.Text:
                            key = ElementState.MakeKey(item.ItemType, typeof(string));
                            break;

                        default:
                            Debug.Assert(false, string.Format("OutputItemType.{0} is not supported.", item.ItemType));
                            key = null;
                            break;
                    }

                    result.Add(key, item.GroupOrdinal);
                    index++;
                }

                return result;
            }

            /// <summary>
            /// Makes a key used to work with the ordinal dictionary.
            /// </summary>
            /// <param name="itemType">The output type of the element.</param>
            /// <param name="type">The type of the element.</param>
            /// <returns>A value to use as a key in the ordinal dictionary.</returns>
            private static string MakeKey(OutputItemType itemType, Type type)
            {
                return string.Join(":", itemType, type);
            }
        }
    }
}
