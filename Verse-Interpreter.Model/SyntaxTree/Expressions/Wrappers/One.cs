namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

public class One : Wrapper
{
    public override One DeepCopy()
    {
        return new One
        {
            E = E.DeepCopy()
        };
    }

    public override One DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
    {
        return new One
        {
            E = E.DeepCopyButReplaceChoice(choice, newExpression)
        };
    }

    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString() => $"One{{{E}}}";
}
