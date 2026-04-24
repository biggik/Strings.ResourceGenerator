using Microsoft.CodeAnalysis;
using Shouldly;
using Strings.ResourceGenerator.Generators;
using Strings.ResourceGenerator.Generators.StringsFile;
using Strings.ResourceGenerator.Models;

namespace UnitTests;

public class LargeMultiLocaleSyntaxTests
{
    [Fact]
    public void CoreStrings_fixture_generates_parseable_cs()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "TestData", "CoreStrings.strings");
        File.Exists(path).ShouldBeTrue($"Copy-to-output fixture missing: {path}");

        var lines = File.ReadAllLines(path);
        var gen = StringsProvider.Provide(
            "CoreStrings",
            StringConfiguration.DefaultConfiguration,
            (path, lines));

        var src = gen.Generate();
        var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(src);
        var errors = tree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        errors.ShouldBeEmpty(string.Join("\n", errors.Select(e => e.ToString())));
    }
}
