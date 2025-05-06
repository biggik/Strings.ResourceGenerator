namespace Strings.ResourceGenerator.Helpers;

internal static class DocumentationEncoder
{
    public static string Encode(string s)
    {
        return s
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("'", "&apos;")
            .Replace("\"", "&quot;")
            ;
    }
}
