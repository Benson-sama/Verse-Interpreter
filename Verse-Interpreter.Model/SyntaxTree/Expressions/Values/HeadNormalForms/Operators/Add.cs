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
    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    /// <summary>
    /// This method creates a string representation of this <see cref="Add"/>.
    /// </summary>
    /// <returns>Always "<see cref="Add"/>".</returns>
    public override string ToString() => nameof(Add);
}
