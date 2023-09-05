namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public class Choice : Expression
{
    public required Expression E1 { get; set; }

    public required Expression E2 { get; set; }

    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString() => $"{E1} | {E2}";
}
