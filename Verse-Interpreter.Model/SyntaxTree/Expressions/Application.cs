using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public class Application : Expression
{
    public required Value V1 { get; set; }

    public required Value V2 { get; set; }

    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString() => $"{V1} {V2}";
}
