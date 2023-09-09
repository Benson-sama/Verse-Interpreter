//-----------------------------------------------------------
// <copyright file="Lambda.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Lambda class.</summary>
//-----------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;

/// <summary>
/// Class <see cref="Lambda"/> serves as a <see cref="HeadNormalForm"/> for anonymous functions in the Verse syntax.
/// </summary>
public class Lambda : HeadNormalForm
{
    /// <summary>
    /// <c>Parameter</c> represents the parameter that this <see cref="Lambda"/> holds.
    /// </summary>
    public required Variable Parameter { get; set; }

    /// <summary>
    /// <c>E</c> represents the function body of this <see cref="Lambda"/>.
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
    /// This method creates a string representation of this <see cref="Lambda"/>.
    /// </summary>
    /// <returns>A string in the format "({Parameter} => {E})".</returns>
    public override string ToString() => $"({Parameter} => {E})";
}
