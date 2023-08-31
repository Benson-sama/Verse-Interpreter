using System.Xml.Linq;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

public class Integer : HeadNormalForm
{
    public Integer(int value) =>
        Value = value;

    public int Value { get; set; }

    public bool Equals(Integer? other)
    {
        if (other is null)
            return false;

        if (Value == other.Value)
            return true;

        return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Integer integer)
            return Equals(integer);

        return false;
    }

    public override int GetHashCode()
        => Value.GetHashCode();

    public override Integer DeepCopy()
        => new(Value);

    public override Integer DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
        => new(Value);

    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString() => $"{Value}";
}
