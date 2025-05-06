using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Strings.ResourceGenerator.Generators.YamlFile
{
    internal static class YamlGatherer
    {
        public static IEnumerable<LocalizerGenerator> Gather(IEnumerable<AdditionalText> additionalFiles, System.Threading.CancellationToken cancellationToken)
        {
            // Get all .yaml files from AdditionalText
            var allYamlFiles = additionalFiles
                .Where(at => at.Path.EndsWith(".yaml"));

            foreach (var yamlFile in allYamlFiles)
            {
                // Read the file content
                var fileContent = yamlFile.GetText(cancellationToken)?.ToString();
                if (fileContent != null)
                {
                    // Extract the class name from the file name
                    var clazz = Path.GetFileNameWithoutExtension(yamlFile.Path).Split('.')[0];

                    // Create and yield a LocalizerGenerator for the YAML file
                    yield return YamlProvider.Provide(yamlFile.Path, clazz, fileContent);
                }
            }
        }
    }
}
