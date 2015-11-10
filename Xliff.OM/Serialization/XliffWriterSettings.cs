namespace Localization.Xliff.OM.Serialization
{
    using System.Collections.Generic;
    using Localization.Xliff.OM.Validators;

    /// <summary>
    /// This class controls the <see cref="XliffWriter"/> when writing XLIFF content.
    /// </summary>
    public class XliffWriterSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XliffWriterSettings"/> class.
        /// </summary>
        public XliffWriterSettings()
        {
            this.Detail = OutputDetail.Explicit;
            this.IncludeExtensions = true;
            this.Indent = false;
            this.IndentChars = null;
            this.Validators = new List<IXliffValidator>();
            this.Validators.Add(new StandardValidator());
        }

        #region Properties
        /// <summary>
        /// Gets or sets the amount of detail to include when writing XLIFF documents.
        /// </summary>
        /// <remarks>The default is Output.Explicit.</remarks>
        public OutputDetail Detail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to write extension information during serialization.
        /// </summary>
        public bool IncludeExtensions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to indent the output so it's more human readable.
        /// </summary>
        public bool Indent { get; set; }

        /// <summary>
        /// Gets or sets the characters to print as indent characters if Indent is set to true.
        /// </summary>
        public string IndentChars { get; set; }

        /// <summary>
        /// Gets the list of validators to call prior to writing data to ensure the
        /// <see cref="Localization.Xliff.OM.Core.XliffDocument"/> is well formed and structurally sound.
        /// </summary>
        public IList<IXliffValidator> Validators { get; private set; }
        #endregion Properties
    }
}
