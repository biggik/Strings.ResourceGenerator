using System.Text;

namespace Strings.ResourceGenerator.Helpers;

/// <summary>
/// Embeds <c>.strings</c> values into generated C# string literals.
/// </summary>
/// <remarks>
/// Contract matches legacy <c>GenerateLocalizers</c>: the file already contains C#-literal
/// spelling (<c>\r\n</c>, <c>\"</c>, <c>\\</c>). Backslash sequences are copied through so the
/// C# compiler interprets them — they are not expanded and re-escaped.
/// <list type="bullet">
/// <item>Bare <c>"</c> (not preceded by <c>\</c>) is escaped so accidental quotes do not break codegen.</item>
/// <item>Real control characters (actual CR/LF in the file) are escaped to <c>\r</c>/<c>\n</c>/…</item>
/// </list>
/// </remarks>
internal static class CSharpStringLiteral
{
    /// <summary>
    /// Contents for a normal <c>"..."</c> literal (no surrounding quotes).
    /// </summary>
    public static string EscapeContentsForDoubleQuoted(string value) => EscapeForNonVerbatimLiteral(value);

    /// <summary>
    /// Contents for an interpolated <c>$"..."</c> literal. Same rules; does not escape
    /// <c>{</c>/<c>}</c> (interpolation holes belong to the parser).
    /// </summary>
    public static string EscapeContentsForInterpolated(string value) => EscapeForNonVerbatimLiteral(value);

    private static string EscapeForNonVerbatimLiteral(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        var sb = new StringBuilder(value.Length + 16);
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsSurrogate(c))
            {
                if (i + 1 < value.Length && char.IsSurrogatePair(value, i))
                {
                    var cp = char.ConvertToUtf32(value, i);
                    sb.AppendFormat("\\U{0:x8}", cp);
                    i++;
                }
                else
                {
                    sb.AppendFormat("\\u{0:x4}", (ushort)c);
                }

                continue;
            }

            switch (c)
            {
                case '\\':
                    // Pass through — part of author escapes (\r, \n, \\, \", …).
                    sb.Append('\\');
                    break;
                case '\"':
                    // Odd number of preceding '\' → already escaped in .strings; even → bare quote.
                    if (IsEscapedByPrecedingBackslashes(value, i))
                    {
                        sb.Append('"');
                    }
                    else
                    {
                        sb.Append("\\\"");
                    }

                    break;
                case '\0':
                    sb.Append("\\0");
                    break;
                case '\a':
                    sb.Append("\\a");
                    break;
                case '\b':
                    sb.Append("\\b");
                    break;
                case '\f':
                    sb.Append("\\f");
                    break;
                case '\n':
                    sb.Append("\\n");
                    break;
                case '\r':
                    sb.Append("\\r");
                    break;
                case '\t':
                    sb.Append("\\t");
                    break;
                case '\v':
                    sb.Append("\\v");
                    break;
                default:
                    if (char.IsControl(c) || c >= 127 && c < 160)
                    {
                        sb.AppendFormat("\\u{0:x4}", (ushort)c);
                    }
                    else
                    {
                        sb.Append(c);
                    }

                    break;
            }
        }

        return sb.ToString();
    }

    private static bool IsEscapedByPrecedingBackslashes(string value, int index)
    {
        var count = 0;
        for (var j = index - 1; j >= 0 && value[j] == '\\'; j--)
        {
            count++;
        }

        return count % 2 == 1;
    }
}
