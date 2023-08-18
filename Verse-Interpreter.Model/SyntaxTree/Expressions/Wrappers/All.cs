namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

public class All : Wrapper
{
    public override All DeepCopy()
    {
        return new All
        {
            E = E.DeepCopy()
        };
    }

    public override string ToString() => $"All{{{E}}}";
}
