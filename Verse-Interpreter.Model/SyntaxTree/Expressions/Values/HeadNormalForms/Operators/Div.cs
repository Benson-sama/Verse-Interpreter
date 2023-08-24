namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

public class Div : Operator
{
    public override Div DeepCopy() => new();

    public override Div DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
        => new();

    public override string ToString() => "Div";
}
