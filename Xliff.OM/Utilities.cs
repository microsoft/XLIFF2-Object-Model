namespace Localization.Xliff.OM
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.TranslationCandidates;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class provides utility methods for the project.
    /// </summary>
    public static class Utilities
    {
        #region Methods
        /// <summary>
        /// Gets all the selectable inline tags under a container and stores them in the specified list.
        /// </summary>
        /// <param name="container">The container to search under.</param>
        /// <param name="list">The list to add the selectable inline tags to.</param>
        internal static void AddAllSelectableInlineTags(IResourceStringContentContainer container, List<ISelectable> list)
        {
            if (container != null)
            {
                foreach (ResourceStringContent content in container.Text)
                {
                    ISelectable selectable;

                    selectable = content as ISelectable;
                    if (selectable != null)
                    {
                        list.Add(selectable);
                    }

                    // Some inline tags support nested inline tags.
                    Utilities.AddAllSelectableInlineTags(content as IResourceStringContentContainer, list);
                }
            }
        }

        /// <summary>
        /// Compares two languages and returns a value indicating whether they are equivalent.
        /// </summary>
        /// <param name="lang1">The first language.</param>
        /// <param name="lang2">The language to compare against the first language.</param>
        /// <returns>True if the languages are equivalent, otherwise false.</returns>
        internal static bool AreLanguagesEqual(string lang1, string lang2)
        {
            return string.Equals(lang1, lang2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Enumerates the inline tags of the container and stores a hash of <see cref="Data"/> references
        /// in <paramref name="idDataMap"/>. Also stores a map from Id to the inline tag with that Id.
        /// </summary>
        /// <param name="container">The resource that contains inline tags to enumerate.</param>
        /// <param name="idDataMap">A dictionary whose key is the inline tag Id and the value is a hash composed of
        /// all the <see cref="Data"/> references referenced by the inline tag. If the inline tag doesn't support Data
        /// references then the item will not be included in the dictionary.</param>
        /// <param name="idMap">A dictionary whose key is the inline tag Id and the value is the corresponding
        /// inline tag. This value may be null if the map is not needed by the caller..</param>
        private static void BuildDataReferenceMap(
                                                  IResourceStringContentContainer container,
                                                  Dictionary<string, string> idDataMap,
                                                  Dictionary<string, IIdentifiable> idMap)
        {
            foreach (ResourceStringContent content in container.Text)
            {
                IIdentifiable identifiable;

                identifiable = content as IIdentifiable;
                if ((identifiable != null) && content.SupportsDataReferences)
                {
                    StringBuilder hash;

                    hash = new StringBuilder();
                    foreach (KeyValuePair<string, string> pair in content.AllDataReferences)
                    {
                        hash.AppendFormat("|||{0}:{1}", pair.Key, pair.Value);
                    }

                    idDataMap[identifiable.Id] = hash.ToString();

                    if (idMap != null)
                    {
                        idMap[identifiable.Id] = identifiable;
                    }
                }

                // Some inline tags support nested inline tags.
                if (content is IResourceStringContentContainer)
                {
                    Utilities.BuildDataReferenceMap((IResourceStringContentContainer)content, idDataMap, idMap);
                }
            }
        }

        /// <summary>
        /// Collapses all the elements for all the specified elements and returns them in a single list. This includes
        /// all children of this element and all their children. Only elements of the specified type are returned.
        /// </summary>
        /// <typeparam name="TElement">The type of elements to get.</typeparam>
        /// <param name="elements">The elements to search in.</param>
        /// <returns>A list of elements under the specified elements.</returns>
        internal static List<TElement> CollapseChildren<TElement>(IEnumerable<XliffElement> elements)
                where TElement : class
        {
            return Utilities.CollapseChildren<TElement>(elements, CollapseScope.Default);
        }

        /// <summary>
        /// Collapses all the elements for all the specified elements and returns them in a single list. The scope
        /// determines which elements to search in. Only elements of the specified type are returned.
        /// </summary>
        /// <typeparam name="TElement">The type of elements to get.</typeparam>
        /// <param name="elements">The elements to search in.</param>
        /// <param name="scope">The scope that defines what elements to look at as part of the collapse.</param>
        /// <returns>A list of elements under the element.</returns>
        internal static List<TElement> CollapseChildren<TElement>(IEnumerable<XliffElement> elements, CollapseScope scope)
                where TElement : class
        {
            List<TElement> result;

            result = new List<TElement>();
            foreach (XliffElement element in elements)
            {
                result.AddRange(element.CollapseChildren<TElement>(scope));
            }

            return result;
        }

        /// <summary>
        /// Creates the SelectorId that identifies an element.
        /// </summary>
        /// <param name="prefix">The prefix that identifies the type of the element.</param>
        /// <param name="id">The Id of the element.</param>
        /// <returns>A SelectorId that identifies an element.</returns>
        internal static string CreateSelectorId(string prefix, string id)
        {
            return prefix + Utilities.Constants.SelectorPathEquals + id;
        }

        /// <summary>
        /// Updates inline tag identifiers for a <see cref="Target"/> element,
        /// so that they match <see cref="Source"/> inline tags with identical references.
        /// </summary>
        /// <param name="target">Target element to update.</param>
        public static void FixTargetIdentifiers(Target target)
        {
            Dictionary<string, string> idDataMapTarget;
            Dictionary<string, IIdentifiable> idMapTarget;
            Source source;

            // Get the mapping from Target inline tag Id to the hash of Data references.
            idDataMapTarget = new Dictionary<string, string>();
            idMapTarget = new Dictionary<string, IIdentifiable>();
            Utilities.BuildDataReferenceMap(target, idDataMapTarget, idMapTarget);

            if (idDataMapTarget.Count > 0)
            {
                Dictionary<string, string> idDataMapSource;
                IContainerResource container;

                // At least one target inline tag must have the same Id as a source, otherwise
                // validation will fail.
                container = target.FindAncestor<IContainerResource>();
                if (container == null)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.Codec_ElementNotDescendant_Format,
                                            "Target",
                                            "Ignorable, Match, Segment");
                    throw new InvalidOperationException(message);
                }

                source = container.Source;
                if (source == null)
                {
                    throw new InvalidOperationException(Properties.Resources.Utilities_TargetMustHaveSource);
                }

                // Get the mapping from Source inline tag Id to the hash of Data references.
                idDataMapSource = new Dictionary<string, string>();
                Utilities.BuildDataReferenceMap(source, idDataMapSource, null);

                // Match up the Ids between target and source. The keys in the dictionaries are different
                // because the elements themselves have different Ids, but the values (hash) may match if the
                // elements reference the same set of Data elements. If that happens, map the Target element
                // to the Source element.
                foreach (string id in idDataMapTarget.Keys)
                {
                    foreach (string idSource in idDataMapSource.Keys)
                    {
                        if (idDataMapSource[idSource] == idDataMapTarget[id])
                        {
                            idMapTarget[id].Id = idSource;

                            // Remove the id from the source map to avoid reusing it for another target.
                            idDataMapSource.Remove(idSource);

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the selectable inline tags under a <see cref="Unit"/> that reside under a
        /// <see cref="Source"/> element.
        /// </summary>
        /// <param name="unit">The unit to search under.</param>
        /// <param name="fromSource">True to get all inline tags from Sources, otherwise get them from Targets.</param>
        /// <returns>A list of selectable inline tags.</returns>
        internal static List<ISelectable> GetAllSelectableInlineTags(Unit unit, bool fromSource)
        {
            List<ISelectable> result;

            result = new List<ISelectable>();
            foreach (ContainerResource container in unit.Resources)
            {
                if (fromSource)
                {
                    Utilities.AddAllSelectableInlineTags(container.Source, result);
                }
                else if (container.Target != null)
                {
                    Utilities.AddAllSelectableInlineTags(container.Target, result);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the contents of a stream in the form of a string.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>The text in the stream.</returns>
        /// <remarks>This method seeks to the beginning of the stream before reading from it.</remarks>
        internal static string GetAllStreamText(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using (TextReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Gets a <see cref="OriginalData"/> from the ancestor that stores the original data for the specified element.
        /// </summary>
        /// <param name="element">The element whose original data host to find.</param>
        /// <returns>The OriginalData that stores the data for the encoded element.</returns>
        internal static OriginalData GetOriginalData(XliffElement element)
        {
            OriginalData result;
            Match match;

            result = null;

            match = element as Match;
            if (match == null)
            {
                match = element.FindAncestor<Match>();
            }

            if (match != null)
            {
                result = match.OriginalData;
            }
            else
            {
                Unit unit;

                unit = element as Unit;
                if (unit == null)
                {
                    unit = element.FindAncestor<Unit>();
                }

                if (unit != null)
                {
                    result = unit.OriginalData;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a value indicating whether any attribute extensions are present on the specified extensible object.
        /// </summary>
        /// <param name="extensible">The object that may have extensions to check.</param>
        /// <returns>True if at least one attribute extension exists, otherwise false.</returns>
        internal static bool HasExtensionAttributes(IExtensible extensible)
        {
            bool result;

            result = false;
            if (extensible.HasExtensions)
            {
                foreach (IExtension extension in extensible.Extensions)
                {
                    result = extension.HasAttributes;
                    if (result)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a value indicating whether any element extensions are present on the specified extensible object.
        /// </summary>
        /// <param name="extensible">The object that may have extensions to check.</param>
        /// <returns>True if at least one element extension exists, otherwise false.</returns>
        internal static bool HasExtensionChildElements(IExtensible extensible)
        {
            bool result;

            result = false;
            if (extensible.HasExtensions)
            {
                foreach (IExtension extension in extensible.Extensions)
                {
                    result = extension.HasChildren;
                    if (result)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a value indicating whether the specified namespace represents an XLIFF core namespace.
        /// </summary>
        /// <param name="ns">The namespace to check.</param>
        /// <returns>True if the namespace represents an XLIFF core namespace, otherwise false.</returns>
        internal static bool IsCoreNamespace(string ns)
        {
            return ns == NamespaceValues.Core;
        }

        /// <summary>
        /// Returns a value indicating whether the specified namespace represents an XLIFF module namespace.
        /// </summary>
        /// <param name="ns">The namespace to check.</param>
        /// <returns>True if the namespace represents an XLIFF module namespace, otherwise false.</returns>
        internal static bool IsModuleNamespace(string ns)
        {
            return (ns == NamespaceValues.ChangeTrackingModule) ||
                   (ns == NamespaceValues.FormatStyleModule) ||
                   (ns == NamespaceValues.GlossaryModule) ||
                   (ns == NamespaceValues.MetadataModule) ||
                   (ns == NamespaceValues.ResourceDataModule) ||
                   (ns == NamespaceValues.SizeRestrictionModule) ||
                   (ns == NamespaceValues.TranslationCandidatesModule) ||
                   (ns == NamespaceValues.ValidationModule);
        }

        /// <summary>
        /// Checks if the specified value is compliant with the BCP-47 standard.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is compliant, otherwise false.</returns>
        /// <remarks>See <a href="http://tools.ietf.org/html/bcp47">BCP-47 standard</a> for more details.</remarks>
        internal static bool IsValidBcp47Language(string value)
        {
            const string Alpha = "[A-Za-z]";
            const string Digit = "[0-9]";
            const string AlphaNum = "[A-Za-z0-9]";
            const string Regular = "(art-lojban|cel-gaulish|no-bok|no-nyn|zh-guoyu|zh-hakka|zh-min|zh-min-nan|zh-xiang)";
            const string Irregular = "(en-GB-oed|i-ami|i-bnn|i-default|i-enochian|i-hak|i-klingon|i-lux|i-mingo|i-navajo|i-pwn|i-tao|i-tay|i-tsu|sgn-BE-FR|sgn-BE-NL|sgn-CH-DE)";
            const string Grandfathered = "(" + Irregular + "|" + Regular + ")";
            const string PrivateUse = "(x(-" + AlphaNum + "{1,8})+)";
            const string Singleton = "[0-9A-WY-Za-wy-z]";
            const string Extension = "(" + Singleton + "(-" + AlphaNum + "{2,8})+)";
            const string Variant = "(" + AlphaNum + "{5,8}|" + Digit + AlphaNum + "{3})";
            const string Region = "(" + Alpha + "{2}|" + Digit + "{3})";
            const string Script = "(" + Alpha + "{4})";
            const string ExtLang = "(" + Alpha + "{3}(-" + Alpha + "{3}){0,2})";
            const string Language = "((" + Alpha + "{2,3}(-" + ExtLang + ")?)|" + Alpha + "{4}|" + Alpha + "{5,8})";
            const string LangTag = "(" + Language + "(-" + Script + ")?" + "(-" + Region + ")?" + "(-" + Variant + ")*" + "(-" + Extension + ")*" + "(-" + PrivateUse + ")?" + ")";
            const string LanguageTag = @"^(" + Grandfathered + "|" + LangTag + "|" + PrivateUse + ")$";

            return System.Text.RegularExpressions.Regex.IsMatch(value, LanguageTag);
        }

        /// <summary>
        /// Returns a value indicating whether the given name conforms to the NMTOKEN standard.
        /// </summary>
        /// <param name="name">The name to validate.</param>
        /// <param name="exceptionInfo">If validation fails, this will contain the exception that is thrown by the
        /// validator.</param>
        /// <returns>True if the name conforms to a valid NMTOKEN, otherwise false.</returns>
        internal static bool IsValidNMTOKEN(string name, out Exception exceptionInfo)
        {
            exceptionInfo = null;

            try
            {
                XmlConvert.VerifyNMTOKEN(name);
            }
            catch (Exception e)
            {
                exceptionInfo = e;
            }

            return exceptionInfo == null;
        }

        /// <summary>
        /// Returns a value indicating whether the specified path represents a well-formed selector path excluding
        /// resource strings and inline tags. The actual path is not validated, just the structure of it.
        /// </summary>
        /// <param name="path">The path to examine.</param>
        /// <param name="restrictPrefix">If true, the prefixes of the selector path will be validated as if they are
        /// for extensions or modules. If false, the prefixes won't be validated meaning they may be from core elements,
        /// extensions, or modules.</param>
        /// <returns>True if the path represents a well-formed selector path, false if it does not.</returns>
        /// <remarks>Resource strings and inline tags don't require a prefix, nor do they support extensions so
        /// validating those requires more knowledge about the XliffElements and isn't done here.
        /// <para>
        /// This method does not validate that the path is actually valid. For instance, it doesn't validate that a path
        /// looks like "#/f=f1" rather than "#/foo=bar" (ie. the root fragment MUST be a file). Instead, this method
        /// just looks for paths of the form:
        ///     expression          ::= "#" ["/"] selector {selectorSeparator selector}
        ///     selector            ::= prefix prefixSeparator id
        ///     prefix              ::= NMTOKEN
        ///     id                  ::= NMTOKEN
        ///     prefixSeparator     ::= "="
        ///     selectorSeparator   ::= "/"
        /// </para>
        /// </remarks>
        internal static bool IsWellFormedSelectorPathExcludingResourceStrings(string path, bool restrictPrefix)
        {
            bool result;
            int requiredPrefixLength;

            result = false;
            requiredPrefixLength = restrictPrefix ? Constants.MinModuleOrExtensionSelectorPathPrefixLength : 1;

            if ((path != null) && path.StartsWith(Constants.SelectorPathIndictator))
            {
                string[] parts;

                // Remove the # so the path is now either f=f1/... or /f=f1/...
                path = path.Substring(Constants.SelectorPathIndictator.Length);
                parts = path.Split(new[] { Constants.SelectorPathSeparator });

                result = true;
                for (int i = 0; result && (i < parts.Length); i++)
                {
                    int index;
                    string fragment;

                    fragment = parts[i];

                    if ((i == 0) && (fragment.Length == 0))
                    {
                        // The path looks like #/f=f1 so the first fragment is empty.
                        continue;
                    }

                    // The fragment must look like f=f1 now.
                    result = false;

                    // Separate the f from f1 in "f=f1".
                    index = fragment.IndexOf(Constants.SelectorPathEquals);
                    if (index > 0)
                    {
                        Exception exceptionInfo;
                        string prefix;
                        string id;

                        prefix = fragment.Substring(0, index);
                        id = fragment.Substring(index + 1);

                        result = (prefix.Length >= requiredPrefixLength) &&
                                 (id.Length > 0) &&
                                 Utilities.IsValidNMTOKEN(prefix, out exceptionInfo) &&
                                 Utilities.IsValidNMTOKEN(id, out exceptionInfo);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Checks to see if a Unicode code point represents a symbol that is safe to include in an XML file.
        /// </summary>
        /// <param name="code">Code point to verify.</param>
        /// <returns>
        /// true - code point represents a symbol that can be included in an XML file, 
        /// false - code point needs to be escaped before including it.
        /// </returns>
        public static bool IsValidXmlCodePoint(int code)
        {
            // Valid XML characters are defined at http://www.w3.org/TR/2000/REC-xml-20001006#NT-Char.
            // Code points ::= #x9 | #xA | #xD | [#x20 - #xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]
            // Note that [#x10000-#x10FFFF] range is for surrogate pairs which cannot be represented
            // by a single char, so we don't need to include that range in the check.
            if ((code == 0x9) ||
                (code == 0xA) ||
                (code == 0xD) ||
                ((code >= 0x20) && (code <= 0xD7FF)) ||
                ((code >= 0xE000) && (code <= 0xFFFD)) ||
                ((code >= 0x10000) && (code <= 0x10FFFF)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Converts a reference to a valid IRI.
        /// </summary>
        /// <param name="reference">The reference to convert.</param>
        /// <returns>An IRI for the reference, or null if reference is null.</returns>
        public static string MakeIri(string reference)
        {
            return (reference == null) ? null : ("#" + reference);
        }

        /// <summary>
        /// Converts the path to a valid selector path.
        /// </summary>
        /// <param name="path">The path to convert.</param>
        /// <returns>A valid selector path.</returns>
        internal static string MakeRelativeSelector(string path)
        {
            return Utilities.Constants.SelectorPathIndictator + path;
        }

        /// <summary>
        /// Removes the last fragment in a selector path.
        /// </summary>
        /// <param name="path">The selector path.</param>
        /// <param name="fragment">The fragment that is removed, or null if one is not present.</param>
        /// <returns>The selector path with the last fragment removed.</returns>
        internal static string RemoveLastFragment(string path, out string fragment)
        {
            int index;
            string result;

            index = path.LastIndexOf(Utilities.Constants.SelectorPathSeparator);
            if (index > 0)
            {
                result = path.Substring(0, index);
                fragment = path.Substring(index + 1);
            }
            else
            {
                result = path;
                fragment = null;
            }

            return result;
        }

        /// <summary>
        /// Removes the characters from a selector path that indicate the path is a selector path. This leaves just
        /// the selection text behind (ex. f=file/g=group1).
        /// </summary>
        /// <param name="path">The path that may contain a selector indicator to remove.</param>
        /// <returns>The path without the leading selector indicator.</returns>
        internal static string RemoveSelectorIndicator(string path)
        {
            string result;

            Utilities.TryRemoveSelectorIndicator(path, out result);
            return result;
        }

        /// <summary>
        /// Sets the parent of an <see cref="XliffElement"/>.
        /// </summary>
        /// <param name="element">The element whose parent to set.</param>
        /// <param name="parent">The parent to set.</param>
        internal static void SetParent(XliffElement element, XliffElement parent)
        {
            if (element != null)
            {
                element.Parent = parent;
            }
        }

        /// <summary>
        /// Handler called when getting the custom Directionality value from a spanning code element. The value
        /// depends on the hierarchy of the ancestry.
        /// </summary>
        /// <param name="element">The element that the property is for.</param>
        /// <param name="property">The name of the property that is to be returned.</param>
        /// <returns>The custom value.</returns>
        internal static object SpanningCodeInheritanceHandler(XliffElement element, string property)
        {
            object result;

            Debug.Assert(property == "Directionality", "Property name is not supported.");

            result = ContentDirectionality.Auto;
            if ((element is SpanningCode) || (element is SpanningCodeEnd) || (element is SpanningCodeStart))
            {
                XliffElement parent;

                parent = element.Parent;
                while (parent != null)
                {
                    if (parent is SpanningCode)
                    {
                        parent.GetPropertyValue("Directionality", false, out result);
                        break;
                    }
                    else if ((parent is Target) || (parent is Source))
                    {
                        bool isTarget;

                        isTarget = parent is Target;
                        while ((parent != null) && !(parent is Unit))
                        {
                            parent = parent.Parent;
                        }

                        if (parent != null)
                        {
                            if (isTarget)
                            {
                                parent.GetPropertyValue("TargetDirectionality", false, out result);
                            }
                            else
                            {
                                parent.GetPropertyValue("SourceDirectionality", false, out result);
                            }
                        }

                        break;
                    }

                    parent = parent.Parent;
                }
            }

            return result;
        }

        /// <summary>
        /// Splits a string of the form "prefix:value" into different strings.
        /// </summary>
        /// <param name="text">The text to split.</param>
        /// <param name="delimiter">The delimiter that divides the prefix and value.</param>
        /// <param name="prefix">The value of the prefix, or null if the delimiter was not found.</param>
        /// <param name="value">The text after the first delimiter found. This may contain the delimiter character
        /// if the text contains more than one delimiter characters. If no delimiter was found then this contains
        /// all the text.</param>
        /// <returns>True if the text is of the form "prefix:value" and was split successfully, otherwise
        /// false.</returns>
        internal static bool SplitPrefixValue(string text, char delimiter, out string prefix, out string value)
        {
            bool result;

            prefix = null;
            value = null;
            result = false;

            if (text != null)
            {
                int index;

                index = text.IndexOf(delimiter);
                if (index >= 0)
                {
                    prefix = text.Substring(0, index);
                    value = text.Substring(index + 1);
                    result = true;
                }
                else
                {
                    value = text;
                }
            }

            return result;
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> exception if the specified value is not null.
        /// </summary>
        /// <param name="host">The element which contains the property to which to assign a new value.</param>
        /// <param name="propertyName">The name of the property trying to be assigned.</param>
        /// <param name="value">The current value of the property.</param>
        internal static void ThrowIfPropertyNotNull(XliffElement host, string propertyName, object value)
        {
            if (value != null)
            {
                string message;

                message = string.Format(
                                        Properties.Resources.XliffElement_ChildAlreadyExists_Format,
                                        host.GetType().Name,
                                        propertyName);
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Converts a list of objects to a list of ISelectable objects. Each object that is implements ISelectable
        /// is added to the list, otherwise the object is ignored.
        /// </summary>
        /// <param name="enumerable">The objects to enumerate.</param>
        /// <returns>A list of objects that implement ISelectable.</returns>
        internal static List<ISelectable> ToISelectableList(IEnumerable enumerable)
        {
            List<ISelectable> result;

            result = new List<ISelectable>();
            foreach (object element in enumerable)
            {
                ISelectable selectable;

                selectable = element as ISelectable;
                if (selectable != null)
                {
                    result.Add(selectable);
                }
            }

            return result;
        }

        /// <summary>
        /// Parses an IRI and returns the string used in the reference.
        /// </summary>
        /// <param name="iri">An IRI in the form of #id where id is the part to return.</param>
        /// <param name="reference">The Id referenced in the IRI, or the <paramref name="iri"/> if the IRI is not of the
        /// correct format.</param>
        /// <returns>True if the <paramref name="iri"/> is a valid IRI, otherwise false.</returns>
        public static bool TryParseIri(string iri, out string reference)
        {
            bool result;

            if (!string.IsNullOrEmpty(iri) && (iri[0] == '#'))
            {
                reference = iri.Substring(1);
                result = true;
            }
            else
            {
                result = false;
                reference = iri;
            }

            return result;
        }

        /// <summary>
        /// Removes the characters from a selector path that indicate the path is a selector path. This leaves just
        /// the selection text behind (ex. f=file/g=group1).
        /// </summary>
        /// <param name="path">The path that may contain a selector indicator to remove.</param>
        /// <param name="modifiedPath">The path without the leading selector indicator.</param>
        /// <returns>True if the selector indicator was removed, otherwise false.</returns>
        internal static bool TryRemoveSelectorIndicator(string path, out string modifiedPath)
        {
            bool result;

            if (path.StartsWith(Utilities.Constants.SelectorPathIndictator))
            {
                modifiedPath = path.Substring(Utilities.Constants.SelectorPathIndictator.Length);
                result = true;
            }
            else
            {
                modifiedPath = path;
                result = false;
            }

            return result;
        }
        #endregion Methods

        /// <summary>
        /// This class contains constant values that are used in project.
        /// </summary>
        public static class Constants
        {
            /// <summary>
            /// The minimum length of a selector path prefix for an extension or module.
            /// </summary>
            public const int MinModuleOrExtensionSelectorPathPrefixLength = 2;

            /// <summary>
            /// The character that represents the beginning of a selector path.
            /// </summary>
            public const string SelectorPathIndictator = "#";

            /// <summary>
            /// The character that separates selector items.
            /// </summary>
            public const char SelectorPathSeparator = '/';

            /// <summary>
            /// The character that separates a selector type from the item Id.
            /// </summary>
            public const char SelectorPathEquals = '=';
        }
    }
}
