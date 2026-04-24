using System;
using System.Collections.Generic;
using Strings.ResourceGenerator.Generators.Emit;
using Strings.ResourceGenerator.Generators.Interfaces;
using Strings.ResourceGenerator.Helpers;
using Strings.ResourceGenerator.Models;
using Strings.ResourceGenerator.Resources;

namespace Strings.ResourceGenerator.Generators;

/// <summary>
/// Generate implementation code from an IGenerator
/// </summary>
internal static class ImplementationGenerator
{
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
    }

    public static string GeneratedClassName(this IGenerator generator, string locale = null)
        => $"GeneratedLocalizerFor{generator.Data.ClassName}{locale ?? generator.Data.Locale}";

    /// <summary>
    /// Generate an Interface implementation
    /// </summary>
    public static string GenerateInterface(this IGenerator generator)
        => Generate(generator, Level.Interface, withDocumentation: false);

    /// <summary>
    /// Generate a BaseType implementation
    /// </summary>
    public static string GenerateBaseType(this IGenerator generator)
        => Generate(generator, Level.BaseType, withDocumentation: false);

    /// <summary>
    /// Generate an Implementation implementation
    /// </summary>
    public static string GenerateClass(this IGenerator generator)
        => Generate(generator, Level.Implementation, withDocumentation: false);

    /// <summary>
    /// Generate a static accessor implementation
    /// </summary>
    public static string GenerateStaticAccessors(this IGenerator generator)
        => Generate(generator, Level.StaticAccessors, withDocumentation: true);

    /// <summary>
    /// Indent levels from column 0 for the root column of a generated nested fragment (
    /// <see cref="Level.StaticAccessors"/> vs interface/base/implementation chunks).
    /// </summary>
    private static int OuterIndentLevels(Level level)
        => level == Level.StaticAccessors ? 1 : 2;

    /// <summary>
    /// Space prefix matching legacy <c>Generate</c> output for embedded fragments (<see cref="ExcludeAttributeIndented"/> and dictionary initializer).
    /// </summary>
    private static string OuterIndentPrefix(Level level)
        => Indent.Spaces(OuterIndentLevels(level));

    private static string Generate(IGenerator generator, Level level, bool withDocumentation)
    {
        var w = new CodeWriter();
        const int outerLevels = 2;
        for (var i = 0; i < outerLevels; i++)
        {
            w.PushIndent();
        }

        var indentPrefix = OuterIndentPrefix(level);
        string constructor = null;

        if (level == Level.Interface)
        {
            w.Line($"#region IGeneratedLocalizerFor{generator.Data.ClassName}");
            w.Line("/// <summary>");
            w.Line($"/// Interface IGeneratedLocalizerFor{generator.Data.ClassName} for string access to {generator.Data.ClassName} resources");
            w.Line("/// </summary>");
            var modifier = generator.Data.Config.GeneratePublic ? "public" : "internal";
            w.Line($"{modifier} interface IGeneratedLocalizerFor{generator.Data.ClassName}");
        }
        else if (level == Level.BaseType)
        {
            constructor = $"protected GeneratedLocalizerFor{generator.Data.ClassName}Base()";
            w.Line($"#region GeneratedLocalizerFor{generator.Data.ClassName}Base");
            w.EmitLine(indentPrefix.ExcludeAttributeIndented(
                generator.Data.Config.ExcludeFromCodeCoverage,
                generator.Data.Config.ExcludeFromCodeCoverageMessage));

            w.Line($"private abstract class GeneratedLocalizerFor{generator.Data.ClassName}Base");
        }
        else if (level == Level.Implementation)
        {
            w.Line($"#region {generator.GeneratedClassName()}");
            w.EmitLine(indentPrefix.ExcludeAttributeIndented(
                generator.Data.Config.ExcludeFromCodeCoverage,
                generator.Data.Config.ExcludeFromCodeCoverageMessage));

            w.Line($"private class {generator.GeneratedClassName()} : GeneratedLocalizerFor{generator.Data.ClassName}Base, IGeneratedLocalizerFor{generator.Data.ClassName}");
        }
        else if (level == Level.StaticAccessors)
        {
            constructor = $"static {generator.Data.ClassName}()";
        }

        void WriteTypeBody()
        {
            if (level == Level.Interface)
            {
                w.Line("/// <summary>");
                w.Line($"/// {LocalStrings.Unescape(LocalStrings.GetStringDoc)}");
                w.Line("/// </summary>");
                w.Line("string GetString(string name, params object[] args);");
                w.Line();
                w.Line("/// <summary>");
                w.Line($"/// {LocalStrings.Unescape(LocalStrings.GetStringOrEmptyDoc)}");
                w.Line("/// </summary>");
                w.Line("string GetStringOrEmpty(string name, params object[] args);");
            }

            if (level == Level.BaseType || level == Level.StaticAccessors)
            {
                WriteLazyLookupDictionaryAndRuntimeMethods(w, generator, level, constructor);
            }

            if (level == Level.Implementation || level == Level.StaticAccessors)
            {
                WriteInitializeLookupDictionary(w, generator, level);
            }

            if (level != Level.BaseType)
            {
                if (level == Level.Implementation)
                {
                    w.Line("#region String accessors");
                }

                foreach (var res in generator.Data.Resources)
                {
                    if (level == Level.Interface)
                    {
                        var fullDecl = $"{Indent.Spaces(1)}{res.InterfaceDeclaration}";
                        w.EmitLine(Documentation(fullDecl, res.Context, res.CleanValueNonImplementation));
                        w.EmitLine(fullDecl);
                    }
                    else if (level == Level.Implementation || level == Level.StaticAccessors)
                    {
                        var preferConst = !generator.Data.IsMultipleLanguages
                            && generator.Data.Config.PreferConstOverStatic
                            && res.StringType == StringType.Simple;
                        var decl = level == Level.Implementation
                            ? res.PublicProperty
                            : res.PublicStaticProperty(preferConst);
                        var extraIndent = level == Level.Implementation ? Indent.Spaces(1) : "";
                        var impl = res.ClassLine(preferConst);

                        var fullDecl = extraIndent + Splitter.SplitDeclAndImpl(decl, impl);

                        if (withDocumentation)
                        {
                            w.EmitLine(Documentation(fullDecl, res.Context, res.CleanValueNonImplementation));
                        }

                        w.EmitLine(fullDecl);
                    }
                }

                if (level == Level.Implementation)
                {
                    w.Line("#endregion");
                }
            }
        }

        if (level != Level.StaticAccessors)
        {
            using (w.OpenBlock())
            {
                WriteTypeBody();
            }

            w.Line("#endregion");
        }
        else
        {
            WriteTypeBody();
        }

        for (var i = 0; i < outerLevels; i++)
        {
            w.PopIndent();
        }

        return w.ToString();
    }

    /// <summary>
    /// Shared by <see cref="Level.BaseType"/> and <see cref="Level.StaticAccessors"/> — lazy dictionary, ctor, GetString / GetStringOrEmpty.
    /// </summary>
    private static void WriteLazyLookupDictionaryAndRuntimeMethods(
        CodeWriter w,
        IGenerator generator,
        Level level,
        string constructor)
    {
        var modifier = level == Level.StaticAccessors ? " static " : " ";
        if (level == Level.BaseType)
        {
            w.Line("#region Generic string lookup (by dictionary)");
        }

        w.Line($"private {(level == Level.StaticAccessors ? "static " : "")}readonly Lazy<Dictionary<string, string>> lookup;");
        w.Line($"{(level == Level.StaticAccessors ? "private static" : "protected")} Dictionary<string, string> Lookup => lookup.Value;");
        w.Line();
        if (!string.IsNullOrEmpty(constructor))
        {
            w.Line(constructor);
            using (w.OpenBlock())
            {
                w.Line("lookup = new Lazy<Dictionary<string, string>>(() => InitializeLookupResources());");
            }

            w.Line();
        }

        w.Line("/// <summary>");
        w.Line($"/// {LocalStrings.Unescape(LocalStrings.GetStringDoc)}");
        w.Line("/// </summary>");
        w.Line($"public{modifier}string GetString(string name, params object[] args)");
        using (w.OpenBlock())
        {
            w.Line("if (lookup.Value.ContainsKey(name))");
            using (w.OpenBlock())
            {
                w.Line("var s = lookup.Value[name];");
                w.Line("return args == null || args.Length == 0 ? s : string.Format(s, args);");
            }

            w.Line();
            w.Line($"throw new ArgumentException($\"Lookup value {{name}} is not found in localized resources in {generator.Data.ClassName}\");");
        }

        w.Line();
        w.Line("/// <summary>");
        w.Line($"/// {LocalStrings.Unescape(LocalStrings.GetStringOrEmptyDoc)}");
        w.Line("/// </summary>");
        w.Line($"public{modifier}string GetStringOrEmpty(string name, params object[] args)");
        using (w.OpenBlock())
        {
            w.Line("try");
            using (w.OpenBlock())
            {
                w.Line("return GetString(name, args);");
            }

            w.Line("catch (ArgumentException)");
            using (w.OpenBlock())
            {
                w.Line("return \"\";");
            }
        }

        w.Line();

        if (level == Level.BaseType)
        {
            w.Line("protected abstract Dictionary<string, string> InitializeLookupResources();");
            w.Line("#endregion");
            w.Line();
        }
    }

    private static void WriteInitializeLookupDictionary(CodeWriter w, IGenerator generator, Level level)
    {
        var modifier = level == Level.Implementation ? "protected override" : "static";
        if (level == Level.Implementation)
        {
            w.Line("#region Generic string lookup (by dictionary)");
        }

        w.Line($"{modifier} Dictionary<string, string> InitializeLookupResources() =>");
        w.PushIndent();
        w.Line("new Dictionary<string, string>");
        w.OpenBlockForSemicolonClose();
        foreach (var res in generator.Data.Resources)
        {
            w.Line($"{{ \"{res.Key}\", \"{res.CleanValueImplementationForDoubleQuotedLiteral}\" }},");
        }

        w.CloseBlockWithSemicolon();
        w.PopIndent();

        if (level == Level.Implementation)
        {
            w.Line("#endregion");
        }
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
        return string.Join("\n", DocumentationLines(declaration, context, values));
    }

    private static string FormatForDoc(string v)
    {
        return DocumentationEncoder.Encode(
            v.Replace("\\", "")
             .Replace("\"", "'")
        );
    }
}
