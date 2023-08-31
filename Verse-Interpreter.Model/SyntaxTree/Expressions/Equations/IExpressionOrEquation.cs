namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

public interface IExpressionOrEquation : ISyntaxTreeNodeVisitable
{
    public IExpressionOrEquation DeepCopy();

    public IExpressionOrEquation DeepCopyButReplaceChoice(Choice choice, Expression newExpression);
}
