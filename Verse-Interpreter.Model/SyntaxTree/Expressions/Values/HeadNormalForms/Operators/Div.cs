//-----------------------------------------------------------
// <copyright file="Div.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Div class.</summary>
//-----------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

/// <summary>
/// Class <see cref="Div"/> serves as an <see cref="Operator"/> for arithmetic divisions.
/// </summary>
public class Div : Operator
{
    /// <summary>
    /// This method creates a deep copy of this <see cref="Div"/> to avoid shared references.
    /// </summary>
    /// <returns>The new <see cref="Div"/> as a deep copy of this instance.</returns>
    public override Div DeepCopy() => new();

    public override Div DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
        => new();

    /// <summary>
    /// This method creates a string representation of this <see cref="Div"/>.
    /// </summary>
    /// <returns>Always "<see cref="Div"/>".</returns>
    public override string ToString() => nameof(Div);
}
