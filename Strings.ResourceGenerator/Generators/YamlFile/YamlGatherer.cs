using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Strings.ResourceGenerator.Generators.YamlFile
{
    internal static class YamlGatherer
    {
        public static IReadOnlyCollection<LocalizerGenerator> Gather(GeneratorExecutionContext context)
        {
            return Gather().ToList();

            IEnumerable<LocalizerGenerator> Gather()
            {
                // Get all .yaml files
                var allYamlFiles = context.AdditionalFiles.Where(at => at.Path.EndsWith(".yaml"));

                foreach (var yamlFile in allYamlFiles)
                {
                    Debug.WriteLine($"ResourceGenerator: {Path.GetFileName(yamlFile.Path)}");
                    
                    var clazz = Path.GetFileNameWithoutExtension(yamlFile.Path).Split('.')[0];

                    yield return YamlProvider.Provide(yamlFile.Path, clazz, yamlFile.GetText(context.CancellationToken).ToString());
                }
            }
        }
    }
}
