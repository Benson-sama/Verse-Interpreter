namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

public class All : Wrapper
{
    public required Expression E { get; set; }

    public override string ToString() => $"All{{{E}}}";
}
