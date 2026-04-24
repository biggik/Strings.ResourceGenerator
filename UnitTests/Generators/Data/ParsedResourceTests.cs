using Shouldly;
using Strings.ResourceGenerator.Generators;
using Strings.ResourceGenerator.Generators.Data;
using Strings.ResourceGenerator.Generators.Parsers;

namespace UnitTests.Generators.Data;

public class ParsedResourceTests
{
    [Fact]
    public void ParsedResource_SimpleValue_Validates()
    {
        string locale = null;
        var key = "MyKey";
        var value = "My simple value";

        var res = new ResourceString(locale, key, value);
        var parser = new ResourceParser(locale, key, value);
        var undertest = new ParsedResource(parser, res);

        undertest.StaticProperty.Trim().ShouldBe("public static string MyKey => Current.MyKey;");
        undertest.InterfaceDeclaration.Trim().ShouldBe("string MyKey { get; }");
        undertest.PublicProperty.Trim().ShouldBe("public string MyKey");

        undertest.PublicStaticProperty(false).Trim().ShouldBe("public static string MyKey");
        undertest.PublicStaticProperty(true).Trim().ShouldBe("public const string MyKey");

        undertest.ClassLine(false).Trim().ShouldBe("=> \"My simple value\";");
        undertest.ClassLine(true).Trim().ShouldBe("= \"My simple value\";");
    }

    [Fact]
    public void ParsedResource_StandardFormat_Validates()
    {
        string locale = null;
        var key = "MyKey";
        var value = "My simple {0}";

        var res = new ResourceString(locale, key, value);
        var parser = new ResourceParser(locale, key, value);
        var undertest = new ParsedResource(parser, res);

        undertest.StaticProperty.Trim().ShouldBe("public static string MyKey(object arg0) => Current.MyKey(arg0);");
        undertest.InterfaceDeclaration.Trim().ShouldBe("string MyKey(object arg0);");
        undertest.PublicProperty.Trim().ShouldBe("public string MyKey(object arg0)");

        undertest.PublicStaticProperty(false).Trim().ShouldBe("public static string MyKey(object arg0)");
        undertest.PublicStaticProperty(true).Trim().ShouldBe("public const string MyKey(object arg0)");

        undertest.ClassLine(false).Trim().ShouldBe("=> Format(\"My simple {0}\", arg0);");
        undertest.ClassLine(true).Trim().ShouldBe("= Format(\"My simple {0}\", arg0);");
    }

    [Fact]
    public void ParsedResource_StandardTwoFormat_Validates()
    {
        string locale = null;
        var key = "MyKey";
        var value = "{1} simple {0}";

        var res = new ResourceString(locale, key, value);
        var parser = new ResourceParser(locale, key, value);
        var undertest = new ParsedResource(parser, res);

        undertest.StaticProperty.Trim().ShouldBe("public static string MyKey(object arg0, object arg1) => Current.MyKey(arg0, arg1);");
        undertest.InterfaceDeclaration.Trim().ShouldBe("string MyKey(object arg0, object arg1);");
        undertest.PublicProperty.Trim().ShouldBe("public string MyKey(object arg0, object arg1)");

        undertest.PublicStaticProperty(false).Trim().ShouldBe("public static string MyKey(object arg0, object arg1)");
        undertest.PublicStaticProperty(true).Trim().ShouldBe("public const string MyKey(object arg0, object arg1)");

        undertest.ClassLine(false).Trim().ShouldBe("=> Format(\"{1} simple {0}\", arg0, arg1);");
        undertest.ClassLine(true).Trim().ShouldBe("= Format(\"{1} simple {0}\", arg0, arg1);");
    }

    [Fact]
    public void ParsedResource_InterpolatedFormat_Validates()
    {
        string locale = null;
        var key = "MyKey";
        var value = "My simple {value}";

        var res = new ResourceString(locale, key, value);
        var parser = new ResourceParser(locale, key, value);
        var undertest = new ParsedResource(parser, res);

        undertest.StaticProperty.Trim().ShouldBe("public static string MyKey(object value) => Current.MyKey(value);");
        undertest.InterfaceDeclaration.Trim().ShouldBe("string MyKey(object value);");
        undertest.PublicProperty.Trim().ShouldBe("public string MyKey(object value)");

        undertest.PublicStaticProperty(false).Trim().ShouldBe("public static string MyKey(object value)");
        undertest.PublicStaticProperty(true).Trim().ShouldBe("public const string MyKey(object value)");

        undertest.ClassLine(false).Trim().ShouldBe("=> $\"My simple {value}\";");
        undertest.ClassLine(true).Trim().ShouldBe("= $\"My simple {value}\";");
    }

    [Fact]
    public void ParsedResource_InterpolatedTwoFormat_Validates()
    {
        string locale = null;
        var key = "MyKey";
        var value = "{who} simple {value}";

        var res = new ResourceString(locale, key, value);
        var parser = new ResourceParser(locale, key, value);
        var undertest = new ParsedResource(parser, res);

        undertest.StaticProperty.Trim().ShouldBe("public static string MyKey(object who, object value) => Current.MyKey(who, value);");
        undertest.InterfaceDeclaration.Trim().ShouldBe("string MyKey(object who, object value);");
        undertest.PublicProperty.Trim().ShouldBe("public string MyKey(object who, object value)");

        undertest.PublicStaticProperty(false).Trim().ShouldBe("public static string MyKey(object who, object value)");
        undertest.PublicStaticProperty(true).Trim().ShouldBe("public const string MyKey(object who, object value)");

        undertest.ClassLine(false).Trim().ShouldBe("=> $\"{who} simple {value}\";");
        undertest.ClassLine(true).Trim().ShouldBe("= $\"{who} simple {value}\";");
    }

    [Fact]
    public void ParsedResource_Escaped_Validates()
    {
        string locale = null;
        var key = "MyKey";
        var value = "{{ParameterName}} simple";

        var res = new ResourceString(locale, key, value);
        var parser = new ResourceParser(locale, key, value);
        var undertest = new ParsedResource(parser, res);

        undertest.StaticProperty.Trim().ShouldBe("public static string MyKey => Current.MyKey;");
        undertest.InterfaceDeclaration.Trim().ShouldBe("string MyKey { get; }");
        undertest.PublicProperty.Trim().ShouldBe("public string MyKey");

        undertest.PublicStaticProperty(false).Trim().ShouldBe("public static string MyKey");
        undertest.PublicStaticProperty(true).Trim().ShouldBe("public const string MyKey");

        undertest.ClassLine(false).Trim().ShouldBe("=> \"{ParameterName} simple\";");
        undertest.ClassLine(true).Trim().ShouldBe("= \"{ParameterName} simple\";");
    }

    [Fact]
    public void ParsedResource_MixedEscapedAndInterpolated_Validates()
    {
        string locale = null;
        var key = "MyKey";
        var value = "{{ParameterName}} simple {value@string}";

        var res = new ResourceString(locale, key, value);
        var parser = new ResourceParser(locale, key, value);
        var undertest = new ParsedResource(parser, res);

        undertest.StaticProperty.Trim().ShouldBe("public static string MyKey(string value) => Current.MyKey(value);");
        undertest.InterfaceDeclaration.Trim().ShouldBe("string MyKey(string value);");
        undertest.PublicProperty.Trim().ShouldBe("public string MyKey(string value)");

        undertest.PublicStaticProperty(false).Trim().ShouldBe("public static string MyKey(string value)");
        undertest.PublicStaticProperty(true).Trim().ShouldBe("public const string MyKey(string value)");

        undertest.ClassLine(false).Trim().ShouldBe("=> $\"{{ParameterName}} simple {value}\";");
        undertest.ClassLine(true).Trim().ShouldBe("= $\"{{ParameterName}} simple {value}\";");
    }
}
