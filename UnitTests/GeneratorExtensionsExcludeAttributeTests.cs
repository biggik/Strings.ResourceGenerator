using Shouldly;
using Strings.ResourceGenerator.Generators;

namespace UnitTests;

public class GeneratorExtensionsExcludeAttributeTests
{
    [Fact]
    public void ExcludeAttributeIndented_WhenNotApplied_ReturnsNull()
    {
        string.Empty.ExcludeAttributeIndented(apply: false, "any").ShouldBeNull();
    }

    [Fact]
    public void ExcludeAttributeIndented_WhenApplied_ContainsCoverageAttributeAndJustification()
    {
        var line = "    ".ExcludeAttributeIndented(apply: true, "Generated code");

        line.ShouldNotBeNull();
        line.ShouldContain("ExcludeFromCodeCoverage");
        line.ShouldContain("Justification = \"Generated code\"");
    }
}
