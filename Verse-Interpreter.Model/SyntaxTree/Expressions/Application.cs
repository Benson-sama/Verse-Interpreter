using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public class Application : Expression
{
    public required Value V1 { get; set; }

    public required Value V2 { get; set; }

    public override Application DeepCopy()
    {
        return new Application
        {
            V1 = V1.DeepCopy(),
            V2 = V2.DeepCopy()
        };
    }

    public override Application DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
    {
        return new Application
        {
            V1 = V1.DeepCopyButReplaceChoice(choice, newExpression),
            V2 = V2.DeepCopyButReplaceChoice(choice, newExpression)
        };
    }

    public override string ToString() => $"{V1} {V2}";
}
