using Shouldly;
using Strings.ResourceGenerator.Generators.Emit;

namespace UnitTests.Emit;

public class IndentTests
{
    [Theory]
    [InlineData(0, "")]
    [InlineData(1, "    ")]
    [InlineData(2, "        ")]
    [InlineData(3, "            ")]
    public void Spaces_matches_levels_times_IndentWidth(int levels, string expected)
    {
        Indent.Spaces(levels).ShouldBe(expected);
    }
}
