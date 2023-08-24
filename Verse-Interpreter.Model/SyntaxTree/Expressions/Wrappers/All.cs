using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

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

    public override All DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
    {
        return new All
        {
            E = E.DeepCopyButReplaceChoice(choice, newExpression)
        };
    }

    public override string ToString() => $"All{{{E}}}";
}
