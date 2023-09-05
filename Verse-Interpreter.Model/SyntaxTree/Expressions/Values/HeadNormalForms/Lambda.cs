using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

public class Lambda : HeadNormalForm
{
    public required Variable Parameter { get; set; }

    public required Expression E { get; set; }

    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString() => $"({Parameter} => {E})";
}
