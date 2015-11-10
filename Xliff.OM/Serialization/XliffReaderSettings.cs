namespace Localization.Xliff.OM.Serialization
{
    using System.Collections.Generic;
    using Localization.Xliff.OM.Validators;

    /// <summary>
    /// This class controls the <see cref="XliffReader"/> when reading XLIFF content.
    /// </summary>
    public class XliffReaderSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XliffReaderSettings"/> class.
        /// </summary>
        public XliffReaderSettings()
        {
            this.IncludeExtensions = true;
            this.Validators = new List<IXliffValidator>();
            this.Validators.Add(new StandardValidator());
        }

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether to store extension information during deserialization.
        /// </summary>
        public bool IncludeExtensions { get; set; }

        /// <summary>
        /// Gets the list of validators to call prior to reading data to ensure the
        /// <see cref="Localization.Xliff.OM.Core.XliffDocument"/> is well formed and structurally sound.
        /// </summary>
        public IList<IXliffValidator> Validators { get; private set; }
        #endregion Properties
    }
}
