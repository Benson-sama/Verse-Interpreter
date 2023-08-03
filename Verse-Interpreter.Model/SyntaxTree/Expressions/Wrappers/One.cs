namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

public class One : Wrapper
{
    public required Expression E { get; set; }

    public override string ToString() => $"One{{{E}}}";
}
