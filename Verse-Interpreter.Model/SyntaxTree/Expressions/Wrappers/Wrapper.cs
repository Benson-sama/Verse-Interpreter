namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

public abstract class Wrapper : Expression
{
    public required Expression E { get; set; }
}
