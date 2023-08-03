using System.Text;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

public class Tuple : HeadNormalForm
{
    private static readonly Tuple _empty = new() { Values = Array.Empty<Value>() };

    public static Tuple Empty { get => _empty; }

    public required IEnumerable<Value> Values { get; set; }

    public override string ToString()
    {
        if (!Values.Any())
            return "()";

        StringBuilder sb = new("(");
        sb.Append($"{Values.First()}");

        foreach (Value value in Values.Skip(1))
        {
            sb.Append($", {value}");
        }

        sb.Append(')');
        return sb.ToString();
    }
}
