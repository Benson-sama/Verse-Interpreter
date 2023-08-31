using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

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

    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString() => $"{Eq}; {E}";
}
