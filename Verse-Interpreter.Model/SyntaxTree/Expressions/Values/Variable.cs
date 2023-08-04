namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

public class Variable : Value
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
}
