using Shouldly;
using Strings.ResourceGenerator.Generators;
using Strings.ResourceGenerator.Generators.StringsFile;
using Strings.ResourceGenerator.Models;

namespace UnitTests;

public class StringsGathererConfigTests
{
    [Fact]
    public void GetConfigFromDictionary_Empty_UsesBuiltInDefaults()
    {
        var cfg = StringsGatherer.GetConfigFromDictionary([], "unit test");

        cfg.NameSpace.ShouldBe(StringConfiguration.DefaultNamespace);
        cfg.Prefix.ShouldBe("");
        cfg.GeneratePublic.ShouldBeFalse();
        cfg.ConfigurationSource.ShouldBe("unit test");
    }

    [Fact]
    public void GetConfigFromDictionary_AllKeys_AppliesParsingRules()
    {
        var cfg = StringsGatherer.GetConfigFromDictionary(
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [Constants.Namespace] = "My.App.Resources",
                [Constants.Prefix] = "Loc",
                [Constants.Public] = "true",
                [Constants.PreferConst] = "false",
                [Constants.ExcludeCoverage] = "false",
                [Constants.ExcludeCoverageMessage] = "Because tests",
            },
            "strings.config");

        cfg.NameSpace.ShouldBe("My.App.Resources");
        cfg.Prefix.ShouldBe("Loc");
        cfg.GeneratePublic.ShouldBeTrue();
        cfg.PreferConstOverStatic.ShouldBeFalse();
        cfg.ExcludeFromCodeCoverage.ShouldBeFalse();
        cfg.ExcludeFromCodeCoverageMessage.ShouldBe("Because tests");
    }

    [Fact]
    public void GetConfigFromDictionary_BlankExcludeMessage_FallsBackToDefaultJustification()
    {
        var cfg = StringsGatherer.GetConfigFromDictionary(
            new Dictionary<string, string>
            {
                [Constants.ExcludeCoverageMessage] = "   ",
            },
            "x");

        cfg.ExcludeFromCodeCoverageMessage.ShouldBe(StringConfiguration.DefaultExclusionJustification);
    }
}
