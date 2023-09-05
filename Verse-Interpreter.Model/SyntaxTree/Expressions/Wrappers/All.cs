using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

public class All : Wrapper
{
    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString() => $"All{{{E}}}";
}
