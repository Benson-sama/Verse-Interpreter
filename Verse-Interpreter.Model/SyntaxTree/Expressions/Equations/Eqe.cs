namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

public class Eqe : Expression
{
    public required IExpressionOrEquation Eq { get; set; }

    public required Expression E { get; set; }

    public override Eqe DeepCopy()
    {
        return new Eqe
        {
            Eq = Eq.DeepCopy(),
            E = E.DeepCopy()
        };
    }

    public override Eqe DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
    {
        return new Eqe
        {
            Eq = Eq.DeepCopyButReplaceChoice(choice, newExpression),
            E = E.DeepCopyButReplaceChoice(choice, newExpression)
        };
    }

    public override string ToString() => $"{Eq}; {E}";
}
