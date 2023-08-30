using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.Build;

public class VariableFactory : IVariableFactory
{
    private List<string> _registeredNames = new List<string>();

    private char? _currentCharacter = 'x';

    private int _currentNumber = 0;

    public Variable Next()
    {
        if (_currentCharacter is null)
            return new Variable(Guid.NewGuid().ToString());

        string name = $"{_currentCharacter}{_currentNumber}";
        if (_registeredNames.Contains(name))
            return Next();

        Variable variable = new(name);

        if (_currentNumber is int.MaxValue)
            UseNextCharacter();
        else
            _currentNumber++;

        return variable;
    }

    public void RegisterUsedName(string name)
    {
        if (_registeredNames.Contains(name))
            throw new Exception($"Unable to register name \"{name}\" as it is already used.");

        _registeredNames.Add(name);
    }

    private void UseNextCharacter()
    {
        _currentCharacter = _currentCharacter switch
        {
            'x' => 'y',
            'y' => 'z',
            'z' => 'f',
            'f' => 'g',
            _ => throw new ArgumentNullException(nameof(_currentCharacter), "Cannot be null.")
        };
        _currentNumber = 0;
    }
}
