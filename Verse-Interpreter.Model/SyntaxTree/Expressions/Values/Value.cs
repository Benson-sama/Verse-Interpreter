namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

public abstract class Value : Expression
{
    public abstract override Value DeepCopy();

    public abstract override Value DeepCopyButReplaceChoice(Choice choice, Expression newExpression);
}
