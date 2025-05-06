using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Strings.ResourceGenerator.Models;

namespace Strings.ResourceGenerator.Generators.StringsFile;

internal static class StringsGatherer
{
    public static IEnumerable<LocalizerGenerator> Gather(IEnumerable<AdditionalText> additionalFiles, System.Threading.CancellationToken cancellationToken)
    {
        // Get all .strings files from AdditionalText
        var allStringFiles = additionalFiles
            .Where(at => at.Path.EndsWith(".strings"));

        if (allStringFiles.Any())
        {
            // Get all strings.config files
            var allStringConfigFiles = additionalFiles
                .Where(at => at.Path.EndsWith("strings.config"))
                .Select(x => (name: Path.GetFileName(x.Path), text: x.GetText()?.ToString()))
                .Where(x => x.text != null)
                .ToDictionary(x => x.name, y => y.text);

            // Group all .strings files by full path + base name (e.g. mystrings from mystrings.strings and mystrings.is.strings)
            var results = from file in allStringFiles
                          group file by Path.Combine(
                              Path.GetDirectoryName(file.Path),
                              Path.GetFileName(file.Path).Split('.')[0]) into g
                          orderby g.Key.Length
                          select new
                          {
                              Directory = Path.GetDirectoryName(g.Key),
                              Clazz = Path.GetFileName(g.Key).Split('.')[0],
                              Files = g.ToList()
                          };

            // Get the config for the .strings file, the generic config, or defaults
            var config = GetConfig(allStringConfigFiles, results.First().Clazz);

            // For each such group, compile
            foreach (var toCompile in results)
            {
                var sources = toCompile.Files.Select(x =>
                    new
                    {
                        Locale = Path.GetFileNameWithoutExtension(x.Path)
                                     .Replace($"{toCompile.Clazz}.", ""),
                        File = x.Path,
                        Lines = x.GetText(cancellationToken)?.ToString()
                                     ?.Split('\r', '\n')
                                     ?.Where(line => !string.IsNullOrEmpty(line))
                    })
                    .Where(x => x.Lines != null)
                    .ToList();

                yield return StringsProvider.Provide(
                    toCompile.Clazz,
                    config,
                    sources.Select(x => (path: x.File, lines: x.Lines!)).ToArray());
            }
        }
    }

    private static StringConfiguration GetConfig(Dictionary<string, string> lookup, string clazz)
    {
        var key = clazz;
        if (!lookup.ContainsKey(key))
        {
            // See if the generic one is there
            key = "strings.config";
        }
        if (lookup.ContainsKey(key))
        {
            var config = lookup[key]
                            .Split('\r', '\n')
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Select(x => (x.Split('=')[0], x.Split('=')[1]))
                            .ToDictionary(x => x.Item1, x => x.Item2);

            return GetConfigFromDictionary(config, "From strings.config");
        }

        return StringConfiguration.DefaultConfiguration;
    }

    internal static StringConfiguration GetConfigFromDictionary(Dictionary<string, string> config, string source)
    {
        return new StringConfiguration
        {
            NameSpace = config.ContainsKey(Constants.Namespace) ? config[Constants.Namespace] : StringConfiguration.DefaultNamespace,
            Prefix = config.ContainsKey(Constants.Prefix) ? config[Constants.Prefix] : "",
            GeneratePublic = config.ContainsKey(Constants.Public) && config[Constants.Public] == "true",
            PreferConstOverStatic = !config.ContainsKey(Constants.PreferConst) || config[Constants.PreferConst] == "true",
            ExcludeFromCodeCoverage = !config.ContainsKey(Constants.ExcludeCoverage) || config[Constants.ExcludeCoverage] == "true",
            ExcludeFromCodeCoverageMessage = (config.ContainsKey(Constants.ExcludeCoverageMessage) && !string.IsNullOrWhiteSpace(config[Constants.ExcludeCoverageMessage]))
                ? config[Constants.ExcludeCoverageMessage]
                : StringConfiguration.DefaultExclusionJustification,
            ConfigurationSource = source
        };
    }
}
