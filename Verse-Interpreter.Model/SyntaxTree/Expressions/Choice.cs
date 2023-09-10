﻿//-----------------------------------------------------------
// <copyright file="Choice.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Choice class.</summary>
//-----------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

/// <summary>
/// Class <see cref="Choice"/> serves as a choice between two <see cref="Expression"/>s in the Verse syntax.
/// </summary>
public class Choice : Expression
{
    /// <summary>
    /// Field <c>E1</c> represents the first choice.
    /// </summary>
    public required Expression E1 { get; set; }

    /// <summary>
    /// Field <c>E2</c> represents the second choice.
    /// </summary>
    public required Expression E2 { get; set; }

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
    /// This method creates a string representation of this <see cref="Choice"/>.
    /// </summary>
    /// <returns>A string in the format "{E1} {E2}".</returns>
    public override string ToString() => $"{E1} | {E2}";
}
