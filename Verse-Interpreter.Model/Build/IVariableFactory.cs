//-------------------------------------------------------------------
// <copyright file="IVariableFactory.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the IVariableFactory class.</summary>
//-------------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.Build;

/// <summary>
/// Interface <see cref="IVariableFactory"/> serves as an interface for <see cref="Variable"/> factories.
/// </summary>
public interface IVariableFactory
{
    /// <summary>
    /// Generates a fresh <see cref="Variable"/>.
    /// </summary>
    /// <returns>The generated <see cref="Variable"/>.</returns>
    Variable Next();

    /// <summary>
    /// Registers the <paramref name="name"/> as used to prevent generating with it.
    /// </summary>
    /// <param name="name"><c>name</c> is the name to register as used.</param>
    void RegisterUsedName(string name);
}
