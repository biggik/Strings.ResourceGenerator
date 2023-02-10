using Strings.ResourceGenerator.Generators.Interfaces;
using Strings.ResourceGenerator.Helpers;
using Strings.ResourceGenerator.Models;
using Strings.ResourceGenerator.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strings.ResourceGenerator.Generators
{
    /// <summary>
    /// Generate implementation code from an IGenerator
    /// </summary>
    internal static class ImplementationGenerator
    {
        /// <summary>
        /// Generate headers for string generation
        /// </summary>
        /// <param name="prefix">The prefix for each generated line</param>
        /// <returns>An enumerable of strings</returns>
        internal static IEnumerable<string> Headers(this IGenerator generator, string prefix)
        {
            static string PluralOrSingular(int i) => i == 1 ? "string" : "strings";
            static string Formatted(int i) => string.Format("{0,4}", i);

            int differentTypes = 0;
            int cnt = generator.Data.Resources.Count(x => x.StringType == StringType.Simple);
            if (cnt > 0)
            {
                differentTypes++;
                yield return $"{prefix}{Constants.Ind1}{Formatted(cnt)} {PluralOrSingular(cnt)} without parameters";
            }
            cnt = generator.Data.Resources.Count(x => x.StringType == StringType.Format);
            if (cnt > 0)
            {
                differentTypes++;
                yield return $"{prefix}{Constants.Ind1}{Formatted(cnt)} {PluralOrSingular(cnt)} with String.Format replacements";
            }
            cnt = generator.Data.Resources.Count(x => x.StringType == StringType.Interpolation);
            if (cnt > 0)
            {
                differentTypes++;
                yield return $"{prefix}{Constants.Ind1}{Formatted(cnt)} {PluralOrSingular(cnt)} with interpolation parameters";
            }

            if (differentTypes > 1)
            {
                var count = generator.Data.Resources.Count;
                yield return $"{prefix}{Constants.Ind1}====";
                yield return $"{prefix}{Constants.Ind1}{Formatted(count)} total strings";
            }

            if (generator.Data.CommentedLines > 0)
            {
                yield return $"{Constants.Ind1}//";
                yield return $"{prefix}{Constants.Ind1}{Formatted(generator.Data.CommentedLines)} commented out {PluralOrSingular(generator.Data.CommentedLines)}";
            }
        }

        /// <summary>
        /// The possible levels being generated
        /// </summary>
        public enum Level
        {
            /// <summary>
            /// The Interface level is used for generating an interface that all language accessors
            /// for the same resource must implement
            /// </summary>
            Interface,

            /// <summary>
            /// The BaseType level is used for generating a base type that different accessors for the
            /// same resource must override
            /// </summary>
            BaseType,

            /// <summary>
            /// The Implementation level is used for generating an override for a languge implementation of
            /// a BaseType or Interface
            /// </summary>
            Implementation,

            /// <summary>
            /// The StaticAccessors level is used for generating the one-and-only implementation of a 
            /// resource that only has one language
            /// </summary>
            StaticAccessors
        };

        public static string GeneratedClassName(this IGenerator generator, string locale = null)
            => $"GeneratedLocalizerFor{generator.Data.ClassName}{locale ?? generator.Data.Locale}";

        /// <summary>
        /// Generate an Interface implementation
        /// </summary>
        /// <returns></returns>
        public static string GenerateInterface(this IGenerator generator)
            => Generate(generator, Level.Interface, withDocumentation: false);

        /// <summary>
        /// Generate a BaseType implementation
        /// </summary>
        /// <returns></returns>
        public static string GenerateBaseType(this IGenerator generator)
            => Generate(generator, Level.BaseType, withDocumentation: false);

        /// <summary>
        /// Generate an Implementation implementation
        /// </summary>
        /// <returns></returns>
        public static string GenerateClass(this IGenerator generator)
            => Generate(generator, Level.Implementation, withDocumentation: false);

        /// <summary>
        /// Generate a static accessor implementation
        /// </summary>
        /// <returns></returns>
        public static string GenerateStaticAccessors(this IGenerator generator)
            => Generate(generator, Level.StaticAccessors, withDocumentation: true);

        private static string Generate(IGenerator generator, Level level, bool withDocumentation)
        {
            string indent = (level == Level.StaticAccessors ? "" : Constants.Ind1) + Constants.Ind1;
            var buf = new StringBuilder();

            string constructor = null;
            if (level == Level.Interface)
            {
                buf.AppendLine($"{indent}#region IGeneratedLocalizerFor{generator.Data.ClassName}");
                buf.AppendLine($"{indent}/// <summary>");
                buf.AppendLine($"{indent}/// Interface IGeneratedLocalizerFor{generator.Data.ClassName} for string access to {generator.Data.ClassName} resources");
                buf.AppendLine($"{indent}/// </summary>");
                var modifier = generator.Data.Config.GeneratePublic ? "public" : "internal";
                buf.AppendLine($"{indent}{modifier} interface IGeneratedLocalizerFor{generator.Data.ClassName}");
            }
            else if (level == Level.BaseType)
            {
                constructor = $"protected GeneratedLocalizerFor{generator.Data.ClassName}Base()";
                buf.AppendLine($"{indent}#region GeneratedLocalizerFor{generator.Data.ClassName}Base");
                buf.AddExcludeAttribute(generator.Data.Config.ExcludeFromCodeCoverage, indent);
                buf.AppendLine($"{indent}private abstract class GeneratedLocalizerFor{generator.Data.ClassName}Base");
            }
            else if (level == Level.Implementation)
            {
                buf.AppendLine($"{indent}#region {generator.GeneratedClassName()}");
                buf.AddExcludeAttribute(generator.Data.Config.ExcludeFromCodeCoverage, indent);
                buf.AppendLine($"{indent}private class {generator.GeneratedClassName()} : GeneratedLocalizerFor{generator.Data.ClassName}Base, IGeneratedLocalizerFor{generator.Data.ClassName}");
            }
            else if (level == Level.StaticAccessors)
            {
                constructor = $"static {generator.Data.ClassName}()";
            }

            if (level != Level.StaticAccessors)
            {
                buf.AppendLine($"{indent}{{");
            }

            if (level == Level.Interface)
            {
                buf.AppendLine($"{indent}{Constants.Ind1}/// <summary>");
                buf.AppendLine($"{indent}{Constants.Ind1}/// {LocalStrings.Unescape(LocalStrings.GetStringDoc)}");
                buf.AppendLine($"{indent}{Constants.Ind1}/// </summary>");
                buf.AppendLine($"{indent}{Constants.Ind1}string GetString(string name, params object[] args);");
                buf.AppendLine();
                buf.AppendLine($"{indent}{Constants.Ind1}/// <summary>");
                buf.AppendLine($"{indent}{Constants.Ind1}/// {LocalStrings.Unescape(LocalStrings.GetStringOrEmptyDoc)}");
                buf.AppendLine($"{indent}{Constants.Ind1}/// </summary>");
                buf.AppendLine($"{indent}{Constants.Ind1}string GetStringOrEmpty(string name, params object[] args);");
            }
            if (level == Level.BaseType || level == Level.StaticAccessors)
            {
                var modifier = level == Level.StaticAccessors ? " static " : " ";
                if (level == Level.BaseType)
                {
                    buf.AppendLine($"{indent}{Constants.Ind1}#region Generic string lookup (by dictionary)");
                }
                buf.AppendLine($"{indent}{Constants.Ind1}private {(level == Level.StaticAccessors ? "static " : "")}readonly Lazy<Dictionary<string, string>> lookup;");
                buf.AppendLine($"{indent}{Constants.Ind1}{(level == Level.StaticAccessors ? "private static" : "protected")} Dictionary<string, string> Lookup => lookup.Value;");
                buf.AppendLine();
                if (!string.IsNullOrEmpty(constructor))
                {
                    buf.AppendLine($"{indent}{Constants.Ind1}{constructor}");
                    buf.AppendLine($"{indent}{Constants.Ind1}{{");
                    buf.AppendLine($"{indent}{Constants.Ind2}lookup = new Lazy<Dictionary<string, string>>(() => InitializeLookupResources());");
                    buf.AppendLine($"{indent}{Constants.Ind1}}}");
                    buf.AppendLine();
                }
                buf.AppendLine($"{indent}{Constants.Ind1}/// <summary>");
                buf.AppendLine($"{indent}{Constants.Ind1}/// {LocalStrings.Unescape(LocalStrings.GetStringDoc)}");
                buf.AppendLine($"{indent}{Constants.Ind1}/// </summary>");
                buf.AppendLine($"{indent}{Constants.Ind1}public{modifier}string GetString(string name, params object[] args)");
                buf.AppendLine($"{indent}{Constants.Ind1}{{");
                buf.AppendLine($"{indent}{Constants.Ind2}if (lookup.Value.ContainsKey(name))");
                buf.AppendLine($"{indent}{Constants.Ind2}{{");
                buf.AppendLine($"{indent}{Constants.Ind3}var s = lookup.Value[name];");
                buf.AppendLine($"{indent}{Constants.Ind3}return args == null || args.Length == 0 ? s : string.Format(s, args);");
                buf.AppendLine($"{indent}{Constants.Ind2}}}");
                buf.AppendLine();
                buf.AppendLine($"{indent}{Constants.Ind2}throw new ArgumentException($\"Lookup value {{name}} is not found in localized resources in {generator.Data.ClassName}\");");
                buf.AppendLine($"{indent}{Constants.Ind1}}}");
                buf.AppendLine();
                buf.AppendLine($"{indent}{Constants.Ind1}/// <summary>");
                buf.AppendLine($"{indent}{Constants.Ind1}/// {LocalStrings.Unescape(LocalStrings.GetStringOrEmptyDoc)}");
                buf.AppendLine($"{indent}{Constants.Ind1}/// </summary>");
                buf.AppendLine($"{indent}{Constants.Ind1}public{modifier}string GetStringOrEmpty(string name, params object[] args)");
                buf.AppendLine($"{indent}{Constants.Ind1}{{");
                buf.AppendLine($"{indent}{Constants.Ind2}try");
                buf.AppendLine($"{indent}{Constants.Ind2}{{");
                buf.AppendLine($"{indent}{Constants.Ind3}return GetString(name, args);");
                buf.AppendLine($"{indent}{Constants.Ind2}}}");
                buf.AppendLine($"{indent}{Constants.Ind2}catch (ArgumentException)");
                buf.AppendLine($"{indent}{Constants.Ind2}{{");
                buf.AppendLine($"{indent}{Constants.Ind3}return \"\";");
                buf.AppendLine($"{indent}{Constants.Ind2}}}");
                buf.AppendLine($"{indent}{Constants.Ind1}}}");

                buf.AppendLine();

                if (level == Level.BaseType)
                {
                    buf.AppendLine($"{indent}{Constants.Ind1}protected abstract Dictionary<string, string> InitializeLookupResources();");
                    buf.AppendLine($"{indent}{Constants.Ind1}#endregion");
                    buf.AppendLine();
                }
            }
            if (level == Level.Implementation || level == Level.StaticAccessors)
            {
                var modifier = level == Level.Implementation ? "protected override" : "static";
                if (level == Level.Implementation)
                {
                    buf.AppendLine($"{indent}{Constants.Ind1}#region Generic string lookup (by dictionary)");
                }
                buf.AppendLine($"{indent}{Constants.Ind1}{modifier} Dictionary<string, string> InitializeLookupResources() =>");
                buf.AppendLine($"{indent}{Constants.Ind2}new Dictionary<string, string>");
                buf.AppendLine($"{indent}{Constants.Ind2}{{");
                foreach (var res in generator.Data.Resources)
                {
                    buf.AppendLine($"{indent}{Constants.Ind3}{{ \"{res.Key}\", \"{res.CleanValueImplementation}\" }},");
                }
                buf.AppendLine($"{indent}{Constants.Ind2}}};");
                if (level == Level.Implementation)
                {
                    buf.AppendLine($"{indent}{Constants.Ind1}#endregion");
                }
            }

            if (level != Level.BaseType)
            {
                if (level == Level.Implementation)
                {
                    buf.AppendLine($"{indent}{Constants.Ind1}#region String accessors");
                }

                foreach (var res in generator.Data.Resources)
                {
                    if (level == Level.Interface)
                    {
                        var fullDecl = $"{Constants.Ind1}{res.InterfaceDeclaration}";
                        buf.AppendLine(Documentation(fullDecl, res.Context, res.CleanValueNonImplementation));
                        buf.AppendLine(fullDecl);
                    }
                    else if (level == Level.Implementation || level == Level.StaticAccessors)
                    {
                        var preferConst = !generator.Data.IsMultipleLanguages
                            && generator.Data.Config.PreferConstOverStatic
                            && res.StringType == StringType.Simple;
                        var decl = level == Level.Implementation
                            ? res.PublicProperty
                            : res.PublicStaticProperty(preferConst);
                        var extraIndent = level == Level.Implementation ? Constants.Ind1 : "";
                        var impl = res.ClassLine(preferConst);

                        var fullDecl = extraIndent + Splitter.SplitDeclAndImpl(decl, impl, extraIndent);

                        if (withDocumentation)
                        {
                            buf.AppendLine(Documentation(fullDecl, res.Context, res.CleanValueNonImplementation));
                        }

                        buf.AppendLine(fullDecl);
                    }
                }
                if (level == Level.Implementation)
                {
                    buf.AppendLine($"{indent}{Constants.Ind1}#endregion");
                }
            }

            if (level != Level.StaticAccessors)
            {
                buf.AppendLine($"{indent}}}");
                buf.AppendLine($"{indent}#endregion");
            }
            return buf.ToString();
        }

        /// <summary>
        /// Generate a documentation for an implementation
        /// </summary>
        /// <param name="declaration">The implementation declaration</param>
        /// <param name="values">Values for the documentation part</param>
        /// <returns>A documented declaration</returns>
        public static IEnumerable<string> DocumentationLines(string declaration, string context, params string[] values)
        {
            var declIndent = new string(' ', declaration.Length - declaration.TrimStart().Length);

            yield return "";
            yield return $"{declIndent}/// <summary>";

            for (int i = 0; i < values.Length; i++)
            {
                yield return $"{declIndent}/// {FormatForDoc(values[i])}{(i == values.Length - 1 ? "" : " <br/>")}";
            }
            if (!string.IsNullOrWhiteSpace(context))
            {
                yield return $"{declIndent}/// <remarks>";
                yield return $"{declIndent}/// <para><b>Context:</b></para>";
                yield return $"{declIndent}/// <para>{context}</para>";
                yield return $"{declIndent}/// </remarks>";
            }
            yield return $"{declIndent}/// </summary>";
        }

        /// <summary>
        /// Generate a documentation for an implementation
        /// </summary>
        /// <param name="declaration">The implementation declaration</param>
        /// <param name="context">The context for the value</param>
        /// <param name="values">Values for the documentation part</param>
        /// <returns>A documented declaration</returns>
        public static string Documentation(string declaration, string context, params string[] values)
        {
            return string.Join(Environment.NewLine, DocumentationLines(declaration, context, values));
        }

        private static string FormatForDoc(string v)
        {
            return DocumentationEncoder.Encode(
                v.Replace("\\", "")
                 .Replace("\"", "'")
            );
        }
    }
}
