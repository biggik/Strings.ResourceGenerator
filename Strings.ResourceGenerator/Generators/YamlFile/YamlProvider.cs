using Strings.ResourceGenerator.Models;
using YamlDotNet.Serialization;

namespace Strings.ResourceGenerator.Generators.YamlFile;

internal static class YamlProvider
{
    public static LocalizerGenerator Provide(string path, string clazz, string yaml)
    {
        var deserializer = new DeserializerBuilder()
            .Build();

        var model = deserializer.Deserialize<StringsModel>(yaml);

        return model.ToGenerator(path, clazz, (d, l) => new YamlGenerator(d, l));
    }
}