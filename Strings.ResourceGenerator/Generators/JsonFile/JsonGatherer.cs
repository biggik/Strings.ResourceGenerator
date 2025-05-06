using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Strings.ResourceGenerator.Generators.JsonFile
{
    internal static class JsonGatherer
    {
        public static IEnumerable<LocalizerGenerator> Gather(IEnumerable<AdditionalText> additionalFiles, System.Threading.CancellationToken cancellationToken)
        {
            // Get all .json files from AdditionalText
            var allJsonFiles = additionalFiles
                .Where(at => at.Path.EndsWith(".json"));

            foreach (var jsonFile in allJsonFiles)
            {
                // Read the file content
                var fileContent = jsonFile.GetText(cancellationToken)?.ToString();
                if (fileContent != null)
                {
                    // Extract the class name from the file name
                    var clazz = Path.GetFileNameWithoutExtension(jsonFile.Path).Split('.')[0];

                    // Create and yield a LocalizerGenerator for the JSON file
                    yield return JsonProvider.Provide(jsonFile.Path, clazz, fileContent);
                }
            }
        }
    }
}
