//-----------------------------------------------------------
// <copyright file="Sub.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Sub class.</summary>
//-----------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

/// <summary>
/// Class <see cref="Sub"/> serves as an <see cref="Operator"/> for arithmetic subtractions.
/// </summary>
public class Sub : Operator
{
    /// <summary>
    /// This method creates a deep copy of this <see cref="Sub"/> to avoid shared references.
    /// </summary>
    /// <returns>The new <see cref="Sub"/> as a deep copy of this instance.</returns>
    public override Sub DeepCopy() => new();

    public override Sub DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
        => new();

    /// <summary>
    /// This method creates a string representation of this <see cref="Sub"/>.
    /// </summary>
    /// <returns>Always "<see cref="Sub"/>".</returns>
    public override string ToString() => nameof(Sub);
}
