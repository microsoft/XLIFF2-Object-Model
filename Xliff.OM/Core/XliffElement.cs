namespace Localization.Xliff.OM
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Localization.Xliff.OM.Exceptions;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This is a base class for all XLIFF element classes. It contains all the values for attributes and manages
    /// data used for serialization.
    /// </summary>
    public abstract class XliffElement : IXliffDataProvider, IXliffDataConsumer
    {
        #region Static Variables
        /// <summary>
        /// An array of empty objects used in reflection.
        /// </summary>
        private static readonly object[] EmptyObjects = new object[] { };

        /// <summary>
        /// An array of empty <see cref="OutputItem"/> objects.
        /// </summary>
        private static readonly OutputItem[] EmptyOutputItems = new OutputItem[] { };
        #endregion Static Variables

        #region Member Variables
        /// <summary>
        /// All the attribute values. The key is the CSharp property Name, the value is the attribute value.
        /// </summary>
        private readonly IDictionary<string, AttributeData> attributes;

        /// <summary>
        /// A map from an XLIFF Name to a class type for all the children contained within this class.
        /// </summary>
        private readonly IDictionary<Type, XmlNameInfo> childMap;

        /// <summary>
        /// Indicates whether an attribute was disabled.
        /// </summary>
        private bool hasDisabledAttribute;

        /// <summary>
        /// A value indicating whether children and attributes were registered via RegisterElementInformation.
        /// </summary>
        private bool registered = false;
        #endregion Member Variables

        /// <summary>
        /// Initializes a new instance of the <see cref="XliffElement"/> class.
        /// </summary>
        protected XliffElement()
        {
            this.attributes = new Dictionary<string, AttributeData>();
            this.childMap = new Dictionary<Type, XmlNameInfo>();
        }

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        protected virtual bool HasChildren
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has text.
        /// </summary>
        protected virtual bool HasInnerText
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has children.
        /// </summary>
        bool IXliffDataProvider.HasXliffChildren
        {
            get { return this.HasChildren; }
        }

        /// <summary>
        /// Gets a value indicating whether the provider has text.
        /// </summary>
        bool IXliffDataProvider.HasXliffText
        {
            get { return this.HasInnerText; }
        }

        /// <summary>
        /// Gets the order in which to write children to a file so the output conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be output in order to be compliant. This method is
        /// used during serialization to ensure the elements and text are output in the order specified by that schema.
        /// </summary>
        protected virtual IEnumerable<OutputItem> OutputOrder
        {
            get { return XliffElement.EmptyOutputItems; }
        }

        /// <summary>
        /// Gets the parent element this element is hosted in. This is used to ensure that elements are
        /// not reused and cause a cycle in the data.
        /// </summary>
        public XliffElement Parent { get; internal set; }

        /// <summary>
        /// Gets the first ancestor that implements <see cref="ISelectable"/>, or null if there isn't one.
        /// </summary>
        public ISelectable SelectableAncestor
        {
            get
            {
                ISelectable result;
                XliffElement parent;

                parent = this;

                do
                {
                    parent = parent.Parent;
                    result = parent as ISelectable;
                }
                while ((parent != null) && (result == null));

                return result;
            }
        }

        /// <summary>
        /// Gets the order in which to read children from a file so the input conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be input in order to be compliant. This method is
        /// used during deserialization to ensure the elements and text are input in the order specified by that schema.
        /// </summary>
        IEnumerable<OutputItem> IXliffDataConsumer.XliffInputOrder
        {
            get { return this.OutputOrder; }
        }

        /// <summary>
        /// Gets the order in which to write children to a file so the output conforms to a defined schema. The xliff schema
        /// describes the exact order that elements and text must be output in order to be compliant. This method is
        /// used during serialization to ensure the elements and text are output in the order specified by that schema.
        /// </summary>
        IEnumerable<OutputItem> IXliffDataProvider.XliffOutputOrder
        {
            get { return this.OutputOrder; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Adds a list of <see cref="XliffElement"/>s to another list as a key-value pair. The key is The name
        /// to assign to the elements, and the value is the element. The name of the object comes from the children
        /// types defined on the object.
        /// </summary>
        /// <param name="elements">The elements to add to the list. This should be a list of
        /// <see cref="XliffElement"/> derived objects.</param>
        /// <param name="list">The list to add the elements to. If this value is null, the list will be created.</param>
        internal void AddChildElementsToList(IEnumerable elements, ref List<ElementInfo> list)
        {
            if (elements != null)
            {
                foreach (object element in elements)
                {
                    if (list == null)
                    {
                        list = new List<ElementInfo>();
                    }

                    list.Add(new ElementInfo(this.childMap[element.GetType()], (XliffElement)element));
                }
            }
        }

        /// <summary>
        /// Adds the specified <see cref="XliffElement"/> to this object in the appropriate place depending on
        /// the type and attributes.
        /// </summary>
        /// <param name="name">The name of the child to add.</param>
        /// <param name="child">The <see cref="XliffElement"/> to add.</param>
        void IXliffDataConsumer.AddXliffChild(XmlNameInfo name, IXliffDataConsumer child)
        {
            ArgValidator validator;

            validator = ArgValidator.Create(child, "child").IsNotNull().IsOfType(typeof(XliffElement));

            if (this.registered)
            {
                validator.IsOfType(this.childMap.Keys);
            }

            if (!this.StoreChild(new ElementInfo(name, (XliffElement)child)))
            {
                string message;

                message = string.Format(
                                        Properties.Resources.XliffElement_InvalidElement_Format,
                                        child.GetType().Name,
                                        this.GetType().Name);
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Constructs the selector path for this element.
        /// </summary>
        /// <returns>The full path of the item from the root document.</returns>
        protected string BuildSelectorPath()
        {
            return this.BuildSelectorPath(true);
        }

        /// <summary>
        /// Constructs the selector path for this element.
        /// </summary>
        /// <param name="includeLeafFragment">If true, leaf elements will be included in the selector path. If false,
        /// leaf elements will be left out because another leaf element is being included in the path. Some elements
        /// may be children of other elements in the element hierarchy but in the selector path, they may reside at
        /// the same level as a leaf fragment, and this argument indicates how to handle elements in this case.</param>
        /// <returns>The full path of the item from the root document.</returns>
        private string BuildSelectorPath(bool includeLeafFragment)
        {
            ISelectable selectable;
            string result;

            result = null;

            selectable = this as ISelectable;
            if (selectable != null)
            {
                XliffElement ancestor;

                ancestor = this.SelectableAncestor as XliffElement;
                if (ancestor != null)
                {
                    if ((selectable.Id == null) || (!includeLeafFragment && selectable.IsLeafFragment))
                    {
                        result = ancestor.BuildSelectorPath(true);
                    }
                    else
                    {
                        result = ancestor.BuildSelectorPath(false) +
                                 Utilities.Constants.SelectorPathSeparator +
                                 selectable.SelectorId;
                    }
                }
                else
                {
                    result = selectable.SelectorId;
                }
            }

            return result;
        }

        /// <summary>
        /// Collapses all the elements under this element into a list. This includes all children of this element
        /// and all their children. Only elements of the specified type are returned.
        /// </summary>
        /// <typeparam name="TElement">The type of elements to get.</typeparam>
        /// <returns>A list of elements under the element.</returns>
        public List<TElement> CollapseChildren<TElement>() where TElement : class
        {
            return this.CollapseChildren<TElement>(CollapseScope.Default);
        }

        /// <summary>
        /// Collapses all the elements under this element into a list. This includes all children of this element
        /// and all their children. Only elements of the specified type are returned.
        /// </summary>
        /// <typeparam name="TElement">The type of elements to get.</typeparam>
        /// <param name="scope">The scope that defines what elements to look at as part of the collapse.</param>
        /// <returns>A list of elements under the element.</returns>
        public List<TElement> CollapseChildren<TElement>(CollapseScope scope) where TElement : class
        {
            List<TElement> result;
            List<XliffElement> temp;

            temp = new List<XliffElement>();
            this.CollapseChildren(scope, 0, new[] { typeof(TElement) }, null, temp);

            result = new List<TElement>();
            foreach (XliffElement element in temp)
            {
                result.Add(element as TElement);
            }

            return result;
        }

        /// <summary>
        /// Collapses all the elements under this element into a list. This includes all children of this element
        /// and all their children. Only elements of the specified type are returned.
        /// </summary>
        /// <param name="elementTypes">The types of children to return.</param>
        /// <returns>A list of elements under the element.</returns>
        public List<XliffElement> CollapseChildren(Type[] elementTypes)
        {
            return this.CollapseChildren(elementTypes, null);
        }

        /// <summary>
        /// Collapses all the elements under this element into a list. This includes all children of this element
        /// and all their children. Only elements of the specified type are returned.
        /// </summary>
        /// <param name="elementTypes">The types of children to return.</param>
        /// <param name="filterMethod">An optional method that will be called before children are added to the return
        /// list. If the method returns true for a specified child, the child may be returned along and its matching
        /// descendants. If the method returns false for a specified child, the child will not be returned, nor will
        /// any of its descendants.</param>
        /// <returns>A list of elements under the element.</returns>
        public List<XliffElement> CollapseChildren(Type[] elementTypes, Func<XliffElement, bool> filterMethod)
        {
            List<XliffElement> result;

            result = new List<XliffElement>();
            this.CollapseChildren(CollapseScope.Default, 0, elementTypes, filterMethod, result);

            return result;
        }

        /// <summary>
        /// Collapses all the elements under this element into a list. This includes all children of this element
        /// and all their children. Only elements of the specified type are returned.
        /// </summary>
        /// <param name="scope">The scope that defines what elements to look at as part of the collapse.</param>
        /// <param name="depth">The depth of the recursion.</param>
        /// <param name="elementTypes">The types of children to return.</param>
        /// <param name="filterMethod">An optional method that will be called before children are added to the return
        /// list. If the method returns true for a specified child, the child may be returned along and its matching
        /// descendants. If the method returns false for a specified child, the child will not be returned, nor will
        /// any of its descendants.</param>
        /// <param name="list">The list of elements that match the type specified.</param>
        private void CollapseChildren(
                                      CollapseScope scope,
                                      int depth,
                                      Type[] elementTypes,
                                      Func<XliffElement, bool> filterMethod,
                                      List<XliffElement> list)
        {
            bool hasTypeMatch;
            bool processDescendants;

            if ((filterMethod == null) || filterMethod(this))
            {
                processDescendants = ((depth == 0) && scope.HasFlag(CollapseScope.TopLevelDescendants)) ||
                                     scope.HasFlag(CollapseScope.AllDescendants);

                hasTypeMatch = false;
                foreach (Type type in elementTypes)
                {
                    if (type.IsInstanceOfType(this))
                    {
                        hasTypeMatch = true;
                        break;
                    }
                }

                if (scope.HasFlag(CollapseScope.CurrentElement) && hasTypeMatch)
                {
                    list.Add(this);
                }

                if (processDescendants)
                {
                    int recurseDepth;

                    recurseDepth = depth + 1;

                    if (scope.HasFlag(CollapseScope.CoreElements) || scope.HasFlag(CollapseScope.Extensions))
                    {
                        CollapseScope recurseScope;

                        if (scope.HasFlag(CollapseScope.CoreElements))
                        {
                            // The recursive call adds the element so CurrentElement must be included.
                            recurseScope = scope | CollapseScope.CurrentElement;
                        }
                        else
                        {
                            // The recursive call adds the element so CurrentElement must not be included because only
                            // extensions are expected.
                            recurseScope = scope & ~CollapseScope.CurrentElement;
                        }

                        if (this.HasChildren)
                        {
                            foreach (ElementInfo child in this.GetChildren())
                            {
                                child.Element.CollapseChildren(
                                                               recurseScope,
                                                               recurseDepth,
                                                               elementTypes,
                                                               filterMethod,
                                                               list);
                            }
                        }
                    }

                    // Add extensions.
                    if (scope.HasFlag(CollapseScope.Extensions) && (this is IExtensible))
                    {
                        IExtensible extensible;
                        CollapseScope recurseScope;

                        extensible = (IExtensible)this;

                        // The recursive call adds the element so CurrentElement must be included. Extensions may have
                        // native Xliff types but those are treated as extensions in this context, so include
                        // CoreElements in the recursive call.
                        recurseScope = scope | CollapseScope.CurrentElement | CollapseScope.CoreElements;

                        foreach (IExtension extension in extensible.Extensions)
                        {
                            foreach (ElementInfo info in extension.GetChildren())
                            {
                                info.Element.CollapseChildren(
                                                              recurseScope,
                                                              recurseDepth,
                                                              elementTypes,
                                                              filterMethod,
                                                              list);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="XliffElement"/> depending on the XLIFF element Name.
        /// </summary>
        /// <param name="name">The XLIFF element Name.</param>
        /// <returns>An instance of a class associated with the specified XLIFF Name.</returns>
        public virtual XliffElement CreateElement(XmlNameInfo name)
        {
            Type elementType;
            XliffElement result;

            elementType = null;
            foreach (Type type in this.childMap.Keys)
            {
                XmlNameInfo info;

                // If namespace isn't supplied then the information isn't available to compare so assume
                // they match. This happens if decoding Xml fragments without the full document.
                info = this.childMap[type];
                if ((name.LocalName == info.LocalName) &&
                    ((name.Namespace == null) || (name.Namespace == info.Namespace)))
                {
                    elementType = type;
                    break;
                }
            }

            result = null;
            if (elementType != null)
            {
                // Cannot call Activator.CreateInstance because the .ctor may not be public.
                // Cannot call GetConstructor() directly because it doesn't exist in PCL.
                result = (XliffElement)Reflector.InvokeDefaultConstructor(elementType);
            }

            return result;
        }

        /// <summary>
        /// Creates a new <see cref="XliffElement"/> depending on the XLIFF element Name.
        /// </summary>
        /// <param name="name">The XLIFF element Name.</param>
        /// <returns>An instance of a class associated with the specified XLIFF Name.</returns>
        IXliffDataConsumer IXliffDataConsumer.CreateXliffElement(XmlNameInfo name)
        {
            return this.CreateElement(name);
        }

        /// <summary>
        /// Enables or disables an attribute. If an attribute is disabled, access to it (read or write) will cause
        /// a NotSupportedException to be thrown.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="enable">Indicates whether the property is enabled or not.</param>
        protected void EnableAttribute(string name, bool enable)
        {
            this.hasDisabledAttribute |= !enable;
            this.attributes[name].IsSupported = enable;
        }

        /// <summary>
        /// Finds the first ancestor of the matching type.
        /// </summary>
        /// <typeparam name="TElement">The type of the ancestor to find.</typeparam>
        /// <returns>The first ancestor of the matching type, or null if one is not found.</returns>
        public TElement FindAncestor<TElement>() where TElement : class
        {
            XliffElement element;

            element = this.Parent;
            while ((element != null) && !(element is TElement))
            {
                element = element.Parent;
            }

            return element as TElement;
        }

        /// <summary>
        /// Gets the child <see cref="XliffElement"/>s contained within this object.
        /// </summary>
        /// <returns>A list of child elements stored as a pair consisting of the XLIFF Name for the child and
        /// the child itself.</returns>
        protected virtual List<ElementInfo> GetChildren()
        {
            return new List<ElementInfo>();
        }

        /// <summary>
        /// Gets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <returns>The text within the <see cref="XliffElement"/>.</returns>
        protected virtual string GetInnerText()
        {
            return null;
        }

        /// <summary>
        /// Gets the value of a property.
        /// </summary>
        /// <param name="property">The name of the property to get.</param>
        /// <returns>The value of the attribute in its native type.</returns>
        protected object GetPropertyValue(string property)
        {
            object result;

            this.GetPropertyValue(property, false, out result);
            return result;
        }

        /// <summary>
        /// Gets the value of a property.
        /// </summary>
        /// <param name="property">The name of the property to get.</param>
        /// <param name="asString">True if the value is returned as a string, false if it is returned in its
        /// native type.</param>
        /// <param name="result">The value of the attribute in its native type or a string. The value may come from
        /// this object or inherited from an ancestor.</param>
        /// <returns>True if the property exists on this object, otherwise false.</returns>
        internal bool GetPropertyValue(string property, bool asString, out object result)
        {
            return this.GetPropertyValue(property, asString, false, out result);
        }

        /// <summary>
        /// Gets the value of a property.
        /// </summary>
        /// <param name="property">The name of the property to get.</param>
        /// <param name="asString">True if the value is returned as a string, false if it is returned in its
        /// native type.</param>
        /// <param name="inheritedValueOnly">True to ignore the value of the attribute on this object and instead return only the
        /// inherited value if one exists. If false, the value of the attribute on this object will be returned if
        /// present, otherwise an inherited value may be returned.</param>
        /// <param name="result">The value of the attribute in its native type or a string. The value may come from
        /// this object or inherited from an ancestor.</param>
        /// <returns>True if the property exists on this object, otherwise false.</returns>
        internal bool GetPropertyValue(string property, bool asString, bool inheritedValueOnly, out object result)
        {
            AttributeData data;
            bool hasProperty;

            result = null;
            hasProperty = this.attributes.TryGetValue(property, out data);

            if (hasProperty && data.IsSupported)
            {
                if (asString)
                {
                    // This uses the converter of the ancestor property, not the property of this object.
                    result = data.GetStringValue();
                }
                else
                {
                    result = data.Value;
                }

                if ((inheritedValueOnly || !data.HasValue) && data.InheritValue)
                {
                    Dictionary<Type, string> inheritanceMap;
                    InheritanceInfo.InheritanceHandlerDelegate handler;
                    XliffElement ancestor;

                    handler = null;

                    inheritanceMap = new Dictionary<Type, string>();
                    foreach (InheritanceInfo info in data.InheritanceList)
                    {
                        Type type;

                        if (info.InheritanceHandler != null)
                        {
                            handler = info.InheritanceHandler;

                            // Only one handler is used.
                            break;
                        }

                        type = info.AncestorInheritanceType ?? typeof(XliffElement);
                        inheritanceMap[type] = info.AncestorPropertyName;
                    }

                    if (handler != null)
                    {
                        // Custom handler overrides the rest of the inheritance settings.
                        result = handler(this, property);

                        if (asString)
                        {
                            result = data.GetStringValue(result);
                        }

                        // Handlers must always return a value.
                        hasProperty = true;
                    }
                    else
                    {
                        // If no ancestor type, then use the parent which is an XliffElement.
                        ancestor = this;
                        hasProperty = false;

                        do
                        {
                            ancestor = ancestor.Parent;

                            foreach (Type key in inheritanceMap.Keys)
                            {
                                if (key.IsInstanceOfType(ancestor))
                                {
                                    string newProperty;

                                    // Get a different property Name if specified.
                                    newProperty = inheritanceMap[key] ?? property;

                                    if (asString)
                                    {
                                        // This uses the converter of the ancestor property, not the property of
                                        // this object.
                                        hasProperty = ancestor.GetPropertyValue(newProperty, true, false, out result);
                                    }
                                    else
                                    {
                                        hasProperty = ancestor.GetPropertyValue(newProperty, false, false, out result);
                                    }

                                    if (hasProperty)
                                    {
                                        break;
                                    }
                                }
                            }

                            if (hasProperty)
                            {
                                break;
                            }
                        }
                        while (ancestor != null);
                    }
                }
            }

            return hasProperty;
        }

        /// <summary>
        /// Gets the list of attributes stored in this object. These correspond to XLIFF element attributes.
        /// </summary>
        /// <returns>A dictionary of attributes where the key is the XLIFF Name and the value is the full
        /// attribute details.</returns>
        IEnumerable<IAttributeDataProvider> IXliffDataConsumer.GetXliffAttributes()
        {
            List<IAttributeDataProvider> result;

            result = new List<IAttributeDataProvider>();

            if (this.hasDisabledAttribute)
            {
                foreach (KeyValuePair<string, AttributeData> data in this.attributes)
                {
                    if (data.Value.IsSupported)
                    {
                        result.Add(new AttributeDataProvider(this, data.Key, data.Value));
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, AttributeData> data in this.attributes)
                {
                    result.Add(new AttributeDataProvider(this, data.Key, data.Value));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the list of attributes stored in this object. These correspond to XLIFF element attributes.
        /// </summary>
        /// <returns>A dictionary of attributes where the key is the XLIFF Name and the value is the full
        /// attribute details.</returns>
        IEnumerable<IAttributeDataProvider> IXliffDataProvider.GetXliffAttributes()
        {
            return ((IXliffDataConsumer)this).GetXliffAttributes();
        }

        /// <summary>
        /// Gets the child <see cref="XliffElement"/>s contained within this object.
        /// </summary>
        /// <returns>A list of child elements stored as a pair consisting of the XLIFF Name for the child and
        /// the child itself.</returns>
        IEnumerable<ElementInfo> IXliffDataProvider.GetXliffChildren()
        {
            return this.GetChildren();
        }

        /// <summary>
        /// Gets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <returns>The text within the <see cref="XliffElement"/>.</returns>
        string IXliffDataProvider.GetXliffText()
        {
            return this.GetInnerText();
        }

        /// <summary>
        /// Registers the definition of an attribute so its value can be stored and retrieved.
        /// </summary>
        /// <param name="ns">The Xml namespace the attribute should be serialized in.</param>
        /// <param name="localName">The Xml local name the attribute should be serialized as.</param>
        /// <param name="friendlyName">The name used to reference the attribute in code.</param>
        /// <param name="value">The default value of the attribute.</param>
        protected void RegisterAttribute(string ns, string localName, string friendlyName, object value)
        {
            AttributeData data;

            ArgValidator.Create(localName, "localName").IsNotNullOrWhitespace();
            ArgValidator.Create(friendlyName, "friendlyName").IsNotNullOrWhitespace();

            if (this.attributes.ContainsKey(friendlyName))
            {
                string message;

                message = string.Format(
                                        Properties.Resources.XliffElement_AttributeAlreadyRegistered_Format,
                                        friendlyName);
                throw new AttributeAlreadyRegisteredException(message);
            }

            data = new AttributeData(this.GetType().Name, new XmlNameInfo(ns, localName), false, value, true);
            this.attributes.Add(friendlyName, data);
        }

        /// <summary>
        /// Registers children and attribute definitions via a supplied map.
        /// </summary>
        /// <param name="information">The information about children and attributes to register.</param>
        internal void RegisterElementInformation(IElementInformation information)
        {
            if (!this.registered)
            {
                foreach (KeyValuePair<Type, XmlNameInfo> pair in information.ChildMap)
                {
                    this.childMap.Add(pair);
                }

                foreach (KeyValuePair<string, AttributeData> pair in information.AttributeMap)
                {
                    this.attributes.Add(pair);
                }

                this.registered = true;
            }
        }

        /// <summary>
        /// Selects an <see cref="XliffElement"/> matching the selection query.
        /// </summary>
        /// <param name="path">The selection query that is relative to the current object.</param>
        /// <returns>The object that was selected from the query path, or null if no match was found.</returns>
        /// <example>The value of <paramref name="path"/> might look something like "g=group1/f=file1/u=unit1/n=note1"
        /// which is a relative path from the current object, not a full path from the document root.</example>
        protected ISelectable SelectElement(string path)
        {
            int index;
            string elementId;
            ISelectable result;
            List<ElementInfo> children;

            // The path looks like "f=f1/group=g1...". Remove the first part because that's the child of this object.
            index = path.IndexOf(Utilities.Constants.SelectorPathSeparator);
            elementId = (index >= 0) ? path.Substring(0, index) : path;
            result = null;

            // Check if any children match the selection criteria first.
            children = this.GetChildren();
            if (children != null)
            {
                foreach (ElementInfo pair in children)
                {
                    ISelectable child;

                    child = pair.Element as ISelectable;

                    if (child != null)
                    {
                        if (child.SelectorId == elementId)
                        {
                            ISelectNavigable navigable;

                            // Found the child.
                            navigable = child as ISelectNavigable;
                            if (index < 0)
                            {
                                // The elementId is a leaf in the pattern, so this child is the one to return.
                                // The child matches "f=file1" in the pattern "f=file1";
                                result = child;
                            }
                            else if (navigable == null)
                            {
                                // Nothing else to search, so no match found.
                            }
                            else
                            {
                                // Continue searching the rest of the path starting with the child. The child matches
                                // "f=file1" in the pattern "#f=file1/g=group1". Search for "g=group1".
                                result = navigable.Select(Utilities.MakeRelativeSelector(path.Substring(index + 1)));
                            }

                            break;
                        }
                    }
                }
            }

            if ((result == null) && (children != null))
            {
                // No match was found for the elementId. Try searching the children with the same pattern.
                foreach (ElementInfo pair in children)
                {
                    ISelectNavigable navigable;

                    navigable = pair.Element as ISelectNavigable;
                    if (navigable != null)
                    {
                        result = navigable.Select(Utilities.MakeRelativeSelector(path));
                        if (result != null)
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="text">The text to set.</param>
        protected virtual void SetInnerText(string text)
        {
            string message;

            message = string.Format(
                                    Properties.Resources.XliffElement_InnerTextNotSupported_Format,
                                    this.GetType().Name);
            throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Gets the value of a property.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="property">The name of the property to set.</param>
        protected void SetPropertyValue(object value, string property)
        {
            this.attributes[property].Value = value;
        }

        /// <summary>
        /// Sets the text associated with the object.
        /// </summary>
        /// <param name="text">The text associated with the object.</param>
        void IXliffDataConsumer.SetXliffText(string text)
        {
            this.SetInnerText(text);
        }

        /// <summary>
        /// Stores the <see cref="XliffElement"/> as a child of this <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>True if the child was stored, otherwise false.</returns>
        protected virtual bool StoreChild(ElementInfo child)
        {
            return false;
        }

        /// <summary>
        /// Tries to set the value of an attribute.
        /// </summary>
        /// <param name="name">The XLIFF Name of the attribute.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>A <see cref="SetAttributeResult"/> that indicates whether the attribute was set.</returns>
        SetAttributeResult IXliffDataConsumer.TrySetAttributeValue(XmlNameInfo name, string value)
        {
            SetAttributeResult result;

            result = SetAttributeResult.Success;
            if (name.LocalName == NamespacePrefixes.XmlNamespace)
            {
                // xmlns shouldn't be stored at all.
                result = SetAttributeResult.ReservedName;
            }
            else if (this.TrySetPropertyValue(name, value))
            {
                // Property saved.
                result = SetAttributeResult.Success;
            }
            else if ((name.Prefix == NamespacePrefixes.Xml) || Utilities.IsModuleNamespace(name.Namespace))
            {
                // xml:xx is not an extension.
                // Known modules cannot be stored as extensions.
                result = SetAttributeResult.InvalidAttribute;
            }
            else
            {
                // abc:xx might be an extension.
                result = SetAttributeResult.PossibleExtension;
            }

            return result;
        }

        /// <summary>
        /// Tries to set the value of an attribute.
        /// </summary>
        /// <param name="name">The XLIFF Name of the attribute.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>This method always returns true.</returns>
        protected virtual bool TrySetPropertyValue(XmlNameInfo name, string value)
        {
            bool result;

            result = false;
            foreach (AttributeData attribute in this.attributes.Values)
            {
                if (attribute.IsSupported &&
                    (name.Prefix != NamespacePrefixes.XmlNamespace) &&
                    (name.LocalName == attribute.LocalName) &&
                    (
                        name.Namespace == attribute.Namespace ||
                        attribute.IgnoreNamespace && name.Prefix == NamespacePrefixes.Xml
                    ))
                {
                    attribute.SetValue(value);
                    result = true;
                    break;
                }
            }

            return result;
        }
        #endregion Methods
    }
}
