using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public class Eqe : Expression
{
    public required IExpressionOrEquation Eq { get; set; }

    public required Expression E { get; set; }

    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString() => $"{Eq}; {E}";
}
