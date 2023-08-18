namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

public class Mult : Operator
{
    public override Mult DeepCopy() => new();

    public override string ToString() => "Mult";
}
