namespace Strings.ResourceGenerator.Resources
{
    internal static class Strings
    {
        public static string FormatDoc => "Format the string identified by <paramref name=\"name\"/> with the given parameters";
        public static string GetStringDoc => "Get the string identified by <paramref name=\"name\"/> formatted with the specified parameters";
        public static string GetStringOrEmptyDoc => "Get the string identified by <paramref name=\"name\"/> formatted with the specified parameters, or empty string if not found";
        public static string UnescapeDoc => "Unescape unnecessary escapes when string is not used as part of code";

        public static string Unescape (string s)
        {
            return s.Replace("\\", "");
        }
    }
}
