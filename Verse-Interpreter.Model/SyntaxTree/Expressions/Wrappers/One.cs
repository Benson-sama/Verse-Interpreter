namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

public class One : Wrapper
{
    public override One DeepCopy()
    {
        return new One
        {
            E = E.DeepCopy()
        };
    }

    public override One DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
    {
        return new One
        {
            E = E.DeepCopyButReplaceChoice(choice, newExpression)
        };
    }

    public override string ToString() => $"One{{{E}}}";
}
