//-----------------------------------------------------------
// <copyright file="Eqe.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Eqe class.</summary>
//-----------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

/// <summary>
/// Class <see cref="Eqe"/> serves as an <see cref="Expression"/> to sequence expressions in the Verse syntax.
/// </summary>
public class Eqe : Expression
{
    /// <summary>
    /// Field <c>Eq</c> represents the left side <see cref="IExpressionOrEquation"/> of the sequence.
    /// </summary>
    public required IExpressionOrEquation Eq { get; set; }

    /// <summary>
    /// Field <c>E</c> represents the right side <see cref="Expression"/> of the sequence.
    /// </summary>
    public required Expression E { get; set; }

    /// <summary>
    /// This method accepts an <see cref="ISyntaxTreeNodeVisitor"/> in order to call it back.
    /// </summary>
    /// <param name="visitor"><c>visitor</c> is the <see cref="ISyntaxTreeNodeVisitor"/> to call back.</param>
    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    /// <summary>
    /// This method accepts an <see cref="ISyntaxTreeNodeVisitor{T}"/> in order to call it back.
    /// </summary>
    /// <typeparam name="T">The return type of this operation.</typeparam>
    /// <param name="visitor"><c>visitor</c> is the <see cref="ISyntaxTreeNodeVisitor{T}"/> to call back.</param>
    /// <returns>The result of the <c>visitor</c> callback.</returns>
    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    /// <summary>
    /// This method creates a string representation of this <see cref="Eqe"/>.
    /// </summary>
    /// <returns>A string in the format "{Eq}; {E}".</returns>
    public override string ToString() => $"{Eq}; {E}";
}
