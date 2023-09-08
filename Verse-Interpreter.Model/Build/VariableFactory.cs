//-------------------------------------------------------------------
// <copyright file="VariableFactory.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the VariableFactory class.</summary>
//-------------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.Build;

/// <summary>
/// Class <see cref="VariableFactory"/> serves as a factory for generating fresh <see cref="Variable"/> instances.
/// </summary>
public class VariableFactory : IVariableFactory
{
    /// <summary>
    /// Field <c>_registeredNames</c> represents the list of registered names to avoid.
    /// </summary>
    private readonly List<string> _registeredNames = new();

    /// <summary>
    /// Field <c>_currentCharacter</c> represents the current character to be used for generating fresh variables.
    /// </summary>
    private char? _currentCharacter = 'x';

    /// <summary>
    /// Field <c>_currentNumber</c> represents the current number to be used for generating fresh variables.
    /// </summary>
    private int _currentNumber = 0;

    /// <summary>
    /// Generates a fresh <see cref="Variable"/> in the format "{<c>_currentCharacter</c>}{<c>_currentNumber</c>}".
    /// Incrementing <c>_currentNumber</c>, and incrementing <c>currentCharacter</c> on overflow.
    /// </summary>
    /// <returns>The generated <see cref="Variable"/>.</returns>
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

    /// <summary>
    /// Registers the <paramref name="name"/> as used to prevent generating with it.
    /// </summary>
    /// <param name="name"><c>name</c> is the name to register as used.</param>
    public void RegisterUsedName(string name)
    {
        if (_registeredNames.Contains(name))
            throw new Exception($"Unable to register name \"{name}\" as it is already used.");

        _registeredNames.Add(name);
    }

    /// <summary>
    /// Shifts the private <c>_currentCharacter</c> to the next. ('x' -> 'y' -> ... -> 'g')
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// Is raised when the <c>_currentCharacter</c> is 'g' on method call.
    /// </exception>
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
