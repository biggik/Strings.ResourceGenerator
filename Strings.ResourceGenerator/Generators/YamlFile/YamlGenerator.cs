using System.Collections.Generic;
using Strings.ResourceGenerator.Generators.Data;
using Strings.ResourceGenerator.Generators.Interfaces;
using Strings.ResourceGenerator.Models;

namespace Strings.ResourceGenerator.Generators.YamlFile;

internal class YamlGenerator : IGenerator
{
    public GeneratorData Data { get; }

    /// <summary>
    /// Creates a new instance of YamlGenerator and parses the strings from the yaml
    /// file for resources
    /// </summary>
    /// <param name="data">The data for the generator</param>
    /// <param name="strings">All strings from the yaml file</param>
    public YamlGenerator(GeneratorData data, List<ResourceStringModel> strings)
    {
        Data = data;
        Data.Resources = this.InitializeResources(strings);
    }
}
