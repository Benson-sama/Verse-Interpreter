using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

public class Equation : IExpressionOrEquation
{
    public required Expression E { get; set; }

    public required Value V { get; set; }

    IExpressionOrEquation IExpressionOrEquation.DeepCopy()
    {
        return new Equation
        {
            V = V.DeepCopy(),
            E = E.DeepCopy()
        };
    }

    public override string ToString() => $"{V}={E}";

}
