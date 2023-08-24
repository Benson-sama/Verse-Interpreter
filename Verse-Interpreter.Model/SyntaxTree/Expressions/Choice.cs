namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public class Choice : Expression
{
    public required Expression E1 { get; set; }

    public required Expression E2 { get; set; }

    public override Choice DeepCopy()
    {
        return new Choice
        {
            E1 = E1.DeepCopy(),
            E2 = E2.DeepCopy()
        };
    }

    public override Expression DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
    {
        if (this == choice)
            return newExpression;

        return new Choice
        {
            E1 = E1.DeepCopyButReplaceChoice(choice, newExpression),
            E2 = E2.DeepCopyButReplaceChoice(choice, newExpression)
        };
    }

    public override string ToString() => $"{E1} | {E2}";
}
