using System;
using System.Collections.Generic;
using System.Linq;
using Strings.ResourceGenerator.Generators.Data;
using Strings.ResourceGenerator.Generators.Interfaces;
using Strings.ResourceGenerator.Models;

namespace Strings.ResourceGenerator.Generators.StringsFile;

internal class StringsGenerator : IGenerator
{
    internal const string RawString = "\"\"\"";

    public GeneratorData Data { get; }

    /// <summary>
    /// Creates a new instance of StringGenerator and parses lines from a .strings
    /// file for resources
    /// </summary>
    /// <param name="data">The data for the generator</param>
    /// <param name="strings">All strings from the strings file</param>
    public StringsGenerator(GeneratorData data, List<ResourceStringModel> strings)
    {
        Data = data;
        Data.Resources = this.InitializeResources(strings);
    }

    /// <summary>
    /// Creates a new instance of StringGenerator and parses lines from a .strings
    /// file for resources
    /// </summary>
    /// <param name="data">The data for the generator</param>
    /// <param name="lines">Source lines for generation</param>
    public StringsGenerator(GeneratorData data, IEnumerable<string> lines)
    {
        Data = data;
        Data.Resources = InitializeResources(lines.ToList());
    }

    private List<ResourceString> InitializeResources(IReadOnlyList<string> lines)
    {
        return (from r in GetEntries(lines).OrderBy(x => x.Key).ToList()
                select new ResourceString(Data.Locale, r.Key, r.Value)
               ).ToList();
    }

    private IEnumerable<KeyValuePair<string, string>> GetEntries(IReadOnlyList<string> lines)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            if (IsCommented(line.TrimStart()))
            {
                Data.CommentedLines++;
                continue;
            }

            // Parse the line to a key / value pair
            string key, value;
            try
            {
                var pos = line.IndexOf("=");
                key = line.Substring(0, pos).Trim();
                value = line.Substring(pos + 1).Trim();
                if (value.Trim() == RawString)
                {
                    // Multi-line raw string
                    value = "";
                    var j = i + 1;
                    while (j < lines.Count && lines[j].Trim() != RawString)
                    {
                        value += lines[j].Trim();
                        if (!value.EndsWith("\\n"))
                        {
                            value += " ";
                        }
                        j++;
                    }

                    if (lines[j].Trim() == RawString)
                    {
                        // found the end, use it
                        i = j;
                        value = value.Trim();
                    }
                    else
                    {
                        value = "ERROR: Missing closing \"\"\" for multi-line string";
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidOperationException($"{line} cannot be broken into a key/value pair");
            }
            value = value.Replace("\"", "\\\"");
            yield return new KeyValuePair<string, string>(key, value);
        }
    }

    /// <summary>
    /// Indicates whether the line is a comment line
    /// Comments are supported by the line starting either with
    ///    #
    ///    //
    /// </summary>
    /// <param name="line">The line to check</param>
    /// <returns>True if a comment line</returns>
    internal static bool IsCommented(string line)
    {
        line = line.TrimStart();
        return (line.Length > 0 && line[0] == '#')
            || (line.Length > 1 && line[0] == '/' && line[1] == '/');
    }
}
