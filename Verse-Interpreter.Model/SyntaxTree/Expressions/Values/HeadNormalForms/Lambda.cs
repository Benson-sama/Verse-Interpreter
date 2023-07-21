namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

public class Lambda : HeadNormalForm
{
    public required IEnumerable<Variable> Parameters { get; set; }

    public required Expression E { get; set; }
}
