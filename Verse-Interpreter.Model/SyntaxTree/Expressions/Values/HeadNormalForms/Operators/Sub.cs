namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

public class Sub : Operator
{
    public override Sub DeepCopy() => new();

    public override string ToString() => "Sub";
}
