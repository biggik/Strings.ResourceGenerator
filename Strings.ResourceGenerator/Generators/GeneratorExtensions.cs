using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Strings.ResourceGenerator.Generators.Data;
using Strings.ResourceGenerator.Generators.Interfaces;
using Strings.ResourceGenerator.Models;

namespace Strings.ResourceGenerator.Generators;

internal static class GeneratorExtensions
{
    public static LocalizerGenerator ToGenerator(this StringsModel model,
        string path, string clazz,
        Func<GeneratorData, List<ResourceStringModel>, IGenerator> createGenerator)
    {
        var generator = new LocalizerGenerator(model.Config, clazz);

        var locales = new List<string> { Constants.Neutral };
        var isMultipleLanguages = model.Strings.Any(x => (x.Values?.Count ?? 0) > 1);
        if (isMultipleLanguages)
        {
            locales = model.Strings
                           .Where(x => x.Values != null)
                           .SelectMany(x => x.Values)
                           .Select(x => x.Locale?.ToUpper() ?? Constants.Neutral)
                           .Distinct()
                           .ToList();
        }

        foreach (var locale in locales)
        {
            var data = new GeneratorData
            {
                Config = model.Config,
                Locale = locale,
                ClassName = $"{model.Config.Prefix}{clazz}",
                SourceFile = path,
                IsMultipleLanguages = isMultipleLanguages
            };

            generator.AddStringLocalizer(createGenerator(data, model.Strings), locale);
        }

        return generator;

    }

    public static List<ResourceString> InitializeResources(this IGenerator generator, List<ResourceStringModel> strings)
    {
        bool IsLocale(ResourceStringValue rsv)
        {
            return (generator.Data.Locale == Constants.Neutral && string.IsNullOrEmpty(rsv.Locale))
                    || string.Equals(rsv.Locale, generator.Data.Locale, StringComparison.InvariantCultureIgnoreCase);
        }
        var multiValue = strings.Where(x => x.Values != null && x.Values.Any(IsLocale))
                                .Select(x =>
                                {
                                    var rsv = x.Values.First(IsLocale);
                                    return new ResourceString(generator.Data.Locale, x.Key, rsv.Value, x.Context);
                                })
                                .ToList();
        var singleValue = strings.Where(x => generator.Data.Locale == Constants.Neutral && !string.IsNullOrEmpty(x.Value))
                                 .Select(x => new ResourceString(generator.Data.Locale, x.Key, x.Value, x.Context))
                                 .ToList();
        return Enumerable.Concat(multiValue, singleValue)
                         .OrderBy(x => x.Key)
                         .ToList();
    }

    public static void AddExcludeAttribute(this StringBuilder buf, bool apply, string message, string indent)
    {
        if (apply)
        {
            buf.AppendLine(indent.ExcludeAttributeIndented(apply, message));
        }
    }

    public static string ExcludeAttributeIndented(this string indent, bool apply, string message)
    {
        return apply
            ? $"{indent}[{typeof(ExcludeFromCodeCoverageAttribute).FullName
                .Replace("Attribute", "")}]"
                .Replace("]", $"(Justification = \"{message}\")]")
            : null;
    }
}
