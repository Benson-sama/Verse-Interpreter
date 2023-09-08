//-------------------------------------------------------------
// <copyright file="IRewriter.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the IRewriter interface.</summary>
//-------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;

namespace Verse_Interpreter.Model.Rewrite;

/// <summary>
/// Interface <see cref="IRewriter"/> serves as an interface for classes that
/// handle the "execution" of a <see cref="VerseProgram"/> by rewriting it.
/// </summary>
public interface IRewriter
{
    /// <summary>
    /// This method rewrites the <paramref name="verseProgram"/> to a single <see cref="Expression"/>.
    /// </summary>
    /// <param name="verseProgram"><c>verseProgram</c> is the <see cref="VerseProgram"/> to rewrite.</param>
    /// <returns>The <see cref="Expression"/> representing the result of the <see cref="VerseProgram"/>.</returns>
    Expression Rewrite(VerseProgram verseProgram);
}
