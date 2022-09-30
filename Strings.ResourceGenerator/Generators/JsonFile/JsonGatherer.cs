using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Strings.ResourceGenerator.Generators.JsonFile
{
    internal static class JsonGatherer
    {
        public static IReadOnlyCollection<LocalizerGenerator> Gather(GeneratorExecutionContext context)
        {
            return Gather().ToList();

            IEnumerable<LocalizerGenerator> Gather()
            {
                // Get all .json files
                var allJsonFiles = context.AdditionalFiles.Where(at => at.Path.EndsWith(".json"));

                foreach (var jsonFile in allJsonFiles)
                {
                    Debug.WriteLine($"ResourceGenerator: {Path.GetFileName(jsonFile.Path)}");
                    
                    var clazz = Path.GetFileNameWithoutExtension(jsonFile.Path).Split('.')[0];

                    yield return JsonProvider.Provide(jsonFile.Path, clazz, jsonFile.GetText(context.CancellationToken).ToString());
                }
            }
        }
    }
}
