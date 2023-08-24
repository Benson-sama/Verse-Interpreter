using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public abstract class Expression : SyntaxTreeNode, IExpressionOrEquation
{
    public abstract override Expression DeepCopy();

    public abstract override Expression DeepCopyButReplaceChoice(Choice choice, Expression newExpression);

    IExpressionOrEquation IExpressionOrEquation.DeepCopy() => DeepCopy();

    IExpressionOrEquation IExpressionOrEquation.DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
        => DeepCopyButReplaceChoice(choice, newExpression);
}
