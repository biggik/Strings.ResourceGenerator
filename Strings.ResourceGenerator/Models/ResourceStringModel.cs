using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Strings.ResourceGenerator.Models;

/// <summary>
/// A resource string, either with a single <code>Value</code>
/// </summary>
public class ResourceStringModel
{
    /// <summary>
    /// The unique key for the resource string
    /// </summary>
    [YamlMember(Alias = "key", DefaultValuesHandling = DefaultValuesHandling.OmitNull, ApplyNamingConventions = false)]
    public string Key { get; set; }

    /// <summary>
    /// A value for the resource king, where no translation to multiple languages is required
    /// </summary>
    [YamlMember(Alias = "value", DefaultValuesHandling = DefaultValuesHandling.OmitNull, ApplyNamingConventions = false)]
    public string Value { get; set; }

    /// <summary>
    /// The values for the resource string, locale specific
    /// </summary>
    [YamlMember(Alias = "values", DefaultValuesHandling = DefaultValuesHandling.OmitNull, ApplyNamingConventions = false)]
    public List<ResourceStringValue> Values { get; set; }

    /// <summary>
    /// An optional context for where the resource is used
    /// </summary>
    [YamlMember(Alias = "context", DefaultValuesHandling = DefaultValuesHandling.OmitNull, ApplyNamingConventions = false)]
    public string Context { get; set; }
}