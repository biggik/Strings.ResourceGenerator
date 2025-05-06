using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using Strings.ResourceGenerator.Exceptions;
using Strings.ResourceGenerator.Generators.StringsFile;
using Strings.ResourceGenerator.Generators.YamlFile;
using Strings.ResourceGenerator.Generators.JsonFile;
using System.Linq;

namespace Strings.ResourceGenerator
{
    /// <summary>
    /// A code generator that looks for resource files and generates classes for them
    /// </summary>
    [Generator]
    public class ResourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Combine Compilation and AdditionalText files into a single pipeline
            var resourceGenerators = context.AdditionalTextsProvider.Collect()
                .SelectMany((additionalFiles, cancellationToken) =>
                {
                    var stringsGenerators = StringsGatherer.Gather(additionalFiles, cancellationToken);
                    var yamlGenerators = YamlGatherer.Gather(additionalFiles, cancellationToken);
                    var jsonGenerators = JsonGatherer.Gather(additionalFiles, cancellationToken);

                    return stringsGenerators
                        .Concat(yamlGenerators)
                        .Concat(jsonGenerators);
                });

            // Register the source output
            context.RegisterSourceOutput(resourceGenerators, (context, generator) =>
            {
                try
                {
                    // Generate the source code
                    var generated = generator.Generate();
                    var sourceText = SourceText.From(generated, Encoding.UTF8);

                    // Add the generated source
                    context.AddSource($"{generator.Clazz}.g.cs", sourceText);
                }
                catch (StringGeneratorException sge)
                {
                    // Report diagnostics for any errors
                    var rule = new DiagnosticDescriptor(
                        "SG0001",
                        "Error during string resource generation",
                        "Error: {0}",
                        "String generation",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true,
                        "String generator error");

                    foreach (var error in sge.Errors)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(rule, Location.None, error));
                    }
                }
            });
        }
    }
}
