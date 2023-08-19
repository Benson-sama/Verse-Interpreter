using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.SyntaxTree;

public class VerseProgram : SyntaxTreeNode
{
    public required Wrapper Wrapper { get; set; }

    public override VerseProgram DeepCopy()
        => new() { Wrapper = Wrapper.DeepCopy() };

    public override string ToString() => $"Program{Wrapper}";
}
