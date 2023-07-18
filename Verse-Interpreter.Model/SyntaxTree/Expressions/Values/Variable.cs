namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

public class Variable : Value
{
    public Variable(string name) =>
        Name = name;

    public string Name { get; set; }
}
