namespace Localization.Xliff.OM
{
    using System;

    /// <summary>
    /// This class contains the name information about Xml fragments (elements or attributes).
    /// </summary>
    public class XmlNameInfo
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlNameInfo"/> class by cloning another <see cref="XmlNameInfo"/>.
        /// </summary>
        /// <param name="info">The instance to clone.</param>
        public XmlNameInfo(XmlNameInfo info)
            : this(info.Prefix, info.Namespace, info.LocalName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlNameInfo"/> class.
        /// </summary>
        /// <param name="localName">The local name of the Xml fragment.</param>
        public XmlNameInfo(string localName)
            : this(null, null, localName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlNameInfo"/> class.
        /// </summary>
        /// <param name="ns">The namespace of the Xml fragment.</param>
        /// <param name="localName">The local name of the Xml fragment.</param>
        public XmlNameInfo(string ns, string localName)
            : this(null, ns, localName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlNameInfo"/> class.
        /// </summary>
        /// <param name="prefix">The prefix of the Xml fragment.</param>
        /// <param name="ns">The namespace of the Xml fragment.</param>
        /// <param name="localName">The local name of the Xml fragment.</param>
        public XmlNameInfo(string prefix, string ns, string localName)
        {
            // The value of localName may be null if the fragment is a text fragment (ie. PlainText).
            if ((localName != null) && localName.Contains(":"))
            {
                throw new ArgumentException(Properties.Resources.XmlNameInfo_InvalidLocalName, "localName");
            }

            this.LocalName = string.IsNullOrWhiteSpace(localName) ? null : localName;
            this.Namespace = string.IsNullOrWhiteSpace(ns) ? null : ns;
            this.Prefix = string.IsNullOrWhiteSpace(prefix) ? null : prefix;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the local name of the Xml fragment.
        /// </summary>
        public string LocalName { get; private set; }

        /// <summary>
        /// Gets the name of the Xml fragment including the prefix (if available) and the local name.
        /// </summary>
        public string Name
        {
            get
            {
                return (this.Prefix == null) ? this.LocalName : (this.Prefix + ":" + this.LocalName);
            }
        }
        
        /// <summary>
        /// Gets the namespace of the Xml fragment.
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// Gets the prefix of the Xml fragment.
        /// </summary>
        public string Prefix { get; private set; }
        #endregion Properties

        #region Static Methods
        /// <summary>
        /// Operator that determines whether two <see cref="XmlNameInfo"/> objects are equal.
        /// </summary>
        /// <param name="info1">The first <see cref="XmlNameInfo"/> to compare.</param>
        /// <param name="info2">The <see cref="XmlNameInfo"/> to which compare against <paramref name="info1"/>.</param>
        /// <returns>True if the objects are equivalent, otherwise false.</returns>
        public static bool operator ==(XmlNameInfo info1, XmlNameInfo info2)
        {
            return object.ReferenceEquals(info1, null) ? object.ReferenceEquals(info2, null) : info1.Equals(info2);
        }

        /// <summary>
        /// Operator that determines whether two <see cref="XmlNameInfo"/> objects are not equal.
        /// </summary>
        /// <param name="info1">The first <see cref="XmlNameInfo"/> to compare.</param>
        /// <param name="info2">The <see cref="XmlNameInfo"/> to which compare against <paramref name="info1"/>.</param>
        /// <returns>True if the objects are not equivalent, otherwise false.</returns>
        public static bool operator !=(XmlNameInfo info1, XmlNameInfo info2)
        {
            return !(info1 == info2);
        }
        #endregion Static Methods

        #region Methods
        /// <summary>
        /// Determines whether an object is equivalent to this object.
        /// </summary>
        /// <param name="obj">The object to which compare against this.</param>
        /// <returns>True if the objects are equivalent, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            XmlNameInfo info;

            info = obj as XmlNameInfo;
            return !object.ReferenceEquals(info, null) && this.Equals(info.Prefix, info.Namespace, info.LocalName);
        }

        /// <summary>
        /// Determines whether this object is equivalent to the named information.
        /// </summary>
        /// <param name="prefix">The prefix of the Xml fragment.</param>
        /// <param name="ns">The namespace of the Xml fragment.</param>
        /// <param name="localName">The local name of the Xml fragment.</param>
        /// <returns>True if this objects has the same name information as that specified, otherwise false.</returns>
        private bool Equals(string prefix, string ns, string localName)
        {
            prefix = string.IsNullOrWhiteSpace(prefix) ? null : prefix;
            ns = string.IsNullOrWhiteSpace(ns) ? null : ns;
            localName = string.IsNullOrWhiteSpace(localName) ? null : localName;

            return (prefix == this.Prefix) && (ns == this.Namespace) && (localName == this.LocalName);
        }

        /// <summary>
        /// Gets a hash code representing the object.
        /// </summary>
        /// <returns>A hash of the object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Methods
    }
}
