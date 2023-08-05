using Verse_Interpreter.Model.SyntaxTree.Expressions;

namespace Verse_Interpreter.Model;

public interface IRewriter
{
    Expression TryRewrite(Expression expression);

    bool RuleApplied { get; }
}
