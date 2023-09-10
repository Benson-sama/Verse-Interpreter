//---------------------------------------------------------------
// <copyright file="Application.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Application class.</summary>
//---------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

/// <summary>
/// Class <see cref="Application"/> serves as an <see cref="Expression"/> for value applications in the Verse syntax.
/// </summary>
public class Application : Expression
{
    /// <summary>
    /// Field <c>V1</c> represents the <see cref="Value"/> that gets the second <see cref="Value"/> applied.
    /// </summary>
    public required Value V1 { get; set; }

    /// <summary>
    /// Field <c>V2</c> represents the <see cref="Value"/> that gets applied to the first <see cref="Value"/>.
    /// </summary>
    public required Value V2 { get; set; }

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
    /// This method creates a string representation of this <see cref="Application"/>.
    /// </summary>
    /// <returns>A string in the format "{V1} {V2}".</returns>
    public override string ToString() => $"{V1} {V2}";
}
