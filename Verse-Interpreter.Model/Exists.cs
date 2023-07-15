namespace Verse_Interpreter.Model;

public class Exists : Expression
{
    public required Variable V { get; set; }

    public required Expression E { get; set; }
}
