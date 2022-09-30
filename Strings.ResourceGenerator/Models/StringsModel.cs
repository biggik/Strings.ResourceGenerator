using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Strings.ResourceGenerator.Models
{
    /// <summary>
    /// The base for structured models
    /// </summary>
    public class StringsModel
    {
        /// <summary>
        /// Configuration for the string
        /// </summary>
        [YamlMember(Alias = "config", ApplyNamingConventions = false)]
        public StringConfiguration Config { get; set; }

        /// <summary>
        /// The resource strings
        /// </summary>
        [YamlMember(Alias = "strings", ApplyNamingConventions = false)]
        public List<ResourceStringModel> Strings { get; set; }
    }
}