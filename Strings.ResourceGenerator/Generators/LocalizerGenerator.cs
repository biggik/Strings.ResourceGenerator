﻿using Strings.ResourceGenerator.Data.Generators;
using Strings.ResourceGenerator.Exceptions;
using Strings.ResourceGenerator.Generators.Interfaces;
using Strings.ResourceGenerator.Models;
using Strings.ResourceGenerator.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Strings.ResourceGenerator.Generators
{
    internal class LocalizerGenerator
    {
        private readonly Dictionary<string, IGenerator> generators = new();

        internal LocalizerGenerator(StringConfiguration config, string clazz)
        {
            this.config = config;
            Clazz = clazz;
        }

        private string AccessModifier => config.GeneratePublic ? "public" : "internal";
        private string Prefix => config.Prefix;

        /// <summary>
        /// Add a generator for a locale
        /// </summary>
        /// <param name="generator"></param>
        public void AddStringLocalizer(IGenerator generator, string locale)
        {
            if (string.IsNullOrWhiteSpace(locale))
            {
                locale = Constants.Neutral;
            }
            generators[locale] = generator;
        }

        public bool IsMultipleLanguages => generators.Count > 1;

        public string Clazz { get; }
        private readonly List<string> errors = new();
        private readonly StringConfiguration config;

        /// <summary>
        /// Generate resource classes for the localizer
        /// </summary>
        /// <param name="nameSpace">The namespace to generate the localizer in</param>
        /// <param name="clazz">The class name for the localizer</param>
        /// <returns>The text for the string localizer</returns>
        /// <exception cref="StringGeneratorException"></exception>
        public string Generate()
        {
            return string.Join(Environment.NewLine, Lines());

            IEnumerable<string> Lines()
            {
                IEnumerable<string> Headers()
                {
                    const string prefix = $"{Constants.Ind1}// ";

                    yield return $"{prefix} ResurceAccessorGenerator by Status ehf - Generated {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} (UTC)";
                    yield return $"{prefix} Generatord from: {string.Join(", ", generators.Select(x => x.Value.Data.SourceFile))}";
                    foreach (var line in generators.First().Value.Headers(prefix))
                    {
                        yield return line;
                    }

                    var v = GetType().Assembly.GetName().Version;
                    yield return $"{Constants.Ind1}/// <summary>";
                    yield return $"{Constants.Ind1}/// Generated string accessor class for {Clazz}";
                    yield return $"{Constants.Ind1}/// </summary>";
                }

                yield return "// <auto-generated/>";
                yield return "";

                // Setup the namespace/class;
                yield return "using System;";
                yield return "using System.Collections.Generic;";
                yield return "using System.Globalization;";
                yield return "";
                yield return $"namespace {config.NameSpace}";
                yield return "{";
                foreach (var header in Headers())
                {
                    yield return header;
                }
                yield return $"{Constants.Ind1}{AccessModifier} static class {Prefix}{Clazz}";
                yield return $"{Constants.Ind1}{{";

                if (IsMultipleLanguages)
                {
                    yield return $"{Constants.Ind2}private static readonly Lazy<IGeneratedLocalizerFor{Clazz}> _localizer = new Lazy<IGeneratedLocalizerFor{Clazz}>(InitializeLocalizer);";
                    yield return "";
                    yield return $"{Constants.Ind2}private static IGeneratedLocalizerFor{Clazz} InitializeLocalizer()";
                    yield return $"{Constants.Ind2}{{";

                    // Add initializer functions to initialize localizer instance
                    int index = 0;
                    foreach (var generator in generators.Values.Where(x => !x.Data.IsNeutralLanguage).Select(x => x))
                    {
                        yield return $"{Constants.Ind3}{(index == 0 ? "" : "else ")}if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == \"{generator.Data.Locale}\")";
                        yield return $"{Constants.Ind3}{{";
                        yield return $"{Constants.Ind4}return new {generator.GeneratedClassName(generator.Data.Locale)}();";
                        yield return $"{Constants.Ind3}}}";
                        index++;
                    }

                    if (generators.Count > 1)
                    {
                        yield return $"{Constants.Ind3}else";
                        yield return $"{Constants.Ind3}{{";
                        yield return $"{Constants.Ind4}return new GeneratedLocalizerFor{Clazz}Neutral();";
                        yield return $"{Constants.Ind3}}}";
                    }
                    else
                    {
                        yield return $"{Constants.Ind3}return new GeneratedLocalizerFor{Clazz}Neutral();";
                    }

                    yield return $"{Constants.Ind2}}}";
                }

                // Loop through the resources and generate interface and locale classes
                yield return "";
                if (IsMultipleLanguages)
                {
                    yield return $"{Constants.Ind2}#region String accessors";
                    foreach (var res in generators[Constants.Neutral].Data.Resources)
                    {
                        var decl = res.StaticProperty;

                        IEnumerable<string> Values()
                        {
                            foreach (var generator in generators.Values)
                            {
                                var subRes = generator.Data.Resources.FirstOrDefault(x => x.Key == res.Key);
                                yield return $"{generator.Data.Locale}: {subRes?.CleanValue ?? "Missing documentation"}";
                            }
                        }

                        foreach (var value in ImplementationGenerator.DocumentationLines(decl, res.Context, Values().ToArray()))
                        {
                            yield return value;
                        }

                        yield return decl;
                    }
                    yield return $"{Constants.Ind2}#endregion";
                }
                else
                {
                    yield return generators[generators.Keys.First()].GenerateStaticAccessors();
                }

                yield return "";
                yield return $"{Constants.Ind2}/// <summary>";
                yield return $"{Constants.Ind2}/// {Strings.Unescape(Strings.FormatDoc)}";
                yield return $"{Constants.Ind2}/// </summary>";
                yield return $"{Constants.Ind2}{AccessModifier} static string Format(string name, params object[] args)";
                yield return $"{Constants.Ind2}{{";

                yield return $"{Constants.Ind3}return args == null || args.Length == 0 ? name : string.Format(name, args);";
                yield return $"{Constants.Ind2}}}";
                yield return "";
                yield return $"{Constants.Ind2}/// <summary>";
                yield return $"{Constants.Ind2}/// {Strings.Unescape(Strings.UnescapeDoc)}";
                yield return $"{Constants.Ind2}/// </summary>";
                yield return $"{Constants.Ind2}public static string Unescape(string value)";
                yield return $"{Constants.Ind2}{{";

                yield return $"{Constants.Ind3}return value.Replace(\"\\\\\", \"\");";
                yield return $"{Constants.Ind2}}}";
                yield return "";

                if (IsMultipleLanguages)
                {
                    yield return $"{Constants.Ind2}/// <summary>";
                    yield return $"{Constants.Ind2}/// {Strings.Unescape(Strings.GetStringDoc)}";
                    yield return $"{Constants.Ind2}/// </summary>";
                    yield return $"{Constants.Ind2}public static string GetString(string name, params object[] args) => _localizer.Value.GetString(name, args);";
                    yield return "";
                    yield return $"{Constants.Ind2}/// <summary>";
                    yield return $"{Constants.Ind2}/// {Strings.Unescape(Strings.GetStringOrEmptyDoc)}";
                    yield return $"{Constants.Ind2}/// ";
                    yield return $"{Constants.Ind2}/// </summary>";
                    yield return $"{Constants.Ind2}public static string GetStringOrEmpty(string name, params object[] args) => _localizer.Value.GetStringOrEmpty(name, args);";
                    yield return "";

                    // Validate that each resource string is represented in all locales for the localization
                    var keySeenDict = new Dictionary<string, int>();
                    foreach (var generator in generators.Values)
                    {
                        foreach (var resource in generator.Data.Resources)
                        {
                            keySeenDict[resource.Key] = keySeenDict.ContainsKey(resource.Key)
                                ? keySeenDict[resource.Key] + 1
                                : 1;
                        }
                    }

                    // Each key should be represented the same number of times as we have generators
                    if (keySeenDict.Any(x => x.Value != generators.Count))
                    {
                        errors.Add($"{generators.First().Value.Data.SourceFile}: Invalid resource values for the following resource: " +
                            string.Join(", ", from seen in keySeenDict
                                              where seen.Value != generators.Count
                                              select seen.Key));
                    }

                    // Generate inteface and implementation classes as private classes to the static accessor class
                    yield return generators[Constants.Neutral].GenerateInterface();
                    yield return generators[Constants.Neutral].GenerateBaseType();
                    foreach (var impl in generators.Values.OrderBy(x => x.Data.Locale))
                    {
                        yield return impl.GenerateClass();
                    }
                }

                yield return $"{Constants.Ind1}}}";
                yield return "}";

                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        Debug.WriteLine($"ResourceGenerator: {error}");
                    }
                    throw new StringGeneratorException($"Errors during string generation", errors);
                }
            }
        }
    }
}