using YamlDotNet.Serialization;

namespace Strings.ResourceGenerator.Models;

/// <summary>
/// A resource string value for a specific locale
/// </summary>
public class ResourceStringValue
{
    /// <summary>
    /// The locale the resource belongs to. A missing value is allowed for one of the strings
    /// which means the value is for the neutral locale
    /// </summary>
    [YamlMember(Alias = "locale", ApplyNamingConventions = false)]
    public string Locale { get; set; }

    /// <summary>
    /// The string value for the locale
    /// </summary>
    [YamlMember(Alias = "value", ApplyNamingConventions = false)]
    public string Value { get; set; }
}