namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

public class Fail : Expression
{
    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString() => "false?";
}
