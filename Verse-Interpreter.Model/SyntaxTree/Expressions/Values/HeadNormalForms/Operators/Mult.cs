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
    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    /// <summary>
    /// This method creates a string representation of this <see cref="Mult"/>.
    /// </summary>
    /// <returns>Always "<see cref="Mult"/>".</returns>
    public override string ToString() => nameof(Mult);
}
