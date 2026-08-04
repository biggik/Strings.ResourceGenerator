using Shouldly;
using Strings.ResourceGenerator.Generators;
using Strings.ResourceGenerator.Models;

namespace UnitTests.Generators;

public class ResourceStringTests
{
    [Fact]
    public void ResourceString_Simple_Correctly()
    {
        var undertest = new ResourceString(null, "MyKey", "My simple value");

        // Simple assertions
        undertest.Locale.ShouldBeNull();
        undertest.Key.ShouldBe("MyKey");
        undertest.Value.ShouldBe("My simple value");

        undertest.StringType.ShouldBe(StringType.Simple);
        undertest.CleanValueImplementation.ShouldBe("My simple value");
        undertest.Parameters.Count().ShouldBe(0);

        //rs.StaticProperty.Should().Be();
        //rs.InterfaceDeclaration.Should().Be();
        //rs.PublicProperty.Should().Be();
        //rs.PublicStaticProperty.Should().Be();
        //rs.ClassLine.Should().Be();
    }

    /// <summary>
    /// GenerateLocalizers / 5.2: authors write <c>\r\n</c> in .strings as C#-literal text.
    /// Emit must pass those sequences through (not double backslashes) so the C# compiler
    /// produces real newlines in the const/accessor.
    /// </summary>
    [Fact]
    public void ResourceString_CrLfEscapesInStringsFile_BecomeRealNewlinesInConst()
    {
        // Characters as stored when reading Key=line1\r\n\r\nline2 from a .strings file
        const string fromStringsFile = "line1\\r\\n\\r\\nline2";

        var undertest = new ResourceString(null, "UpdateAvailable", fromStringsFile);

        // Clean value keeps author escapes — no expand-to-real-newline step
        undertest.CleanValueImplementation.ShouldBe(fromStringsFile);

        var emitted = undertest.CleanValueImplementationForDoubleQuotedLiteral;
        // Must not turn \r into \\r (that yielded visible \r\n in the UI)
        emitted.ShouldBe(fromStringsFile);

        var syntax = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(
            "class T { const string S = \"" + emitted + "\"; }"
        );
        var literal = syntax
            .GetRoot()
            .DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax>()
            .Single();
        literal.Token.ValueText.ShouldBe("line1\r\n\r\nline2");
    }

    [Fact]
    public void ResourceString_QuotedTextAndBackslashPath_RoundTripThroughCsharpLiteral()
    {
        // .strings: Say \"hi\" and D:\\data\\file  (avoid \t/\n/\r in path — same as C# literals)
        const string fromStringsFile = "Say \\\"hi\\\" and D:\\\\data\\\\file";
        var undertest = new ResourceString(null, "PathMsg", fromStringsFile);

        var emitted = undertest.CleanValueImplementationForDoubleQuotedLiteral;
        var syntax = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(
            "class T { const string S = \"" + emitted + "\"; }"
        );
        var literal = syntax
            .GetRoot()
            .DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax>()
            .Single();
        literal.Token.ValueText.ShouldBe("Say \"hi\" and D:\\data\\file");
    }

    [Theory]
    [InlineData("My simple value: {0}", "My simple value: {0}", "", "object")]
    [InlineData("My simple value: {0@string}", "My simple value: {0}", "", "string")]
    [InlineData("My simple value: {0:n2@int}", "My simple value: {0:n2}", ":n2", "int")]
    public void ResourceString_StandardParameter_Correctly(string value, string expectedValue, string expectedFormat, string expectedType)
    {
        var undertest = new ResourceString(null, "MyKey", value);

        // Simple assertions
        undertest.Locale.ShouldBeNull();
        undertest.Key.ShouldBe("MyKey");
        undertest.Value.ShouldBe(expectedValue);

        undertest.StringType.ShouldBe(StringType.Format);
        undertest.CleanValueImplementation.ShouldBe(expectedValue);

        undertest.Parameters.Count().ShouldBe(1);
        undertest.Parameters[0].Name.ShouldBe("arg0");
        undertest.Parameters[0].OriginalName.ShouldBe("0");
        undertest.Parameters[0].Format.ShouldBe(expectedFormat);
        undertest.Parameters[0].Type.ShouldBe(expectedType);
        undertest.Parameters[0].Order.ShouldBe(0);
    }

    [Fact]
    public void ResourceString_StandardTwoParameter_Correctly()
    {
        string value = "My {1:n2} simple value: {0@string}";
        string expectedValue = "My {1:n2} simple value: {0}";
        var undertest = new ResourceString(null, "MyKey", value);

        // Simple assertions
        undertest.Locale.ShouldBeNull();
        undertest.Key.ShouldBe("MyKey");
        undertest.Value.ShouldBe(expectedValue);

        undertest.StringType.ShouldBe(StringType.Format);
        undertest.CleanValueImplementation.ShouldBe(expectedValue);

        undertest.Parameters.Count().ShouldBe(2);

        undertest.Parameters[0].Name.ShouldBe("arg0");
        undertest.Parameters[0].OriginalName.ShouldBe("0");
        undertest.Parameters[0].Format.ShouldBe("");
        undertest.Parameters[0].Type.ShouldBe("string");
        undertest.Parameters[0].Order.ShouldBe(0);

        undertest.Parameters[1].Name.ShouldBe("arg1");
        undertest.Parameters[1].OriginalName.ShouldBe("1");
        undertest.Parameters[1].Format.ShouldBe(":n2");
        undertest.Parameters[1].Type.ShouldBe("object");
        undertest.Parameters[1].Order.ShouldBe(1);
    }

    [Theory]
    [InlineData("{0}", "arg0", 0, StringType.Format)]
    [InlineData("{value}", "value", 100, StringType.Interpolation)]
    public void ResourceString_EscapedAndUnescaped_Correctly(string parameter, string parameterName, int expectedOrder, StringType expectedStringType)
    {
        string value = "{{ParameterName}} should have value " + parameter;
        string expectedValue = value;
        var undertest = new ResourceString(null, "MyKey", value);

        // Simple assertions
        undertest.Locale.ShouldBeNull();
        undertest.Key.ShouldBe("MyKey");
        undertest.Value.ShouldBe(expectedValue);

        undertest.StringType.ShouldBe(expectedStringType);
        undertest.CleanValueImplementation.ShouldBe(expectedValue);

        undertest.Parameters.Count().ShouldBe(1);

        undertest.Parameters[0].Name.ShouldBe(parameterName);
        undertest.Parameters[0].OriginalName.ShouldBe(parameter.Trim('{', '}'));
        undertest.Parameters[0].Format.ShouldBe("");
        undertest.Parameters[0].Type.ShouldBe("object");
        undertest.Parameters[0].Order.ShouldBe(expectedOrder);
    }
}
