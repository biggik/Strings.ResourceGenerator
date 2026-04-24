using System;
using System.Collections.Generic;

namespace Strings.ResourceGenerator.Generators.Emit;

/// <summary>
/// Emits C#-style indented lines and paired <c>{</c> / <c>}</c> blocks. Joins lines with LF only;
/// there is no extra newline after the final line (same idea as <c>string.Join("\n", lines)</c>).
/// </summary>
internal sealed class CodeWriter
{
    /// <summary>
    /// Spaces per indent level; matches <see cref="Indent.Spaces"/>.
    /// </summary>
    public const int IndentWidth = 4;

    private readonly List<string> _lines = [];
    private int _depth;

    /// <summary>
    /// Current logical indent depth (number of <see cref="IndentWidth"/>-space steps). Used after <see cref="PushIndent"/> / <see cref="OpenBlock"/>.
    /// </summary>
    public int Depth => _depth;

    private string Indent => _depth == 0 ? string.Empty : new string(' ', _depth * IndentWidth);

    /// <summary>
    /// Increases indent for following <see cref="Line(string)"/> calls without emitting a brace.
    /// </summary>
    public void PushIndent() => _depth++;

    /// <summary>
    /// Reverses <see cref="PushIndent"/>.
    /// </summary>
    public void PopIndent()
    {
        if (_depth == 0)
        {
            throw new InvalidOperationException("Unbalanced PopIndent.");
        }

        _depth--;
    }

    /// <summary>
    /// <see cref="PushIndent"/> on construction, <see cref="PopIndent"/> on dispose.
    /// </summary>
    public IndentedScope Indented() => new(this);

    /// <summary>
    /// Append one logical line at the current indent. Skips <paramref name="text"/> when it is <c>null</c>
    /// (useful when migrating callers that previously filtered null entries).
    /// </summary>
    public void Line(string text)
    {
        if (text == null)
        {
            return;
        }

        _lines.Add(Indent + text);
    }

    /// <summary>
    /// Append a blank line (empty content, no indent).
    /// </summary>
    public void Line() => _lines.Add(string.Empty);

    private void PushBlockDepthWithBraceLine()
    {
        _lines.Add(Indent + "{");
        _depth++;
    }

    /// <summary>
    /// Writes <c>{</c> at the current indent, increases depth, and returns a scope that emits the matching <c>}</c> on dispose
    /// (prefer <c>using (w.OpenBlock()) { … }</c> or <c>using var _ = w.OpenBlock();</c>).
    /// </summary>
    public BlockScope OpenBlock()
    {
        PushBlockDepthWithBraceLine();
        return new BlockScope(this);
    }

    /// <summary>
    /// Opens a block that must be closed with <see cref="CloseBlockWithSemicolon"/> (<c>};</c>), not <see cref="CloseBlock"/>.
    /// </summary>
    public void OpenBlockForSemicolonClose()
    {
        PushBlockDepthWithBraceLine();
    }

    /// <summary>
    /// Decrease depth and write <c>}</c> at the new current indent.
    /// </summary>
    /// <exception cref="InvalidOperationException">If there is no open block.</exception>
    public void CloseBlock()
    {
        if (_depth == 0)
        {
            throw new InvalidOperationException("Unbalanced block: CloseBlock with no matching OpenBlock.");
        }

        _depth--;
        _lines.Add(Indent + "}");
    }

    /// <summary>
    /// Like <see cref="CloseBlock"/>, but emits <c>};</c> (e.g. closing an object initializer that ends a statement).
    /// </summary>
    public void CloseBlockWithSemicolon()
    {
        if (_depth == 0)
        {
            throw new InvalidOperationException("Unbalanced block: CloseBlockWithSemicolon with no matching OpenBlock.");
        }

        _depth--;
        _lines.Add(Indent + "};");
    }

    /// <summary>
    /// Disposable scope returned by <see cref="OpenBlock"/>; calls <see cref="CloseBlock"/> on dispose.
    /// Reference type so nested <c>using</c> cannot duplicate <see cref="Dispose"/> via struct copies.
    /// </summary>
    public sealed class BlockScope : IDisposable
    {
        private CodeWriter _writer;

        internal BlockScope(CodeWriter writer) => _writer = writer;

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.CloseBlock();
                _writer = null;
            }
        }
    }

    /// <summary>
    /// Disposable scope that calls <see cref="PushIndent"/> on construction and <see cref="PopIndent"/> on dispose.
    /// </summary>
    public sealed class IndentedScope : IDisposable
    {
        private CodeWriter _writer;

        internal IndentedScope(CodeWriter writer)
        {
            _writer = writer;
            _writer.PushIndent();
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.PopIndent();
                _writer = null;
            }
        }
    }

    /// <summary>
    /// Emits pre-indented generator output: skips <paramref name="text"/> when null; blank physical line when empty;
    /// otherwise splits on newlines (normalizes CRLF to LF) and trims a trailing empty segment from a final newline.
    /// </summary>
    public void EmitLine(string text)
    {
        if (text == null)
        {
            return;
        }

        if (text.Length == 0)
        {
            _lines.Add(string.Empty);
            return;
        }

        var normalized = text.Replace("\r\n", "\n");
        var segments = normalized.Split('\n');
        var end = segments.Length;
        while (end > 0 && segments[end - 1].Length == 0)
        {
            end--;
        }

        for (var i = 0; i < end; i++)
        {
            _lines.Add(segments[i]);
        }
    }

    /// <summary>
    /// Joins all lines with <c>\n</c>. Does not append an extra newline after the last line.
    /// </summary>
    public override string ToString() => string.Join("\n", _lines);
}
