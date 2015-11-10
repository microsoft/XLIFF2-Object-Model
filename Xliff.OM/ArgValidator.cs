namespace Localization.Xliff.OM
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Localization.Xliff.OM.Exceptions;

    /// <summary>
    /// This class validates arguments and throws exceptions if arguments are invalid based on the context.
    /// </summary>
    internal sealed class ArgValidator
    {
        #region Member Variables
        /// <summary>
        /// The argument to validate.
        /// </summary>
        private object argument;

        /// <summary>
        /// The Name of the argument.
        /// </summary>
        private string name;
        #endregion Member Variables

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgValidator"/> class.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <param name="name">The Name of the argument.</param>
        private ArgValidator(object argument, string name)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(name), "name must be specified.");

            this.argument = argument;
            this.name = name;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ArgValidator"/> class.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <param name="name">The Name of the argument.</param>
        /// <returns>An instance of the <see cref="ArgValidator"/> class.</returns>
        public static ArgValidator Create(object argument, string name)
        {
            return new ArgValidator(argument, name);
        }

        /// <summary>
        /// Validates that the <see cref="XliffElement"/> is not already used as a child for another
        /// <see cref="XliffElement"/>. If the element is a child, then an <see cref="ElementReuseException"/>
        /// is thrown.
        /// </summary>
        /// <param name="element">The element to check.</param>
        public static void ParentIsNull(XliffElement element)
        {
            if ((element != null) && (element.Parent != null))
            {
                string message;

                message = string.Format(Properties.Resources.ArgValidator_ElementReused_Format, element.GetType().Name);
                throw new ElementReuseException(message);
            }
        }

        /// <summary>
        /// Validates that an argument is of a specified type and throws an ArgumentException on failure.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <returns>The instance of this class.</returns>
        public ArgValidator IsOfType(Type type)
        {
            if (!type.IsInstanceOfType(this.argument))
            {
                string message;

                message = string.Format(
                                        Properties.Resources.ArgValidator_InvalidType_Format,
                                        this.argument.GetType().Name,
                                        type.Name);
                throw new ArgumentException(message, this.name);
            }

            return this;
        }

        /// <summary>
        /// Validates that an argument type is recognized and throws an ArgumentException on failure.
        /// </summary>
        /// <param name="types">The types to check for.</param>
        /// <returns>The instance of this class.</returns>
        public ArgValidator IsOfType(IEnumerable<Type> types)
        {
            bool result;

            result = false;
            foreach (Type type in types)
            {
                result |= type.IsInstanceOfType(this.argument);
            }

            if (!result)
            {
                string message;

                message = string.Format(
                                        Properties.Resources.ArgValidator_InvalidTypeOfMany_Format,
                                        this.argument.GetType().Name);
                throw new ArgumentException(message, this.name);
            }

            return this;
        }

        /// <summary>
        /// Validates that an argument is not null and throws an ArgumentNullException on failure.
        /// </summary>
        /// <remarks>If the argument is a string it is also checked for whitespace only.</remarks>
        /// <seealso cref="ArgValidator.IsNotNullOrWhitespace"/>
        /// <returns>The instance of this class.</returns>
        public ArgValidator IsNotNull()
        {
            return this.IsNotNull(true);
        }

        /// <summary>
        /// Validates that an argument is not null and throws an ArgumentNullException on failure.
        /// </summary>
        /// <param name="checkStringContent">If true, the string contents are compared to whitespace, otherwise the
        /// contents are not checked.</param>
        /// <remarks>If <paramref name="checkStringContent"/> is true and the argument is a string it is also checked
        /// for whitespace only.</remarks>
        /// <seealso cref="ArgValidator.IsNotNullOrWhitespace"/>
        /// <returns>The instance of this class.</returns>
        public ArgValidator IsNotNull(bool checkStringContent)
        {
            if (this.argument == null)
            {
                throw new ArgumentNullException(this.name);
            }
            else if (checkStringContent && (this.argument is string))
            {
                this.IsNotNullOrWhitespace();
            }

            return this;
        }

        /// <summary>
        /// Validates that a string argument is not null or empty and throws an ArgumentNullException on failure.
        /// </summary>
        /// <returns>The instance of this class.</returns>
        public ArgValidator IsNotNullOrEmpty()
        {
            if (object.Equals(this.argument, null))
            {
                throw new ArgumentNullException(this.name);
            }
            else if (this.argument is string)
            {
                if (string.IsNullOrEmpty(this.argument as string))
                {
                    throw new ArgumentNullException(this.name);
                }
            }
            else if (this.argument is IEnumerable)
            {
                IEnumerable enumerable;
                bool hasItem;

                hasItem = false;

                enumerable = (IEnumerable)this.argument;
                foreach (object temp in enumerable)
                {
                    hasItem = true;
                    break;
                }

                if (!hasItem)
                {
                    throw new ArgumentNullException(this.name);
                }
            }

            return this;
        }

        /// <summary>
        /// Validates that a string argument is not null or contains whitespace only and throws an
        /// ArgumentNullException on failure.
        /// </summary>
        /// <returns>The instance of this class.</returns>
        public ArgValidator IsNotNullOrWhitespace()
        {
            if (string.IsNullOrWhiteSpace(this.argument as string))
            {
                throw new ArgumentNullException(this.name);
            }

            return this;
        }

        /// <summary>
        /// Validates that an argument is either null or a valid string that is not just whitespace and throws
        /// an ArgumentNullException on failure.
        /// </summary>
        /// <returns>The instance of this class.</returns>
        public ArgValidator IsNotWhitespaceOnly()
        {
            if (this.argument != null)
            {
                this.IsNotNullOrWhitespace();
            }

            return this;
        }

        /// <summary>
        /// Validates that an argument represents a valid Id string.
        /// </summary>
        /// <param name="element">The object that contains the argument to validate.</param>
        /// <returns>The instance of this class.</returns>
        public ArgValidator IsValidId(object element)
        {
            string value;

            value = this.argument as string;
            if (value != null)
            {
                Exception exception;

                if (!Utilities.IsValidNMTOKEN(value, out exception))
                {
                    string message;

                    message = string.Format(Properties.Resources.ArgValidator_InvalidId_Format, value);
                    throw new InvalidIdException(message, exception);
                }
            }

            return this;
        }

        /// <summary>
        /// Validates that an argument is a string that starts with a certain string.
        /// </summary>
        /// <param name="prefix">The prefix the argument is expected to start with.</param>
        /// <returns>The instance of this class.</returns>
        public ArgValidator StartsWith(string prefix)
        {
            string value;

            value = this.argument as string;
            if ((value == null) || !value.StartsWith(prefix))
            {
                string message;

                message = string.Format(Properties.Resources.ArgValidator_StringDoesNotStartWith_Format, value, prefix);
                throw new ArgumentException(message, this.name);
            }

            return this;
        }
    }
}
