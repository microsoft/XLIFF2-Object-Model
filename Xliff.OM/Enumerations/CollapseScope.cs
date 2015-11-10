namespace Localization.Xliff.OM
{
    using System;

    /// <summary>
    /// Defines what to return when collapsing elements.
    /// </summary>
    [Flags]
    public enum CollapseScope
    {
        /// <summary>
        /// Don't return anything.
        /// </summary>
        None = 0,

        /// <summary>
        /// Collapse elements at any level of the hierarchy.
        /// </summary>
        AllDescendants = 1 << 0,

        /// <summary>
        /// Collapse elements only at the first level of the hierarchy.
        /// </summary>
        TopLevelDescendants = 1 << 1,

        /// <summary>
        /// Include the current element in the search.
        /// </summary>
        CurrentElement = 1 << 2,

        /// <summary>
        /// Include core Xliff elements in the search. Core Xliff elements that reside under extensions are considered
        /// extensions, not CoreElements.
        /// </summary>
        CoreElements = 1 << 3,

        /// <summary>
        /// Include extension elements in the search. Core Xliff elements that reside under extensions are considered
        /// extensions, not CoreElements.
        /// </summary>
        Extensions = 1 << 4,

        /// <summary>
        /// Include all elements at all levels of the hierarchy.
        /// </summary>
        All = CollapseScope.AllDescendants | CollapseScope.CurrentElement | CollapseScope.CoreElements | CollapseScope.Extensions,

        /// <summary>
        /// Include all core elements at all levels of the hierarchy, excluding the current element.
        /// </summary>
        Default = CollapseScope.AllDescendants | CollapseScope.CoreElements,
    }
}
