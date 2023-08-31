namespace Verse_Interpreter.Model.SyntaxTree;

public interface ISyntaxTreeNodeVisitable
{
    void Accept(ISyntaxTreeNodeVisitor visitor);

    T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor);
}
