using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Strings.ResourceGenerator.Generators.JsonFile
{
    internal static class JsonGatherer
    {
        public static bool IsMatch(string filePath) => filePath.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase);

        public static string Gather(SourceText source)
        {

        }

        public static IReadOnlyCollection<LocalizerGenerator> Gather(IncrementalGeneratorInitializationContext context)
        {
            return Gather().ToList();

            IEnumerable<LocalizerGenerator> Gather()
            {
                // Get all .json files
                var allJsonFiles = context.AdditionalFiles.Where(at => at.Path.EndsWith(".json"));

                foreach (var jsonFile in allJsonFiles)
                {
                    var clazz = Path.GetFileNameWithoutExtension(jsonFile.Path).Split('.')[0];

                    yield return JsonProvider.Provide(jsonFile.Path, clazz, jsonFile.GetText(context.CancellationToken).ToString());
                }
            }
        }
    }
}
