using Shouldly;
using Strings.ResourceGenerator.Generators.Emit;

namespace UnitTests.Emit;

public class CodeWriterTests
{
    [Fact]
    public void Empty_writer_produces_empty_string()
    {
        new CodeWriter().ToString().ShouldBe(string.Empty);
    }

    [Fact]
    public void Single_line_has_no_trailing_newline()
    {
        var w = new CodeWriter();
        w.Line("using System;");
        w.ToString().ShouldBe("using System;");
    }

    [Fact]
    public void Two_lines_joined_with_single_newline()
    {
        var w = new CodeWriter();
        w.Line("a");
        w.Line("b");
        w.ToString().ShouldBe("a\nb");
    }

    [Fact]
    public void Blank_line_is_empty_row_in_join()
    {
        var w = new CodeWriter();
        w.Line("a");
        w.Line();
        w.Line("b");
        w.ToString().ShouldBe("a\n\nb");
    }

    [Fact]
    public void Null_line_is_skipped()
    {
        var w = new CodeWriter();
        w.Line("x");
        w.Line(null);
        w.Line("y");
        w.ToString().ShouldBe("x\ny");
    }

    [Fact]
    public void EmitLine_empty_string_inserts_blank_line()
    {
        var w = new CodeWriter();
        w.Line("a");
        w.EmitLine("");
        w.Line("b");
        w.ToString().ShouldBe("a\n\nb");
    }

    [Fact]
    public void PushIndent_PopIndent_and_Depth_track_logical_levels()
    {
        var w = new CodeWriter();
        w.Depth.ShouldBe(0);
        w.PushIndent();
        w.Depth.ShouldBe(1);
        w.Line("a");
        w.PopIndent();
        w.Depth.ShouldBe(0);
        w.ToString().ShouldBe("    a");
    }

    [Fact]
    public void Indented_scope_restores_depth_on_dispose()
    {
        var w = new CodeWriter();
        w.Line("outer");
        using (w.Indented())
        {
            w.Line("inner");
        }

        w.Line("outer2");
        w.ToString().ShouldBe("outer\n    inner\nouter2");
    }

    [Fact]
    public void EmitLine_splits_multiline_and_trims_trailing_blank_from_terminal_newline()
    {
        var w = new CodeWriter();
        w.EmitLine("a\nb\n");
        w.ToString().ShouldBe("a\nb");
    }

    [Fact]
    public void OpenBlock_and_CloseBlock_emit_braces_at_correct_indent()
    {
        var w = new CodeWriter();
        w.Line("namespace N");
        w.OpenBlock();
        w.Line("class C");
        w.OpenBlock();
        w.Line("void M();");
        w.CloseBlock();
        w.CloseBlock();

        // Match CodeWriter / string.Join: LF only (not CRLF from source literals on Windows).
        w.ToString().ShouldBe(
            "namespace N\n"
            + "{\n"
            + "    class C\n"
            + "    {\n"
            + "        void M();\n"
            + "    }\n"
            + "}");
    }

    [Fact]
    public void Using_OpenBlock_matches_manual_Open_Close()
    {
        var manual = new CodeWriter();
        manual.Line("class C");
        manual.OpenBlock();
        manual.Line("// body");
        manual.CloseBlock();

        var scoped = new CodeWriter();
        scoped.Line("class C");
        using (scoped.OpenBlock())
        {
            scoped.Line("// body");
        }

        scoped.ToString().ShouldBe(manual.ToString());
    }

    [Fact]
    public void Nested_using_disposes_inner_before_outer()
    {
        var w = new CodeWriter();
        w.Line("a");
        using (w.OpenBlock())
        {
            w.Line("b");
            using (w.OpenBlock())
            {
                w.Line("c");
            }

            w.Line("d");
        }

        w.ToString().ShouldBe(
            "a\n"
            + "{\n"
            + "    b\n"
            + "    {\n"
            + "        c\n"
            + "    }\n"
            + "    d\n"
            + "}");
    }

    [Fact]
    public void CloseBlock_without_Open_throws()
    {
        var w = new CodeWriter();
        Should.Throw<InvalidOperationException>(() => w.CloseBlock());
    }

    [Fact]
    public void CloseBlockWithSemicolon_emits_brace_semicolon_and_restores_depth()
    {
        var w = new CodeWriter();
        w.Line("x =>");
        w.OpenBlockForSemicolonClose();
        w.Line("y");
        w.CloseBlockWithSemicolon();
        w.ToString().ShouldBe("x =>\n{\n    y\n};");
    }

    [Fact]
    public void IndentWidth_matches_four_spaces_per_level()
    {
        var w = new CodeWriter();
        w.OpenBlock();
        w.OpenBlock();
        w.Line("x");
        w.CloseBlock();
        w.CloseBlock();

        var lines = w.ToString().Split('\n');
        lines[2].ShouldStartWith(new string(' ', CodeWriter.IndentWidth * 2));
    }
}
