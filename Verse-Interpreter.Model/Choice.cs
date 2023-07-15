namespace Verse_Interpreter.Model;

public class Choice : Expression
{
    public required Expression E1 { get; set; }

    public required Expression E2 { get; set; }
}
