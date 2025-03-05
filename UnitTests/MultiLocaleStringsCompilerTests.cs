using FluentAssertions;
using Strings.ResourceGenerator.Generators.StringsFile;
using Strings.ResourceGenerator.Models;
using UnitTests.Utils;

namespace UnitTests
{
    public class MultiLocaleStringsCompilerTests
    {
        [Fact]
        public void MultiLocaleResourceTest()
        {
            var lines = File.ReadAllLines(@"..\..\..\..\Strings.ResourceGenerator.Examples\Resources\MultiLocaleStrings.strings");

            var config = new StringConfiguration { NameSpace = "Some.Namespace", GeneratePublic = true };

            var generator = StringsProvider.Provide(
                "MultiLocaleStrings",
                config,
                ("MultiLocaleStrings.strings", lines));

            generator.Config.GeneratePublic.Should().BeTrue();
            generator.Config.NameSpace.Should().Be("Strings.ResourceGenerator.Examples.Resources");
            generator.Config.ExcludeFromCodeCoverage.Should().BeTrue();

            var src = generator.Generate();
#if DUMPGENERATION
            DebugDump.Dump(nameof(MultiLocaleStringsCompilerTests), src);
#endif
            src.Should().NotBeNull();
        }
    }
}