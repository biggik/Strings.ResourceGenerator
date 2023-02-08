using Strings.ResourceGenerator.Models;
using Strings.ResourceGenerator.Generators.StringsFile;
using FluentAssertions;

namespace UnitTests
{
    public class StringsCompilerTests
    {
        [Fact]
        public void MuliLocaleResourceTest()
        {
            var sources = new (string file, string[] lines)[]
            {
                new (
                    "Strings.strings",
                    new string[]
                    {
                        "InterpolatedFormatString=This is a string that uses {interpolation} format to add parameters",
                        "InterpolatedWithFormatting={number:n2} is a formatted number with two decimals",
                        "InterpolatedWithOrderingTypeAndFormatting={number:n2@int@1} is a formatted, typed and ordered {description@string@0}, with two decimals",
                        "InterpolatedWithTypeAndFormatting={number:o@DateTime} is a formatted, typed date and time, with roundtrip formatting",
                        "SimpleString=This is a simple string",
                        "StandardFormatString=This is a string that uses {0} format to add parameters",
                        "StandardWithFormatting={0:n2} is a formatted number with two decimals",
                        "StandardWithOrderingTypeAndFormatting={0:n2@int@1} is a formatted, typed and ordered number {1@string@0}, with two decimals",
                        "StandardWithTypeAndFormatting={0:o@DateTime} is a formatted, typed date and time, with roundtrip formatting",
                        "MixOfExcapedAndUnescaped={{Escaped}} and {unescaped}",
                    }
                    ),
                new (
                    "Strings.is.strings",
                    new string[]
                    {
                        "InterpolatedFormatString=Þetta er strengur sem notar {interpolation} formun til að bæta við færibreytum",
                        "InterpolatedWithFormatting={number:n2} er formuð tala með tveim aukastöfum",
                        "InterpolatedWithOrderingTypeAndFormatting={number:n2@int@1} er formuð, týpuð og röðuð {description@string@0}, með tveim aukastöfum",
                        "InterpolatedWithTypeAndFormatting={number:o@DateTime} er formuð, týpuð dagsetning, með roundtrip formun",
                        "SimpleString=Þetta er einfaldur strengur",
                        "StandardFormatString=Þetta er strengur sem notar {0} formun til að bæta við færibreytum",
                        "StandardWithFormatting={0:n2} er formuð tala með tveim aukastöfum",
                        "StandardWithOrderingTypeAndFormatting={0:n2@int@1} er formuð, týpuð og röðuð {1@string@0}, með tveim aukastöfum",
                        "StandardWithTypeAndFormatting={0:o@DateTime} er formuð, týpuð dagsetning, með roundtrip formun",
                        "MixOfExcapedAndUnescaped={{Escaped}} og {unescaped}",
                    }
                )
            };

            var config = new StringConfiguration { NameSpace = "Some.Namespace", GeneratePublic = true };
            var generator = StringsProvider.Provide(
                "Strings", 
                config,
                sources.Select(x => (path: x.file, lines: x.lines.Select(x => x))).ToArray());

            var src = generator.Generate();
            File.WriteAllText($@"c:\tmp\generated.{nameof(StringsCompilerTests)}.cs", src);
            src.Should().NotBeNull();
        }

        [Fact]
        public void MultiLanguageExampleTests()
        {
            var lines = File.ReadAllLines(@"..\..\..\..\Strings.ResourceGenerator.Examples\Resources\MultiLanguageExample.strings");

            var config = new StringConfiguration { NameSpace = "Some.Namespace", GeneratePublic = true };

            var generator = StringsProvider.Provide(
                "MultiLanguageExampleStrings",
                config,
                ("MultiLanguageExample.strings", lines));

            generator.Config.GeneratePublic.Should().BeTrue();
            generator.Config.NameSpace.Should().Be("Some.Namespace");
            generator.Config.ExcludeFromCodeCoverage.Should().BeTrue();

            var src = generator.Generate();
            File.WriteAllText($@"c:\tmp\generated.{nameof(StringsCompilerTests)}Multi.cs", src);
            src.Should().NotBeNull();
        }

        [Fact]
        public void MuliLocaleResourceTestaaa()
        {
            var sources = new (string file, string[] lines)[]
            {
                new (
                    "Strings.strings",
                    new string[]
                    {
                        "MixOfExcapedAndUnescaped={{Escaped}} and {unescaped}",
                    }
                    ),
                new (
                    "Strings.is.strings",
                    new string[]
                    {
                        "MixOfExcapedAndUnescaped={{Escaped}} og {unescaped}",
                    }
                )
            };

            var config = new StringConfiguration { NameSpace = "Some.Namespace", GeneratePublic = true };
            var generator = StringsProvider.Provide(
                "Strings",
                config,
                sources.Select(x => (path: x.file, lines: x.lines.Select(x => x))).ToArray());

            var src = generator.Generate();
            File.WriteAllText($@"c:\tmp\generated.{nameof(StringsCompilerTests)}.cs", src);
            src.Should().NotBeNull();
        }
    }
}