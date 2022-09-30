namespace Strings.ResourceGenerator.Models
{
    /// <summary>
    /// The possible type of strings regognized by the parsing
    /// </summary>
    public enum StringType
    {
        /// <summary>
        /// The default string type - never used directly in code
        /// </summary>
        NotSet,

        /// <summary>
        /// A Simple string type is a resource string without any parameters
        /// </summary>
        Simple,

        /// <summary>
        /// A Format string type is a resource string that makes use of String.Format replacements
        /// of bracketed values (e.g. "Name is {0}")
        /// </summary>
        Format,

        /// <summary>
        /// An interpolated string type is a resource string that makes use of named interpolation replacements
        /// (e.g. "Name is {name}")
        /// </summary>
        Interpolation
    }

}
