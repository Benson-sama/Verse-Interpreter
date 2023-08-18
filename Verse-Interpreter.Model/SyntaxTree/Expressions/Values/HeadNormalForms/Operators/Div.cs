namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

public class Div : Operator
{
    public override Div DeepCopy() => new();

    public override string ToString() => "Div";
}
