using FluentAssertions;
using Strings.ResourceGenerator.Generators.JsonFile;

namespace UnitTests;

public class JsonCompilerTests
{
    [Fact]
    public void MultiLocaleResourceTest()
    {
        var json = File.ReadAllText(@"..\..\..\..\Strings.ResourceGenerator.Examples\Resources\JsonExample.json");
        var generator = JsonProvider.Provide("json.json", "MyClass", json);
        var src = generator.Generate();
        File.WriteAllText($@"c:\tmp\generated.{nameof(JsonCompilerTests)}.cs", src);
        src.Should().NotBeNull();
    }
}