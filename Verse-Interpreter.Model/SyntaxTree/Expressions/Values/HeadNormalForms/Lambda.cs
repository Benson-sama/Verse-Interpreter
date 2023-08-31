using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

public class Lambda : HeadNormalForm
{
    public required Variable Parameter { get; set; }

    public required Expression E { get; set; }

    public override Lambda DeepCopy()
    {
        return new Lambda
        {
            Parameter = Parameter.DeepCopy(),
            E = E.DeepCopy()
        };
    }

    public override Lambda DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
    {
        return new Lambda
        {
            Parameter = Parameter.DeepCopyButReplaceChoice(choice, newExpression),
            E = E.DeepCopyButReplaceChoice(choice, newExpression)
        };
    }

    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString() => $"({Parameter} => {E})";
}
