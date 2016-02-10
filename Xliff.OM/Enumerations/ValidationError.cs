namespace Localization.Xliff.OM.Validators
{
    /// <summary>
    /// Defines error numbers that identify specific validation errors. 
    /// </summary>
    internal static class ValidationError
    {
        /// <summary>
        /// A base value from which all other values extend.
        /// </summary>
        internal const int BaseValue = 20000;

        /// <summary>
        /// The last used offset from the ValidationError.BaseValue that was used. This value should be incremented each time a new
        /// error number is added.
        /// </summary>
        internal const int LastUsedOffset = 159;

        /// <summary>
        /// Indicates a ChangeTrack does not contain any RevisionContainers.
        /// </summary>
        internal const int ChangeTrackMissingRevisions = ValidationError.BaseValue + 138;

        /// <summary>
        /// Indicates a CodeBase CanCopy value on the start inline tag doesn't match the corresponding end.
        /// </summary>
        internal const int CodeBaseCanCopyMismatch = ValidationError.BaseValue + 1;

        /// <summary>
        /// Indicates a CodeBase CanDelete value on the start inline tag doesn't match the corresponding end.
        /// </summary>
        internal const int CodeBaseCanDeleteMismatch = ValidationError.BaseValue + 2;

        /// <summary>
        /// Indicates a CodeBase CanOverlap value on the start inline tag doesn't match the corresponding end.
        /// </summary>
        internal const int CodeBaseCanOverlapMismatch = ValidationError.BaseValue + 3;
            
        /// <summary>
        /// Indicates a CodeBase CanReorder value on the start inline tag doesn't match the corresponding end.
        /// </summary>
        internal const int CodeBaseCanReorderMismatch = ValidationError.BaseValue + 151;

        /// <summary>
        /// Indicates a CodeBase CopyOf value refers to an item of a different type.
        /// </summary>
        internal const int CodeBaseCopyOfTypeMismatch = ValidationError.BaseValue + 4;

        /// <summary>
        /// Indicates a CodeBase Directionality value on the start inline tag doesn't match the corresponding end.
        /// </summary>
        internal const int CodeBaseDirectionalityMismatch = ValidationError.BaseValue + 10;

        /// <summary>
        /// Indicates a CodeBase Id is not an NMTOKEN.
        /// </summary>
        internal const int CodeBaseIdNotNMToken = ValidationError.BaseValue + 5;

        /// <summary>
        /// Indicates a CodeBase Id is null.
        /// </summary>
        internal const int CodeBaseIdNull = ValidationError.BaseValue + 6;

        /// <summary>
        /// Indicates a CodeBase CopyOf refers to an invalid element.
        /// </summary>
        internal const int CodeBaseInvalidCopyOf = ValidationError.BaseValue + 7;

        /// <summary>
        /// Indicates an inline tag sequence in the source has an invalid hierarchy.
        /// </summary>
        internal const int CodeBaseInvalidSourceSequenceHierarchy = ValidationError.BaseValue + 112;

        /// <summary>
        /// Indicates an inline tag sequence in the target has an invalid hierarchy.
        /// </summary>
        internal const int CodeBaseInvalidTargetSequenceHierarchy = ValidationError.BaseValue + 113;

        /// <summary>
        /// Indicates a CodeBase CanCopy or CanDelete is set to true although CanReorder not set to Yes.
        /// </summary>
        internal const int CodeBaseMismatchedCanReorderCopyDelete = ValidationError.BaseValue + 8;

        /// <summary>
        /// Indicates the hierarchy of an inline tag sequence in the target doesn't match the hierarchy of the source.
        /// </summary>
        internal const int CodeBaseSequenceHierarchyMismatch = ValidationError.BaseValue + 114;

        /// <summary>
        /// Indicates a inline tag sequence is out of order.
        /// </summary>
        internal const int CodeBaseSequenceMismatch = ValidationError.BaseValue + 9;

        /// <summary>
        /// Indicates a inline tag sequence cannot be found.
        /// </summary>
        internal const int CodeBaseSequenceNotFound = ValidationError.BaseValue + 103;

        /// <summary>
        /// Indicates a inline tag sequence begins with an element whose CanReorder is set to FirstNo.
        /// </summary>
        internal const int CodeBaseSequenceStartsWithCanReorderNo = ValidationError.BaseValue + 11;

        /// <summary>
        /// Indicates a CodeBase CopyOf in a Source refers to an element of a different type, or that CanCopy is set to
        /// false.
        /// </summary>
        internal const int CodeBaseSourceCopyOfTypeMismatchOrNotCanCopy = ValidationError.BaseValue + 12;

        /// <summary>
        /// Indicates a CodeBase SubType value is invalid.
        /// </summary>
        internal const int CodeBaseSubTypeInvalid = ValidationError.BaseValue + 13;

        /// <summary>
        /// Indicates a CodeBase SubType value on the start inline tag doesn't match the corresponding end.
        /// </summary>
        internal const int CodeBaseSubTypeMismatch = ValidationError.BaseValue + 104;

        /// <summary>
        /// Indicates a CodeBase SubType value is incorrect for a Formatting Type.
        /// </summary>
        internal const int CodeBaseSubTypeMismatchFormatting = ValidationError.BaseValue + 14;

        /// <summary>
        /// Indicates a CodeBase SubType value is incorrect for a Variable Type.
        /// </summary>
        internal const int CodeBaseSubTypeMismatchUserInterface = ValidationError.BaseValue + 15;

        /// <summary>
        /// Indicates a CodeBase was deleted in the target even though the source had CanDelete set to false.
        /// </summary>
        internal const int CodeBaseTagDeleted = ValidationError.BaseValue + 16;

        /// <summary>
        /// Indicates a CodeBase CopyOf in a target refers to an element of a different type, or that CanCopy is set to
        /// false.
        /// </summary>
        internal const int CodeBaseTargetCopyOfTypeMismatchOrNotCanCopy = ValidationError.BaseValue + 17;

        /// <summary>
        /// Indicates a CodeBase Type value on the start inline tag doesn't match the corresponding end.
        /// </summary>
        internal const int CodeBaseTypeMismatch = ValidationError.BaseValue + 105;

        /// <summary>
        /// Indicates a CodeBase Type was not specified.
        /// </summary>
        internal const int CodeBaseTypeNotSpecified = ValidationError.BaseValue + 18;

        /// <summary>
        /// Indicates a CodeBase contains both a CopyOf reference and a DataReference.
        /// </summary>
        internal const int CodeBaseWithCopyOfAndDataRef = ValidationError.BaseValue + 111;

        /// <summary>
        /// Indicates the CodePoint Code value is invalid.
        /// </summary>
        internal const int CodePointInvalidCode = ValidationError.BaseValue + 19;

        /// <summary>
        /// Indicates a ContainerResource Id is not an NMTOKEN.
        /// </summary>
        internal const int ContainerResourceIdNotNMToken = ValidationError.BaseValue + 20;

        /// <summary>
        /// Indicates a ContainerResource contains a target inline tag whose corresponding source inline tag is of a
        /// different type.
        /// </summary>
        internal const int ContainerResourceTypesWithSameIdMismatch = ValidationError.BaseValue + 21;

        /// <summary>
        /// Indicates a Data Id is not an NMTOKEN.
        /// </summary>
        internal const int DataIdNotNMToken = ValidationError.BaseValue + 22;

        /// <summary>
        /// Indicates a Data Id is null.
        /// </summary>
        internal const int DataIdNull = ValidationError.BaseValue + 23;

        /// <summary>
        /// Indicates a Data Space is not set to Preserve.
        /// </summary>
        internal const int DataSpaceNotPreserve = ValidationError.BaseValue + 24;

        /// <summary>
        /// Indicates a XliffDocument does not contain any Files.
        /// </summary>
        internal const int DocumentMissingFile = ValidationError.BaseValue + 25;

        /// <summary>
        /// Indicates a XliffDocument doesn't specify a target language.
        /// </summary>
        internal const int DocumentMissingTargetLang = ValidationError.BaseValue + 26;

        /// <summary>
        /// Indicates a XliffDocument SourceLanguage is invalid.
        /// </summary>
        internal const int DocumentSourceLangInvalid = ValidationError.BaseValue + 107;

        /// <summary>
        /// Indicates a XliffDocument doesn't specify a source language.
        /// </summary>
        internal const int DocumentSourceLangNull = ValidationError.BaseValue + 27;

        /// <summary>
        /// Indicates a XliffDocument TargetLanguage is invalid.
        /// </summary>
        internal const int DocumentTargetLangInvalid = ValidationError.BaseValue + 108;

        /// <summary>
        /// Indicates a XliffDocument doesn't specify a version.
        /// </summary>
        internal const int DocumentVersionNull = ValidationError.BaseValue + 28;

        /// <summary>
        /// Indicates a XliffElement Id is a duplicate.
        /// </summary>
        internal const int ElementIdDuplicate = ValidationError.BaseValue + 29;

        /// <summary>
        /// Indicates a XliffElement Id contains only whitespace.
        /// </summary>
        internal const int ElementIdEmpty = ValidationError.BaseValue + 30;

        /// <summary>
        /// Indicates a EquivalentStorage from the SizeRestriction module is specified for a SpanningCodeEnd whose
        /// Isolated value is set to false.
        /// </summary>
        internal const int EquivalentStorageWithSpanEndNotIsolated = ValidationError.BaseValue + 145;

        /// <summary>
        /// Indicates an extension contains an element whose Id is a duplicate among siblings.
        /// </summary>
        internal const int ExtensionIdDuplicate = ValidationError.BaseValue + 115;

        /// <summary>
        /// Indicates a File Id is not an NMTOKEN.
        /// </summary>
        internal const int FileIdNotNMToken = ValidationError.BaseValue + 31;

        /// <summary>
        /// Indicates a File Id is null.
        /// </summary>
        internal const int FileIdNull = ValidationError.BaseValue + 32;

        /// <summary>
        /// Indicates a File does not contain any TranslationContainers.
        /// </summary>
        internal const int FileMissingContainer = ValidationError.BaseValue + 33;

        /// <summary>
        /// Indicates a format style is specified for a SpanningCodeEnd whose Isolated value is set to false.
        /// </summary>
        internal const int FormatStyleWithSpanEndNotIsolated = ValidationError.BaseValue + 116;

        /// <summary>
        /// Indicates a sub-format style is used when no format style is present.
        /// </summary>
        internal const int FormatStyleSubFormatWithoutFormat = ValidationError.BaseValue + 117;

        /// <summary>
        /// Indicates a GlossaryEntry does not contain any Translations or Definition.
        /// </summary>
        internal const int GlossaryEntryChildrenMissing = ValidationError.BaseValue + 34;

        /// <summary>
        /// Indicates a GlossaryEntry Id is a duplicate.
        /// </summary>
        internal const int GlossaryEntryIdDuplicate = ValidationError.BaseValue + 35;

        /// <summary>
        /// Indicates a GlossaryEntry Id is not an NMTOKEN.
        /// </summary>
        internal const int GlossaryEntryIdNotNMToken = ValidationError.BaseValue + 36;

        /// <summary>
        /// Indicates a Glossary does not contain any GlossaryEntries.
        /// </summary>
        internal const int GlossaryMissingEntry = ValidationError.BaseValue + 37;

        /// <summary>
        /// Indicates a GlossaryTranslation Id is a duplicate.
        /// </summary>
        internal const int GlossaryTranslationIdDuplicate = ValidationError.BaseValue + 38;

        /// <summary>
        /// Indicates a GlossaryTranslation Id is not an NMTOKEN.
        /// </summary>
        internal const int GlossaryTranslationIdNotNMToken = ValidationError.BaseValue + 39;

        /// <summary>
        /// Indicates an IRI is invalid.
        /// </summary>
        internal const int IriInvalid = ValidationError.BaseValue + 40;

        /// <summary>
        /// Indicates the Property value on the Item from the ChangeTrack module is null or empty.
        /// </summary>
        internal const int ItemPropertyNull = ValidationError.BaseValue + 152;

        /// <summary>
        /// Indicates a MarkedSpanEnd has a StartReference that is already referred to by another inline tag.
        /// </summary>
        internal const int MarkedSpanEndDuplicateStartRef = ValidationError.BaseValue + 41;

        /// <summary>
        /// Indicates a MarkedSpanEnd StartReference is null.
        /// </summary>
        internal const int MarkedSpanEndStartRefNull = ValidationError.BaseValue + 42;

        /// <summary>
        /// Indicates a MarkedSpan Id is not an NMTOKEN.
        /// </summary>
        internal const int MarkedSpanIdNotNMToken = ValidationError.BaseValue + 43;

        /// <summary>
        /// Indicates a MarkedSpan Id is null.
        /// </summary>
        internal const int MarkedSpanIdNull = ValidationError.BaseValue + 44;

        /// <summary>
        /// Indicates a MarkedSpan Reference is invalid.
        /// </summary>
        internal const int MarkedSpanInvalidReference = ValidationError.BaseValue + 45;

        /// <summary>
        /// Indicates a MarkedSpan Reference is not a well-formed selector path.
        /// </summary>
        internal const int MarkedSpanInvalidReferenceSelectorPath = ValidationError.BaseValue + 150;

        /// <summary>
        /// Indicates a MarkedSpan has an invalid value for Type.
        /// </summary>
        internal const int MarkedSpanInvalidType = ValidationError.BaseValue + 46;

        /// <summary>
        /// Indicates a MarkedSpan has a Reference and Value, or neither.
        /// </summary>
        internal const int MarkedSpanReferenceAndValueSpecified = ValidationError.BaseValue + 47;

        /// <summary>
        /// Indicates a MarkedSpanStart Id is not an NMTOKEN.
        /// </summary>
        internal const int MarkedSpanStartIdNotNMToken = ValidationError.BaseValue + 48;

        /// <summary>
        /// Indicates a MarkedSpanStart Id is null.
        /// </summary>
        internal const int MarkedSpanStartIdNull = ValidationError.BaseValue + 49;

        /// <summary>
        /// Indicates a MarkedSpanStart Reference is invalid.
        /// </summary>
        internal const int MarkedSpanStartInvalidReference = ValidationError.BaseValue + 50;

        /// <summary>
        /// Indicates a MarkedSpan Reference is not a well-formed selector path.
        /// </summary>
        internal const int MarkedSpanStartInvalidReferenceSelectorPath = ValidationError.BaseValue + 159;

        /// <summary>
        /// Indicates a MarkedSpanStart has an invalid value for Type.
        /// </summary>
        internal const int MarkedSpanStartInvalidType = ValidationError.BaseValue + 51;

        /// <summary>
        /// Indicates a MarkedSpanStart has a Reference and Value, or neither.
        /// </summary>
        internal const int MarkedSpanStartReferenceAndValueSpecified = ValidationError.BaseValue + 52;

        /// <summary>
        /// Indicates a MarkedSpanStart occurs after the corresponding MarkedSpanEnd.
        /// </summary>
        internal const int MarkedSpanStartTagOccursAfterEndTag = ValidationError.BaseValue + 53;

        /// <summary>
        /// Indicates a Match Id is not an NMTOKEN.
        /// </summary>
        internal const int MatchIdNotNMToken = ValidationError.BaseValue + 54;

        /// <summary>
        /// Indicates a Match is missing a SourceReference.
        /// </summary>
        internal const int MatchMissingSourceRef = ValidationError.BaseValue + 55;

        /// <summary>
        /// Indicates a Match MatchQuality value is out of the allowable range.
        /// </summary>
        internal const int MatchQualityNotInRange = ValidationError.BaseValue + 56;

        /// <summary>
        /// Indicates a Match Similarity value is out of the allowable range.
        /// </summary>
        internal const int MatchSimilarityNotInRange = ValidationError.BaseValue + 57;

        /// <summary>
        /// Indicates a Match SourceReference is null.
        /// </summary>
        internal const int MatchSourceRefNull = ValidationError.BaseValue + 58;

        /// <summary>
        /// Indicates a Match MatchSuitability value is out of the allowable range.
        /// </summary>
        internal const int MatchSuitabilityNotInRange = ValidationError.BaseValue + 59;

        /// <summary>
        /// Indicates a Match Target is null.
        /// </summary>
        internal const int MatchTargetNull = ValidationError.BaseValue + 60;

        /// <summary>
        /// Indicates a Metadata Id is a duplicate.
        /// </summary>
        internal const int MetadataIdDuplicate = ValidationError.BaseValue + 61;

        /// <summary>
        /// Indicates a Metadata Id is not an NMTOKEN.
        /// </summary>
        internal const int MetadataIdNotNMToken = ValidationError.BaseValue + 62;

        /// <summary>
        /// Indicates a Metadata does not contain any MetaGroups.
        /// </summary>
        internal const int MetadataMissingGroup = ValidationError.BaseValue + 63;

        /// <summary>
        /// Indicates a MetaGroup Id is not an NMTOKEN.
        /// </summary>
        internal const int MetaGroupIdNotNMToken = ValidationError.BaseValue + 64;

        /// <summary>
        /// Indicates a MetaGroup does not contain any MetaElements.
        /// </summary>
        internal const int MetaGroupMissingContainer = ValidationError.BaseValue + 65;

        /// <summary>
        /// Indicates a Meta Type value is null.
        /// </summary>
        internal const int MetaTypeNull = ValidationError.BaseValue + 66;

        /// <summary>
        /// Indicates a ResourceData module ResourceItem MimeType is not specified.
        /// </summary>
        internal const int MimeTypeNotSpecified = ValidationError.BaseValue + 122;

        /// <summary>
        /// Indicates a Note Id is not an NMTOKEN.
        /// </summary>
        internal const int NoteIdNotNMToken = ValidationError.BaseValue + 67;

        /// <summary>
        /// Indicates a Note Priority is invalid.
        /// </summary>
        internal const int NoteInvalidPriority = ValidationError.BaseValue + 68;

        /// <summary>
        /// Indicates Notes contains no Note elements.
        /// </summary>
        internal const int NotesMissingNote = ValidationError.BaseValue + 69;

        /// <summary>
        /// Indicates the value in text of the form prefix:value is empty.
        /// </summary>
        internal const int PrefixValueEmpty = ValidationError.BaseValue + 70;

        /// <summary>
        /// Indicates the prefix in text of the form prefix:value is empty.
        /// </summary>
        internal const int PrefixValueInvalid = ValidationError.BaseValue + 71;

        /// <summary>
        /// Indicates the text of the form prefix:value is missing the colon.
        /// </summary>
        internal const int PrefixValueMissingColon = ValidationError.BaseValue + 72;

        /// <summary>
        /// Indicates the Profile from ProfileData in the SizeRestriction module is null or empty string.
        /// </summary>
        internal const int ProfileDataProfileNull = ValidationError.BaseValue + 148;

        /// <summary>
        /// Indicates a ResourceData module Reference HRef is not specified.
        /// </summary>
        internal const int ReferenceHRefNotSpecified = ValidationError.BaseValue + 126;

        /// <summary>
        /// Indicates a ResourceData module Reference Language is invalid.
        /// </summary>
        internal const int ReferenceLangInvalid = ValidationError.BaseValue + 127;

        /// <summary>
        /// Indicates a ResourceData module ResourceData doesn't contain any ResourceItems or ResourceItemRefs.
        /// </summary>
        internal const int ResourceDataMissingItems = ValidationError.BaseValue + 153;

        /// <summary>
        /// Indicates a ResourceData module ResourceItem Id is a duplicate.
        /// </summary>
        internal const int ResourceItemIdDuplicate = ValidationError.BaseValue + 121;

        /// <summary>
        /// Indicates a ResourceData module ResourceItem Id is not an NMTOKEN.
        /// </summary>
        internal const int ResourceItemIdNotNMToken = ValidationError.BaseValue + 118;

        /// <summary>
        /// Indicates a ResourceData module ResourceItem doesn't contain a Source, Target, or Reference.
        /// </summary>
        internal const int ResourceItemMissingChildren = ValidationError.BaseValue + 154;

        /// <summary>
        /// Indicates a ResourceData module Source or Target contains an HRef value and is not empty, or does not contain
        /// an HRef value and is empty.
        /// </summary>
        internal const int ResourceItemReferenceBaseHRefAndSubject = ValidationError.BaseValue + 156;

        /// <summary>
        /// Indicates a ResourceData module ResourceItemRef Id is a duplicate.
        /// </summary>
        internal const int ResourceItemRefIdDuplicate = ValidationError.BaseValue + 123;

        /// <summary>
        /// Indicates a ResourceData module ResourceItemRef Id is not an NMTOKEN.
        /// </summary>
        internal const int ResourceItemRefIdNotNMToken = ValidationError.BaseValue + 125;

        /// <summary>
        /// Indicates a ResourceData module ResourceItemRef Reference is invalid.
        /// </summary>
        internal const int ResourceItemRefInvalidReference = ValidationError.BaseValue + 124;

        /// <summary>
        /// Indicates a ResourceData module ResourceItem Source HRef is not specified when it should be.
        /// </summary>
        internal const int ResourceItemSourceHRefNotSpecified = ValidationError.BaseValue + 128;

        /// <summary>
        /// Indicates a ResourceData module ResourceItem Source HRef is specified when it shouldn't be.
        /// </summary>
        internal const int ResourceItemSourceHRefSpecified = ValidationError.BaseValue + 129;

        /// <summary>
        /// Indicates a ResourceData module ResourceItem Source Language is invalid.
        /// </summary>
        internal const int ResourceItemSourceLangInvalid = ValidationError.BaseValue + 119;

        /// <summary>
        /// Indicates a ResourceData module ResourceItem Source Language doesn't match the document source language.
        /// </summary>
        internal const int ResourceItemSourceLangMismatch = ValidationError.BaseValue + 130;

        /// <summary>
        /// Indicates a ResourceData module ResourceItem Source is missing when Context is set to false.
        /// </summary>
        internal const int ResourceItemSourceMissingWithNoContext = ValidationError.BaseValue + 155;

        /// <summary>
        /// Indicates a ResourceData module ResourceItem Target HRef is not specified when it should be.
        /// </summary>
        internal const int ResourceItemTargetHRefNotSpecified = ValidationError.BaseValue + 131;

        /// <summary>
        /// Indicates a ResourceData module ResourceItem Target HRef is specified when it should not be.
        /// </summary>
        internal const int ResourceItemTargetHRefSpecified = ValidationError.BaseValue + 132;

        /// <summary>
        /// Indicates a ResourceData module ResourceItem Target Language is invalid.
        /// </summary>
        internal const int ResourceItemTargetLangInvalid = ValidationError.BaseValue + 120;

        /// <summary>
        /// Indicates a ResourceData module ResourceItem Target Language doesn't match the document target language.
        /// </summary>
        internal const int ResourceItemTargetLangMismatch = ValidationError.BaseValue + 137;

        /// <summary>
        /// Indicates the Space value of source and target don't match.
        /// </summary>
        internal const int ResourceStringSpaceMismatch = ValidationError.BaseValue + 73;

        /// <summary>
        /// Indicates a ResourceStringContent is invalid.
        /// </summary>
        internal const int ResourceStringContentInvalid = ValidationError.BaseValue + 74;

        /// <summary>
        /// Indicates a Revision does not contain any Items.
        /// </summary>
        internal const int RevisionMissingItems = ValidationError.BaseValue + 139;

        /// <summary>
        /// Indicates the AppliesTo of a RevisionsContainer is not an NMTOKEN.
        /// </summary>
        internal const int RevisionsContainerAppliesToNotNMToken = ValidationError.BaseValue + 140;

        /// <summary>
        /// Indicates the CurrentVersion of a RevisionsContainer is not an NMTOKEN.
        /// </summary>
        internal const int RevisionsContainerCurrentVersionNotNMToken = ValidationError.BaseValue + 141;

        /// <summary>
        /// Indicates a RevisionsContainer does not contain any Revisions.
        /// </summary>
        internal const int RevisionsContainerMissingRevisions = ValidationError.BaseValue + 143;

        /// <summary>
        /// Indicates the Reference of a RevisionsContainer is not an NMTOKEN.
        /// </summary>
        internal const int RevisionsContainerReferenceNotNMToken = ValidationError.BaseValue + 142;

        /// <summary>
        /// Indicates the Version of a Revision is not an NMTOKEN.
        /// </summary>
        internal const int RevisionVersionNotNMToken = ValidationError.BaseValue + 144;

        /// <summary>
        /// Indicates a Validation module Rule has an invalid definition.
        /// </summary>
        internal const int RuleInvalidDefinition = ValidationError.BaseValue + 133;

        /// <summary>
        /// Indicates a Validation module Rule has an invalid definition when ExistsInSource is true.
        /// </summary>
        internal const int RuleInvalidExistsInSource = ValidationError.BaseValue + 134;

        /// <summary>
        /// Indicates a Validation module Rule has an invalid value for Occurs.
        /// </summary>
        internal const int RuleInvalidOccurs = ValidationError.BaseValue + 135;

        /// <summary>
        /// Indicates that SizeInfo and SizeInfoReference from the SizeRestriction module are either both present or
        /// neither are present.
        /// </summary>
        internal const int SameSizeInfoAndSizeInfoReferencePresence = ValidationError.BaseValue + 149;

        /// <summary>
        /// Indicates a SizeInfo from the SizeRestriction module is specified for a SpanningCodeEnd whose Isolated
        /// value is set to false.
        /// </summary>
        internal const int SizeInfoWithSpanEndNotIsolated = ValidationError.BaseValue + 146;

        /// <summary>
        /// Indicates a SizeInfoReference from the SizeRestriction module is specified for a SpanningCodeEnd whose
        /// Isolated value is set to false.
        /// </summary>
        internal const int SizeInfoRefWithSpanEndNotIsolated = ValidationError.BaseValue + 147;

        /// <summary>
        /// Indicates a Skeleton has both <see cref="Localization.Xliff.OM.Core.Skeleton.HRef"/> and Text or
        /// neither of them.
        /// </summary>
        internal const int SkeletonHasHRefAndTextOrNeither = ValidationError.BaseValue + 75;

        /// <summary>
        /// Indicates a Skeleton <see cref="Localization.Xliff.OM.Core.Skeleton.HRef"/> is empty.
        /// </summary>
        internal const int SkeletonHRefEmpty = ValidationError.BaseValue + 76;

        /// <summary>
        /// Indicates the language on a Source is invalid.
        /// </summary>
        internal const int SourceLangInvalid = ValidationError.BaseValue + 109;

        /// <summary>
        /// Indicates the language on a Source doesn't match the document SourceLanguage.
        /// </summary>
        internal const int SourceLangMismatch = ValidationError.BaseValue + 77;

        /// <summary>
        /// Indicates a Source is null.
        /// </summary>
        internal const int SourceNull = ValidationError.BaseValue + 78;

        /// <summary>
        /// Indicates a SpanningCodeEnd has a null Id.
        /// </summary>
        internal const int SpanningCodeEndIdNull = ValidationError.BaseValue + 158;

        /// <summary>
        /// Indicates a SpanningCodeEnd has an invalid DataReference.
        /// </summary>
        internal const int SpanningCodeEndInvalidDataRef = ValidationError.BaseValue + 79;

        /// <summary>
        /// Indicates a SpanningCodeEnd has Isolated set to true and also has a StartReference value.
        /// </summary>
        internal const int SpanningCodeEndIsolatedWithStartRef = ValidationError.BaseValue + 80;

        /// <summary>
        /// Indicates a SpanningCodeEnd has Isolated set to false and does not have a StartReference value.
        /// </summary>
        internal const int SpanningCodeEndNotIsolatedOrStartRef = ValidationError.BaseValue + 81;

        /// <summary>
        /// Indicates a SpanningCodeEnd has an invalid StartReference.
        /// </summary>
        internal const int SpanningCodeEndStartRefInvalid = ValidationError.BaseValue + 82;

        /// <summary>
        /// Indicates a SpanningCodeEnd has a value for StartReference and for Id.
        /// </summary>
        internal const int SpanningCodeEndStartRefAndIdSpecified = ValidationError.BaseValue + 157;

        /// <summary>
        /// Indicates a SpanningCodeEnd has an invalid SubFlows value.
        /// </summary>
        internal const int SpanningCodeEndSubFlowsInvalid = ValidationError.BaseValue + 83;

        /// <summary>
        /// Indicates a SpanningCode has an invalid DataReferenceEnd.
        /// </summary>
        internal const int SpanningCodeInvalidDataRefEnd = ValidationError.BaseValue + 84;

        /// <summary>
        /// Indicates a SpanningCode has an invalid DataReferenceStart.
        /// </summary>
        internal const int SpanningCodeInvalidDataRefStart = ValidationError.BaseValue + 85;

        /// <summary>
        /// Indicates a SpanningCode has an invalid SubFlowsEnd value.
        /// </summary>
        internal const int SpanningCodeInvalidSubFlowsEnd = ValidationError.BaseValue + 86;

        /// <summary>
        /// Indicates a SpanningCode has an invalid SubFlowsStart value.
        /// </summary>
        internal const int SpanningCodeInvalidSubFlowsStart = ValidationError.BaseValue + 87;

        /// <summary>
        /// Indicates a SpanningCodeStart has an invalid DataReference.
        /// </summary>
        internal const int SpanningCodeStartDataRefInvalid = ValidationError.BaseValue + 88;

        /// <summary>
        /// Indicates a SpanningCodeStart has Isolated set to true while a SpanningCodeEnd has a reference to it.
        /// </summary>
        internal const int SpanningCodeStartIsolatedWithRef = ValidationError.BaseValue + 89;

        /// <summary>
        /// Indicates a SpanningCodeStart has Isolated set to false and does not have a corresponding SpanningCodeEnd.
        /// </summary>
        internal const int SpanningCodeStartNotIsolated = ValidationError.BaseValue + 106;

        /// <summary>
        /// Indicates a SpanningCodeStart has an invalid SubFlows value.
        /// </summary>
        internal const int SpanningCodeStartSubflowsInvalid = ValidationError.BaseValue + 90;

        /// <summary>
        /// Indicates a SpanningCodeStart occurs after the corresponding SpanningCodeEnd.
        /// </summary>
        internal const int SpanningCodeStartTagOccursAfterEndTag = ValidationError.BaseValue + 91;

        /// <summary>
        /// Indicates a StandaloneCode has an invalid DataReference.
        /// </summary>
        internal const int StandaloneCodeDataRefInvalid = ValidationError.BaseValue + 92;

        /// <summary>
        /// Indicates a StandaloneCode has an invalid SubFlows value.
        /// </summary>
        internal const int StandaloneCodeSubflowsInvalid = ValidationError.BaseValue + 93;

        /// <summary>
        /// Indicates an inline tag has an invalid StartReference.
        /// </summary>
        internal const int TagStartRefInvalid = ValidationError.BaseValue + 94;

        /// <summary>
        /// Indicates the language on a Target is invalid.
        /// </summary>
        internal const int TargetLangInvalid = ValidationError.BaseValue + 110;

        /// <summary>
        /// Indicates the language on a Target doesn't match the document TargetLanguage.
        /// </summary>
        internal const int TargetLangMismatch = ValidationError.BaseValue + 95;

        /// <summary>
        /// Indicates a Target order is a duplicate.
        /// </summary>
        internal const int TargetOrderDuplicate = ValidationError.BaseValue + 96;

        /// <summary>
        /// Indicates a Target order is a invalid.
        /// </summary>
        internal const int TargetOrderInvalid = ValidationError.BaseValue + 97;

        /// <summary>
        /// Indicates a TranslationContainer Id is not an NMTOKEN.
        /// </summary>
        internal const int TranslationContainerIdNotNMToken = ValidationError.BaseValue + 98;

        /// <summary>
        /// Indicates a TranslationContainer Id is null.
        /// </summary>
        internal const int TranslationContainerIdNull = ValidationError.BaseValue + 99;

        /// <summary>
        /// Indicates an unhandled exception occurred.
        /// </summary>
        internal const int UnhandledException = ValidationError.BaseValue + 100;

        /// <summary>
        /// Indicates a Unit does not contain any ContainerResources.
        /// </summary>
        internal const int UnitMissingResource = ValidationError.BaseValue + 101;

        /// <summary>
        /// Indicates a Unit does not contain any Segments.
        /// </summary>
        internal const int UnitMissingSegment = ValidationError.BaseValue + 102;

        /// <summary>
        /// Indicates a Validation module Validation contains no Rules.
        /// </summary>
        internal const int ValidationMissingRules = ValidationError.BaseValue + 136;
    }
}
