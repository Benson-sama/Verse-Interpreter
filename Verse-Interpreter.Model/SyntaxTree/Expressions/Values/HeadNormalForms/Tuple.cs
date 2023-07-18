namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

public class Tuple : HeadNormalForm
{
    public required Value[] Values { get; set; }
}
