using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Strings.ResourceGenerator.Generators.Parsers;

namespace Strings.ResourceGenerator.Helpers
{
    internal static class Splitter
    {
        internal static string SplitDeclAndImpl(string decl, string impl, string indent)
        {
            const int magic = 140;
            bool HasInterpolation(MatchCollection matches)
            {
                if (matches.Count > 0)
                {
                    return
                        (from m in matches.Cast<Match>()
                         where m.Groups["interpolated"].Success
                         select m).Any();
                }
                return false;
            }
            IEnumerable<string> SplitLongLines(string s)
            {
                if (s.Contains("Format(\""))
                {
                    yield return s;
                }
                else
                {
                    while (s.Length > magic)
                    {
                        int pos = s.Substring(0, magic).LastIndexOf(' ');

                        var f = s.Substring(0, pos + 1);
                        if (f.Contains("$\""))
                        {
                            var matchesFirst = ResourceParser.ParameterRegex.Matches(f);
                            if (!HasInterpolation(matchesFirst))
                            {
                                f = f.Replace("$\"", "\"");
                            }
                        }
                        yield return f + "\"";

                        s = s.Substring(pos + 1);
                        var matches = ResourceParser.ParameterRegex.Matches(s);

                        s = "+ " + (HasInterpolation(matches) ? "$" : "") + "\"" + s;
                    }

                    var matchesLast = ResourceParser.ParameterRegex.Matches(s);
                    if (!HasInterpolation(matchesLast))
                    {
                        s = s.Replace("$\"", "\"");
                    }
                    yield return s;
                }
            }

            IEnumerable<string> SplitOnNewLines(string s)
            {
                int pos = s.IndexOf("\\n");
                while (pos > 0)
                {
                    var f = s.Substring(0, pos + 2);

                    if (f.Contains("$\""))
                    {
                        var matchesFirst = ResourceParser.ParameterRegex.Matches(f);
                        if (!HasInterpolation(matchesFirst))
                        {
                            f = f.Replace("$\"", "\"");
                        }
                    }
                    yield return f + "\"";

                    s = s.Substring(pos + 2);
                    var matchesRemainder = ResourceParser.ParameterRegex.Matches(s);
                    s = "+ " + (HasInterpolation(matchesRemainder) ? "$" : "") + "\"" + s;
                    pos = s.IndexOf("\\n");
                }
                yield return s;
            }

            if ((decl.Trim() + impl.Trim()).Length > magic)
            {
                var declIndent = new string(' ', decl.TrimEnd().Length - decl.Trim().Length);
                return decl + Environment.NewLine + declIndent.Substring(declIndent.Length - 4)
                    + string.Join(Environment.NewLine + declIndent,
                        from l1 in SplitOnNewLines(impl.Trim())
                        from l2 in SplitLongLines(l1)
                        select $"{declIndent}{indent}{l2}")
                    ;
            }
            else
            {
                return $"{decl} {impl.Trim()}";
            }
        }
    }
}
