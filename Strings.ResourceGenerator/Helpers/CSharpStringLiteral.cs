using System.Text;

namespace Strings.ResourceGenerator.Helpers;

/// <summary>
/// Escapes text for embedding in generated C# source (no extra assemblies required at analyzer load time).
/// </summary>
internal static class CSharpStringLiteral
{
    /// <summary>
    /// Escapes text placed inside a normal <c>"..."</c> literal (contents only, no surrounding quotes).
    /// </summary>
    public static string EscapeContentsForDoubleQuoted(string value)
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
                    sb.Append("\\\\");
                    break;
                case '\"':
                    sb.Append("\\\"");
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

    /// <summary>
    /// Escapes <c>\</c> and <c>"</c> for text inside an interpolated string (<c>$"..."</c>).
    /// </summary>
    public static string EscapeContentsForInterpolated(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
