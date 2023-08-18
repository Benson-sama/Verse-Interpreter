namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

public class Add : Operator
{
    public override Add DeepCopy() => new();

    public override string ToString() => "Add";
}
