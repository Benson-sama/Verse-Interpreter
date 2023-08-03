using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public class Application : Expression
{
    public required Value V1 { get; set; }

    public required Value V2 { get; set; }

    public override string ToString() => $"{V1} {V2}";
}
