using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public class Application : Expression
{
    public required Value V1 { get; set; }

    public required Value V2 { get; set; }

    public override Application DeepCopy()
    {
        return new Application
        {
            V1 = V1.DeepCopy(),
            V2 = V2.DeepCopy()
        };
    }

    public override string ToString() => $"{V1} {V2}";
}
