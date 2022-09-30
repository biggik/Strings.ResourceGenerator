using Strings.ResourceGenerator.Generators.JsonFile;

namespace UnitTests
{
    public class JsonComplerTests
    {
        [Fact]
        public void MultiLocaleResourceTest()
        {
            var json = File.ReadAllText(@"..\..\..\..\Strings.ResourceGenerator.Examples\Resources\JsonExample.json");
            var generator = JsonProvider.Provide("json.json", "MyClass", json);
            var src = generator.Generate();
            File.WriteAllText(@"c:\tmp\generated.cs", src);
            Assert.NotNull(src);
        }
    }
}