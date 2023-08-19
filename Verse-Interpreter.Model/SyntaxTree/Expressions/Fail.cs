namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public class Fail : Expression
{
    public override Fail DeepCopy() => new();

    public override string ToString() => "fail";
}
