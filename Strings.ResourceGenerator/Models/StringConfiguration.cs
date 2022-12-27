using Newtonsoft.Json;
using Strings.ResourceGenerator.Generators;
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
        [YamlMember(Alias = Constants.Namespace, ApplyNamingConventions = false)]
        public string NameSpace { get; set; }

        /// <summary>
        /// Prefix to apply to class name
        /// </summary>
        [YamlMember(Alias = Constants.Prefix, ApplyNamingConventions = false)]
        public string Prefix { get; set; }

        /// <summary>
        /// If true, the generated class will be public, otherwise internal
        /// </summary>
        [YamlMember(Alias = Constants.Public, ApplyNamingConventions = false)]
        [JsonProperty(Constants.Public)]
        public bool GeneratePublic { get; set; }

        /// <summary>
        /// If set to true, then, where possible, accessors are generated as <code>public const string</code>
        /// instead of <code>public static string</code>
        /// This is not possible for multiple languages, since there a lookup is done based on the locale, 
        /// so the value is never constant
        /// </summary>
        [YamlMember(Alias = "prefer_const", ApplyNamingConventions = false)]
        [JsonProperty(Constants.PreferConst)]
        public bool PreferConstOverStatic { get; set; }

        /// <summary>
        /// If set to true (default) then an [ExcludeFromCodeCoverage] attribute will be
        /// added to all generated classes
        /// </summary>
        [YamlMember(Alias = "exclude_coverage", ApplyNamingConventions = false)]
        [JsonProperty(Constants.ExcludeCoverage)]
        public bool ExcludeFromCodeCoverage { get; set; } = true;

        [YamlIgnore]
        [JsonIgnore]
        internal string ConfigurationSource { get; set; }

        internal static StringConfiguration DefaultConfiguration
            => new()
            {
                NameSpace = DefaultNamespace,
                Prefix = ""
            };
    }
}
