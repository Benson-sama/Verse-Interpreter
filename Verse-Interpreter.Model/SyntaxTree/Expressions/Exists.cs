using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public class Exists : Expression
{
    public required Variable V { get; set; }

    public required Expression E { get; set; }

    public override Exists DeepCopy()
    {
        return new Exists
        {
            V = V.DeepCopy(),
            E = E.DeepCopy()
        };
    }

    public override Exists DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
    {
        return new Exists
        {
            V = V.DeepCopyButReplaceChoice(choice, newExpression),
            E = E.DeepCopyButReplaceChoice(choice, newExpression)
        };
    }

    public override string ToString() => $"E{V}. {E}";
}
