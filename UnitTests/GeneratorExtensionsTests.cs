using System.Text;
using Shouldly;
using Strings.ResourceGenerator.Generators;
using Strings.ResourceGenerator.Generators.Data;
using Strings.ResourceGenerator.Generators.Interfaces;
using Strings.ResourceGenerator.Generators.YamlFile;
using Strings.ResourceGenerator.Models;

namespace UnitTests;

public class GeneratorExtensionsTests
{
    private sealed class StubGenerator : IGenerator
    {
        public required GeneratorData Data { get; init; }
    }

    [Fact]
    public void InitializeResources_OrdersByKey()
    {
        var gen = new StubGenerator
        {
            Data = new GeneratorData { Locale = Constants.Neutral }
        };

        var list = new List<ResourceStringModel>
        {
            new() { Key = "Zed", Value = "z" },
            new() { Key = "Alpha", Value = "a" },
        };

        var resources = gen.InitializeResources(list);
        resources.Select(r => r.Key).ShouldBe(new[] { "Alpha", "Zed" });
    }

    [Fact]
    public void InitializeResources_PicksMatchingLocale_InSingleValueRow()
    {
        var gen = new StubGenerator
        {
            Data = new GeneratorData { Locale = "DE" }
        };

        var list = new List<ResourceStringModel>
        {
            new()
            {
                Key = "Greeting",
                Values =
                [
                    new ResourceStringValue { Locale = "en", Value = "Hi" },
                    new ResourceStringValue { Locale = "de", Value = "Hallo" },
                ]
            }
        };

        var resources = gen.InitializeResources(list);
        resources.ShouldHaveSingleItem();
        resources[0].Value.ShouldBe("Hallo");
    }

    [Fact]
    public void InitializeResources_NeutralRow_WithEmptyLocaleValue()
    {
        var gen = new StubGenerator
        {
            Data = new GeneratorData { Locale = Constants.Neutral }
        };

        var list = new List<ResourceStringModel>
        {
            new()
            {
                Key = "N",
                Values = [new ResourceStringValue { Locale = "", Value = "neutral-only" }]
            }
        };

        var resources = gen.InitializeResources(list);
        resources.ShouldHaveSingleItem();
        resources[0].Value.ShouldBe("neutral-only");
    }

    [Fact]
    public void ToGenerator_SingleLanguage_YieldsOneLocalizer_NeutralOnly()
    {
        var model = new StringsModel
        {
            Config = StringConfiguration.DefaultConfiguration,
            Strings =
            [
                new ResourceStringModel { Key = "A", Value = "v" }
            ]
        };

        var loc = model.ToGenerator("file.yaml", "My", (d, strings) => new YamlGenerator(d, strings));

        loc.IsMultipleLanguages.ShouldBeFalse();
        loc.Generate().ShouldContain("My");
    }

    [Fact]
    public void ToGenerator_MultiLocale_YieldsMultipleLanguages()
    {
        // Multi-language generation expects a Neutral bucket (see LocalizerGenerator.Generate).
        var model = new StringsModel
        {
            Config = StringConfiguration.DefaultConfiguration,
            Strings =
            [
                new ResourceStringModel
                {
                    Key = "Greeting",
                    Values =
                    [
                        new ResourceStringValue { Locale = "", Value = "Hello" },
                        new ResourceStringValue { Locale = "en", Value = "Hi" },
                        new ResourceStringValue { Locale = "is", Value = "Hæ" },
                    ]
                }
            ]
        };

        var loc = model.ToGenerator("file.yaml", "Loc", (d, strings) => new YamlGenerator(d, strings));

        loc.IsMultipleLanguages.ShouldBeTrue();
        var src = loc.Generate();
        src.ShouldContain("InitializeLocalizerFor");
        src.ShouldContain("EN");
        src.ShouldContain("IS");
    }

    [Fact]
    public void AddExcludeAttribute_WhenApplied_AppendsIndentedLine()
    {
        var sb = new StringBuilder();
        sb.AddExcludeAttribute(apply: true, "Custom", "    ");
        var text = sb.ToString();
        text.ShouldContain("ExcludeFromCodeCoverage");
        text.ShouldContain("Custom");
    }

    [Fact]
    public void AddExcludeAttribute_WhenNotApplied_LeavesBuilderEmpty()
    {
        var sb = new StringBuilder();
        sb.AddExcludeAttribute(apply: false, "ignored", "    ");
        sb.Length.ShouldBe(0);
    }
}
