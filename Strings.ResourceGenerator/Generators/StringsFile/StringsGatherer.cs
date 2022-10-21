using Strings.ResourceGenerator.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Strings.ResourceGenerator.Generators.StringsFile
{
    internal static class StringsGatherer
    {
        public static IReadOnlyCollection<LocalizerGenerator> Gather(GeneratorExecutionContext context)
        {
            return Gather().ToList();

            IEnumerable<LocalizerGenerator> Gather()
            {
                // Get all .strings files
                var allStringFiles = context.AdditionalFiles.Where(at => at.Path.EndsWith(".strings"));

                if (allStringFiles.Any())
                {
                    // Get all strings.config files
                    var allStringConfigFiles = context.AdditionalFiles.Where(at => at.Path.EndsWith("strings.config"))
                        .Select(x => (name: Path.GetFileName(x.Path), text: x.GetText()))
                        .ToDictionary(x => x.name, y => y.text.ToString());

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
                                Lines = x.GetText(context.CancellationToken)
                                         .ToString()
                                         .Split('\r', '\n')
                                         .Where(x => !string.IsNullOrEmpty(x))
                                         //.Select(x => x.Text.ToString())
                            })
                            .ToList();

                        yield return StringsProvider.Provide(
                            toCompile.Clazz,
                            config,
                            sources.Select(x => (path: x.File, lines: x.Lines)).ToArray());
                    }
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

                return GetConfigFromDictionary(config);
            }

            return StringConfiguration.DefaultConfiguration;
        }

        internal static StringConfiguration GetConfigFromDictionary(Dictionary<string, string> config)
        {
            return new StringConfiguration
            {
                NameSpace = config.ContainsKey("namespace") ? config["namespace"] : StringConfiguration.DefaultNamespace,
                Prefix = config.ContainsKey("prefix") ? config["prefix"] : "",
                GeneratePublic = config.ContainsKey("public") && config["public"] == "true",
                PreferConstOverStatic = !config.ContainsKey("preferConst") || config.ContainsKey("preferConst") && config["preferConst"] == "true"
            };
        }
    }
}
