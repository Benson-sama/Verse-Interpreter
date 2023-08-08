namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

public class Variable : Value, IEquatable<Variable>
{
    public Variable(string name) =>
        Name = name;

    public string Name { get; set; }

    public override string ToString()
    {
        return Name.Length <= 5 ?
            Name
            : string.Concat(Name.Take(5).Concat("²"));
    }

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

    public override int GetHashCode()
        => Name.GetHashCode();
}
