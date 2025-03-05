using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Strings.ResourceGenerator.Generators.YamlFile
{
    internal static class YamlGatherer
    {
        public static bool IsMatch(string filePath) => filePath.EndsWith(".yaml", System.StringComparison.OrdinalIgnoreCase);

        public static string Gather(SourceText source)
        {

        }

        public static IReadOnlyCollection<LocalizerGenerator> Gather(IncrementalGeneratorInitializationContext context)
        {
            return Gather().ToList();

            IEnumerable<LocalizerGenerator> Gather()
            {
                // Get all .yaml files
                var allYamlFiles = context.AdditionalTextsProvider
                    .Where(at => at.Path.EndsWith(".yaml"))
                    .Select((a, c) => (Path.GetFileNameWithoutExtension(a.Path), a.GetText(c)!.ToString()));

                var compilationAndFiles = context.CompilationProvider.Combine(allYamlFiles.Collect());
                context.RegisterSourceOutput(compilationAndFiles, static (productionContext, sourceContext) =>
                {
                    if (sourceContext == null || !sourceContext.Methods.Any())
                        return;

                    var sourceText = SourceText.From($$"""
                                               namespace {{sourceContext.Namespace}};
                                               public interface I{{sourceContext.ClassName}}
                                               {
                                                   {{string.Join("\n    ", sourceContext.Methods)}}
                                               }
                    
                                               """
                    , Encoding.UTF8);

                    context.AddSource($"I{sourceContext.ClassName}.g.cs", sourceText);
                });

                foreach (var yamlFile in allYamlFiles)
                {
                    var clazz = Path.GetFileNameWithoutExtension(yamlFile.Path).Split('.')[0];

                    yield return YamlProvider.Provide(yamlFile.Path, clazz, yamlFile.GetText(context.CancellationToken).ToString());
                }
            }
        }
    }
}
