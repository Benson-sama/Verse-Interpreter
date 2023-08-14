using System.Collections;
using System.Text;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

public class VerseTuple : HeadNormalForm, IEnumerable<Value>
{
    private static readonly VerseTuple _empty = new(Array.Empty<Value>());

    public VerseTuple(Value value) => Values = Enumerable.Repeat(value, 1);

    public VerseTuple(Value firstValue, Value secondValue) => Values = new Value[] { firstValue, secondValue };

    public VerseTuple(IEnumerable<Value> values) => Values = values;

    public static VerseTuple Empty { get => _empty; }

    public IEnumerable<Value> Values { get; set; }

    public IEnumerator<Value> GetEnumerator() => Values.GetEnumerator();

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

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
