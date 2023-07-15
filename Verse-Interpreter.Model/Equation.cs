namespace Verse_Interpreter.Model;

public class Equation : Expression
{
    public required Expression E { get; set; }

    public required Value V { get; set; }
}
