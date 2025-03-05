using FluentAssertions;
using Strings.ResourceGenerator.Generators.StringsFile;
using Strings.ResourceGenerator.Models;
using UnitTests.Utils;

namespace UnitTests
{
    public class MessagesCompilerTests
    {
        [Fact]
        public void Messages_Compile_OK()
        {
            var lines = File.ReadAllLines(@"..\..\..\..\Strings.ResourceGenerator.Examples\Resources\Messages.strings");

            var config = new StringConfiguration { NameSpace = "Some.Namespace", GeneratePublic = true };

            var generator = StringsProvider.Provide(
                "Messages",
                config,
                ("Messages.strings", lines));

            generator.Config.GeneratePublic.Should().BeTrue();
            generator.Config.NameSpace.Should().Be("Strings.ResourceGenerator.Examples.Resources");
            generator.Config.ExcludeFromCodeCoverage.Should().BeTrue();

            var src = generator.Generate();
#if DUMPGENERATION
            DebugDump.Dump(nameof(MessagesCompilerTests), src);
#endif
            src.Should().NotBeNull();
        }
    }
}
