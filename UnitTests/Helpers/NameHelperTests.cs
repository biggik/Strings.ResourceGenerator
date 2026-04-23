using Shouldly;
using Strings.ResourceGenerator.Helpers;

namespace UnitTests.Helpers;

public class NameHelperTests
{
    [Theory]
    [InlineData("hello", "hello")]
    [InlineData("HelloWorld", "HelloWorld")]
    [InlineData("int", "_int")]
    [InlineData("class", "_class")]
    [InlineData("public", "_public")]
    public void CreateValidIdentifier_EscapesKeywords(string input, string expected)
    {
        NameHelper.CreateValidIdentifier(input).ShouldBe(expected);
    }

    [Fact]
    public void CreateValidIdentifier_DoubleLeadingUnderscore_NotKeyword_GetsExtraUnderscore()
    {
        // Matches Reference Source rule: identifiers starting with "__" need a leading "_"
        NameHelper.CreateValidIdentifier("__event").ShouldBe("___event");
    }
}
