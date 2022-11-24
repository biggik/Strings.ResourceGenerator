namespace Strings.ResourceGenerator.Helpers
{
    internal static class Splitter
    {
        internal static string SplitDeclAndImpl(string decl, string impl, string indent)
        {
            return $"{decl} {impl.Trim()}";
        }
    }
}
