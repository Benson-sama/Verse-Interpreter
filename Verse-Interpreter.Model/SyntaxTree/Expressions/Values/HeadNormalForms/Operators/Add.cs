//-----------------------------------------------------------
// <copyright file="Add.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Add class.</summary>
//-----------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

/// <summary>
/// Class <see cref="Add"/> serves as an <see cref="Operator"/> for arithmetic additions.
/// </summary>
public class Add : Operator
{
    /// <summary>
    /// This method creates a deep copy of this <see cref="Add"/> to avoid shared references.
    /// </summary>
    /// <returns>The new <see cref="Add"/> as a deep copy of this instance.</returns>
    public override Add DeepCopy() => new();

    public override Add DeepCopyButReplaceChoice(Choice choice, Expression newExpression)
        => new();

    /// <summary>
    /// This method creates a string representation of this <see cref="Add"/>.
    /// </summary>
    /// <returns>Always "<see cref="Add"/>".</returns>
    public override string ToString() => nameof(Add);
}
