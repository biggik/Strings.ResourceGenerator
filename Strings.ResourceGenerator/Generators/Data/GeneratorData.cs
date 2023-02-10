using Strings.ResourceGenerator.Models;
using System.Collections.Generic;

namespace Strings.ResourceGenerator.Generators.Data
{
    internal record GeneratorData
    {
        public List<ResourceString> Resources { get; set; }

        public string SourceFile { get; set; }
        public string Locale { get; set; }
        public string ClassName { get; set; }
        public int CommentedLines { get; set; }
        public bool IsMultipleLanguages { get; set; }

        /// <summary>
        /// Indicator for whether the current .strings file is the Neutral language file
        /// </summary>
        public bool IsNeutralLanguage => Locale == Constants.Neutral;

        public StringConfiguration Config { get; internal set; }
    }
}
