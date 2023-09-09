//------------------------------------------------------------
// <copyright file="Equation.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Equation class.</summary>
//------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

/// <summary>
/// Class <see cref="Equation"/> serves as an equation in the Verse syntax.
/// </summary>
public class Equation : IExpressionOrEquation
{
    /// <summary>
    /// Property <c>V</c> represents the <see cref="Value"/> that gets the <see cref="Expression"/> assigned.
    /// </summary>
    public required Value V { get; set; }

    /// <summary>
    /// Property <c>E</c> represents the <see cref="Expression"/> that gets assigned to the <see cref="Value"/>.
    /// </summary>
    public required Expression E { get; set; }

    /// <summary>
    /// This method accepts an <see cref="ISyntaxTreeNodeVisitor"/> in order to call it back.
    /// </summary>
    /// <param name="visitor"><c>visitor</c> is the <see cref="ISyntaxTreeNodeVisitor"/> to call back.</param>
    public void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    /// <summary>
    /// This method accepts an <see cref="ISyntaxTreeNodeVisitor{T}"/> in order to call it back.
    /// </summary>
    /// <typeparam name="T">The return type of this operation.</typeparam>
    /// <param name="visitor"><c>visitor</c> is the <see cref="ISyntaxTreeNodeVisitor{T}"/> to call back.</param>
    /// <returns>The result of the <c>visitor</c> callback.</returns>
    public T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);

    /// <summary>
    /// This method creates a string representation of this <see cref="Equation"/>.
    /// </summary>
    /// <returns>A string in the format "({V}={E})".</returns>
    public override string ToString() => $"{V}={E}";
}
