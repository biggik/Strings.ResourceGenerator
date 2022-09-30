using Strings.ResourceGenerator.Models;
using System.Diagnostics;
using YamlDotNet.Serialization;

namespace Strings.ResourceGenerator.Generators.YamlFile
{
    internal static class YamlProvider
    {
        public static LocalizerGenerator Provide(string path, string clazz, string yaml)
        {
            Debug.WriteLine($"ResourceGenerator: Yaml generating for {clazz}");

            var deserializer = new DeserializerBuilder()
                //.WithNamingConvention(CamelCaseNamingConvention.Instance)
                //.WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml
                .Build();

            var model = deserializer.Deserialize<StringsModel>(yaml);

            return model.ToGenerator(path, clazz, (d, l) => new YamlGenerator(d, l));
        }
    }
}