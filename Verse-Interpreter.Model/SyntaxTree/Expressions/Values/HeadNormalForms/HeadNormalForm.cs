namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

public abstract class HeadNormalForm : Value
{
    public abstract override HeadNormalForm DeepCopy();

    public abstract override HeadNormalForm DeepCopyButReplaceChoice(Choice choice, Expression newExpression);
}
