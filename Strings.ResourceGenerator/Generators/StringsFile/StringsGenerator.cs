using Strings.ResourceGenerator.Generators.Data;
using Strings.ResourceGenerator.Generators.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Strings.ResourceGenerator.Generators.StringsFile
{
    internal class StringsGenerator : IGenerator
    {
        public GeneratorData Data { get; }

        /// <summary>
        /// Creates a new instance of StringGenerator and parses lines from a .strings
        /// file for resources
        /// </summary>
        /// <param name="data">The data for the generator</param>
        /// <param name="lines">Source lines for generation</param>
        public StringsGenerator(GeneratorData data, IEnumerable<string> lines)
        {
            Data = data;
            Data.Resources = InitializeResources(lines);
        }

        private List<ResourceString> InitializeResources(IEnumerable<string> lines)
        {
            return (from r in GetEntries(lines).OrderBy(x => x.Key).ToList()
                    select new ResourceString(Data.Locale, r.Key, r.Value)
                   ).ToList();
        }

        private IEnumerable<KeyValuePair<string, string>> GetEntries(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
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
        private static bool IsCommented(string line)
        {
            line = line.TrimStart();
            return line.Length > 0 && line[0] == '#'
                || line.Length > 1 && line[0] == '/' && line[1] == '/';
        }
    }
}
