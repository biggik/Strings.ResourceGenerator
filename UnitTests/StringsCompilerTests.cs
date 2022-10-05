using Strings.ResourceGenerator.Generators;
using Strings.ResourceGenerator.Models;
using Strings.ResourceGenerator.Generators.Data;
using Strings.ResourceGenerator.Generators.StringsFile;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnitTests
{
    public class StringsComplerTests
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
                    }
                    ),
                new (
                    "Strings.is.strings",
                    new string[]
                    {
                        "InterpolatedFormatString=�etta er strengur sem notar {interpolation} formun til a� b�ta vi� f�ribreytum",
                        "InterpolatedWithFormatting={number:n2} er formu� tala me� tveim aukast�fum",
                        "InterpolatedWithOrderingTypeAndFormatting={number:n2@int@1} er formu�, t�pu� og r��u� {description@string@0}, me� tveim aukast�fum",
                        "InterpolatedWithTypeAndFormatting={number:o@DateTime} er formu�, t�pu� dagsetning, me� roundtrip formun",
                        "SimpleString=�etta er einfaldur strengur",
                        "StandardFormatString=�etta er strengur sem notar {0} formun til a� b�ta vi� f�ribreytum",
                        "StandardWithFormatting={0:n2} er formu� tala me� tveim aukast�fum",
                        "StandardWithOrderingTypeAndFormatting={0:n2@int@1} er formu�, t�pu� og r��u� {1@string@0}, me� tveim aukast�fum",
                        "StandardWithTypeAndFormatting={0:o@DateTime} er formu�, t�pu� dagsetning, me� roundtrip formun",
                    }
                    )
            };

            var config = new StringConfiguration { NameSpace = "Some.Namespace", GeneratePublic = true };
            var generator = StringsProvider.Provide(
                "Strings", 
                config,
                sources.Select(x => (path: x.file, lines: x.lines.Select(x => x))).ToArray());

            var src = generator.Generate();
            File.WriteAllText(@"c:\tmp\generated.cs", src);
            Assert.NotNull(src);
        }
    }
}