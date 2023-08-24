using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.SyntaxTree;

public class VerseProgram : SyntaxTreeNode
{
    public required Expression E { get; set; }

    public override VerseProgram DeepCopy()
        => new() { E = E.DeepCopy() };

    public override VerseProgram DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
        => new() { E = E.DeepCopyButReplaceChoice(choice, newExpression) };

    public override string ToString() => $"Program{E}";
}
