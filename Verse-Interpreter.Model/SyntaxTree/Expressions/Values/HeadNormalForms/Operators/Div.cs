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
    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    /// <summary>
    /// This method creates a string representation of this <see cref="Div"/>.
    /// </summary>
    /// <returns>Always "<see cref="Div"/>".</returns>
    public override string ToString() => nameof(Div);
}
