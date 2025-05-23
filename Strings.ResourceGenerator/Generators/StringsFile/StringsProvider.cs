﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Strings.ResourceGenerator.Generators.Data;
using Strings.ResourceGenerator.Models;

namespace Strings.ResourceGenerator.Generators.StringsFile;

internal static class StringsProvider
{
    private static readonly Regex resourceRegex = new(@"^((?<locale>\w\w(-\w\w)?):)?(((?<key>[a-zA-Z_][\w_]*))=)?(?<value>.*)$");

    public static LocalizerGenerator Provide(
        string clazz,
        StringConfiguration config,
        params (string path, IEnumerable<string> lines)[] resources)
    {
        if (resources.Length > 1)
        {
            // Generate for each
            var generator = new LocalizerGenerator(config, clazz);
            foreach (var (path, lines) in resources)
            {
                var locale = Path.GetFileNameWithoutExtension(path)
                                 .Replace($"{clazz}.", "");
                locale = locale == clazz
                    ? Constants.Neutral
                    : locale.ToUpper();

                var data = new GeneratorData
                {
                    Config = config,
                    Locale = locale,
                    ClassName = $"{config.Prefix}{clazz}",
                    SourceFile = path,
                    IsMultipleLanguages = resources.Length > 1
                };
                generator.AddStringLocalizer(new StringsGenerator(data, lines), locale);
            }

            return generator;
        }
        else
        {
            var sections = SplitBySections(resources.First().lines);
            var (section, lines) = sections.FirstOrDefault(x => x.section == "Configuration");
            var resourceSection = sections.FirstOrDefault(x => x.section != "Configuration");

            var resourceStrings = ToResourceStrings(resourceSection.lines).ToArray();

            if (lines != null)
            {
                config = StringsGatherer.GetConfigFromDictionary(lines
                    .Select(x => (x.Split('=')[0], x.Split('=')[1]))
                    .ToDictionary(x => x.Item1, x => x.Item2),
                    "From strings resource");
            }

            var model = new StringsModel { Config = config, Strings = resourceStrings.ToList() };

            return model.ToGenerator(resources.First().path, clazz, (d, l) => new StringsGenerator(d, l));
        }

    }

    /// <summary>
    /// Returns each [section] with it's own lines, except empty lines
    /// If no [section] is found, then all other lines are returned with the <code>null</code> section
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    private static IEnumerable<(string section, string[] lines)> SplitBySections(IEnumerable<string> lines)
    {
        string currentSection = null;
        var sectionLines = new List<string>();

        foreach (var line in lines)
        {
            if (line.StartsWith("["))
            {
                if (currentSection != null)
                {
                    yield return (currentSection, sectionLines.ToArray());
                }
                currentSection = line.Trim('[', ']');
                sectionLines = [];
            }
            else if (!string.IsNullOrWhiteSpace(line))
            {
                sectionLines.Add(line);
            }
        }

        yield return (currentSection, sectionLines.ToArray());
    }

    /// <summary>
    /// Break the lines into resource string models
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    private static IEnumerable<ResourceStringModel> ToResourceStrings(string[] lines)
    {
        bool inComment = false;
        var current = new ResourceStringModel();

        bool CurrentIsValid()
        {
            return current.Key != null;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.Trim().StartsWith("/*"))
            {
                if (CurrentIsValid())
                {
                    yield return current;
                    current = new ResourceStringModel();
                }

                var cl = line.Trim();
                if (cl.EndsWith("*/"))
                {
                    cl = cl.Substring(0, cl.Length - 2).Trim();
                }
                else
                {
                    inComment = true;
                }
                current.Context = cl.Substring(2).Trim();
            }
            else if (inComment)
            {
                var cl = line.Trim();
                if (cl.EndsWith("*/"))
                {
                    inComment = false;
                    cl = cl.Substring(0, line.Length - 2).Trim();
                }
                current.Context += " " + cl.Trim();
            }
            else if (StringsGenerator.IsCommented(line))
            {
                // TODO: count
                continue;
            }
            else
            {
                var match = resourceRegex.Match(line);

                var locale = match.Groups["locale"];
                var key = match.Groups["key"];
                var value = match.Groups["value"];

                var resourceKey = key.Success ? key.Value : null;
                // If locale is specified, and we have a current entry, then add the value for the same key 
                if (CurrentIsValid() && !key.Success && locale.Success)
                {
                    resourceKey = current.Key;
                }

                if (resourceKey != null && value.Success)
                {
                    // This means we have an identified resource

                    var stringValue = value.Value;

                    if (stringValue.Trim() == StringsGenerator.RawString)
                    {
                        // Multi-line raw string
                        stringValue = "";
                        var j = i + 1;
                        while (j < lines.Length && lines[j].Trim() != StringsGenerator.RawString)
                        {
                            stringValue += lines[j].Trim();
                            if (!stringValue.EndsWith("\\n"))
                            {
                                stringValue += " ";
                            }
                            j++;
                        }

                        if (lines[j].Trim() == StringsGenerator.RawString)
                        {
                            // found the end, use it
                            i = j;
                            stringValue = stringValue.Trim();
                        }
                        else
                        {
                            stringValue = "ERROR: Missing closing \"\"\" for multi-line string";
                        }
                    }

                    if (CurrentIsValid() && resourceKey != current.Key)
                    {
                        yield return current;
                        current = new ResourceStringModel
                        {
                            Key = resourceKey
                        };
                    }
                    else
                    {
                        current.Key ??= resourceKey;
                    }

                    current.Values ??= [];
                    current.Values.Add(new ResourceStringValue { Locale = locale.Success ? locale.Value : null, Value = stringValue });
                }
            }
        }

        if (CurrentIsValid())
        {
            yield return current;
        }
    }
}
