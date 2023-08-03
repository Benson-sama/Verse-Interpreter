using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

public class Equation : Expression
{
    public required Expression E { get; set; }

    public required Value V { get; set; }

    public override string ToString() => $"{V}={E}";
}
