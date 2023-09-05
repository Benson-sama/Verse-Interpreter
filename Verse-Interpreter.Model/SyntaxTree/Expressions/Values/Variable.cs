namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

public class Variable : Value, IEquatable<Variable>
{
    public Variable(string name) =>
        Name = name;

    public string Name { get; set; }

    public bool Equals(Variable? other)
    {
        if (other is null)
            return false;

        if (Name == other.Name)
            return true;

        return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Variable variable)
            return Equals(variable);

        return false;
    }

    public static bool operator ==(Variable firstVariable, Variable secondVariable)
        => firstVariable.Equals(secondVariable);

    public static bool operator !=(Variable firstVariable, Variable secondVariable)
        => !firstVariable.Equals(secondVariable);

    public override int GetHashCode()
        => Name.GetHashCode();

    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    public override string ToString()
    {
        return Name.Length <= 5 ?
            Name
            : string.Concat(Name.Take(5).Concat("²"));
    }
}
