namespace Strings.ResourceGenerator.Generators.Emit;

/// <summary>
/// Logical indentation for generated C# source. One level equals <see cref="CodeWriter.IndentWidth"/> spaces.
/// </summary>
internal static class Indent
{
    /// <summary>
    /// Returns a string of <paramref name="levels"/> × <see cref="CodeWriter.IndentWidth"/> space characters.
    /// </summary>
    public static string Spaces(int levels)
    {
        if (levels <= 0)
        {
            return string.Empty;
        }

        return new string(' ', levels * CodeWriter.IndentWidth);
    }
}
