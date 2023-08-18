namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

public class Eqe : Expression
{
    public required Expression Eq { get; set; }

    public required Expression E { get; set; }

    public override Eqe DeepCopy()
    {
        return new Eqe
        {
            Eq = Eq.DeepCopy(),
            E = E.DeepCopy()
        };
    }

    public override string ToString() => $"{Eq}; {E}";
}
