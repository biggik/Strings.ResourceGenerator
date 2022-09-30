namespace Strings.ResourceGenerator.Helpers
{
    // Heavily copied from https://github.com/microsoft/referencesource/blob/master/System/compmod/microsoft/csharp/csharpcodeprovider.cs
    internal static class NameHelper
    {
        public static string CreateValidIdentifier(string name)
        {
            if (IsPrefixTwoUnderscore(name))
            {
                name = "_" + name;
            }

            while (IsKeyword(name))
            {
                name = "_" + name;
            }

            return name;
        }

        static bool IsKeyword(string value)
        {
            return FixedStringLookup.Contains(keywords, value, false);
        }

        static bool IsPrefixTwoUnderscore(string value)
        {
            return value.Length < 3
                ? false
                : ((value[0] == '_') && (value[1] == '_') && (value[2] != '_'));
        }

        private static readonly string[][] keywords = new string[][] {
            null,           // 1 character
            new string[] {  // 2 characters
                "as",
                "do",
                "if",
                "in",
                "is",
            },
            new string[] {  // 3 characters
                "for",
                "int",
                "new",
                "out",
                "ref",
                "try",
            },
            new string[] {  // 4 characters
                "base",
                "bool",
                "byte",
                "case",
                "char",
                "else",
                "enum",
                "goto",
                "lock",
                "long",
                "null",
                "this",
                "true",
                "uint",
                "void",
            },
            new string[] {  // 5 characters
                "break",
                "catch",
                "class",
                "const",
                "event",
                "false",
                "fixed",
                "float",
                "sbyte",
                "short",
                "throw",
                "ulong",
                "using",
                "while",
            },
            new string[] {  // 6 characters
                "double",
                "extern",
                "object",
                "params",
                "public",
                "return",
                "sealed",
                "sizeof",
                "static",
                "string",
                "struct",
                "switch",
                "typeof",
                "unsafe",
                "ushort",
            },
            new string[] {  // 7 characters
                "checked",
                "decimal",
                "default",
                "finally",
                "foreach",
                "private",
                "virtual",
            },
            new string[] {  // 8 characters
                "abstract",
                "continue",
                "delegate",
                "explicit",
                "implicit",
                "internal",
                "operator",
                "override",
                "readonly",
                "volatile",
            },
            new string[] {  // 9 characters
                "__arglist",
                "__makeref",
                "__reftype",
                "interface",
                "namespace",
                "protected",
                "unchecked",
            },
            new string[] {  // 10 characters
                "__refvalue",
                "stackalloc",
            },
        };
    }
}
