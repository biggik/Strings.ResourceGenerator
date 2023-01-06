using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Strings.ResourceGenerator.Generators.Data;
using Strings.ResourceGenerator.Helpers;
using Strings.ResourceGenerator.Models;

namespace Strings.ResourceGenerator.Generators.Parsers
{
    /// <summary>
    /// A class that parses a resource string
    /// </summary>
    internal class ResourceParser
    {
        internal static Regex ParameterRegex { get; } = CreateRegex();
        static readonly string doubleEscape = Guid.NewGuid().ToString();
        static readonly string escapedLeftBracket = Guid.NewGuid().ToString();
        static readonly string escapedRightBracket = Guid.NewGuid().ToString();
        static readonly string doubleLeftBracket = Guid.NewGuid().ToString();
        static readonly string doubleRightBracket = Guid.NewGuid().ToString();

        private static Regex CreateRegex()
        {
            const string rxFormat = @"(?<format>\:[a-zA-Z0-9\-\/.]+)?";   // optional format specifier
            const string rxType = @"(\@(?<type>[a-zA-Z]+[a-zA-Z0-9.]*))?"; // an optional type specifier
            const string rxOrder = @"(\@(?<order>[0-9]+))?";              // an optional signature order identifier

            var parts =
               $@"
                (
                    (?<standard>                                  // Using string.Format
                        \{{                                       // opening bracket for replacement
                            (?<part>[0-9]+)                       // the replacement identifier (number from 0 upwards)
                            {rxFormat}
                            {rxType}
                            {rxOrder}
                        \}}                                       // closing bracket for replacement
                    )
                    |                                             // or
                    (?<interpolated>                              // Using interpolated string
                        \{{                                       // opening bracket for replacement
                            (?<part>[a-zA-Z][a-zA-Z0-9_]*)        // the replacement identifier (named parameter)
                            {rxFormat}
                            {rxType}
                            {rxOrder}
                        \}}                                       // closing bracket for replacement
                    )
                )
                ";

            var regexString =
                string.Join("",
                    from part in parts.Split('\r', '\n')
                    let pos = part.IndexOf("   //")
                    let rpart = part.Substring(0, pos > 0 ? pos : part.Length)
                    select rpart.Trim()
                );
            return new Regex(regexString, RegexOptions.Compiled);
        }

        /// <summary>
        /// Parameters to the resource string
        /// </summary>
        public List<ResourceStringParameter> Parameters { get; } = new List<ResourceStringParameter>();

        /// <summary>
        /// A list of signature parameters with type
        /// </summary>
        public List<string> SignatureParametersWithType { get; private set; }
        
        /// <summary>
        /// A list of signature parameter names
        /// </summary>
        public List<string> SignatureParameters { get; private set; }

        /// <summary>
        /// The locale this resource belongs to
        /// </summary>
        public string Locale { get; }

        /// <summary>
        /// The resource key - i.e. the name of the resource string
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The original resource string
        /// </summary>
        public string OriginalValue { get; }

        /// <summary>
        /// The parsed resource string
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// The string type of this resource string
        /// </summary>
        public StringType StringType { get; }

        /// <summary>
        /// Constructs a new instance of a resource string
        /// </summary>
        /// <param name="locale">The locale this resource belongs to</param>
        /// <param name="key">The key to this resource</param>
        /// <param name="value">The raw value of the resource</param>
        /// <exception cref="ArgumentException">In case of a bad parameter declaration, this exception is thrown</exception>
        /// <exception cref="InvalidDataException">In case of a mix of {0} and {interpolated} parameters, this exception is thrown</exception>
        public ResourceParser(string locale, string key, string value)
        {
            Key = key;
            OriginalValue = value;
            Locale = locale;

            // Remove escaped values
            value = value
                .Replace("\\\\", doubleEscape)
                .Replace("{{", doubleLeftBracket)
                .Replace("}}", doubleRightBracket)
                .Replace("\\{", escapedLeftBracket)
                .Replace("\\}", escapedRightBracket);

            var matches = ParameterRegex.Matches(value);

            if (matches.Count > 0)
            {
                string ParameterName(Match m) => m.Groups["part"].Success 
                    ? m.Groups["part"].Value 
                    : throw new ArgumentException(@"No 'part' found in {s}");
                string ParameterFormat(Match m) => m.Groups["format"].Success 
                    ? m.Groups["format"].Value 
                    : "";
                (string type, string key) ParameterTypeAndKey(Match m)
                {
                    bool isStandard = m.Groups["standard"].Success;
                    var paramKey = $"{key}.{(isStandard ? "arg" : "")}{ParameterName(m)}";
                    var type = m.Groups["type"].Success
                        ? m.Groups["type"].Value
                        : "object";
                    return (type, paramKey);
                }
                int index = 0;
                int Order(Match m) => m.Groups["order"].Success
                    ? int.Parse(m.Groups["order"].Value)
                    : m.Groups["standard"].Success
                        ? int.Parse(ParameterName(m))
                        : 100 + index++;

                var parameterComparer = new ParameterComparer();

                List<CapturedData> Parse(IEnumerable<Match> matches2, Func<string, string> nameFunc, Func<string, string> prefixFunc)
                {
                    return (from m in matches2
                            let name = nameFunc(ParameterName(m))
                            let format = ParameterFormat(m)
                            let typeAndKey = ParameterTypeAndKey(m)
                            let order = Order(m)
                            orderby order
                            select new CapturedData(m.Value, typeAndKey.type, typeAndKey.key, format, prefixedName: prefixFunc(name), name, order)
                           )
                           .Distinct(parameterComparer)
                           .ToList();
                }

                IEnumerable<Match> GroupMatches(string groupName)
                                     => from m in matches.Cast<Match>()
                                        where m.Groups[groupName].Success
                                        select m;
                var standard = Parse(GroupMatches("standard"),
                                     x => x,
                                     x => $"arg{x}");
                var interpolated = Parse(GroupMatches("interpolated"),
                                     x => NameHelper.CreateValidIdentifier(x),
                                     x => x);

                if (standard.Any() && interpolated.Any())
                {
                    var std = string.Join(", ", standard.Select(x => x.Original));
                    var ipl = string.Join(", ", interpolated.Select(x => x.Original));
                    throw new InvalidDataException($"[{locale}]{key}: A mix of standard replacements ({std}) and interpolation ({ipl}) is not supported. Please select one or the other");
                }

                // Ensure that we have valid identifiers for interpolation
                // TODO: do this after replacements below ?
                var fixesMap = (from m in GroupMatches("interpolated")
                                let name = ParameterName(m)
                                let fixedName = NameHelper.CreateValidIdentifier(name)
                                where name != fixedName
                                select (name, fixedName)
                               ).Distinct().ToList();

                if (fixesMap.Any())
                {
                    foreach (var (name, fixedName) in fixesMap)
                    {
                        value = value.Replace($"{{{name}}}", $"{{{fixedName}}}");
                    };
                }

                var signature = standard.Any() ? standard : interpolated;

                Parameters = (from part in signature
                              select new ResourceStringParameter(part.PrefixedName, 
                                                                 part.Name, 
                                                                 part.Format, 
                                                                 part.Type, 
                                                                 part.Order)
                             ).ToList();

                SignatureParametersWithType = (from part in signature select $"{part.Type} {part.PrefixedName}").ToList();
                SignatureParameters = (from part in signature select part.PrefixedName).ToList();

                StringType = standard.Any()
                    ? StringType.Format
                    : StringType.Interpolation;

                foreach (var data in signature)
                {
                    value = value
                        // name:format@type@order => name:format
                        .Replace($"{{{data.Name}{data.Format}@{data.Type}@{data.Order}}}", $"{{{data.Name}{data.Format}}}")
                        // name@type@order => name
                        .Replace($"{{{data.Name}@{data.Type}@{data.Order}}}", $"{{{data.Name}}}")
                        // name:format@type => name:format
                        .Replace($"{{{data.Name}{data.Format}@{data.Type}}}", $"{{{data.Name}{data.Format}}}")
                        // name:format@order => name:format
                        .Replace($"{{{data.Name}{data.Format}@{data.Order}}}", $"{{{data.Name}{data.Format}}}")
                        // name@type => name
                        .Replace($"{{{data.Name}@{data.Type}}}", $"{{{data.Name}}}")
                        // name@order => name
                        .Replace($"{{{data.Name}@{data.Order}}}", $"{{{data.Name}}}")
                        ;
                }
            }
            else
            {
                StringType = StringType.Simple;
            }

            // Set value, and replace magic values with unescaped values
            Value = value
                        .Replace(escapedRightBracket, "}")
                        .Replace(escapedLeftBracket, "{")
                        .Replace(doubleRightBracket, "}}")
                        .Replace(doubleLeftBracket, "{{")
                        .Replace(doubleEscape, "\\\\")

                        // And simplify unnecessary escapes
                        .Replace("\\\\r", "\\r")
                        .Replace("\\\\n", "\\n")
                        .Replace("\\\\t", "\\t")
                        ;
        }
    }
}
