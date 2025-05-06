namespace Strings.ResourceGenerator.Helpers;

internal static class Splitter
{
    internal static string SplitDeclAndImpl(string decl, string impl)
    {
        return $"{decl} {impl.Trim()}";
    }
}
