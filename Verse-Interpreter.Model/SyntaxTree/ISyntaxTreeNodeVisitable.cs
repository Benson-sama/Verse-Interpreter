//----------------------------------------------------------------------------
// <copyright file="ISyntaxTreeNodeVisitable.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ISyntaxTreeNodeVisitable interface.</summary>
//----------------------------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree;

/// <summary>
/// Interface <see cref="ISyntaxTreeNodeVisitable"/> defines methods that accept an
/// <see cref="ISyntaxTreeNodeVisitor"/> or <see cref="ISyntaxTreeNodeVisitor{T}"/>.
/// </summary>
public interface ISyntaxTreeNodeVisitable
{
    /// <summary>
    /// This method accepts an <see cref="ISyntaxTreeNodeVisitor"/> in order to call it back.
    /// </summary>
    /// <param name="visitor"><c>visitor</c> is the <see cref="ISyntaxTreeNodeVisitor"/> to call back.</param>
    void Accept(ISyntaxTreeNodeVisitor visitor);

    /// <summary>
    /// This method accepts an <see cref="ISyntaxTreeNodeVisitor{T}"/> in order to call it back.
    /// </summary>
    /// <typeparam name="T">The return type of this operation.</typeparam>
    /// <param name="visitor"><c>visitor</c> is the <see cref="ISyntaxTreeNodeVisitor{T}"/> to call back.</param>
    /// <returns>The result of the <c>visitor</c> callback.</returns>
    T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor);
}
