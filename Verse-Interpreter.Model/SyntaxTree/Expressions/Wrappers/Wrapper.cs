namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

public abstract class Wrapper : Expression
{
    public abstract override Wrapper DeepCopy();

    public abstract override Wrapper DeepCopyButReplaceChoice(Choice choice, Expression newExpression);

    public required Expression E { get; set; }
}
