using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public class Exists : Expression
{
    public required Variable V { get; set; }

    public required Expression E { get; set; }

    public override string ToString() => $"∃{V}. {E}";
}
