namespace Localization.Xliff.OM.Core
{
    using System;
    using System.Xml;
    using Localization.Xliff.OM.Exceptions;
    using Localization.Xliff.OM.XmlNames;

    /// <summary>
    /// This class represents processing instruction in resource content.
    /// </summary>
    /// <seealso cref="ResourceStringContent"/>
    public class ProcessingInstructionTag : ResourceStringContent
    {
        /// <summary>
        /// The name of the processing instruction.
        /// </summary>
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingInstructionTag"/> class.
        /// </summary>
        /// <param name="name">The name of the processing instruction.</param>
        /// <param name="text">The instruction text.</param>
        public ProcessingInstructionTag(string name, string text)
        {
            this.Name = name;
            this.Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingInstructionTag"/> class.
        /// </summary>
        internal ProcessingInstructionTag()
        {
        }

        #region Properties
        /// <summary>
        /// Gets or sets the name of the processing instruction.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                try
                {
                    XmlConvert.VerifyName(value);
                    this.name = value;
                }
                catch (ArgumentNullException)
                {
                    throw;
                }
                catch (XmlException e)
                {
                    string message;

                    message = string.Format(
                                            Properties.Resources.Xml_ProcessingInstruction_InvalidName_Format,
                                            value);
                    throw new InvalidXmlSpecifierException(message, e);
                }
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets a value indicating whether the provider has text.
        /// </summary>
        protected override bool HasInnerText
        {
            get { return !string.IsNullOrEmpty(this.Text); }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            ProcessingInstructionTag text;

            text = obj as ProcessingInstructionTag;
            return !object.Equals(text, null) && (text.Text == this.Text);
        }

        /// <summary>
        /// Gets as a hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return (this.Text != null) ? this.Text.GetHashCode() : base.GetHashCode();
        }

        /// <summary>
        /// Gets the text associated with this <see cref="XliffElement"/>.
        /// </summary>
        /// <returns>The text within the <see cref="XliffElement"/>.</returns>
        protected override string GetInnerText()
        {
            return this.Text;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Xml.ProcessingInstructionBeginTag + this.Text + Xml.ProcessingInstructionEndTag;
        }
        #endregion Methods
    }
}
