using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

public class Equation : IExpressionOrEquation
{
    public required Expression E { get; set; }

    public required Value V { get; set; }

    public void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString() => $"{V}={E}";
}
