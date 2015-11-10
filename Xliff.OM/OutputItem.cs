namespace Localization.Xliff.OM
{
    using System;

    /// <summary>
    /// This class represents an item to serialize.
    /// </summary>
    public class OutputItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputItem"/> class.
        /// </summary>
        /// <param name="itemType">The type of item to output.</param>
        /// <param name="childType">The type of the child to output, if the <paramref name="itemType"/> is Child.</param>
        /// <param name="groupOrdinal">The ordinal value of the grouping of elements that this item should be
        /// serialized with.</param>
        public OutputItem(OutputItemType itemType, Type childType, int groupOrdinal)
        {
            if (itemType == OutputItemType.Child)
            {
                ArgValidator.Create(childType, "childType").IsNotNull();
            }

            this.ItemType = itemType;
            this.ChildType = childType;
            this.GroupOrdinal = groupOrdinal;
        }

        /// <summary>
        /// Gets the type of the child to output. This value is only valid if ItemType is Child.
        /// </summary>
        public Type ChildType { get; private set; }

        /// <summary>
        /// Gets the ordinal value of the grouping of elements that this item should be serialized with.
        /// </summary>
        public int GroupOrdinal { get; private set; }

        /// <summary>
        /// Gets the type of item to output.
        /// </summary>
        public OutputItemType ItemType { get; private set; }
    }
}
