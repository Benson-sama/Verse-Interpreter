namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

public class Gt : Operator
{
    public override Gt DeepCopy() => new();

    public override Gt DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
        => new();

    public override string ToString() => "Gt";
}
