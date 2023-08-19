using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public abstract class Expression : SyntaxTreeNode, IExpressionOrEquation
{
    public abstract override Expression DeepCopy();

    IExpressionOrEquation IExpressionOrEquation.DeepCopy() => DeepCopy();
}
