namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

public class Integer : HeadNormalForm
{
    public Integer(int value) =>
        Value = value;

    public int Value { get; set; }

    public override Integer DeepCopy()
        => new(Value);

    public override string ToString() => $"{Value}";
}
