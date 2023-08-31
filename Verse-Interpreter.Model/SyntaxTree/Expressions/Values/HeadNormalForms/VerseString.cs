namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

public class VerseString : HeadNormalForm
{
    public VerseString(string text) =>
        Text = text;

    public string Text { get; set; }

    public bool Equals(VerseString? other)
    {
        if (other is null)
            return false;

        if (Text == other.Text)
            return true;

        return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is VerseString text)
            return Equals(text);

        return false;
    }

    public override int GetHashCode()
        => Text.GetHashCode();

    public override VerseString DeepCopy()
        => new(Text);

    public override VerseString DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
        => new(Text);

    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString() => $"\"{Text}\"";
}
