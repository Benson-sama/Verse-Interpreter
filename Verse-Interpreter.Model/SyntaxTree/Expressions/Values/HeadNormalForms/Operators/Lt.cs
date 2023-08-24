namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

public class Lt : Operator
{
    public override Lt DeepCopy() => new();

    public override Lt DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
        => new();

    public override string ToString() => "Lt";
}
