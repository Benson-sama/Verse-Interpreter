//---------------------------------------------------------------------------
// <copyright file="IVerseSyntaxTreeBuilder.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the IVerseSyntaxTreeBuilder class.</summary>
//---------------------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Build;

/// <summary>
/// Interace <see cref="IVerseSyntaxTreeBuilder"/> serves as an interface for classes that convert
/// ANTLR4 syntax trees to custom syntax trees.
/// </summary>
public interface IVerseSyntaxTreeBuilder
{
    /// <summary>
    /// Builds a custom syntax tree based on the given ANTLR4 syntax tree <paramref name="context"/>.
    /// Uses the <paramref name="wrapperFactory"/> to wrap the built <see cref="Expression"/>, which then
    /// gets placed inside the <see cref="VerseProgram"/>.
    /// </summary>
    /// <param name="context"><c>context</c> is the ANTLR4 syntax tree.</param>
    /// <param name="wrapperFactory"><c>wrapperFactory</c> is the factory that provides the <see cref="Wrapper"/>.</param>
    /// <returns>The built custom syntax tree as a <see cref="VerseProgram"/>.</returns>
    VerseProgram BuildCustomSyntaxTree(VerseParser.ProgramContext context, Func<Expression, Wrapper> wrapperFactory);
}
