using System.IO;
using Shouldly;
using Strings.ResourceGenerator.Generators;
using Strings.ResourceGenerator.Models;
using Strings.ResourceGenerator.Generators.Parsers;

namespace UnitTests.Generators;

public class ResourceParserBehaviorTests
{
    [Fact]
    public void Throws_InvalidData_WhenMixingOrdinalAndInterpolation()
    {
        Should.Throw<InvalidDataException>(() =>
            _ = new ResourceString(null, "Mixed", "{0} plus {named}"));

        var ex = Should.Throw<InvalidDataException>(() =>
            _ = new ResourceString("en", "Mixed", "{0} plus {named}"));
        ex.Message.ShouldContain("Mixed");
        ex.Message.ShouldContain("standard");
        ex.Message.ShouldContain("interpolation");
    }

    [Fact]
    public void KeywordAsInterpolationParameter_IsRenamedForValidIdentifier()
    {
        var rs = new ResourceString(null, "Kw", "Hello {int}");

        rs.StringType.ShouldBe(StringType.Interpolation);
        rs.Value.ShouldBe("Hello {_int}");
        rs.Parameters.ShouldHaveSingleItem();
        rs.Parameters[0].Name.ShouldBe("_int");
    }

    [Fact]
    public void DoubleBraceEscaping_StaysSimple_AndRoundTripsMechanicalReplacement()
    {
        var p = new ResourceParser(null, "K", "{{notparam}}");

        p.StringType.ShouldBe(StringType.Simple);
        // Mechanical {{ / }} tokenization is restored after parsing (no format placeholders).
        p.Value.ShouldBe("{{notparam}}");
    }
}
