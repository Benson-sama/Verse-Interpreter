//--------------------------------------------------------------
// <copyright file="Expression.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Expression class.</summary>
//--------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

namespace Verse_Interpreter.Model.SyntaxTree.Expressions;

/// <summary>
/// Class <see cref="Expression"/> serves as a base class for expressions in the Verse syntax.
/// </summary>
public abstract class Expression : SyntaxTreeNode, IExpressionOrEquation, ISyntaxTreeNodeVisitable
{
    /// <summary>
    /// This method accepts an <see cref="ISyntaxTreeNodeVisitor"/> in order to call it back.
    /// </summary>
    /// <param name="visitor"><c>visitor</c> is the <see cref="ISyntaxTreeNodeVisitor"/> to call back.</param>
    public abstract void Accept(ISyntaxTreeNodeVisitor visitor);

    /// <summary>
    /// This method accepts an <see cref="ISyntaxTreeNodeVisitor{T}"/> in order to call it back.
    /// </summary>
    /// <typeparam name="T">The return type of this operation.</typeparam>
    /// <param name="visitor"><c>visitor</c> is the <see cref="ISyntaxTreeNodeVisitor{T}"/> to call back.</param>
    /// <returns>The result of the <c>visitor</c> callback.</returns>
    public abstract T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor);
}
