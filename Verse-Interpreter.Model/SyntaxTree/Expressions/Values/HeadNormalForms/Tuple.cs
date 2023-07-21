namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

public class Tuple : HeadNormalForm
{
    private static readonly Tuple _empty = new() { Values = Array.Empty<Value>() };

    public static Tuple Empty { get => _empty; }

    public required IEnumerable<Value> Values { get; set; }
}
