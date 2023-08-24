namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

public abstract class Operator : HeadNormalForm
{
    public abstract override Operator DeepCopy();

    public abstract override Operator DeepCopyButReplaceChoice(Choice choice, Expression newExpression);
}
