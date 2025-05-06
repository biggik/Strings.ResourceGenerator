using System.Collections.Generic;
using Strings.ResourceGenerator.Generators.Data;
using Strings.ResourceGenerator.Generators.Interfaces;
using Strings.ResourceGenerator.Models;

namespace Strings.ResourceGenerator.Generators.JsonFile;

internal class JsonGenerator : IGenerator
{
    public GeneratorData Data { get; }

    /// <summary>
    /// Creates a new instance of JsonGenerator and parses the strings from the Json
    /// file for resources
    /// </summary>
    /// <param name="data">The data for the generator</param>
    /// <param name="strings">All strings from the Json file</param>
    public JsonGenerator(GeneratorData data, List<ResourceStringModel> strings)
    {
        Data = data;
        Data.Resources = this.InitializeResources(strings);
    }
}
