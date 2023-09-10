//-----------------------------------------------------------
// <copyright file="Exists.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Exists class.</summary>
//-----------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

/// <summary>
/// Class <see cref="Exists"/> serves as an <see cref="Expression"/> to bring a <see cref="Variable"/> into scope in the Verse syntax.
/// </summary>
public class Exists : Expression
{
    /// <summary>
    /// Field <c>V</c> represents the <see cref="Variable"/> to bring into scope.
    /// </summary>
    public required Variable V { get; set; }

    /// <summary>
    /// Field <c>E</c> represents the child <see cref="Expression"/> where <c>V</c> is in scope.
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
    /// This method creates a string representation of this <see cref="Exists"/>.
    /// </summary>
    /// <returns>A string in the format "E{V}. {E}".</returns>
    public override string ToString() => $"E{V}. {E}";
}
