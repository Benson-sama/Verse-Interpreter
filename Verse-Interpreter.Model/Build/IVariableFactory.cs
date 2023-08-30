using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.Build;

public interface IVariableFactory
{
    Variable Next();

    void RegisterUsedName(string name);
}
