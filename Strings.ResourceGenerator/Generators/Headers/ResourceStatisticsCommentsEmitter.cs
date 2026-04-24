using System.Linq;
using Strings.ResourceGenerator.Generators.Emit;
using Strings.ResourceGenerator.Generators.Interfaces;
using Strings.ResourceGenerator.Models;

namespace Strings.ResourceGenerator.Generators.Headers;

/// <summary>
/// Emits the commented resource statistics block (counts by string kind, totals, commented-line summary)
/// written beneath the generator banner in the namespace. Expects <paramref name="w"/> to be positioned
/// at namespace-body indent (same as when writing <c>///</c> summary lines).
/// </summary>
internal static class ResourceStatisticsCommentsEmitter
{
    /// <summary>
    /// Spaces placed after <c>// </c> on statistic lines so the numeric column lines up across lines.
    /// This is not structural nesting: <see cref="CodeWriter.PushIndent"/> only prefixes
    /// whole lines from column 0; it cannot insert a gap between <c>//</c> and the rest of the same line.
    /// Width matches one logical indent step (<see cref="Indent.Spaces"/>(1)) for historical layout only.
    /// </summary>
    private static readonly string CommentColumnGap = Indent.Spaces(1);

    public static void Write(CodeWriter w, IGenerator generator)
    {
        static string PluralOrSingular(int i) => i == 1 ? "string" : "strings";
        static string Formatted(int i) => string.Format("{0,4}", i);

        var differentTypes = 0;
        var cnt = generator.Data.Resources.Count(x => x.StringType == StringType.Simple);
        if (cnt > 0)
        {
            differentTypes++;
            w.Line($"// {CommentColumnGap}{Formatted(cnt)} {PluralOrSingular(cnt)} without parameters");
        }

        cnt = generator.Data.Resources.Count(x => x.StringType == StringType.Format);
        if (cnt > 0)
        {
            differentTypes++;
            w.Line($"// {CommentColumnGap}{Formatted(cnt)} {PluralOrSingular(cnt)} with String.Format replacements");
        }

        cnt = generator.Data.Resources.Count(x => x.StringType == StringType.Interpolation);
        if (cnt > 0)
        {
            differentTypes++;
            w.Line($"// {CommentColumnGap}{Formatted(cnt)} {PluralOrSingular(cnt)} with interpolation parameters");
        }

        if (differentTypes > 1)
        {
            var count = generator.Data.Resources.Count;
            w.Line($"// {CommentColumnGap}====");
            w.Line($"// {CommentColumnGap}{Formatted(count)} total strings");
        }

        if (generator.Data.CommentedLines > 0)
        {
            w.Line("//");
            w.Line($"// {CommentColumnGap}{Formatted(generator.Data.CommentedLines)} commented out {PluralOrSingular(generator.Data.CommentedLines)}");
        }
    }
}
