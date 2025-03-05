using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using Strings.ResourceGenerator.Exceptions;
using Strings.ResourceGenerator.Generators.StringsFile;
using System.Diagnostics;
using Strings.ResourceGenerator.Generators.YamlFile;
using System.Linq;
using Strings.ResourceGenerator.Generators.JsonFile;
using System.IO;

namespace Strings.ResourceGenerator
{
    /// <summary>
    /// A code generator that looks for resource files and generates classes for them
    /// </summary>
    [Generator]
    public class ResourceGenerator : IIncrementalGenerator
    {
        /// <inheritdoc />
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var pipeline = context.AdditionalTextsProvider
                .Where(static (text) => 
                    StringsGatherer.IsMatch(text.Path) 
                    || YamlGatherer.IsMatch(text.Path) 
                    || JsonGatherer.IsMatch(text.Path))
                .Select(static (text, cancellationToken) =>
                {
                    var code = StringsGatherer.IsMatch(text.Path)
                        ? StringsGatherer.Gather(text)
                        : YamlGatherer.IsMatch(text.Path)
                            ? YamlGatherer.Gather(source)
                            : JsonGatherer.Gather(source);

                    MyXmlToCSharpCompiler.Compile(text.GetText(cancellationToken));
                    return (name, code);
                });

            var stringSources = context.AdditionalTextsProvider.
                        .a
                       .CreateSyntaxProvider(CouldBeEnumerationAsync, GetEnumTypeOrNull)
                       .Where(type => type is not null)
                       .Collect();

            var stringsGenerators = StringsGatherer.Gather(context);
            var yamlGenerators = YamlGatherer.Gather(context);
            var jsonGenerators = JsonGatherer.Gather(context);

            foreach (var generator in Enumerable.Concat(Enumerable.Concat(stringsGenerators, yamlGenerators), jsonGenerators))
            { 
                try
                {
                    var generated = generator.Generate();
                    var sourceText = SourceText.From(generated, Encoding.UTF8);
                    context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                        $"{generator.Clazz}.g.cs", sourceText));
                }
                catch (StringGeneratorException sge)
                {
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
            }
        }
    }
}
