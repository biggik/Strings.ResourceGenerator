using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Strings.ResourceGenerator.Models
{
    /// <summary>
    /// The configuration for the strings generation
    /// </summary>
    public class StringConfiguration
    {
        public const string DefaultNamespace = "Strings.Resources";

        /// <summary>
        /// The namespace to apply to generated code
        /// </summary>
        [YamlMember(Alias = "namespace", ApplyNamingConventions = false)]
        public string NameSpace { get; set; }

        /// <summary>
        /// Prefix to apply to class name
        /// </summary>
        [YamlMember(Alias = "prefix", ApplyNamingConventions = false)]
        public string Prefix { get; set; }

        /// <summary>
        /// If true, the generated class will be public, otherwise internal
        /// </summary>
        [YamlMember(Alias = "public", ApplyNamingConventions = false)]
        [JsonProperty("public")]
        public bool GeneratePublic { get; set; }

        /// <summary>
        /// If set to true, then, where possible, accessors are generated as <code>public const string</code>
        /// instead of <code>public static string</code>
        /// This is not possible for multiple languages, since there a lookup is done based on the locale, 
        /// so the value is never constant
        /// </summary>
        [YamlMember(Alias = "prefer_const", ApplyNamingConventions = false)]
        [JsonProperty("preferConst")]
        public bool PreferConstOverStatic { get; set; }

        internal static StringConfiguration DefaultConfiguration
            => new()
            {
                NameSpace = DefaultNamespace,
                Prefix = ""
            };
    }
}
