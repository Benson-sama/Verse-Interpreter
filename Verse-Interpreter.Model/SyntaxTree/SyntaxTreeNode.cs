using Verse_Interpreter.Model.SyntaxTree.Expressions;

namespace Verse_Interpreter.Model.SyntaxTree;

public abstract class SyntaxTreeNode
{
    public abstract SyntaxTreeNode DeepCopy();

    public abstract SyntaxTreeNode DeepCopyButReplaceChoice(Choice choice, Expression newExpression);
}
