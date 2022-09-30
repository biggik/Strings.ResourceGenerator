using System;
using System.Collections.Generic;
using System.Linq;

namespace Strings.ResourceGenerator.Exceptions
{
    /// <summary>
    /// An exception thrown by string generation
    /// </summary>
    public class StringGeneratorException : Exception
    {
        /// <summary>
        /// Errors encountered during generation
        /// </summary>
        public List<string> Errors { get; }

        /// <summary>
        /// Creates a new instance of <see cref="StringGeneratorException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="errors">Errors encountered</param>
        public StringGeneratorException(string message, IEnumerable<string> errors)
            : base(message)
        {
            Errors = errors.ToList();
        }
    }
}
