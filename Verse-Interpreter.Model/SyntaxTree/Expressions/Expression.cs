using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public abstract class Expression : SyntaxTreeNode, IExpressionOrEquation, ISyntaxTreeNodeVisitable
{
    public abstract void Accept(ISyntaxTreeNodeVisitor visitor);

    public abstract T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor);
}
