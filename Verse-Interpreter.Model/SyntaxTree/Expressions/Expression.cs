using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public abstract class Expression : SyntaxTreeNode, IExpressionOrEquation, ISyntaxTreeNodeVisitable
{
    public abstract override Expression DeepCopy();

    public abstract override Expression DeepCopyButReplaceChoice(Choice choice, Expression newExpression);

    IExpressionOrEquation IExpressionOrEquation.DeepCopy() => DeepCopy();

    IExpressionOrEquation IExpressionOrEquation.DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
        => DeepCopyButReplaceChoice(choice, newExpression);

    public abstract void Accept(ISyntaxTreeNodeVisitor visitor);

    public abstract T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor);
}
