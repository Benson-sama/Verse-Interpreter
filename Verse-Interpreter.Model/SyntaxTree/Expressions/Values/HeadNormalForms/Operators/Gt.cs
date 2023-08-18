namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

public class Gt : Operator
{
    public override Gt DeepCopy() => new();

    public override string ToString() => "Gt";
}
