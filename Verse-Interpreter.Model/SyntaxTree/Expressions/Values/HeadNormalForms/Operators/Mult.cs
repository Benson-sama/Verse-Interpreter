//-----------------------------------------------------------
// <copyright file="Mult.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Mult class.</summary>
//-----------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

/// <summary>
/// Class <see cref="Mult"/> serves as an <see cref="Operator"/> for arithmetic multiplications.
/// </summary>
public class Mult : Operator
{
    /// <summary>
    /// This method creates a deep copy of this <see cref="Mult"/> to avoid shared references.
    /// </summary>
    /// <returns>The new <see cref="Mult"/> as a deep copy of this instance.</returns>
    public override Mult DeepCopy() => new();

    public override Mult DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
        => new();

    /// <summary>
    /// This method creates a string representation of this <see cref="Mult"/>.
    /// </summary>
    /// <returns>Always "<see cref="Mult"/>".</returns>
    public override string ToString() => nameof(Mult);
}
