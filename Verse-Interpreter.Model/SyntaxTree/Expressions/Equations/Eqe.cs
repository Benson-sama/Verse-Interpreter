namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

public class Eqe : Expression
{
    public required Expression Eq { get; set; }

    public required Expression E { get; set; }
}
