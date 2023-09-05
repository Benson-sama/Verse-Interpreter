//-----------------------------------------------------------
// <copyright file="Lt.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Lt class.</summary>
//-----------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

/// <summary>
/// Class <see cref="Lt"/> serves as an <see cref="Operator"/> for "less than" comparisons.
/// </summary>
public class Lt : Operator
{
    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    /// <summary>
    /// This method creates a string representation of this <see cref="Lt"/>.
    /// </summary>
    /// <returns>Always "<see cref="Lt"/>".</returns>
    public override string ToString() => nameof(Lt);
}
