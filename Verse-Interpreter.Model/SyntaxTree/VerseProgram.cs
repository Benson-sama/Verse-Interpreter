using Verse_Interpreter.Model.SyntaxTree.Expressions;

namespace Verse_Interpreter.Model.SyntaxTree;

public class VerseProgram : SyntaxTreeNode
{
    public required Expression E { get; set; }

    public override string ToString() => $"Program{E}";
}
