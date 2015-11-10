namespace Localization.Xliff.OM.Extensibility.Tests
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class represents a list of <see cref="XliffData"/> elements.
    /// </summary>
    internal class XliffDataList : List<XliffData>
    {
        /// <summary>
        /// Adds a <see cref="XliffData"/> to the list.
        /// </summary>
        /// <param name="ns">The Xml namespace of the element.</param>
        /// <param name="localName">The Xml local name of the element.</param>
        /// <param name="type">The type associated with the element.</param>
        /// <returns>The newly created and added <see cref="XliffData"/>.</returns>
        public XliffData AddChild(string ns, string localName, Type type)
        {
            return this.AddChild(null, ns, localName, type);
        }

        /// <summary>
        /// Adds a <see cref="XliffData"/> to the list.
        /// </summary>
        /// <param name="prefix">The Xml prefix of the element.</param>
        /// <param name="ns">The Xml namespace of the element.</param>
        /// <param name="localName">The Xml local name of the element.</param>
        /// <param name="type">The type associated with the element.</param>
        /// <returns>The newly created and added <see cref="XliffData"/>.</returns>
        public XliffData AddChild(string prefix, string ns, string localName, Type type)
        {
            XliffData data;

            data = new XliffData(prefix, ns, localName, type);
            this.Add(data);

            return data;
        }
    }
}
