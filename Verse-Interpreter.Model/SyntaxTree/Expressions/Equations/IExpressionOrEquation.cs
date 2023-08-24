namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

public interface IExpressionOrEquation
{
    public IExpressionOrEquation DeepCopy();

    public IExpressionOrEquation DeepCopyButReplaceChoice(Choice choice, Expression newExpression);
}
