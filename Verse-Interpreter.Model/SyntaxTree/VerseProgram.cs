using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.SyntaxTree;

public class VerseProgram : Node
{
    public required Wrapper Wrapper { get; set; }

    public override string ToString() => $"Program{Wrapper}";
}
