namespace Localization.Xliff.OM
{
    /// <summary>
    /// This class contains information that describes how to serialize an XLIFF element.
    /// </summary>
    /// <seealso cref="XmlNameInfo"/>
    public class ElementInfo : XmlNameInfo
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementInfo"/> class.
        /// </summary>
        /// <param name="name">The Xml name of the element.</param>
        /// <param name="element">The element this class describes.</param>
        public ElementInfo(XmlNameInfo name, XliffElement element)
            : base(name)
        {
            ArgValidator.Create(element, "element").IsNotNull();

            this.Element = element;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the element this class describes.
        /// </summary>
        public XliffElement Element { get; private set; }
        #endregion Properties
    }
}
