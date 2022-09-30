using Strings.ResourceGenerator.Generators.Data;
using Strings.ResourceGenerator.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Strings.ResourceGenerator.Generators.StringsFile
{
    internal static class StringsProvider
    {
        public static LocalizerGenerator Provide(
            string clazz, 
            StringConfiguration config, 
            (string path, IEnumerable<string> lines)[] resources)
        {
            // Generate for each
            var generator = new LocalizerGenerator(config, clazz);
            foreach (var resource in resources)
            {
                Debug.WriteLine($"ResourceGenerator: {Path.GetFileName(resource.path)}");

                var locale = Path.GetFileNameWithoutExtension(resource.path)
                                 .Replace($"{clazz}.", "");
                if (locale == clazz)
                {
                    locale = Constants.Neutral;
                }
                Debug.WriteLine($"ResourceGenerator: Strings generating for {clazz} [{locale}]");

                var data = new GeneratorData
                {
                    Config = config,
                    Locale = locale,
                    ClassName = clazz,
                    SourceFile = resource.path,
                    IsMultipleLanguages = resources.Length > 1
                };
                generator.AddStringLocalizer(new StringsGenerator(data, resource.lines), locale);
            }

            return generator;
        }
    }
}
