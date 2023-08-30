using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.SyntaxTree.Utility;

public class VariableBuffer
{
    public IEnumerable<Variable> BoundVariables { get; set; } = Enumerable.Empty<Variable>();

    public IEnumerable<Variable> FreeVariables { get; set; } = Enumerable.Empty<Variable>();

    public void Clear()
    {
        BoundVariables = Enumerable.Empty<Variable>();
        FreeVariables = Enumerable.Empty<Variable>();
    }

    public bool IsBound(Variable variable)
        => BoundVariables.Contains(variable);

    public bool IsFree(Variable variable)
        => !IsBound(variable);
}
