namespace Localization.Xliff.OM.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Core.XmlNames;
    using Localization.Xliff.OM.Exceptions;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.ChangeTracking;
    using Localization.Xliff.OM.Modules.FormatStyle;
    using Localization.Xliff.OM.Modules.Glossary;
    using Localization.Xliff.OM.Modules.Metadata;
    using Localization.Xliff.OM.Modules.ResourceData;
    using Localization.Xliff.OM.Modules.SizeRestriction;
    using Localization.Xliff.OM.Modules.TranslationCandidates;
    using Localization.Xliff.OM.Modules.Validation;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class validates <see cref="XliffDocument"/> objects conform to the XLIFF version 2.0 spec defined at
    /// <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html"/>.
    /// </summary>
    /// <seealso cref="IXliffValidator"/>
    public class StandardValidator : IXliffValidator
    {
        #region Member Variables
        /// <summary>
        /// The list of MarkedSpanStart elements from a Source to find during validation to ensure references refer to
        /// valid elements. The value is the MarkedSpanEnd element that refers to the MarkedSpanStart.
        /// </summary>
        private readonly Dictionary<string, MarkedSpanEnd> markedSpanStartToFindSource;

        /// <summary>
        /// The list of MarkedSpanStart elements from a Target to find during validation to ensure references refer to
        /// valid elements. The value is the MarkedSpanEnd element that refers to the MarkedSpanStart.
        /// </summary>
        private readonly Dictionary<string, MarkedSpanEnd> markedSpanStartToFindTarget;

        /// <summary>
        /// The list of SpanningCodeStart elements from a Source to find during validation to ensure references refer to
        /// valid elements. The value is the SpanningCodeEnd element that refers to the SpanningCodeStart.
        /// </summary>
        private readonly Dictionary<string, SpanningCodeEnd> spanningCodeStartToFindSource;

        /// <summary>
        /// The list of SpanningCodeStart elements from a Target to find during validation to ensure references refer to
        /// valid elements. The value is the SpanningCodeEnd element that refers to the SpanningCodeStart.
        /// </summary>
        private readonly Dictionary<string, SpanningCodeEnd> spanningCodeStartToFindTarget;

        /// <summary>
        /// The document to validate.
        /// </summary>
        private XliffDocument document;
        #endregion Member Variables

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardValidator"/> class.
        /// </summary>
        public StandardValidator()
        {
            this.markedSpanStartToFindSource = new Dictionary<string, MarkedSpanEnd>();
            this.markedSpanStartToFindTarget = new Dictionary<string, MarkedSpanEnd>();
            this.spanningCodeStartToFindSource = new Dictionary<string, SpanningCodeEnd>();
            this.spanningCodeStartToFindTarget = new Dictionary<string, SpanningCodeEnd>();
        }

        #region IXliffValidator Implementation
        /// <summary>
        /// Validates the document structure and contents conform to the standard. If a validation error occurs,
        /// a <see cref="ValidationException"/> is thrown.
        /// </summary>
        /// <param name="document">The document to validate.</param>
        public void Validate(XliffDocument document)
        {
            ArgValidator.Create(document, "document").IsNotNull();

            this.document = document;

            try
            {
                this.ValidateImpl();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ValidationException(
                                              ValidationError.UnhandledException,
                                              Properties.Resources.StandardValidator_UnhandledException,
                                              e);
            }
        }
        #endregion IXliffValidator Implementation

        #region Static Methods
        /// <summary>
        /// Finds the first ancestor of a specified type.
        /// </summary>
        /// <typeparam name="TAncestor">The type of ancestor to find.</typeparam>
        /// <param name="element">The element whose ancestor chain to search.</param>
        /// <returns>The matching ancestor or null.</returns>
        private static TAncestor FindAncestor<TAncestor>(XliffElement element) where TAncestor : XliffElement
        {
            XliffElement parent;

            parent = element.Parent;
            while ((parent != null) && (parent.GetType() != typeof(TAncestor)))
            {
                parent = parent.Parent;
            }

            return parent as TAncestor;
        }

        /// <summary>
        /// Finds a <see cref="Data"/> element.
        /// </summary>
        /// <param name="original">The <see cref="OriginalData"/> under which to search.</param>
        /// <param name="id">The Id of the <see cref="Data"/> element to find.</param>
        /// <returns>The matching <see cref="Data"/> element, or null.</returns>
        private static Data FindDataElement(OriginalData original, string id)
        {
            Data result;

            result = null;
            if (original != null)
            {
                foreach (Data data in original.DataElements)
                {
                    if (data.Id == id)
                    {
                        result = data;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets all the selectable <see cref="ResourceStringContent"/> elements that reside under a given element.
        /// This includes all descendants as well.
        /// </summary>
        /// <param name="container">The element to search.</param>
        /// <returns>A dictionary whose key is the Id of the element, and the value is the
        /// <see cref="ResourceStringContent"/> with that Id.</returns>
        /// <remarks>This method assumes there are no Id collisions and an appropriate .NET exception will be thrown if
        /// there is one.</remarks>
        private static Dictionary<string, ResourceStringContent> GetSelectableResourceStringContents(XliffElement container)
        {
            Dictionary<string, ResourceStringContent> result;

            if (container == null)
            {
                result = null;
            }
            else
            {
                result = new Dictionary<string, ResourceStringContent>();
                foreach (ResourceStringContent child in container.CollapseChildren<ResourceStringContent>())
                {
                    ISelectable selectable;

                    selectable = child as ISelectable;
                    if ((selectable != null) && selectable.Id != null)
                    {
                        result.Add(selectable.Id, child);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the set of editing hint sequences from a list of <see cref="ResourceStringContent"/>. A sequence is
        /// a set of 2 or more elements where the first element has CanReorder set to FirstNo, and the subsequent
        /// elements have CanReorder set to No. Single element sequences are not included in the result. If any
        /// validation errors occur in any sequences a <see cref="ValidationException"/> will be thrown.
        /// </summary>
        /// <param name="contents">The list of elements to scan for sequences.</param>
        /// <returns>A dictionary whose key is the Id of the first element in the sequence, and the value is the list
        /// of elements in the sequence.</returns>
        private static Dictionary<string, List<CodeBase>> GetAndValidateEditingHintsSequences(
                                                                List<ResourceStringContent> contents)
        {
            Dictionary<string, List<CodeBase>> result;
            string sequenceStartId;
            List<CodeBase> currentSequence;
            const int MinTagsInSequence = 2;

            result = new Dictionary<string, List<CodeBase>>();
            currentSequence = new List<CodeBase>();
            sequenceStartId = null;

            foreach (ResourceStringContent tag in contents)
            {
                CodeBase codeBase;

                codeBase = tag as CodeBase;
                if (codeBase != null)
                {
                    // CanCopy and CanDelete MUST be set to false if CanReorder is set to No or FirstNo.
                    if ((codeBase.CanReorder != CanReorderValue.Yes) && (codeBase.CanCopy || codeBase.CanDelete))
                    {
                        throw new ValidationException(
                                            ValidationError.CodeBaseMismatchedCanReorderCopyDelete,
                                            Properties.Resources.StandardValidator_CanCopyOrDeleteInvalidForCanReorder,
                                            codeBase.SelectorPath);
                    }

                    switch (codeBase.CanReorder)
                    {
                        case CanReorderValue.FirstNo:
                            // End the existing sequence if one is present.
                            if (currentSequence.Count >= MinTagsInSequence)
                            {
                                result.Add(sequenceStartId, currentSequence);
                            }

                            // Start a new sequence with the current tag as the first in the sequence.
                            currentSequence = new List<CodeBase>();
                            currentSequence.Add(codeBase);
                            sequenceStartId = codeBase.Id;
                            break;

                        case CanReorderValue.No:
                            // Add the current tag to the current sequence. Fail if there is no current sequence.
                            if (currentSequence.Count == 0)
                            {
                                throw new ValidationException(
                                                              ValidationError.CodeBaseSequenceStartsWithCanReorderNo,
                                                              Properties.Resources.StandardValidator_SequenceCannotBeginWithNo,
                                                              codeBase.SelectorPath);
                            }

                            currentSequence.Add(codeBase);
                            break;

                        case CanReorderValue.Yes:
                            // End the existing sequence if one is present.
                            if (currentSequence.Count >= MinTagsInSequence)
                            {
                                result.Add(sequenceStartId, currentSequence);
                            }

                            currentSequence = new List<CodeBase>();
                            sequenceStartId = null;
                            break;

                        default:
                            Debug.Assert(
                                         false,
                                         string.Format("CanReorder value {0} is not supported.", codeBase.CanReorder));
                            break;
                    }
                }
            }

            // Add the remaining sequence if the contents end in a sequence.
            if (currentSequence.Count >= MinTagsInSequence)
            {
                result.Add(sequenceStartId, currentSequence);
            }

            return result;
        }

        /// <summary>
        /// Converts a list of <see cref="ResourceStringContent"/> elements to a dictionary.
        /// </summary>
        /// <param name="contents">The element to convert.</param>
        /// <returns>A dictionary whose key is the Id of the element, and the value is the
        /// <see cref="ResourceStringContent"/> with that Id. If <paramref name="contents"/> is null, null will be
        /// returned.</returns>
        /// <remarks>This method assumes there are no Id collisions and an appropriate .NET exception will be thrown if
        /// there is one.</remarks>
        private static Dictionary<string, ResourceStringContent> GetSelectableResourceStringContents(List<ResourceStringContent> contents)
        {
            Dictionary<string, ResourceStringContent> result;

            result = null;
            if (contents != null)
            {
                result = new Dictionary<string, ResourceStringContent>();
                foreach (ResourceStringContent child in contents)
                {
                    ISelectable selectable;

                    selectable = child as ISelectable;
                    if ((selectable != null) && selectable.Id != null)
                    {
                        result.Add(selectable.Id, child);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets all the selectable <see cref="ResourceStringContent"/> elements that reside under a given element as
        /// a list. This includes all descendants as well.
        /// </summary>
        /// <param name="container">The element to search.</param>
        /// <returns>A list of <see cref="ResourceStringContent"/> residing within the container.</returns>
        private static List<ResourceStringContent> GetSelectableResourceStringContentsAsList(XliffElement container)
        {
            List<ResourceStringContent> result;

            if (container == null)
            {
                result = null;
            }
            else
            {
                result = new List<ResourceStringContent>();
                foreach (ResourceStringContent child in container.CollapseChildren<ResourceStringContent>())
                {
                    ISelectable selectable;

                    selectable = child as ISelectable;
                    if ((selectable != null) && selectable.Id != null)
                    {
                        result.Add(child);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a boolean indicating whether the value is either null or between the <paramref name="min"/>
        /// and <paramref name="max"/> values.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>True if the <paramref name="value"/> is null or is between the <paramref name="min"/>
        /// and <paramref name="max"/> values.</returns>
        private static bool IsInRangeOrNull(float? value, float min, float max)
        {
            return !value.HasValue || ((value.Value >= min) && (value.Value <= max));
        }

        /// <summary>
        /// Throws a <see cref="ValidationException"/> indicating a spanning code end property value doesn't match
        /// its corresponding spanning code start value.
        /// </summary>
        /// <param name="property">The property that differs.</param>
        /// <param name="error">A number that identifies the error.</param>
        /// <param name="selectorPath">The selector path where the error occurs.</param>
        /// <remarks>This method always throws a <see cref="ValidationException"/>.</remarks>
        private static void ThrowSpanningCodeStartEndPropertyMismatchException(
                                                                               string property,
                                                                               int error,
                                                                               string selectorPath)
        {
            string message;

            message = string.Format(
                                    Properties.Resources.StandardValidator_StartEndPropertyMismatch_Format,
                                    property,
                                    typeof(SpanningCodeStart).Name,
                                    typeof(SpanningCodeEnd).Name);
            throw new ValidationException(error, message, selectorPath);
        }
        #endregion Static Methods

        #region Methods
        /// <summary>
        /// Scans the containers and all sub-containers for a <see cref="Target"/> element.
        /// </summary>
        /// <param name="containers">The containers to scan.</param>
        /// <returns>True if at least one <see cref="Target"/> element was found, otherwise false.</returns>
        private bool CheckForTarget(IList<TranslationContainer> containers)
        {
            bool hasTarget;

            hasTarget = false;
            foreach (TranslationContainer container in containers)
            {
                if (container is Group)
                {
                    hasTarget = this.CheckForTarget(((Group)container).Containers);
                }
                else
                {
                    foreach (ContainerResource resource in ((Unit)container).Resources)
                    {
                        hasTarget = resource.Target != null;
                        if (hasTarget)
                        {
                            break;
                        }
                    }
                }

                if (hasTarget)
                {
                    break;
                }
            }

            return hasTarget;
        }

        /// <summary>
        /// Gets the list of <see cref="TranslationContainer"/> elements under the file. This method uses recursion on
        /// all the <see cref="TranslationContainer"/> to return all <see cref="TranslationContainer"/>s.
        /// </summary>
        /// <typeparam name="TContainer">The type of container to get.</typeparam>
        /// <param name="file">The file under which to search.</param>
        /// <returns>A dictionary of <see cref="TranslationContainer"/>s of the specified type that reside under the
        /// file. The key is the container Id, the value is the container.</returns>
        private Dictionary<string, TContainer> GetTranslationContainers<TContainer>(File file)
                where TContainer : TranslationContainer
        {
            Dictionary<string, TContainer> result;
            List<TContainer> list;

            list = this.GetTranslationContainersList<TContainer>(file);
            result = new Dictionary<string, TContainer>();

            foreach (TContainer container in list)
            {
                result.Add(container.Id, (TContainer)container);
            }

            return result;
        }

        /// <summary>
        /// Gets the list of <see cref="TranslationContainer"/> elements under the file. This method uses recursion on
        /// all the <see cref="TranslationContainer"/> to return all <see cref="TranslationContainer"/>s.
        /// </summary>
        /// <typeparam name="TContainer">The type of container to get.</typeparam>
        /// <param name="file">The file under which to search.</param>
        /// <returns>The list of <see cref="TranslationContainer"/> of the specified type that reside under the
        /// file. Results are returned in no specific order.</returns>
        private List<TContainer> GetTranslationContainersList<TContainer>(File file)
                where TContainer : TranslationContainer
        {
            List<TContainer> result;
            Stack<TranslationContainer> children;

            result = new List<TContainer>();
            children = new Stack<TranslationContainer>(file.Containers);
            while (children.Count > 0)
            {
                TranslationContainer container;

                container = children.Pop();
                if (typeof(TContainer).IsInstanceOfType(container))
                {
                    result.Add((TContainer)container);
                }

                if (container is Group)
                {
                    foreach (TranslationContainer child in ((Group)container).Containers)
                    {
                        children.Push(child);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Validates the <see cref="CodePoint"/> conforms to the standard.
        /// </summary>
        /// <param name="point">The element to validate.</param>
        /// <returns>True if the code point was validated, false if the code point is null.</returns>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private bool TryValidateCodePoint(CodePoint point)
        {
            bool result;

            result = point != null;
            if (result)
            {
                if (Utilities.IsValidXmlCodePoint(point.Code))
                {
                    string message;

                    message = string.Format(Properties.Resources.CodePoint_InvalidCode, point.Code);
                    throw new ValidationException(
                                                  ValidationError.CodePointInvalidCode,
                                                  message,
                                                  point.SelectableAncestor.SelectorPath);
                }
            }

            return result;
        }

        /// <summary>
        /// Validates the <see cref="MarkedSpan"/> conforms to the standard.
        /// </summary>
        /// <param name="span">The element to validate.</param>
        /// <param name="contentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="ResourceString"/>.</param>
        /// <param name="sourceContentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="Source"/>. This may be null if the <paramref name="contentMap"/> refers to the Source elements
        /// as well.</param>
        /// <param name="markedSpanStartToFind">The list of MarkedSpanStart elements to find during validation to
        /// ensure references refer to valid elements.  The value is the MarkedSpanEnd element that refers to the
        /// MarkedSpanStart.</param>
        /// <param name="spanningCodeStartToFind">The list of SpanningCodeStart elements to find during validation to
        /// ensure references refer to valid elements. The value is the SpanningCodeEnd element that refers to the
        /// SpanningCodeStart.</param>
        /// <returns>True if the span was validated, false if the span is null.</returns>
        /// <remarks>This method assumes the Id is validated to be unique amongst siblings outside this method.
        /// </remarks>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private bool TryValidateMarkedSpan(
                                           MarkedSpan span,
                                           Dictionary<string, ResourceStringContent> contentMap,
                                           Dictionary<string, ResourceStringContent> sourceContentMap,
                                           Dictionary<string, MarkedSpanEnd> markedSpanStartToFind,
                                           Dictionary<string, SpanningCodeEnd> spanningCodeStartToFind)
        {
            bool result;

            result = span != null;
            if (result)
            {
                // Id must not be null.
                if (string.IsNullOrWhiteSpace(span.Id))
                {
                    throw new ValidationException(
                                                  ValidationError.MarkedSpanIdNull,
                                                  Properties.Resources.StandardValidator_IdNotSpecified,
                                                  span.SelectorPath);
                }
                else
                {
                    this.ValidateNMTOKEN(
                                         span.Id,
                                         false,
                                         "Id",
                                         ValidationError.MarkedSpanIdNotNMToken,
                                         span.SelectorPath);
                }

                // Type is optional and defaults to 'generic' if null.
                if ((span.Type != null) &&
                    (span.Type != MarkedSpanTypes.Comment) &&
                    (span.Type != MarkedSpanTypes.Generic) &&
                    (span.Type != MarkedSpanTypes.Term) &&
                    !this.TryValidatePrefixValueFormat(span.Type, "Type", span.SelectorPath, false))
                {
                    throw new ValidationException(
                                                  ValidationError.MarkedSpanInvalidType,
                                                  Properties.Resources.StandardValidator_InvalidMarkedSpanType,
                                                  span.SelectorPath);
                }

                // If type is set to comment then the value must be set or the reference must be set, but not both.
                if ((span.Type == MarkedSpanTypes.Comment) && ((span.Value == null) == (span.Reference == null)))
                {
                    throw new ValidationException(
                                                  ValidationError.MarkedSpanReferenceAndValueSpecified,
                                                  Properties.Resources.StandardValidator_MarkedSpanReferenceAndValueSpecified,
                                                  span.SelectorPath);
                }

                if (span.Reference != null)
                {
                    if (span.Type == MarkedSpanTypes.Comment)
                    {
                        // The reference must refer to a valid note.
                        Note note;
                        Unit unit;

                        unit = span.FindAncestor<Unit>();
                        note = unit.Select(span.Reference) as Note;

                        // If the path is not a relative path to a note, check if it's a fully qualified path to a note
                        // within the same unit.
                        if (note == null)
                        {
                            XliffDocument document;

                            document = span.FindAncestor<XliffDocument>();
                            note = document.Select(span.Reference) as Note;
                            if (note != null)
                            {
                                Unit referenceUnit;

                                referenceUnit = note.FindAncestor<Unit>();
                                if (!object.ReferenceEquals(unit, referenceUnit))
                                {
                                    // The reference refers to a valid Note, but that note is not within the same unit so
                                    // is invalid.
                                    note = null;
                                }
                            }
                        }

                        if (note == null)
                        {
                            string message;

                            message = string.Format(
                                                    Properties.Resources.StandardValidator_InvalidMarkedSpanReference_Format,
                                                    span.Reference,
                                                    "Note");
                            throw new ValidationException(
                                                      ValidationError.MarkedSpanInvalidReference,
                                                      message,
                                                      span.SelectorPath);
                        }
                    }
                    else if (span.Reference.StartsWith(Utilities.Constants.SelectorPathIndictator) &&
                             (this.document.Select(span.Reference) == null))
                    {
                        // The reference may look like #/f=f1/u=u1/s1 or #/f=f1/u=u1/xyz=abc. If the path can be resolved
                        // without modification, then all is good. Otherwise strip off the trailing fragment and find
                        // the element without that fragment. Then validate that the fragment is well-formed. That
                        // fragment is likely a reference to an extension or module that isn't resolved because extensions
                        // aren't understood so selection can't be done with them.
                        string fragment;
                        string path;

                        path = Utilities.RemoveLastFragment(span.Reference, out fragment);
                        if (this.document.Select(path) == null)
                        {
                            string message;

                            message = string.Format(
                                                    Properties.Resources.StandardValidator_InvalidMarkedSpanReference_Format,
                                                    span.Reference,
                                                    "element");
                            throw new ValidationException(
                                                      ValidationError.MarkedSpanInvalidReference,
                                                      message,
                                                      span.SelectorPath);
                        }
                        else if (fragment != null)
                        {
                            // Assume the fragment refers to an extension or module so the prefix has restrictions.
                            fragment = Utilities.MakeRelativeSelector(fragment);
                            if (!Utilities.IsWellFormedSelectorPathExcludingResourceStrings(fragment, true))
                            {
                                // The reference must be a valid URI. If it looks like a selector path, then verify that it
                                // is a well-formed selector path.
                                string message;

                                message = string.Format(
                                                Properties.Resources.StandardValidator_InvalidMarkedSpanReferenceSelectorPath_Format,
                                                span.Reference);
                                throw new ValidationException(
                                                            ValidationError.MarkedSpanInvalidReferenceSelectorPath,
                                                            message,
                                                            span.SelectorPath);
                            }
                        }
                    }
                }

                this.ValidateResourceStringContentContainer(
                                                            span,
                                                            contentMap,
                                                            sourceContentMap,
                                                            markedSpanStartToFind,
                                                            spanningCodeStartToFind);
                this.ValidateFormatStyles(span, span.SelectorPath);
                this.ValidateSizeRestrictionModule_Attributes(span, span.SelectorPath);
            }

            return result;
        }

        /// <summary>
        /// Validates the <see cref="MarkedSpanEnd"/> conforms to the standard.
        /// </summary>
        /// <param name="span">The element to validate.</param>
        /// <param name="markedSpanStartToFind">The list of MarkedSpanStart elements to find during validation to
        /// ensure references refer to valid elements. The value is the MarkedSpanEnd element that refers to the
        /// MarkedSpanStart.</param>
        /// <returns>True if the span was validated, false if the span is null.</returns>
        /// <remarks>This method assumes the Id is validated to be unique amongst siblings outside this method.
        /// </remarks>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private bool TryValidateMarkedSpanEnd(MarkedSpanEnd span, Dictionary<string, MarkedSpanEnd> markedSpanStartToFind)
        {
            bool result;

            result = span != null;
            if (result)
            {
                if (string.IsNullOrWhiteSpace(span.StartReference))
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.StandardValidator_PropertyNotSpecified_Format,
                                            "StartReference");
                    throw new ValidationException(
                                                  ValidationError.MarkedSpanEndStartRefNull,
                                                  message,
                                                  span.SelectableAncestor.SelectorPath);
                }

                // StartReference refers to matching  MarkedSpanEndStart.
                if (span.StartReference != null)
                {
                    // StartReference refers to a valid MarkedSpanStart.Id and must be a valid NMTOKEN because the
                    // MarkedSpanStart.Id must also be a valid NMTOKEN, so there's no need to validate that here.
                    if (markedSpanStartToFind.ContainsKey(span.StartReference))
                    {
                        string message;

                        message = string.Format(
                                                Properties.Resources.StandardValidator_DuplicateReference_Format,
                                                "MarkedSpanStart",
                                                span.StartReference);
                        throw new ValidationException(
                                                      ValidationError.MarkedSpanEndDuplicateStartRef,
                                                      message,
                                                      span.SelectableAncestor.SelectorPath);
                    }

                    markedSpanStartToFind.Add(span.StartReference, span);
                }
            }

            return result;
        }

        /// <summary>
        /// Validates the <see cref="MarkedSpanStart"/> conforms to the standard.
        /// </summary>
        /// <param name="span">The element to validate.</param>
        /// <returns>True if the span was validated, false if the span is null.</returns>
        /// <remarks>This method assumes the Id is validated to be unique amongst siblings outside this method.
        /// </remarks>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private bool TryValidateMarkedSpanStart(MarkedSpanStart span)
        {
            bool result;

            result = span != null;
            if (result)
            {
                if (string.IsNullOrWhiteSpace(span.Id))
                {
                    throw new ValidationException(
                                                  ValidationError.MarkedSpanStartIdNull,
                                                  Properties.Resources.StandardValidator_IdNotSpecified,
                                                  span.SelectorPath);
                }
                else
                {
                    this.ValidateNMTOKEN(
                                         span.Id,
                                         false,
                                         "Id",
                                         ValidationError.MarkedSpanStartIdNotNMToken,
                                         span.SelectorPath);
                }

                // Type is optional and defaults to 'generic' if null.
                if ((span.Type != null) &&
                    (span.Type != MarkedSpanTypes.Comment) &&
                    (span.Type != MarkedSpanTypes.Generic) &&
                    (span.Type != MarkedSpanTypes.Term) &&
                    !this.TryValidatePrefixValueFormat(span.Type, "Type", span.SelectorPath, false))
                {
                    throw new ValidationException(
                                                  ValidationError.MarkedSpanStartInvalidType,
                                                  Properties.Resources.StandardValidator_InvalidMarkedSpanType,
                                                  span.SelectorPath);
                }

                // If type is set to comment then the value must be set or the reference must be set, but not both.
                if ((span.Type == MarkedSpanTypes.Comment) && ((span.Value == null) == (span.Reference == null)))
                {
                    throw new ValidationException(
                                                  ValidationError.MarkedSpanStartReferenceAndValueSpecified,
                                                  Properties.Resources.StandardValidator_MarkedSpanReferenceAndValueSpecified,
                                                  span.SelectorPath);
                }

                // The reference must refer to a valid note.
                if (span.Reference != null)
                {
                    if (span.Type == MarkedSpanTypes.Comment)
                    {
                        // The reference must refer to a valid note.
                        Note note;
                        Unit unit;

                        unit = span.FindAncestor<Unit>();
                        note = unit.Select(span.Reference) as Note;

                        // If the path is not a relative path to a note, check if it's a fully qualified path to a note
                        // within the same unit.
                        if (note == null)
                        {
                            XliffDocument document;

                            document = span.FindAncestor<XliffDocument>();
                            note = document.Select(span.Reference) as Note;
                            if (note != null)
                            {
                                Unit referenceUnit;

                                referenceUnit = note.FindAncestor<Unit>();
                                if (!object.ReferenceEquals(unit, referenceUnit))
                                {
                                    // The reference refers to a valid Note, but that note is not within the same unit so
                                    // is invalid.
                                    note = null;
                                }
                            }
                        }

                        if (note == null)
                        {
                            string message;

                            message = string.Format(
                                                    Properties.Resources.StandardValidator_InvalidMarkedSpanReference_Format,
                                                    span.Reference,
                                                    "Note");
                            throw new ValidationException(
                                                      ValidationError.MarkedSpanStartInvalidReference,
                                                      message,
                                                      span.SelectorPath);
                        }
                    }
                    else if (span.Reference.StartsWith(Utilities.Constants.SelectorPathIndictator) &&
                             (this.document.Select(span.Reference) == null))
                    {
                        // The reference may look like #/f=f1/u=u1/s1 or #/f=f1/u=u1/xyz=abc. If the path can be resolved
                        // without modification, then all is good. Otherwise strip off the trailing fragment and find
                        // the element without that fragment. Then validate that the fragment is well-formed. That
                        // fragment is likely a reference to an extension or module that isn't resolved because extensions
                        // aren't understood so selection can't be done with them.
                        string fragment;
                        string path;

                        path = Utilities.RemoveLastFragment(span.Reference, out fragment);
                        if (this.document.Select(path) == null)
                        {
                            string message;

                            message = string.Format(
                                                    Properties.Resources.StandardValidator_InvalidMarkedSpanReference_Format,
                                                    span.Reference,
                                                    "element");
                            throw new ValidationException(
                                                      ValidationError.MarkedSpanStartInvalidReference,
                                                      message,
                                                      span.SelectorPath);
                        }
                        else if (fragment != null)
                        {
                            // Assume the fragment refers to an extension or module so the prefix has restrictions.
                            fragment = Utilities.MakeRelativeSelector(fragment);
                            if (!Utilities.IsWellFormedSelectorPathExcludingResourceStrings(fragment, true))
                            {
                                // The reference must be a valid URI. If it looks like a selector path, then verify that it
                                // is a well-formed selector path.
                                string message;

                                message = string.Format(
                                                Properties.Resources.StandardValidator_InvalidMarkedSpanReferenceSelectorPath_Format,
                                                span.Reference);
                                throw new ValidationException(
                                                            ValidationError.MarkedSpanStartInvalidReferenceSelectorPath,
                                                            message,
                                                            span.SelectorPath);
                            }
                        }
                    }
                }

                this.ValidateFormatStyles(span, span.SelectorPath);
                this.ValidateSizeRestrictionModule_Attributes(span, span.SelectorPath);
            }

            return result;
        }

        /// <summary>
        /// Validates that a string matches the pattern of "prefix:value" where the prefix must not match any reserved
        /// values and that prefix and value are not empty strings.
        /// </summary>
        /// <param name="text">The text to validate.</param>
        /// <param name="name">The name of the text to validate.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <param name="throwOnError">If true, then an <see cref="ValidationException"/> is thrown on validation
        /// errors, if false then false is returned on error, otherwise true is returned.</param>
        /// <returns>True if no validate errors occurred, otherwise false.</returns>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs and
        /// <paramref name="throwOnError"/> is true.</exception>
        private bool TryValidatePrefixValueFormat(
                                                  string text,
                                                  string name,
                                                  string selectorPath,
                                                  bool throwOnError)
        {
            string prefix;
            string value;

            return this.TryValidatePrefixValueFormat(
                                                     text,
                                                     name,
                                                     selectorPath,
                                                     false,
                                                     throwOnError,
                                                     out prefix,
                                                     out value);
        }

        /// <summary>
        /// Validates that a string matches the pattern of "prefix:value" where the prefix must not match any reserved
        /// values and that prefix and value are not empty strings.
        /// </summary>
        /// <param name="text">The text to validate.</param>
        /// <param name="name">The name of the text to validate.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <param name="allowXliffPrefix">If true, then the XLIFF prefix (XLF) is supported as a valid prefix.</param>
        /// <param name="throwOnError">If true, then an <see cref="ValidationException"/> is thrown on validation
        /// errors, if false then false is returned on error, otherwise true is returned.</param>
        /// <param name="prefix">On output, the value of the prefix in the format "prefix:value".</param>
        /// <param name="value">On output, the value of the value in the format "prefix:value".</param>
        /// <returns>True if no validate errors occurred, otherwise false.</returns>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs and
        /// <paramref name="throwOnError"/> is true.</exception>
        private bool TryValidatePrefixValueFormat(
                                                  string text,
                                                  string name,
                                                  string selectorPath,
                                                  bool allowXliffPrefix,
                                                  bool throwOnError,
                                                  out string prefix,
                                                  out string value)
        {
            int index;
            int errorNumber;
            string message;

            errorNumber = 0;
            message = null;
            prefix = null;
            value = null;

            index = text.IndexOf(':');
            if (index < 1)
            {
                errorNumber = ValidationError.PrefixValueMissingColon;
                message = string.Format(Properties.Resources.StandardValidator_InvalidPrefix_Format, name);
            }
            else
            {
                prefix = text.Substring(0, index);
                if (string.IsNullOrWhiteSpace(prefix) ||
                    (!allowXliffPrefix &&
                     string.Equals(prefix, SubTypeValues.XliffPrefix, StringComparison.OrdinalIgnoreCase)))
                {
                    errorNumber = ValidationError.PrefixValueInvalid;
                    message = string.Format(Properties.Resources.StandardValidator_InvalidPrefix_Format, name);
                }

                value = text.Substring(index + 1);
                if (string.IsNullOrWhiteSpace(value))
                {
                    errorNumber = ValidationError.PrefixValueEmpty;
                    message = string.Format(Properties.Resources.StandardValidator_InvalidPrefix_Format, name);
                }
            }

            if (throwOnError && (message != null))
            {
                throw new ValidationException(errorNumber, message, selectorPath);
            }

            return message == null;
        }

        /// <summary>
        /// Validates the <see cref="SpanningCode"/> conforms to the standard.
        /// </summary>
        /// <param name="span">The element to validate.</param>
        /// <param name="contentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="ResourceString"/>.</param>
        /// <param name="sourceContentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="Source"/>. This may be null if the <paramref name="contentMap"/> refers to the Source elements
        /// as well.</param>
        /// <param name="markedSpanStartToFind">The list of MarkedSpanStart elements to find during validation to
        /// ensure references refer to valid elements. The value is the MarkedSpanEnd element that refers to the
        /// MarkedSpanStart.</param>
        /// <param name="spanningCodeStartToFind">The list of SpanningCodeStart elements to find during validation to
        /// ensure references refer to valid elements. The value is the SpanningCodeEnd element that refers to the
        /// SpanningCodeStart.</param>
        /// <returns>True if the span was validated, false if the span is null.</returns>
        /// <remarks>This method assumes the Id is validated to be unique amongst siblings outside this method.
        /// </remarks>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private bool TryValidateSpanningCode(
                                             SpanningCode span,
                                             Dictionary<string, ResourceStringContent> contentMap,
                                             Dictionary<string, ResourceStringContent> sourceContentMap,
                                             Dictionary<string, MarkedSpanEnd> markedSpanStartToFind,
                                             Dictionary<string, SpanningCodeEnd> spanningCodeStartToFind)
        {
            bool result;

            result = span != null;
            if (result)
            {
                OriginalData originalData;
                int errorNumber;
                string message;

                originalData = Utilities.GetOriginalData(span);

                errorNumber = 0;
                message = null;

                // DataReferenceEnd and DataReferenceStart refer to Data elements in the OriginalData. Both are optional.
                // If either one refers to a Data element then it must be a valid NMTOKEN because the Data.Id must also
                // be a valid NMTOKEN, so there's no need to validate that here.
                if ((span.DataReferenceEnd != null) &&
                    (null == StandardValidator.FindDataElement(originalData, span.DataReferenceEnd)))
                {
                    errorNumber = ValidationError.SpanningCodeInvalidDataRefEnd;
                    message = string.Format(
                                            Properties.Resources.StandardValidator_InvalidDataReference_Format,
                                            "DataReferenceEnd",
                                            span.DataReferenceEnd);
                }

                if ((message == null) &&
                    (span.DataReferenceStart != null) &&
                    (null == StandardValidator.FindDataElement(originalData, span.DataReferenceStart)))
                {
                    errorNumber = ValidationError.SpanningCodeInvalidDataRefStart;
                    message = string.Format(
                                            Properties.Resources.StandardValidator_InvalidDataReference_Format,
                                            "DataReferenceStart",
                                            span.DataReferenceStart);
                }

                if (message != null)
                {
                    throw new ValidationException(errorNumber, message, span.SelectorPath);
                }

                // SubFlowsEnd and SubFlowsStart refer to units within the file.
                if ((span.SubFlowsEnd != null) || (span.SubFlowsStart != null))
                {
                    File file;
                    Dictionary<string, Unit> units;

                    file = StandardValidator.FindAncestor<File>(span);
                    Debug.Assert(file != null, "Unable to locate File.");

                    units = this.GetTranslationContainers<Unit>(file);

                    // SubFlowsStart and SubFlowsEnd refer to a valid Unit.Id and must be a valid NMTOKEN because the
                    // Unit.Id must also be a valid NMTOKEN, so there's no need to validate that here.
                    if (span.SubFlowsEnd != null)
                    {
                        foreach (string id in span.SubFlowsEnd.Split(new char[] { ' ' }))
                        {
                            if (!units.ContainsKey(id))
                            {
                                message = string.Format(
                                                        Properties.Resources.StandardValidator_SubflowNotFound_Format,
                                                        "SubFlowsEnd",
                                                        span.SubFlowsEnd);
                                throw new ValidationException(
                                                              ValidationError.SpanningCodeInvalidSubFlowsEnd,
                                                              message,
                                                              span.SelectorPath);
                            }
                        }
                    }

                    if (span.SubFlowsStart != null)
                    {
                        foreach (string id in span.SubFlowsStart.Split(new char[] { ' ' }))
                        {
                            if (!units.ContainsKey(id))
                            {
                                message = string.Format(
                                                        Properties.Resources.StandardValidator_SubflowNotFound_Format,
                                                        "SubFlowsStart",
                                                        span.SubFlowsStart);
                                throw new ValidationException(
                                                              ValidationError.SpanningCodeInvalidSubFlowsStart,
                                                              message,
                                                              span.SelectorPath);
                            }
                        }
                    }
                }

                this.ValidateCodeBase(span, true, contentMap, sourceContentMap);
                this.ValidateResourceStringContentContainer(
                                                            span,
                                                            contentMap,
                                                            sourceContentMap,
                                                            markedSpanStartToFind,
                                                            spanningCodeStartToFind);
            }

            return result;
        }

        /// <summary>
        /// Validates the <see cref="SpanningCodeEnd"/> conforms to the standard.
        /// </summary>
        /// <param name="span">The element to validate.</param>
        /// <param name="contentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="ResourceString"/>.</param>
        /// <param name="sourceContentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="Source"/>. This may be null if the <paramref name="contentMap"/> refers to the Source elements
        /// as well.</param>
        /// <param name="spanningCodeStartToFind">The list of SpanningCodeStart elements to find during validation to
        /// ensure references refer to valid elements. The value is the SpanningCodeEnd element that refers to the
        /// SpanningCodeStart.</param>
        /// <returns>True if the span was validated, false if the span is null.</returns>
        /// <remarks>This method assumes the Id is validated to be unique amongst siblings outside this method.
        /// </remarks>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private bool TryValidateSpanningCodeEnd(
                                                SpanningCodeEnd span,
                                                Dictionary<string, ResourceStringContent> contentMap,
                                                Dictionary<string, ResourceStringContent> sourceContentMap,
                                                Dictionary<string, SpanningCodeEnd> spanningCodeStartToFind)
        {
            bool result;

            result = span != null;
            if (result)
            {
                OriginalData originalData;

                this.ValidateCodeBase(span, false, contentMap, sourceContentMap);

                originalData = Utilities.GetOriginalData(span);

                // DataReference refers to Data elements in the OriginalData.
                // If DataReference refers to a Data element then it must be a valid NMTOKEN because the Data.Id must also
                // be a valid NMTOKEN, so there's no need to validate that here.
                if ((span.DataReference != null) &&
                    (null == StandardValidator.FindDataElement(originalData, span.DataReference)))
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.StandardValidator_InvalidDataReference_Format,
                                            "DataReference",
                                            span.DataReference);
                    throw new ValidationException(
                                                  ValidationError.SpanningCodeEndInvalidDataRef,
                                                  message,
                                                  span.SelectorPath);
                }

                // SubFlows refer to units within the file.
                if (span.SubFlows != null)
                {
                    File file;
                    Dictionary<string, Unit> units;

                    file = StandardValidator.FindAncestor<File>(span);
                    Debug.Assert(file != null, "Unable to locate File.");

                    units = this.GetTranslationContainers<Unit>(file);

                    foreach (string id in span.SubFlows.Split(new char[] { ' ' }))
                    {
                        // SubFlows refers to a valid Unit.Id and must be a valid NMTOKEN because the
                        // Unit.Id must also be a valid NMTOKEN, so there's no need to validate that here.
                        if (!units.ContainsKey(id))
                        {
                            string message;

                            message = string.Format(
                                                    Properties.Resources.StandardValidator_SubflowNotFound_Format,
                                                    "SubFlows",
                                                    span.SubFlows);
                            throw new ValidationException(
                                                          ValidationError.SpanningCodeEndSubFlowsInvalid,
                                                          message,
                                                          span.SelectorPath);
                        }
                    }
                }

                // StartReference refers to matching  SpanningCodeStart.
                if (span.StartReference != null)
                {
                    // StartReference refers to a valid SpanningCode.Id and must be a valid NMTOKEN because the
                    // SpanningCodeStart.Id must also be a valid NMTOKEN, so there's no need to validate that here.
                    if (spanningCodeStartToFind.ContainsKey(span.StartReference))
                    {
                        string message;

                        message = string.Format(
                                                Properties.Resources.StandardValidator_DuplicateReference_Format,
                                                "SpanningCodeStart",
                                                span.StartReference);
                        throw new ValidationException(
                                                      ValidationError.SpanningCodeEndStartRefInvalid,
                                                      message,
                                                      span.SelectorPath);
                    }

                    if (span.Isolated)
                    {
                        throw new ValidationException(
                                                      ValidationError.SpanningCodeEndIsolatedWithStartRef,
                                                      Properties.Resources.StandardValidator_SpanningCodeEndIslatedAndStartRef,
                                                      span.SelectorPath);
                    }

                    if (!string.IsNullOrWhiteSpace(span.Id))
                    {
                        // Id cannot be used because StartReference is specified.
                        throw new ValidationException(
                                                      ValidationError.SpanningCodeEndStartRefAndIdSpecified,
                                                      Properties.Resources.StandardValidator_SpanningCodeEndStartRefAndId,
                                                      span.SelectorPath);
                    }

                    spanningCodeStartToFind.Add(span.StartReference, span);
                }
                else if (!span.Isolated)
                {
                    throw new ValidationException(
                                                  ValidationError.SpanningCodeEndNotIsolatedOrStartRef,
                                                  Properties.Resources.StandardValidator_SpanningCodeEndNotIslatedOrStartRef,
                                                  span.SelectorPath);
                }
                else if (string.IsNullOrWhiteSpace(span.Id))
                {
                    // Id must not be null.
                    throw new ValidationException(
                                                  ValidationError.SpanningCodeEndIdNull,
                                                  Properties.Resources.StandardValidator_IdNotSpecified,
                                                  span.SelectorPath);
                }
            }

            return result;
        }

        /// <summary>
        /// Validates the <see cref="SpanningCodeStart"/> conforms to the standard.
        /// </summary>
        /// <param name="span">The element to validate.</param>
        /// <param name="contentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="ResourceString"/>.</param>
        /// <param name="sourceContentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="Source"/>. This may be null if the <paramref name="contentMap"/> refers to the Source elements
        /// as well.</param>
        /// <returns>True if the span was validated, false if the span is null.</returns>
        /// <remarks>This method assumes the Id is validated to be unique amongst siblings outside this method.
        /// </remarks>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private bool TryValidateSpanningCodeStart(
                                                  SpanningCodeStart span,
                                                  Dictionary<string, ResourceStringContent> contentMap,
                                                  Dictionary<string, ResourceStringContent> sourceContentMap)
        {
            bool result;

            result = span != null;
            if (result)
            {
                Unit unit;

                this.ValidateCodeBase(span, true, contentMap, sourceContentMap);

                unit = StandardValidator.FindAncestor<Unit>(span);
                Debug.Assert(unit != null, "Did not find Unit.");

                // DataReference refers to Data elements in the OriginalData.
                // If DataReference refers to a Data element then it must be a valid NMTOKEN because the Data.Id must also
                // be a valid NMTOKEN, so there's no need to validate that here.
                if ((span.DataReference != null) &&
                    (null == StandardValidator.FindDataElement(unit.OriginalData, span.DataReference)))
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.StandardValidator_InvalidDataReference_Format,
                                            "DataReference",
                                            span.DataReference);
                    throw new ValidationException(
                                                  ValidationError.SpanningCodeStartDataRefInvalid,
                                                  message,
                                                  span.SelectorPath);
                }

                // SubFlows refer to units within the file.
                if (span.SubFlows != null)
                {
                    File file;
                    Dictionary<string, Unit> units;

                    file = StandardValidator.FindAncestor<File>(span);
                    Debug.Assert(file != null, "Unable to locate File.");

                    units = this.GetTranslationContainers<Unit>(file);

                    foreach (string id in span.SubFlows.Split(new char[] { ' ' }))
                    {
                        // SubFlows refers to a valid Unit.Id and must be a valid NMTOKEN because the
                        // Unit.Id must also be a valid NMTOKEN, so there's no need to validate that here.
                        if (!units.ContainsKey(id))
                        {
                            string message;

                            message = string.Format(
                                                    Properties.Resources.StandardValidator_SubflowNotFound_Format,
                                                    "SubFlows",
                                                    span.SubFlows);
                            throw new ValidationException(
                                                          ValidationError.SpanningCodeStartSubflowsInvalid,
                                                          message,
                                                          span.SelectorPath);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Validates the <see cref="StandaloneCode"/> conforms to the standard.
        /// </summary>
        /// <param name="code">The element to validate.</param>
        /// <param name="contentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="ResourceString"/>.</param>
        /// <param name="sourceContentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="Source"/>. This may be null if the <paramref name="contentMap"/> refers to the Source elements
        /// as well.</param>
        /// <returns>True if the span was validated, false if the span is null.</returns>
        /// <remarks>This method assumes the Id is validated to be unique amongst siblings outside this method.
        /// </remarks>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private bool TryValidateStandaloneCode(
                                               StandaloneCode code,
                                               Dictionary<string, ResourceStringContent> contentMap,
                                               Dictionary<string, ResourceStringContent> sourceContentMap)
        {
            bool result;

            result = code != null;
            if (result)
            {
                OriginalData originalData;

                originalData = Utilities.GetOriginalData(code);

                // DataReference refers to Data elements in the OriginalData.
                // If DataReference refers to a Data element then it must be a valid NMTOKEN because the Data.Id must also
                // be a valid NMTOKEN, so there's no need to validate that here.
                if ((code.DataReference != null) &&
                    (null == StandardValidator.FindDataElement(originalData, code.DataReference)))
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.StandardValidator_InvalidDataReference_Format,
                                            "DataReference",
                                            code.DataReference);
                    throw new ValidationException(
                                                  ValidationError.StandaloneCodeDataRefInvalid,
                                                  message,
                                                  code.SelectorPath);
                }

                if (code.SubFlows != null)
                {
                    File file;
                    Dictionary<string, Unit> units;

                    file = StandardValidator.FindAncestor<File>(code);
                    Debug.Assert(file != null, "Unable to locate File.");

                    units = this.GetTranslationContainers<Unit>(file);

                    foreach (string id in code.SubFlows.Split(new char[] { ' ' }))
                    {
                        // SubFlows refers to a valid Unit.Id and must be a valid NMTOKEN because the
                        // Unit.Id must also be a valid NMTOKEN, so there's no need to validate that here.
                        if (!units.ContainsKey(id))
                        {
                            string message;

                            message = string.Format(
                                                    Properties.Resources.StandardValidator_SubflowNotFound_Format,
                                                    "SubFlows",
                                                    code.SubFlows);
                            throw new ValidationException(
                                                          ValidationError.StandaloneCodeSubflowsInvalid,
                                                          message,
                                                          code.SelectorPath);
                        }
                    }
                }

                this.ValidateCodeBase(code, true, contentMap, sourceContentMap);
            }

            return result;
        }

        /// <summary>
        /// Validates that the specified language conforms to the BCP-47 standard. If not, a ValidationException is
        /// thrown.
        /// </summary>
        /// <param name="language">The language to validate.</param>
        /// <param name="allowNull">True if the language is allowed to be null. False if the language cannot be null.
        /// </param>
        /// <param name="property">The name of the property being validated.</param>
        /// <param name="errorNumber">A number that identifies the error if there is one.</param>
        /// <param name="selectorPath">The selector path to the element being validated.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateBcp47Language(
                                           string language,
                                           bool allowNull,
                                           string property,
                                           int errorNumber,
                                           string selectorPath)
        {
            if ((!allowNull || (language != null)) && !Utilities.IsValidBcp47Language(language))
            {
                string message;

                message = string.Format(Properties.Resources.StandardValidator_LanguageInvalid_Format, property);
                throw new ValidationException(errorNumber, message, selectorPath);
            }
        }

        /// <summary>
        /// Validates the ChangeTrack element from the ChangeTracking module conforms to the standard.
        /// </summary>
        /// <param name="change">The ChangeTrack to validate. If this value is null, no validation takes place.
        /// </param>
        /// <param name="selectorPath">The selector path of the element being validated.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateChangeTrackingModule_ChangeTrack(ChangeTrack change, string selectorPath)
        {
            if (change != null)
            {
                if (change.Revisions.Count < 1)
                {
                    string message;

                    message = string.Format(Properties.Resources.StandardValidator_NoElements_Format, "Revisions");
                    throw new ValidationException(ValidationError.ChangeTrackMissingRevisions, message, selectorPath);
                }

                foreach (RevisionsContainer container in change.Revisions)
                {
                    this.ValidateChangeTrackingModule_RevisionsContainer(container, selectorPath);
                }
            }
        }

        /// <summary>
        /// Validates the Revision element from the ChangeTracking module conforms to the standard.
        /// </summary>
        /// <param name="revision">The Revision to validate.</param>
        /// <param name="selectorPath">The selector path of the element being validated.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateChangeTrackingModule_Revision(Revision revision, string selectorPath)
        {
            if (revision.Items.Count < 1)
            {
                string message;

                message = string.Format(Properties.Resources.StandardValidator_NoElements_Format, "Items");
                throw new ValidationException(ValidationError.RevisionMissingItems, message, selectorPath);
            }

            this.ValidateNMTOKEN(revision.Version, true, "Version", ValidationError.RevisionVersionNotNMToken, selectorPath);

            foreach (Item item in revision.Items)
            {
                if (string.IsNullOrWhiteSpace(item.Property))
                {
                    throw new ValidationException(
                                                  ValidationError.ItemPropertyNull,
                                                  Properties.Resources.StandardValidator_ItemPropertyNull,
                                                  selectorPath);
                }
            }
        }

        /// <summary>
        /// Validates the RevisionsContainer element from the ChangeTracking module conforms to the standard.
        /// </summary>
        /// <param name="container">The RevisionsContainer to validate.</param>
        /// <param name="selectorPath">The selector path of the element being validated.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateChangeTrackingModule_RevisionsContainer(RevisionsContainer container, string selectorPath)
        {
            if (container.Revisions.Count < 1)
            {
                string message;

                message = string.Format(Properties.Resources.StandardValidator_NoElements_Format, "Revisions");
                throw new ValidationException(ValidationError.RevisionsContainerMissingRevisions, message, selectorPath);
            }

            this.ValidateNMTOKEN(
                                 container.AppliesTo,
                                 false,
                                 "AppliesTo",
                                 ValidationError.RevisionsContainerAppliesToNotNMToken,
                                 selectorPath);

            this.ValidateNMTOKEN(
                                 container.Reference,
                                 true,
                                 "Reference",
                                 ValidationError.RevisionsContainerReferenceNotNMToken,
                                 selectorPath);

            this.ValidateNMTOKEN(
                                 container.CurrentVersion,
                                 true,
                                 "CurrentVersion",
                                 ValidationError.RevisionsContainerCurrentVersionNotNMToken,
                                 selectorPath);

            foreach (Revision revision in container.Revisions)
            {
                this.ValidateChangeTrackingModule_Revision(revision, selectorPath);
            }
        }

        /// <summary>
        /// Validates the <see cref="CodeBase"/> conforms to the standard.
        /// </summary>
        /// <param name="codeBase">The element to validate.</param>
        /// <param name="validateId">True to validate that the Id is present and valid. False if the Id can be anything
        /// other than whitespace.</param>
        /// <param name="contentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="ResourceString"/>.</param>
        /// <param name="sourceContentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="Source"/>. This may be null if the <paramref name="contentMap"/> refers to the Source elements
        /// as well.</param>
        /// <remarks>This method assumes the Id is validated to be unique amongst siblings outside this method.
        /// </remarks>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateCodeBase(
                                      CodeBase codeBase,
                                      bool validateId,
                                      Dictionary<string, ResourceStringContent> contentMap,
                                      Dictionary<string, ResourceStringContent> sourceContentMap)
        {
            if (validateId || (codeBase.Id != null))
            {
                if (string.IsNullOrWhiteSpace(codeBase.Id))
                {
                    throw new ValidationException(
                                                  ValidationError.CodeBaseIdNull,
                                                  Properties.Resources.StandardValidator_IdNotSpecified,
                                                  codeBase.SelectorPath);
                }
                else
                {
                    this.ValidateNMTOKEN(
                                         codeBase.Id,
                                         false,
                                         "Id",
                                         ValidationError.CodeBaseIdNotNMToken,
                                         codeBase.SelectorPath);
                }
            }

            // CopyOf refers to a valid ResourceStringContent.Id and must be a valid NMTOKEN because the
            // ResourceStringContent.Id must also be a valid NMTOKEN, so there's no need to validate that here.
            if (codeBase.CopyOf != null)
            {
                ResourceStringContent element;

                // CopyOf and DataReference (DataReferenceStart, DataReferenceEnd) cannot both be present.
                if (codeBase.SupportsDataReferences && codeBase.AllDataReferences.Count > 0)
                {
                    bool hasDataRef;

                    hasDataRef = false;
                    foreach (KeyValuePair<string, string> dataRef in codeBase.AllDataReferences)
                    {
                        if (dataRef.Value != null)
                        {
                            hasDataRef = true;
                            break;
                        }
                    }

                    if (hasDataRef)
                    {
                        string message;

                        message = string.Format(
                                                Properties.Resources.StandardValidator_CodeBaseWithCopyOfAndDataRef_Format,
                                                codeBase.GetType().Name);
                        throw new ValidationException(
                                                        ValidationError.CodeBaseWithCopyOfAndDataRef,
                                                        message,
                                                        codeBase.SelectorPath);
                    }
                }

                if (!contentMap.TryGetValue(codeBase.CopyOf, out element) &&
                    (sourceContentMap != null) &&
                    !sourceContentMap.TryGetValue(codeBase.CopyOf, out element))
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.StandardValidator_CopyOfNotFound_Format,
                                            codeBase.GetType().Name);
                    throw new ValidationException(
                                                  ValidationError.CodeBaseInvalidCopyOf,
                                                  message,
                                                  codeBase.SelectorPath);
                }
                else if ((element == null) || (element.GetType() != codeBase.GetType()))
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.StandardValidator_InvalidCopyOfElement_Format,
                                            codeBase.GetType().Name,
                                            codeBase.CopyOf);
                    throw new ValidationException(
                                                  ValidationError.CodeBaseCopyOfTypeMismatch,
                                                  message,
                                                  codeBase.SelectorPath);
                }
            }

            if (codeBase.SubType != null)
            {
                string prefix;
                string value;

                if (!codeBase.Type.HasValue)
                {
                    throw new ValidationException(
                                                  ValidationError.CodeBaseTypeNotSpecified,
                                                  Properties.Resources.StandardValidator_CodeBaseTypeNotSpecified,
                                                  codeBase.SelectorPath);
                }

                this.TryValidatePrefixValueFormat(
                                                  codeBase.SubType,
                                                  "SubType",
                                                  codeBase.SelectorPath,
                                                  true,
                                                  false,
                                                  out prefix,
                                                  out value);

                if (string.Equals(
                                  prefix,
                                  SubTypeValues.XliffPrefix,
                                  StringComparison.OrdinalIgnoreCase))
                {
                    int errorNumber;
                    string message;

                    errorNumber = 0;
                    message = null;
                    if ((value == SubTypeValues.XliffSubTypeLineBreak) ||
                        (value == SubTypeValues.XliffSubTypePageBreak) ||
                        (value == SubTypeValues.XliffSubTypeBold) ||
                        (value == SubTypeValues.XliffSubTypeItalic) ||
                        (value == SubTypeValues.XliffSubTypeUnderline))
                    {
                        if (codeBase.Type != CodeType.Formatting)
                        {
                            errorNumber = ValidationError.CodeBaseSubTypeMismatchFormatting;
                            message = Properties.Resources.StandardValidator_XliffSubTypeMismatchFormatting;
                        }
                    }
                    else if (value == SubTypeValues.XliffSubTypeVariable)
                    {
                        if (codeBase.Type != CodeType.UserInterface)
                        {
                            errorNumber = ValidationError.CodeBaseSubTypeMismatchUserInterface;
                            message = Properties.Resources.StandardValidator_XliffSubTypeMismatchUserInterface;
                        }
                    }
                    else
                    {
                        errorNumber = ValidationError.CodeBaseSubTypeInvalid;
                        message = Properties.Resources.StandardValidator_InvalidXliffSubTypeValue;
                    }

                    if (message != null)
                    {
                        throw new ValidationException(errorNumber, message, codeBase.SelectorPath);
                    }
                }
            }

            this.ValidateFormatStyles(codeBase, codeBase.SelectorPath);
            this.ValidateSizeRestrictionModule_Attributes(codeBase, codeBase.SelectorPath);
        }

        /// <summary>
        /// Validates the <see cref="ContainerResource"/> conforms to the standard.
        /// </summary>
        /// <param name="container">The container to validate.</param>
        /// <remarks>This method assumes the Id is validated to be unique amongst siblings outside this method.
        /// </remarks>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateContainerResource(ContainerResource container)
        {
            Dictionary<string, ResourceStringContent> sourceTagIds;
            Dictionary<string, ResourceStringContent> targetTagIds;

            Debug.Assert(container != null, "container should not be null.");

            this.ValidateNMTOKEN(
                                 container.Id,
                                 true,
                                 "Id",
                                 ValidationError.ContainerResourceIdNotNMToken,
                                 container.SelectorPath);

            // Validate only Segments because there is nothing to validate for Ignorable.
            this.ValidateSegment(container as Segment);

            sourceTagIds = StandardValidator.GetSelectableResourceStringContents(container.Source);
            this.ValidateSource(container.Source, container.SelectorPath, sourceTagIds);
            targetTagIds = StandardValidator.GetSelectableResourceStringContents(container.Target);
            this.ValidateTarget(container.Target, true, targetTagIds, sourceTagIds);
            this.ValidateEditingHints(container.Source, container.Target, container.SelectorPath);

            // Validate that target inline tags have equivalent types as the source inline tags if the Ids match.
            if (container.Target != null)
            {
                // The value of Space must be the same between the source and target.
                if (container.Source.Space != container.Target.Space)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.StandardValidator_StartEndPropertyMismatch_Format,
                                            "Space",
                                            typeof(Source).Name,
                                            typeof(Target).Name);
                    throw new ValidationException(
                                                  ValidationError.ResourceStringSpaceMismatch,
                                                  message,
                                                  container.SelectorPath);
                }

                foreach (KeyValuePair<string, ResourceStringContent> child in targetTagIds)
                {
                    ResourceStringContent sourceElement;

                    // The source and target types must match.
                    //
                    // Target Type             Matching Source Type(s)
                    // ---------------------------------------------------------
                    // MarkedSpan           -> MarkedSpan, MarkedSpanStart
                    // MarkedSpanStart      -> MarkedSpan, MarkedSpanStart
                    // SpanningCode         -> SpanningCode, SpanningCodeStart
                    // SpanningCodeStart    -> SpanningCode, SpanningCodeStart
                    // StandaloneCode       -> StandaloneCode
                    if (sourceTagIds.TryGetValue(child.Key, out sourceElement))
                    {
                        bool isMatch;
                        ResourceStringContent targetElement;

                        isMatch = false;
                        targetElement = child.Value;
                        if (sourceElement.GetType() == targetElement.GetType())
                        {
                            isMatch = true;
                        }
                        else if ((sourceElement is MarkedSpan) || (sourceElement is MarkedSpanStart))
                        {
                            isMatch = (targetElement is MarkedSpan) || (targetElement is MarkedSpanStart);
                        }
                        else if ((sourceElement is SpanningCode) || (sourceElement is SpanningCodeStart))
                        {
                            isMatch = (targetElement is SpanningCode) || (targetElement is SpanningCodeStart);
                            if (isMatch)
                            {
                                // SpanningCodeStart tags must not be isolated otherwise they don't behave like
                                // SpanningCodes and aren't equivalent.
                                if ((sourceElement is SpanningCodeStart) && (targetElement is SpanningCode))
                                {
                                    isMatch = !((SpanningCodeStart)sourceElement).Isolated;
                                }
                                else if ((sourceElement is SpanningCode) && (targetElement is SpanningCodeStart))
                                {
                                    isMatch = !((SpanningCodeStart)targetElement).Isolated;
                                }
                            }
                        }

                        if (!isMatch)
                        {
                            string message;

                            message = string.Format(
                                                    Properties.Resources.StandardValidator_InlineTagsDontMatch_Format,
                                                    targetElement.GetType().Name,
                                                    ((ISelectable)targetElement).Id,
                                                    sourceElement.GetType().Name);
                            throw new ValidationException(
                                                          ValidationError.ContainerResourceTypesWithSameIdMismatch,
                                                          message,
                                                          container.SelectorPath);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates the <see cref="Data"/> element conforms to the standard.
        /// </summary>
        /// <param name="data">The <see cref="Data"/> to validate.</param>
        /// <remarks>This method assumes the Id is validated to be unique amongst siblings outside this method.
        /// </remarks>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateData(Data data)
        {
            Debug.Assert(data != null, "data should not be null.");

            // Id must not be null.
            if (string.IsNullOrWhiteSpace(data.Id))
            {
                throw new ValidationException(
                                              ValidationError.DataIdNull,
                                              Properties.Resources.StandardValidator_IdNotSpecified,
                                              data.SelectorPath);
            }
            else
            {
                this.ValidateNMTOKEN(data.Id, false, "Id", ValidationError.DataIdNotNMToken, data.SelectorPath);
            }

            // Space is restricted to Preserve.
            if (data.Space != Preservation.Preserve)
            {
                throw new ValidationException(
                                              ValidationError.DataSpaceNotPreserve,
                                              Properties.Resources.StandardValidator_Space_Not_Preserve,
                                              data.SelectorPath);
            }

            // Data can only contain CodePoint elements so just validate the code points.
            foreach (CodePoint point in data.CollapseChildren<CodePoint>())
            {
                this.TryValidateCodePoint(point);
            }
        }

        /// <summary>
        /// Validates editing hints on the <see cref="Source"/> and <see cref="Target"/> elements and throws a
        /// <see cref="ValidationException"/> if any validation errors occur.
        /// </summary>
        /// <param name="source">The <see cref="Source"/> that target elements must match.</param>
        /// <param name="target">The <see cref="Target"/> to validate. This value may be null.</param>
        /// <param name="selectorPath">The selector path to the <see cref="Segment"/> hosting the source and target.
        /// elements.</param>
        private void ValidateEditingHints(Source source, Target target, string selectorPath)
        {
            List<ResourceStringContent> sourceTags;
            List<ResourceStringContent> targetTags;
            Dictionary<string, ResourceStringContent> sourceTagsMap;
            Dictionary<string, ResourceStringContent> targetTagsMap;

            // Target may be null. Source may not.
            Debug.Assert(source != null, "Source is null.");

            sourceTags = StandardValidator.GetSelectableResourceStringContentsAsList(source);
            sourceTagsMap = StandardValidator.GetSelectableResourceStringContents(sourceTags);
            targetTags = StandardValidator.GetSelectableResourceStringContentsAsList(target);
            targetTagsMap = StandardValidator.GetSelectableResourceStringContents(targetTags);

            this.ValidateEditingHintsCanCopy(sourceTagsMap, targetTagsMap, selectorPath);
            this.ValidateEditingHintsCanDelete(sourceTagsMap, targetTagsMap, selectorPath);
            this.ValidateEditingHintsCanReorder(sourceTags, targetTags);
        }

        /// <summary>
        /// Validates CanCopy editing hints on the <see cref="Source"/> and <see cref="Target"/> elements and throws a
        /// <see cref="ValidationException"/> if any validation errors occur.
        /// </summary>
        /// <param name="sourceTagsMap">A dictionary of <see cref="ResourceStringContent"/> elements contained within
        /// the source element. The key is the Id of the element, and the value is the element itself.</param>
        /// <param name="targetTagsMap">A dictionary of <see cref="ResourceStringContent"/> elements contained within
        /// the target element. The key is the Id of the element, and the value is the element itself.</param>
        /// <param name="selectorPath">The selector path to the <see cref="Segment"/> hosting the source and target.
        /// elements.</param>
        private void ValidateEditingHintsCanCopy(
                                                 Dictionary<string, ResourceStringContent> sourceTagsMap,
                                                 Dictionary<string, ResourceStringContent> targetTagsMap,
                                                 string selectorPath)
        {
            // Validate canCopy by locating all references to inline tags via copyOf. If any references exist to
            // inline tags that have canCopy=no then fail validation.
            foreach (KeyValuePair<string, ResourceStringContent> pair in sourceTagsMap)
            {
                CodeBase codeBase;

                codeBase = pair.Value as CodeBase;
                if (codeBase != null)
                {
                    if (codeBase.CopyOf != null)
                    {
                        CodeBase copySourceCodeBase;
                        ResourceStringContent copySource;

                        copySource = sourceTagsMap[codeBase.CopyOf];
                        Debug.Assert(
                                     copySource != null,
                                     string.Format("Failed to find copyOf source element.", codeBase.CopyOf));
                        copySourceCodeBase = copySource as CodeBase;
                        if ((codeBase.GetType() != copySource.GetType()) || !copySourceCodeBase.CanCopy)
                        {
                            string message;

                            message = string.Format(
                                                    Properties.Resources.StandardValidator_CanCopyFailure_Format,
                                                    codeBase.GetType().Name,
                                                    codeBase.Id);
                            throw new ValidationException(
                                                    ValidationError.CodeBaseSourceCopyOfTypeMismatchOrNotCanCopy,
                                                    message,
                                                    selectorPath);
                        }
                    }
                }
            }

            // targetTagsMap may be null when validating the segment without a target.
            if (targetTagsMap != null)
            {
                // Validate canCopy on the target by locating all references to inline tags via copyOf in the source or
                // target. If any references exist to inline tags that have canCopy=no then fail validation.
                foreach (KeyValuePair<string, ResourceStringContent> pair in targetTagsMap)
                {
                    CodeBase codeBase;

                    codeBase = pair.Value as CodeBase;
                    if (codeBase != null)
                    {
                        if (codeBase.CopyOf != null)
                        {
                            CodeBase copySourceCodeBase;
                            ResourceStringContent copySource;

                            // Find the copy source from the source first. If not there, find it in the target (copy source
                            // is a brand new tag).
                            if (!sourceTagsMap.TryGetValue(codeBase.CopyOf, out copySource))
                            {
                                copySource = targetTagsMap[codeBase.CopyOf];
                            }

                            Debug.Assert(
                                        copySource != null,
                                        string.Format("Failed to find copyOf source element.", codeBase.CopyOf));
                            copySourceCodeBase = copySource as CodeBase;
                            if ((codeBase.GetType() != copySource.GetType()) || !copySourceCodeBase.CanCopy)
                            {
                                string message;

                                message = string.Format(
                                                        Properties.Resources.StandardValidator_CanCopyFailure_Format,
                                                        codeBase.GetType().Name,
                                                        codeBase.Id);
                                throw new ValidationException(
                                                    ValidationError.CodeBaseTargetCopyOfTypeMismatchOrNotCanCopy,
                                                    message,
                                                    selectorPath);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates CanDelete editing hints on the <see cref="Source"/> and <see cref="Target"/> elements and throws a
        /// <see cref="ValidationException"/> if any validation errors occur.
        /// </summary>
        /// <param name="sourceTagsMap">A dictionary of <see cref="ResourceStringContent"/> elements contained within
        /// the source element. The key is the Id of the element, and the value is the element itself.</param>
        /// <param name="targetTagsMap">A dictionary of <see cref="ResourceStringContent"/> elements contained within
        /// the target element. The key is the Id of the element, and the value is the element itself.</param>
        /// <param name="selectorPath">The selector path to the <see cref="Segment"/> hosting the source and target.
        /// elements.</param>
        private void ValidateEditingHintsCanDelete(
                                                   Dictionary<string, ResourceStringContent> sourceTagsMap,
                                                   Dictionary<string, ResourceStringContent> targetTagsMap,
                                                   string selectorPath)
        {
            // targetTagsMap may be null when validating the segment without a target.
            if (targetTagsMap != null)
            {
                List<string> convertedStartTags;

                convertedStartTags = new List<string>();

                // Validate canDelete by locating all inline tags in the source attributed with canDelete=no and fail
                // if they don't exist in the target.
                foreach (KeyValuePair<string, ResourceStringContent> pair in sourceTagsMap)
                {
                    CodeBase codeBase;

                    codeBase = pair.Value as CodeBase;
                    if (codeBase != null)
                    {
                        if (!codeBase.CanDelete)
                        {
                            bool foundTag;
                            ResourceStringContent tag;

                            foundTag = targetTagsMap.TryGetValue(pair.Key, out tag);
                            if (foundTag)
                            {
                                CodeBase tagCode;

                                // Treat start/end pairs as equivalent to spanning codes.
                                tagCode = tag as CodeBase;
                                foundTag = (tagCode != null) &&
                                           (tagCode.Id == codeBase.Id) &&
                                           ((tag.GetType() == codeBase.GetType()) ||
                                                ((tag is SpanningCode) && (codeBase is SpanningCodeStart)) ||
                                                ((tag is SpanningCodeStart) && (codeBase is SpanningCode)));

                                if (foundTag && (codeBase is SpanningCodeStart))
                                {
                                    // Save the Id of the start tag so the end tag doesn't raise an error. The end
                                    // is also replaced by the span.
                                    convertedStartTags.Add(codeBase.Id);
                                }
                            }
                            else
                            {
                                SpanningCodeEnd end;

                                // If the start/end pair got converted to a single span then the end tag won't be found.
                                end = codeBase as SpanningCodeEnd;
                                foundTag = (end != null) &&
                                           (end.StartReference != null) &&
                                           convertedStartTags.Contains(end.StartReference);
                            }

                            if (!foundTag)
                            {
                                string message;

                                message = string.Format(
                                                        Properties.Resources.StandardValidator_CanDeleteFailure_Format,
                                                        codeBase.GetType().Name,
                                                        codeBase.Id);
                                throw new ValidationException(
                                                              ValidationError.CodeBaseTagDeleted,
                                                              message,
                                                              selectorPath);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates CanReorder editing hints on the <see cref="Source"/> and <see cref="Target"/> elements and throws a
        /// <see cref="ValidationException"/> if any validation errors occur.
        /// </summary>
        /// <param name="sourceTags">A list of <see cref="ResourceStringContent"/> elements contained within
        /// the source element. The elements are ordered in the same order as which they appear in the source using a
        /// depth first search traversal.</param>
        /// <param name="targetTags">A list of <see cref="ResourceStringContent"/> elements contained within
        /// the target element. The elements are ordered in the same order as which they appear in the source using a
        /// depth first search traversal.</param>
        private void ValidateEditingHintsCanReorder(
                                                    List<ResourceStringContent> sourceTags,
                                                    List<ResourceStringContent> targetTags)
        {
            Dictionary<string, List<CodeBase>> sourceSequences;

            // Get the editing hints. This also validates the sequence data.
            sourceSequences = StandardValidator.GetAndValidateEditingHintsSequences(sourceTags);

            // Validate that the hierarchy is valid. All tags must reside under the tag with CanReorder=firstNoe
            // or they must be siblings of that tag.
            foreach (KeyValuePair<string, List<CodeBase>> sequence in sourceSequences)
            {
                CodeBase firstAncestor;
                List<string> ids;

                firstAncestor = null;
                ids = new List<string>();
                for (int i = 0; i < sequence.Value.Count; i++)
                {
                    CodeBase ancestor;

                    ancestor = sequence.Value[i].SelectableAncestor as CodeBase;
                    if (i == 0)
                    {
                        firstAncestor = ancestor;
                    }

                    if ((ancestor == null) && (firstAncestor == null))
                    {
                        // The tag essentially resides under the source.
                    }
                    else if ((ancestor == null) ||
                             ((!ids.Contains(ancestor.Id)) &&
                                ((firstAncestor == null) || (ancestor.Id != firstAncestor.Id))))
                    {
                        // The tag resides under another CodeBase and either the first one essentially resides under
                        // the source, or it resides under a different CodeBase.
                        throw new ValidationException(
                                            ValidationError.CodeBaseInvalidSourceSequenceHierarchy,
                                            Properties.Resources.StandardValidator_SequenceAncestorNotInHierarchy,
                                            sequence.Value[i].SelectorPath);
                    }

                    ids.Add(sequence.Value[i].Id);
                }
            }

            // targetTagsMap may be null when validating the segment without a target.
            if (targetTags != null)
            {
                Dictionary<string, List<CodeBase>> targetSequences;

                // Validate canReorder by locating all sequences of length > 1 and verifying the order of the inline
                // tags in the sequence are the same between the source and target.
                targetSequences = StandardValidator.GetAndValidateEditingHintsSequences(targetTags);

                foreach (KeyValuePair<string, List<CodeBase>> sequence in sourceSequences)
                {
                    List<CodeBase> targetSequence;

                    if (targetSequences.TryGetValue(sequence.Key, out targetSequence))
                    {
                        CodeBase firstTargetAncestor;
                        List<string> ids;
                        bool sequenceMatch;

                        firstTargetAncestor = null;
                        ids = new List<string>();

                        sequenceMatch = sequence.Value.Count == targetSequence.Count;
                        for (int i = 0; sequenceMatch && (i < sequence.Value.Count); i++)
                        {
                            CodeBase sourceAncestor;
                            CodeBase targetAncestor;

                            // Verify the tag types are the same, the Ids are the same.
                            sequenceMatch = (sequence.Value[i].GetType() == targetSequence[i].GetType()) &&
                                            (sequence.Value[i].Id == targetSequence[i].Id);

                            sourceAncestor = sequence.Value[i].SelectableAncestor as CodeBase;
                            targetAncestor = targetSequence[i].SelectableAncestor as CodeBase;
                            if (i == 0)
                            {
                                firstTargetAncestor = targetAncestor;
                            }

                            // Verify the heirarchy. If the ancestor Ids match then either the ancestor is part of
                            // the sequence in which case that ancestor will be verified itself within the loop, or
                            // ancestor is not part of the hierarchy and must match the ancestor of the first element
                            // in the sequence.
                            if ((sourceAncestor == null) || !ids.Contains(sourceAncestor.Id))
                            {
                                // The source tag is at the top of the hierarchy. Verify the target tag is also at the
                                // top of the hierarchy.
                                if ((targetAncestor == null) && (firstTargetAncestor == null))
                                {
                                    // The tag essentially resides under the target.
                                }
                                else if ((targetAncestor == null) ||
                                         (firstTargetAncestor == null) ||
                                         (targetAncestor.Id != firstTargetAncestor.Id))
                                {
                                    throw new ValidationException(
                                                ValidationError.CodeBaseInvalidTargetSequenceHierarchy,
                                                Properties.Resources.StandardValidator_SequenceAncestorNotInHierarchy,
                                                sequence.Value[i].SelectorPath);
                                }
                            }
                            else if ((targetAncestor == null) || (sourceAncestor.Id != targetAncestor.Id))
                            {
                                // The tag doesn't have the same hierarchy within the sequence as the source.
                                throw new ValidationException(
                                                ValidationError.CodeBaseSequenceHierarchyMismatch,
                                                Properties.Resources.StandardValidator_DifferentSequenceHierarchy,
                                                sequence.Value[i].SelectorPath);
                            }

                            ids.Add(targetSequence[i].Id);
                        }

                        if (!sequenceMatch)
                        {
                            StringBuilder builder;
                            string message;

                            builder = new StringBuilder();
                            foreach (CodeBase item in sequence.Value)
                            {
                                builder.AppendFormat(
                                                     Properties.Resources.StandardValidator_SequenceEntry_Format,
                                                     item.GetType().Name,
                                                     item.Id);
                            }

                            message = string.Format(
                                                    Properties.Resources.StandardValidator_SequenceDoesntMatch_Format,
                                                    sequence.Value[0].GetType().Name,
                                                    sequence.Key,
                                                    builder.ToString().TrimEnd(new[] { ',' }));
                            throw new ValidationException(
                                                          ValidationError.CodeBaseSequenceMismatch,
                                                          message,
                                                          sequence.Value[0].SelectorPath);
                        }
                    }
                    else
                    {
                        string message;

                        message = string.Format(
                                                Properties.Resources.StandardValidator_SequenceNotFound_Format,
                                                sequence.Value[0].GetType().Name,
                                                sequence.Key);
                        throw new ValidationException(
                                                      ValidationError.CodeBaseSequenceNotFound,
                                                      message,
                                                      sequence.Value[0].SelectorPath);
                    }
                }
            }
        }

        /// <summary>
        /// Validates certain aspects of the children of an extension. This method will also validate the children of
        /// those children.
        /// </summary>
        /// <param name="children">The children of an extension to validate.</param>
        /// <param name="selectorPath">The selector path of the XliffElement that contains the extension.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        /// <remarks>This method will validate that all the Ids for siblings are unique. Because extensions are not
        /// understood, Ids are determined as attributes with "id" as the local name.</remarks>
        private void ValidateExtensionChildren(IEnumerable<ElementInfo> children, string selectorPath)
        {
            if (children != null)
            {
                List<string> childIds;

                childIds = new List<string>();
                foreach (ElementInfo info in children)
                {
                    IXliffDataProvider provider;
                    string id;
                    string xmlId;

                    id = null;
                    xmlId = null;

                    provider = info.Element;
                    foreach (IAttributeDataProvider data in provider.GetXliffAttributes())
                    {
                        if (data.LocalName == AttributeNames.Id)
                        {
                            string value;

                            value = data.Value as string;
                            if (value != null)
                            {
                                if (data.Prefix == NamespacePrefixes.Xml)
                                {
                                    // Found xml:id="xyz".
                                    xmlId = value;
                                }
                                else
                                {
                                    // Found id="xyz" or possibly abc:id="xyz".
                                    id = value;
                                }
                            }
                        }
                    }

                    // Use id="xyz" if found, otherwise use xml:id="xyz".
                    id = id ?? xmlId;
                    if (id != null)
                    {
                        if (childIds.Contains(id))
                        {
                            string message;
                            ISelectable selectable;

                            // The selectable ancestor may be null if the extension's parent wasn't set.
                            selectable = info.Element.SelectableAncestor;

                            message = string.Format(
                                                    Properties.Resources.StandardValidator_DuplicateExtensionId_Format,
                                                    id,
                                                    info.Name,
                                                    info.Namespace);
                            throw new ValidationException(
                                                          ValidationError.ExtensionIdDuplicate,
                                                          message,
                                                          (selectable == null) ? selectorPath : selectable.SelectorPath);
                        }

                        childIds.Add(id);
                    }

                    this.ValidateExtensionChildren(provider.GetXliffChildren(), selectorPath);
                }
            }
        }

        /// <summary>
        /// Validates aspects of all extensions within the document.
        /// </summary>
        /// <param name="document">The document that may contain extensions to validate.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateExtensions(XliffDocument document)
        {
            foreach (XliffElement child in document.CollapseChildren(new[] { typeof(IExtensible) }))
            {
                IExtensible extensible;

                extensible = child as IExtensible;
                if (extensible.HasExtensions && extensible.SupportsElementExtensions)
                {
                    foreach (IExtension extension in extensible.Extensions)
                    {
                        if (extension.HasChildren)
                        {
                            this.ValidateExtensionChildren(
                                                           extension.GetChildren(),
                                                           child.SelectableAncestor.SelectorPath);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates the <see cref="File"/> element conforms to the standard.
        /// </summary>
        /// <param name="file">The <see cref="File"/> to validate.</param>
        /// <remarks>This method assumes the Id is validated to be unique amongst siblings outside this method.
        /// </remarks>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateFile(File file)
        {
            Debug.Assert(file != null, "file should not be null.");

            // Id must not be null.
            if (string.IsNullOrWhiteSpace(file.Id))
            {
                throw new ValidationException(
                                              ValidationError.FileIdNull,
                                              Properties.Resources.StandardValidator_IdNotSpecified,
                                              file.SelectorPath);
            }
            else
            {
                this.ValidateNMTOKEN(file.Id, false, "Id", ValidationError.FileIdNotNMToken, file.SelectorPath);
            }

            // Original must be an IRI. Skipping validation.

            // There must be at least one Unit or Group.
            if (file.Containers.Count < 1)
            {
                string message;

                message = string.Format(Properties.Resources.StandardValidator_NoElements_Format, "Containers");
                throw new ValidationException(ValidationError.FileMissingContainer, message, file.SelectorPath);
            }

            // Group.Id must be unique among sibling Groups.
            // Unit.Id must be unique among sibling Units.
            this.ValidateIds(this.GetTranslationContainersList<Unit>(file), false);
            this.ValidateIds(this.GetTranslationContainersList<Group>(file), false);

            foreach (TranslationContainer container in file.Containers)
            {
                this.ValidateTranslationContainer(container);
            }

            this.ValidateNotes(file);
            this.ValidateSkeleton(file.Skeleton);
            this.ValidateChangeTrackingModule_ChangeTrack(file.Changes, file.SelectorPath);
            this.ValidateFormatStyles(file, file.SelectorPath);
            this.ValidateMetadata(file, file.SelectorPath);
            this.ValidateResourceDataModule_ResourceData(file.ResourceData, file.SelectorPath);
            this.ValidateSizeRestrictionModule_Attributes(file, file.SelectorPath);
            this.ValidateSizeRestrictionModule_ProfileData(file.ProfileData, file.SelectorPath);
            this.ValidateValidationModule_Validation(file.ValidationRules, file.SelectorPath);
        }

        /// <summary>
        /// Validates the <see cref="IFormatStyleAttributes"/> information conforms to the standard.
        /// </summary>
        /// <param name="formatStyles">The format styles to validate.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateFormatStyles(IFormatStyleAttributes formatStyles, string selectorPath)
        {
            SpanningCodeEnd end;

            if ((formatStyles.SubFormatStyle.Count > 0) &&
                (formatStyles.FormatStyle == null))
            {
                // Constraint: If the attribute subFs is used, the attribute fs MUST be specified as well.
                throw new ValidationException(
                                              ValidationError.FormatStyleSubFormatWithoutFormat,
                                              Properties.Resources.StandardValidator_FormatStyleSubFormatWithoutFormat,
                                              selectorPath);
            }

            end = formatStyles as SpanningCodeEnd;
            if ((end != null) && !end.Isolated && (formatStyles.FormatStyle != null))
            {
                // Constraint: The fs MUST only be used with <ec> in cases where the isolated attribute is set to 'yes'.
                // Constraint: The subFs MUST only be used with <ec> in cases where the isolated attribute is set to 'yes'.
                throw new ValidationException(
                                              ValidationError.FormatStyleWithSpanEndNotIsolated,
                                              Properties.Resources.StandardValidator_FormatStyleWithSpanEndNotIsolated,
                                              selectorPath);
            }
        }

        /// <summary>
        /// Validates the <see cref="Glossary"/> element conforms to the standard.
        /// </summary>
        /// <param name="glossary">The <see cref="Glossary"/> to validate.</param>
        /// <param name="textElements">The list of selectable inline tags and segments that glossary references must
        /// match.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateGlossary(Glossary glossary, List<ISelectable> textElements, string selectorPath)
        {
            if (glossary != null)
            {
                List<string> textElementSelectorIds;
                List<string> glossaryIds;

                // There must be at least one GlossaryEntry.
                if (glossary.Entries.Count < 1)
                {
                    string message;

                    message = string.Format(Properties.Resources.StandardValidator_NoElements_Format, "GlossaryEntry");
                    throw new ValidationException(
                                                  ValidationError.GlossaryMissingEntry,
                                                  message,
                                                  selectorPath);
                }

                textElementSelectorIds = new List<string>();
                foreach (ISelectable selectable in textElements)
                {
                    if (selectable.SelectorId != null)
                    {
                        // Store the selector Id because the element may be a target which is prefixed with "t=".
                        textElementSelectorIds.Add(selectable.SelectorId);
                    }
                }

                // Create the list of Ids used within the glossary. Individual children will compare their Ids with
                // this list and add their Ids as appropriate.
                glossaryIds = new List<string>();

                foreach (GlossaryEntry entry in glossary.Entries)
                {
                    this.ValidateGlossaryEntry(entry, glossaryIds, textElementSelectorIds, selectorPath);
                }
            }
        }

        /// <summary>
        /// Validates the <see cref="GlossaryEntry"/> element conforms to the standard.
        /// </summary>
        /// <param name="entry">The <see cref="GlossaryEntry"/> to validate.</param>
        /// <param name="glossaryIds">The list of Ids used within the glossary up to the point this method was called.
        /// This is used to validate that Ids are not duplicated within the glossary.</param>
        /// <param name="textElementSelectorIds">The list of selectable inline tags and segments that glossary
        /// references must match.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateGlossaryEntry(
                                           GlossaryEntry entry,
                                           List<string> glossaryIds,
                                           List<string> textElementSelectorIds,
                                           string selectorPath)
        {
            if (entry.Id != null)
            {
                // Id must be a valid NMTOKEN.
                this.ValidateNMTOKEN(entry.Id, true, "Id", ValidationError.GlossaryEntryIdNotNMToken, selectorPath);

                // Id must be unique between all GlossaryEntry and Translation elements.
                if (glossaryIds.Contains(entry.Id))
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.StandardValidator_DuplicateIdWithinHierarchy_Format,
                                            entry.Id,
                                            typeof(GlossaryEntry).Name,
                                            typeof(Glossary).Name);
                    throw new ValidationException(ValidationError.GlossaryEntryIdDuplicate, message, selectorPath);
                }

                // Add the Id so other elements can check for duplicity.
                glossaryIds.Add(entry.Id);
            }

            if ((entry.Translations.Count == 0) && (entry.Definition == null))
            {
                throw new ValidationException(
                                              ValidationError.GlossaryEntryChildrenMissing,
                                              Properties.Resources.StandardValidator_GlossaryEntryMissingChildren,
                                              selectorPath);
            }

            // Validate the reference points to a valid source text element.
            this.ValidateIriReference(entry.Reference, textElementSelectorIds);

            foreach (Translation translation in entry.Translations)
            {
                this.ValidateGlossaryTranslation(translation, glossaryIds, textElementSelectorIds, selectorPath);
            }
        }

        /// <summary>
        /// Validates the <see cref="Translation"/> element conforms to the standard.
        /// </summary>
        /// <param name="translation">The <see cref="Translation"/> to validate.</param>
        /// <param name="glossaryIds">The list of Ids used within the glossary up to the point this method was called.
        /// This is used to validate that Ids are not duplicated within the glossary.</param>
        /// <param name="textElementSelectorIds">The list of selectable inline tags and segments that glossary
        /// references must match.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateGlossaryTranslation(
                                                 Translation translation,
                                                 List<string> glossaryIds,
                                                 List<string> textElementSelectorIds,
                                                 string selectorPath)
        {
            if (translation.Id != null)
            {
                // Id must be a valid NMTOKEN.
                this.ValidateNMTOKEN(
                                     translation.Id,
                                     true,
                                     "Id",
                                     ValidationError.GlossaryTranslationIdNotNMToken,
                                     selectorPath);

                // Id must be unique between all GlossaryEntry and Translation elements.
                if (glossaryIds.Contains(translation.Id))
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.StandardValidator_DuplicateIdWithinHierarchy_Format,
                                            translation.Id,
                                            typeof(Translation).Name,
                                            typeof(Glossary).Name);
                    throw new ValidationException(
                                                  ValidationError.GlossaryTranslationIdDuplicate,
                                                  message,
                                                  selectorPath);
                }

                // Add the Id so other elements can check for duplicity.
                glossaryIds.Add(translation.Id);
            }

            // Validate the reference points to a valid source text element.
            this.ValidateIriReference(translation.Reference, textElementSelectorIds);
        }

        /// <summary>
        /// Validates the <see cref="Group"/> conforms to the standard.
        /// </summary>
        /// <param name="group">The <see cref="Group"/> to validate.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateGroup(Group group)
        {
            Debug.Assert(group != null, "group should not be null.");

            foreach (TranslationContainer container in group.Containers)
            {
                this.ValidateTranslationContainer(container);
            }
        }

        /// <summary>
        /// Validates that a set of sibling elements have unique Ids across the set.
        /// </summary>
        /// <param name="siblings">The elements to compare for uniqueness.</param>
        /// <param name="uniqueAcrossAllTypes">True if the Id needs to be unique across all sibling types, false
        /// if the Id needs to be unique only across elements of the same type.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateIds(IEnumerable<ISelectable> siblings, bool uniqueAcrossAllTypes)
        {
            this.ValidateIds(siblings, uniqueAcrossAllTypes, null);
        }

        /// <summary>
        /// Validates that a set of sibling elements have unique Ids across the set.
        /// </summary>
        /// <param name="siblings">The elements to compare for uniqueness.</param>
        /// <param name="uniqueAcrossAllTypes">True if the Id needs to be unique across all sibling types, false
        /// if the Id needs to be unique only across elements of the same type.</param>
        /// <param name="selectorPath">The path to the container where validation is occurring. This value is only
        /// used if the item with the Id being validated doesn't have a selector path.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateIds(IEnumerable<IIdentifiable> siblings, bool uniqueAcrossAllTypes, string selectorPath)
        {
            Dictionary<Type, HashSet<string>> idSet;

            idSet = new Dictionary<Type, HashSet<string>>();
            foreach (IIdentifiable element in siblings)
            {
                if (element.Id != null)
                {
                    HashSet<string> set;
                    ISelectable selectable;
                    Type key;
                    string path;

                    selectable = element as ISelectable;
                    path = (selectable == null) ? selectorPath : selectable.SelectorPath;
                    this.ValidateIsNotWhitespaceOnly(element.Id, "Id", path, ValidationError.ElementIdEmpty);

                    key = uniqueAcrossAllTypes ? typeof(XliffElement) : element.GetType();
                    if (!idSet.TryGetValue(key, out set))
                    {
                        set = new HashSet<string>();
                        idSet[key] = set;
                    }

                    if (set.Contains(element.Id))
                    {
                        string message;

                        message = string.Format(Properties.Resources.StandardValidator_DuplicateId_Format, element.Id);
                        throw new ValidationException(ValidationError.ElementIdDuplicate, message, path);
                    }

                    set.Add(element.Id);
                }
            }
        }

        /// <summary>
        /// The core implementation that validates the document.
        /// </summary>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateImpl()
        {
            bool hasTarget;

            // Version is required.
            if (string.IsNullOrWhiteSpace(this.document.Version))
            {
                throw new ValidationException(
                                              ValidationError.DocumentVersionNull,
                                              Properties.Resources.StandardValidator_DocumentVersionNotSpecified,
                                              this.document.SelectorPath);
            }

            if (string.IsNullOrWhiteSpace(this.document.SourceLanguage))
            {
                throw new ValidationException(
                                              ValidationError.DocumentSourceLangNull,
                                              Properties.Resources.StandardValidator_LanguageNotSpecified,
                                              this.document.SelectorPath);
            }

            // SourceLanguage and TargetLanguage must be BCP-47 compliant.
            this.ValidateBcp47Language(
                                       this.document.SourceLanguage,
                                       false,
                                       "SourceLanguage",
                                       ValidationError.DocumentSourceLangInvalid,
                                       this.document.SelectorPath);

            this.ValidateBcp47Language(
                                       this.document.TargetLanguage,
                                       true,
                                       "TargetLanguage",
                                       ValidationError.DocumentTargetLangInvalid,
                                       this.document.SelectorPath);

            // There must be at least one File.
            if (this.document.Files.Count < 1)
            {
                string message;

                message = string.Format(Properties.Resources.StandardValidator_NoElements_Format, "Files");
                throw new ValidationException(
                                              ValidationError.DocumentMissingFile,
                                              message,
                                              this.document.SelectorPath);
            }

            // File Ids must be unique among siblings.
            this.ValidateIds(this.document.Files, false);

            foreach (File file in this.document.Files)
            {
                this.ValidateFile(file);
            }

            // TargetLanguage is required only if there are Target elements. Otherwise TargetLanguage is optional.
            hasTarget = false;
            foreach (File file in this.document.Files)
            {
                hasTarget = this.CheckForTarget(file.Containers);
                if (hasTarget)
                {
                    break;
                }
            }

            if (hasTarget && string.IsNullOrWhiteSpace(this.document.TargetLanguage))
            {
                throw new ValidationException(
                                              ValidationError.DocumentMissingTargetLang,
                                              Properties.Resources.StandardValidator_DocumentTargetLanguageNotSpecified,
                                              this.document.SelectorPath);
            }

            this.ValidateExtensions(this.document);
        }

        /// <summary>
        /// Validates an Id refer to a valid element.
        /// </summary>
        /// <param name="iri">The IRI of the element to find. If this value is null, this method doesn't do anything.</param>
        /// <param name="selectorIds">The list of selector Ids to search in.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateIriReference(string iri, List<string> selectorIds)
        {
            string reference;

            if ((iri != null) &&
                (!Utilities.TryParseIri(iri, out reference) || !selectorIds.Contains(reference)))
            {
                string message;

                message = string.Format(
                                        Properties.Resources.StandardValidator_ElementNotFound_Format,
                                        typeof(ResourceStringContent).Name,
                                        iri);
                throw new ValidationException(ValidationError.IriInvalid, message, reference);
            }
        }

        /// <summary>
        /// Validates that text is either null or a valid string that is not just whitespace.
        /// </summary>
        /// <param name="text">The text to validate.</param>
        /// <param name="name">The name of the text being validated.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <param name="errorNumber">A number that identifies the error.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        public void ValidateIsNotWhitespaceOnly(string text, string name, string selectorPath, int errorNumber)
        {
            if ((text != null) && string.IsNullOrWhiteSpace(text))
            {
                string message;

                message = string.Format(Properties.Resources.StandardValidator_EmptyString_Format, name);
                throw new ValidationException(errorNumber, message, selectorPath);
            }
        }

        /// <summary>
        /// Validates that each <see cref="SpanningCodeStart"/> has Isolated set to true.
        /// </summary>
        /// <param name="list">The list of <see cref="SpanningCodeStart"/> elements to validate.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateIsolated(List<SpanningCodeStart> list)
        {
            foreach (SpanningCodeStart start in list)
            {
                if (!start.Isolated)
                {
                    throw new ValidationException(
                                                  ValidationError.SpanningCodeStartNotIsolated,
                                                  Properties.Resources.StandardValidator_SpanningCodeStartNotIslated,
                                                  start.SelectorPath);
                }
            }
        }

        /// <summary>
        /// Validates the <see cref="Match"/> element conforms to the standard.
        /// </summary>
        /// <param name="match">The <see cref="Match"/> to validate.</param>
        /// <param name="textElements">The list of <see cref="Segment"/>s, <see cref="Ignorable"/>s, and inline
        /// tags under the <see cref="Unit"/> that contains the match.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateMatch(Match match, List<ISelectable> textElements, string selectorPath)
        {
            Dictionary<string, ResourceStringContent> sourceTags;
            const float MinValue = 0;
            const float MaxValue = 100;

            this.ValidateNMTOKEN(match.Id, true, "Id", ValidationError.MatchIdNotNMToken, selectorPath);

            if (!StandardValidator.IsInRangeOrNull(match.MatchQuality, MinValue, MaxValue))
            {
                string message;

                message = string.Format(
                                        Properties.Resources.StandardValidator_ValueNotInRange_Format,
                                        "MatchQuality",
                                        MinValue,
                                        MaxValue);
                throw new ValidationException(ValidationError.MatchQualityNotInRange, message, selectorPath);
            }

            if (!StandardValidator.IsInRangeOrNull(match.MatchSuitability, MinValue, MaxValue))
            {
                string message;

                message = string.Format(
                                        Properties.Resources.StandardValidator_ValueNotInRange_Format,
                                        "MatchSuitability",
                                        MinValue,
                                        MaxValue);
                throw new ValidationException(ValidationError.MatchSuitabilityNotInRange, message, selectorPath);
            }

            if (!StandardValidator.IsInRangeOrNull(match.Similarity, MinValue, MaxValue))
            {
                string message;

                message = string.Format(
                                        Properties.Resources.StandardValidator_ValueNotInRange_Format,
                                        "Similarity",
                                        MinValue,
                                        MaxValue);
                throw new ValidationException(ValidationError.MatchSimilarityNotInRange, message, selectorPath);
            }

            sourceTags = StandardValidator.GetSelectableResourceStringContents(match.Source);
            this.ValidateSource(
                                match.Source,
                                selectorPath,
                                sourceTags);

            // Validate that SourceReference refers to a valid Source span.
            if (match.SourceReference == null)
            {
                string message;

                message = string.Format(
                                        Properties.Resources.StandardValidator_PropertyNotSpecified_Format,
                                        "SourceReference");
                throw new ValidationException(ValidationError.MatchSourceRefNull, message, selectorPath);
            }
            else
            {
                bool foundSourceReference;
                string id;

                // SourceReference points to a span of source text within the same unit, to which the translation
                // candidate is relevant. This may be a segment, ignorable, inline tag from the source, or inline tag
                // from the target.                
                foundSourceReference = false;
                if (Utilities.TryParseIri(match.SourceReference, out id))
                {
                    foreach (ISelectable selectable in textElements)
                    {
                        foundSourceReference |= id == selectable.SelectorId;
                    }
                }

                if (!foundSourceReference)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.StandardValidator_NoMatchingSourceReference_Format,
                                            match.SourceReference);
                    throw new ValidationException(ValidationError.MatchMissingSourceRef, message, selectorPath);
                }
            }

            if (match.SubType != null)
            {
                this.ValidatePrefixValueFormat(match.SubType, "SubType", selectorPath);
            }

            if (match.Target == null)
            {
                throw new ValidationException(
                                              ValidationError.MatchTargetNull,
                                              Properties.Resources.StandardValidator_TargetNotSpecified,
                                              selectorPath);
            }
            else
            {
                this.ValidateTarget(
                                    match.Target,
                                    false,
                                    StandardValidator.GetSelectableResourceStringContents(match.Target),
                                    sourceTags);
                this.ValidateEditingHints(match.Source, match.Target, selectorPath);
            }

            this.ValidateMetadata(match, selectorPath);
        }

        /// <summary>
        /// Validates the <see cref="Meta"/> element conforms to the standard.
        /// </summary>
        /// <param name="meta">The <see cref="Meta"/> to validate.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateMeta(Meta meta, string selectorPath)
        {
            if (string.IsNullOrWhiteSpace(meta.Type))
            {
                string message;

                message = string.Format(Properties.Resources.StandardValidator_PropertyNotSpecified_Format, "Type");
                throw new ValidationException(ValidationError.MetaTypeNull, message, selectorPath);
            }
        }

        /// <summary>
        /// Validates the <see cref="IMetadataStorage"/> element conforms to the standard.
        /// </summary>
        /// <param name="provider">The item that contains the metadata to validate.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateMetadata(IMetadataStorage provider, string selectorPath)
        {
            if (provider.Metadata != null)
            {
                Stack<MetaGroup> elements;
                HashSet<string> set;

                this.ValidateNMTOKEN(provider.Metadata.Id, true, "Id", ValidationError.MetadataIdNotNMToken, selectorPath);

                if (provider.Metadata.Groups.Count < 1)
                {
                    string message;

                    message = string.Format(Properties.Resources.StandardValidator_NoElements_Format, "MetaGroup");
                    throw new ValidationException(ValidationError.MetadataMissingGroup, message, selectorPath);
                }

                foreach (MetaGroup group in provider.Metadata.Groups)
                {
                    this.ValidateMetaGroup(group, selectorPath);
                }

                // Validate that all groups and metadata elements have unique Ids.
                set = new HashSet<string>();
                if (provider.Metadata.Id != null)
                {
                    set.Add(provider.Metadata.Id);
                }

                elements = new Stack<MetaGroup>(provider.Metadata.Groups);
                while (elements.Count > 0)
                {
                    MetaGroup group;

                    group = elements.Pop();
                    if (group.Id != null)
                    {
                        if (set.Contains(group.Id))
                        {
                            string message;

                            message = string.Format(Properties.Resources.StandardValidator_DuplicateId_Format, group.Id);
                            throw new ValidationException(ValidationError.MetadataIdDuplicate, message, selectorPath);
                        }

                        set.Add(group.Id);
                    }

                    foreach (MetaElement child in group.Containers)
                    {
                        if (child is MetaGroup)
                        {
                            elements.Push((MetaGroup)child);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates the <see cref="MetaGroup"/> element conforms to the standard.
        /// </summary>
        /// <param name="group">The <see cref="MetaGroup"/> to validate.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateMetaGroup(MetaGroup group, string selectorPath)
        {
            this.ValidateNMTOKEN(group.Id, true, "Id", ValidationError.MetaGroupIdNotNMToken, selectorPath);

            if (group.Containers.Count < 1)
            {
                string message;

                message = string.Format(Properties.Resources.StandardValidator_NoElements_Format, "MetaElement");
                throw new ValidationException(ValidationError.MetaGroupMissingContainer, message, selectorPath);
            }

            foreach (MetaElement container in group.Containers)
            {
                if (container is MetaGroup)
                {
                    this.ValidateMetaGroup((MetaGroup)container, selectorPath);
                }
                else
                {
                    this.ValidateMeta((Meta)container, selectorPath);
                }
            }
        }

        /// <summary>
        /// Validates that the specified token conforms is a valid NMTOKEN. If not, a ValidationException is thrown.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <param name="allowNull">True if the token is allowed to be null. False if the token cannot be null.</param>
        /// <param name="property">The name of the property being validated.</param>
        /// <param name="errorNumber">A number that identifies the error if there is one.</param>
        /// <param name="selectorPath">The selector path to the element being validated.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateNMTOKEN(string token, bool allowNull, string property, int errorNumber, string selectorPath)
        {
            Exception exceptionInfo;

            if ((!allowNull || (token != null)) && !Utilities.IsValidNMTOKEN(token, out exceptionInfo))
            {
                string message;

                message = string.Format(Properties.Resources.StandardValidator_PropertyNotNMTOKEN_Format, property);
                throw new ValidationException(errorNumber, message, selectorPath, exceptionInfo);
            }
        }

        /// <summary>
        /// Validates the <see cref="Note"/>s elements conform to the standard.
        /// </summary>
        /// <param name="container">The container that holds the notes.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateNotes(INoteContainer container)
        {
            Debug.Assert(container != null, "container should not be null.");

            // Note.Id must be unique among siblings Notes.
            this.ValidateIds(container.Notes, false);

            foreach (Note note in container.Notes)
            {
                const int MinPriority = 1;
                const int MaxPriority = 10;

                this.ValidateNMTOKEN(note.Id, true, "Id", ValidationError.NoteIdNotNMToken, note.SelectorPath);

                // Priority must be between 1 and 10.
                if ((note.Priority < MinPriority) || (note.Priority > MaxPriority))
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.StandardValidator_InvalidPriority_Format,
                                            MinPriority,
                                            MaxPriority);
                    throw new ValidationException(ValidationError.NoteInvalidPriority, message, note.SelectorPath);
                }

                this.ValidateFormatStyles(note, note.SelectorPath);
            }
        }

        /// <summary>
        /// Validates the <see cref="OriginalData"/> element conforms to the standard.
        /// </summary>
        /// <param name="original">The <see cref="OriginalData"/> to validate.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateOriginalData(OriginalData original)
        {
            if (original != null)
            {
                // Data.Id must be unique among all siblings.
                this.ValidateIds(original.DataElements, false);

                foreach (Data data in original.DataElements)
                {
                    this.ValidateData(data);
                }
            }
        }

        /// <summary>
        /// Validates that a string matches the pattern of "prefix:value" where the prefix must not match any reserved
        /// values and that prefix and value are not empty strings.
        /// </summary>
        /// <param name="text">The text to validate.</param>
        /// <param name="name">The name of the text to validate.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidatePrefixValueFormat(string text, string name, string selectorPath)
        {
            this.TryValidatePrefixValueFormat(text, name, selectorPath, true);
        }

        /// <summary>
        /// Validates the ResourceData element from the ResourceData module conforms to the standard.
        /// </summary>
        /// <param name="data">The ResourceData to validate.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateResourceDataModule_ResourceData(ResourceData data, string selectorPath)
        {
            if (data != null)
            {
                HashSet<string> childIds;

                if ((data.ResourceItems.Count == 0) && (data.ResourceItemReferences.Count == 0))
                {
                    throw new ValidationException(
                                                  ValidationError.ResourceDataMissingItems,
                                                  Properties.Resources.StandardValidator_ResourceDataMissingItems,
                                                  selectorPath);
                }

                childIds = this.ValidateResourceDataModule_ResourceItems(data.ResourceItems);
                this.ValidateResourceDataModule_ResourceItemRefs(data.ResourceItemReferences, childIds);
            }
        }

        /// <summary>
        /// Validates the ResourceItemRef elements from the ResourceData module conform to the standard.
        /// </summary>
        /// <param name="resourceItems">The ResourceItemRefs to validate.</param>
        /// <param name="childIds">The list of unique Ids from the sibling ResourceItems. This is used to validate that
        /// the ResourceItemRef Ids are unique across siblings.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateResourceDataModule_ResourceItemRefs(IEnumerable<ResourceItemRef> resourceItems, HashSet<string> childIds)
        {
            HashSet<string> itemIds;

            itemIds = new HashSet<string>(childIds);
            foreach (ResourceItemRef itemRef in resourceItems)
            {
                if (itemRef.Id != null)
                {
                    this.ValidateNMTOKEN(
                                         itemRef.Id,
                                         true,
                                         "Id",
                                         ValidationError.ResourceItemRefIdNotNMToken,
                                         itemRef.SelectorPath);

                    if (childIds.Contains(itemRef.Id))
                    {
                        // Id must be unique among ResourceItems and ResourceItemRefs.
                        string message;

                        message = string.Format(Properties.Resources.StandardValidator_DuplicateId_Format, itemRef.Id);
                        throw new ValidationException(
                                                      ValidationError.ResourceItemRefIdDuplicate,
                                                      message,
                                                      itemRef.SelectorPath);
                    }

                    childIds.Add(itemRef.Id);

                    if ((itemRef == null) || !itemIds.Contains(itemRef.Reference))
                    {
                        string message;

                        message = string.Format(
                                        Properties.Resources.StandardValidator_ResourceItemRefInvalidReference_Format,
                                        (itemRef == null) ? "(null)" : itemRef.Reference);
                        throw new ValidationException(
                                                      ValidationError.ResourceItemRefInvalidReference,
                                                      message,
                                                      itemRef.SelectorPath);
                    }
                }
            }
        }

        /// <summary>
        /// Validates the ResourceItemReferenceBase elements from the ResourceData module.
        /// </summary>
        /// <param name="reference">The ResourceItemReferenceBase to validate.</param>
        /// <param name="subject">The type of object being validated.</param>
        /// <param name="selectorPath">The selector path of the element being validated.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateResourceDataModule_ResourceItemReferenceBase(
                                                                          ResourceItemReferenceBase reference,
                                                                          TranslationSubject subject,
                                                                          string selectorPath)
        {
            if (reference != null)
            {
                // HRef must be a valid IRI - skipping validation

                // HRef is required if and only if source/target is empty.
                if (Utilities.HasExtensionChildElements(reference) == (reference.HRef != null))
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.StandardValidator_HRefNotSpecifiedWhenEmpty_Format,
                                            subject);
                    throw new ValidationException(ValidationError.ResourceItemReferenceBaseHRefAndSubject, message, selectorPath);
                }

                // Language must be a BCP-47 value.
                if (reference.Language != null)
                {
                    int errorNumber1;
                    int errorNumber2;
                    string language;
                    string message;

                    if (subject == TranslationSubject.Source)
                    {
                        errorNumber1 = ValidationError.ResourceItemSourceLangInvalid;
                        errorNumber2 = ValidationError.ResourceItemSourceLangMismatch;
                        language = this.document.SourceLanguage;
                        message = Properties.Resources.StandardValidator_SourceLanguageMismatch;
                    }
                    else
                    {
                        errorNumber1 = ValidationError.ResourceItemTargetLangInvalid;
                        errorNumber2 = ValidationError.ResourceItemTargetLangMismatch;
                        language = this.document.TargetLanguage;
                        message = Properties.Resources.StandardValidator_TargetLanguageMismatch;
                    }

                    this.ValidateBcp47Language(
                                               reference.Language,
                                               true,
                                               "Language",
                                               errorNumber1,
                                               selectorPath);

                    // Language must match the document SourceLanguage or TargetLanguage.
                    if (!Utilities.AreLanguagesEqual(reference.Language, language))
                    {
                        throw new ValidationException(errorNumber2, message, selectorPath);
                    }
                }

                if (!Utilities.HasExtensionChildElements(reference))
                {
                    // HRef is required if source is empty.
                    if (string.IsNullOrWhiteSpace(reference.HRef))
                    {
                        int errorNumber;
                        string message;

                        if (subject == TranslationSubject.Source)
                        {
                            errorNumber = ValidationError.ResourceItemSourceHRefNotSpecified;
                        }
                        else
                        {
                            errorNumber = ValidationError.ResourceItemTargetHRefNotSpecified;
                        }

                        message = string.Format(
                                        Properties.Resources.StandardValidator_HRefNotSpecifiedWhenEmpty_Format,
                                        subject.ToString());
                        throw new ValidationException(errorNumber, message, selectorPath);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(reference.HRef))
                {
                    int errorNumber;
                    string message;

                    if (subject == TranslationSubject.Source)
                    {
                        errorNumber = ValidationError.ResourceItemSourceHRefSpecified;
                    }
                    else
                    {
                        errorNumber = ValidationError.ResourceItemTargetHRefSpecified;
                    }

                    // HRef must not be specified if source is not empty.
                    message = string.Format(
                                    Properties.Resources.StandardValidator_HRefNotSpecifiedWhenEmpty_Format,
                                    subject.ToString());
                    throw new ValidationException(errorNumber, message, selectorPath);
                }
            }
        }

        /// <summary>
        /// Validates the ResourceItem elements from the ResourceData module conform to the standard.
        /// </summary>
        /// <param name="resourceItems">The ResourceItems to validate.</param>
        /// <returns>A set of unique Ids from the list of ResourceItems.</returns>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private HashSet<string> ValidateResourceDataModule_ResourceItems(IEnumerable<ResourceItem> resourceItems)
        {
            HashSet<string> itemIds;

            itemIds = new HashSet<string>();
            foreach (ResourceItem item in resourceItems)
            {
                bool isSourceEmpty;
                bool isTargetEmpty;

                // At least one of Source, Target, or Reference must be specified.
                if ((item.Source == null) && (item.Target == null) & (item.References.Count == 0))
                {
                    throw new ValidationException(
                                                  ValidationError.ResourceItemMissingChildren,
                                                  Properties.Resources.StandardValidator_ResourceItemMissingChildren,
                                                  item.SelectorPath);
                }

                if (item.Id != null)
                {
                    // Id must be a valid NMTOKEN.
                    this.ValidateNMTOKEN(item.Id, true, "Id", ValidationError.ResourceItemIdNotNMToken, item.SelectorPath);

                    // Id must be unique among ResourceItems and ResourceItemRefs.
                    if (itemIds.Contains(item.Id))
                    {
                        string message;

                        message = string.Format(Properties.Resources.StandardValidator_DuplicateId_Format, item.Id);
                        throw new ValidationException(ValidationError.ResourceItemIdDuplicate, message, item.SelectorPath);
                    }

                    itemIds.Add(item.Id);
                }

                // If Context is set to false, the Source must be present.
                if (!item.Context && (item.Source == null))
                {
                    throw new ValidationException(
                                    ValidationError.ResourceItemSourceMissingWithNoContext,
                                    Properties.Resources.StandardValidator_ResourceItemSourceMissingWithNoContext,
                                    item.SelectorPath);
                }

                // MimeType is required if target and source are empty.
                isSourceEmpty = (item.Source == null) || !Utilities.HasExtensionChildElements(item.Source);
                isTargetEmpty = (item.Target == null) || !Utilities.HasExtensionAttributes(item.Target);
                if (isSourceEmpty && isTargetEmpty && string.IsNullOrWhiteSpace(item.MimeType))
                {
                    throw new ValidationException(
                                                    ValidationError.MimeTypeNotSpecified,
                                                    Properties.Resources.StandardValidator_MimeTypeRequired,
                                                    item.SelectorPath);
                }

                this.ValidateResourceDataModule_ResourceItemReferenceBase(
                                                                          item.Source,
                                                                          TranslationSubject.Source,
                                                                          item.SelectorPath);
                this.ValidateResourceDataModule_ResourceItemReferenceBase(
                                                                          item.Target,
                                                                          TranslationSubject.Target,
                                                                          item.SelectorPath);

                foreach (Reference reference in item.References)
                {
                    // HRef is required as IRI.
                    if (string.IsNullOrWhiteSpace(reference.HRef))
                    {
                        throw new ValidationException(
                                        ValidationError.ReferenceHRefNotSpecified,
                                        Properties.Resources.StandardValidator_HRefNotSpecifiedWhenRequired,
                                        item.SelectorPath);
                    }

                    // Language must be a BCP-47 value.
                    this.ValidateBcp47Language(
                                               reference.Language,
                                               true,
                                               "Language",
                                               ValidationError.ReferenceLangInvalid,
                                               item.SelectorPath);
                }
            }

            return itemIds;
        }

        /// <summary>
        /// Validates the <see cref="ResourceStringContent"/> element conforms to the standard.
        /// </summary>
        /// <param name="content">The <see cref="ResourceStringContent"/> to validate.</param>
        /// <param name="contentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="ResourceString"/>.</param>
        /// <param name="sourceContentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="Source"/>. This may be null if the <paramref name="contentMap"/> refers to the Source elements
        /// as well.</param>
        /// <param name="markedSpanStartToFind">The list of MarkedSpanStart elements to find during validation to
        /// ensure references refer to valid elements. The value is the MarkedSpanEnd element that refers to the
        /// MarkedSpanStart.</param>
        /// <param name="spanningCodeStartToFind">The list of SpanningCodeStart elements to find during validation to
        /// ensure references refer to valid elements. The value is the SpanningCodeEnd element that refers to the
        /// SpanningCodeStart.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateResourceStringContent(
                                                   ResourceStringContent content,
                                                   Dictionary<string, ResourceStringContent> contentMap,
                                                   Dictionary<string, ResourceStringContent> sourceContentMap,
                                                   Dictionary<string, MarkedSpanEnd> markedSpanStartToFind,
                                                   Dictionary<string, SpanningCodeEnd> spanningCodeStartToFind)
        {
            bool result;

            // Nothing to validate for CDATA, Comments, PlainText, or ProcessingInstructions.
            result = (content is CDataTag) ||
                     (content is CommentTag) ||
                     (content is PlainText) ||
                     (content is ProcessingInstructionTag);
            result |= this.TryValidateCodePoint(content as CodePoint);
            result |= this.TryValidateMarkedSpan(
                                                 content as MarkedSpan,
                                                 contentMap,
                                                 sourceContentMap,
                                                 markedSpanStartToFind,
                                                 spanningCodeStartToFind);
            result |= this.TryValidateMarkedSpanEnd(content as MarkedSpanEnd, markedSpanStartToFind);
            result |= this.TryValidateMarkedSpanStart(content as MarkedSpanStart);
            result |= this.TryValidateSpanningCode(
                                                   content as SpanningCode,
                                                   contentMap,
                                                   sourceContentMap,
                                                   markedSpanStartToFind,
                                                   spanningCodeStartToFind);
            result |= this.TryValidateSpanningCodeEnd(
                                                      content as SpanningCodeEnd,
                                                      contentMap,
                                                      sourceContentMap,
                                                      spanningCodeStartToFind);
            result |= this.TryValidateSpanningCodeStart(content as SpanningCodeStart, contentMap, sourceContentMap);
            result |= this.TryValidateStandaloneCode(content as StandaloneCode, contentMap, sourceContentMap);

            if (!result)
            {
                string message;

                message = string.Format(
                                        Properties.Resources.StandardValidator_InvalidResourceStringContent_Format,
                                        content.GetType().Name);
                throw new ValidationException(
                                              ValidationError.ResourceStringContentInvalid,
                                              message,
                                              content.SelectableAncestor.SelectorPath);
            }
        }

        /// <summary>
        /// Validates the <see cref="IResourceStringContentContainer"/> and its children conform to the standard.
        /// </summary>
        /// <param name="container">The container to validate.</param>
        /// <param name="contentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="ResourceString"/>.</param>
        /// <param name="sourceContentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="Source"/>. This may be null if the <paramref name="contentMap"/> refers to the Source elements
        /// as well.</param>
        /// <param name="markedSpanStartToFind">The list of MarkedSpanStart elements to find during validation to
        /// ensure references refer to valid elements. The value is the MarkedSpanEnd element that refers to the
        /// MarkedSpanStart.</param>
        /// <param name="spanningCodeStartToFind">The list of SpanningCodeStart elements to find during validation to
        /// ensure references refer to valid elements. The value is the SpanningCodeEnd element that refers to the
        /// SpanningCodeStart.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateResourceStringContentContainer(
                                                            IResourceStringContentContainer container,
                                                            Dictionary<string, ResourceStringContent> contentMap,
                                                            Dictionary<string, ResourceStringContent> sourceContentMap,
                                                            Dictionary<string, MarkedSpanEnd> markedSpanStartToFind,
                                                            Dictionary<string, SpanningCodeEnd> spanningCodeStartToFind)
        {
            foreach (ResourceStringContent content in container.Text)
            {
                this.ValidateResourceStringContent(
                                                   content,
                                                   contentMap,
                                                   sourceContentMap,
                                                   markedSpanStartToFind,
                                                   spanningCodeStartToFind);
            }
        }

        /// <summary>
        /// Validates the <see cref="Segment"/> element conforms to the standard.
        /// </summary>
        /// <param name="segment">The <see cref="Segment"/> to validate.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateSegment(Segment segment)
        {
            if (segment != null)
            {
                if (segment.SubState != null)
                {
                    this.ValidatePrefixValueFormat(segment.SubState, "SubState", segment.SelectorPath);
                }
            }
        }

        /// <summary>
        /// Validates SizeRestriction module attributes on an element conform to the standard.
        /// </summary>
        /// <param name="attributes">The attributes to validate.</param>
        /// <param name="selectorPath">The selector path of the element being validated.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateSizeRestrictionModule_Attributes(ISizeRestrictionAttributes attributes, string selectorPath)
        {
            SpanningCodeEnd end;
            bool hasSizeInfo;
            bool hasSizeRefInfo;
            bool isNotIsolated;

            hasSizeInfo = attributes.SupportsSizeInfo && (attributes.SizeInfo != null);
            hasSizeRefInfo = attributes.SupportsSizeInfoReference && (attributes.SizeInfoReference != null);

            end = attributes as SpanningCodeEnd;
            isNotIsolated = (end != null) && !end.Isolated;

            if (attributes.SupportsEquivalentStorage && (attributes.EquivalentStorage != null) && isNotIsolated)
            {
                string message;

                // Constraint 5.7.5.8: This attribute is only allowed on the <ec> element if that element has the
                // isolated attribute set to yes.
                message = string.Format(
                            Properties.Resources.StandardValidator_SizeRestrictionAttributeWithSpanEndNotIsolated_Format,
                            "EquivalentStorage");
                throw new ValidationException(
                                                ValidationError.EquivalentStorageWithSpanEndNotIsolated,
                                                message,
                                                selectorPath);
            }

            if (hasSizeInfo && isNotIsolated)
            {
                string message;

                // Constraint 5.7.5.9: This attribute is only allowed on the <ec> element if that element has the
                // isolated attribute set to yes.
                message = string.Format(
                            Properties.Resources.StandardValidator_SizeRestrictionAttributeWithSpanEndNotIsolated_Format,
                            "SizeInfo");
                throw new ValidationException(
                                                ValidationError.SizeInfoWithSpanEndNotIsolated,
                                                message,
                                                selectorPath);
            }

            if (hasSizeRefInfo && isNotIsolated)
            {
                string message;

                // Constraint 5.7.5.10: This attribute is only allowed on the <ec> element if that element has the
                // isolated attribute set to yes.
                message = string.Format(
                            Properties.Resources.StandardValidator_SizeRestrictionAttributeWithSpanEndNotIsolated_Format,
                            "SizeInfoReference");
                throw new ValidationException(
                                              ValidationError.SizeInfoRefWithSpanEndNotIsolated,
                                              message,
                                              selectorPath);
            }

            if (hasSizeInfo && hasSizeRefInfo)
            {
                // Constraint 5.7.5.9: sizeInfo MUST NOT be specified if and only if sizeInfoRef is used. They MUST NOT
                // be specified at the same time.
                // Constraint 5.7.5.10: sizeInfoRef MUST NOT be specified if and only if sizeInfo is used. They MUST
                // NOT be specified at the same time.
                throw new ValidationException(
                                              ValidationError.SameSizeInfoAndSizeInfoReferencePresence,
                                              Properties.Resources.StandardValidator_SameSizeInfoAndSizeInfoReferencePresence,
                                              selectorPath);
            }
        }

        /// <summary>
        /// Validates the ProfileData element from the SizeRestriction module conforms to the standard.
        /// </summary>
        /// <param name="data">The ProfileData to validate. If this value is null, no validation takes place.
        /// </param>
        /// <param name="selectorPath">The selector path of the element being validated.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateSizeRestrictionModule_ProfileData(ProfileData data, string selectorPath)
        {
            if (data != null)
            {
                // Profile value is required.
                if (string.IsNullOrWhiteSpace(data.Profile))
                {
                    throw new ValidationException(
                                                  ValidationError.ProfileDataProfileNull,
                                                  Properties.Resources.StandardValidator_ProfileNotSpecified,
                                                  selectorPath);
                }
            }
        }

        /// <summary>
        /// Validates the <see cref="Skeleton"/> element conforms to the standard.
        /// </summary>
        /// <param name="skeleton">The <see cref="Skeleton"/> to validate.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateSkeleton(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                bool hasHRef;
                bool hasText;

                // HRef must not be just whitespace.
                this.ValidateIsNotWhitespaceOnly(
                                                 skeleton.HRef,
                                                 "HRef",
                                                 skeleton.SelectableAncestor.SelectorPath,
                                                 ValidationError.SkeletonHRefEmpty);

                // HRef is required if and only if inner text is empty.
                hasHRef = skeleton.HRef != null;
                hasText = (skeleton.NonTranslatableText != null) || ((IExtensible)skeleton).HasExtensions;
                if (hasHRef == hasText)
                {
                    throw new ValidationException(
                                                  ValidationError.SkeletonHasHRefAndTextOrNeither,
                                                  Properties.Resources.StandardValidator_HRefNotSpecified,
                                                  skeleton.SelectableAncestor.SelectorPath);
                }
            }
        }

        /// <summary>
        /// Validates the <see cref="Source"/> element conforms to the standard.
        /// </summary>
        /// <param name="source">The <see cref="Source"/> to validate.</param>
        /// <param name="selectorPath">The selector path to the element that contains the validation error.</param>
        /// <param name="contentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="ResourceString"/>.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateSource(Source source, string selectorPath, Dictionary<string, ResourceStringContent> contentMap)
        {
            if (source == null)
            {
                throw new ValidationException(
                                              ValidationError.SourceNull,
                                              Properties.Resources.StandardValidator_SourceNotSpecified,
                                              selectorPath);
            }

            if (source.Language != null)
            {
                // Language must be a BCP-47 value.
                this.ValidateBcp47Language(
                                           source.Language,
                                           true,
                                           "Language",
                                           ValidationError.SourceLangInvalid,
                                           this.document.SelectorPath);

                // The explicit or inherited value of lang must match the SourceLanguage of the document.
                if (!Utilities.AreLanguagesEqual(source.Language, this.document.SourceLanguage))
                {
                    throw new ValidationException(
                                                  ValidationError.SourceLangMismatch,
                                                  Properties.Resources.StandardValidator_SourceLanguageMismatch,
                                                  source.SelectableAncestor.SelectorPath);
                }
            }

            this.ValidateResourceStringContentContainer(
                                                        source,
                                                        contentMap,
                                                        null,
                                                        this.markedSpanStartToFindSource,
                                                        this.spanningCodeStartToFindSource);
        }

        /// <summary>
        /// Validates all start references refer to valid tags.
        /// </summary>
        /// <param name="referencesToFind">The list of start references to find.</param>
        /// <param name="inlineTags">The list of all inline tags to search in.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateStartReference(
                                            Dictionary<string, MarkedSpanEnd> referencesToFind,
                                            List<XliffElement> inlineTags)
        {
            if (referencesToFind.Count > 0)
            {
                Dictionary<string, MarkedSpanStart> list;

                list = new Dictionary<string, MarkedSpanStart>();
                foreach (XliffElement element in inlineTags)
                {
                    MarkedSpanStart start;

                    start = element as MarkedSpanStart;
                    if (start != null)
                    {
                        list.Add(start.Id, start);
                    }
                }

                foreach (KeyValuePair<string, MarkedSpanEnd> item in referencesToFind)
                {
                    MarkedSpanStart start;

                    if (list.TryGetValue(item.Key, out start))
                    {
                        int startIndex;
                        int endIndex;

                        startIndex = inlineTags.IndexOf(start);
                        endIndex = inlineTags.IndexOf(item.Value);
                        Debug.Assert(
                                     (startIndex >= 0) && (endIndex >= 0),
                                     "Failed to find MarkedSpanStart or MarkedSpanEnd");
                        if (endIndex < startIndex)
                        {
                            string message;

                            message = string.Format(
                                                    Properties.Resources.StandardValidator_StartTagOccursAfterEndTag_Format,
                                                    typeof(MarkedSpanStart).Name,
                                                    typeof(MarkedSpanEnd).Name);
                            throw new ValidationException(
                                                          ValidationError.MarkedSpanStartTagOccursAfterEndTag,
                                                          message,
                                                          item.Key);
                        }
                    }
                    else
                    {
                        string message;

                        message = string.Format(
                                                Properties.Resources.StandardValidator_ElementNotFound_Format,
                                                typeof(MarkedSpanStart).Name,
                                                item.Key);
                        throw new ValidationException(ValidationError.TagStartRefInvalid, message, item.Key);
                    }
                }
            }
        }

        /// <summary>
        /// Validates all start references refer to valid tags.
        /// </summary>
        /// <param name="referencesToFind">The list of start references to find.</param>
        /// <param name="inlineTags">The list of all inline tags to search in.</param>
        /// <returns>A list of <see cref="SpanningCodeStart"/> elements that were not referenced.</returns>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private List<SpanningCodeStart> ValidateStartReference(
                                            Dictionary<string, SpanningCodeEnd> referencesToFind,
                                            List<ISelectable> inlineTags)
        {
            Dictionary<string, SpanningCodeStart> list;
            List<SpanningCodeStart> unreferencedSpanningCodeStarts;

            list = new Dictionary<string, SpanningCodeStart>();
            unreferencedSpanningCodeStarts = new List<SpanningCodeStart>();
            foreach (ISelectable selectable in inlineTags)
            {
                if (selectable is SpanningCodeStart)
                {
                    list.Add(selectable.Id, (SpanningCodeStart)selectable);
                    unreferencedSpanningCodeStarts.Add((SpanningCodeStart)selectable);
                }
            }

            if (referencesToFind.Count > 0)
            {
                foreach (KeyValuePair<string, SpanningCodeEnd> item in referencesToFind)
                {
                    SpanningCodeStart start;

                    if (list.TryGetValue(item.Key, out start))
                    {
                        int startIndex;
                        int endIndex;

                        startIndex = inlineTags.IndexOf(start);
                        endIndex = inlineTags.IndexOf(item.Value);
                        Debug.Assert(
                                     (startIndex >= 0) && (endIndex >= 0),
                                     "Failed to find SpanningCodeStart or SpanningCodEnd");
                        if (endIndex < startIndex)
                        {
                            string message;

                            message = string.Format(
                                                    Properties.Resources.StandardValidator_StartTagOccursAfterEndTag_Format,
                                                    typeof(SpanningCodeStart).Name,
                                                    typeof(SpanningCodeEnd).Name);
                            throw new ValidationException(
                                                          ValidationError.SpanningCodeStartTagOccursAfterEndTag,
                                                          message,
                                                          item.Key);
                        }

                        // The value of CanCopy must be the same between the start and end.
                        if (start.CanCopy != item.Value.CanCopy)
                        {
                            StandardValidator.ThrowSpanningCodeStartEndPropertyMismatchException(
                                                                    "CanCopy",
                                                                    ValidationError.CodeBaseCanCopyMismatch,
                                                                    item.Key);
                        }

                        // The value of CanDelete must be the same between the start and end.
                        if (start.CanDelete != item.Value.CanDelete)
                        {
                            StandardValidator.ThrowSpanningCodeStartEndPropertyMismatchException(
                                                                    "CanDelete",
                                                                    ValidationError.CodeBaseCanDeleteMismatch,
                                                                    item.Key);
                        }

                        // The value of CanReorder must be equivalent between the start and end. The end cannot start
                        // a new sequence so it must be either Yes or No.
                        if (((start.CanReorder == CanReorderValue.Yes) && (item.Value.CanReorder != CanReorderValue.Yes)) ||
                            ((start.CanReorder != CanReorderValue.Yes) && (item.Value.CanReorder != CanReorderValue.No)))
                        {
                            throw new ValidationException(
                                                    ValidationError.CodeBaseCanReorderMismatch,
                                                    Properties.Resources.StandardValidator_StartEndCanReorderMismatch,
                                                    item.Key);
                        }

                        // The value of CanOverlap must be the same between the start and end.
                        if (start.CanOverlap != item.Value.CanOverlap)
                        {
                            StandardValidator.ThrowSpanningCodeStartEndPropertyMismatchException(
                                                                    "CanOverlap",
                                                                    ValidationError.CodeBaseCanOverlapMismatch,
                                                                    item.Key);
                        }

                        // The value of Directionality must be the same between the start and end.
                        if (start.Directionality != item.Value.Directionality)
                        {
                            StandardValidator.ThrowSpanningCodeStartEndPropertyMismatchException(
                                                                    "Directionality",
                                                                    ValidationError.CodeBaseDirectionalityMismatch,
                                                                    item.Key);
                        }

                        // The value of Isolated must be false because a SpanningCodeEnd refers to it.
                        if (start.Isolated)
                        {
                            string message;

                            message = string.Format(
                                        Properties.Resources.StandardValidator_SpanningCodeStartIslatedWithRef_Format,
                                        item.Value.SelectorPath);
                            throw new ValidationException(
                                                    ValidationError.SpanningCodeStartIsolatedWithRef,
                                                    message,
                                                    start.SelectorPath);
                        }

                        // The value of SubType must be the same between the start and end.
                        if (start.SubType != item.Value.SubType)
                        {
                            StandardValidator.ThrowSpanningCodeStartEndPropertyMismatchException(
                                                                    "SubType",
                                                                    ValidationError.CodeBaseSubTypeMismatch,
                                                                    item.Key);
                        }

                        // The value of Type must be the same between the start and end.
                        if (start.Type != item.Value.Type)
                        {
                            StandardValidator.ThrowSpanningCodeStartEndPropertyMismatchException(
                                                                    "Type",
                                                                    ValidationError.CodeBaseTypeMismatch,
                                                                    item.Key);
                        }

                        // The SpanningCodeStart was referenced so remove it from the unreferenced list.
                        unreferencedSpanningCodeStarts.Remove(start);
                    }
                    else
                    {
                        string message;

                        message = string.Format(
                                                Properties.Resources.StandardValidator_ElementNotFound_Format,
                                                typeof(SpanningCodeStart).Name,
                                                item.Key);
                        throw new ValidationException(ValidationError.TagStartRefInvalid, message, item.Key);
                    }
                }
            }

            return unreferencedSpanningCodeStarts;
        }

        /// <summary>
        /// Validates the <see cref="Target"/> element conforms to the standard.
        /// </summary>
        /// <param name="target">The <see cref="Target"/> to validate.</param>
        /// <param name="checkLanguage">True if the target language should be validated, if false the language
        /// is ignored.</param>
        /// <param name="contentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="ResourceString"/>.</param>
        /// <param name="sourceContentMap">A map of Id to the <see cref="ResourceStringContent"/> that all reside under
        /// a <see cref="Source"/>. This may be null if the <paramref name="contentMap"/> refers to the Source elements
        /// as well.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateTarget(
                                    Target target,
                                    bool checkLanguage,
                                    Dictionary<string, ResourceStringContent> contentMap,
                                    Dictionary<string, ResourceStringContent> sourceContentMap)
        {
            if (target != null)
            {
                Unit unit;
                int maxOrder;

                unit = target.FindAncestor<Unit>();
                maxOrder = unit.CollapseChildren<ContainerResource>().Count;

                // Language must be a BCP-47 value.
                this.ValidateBcp47Language(
                                           target.Language,
                                           true,
                                           "Language",
                                           ValidationError.TargetLangInvalid,
                                           this.document.SelectorPath);

                // The explicit or inherited value of lang must match the TargetLanguage of the document.
                if (checkLanguage && !Utilities.AreLanguagesEqual(target.Language, this.document.TargetLanguage))
                {
                    throw new ValidationException(
                                                  ValidationError.TargetLangMismatch,
                                                  Properties.Resources.StandardValidator_TargetLanguageMismatch,
                                                  target.SelectableAncestor.SelectorPath);
                }

                if ((target.Order <= 0) || (target.Order > maxOrder))
                {
                    throw new ValidationException(
                                                  ValidationError.TargetOrderInvalid,
                                                  string.Format(Properties.Resources.StandardValidator_InvalidOrder_Format, maxOrder + 1),
                                                  target.SelectableAncestor.SelectorPath);
                }

                this.ValidateResourceStringContentContainer(
                                                            target,
                                                            contentMap,
                                                            sourceContentMap,
                                                            this.markedSpanStartToFindTarget,
                                                            this.spanningCodeStartToFindTarget);
            }
        }

        /// <summary>
        /// Validates the <see cref="TranslationContainer"/> element conforms to the standard.
        /// </summary>
        /// <param name="container">The <see cref="TranslationContainer"/> to validate.</param>
        /// <remarks>This method assumes the Id is validated to be unique amongst siblings outside this method.
        /// </remarks>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateTranslationContainer(TranslationContainer container)
        {
            Debug.Assert(container != null, "container should not be null.");

            // Id must not be null.
            if (string.IsNullOrWhiteSpace(container.Id))
            {
                throw new ValidationException(
                                              ValidationError.TranslationContainerIdNull,
                                              Properties.Resources.StandardValidator_IdNotSpecified,
                                              container.SelectorPath);
            }
            else
            {
                this.ValidateNMTOKEN(
                                     container.Id,
                                     false,
                                     "Id",
                                     ValidationError.TranslationContainerIdNotNMToken,
                                     container.SelectorPath);
            }

            this.ValidateNotes(container);

            // Type must match format "prefix:value" where prefix must not be "xlf"
            if (container.Type != null)
            {
                this.ValidatePrefixValueFormat(container.Type, "Type", container.SelectorPath);
            }

            if (container is Group)
            {
                this.ValidateGroup((Group)container);
            }
            else
            {
                this.ValidateUnit((Unit)container);
            }

            this.ValidateChangeTrackingModule_ChangeTrack(container.Changes, container.SelectorPath);
            this.ValidateFormatStyles(container, container.SelectorPath);
            this.ValidateMetadata(container, container.SelectorPath);
            this.ValidateSizeRestrictionModule_Attributes(container, container.SelectorPath);
            this.ValidateSizeRestrictionModule_ProfileData(container.ProfileData, container.SelectorPath);
            this.ValidateValidationModule_Validation(container.ValidationRules, container.SelectorPath);
        }

        /// <summary>
        /// Validates the <see cref="Unit"/> element conforms to the standard.
        /// </summary>
        /// <param name="unit">The <see cref="Unit"/> to validate.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateUnit(Unit unit)
        {
            const CollapseScope Scope = CollapseScope.CoreElements | CollapseScope.TopLevelDescendants;
            HashSet<uint> set;
            List<ISelectable> sourceInlineTags;
            List<ISelectable> targetInlineTags;
            List<ISelectable> selectableUnitDescendants;
            List<ISelectable> selectableUnitSourceDescendants;
            List<ISelectable> selectableUnitTargetDescendants;
            List<SpanningCodeStart> unreferenced;
            List<XliffElement> sourceMarkedSpansStartAndEnd;
            List<XliffElement> targetMarkedSpansStartAndEnd;
            Type[] markedSpanTypes;
            bool hasSegment;
            uint resourceIndex;

            Debug.Assert(unit != null, "unit should not be null.");

            set = new HashSet<uint>();
            this.markedSpanStartToFindSource.Clear();
            this.markedSpanStartToFindTarget.Clear();
            this.spanningCodeStartToFindSource.Clear();
            this.spanningCodeStartToFindTarget.Clear();

            // There must be at least one Segment or Ignorable.
            if (unit.Resources.Count < 1)
            {
                string message;

                message = string.Format(Properties.Resources.StandardValidator_NoElements_Format, "Resources");
                throw new ValidationException(ValidationError.UnitMissingResource, message, unit.SelectorPath);
            }

            markedSpanTypes = new[] { typeof(MarkedSpanStart), typeof(MarkedSpanEnd) };

            // Id from Ignorable, Segment, and all inline tags must be unique within the unit.
            sourceMarkedSpansStartAndEnd = unit.CollapseChildren(markedSpanTypes, e => !(e is Target));
            sourceInlineTags = Utilities.GetAllSelectableInlineTags(unit, true);
            selectableUnitSourceDescendants = new List<ISelectable>(sourceInlineTags);
            selectableUnitSourceDescendants.AddRange(unit.Resources);
            this.ValidateIds(selectableUnitSourceDescendants, true);

            targetMarkedSpansStartAndEnd = unit.CollapseChildren(markedSpanTypes, e => !(e is Source));
            targetInlineTags = Utilities.GetAllSelectableInlineTags(unit, false);
            selectableUnitTargetDescendants = new List<ISelectable>(targetInlineTags);
            selectableUnitTargetDescendants.AddRange(unit.Resources);
            this.ValidateIds(selectableUnitTargetDescendants, true);

            hasSegment = false;
            resourceIndex = 0;
            foreach (ContainerResource container in unit.Resources)
            {
                uint orderNumber;

                hasSegment |= container is Segment;

                // Order is 1 based, not 0 based.
                resourceIndex++;
                this.ValidateContainerResource(container);

                // Target.Order must be unique among siblings. If the order is not explicitly set, then its order
                // is implied by the order under the parent.
                if ((container.Target != null) && container.Target.Order.HasValue)
                {
                    orderNumber = container.Target.Order.Value;
                }
                else
                {
                    orderNumber = resourceIndex;
                }

                if (set.Contains(orderNumber))
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.StandardValidator_DuplicateOrder_Format,
                                            orderNumber);
                    throw new ValidationException(
                                                  ValidationError.TargetOrderDuplicate,
                                                  message,
                                                  container.SelectableAncestor.SelectorPath);
                }

                set.Add(orderNumber);
            }

            // Match up the MarkedSpanEnds.
            this.ValidateStartReference(this.markedSpanStartToFindSource, sourceMarkedSpansStartAndEnd);
            this.ValidateStartReference(this.markedSpanStartToFindTarget, targetMarkedSpansStartAndEnd);

            // Match up the SpanningCodeEnds across the whole unit. Don't look at Match elements.
            sourceInlineTags = Utilities.CollapseChildren<ISelectable>(
                                    Utilities.CollapseChildren<Source>(
                                        unit.CollapseChildren<ContainerResource>(Scope),
                                        Scope));

            unreferenced = this.ValidateStartReference(this.spanningCodeStartToFindSource, sourceInlineTags);
            this.ValidateIsolated(unreferenced);

            targetInlineTags = Utilities.CollapseChildren<ISelectable>(
                                    Utilities.CollapseChildren<Target>(
                                        unit.CollapseChildren<ContainerResource>(Scope),
                                        Scope));

            unreferenced = this.ValidateStartReference(this.spanningCodeStartToFindTarget, targetInlineTags);
            this.ValidateIsolated(unreferenced);

            // There must be at least one segment.
            if (!hasSegment)
            {
                string message;

                message = string.Format(Properties.Resources.StandardValidator_NoElements_Format, "Segment");
                throw new ValidationException(ValidationError.UnitMissingSegment, message, unit.SelectorPath);
            }

            if (unit.HasMatches)
            {
                List<ISelectable> textElements;

                textElements = Utilities.ToISelectableList(unit.CollapseChildren<ContainerResource>());
                textElements.AddRange(sourceInlineTags);
                textElements.AddRange(targetInlineTags);
                this.ValidateIds(unit.Matches, true, unit.SelectorPath);
                foreach (Match match in unit.Matches)
                {
                    this.ValidateMatch(match, textElements, unit.SelectorPath);
                }
            }

            selectableUnitDescendants = new List<ISelectable>(selectableUnitSourceDescendants);
            selectableUnitDescendants.AddRange(selectableUnitTargetDescendants);
            this.ValidateGlossary(unit.Glossary, selectableUnitDescendants, unit.SelectorPath);
            this.ValidateOriginalData(unit.OriginalData);
            this.ValidateResourceDataModule_ResourceData(unit.ResourceData, unit.SelectorPath);
        }

        /// <summary>
        /// Validates the Validation element from the Validation module conforms to the standard.
        /// </summary>
        /// <param name="validation">The Validation to validate. If this value is null, no validation takes place.
        /// </param>
        /// <param name="selectorPath">The selector path of the element being validated.</param>
        /// <exception cref="ValidationException">Exception thrown if a validation error occurs.</exception>
        private void ValidateValidationModule_Validation(Validation validation, string selectorPath)
        {
            if (validation != null)
            {
                // Validation element must have one or more rules.
                if (validation.Rules.Count < 1)
                {
                    string message;

                    message = string.Format(Properties.Resources.StandardValidator_NoElements_Format, "Rules");
                    throw new ValidationException(ValidationError.ValidationMissingRules, message, selectorPath);
                }

                foreach (Rule rule in validation.Rules)
                {
                    int count;

                    // Order must be a number 1 or greater.
                    if (rule.Occurs.HasValue && (rule.Occurs < 1))
                    {
                        string message;

                        message = string.Format(Properties.Resources.StandardValidator_ValueMustBeNOrGreater_Format, "Occurs", 1);
                        throw new ValidationException(ValidationError.RuleInvalidOccurs, message, selectorPath);
                    }

                    // The rule must have exactly one of IsPresent, IsNotPresent, StartsWith, EndsWith, or a custom rule
                    // defined by attributes from any namespace.
                    count = 0;
                    count += (rule.IsPresent == null) ? 0 : 1;
                    count += (rule.IsNotPresent == null) ? 0 : 1;
                    count += (rule.StartsWith == null) ? 0 : 1;
                    count += (rule.EndsWith == null) ? 0 : 1;
                    count += Utilities.HasExtensionAttributes(rule) ? 1 : 0;

                    if (count != 1)
                    {
                        throw new ValidationException(
                                                      ValidationError.RuleInvalidDefinition,
                                                      Properties.Resources.StandardValidator_RuleDefinitionInvalid,
                                                      selectorPath);
                    }

                    if (rule.ExistsInSource)
                    {
                        // The rule must have exactly one of IsPresent, StartsWith, EndsWith.
                        count = 0;
                        count += (rule.IsPresent == null) ? 0 : 1;
                        count += (rule.StartsWith == null) ? 0 : 1;
                        count += (rule.EndsWith == null) ? 0 : 1;

                        if (count != 1)
                        {
                            throw new ValidationException(
                                                      ValidationError.RuleInvalidExistsInSource,
                                                      Properties.Resources.StandardValidator_RuleExistsInSourceInvalid,
                                                      selectorPath);
                        }
                    }
                }
            }
        }
        #endregion Methods
    }
}
