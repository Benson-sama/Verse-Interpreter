namespace Verse_Interpreter.Model;

public class Integer : HeadNormalForm
{
    public Integer(int value) =>
        Value = value;

    public int Value { get; set; }
}
