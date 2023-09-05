//-----------------------------------------------------------
// <copyright file="Gt.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Gt class.</summary>
//-----------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

/// <summary>
/// Class <see cref="Gt"/> serves as an <see cref="Operator"/> for "greater than" comparisons.
/// </summary>
public class Gt : Operator
{
    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    /// <summary>
    /// This method creates a string representation of this <see cref="Gt"/>.
    /// </summary>
    /// <returns>Always "<see cref="Gt"/>".</returns>
    public override string ToString() => nameof(Gt);
}
