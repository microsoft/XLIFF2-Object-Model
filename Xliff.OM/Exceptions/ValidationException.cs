namespace Localization.Xliff.OM.Exceptions
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Exception that indicates a document is invalid.
    /// </summary>
    /// <seealso cref="Exception"/>
    [SuppressMessage("StyleCop.CSharp.Rules.Windows.Uex.UexRules",
                     "UX0810::CustomExceptionClassesShouldHaveSerializableAttribute",
                     Justification = "Portable Class Library doesn't support SerializableAttribute.")]
    public sealed class ValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="selectorPath">The path to the element that has the violation.</param>
        public ValidationException(string message, string selectorPath)
            : this(0, message, selectorPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="selectorPath">The path to the element that has the violation.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null
        /// reference if no inner exception is specified.
        /// </param>
        public ValidationException(string message, string selectorPath, Exception innerException)
            : this(0, message, selectorPath, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null
        /// reference if no inner exception is specified.
        /// </param>
        public ValidationException(string message, Exception innerException)
            : this(0, message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified
        /// error message.
        /// </summary>
        /// <param name="errorNumber">A number that identifies the error.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="selectorPath">The path to the element that has the violation.</param>
        public ValidationException(int errorNumber, string message, string selectorPath)
            : base(message)
        {
            this.Data[ValidationException.Constants.DataSelectorPath] = selectorPath;
            this.ErrorNumber = errorNumber;
            this.SelectorPath = selectorPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="errorNumber">A number that identifies the error.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="selectorPath">The path to the element that has the violation.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null
        /// reference if no inner exception is specified.
        /// </param>
        public ValidationException(int errorNumber, string message, string selectorPath, Exception innerException)
            : base(message, innerException)
        {
            this.Data[ValidationException.Constants.DataSelectorPath] = selectorPath;
            this.ErrorNumber = errorNumber;
            this.SelectorPath = selectorPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="errorNumber">A number that identifies the error.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null
        /// reference if no inner exception is specified.
        /// </param>
        public ValidationException(int errorNumber, string message, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorNumber = errorNumber;
        }

        /// <summary>
        /// Gets a number that identifies the error.
        /// </summary>
        public int ErrorNumber { get; private set; }

        /// <summary>
        /// Gets the selector path of the element that contains the error if one is available.
        /// </summary>
        public string SelectorPath { get; private set; }

        /// <summary>
        /// This class contains constant values that are used in this class.
        /// </summary>
        private static class Constants
        {
            /// <summary>
            /// The name of the Data entry that contains the selector path.
            /// </summary>
            public const string DataSelectorPath = "SelectorPath";
        }
    }
}
