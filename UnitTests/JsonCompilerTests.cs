using FluentAssertions;
using Strings.ResourceGenerator.Generators.JsonFile;
using UnitTests.Utils;

namespace UnitTests
{
    public class JsonCompilerTests
    {
        [Fact]
        public void MultiLocaleResourceTest()
        {
            var json = File.ReadAllText(@"..\..\..\..\Strings.ResourceGenerator.Examples\Resources\JsonExample.json");
            var generator = JsonProvider.Provide("json.json", "MyClass", json);
            var src = generator.Generate();
#if DUMPGENERATION
            DebugDump.Dump(nameof(JsonCompilerTests), src);
#endif
            src.Should().NotBeNull();
        }
    }
}