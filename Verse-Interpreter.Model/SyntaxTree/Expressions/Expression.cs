namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public abstract class Expression : Node
{
    public abstract Expression DeepCopy();
}
