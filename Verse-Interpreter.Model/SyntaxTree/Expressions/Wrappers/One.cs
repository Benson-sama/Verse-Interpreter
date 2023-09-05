namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

public class One : Wrapper
{
    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString() => $"One{{{E}}}";
}
