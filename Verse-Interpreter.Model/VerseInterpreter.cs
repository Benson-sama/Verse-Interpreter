//-------------------------------------------------------------------
// <copyright file="VerseInterpreter.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the VerseInterpreter class.</summary>
//-------------------------------------------------------------------

using System.Diagnostics;
using Antlr4.Runtime;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model;

/// <summary>
/// Class <see cref="VerseInterpreter"/> serves as an interpreter for the
/// functional logic programming language Verse proposed by Epic Games in 2023.
/// </summary>
public class VerseInterpreter
{
    /// <summary>
    /// Field <c>_renderer</c> represents the rendering component used to visualise output.
    /// </summary>
    private readonly IRenderer _renderer;

    /// <summary>
    /// Field <c>_syntaxTreeBuilder</c> represents the syntax tree building component.
    /// It handles conversion and desugaring from ANTLR4 parse trees to a custom datatype tree.
    /// </summary>
    private readonly IVerseSyntaxTreeBuilder _syntaxTreeBuilder;

    /// <summary>
    /// Field <c>_rewriter</c> represents the rewriter component used to handle the "execution" of the Verse program.
    /// </summary>
    private readonly IRewriter _rewriter;

    /// <summary>
    /// This constructor initialises the new <see cref="VerseInterpreter"/> with the given components.
    /// </summary>
    /// <param name="renderer"><c>renderer</c> is the new VerseInterpreter's rendering component.</param>
    /// <param name="syntaxTreeBuilder"><c>syntaxTreeBuilder</c> is the new VerseInterpreter's syntax tree building component.</param>
    /// <param name="rewriter"><c>rewriter</c> is the new VerseInterpreter's rewriting component.</param>
    public VerseInterpreter(IRenderer renderer, IVerseSyntaxTreeBuilder syntaxTreeBuilder, IRewriter rewriter)
        => (_renderer, _syntaxTreeBuilder, _rewriter) = (renderer, syntaxTreeBuilder, rewriter);

    /// <summary>
    /// This method interprets the <paramref name="verseCode"/> using the <paramref name="wrapperFactory"/>.
    /// First the ANTLR4 parse tree gets created, converted and desugared.
    /// The parsed expression is placed in the wrapper from the factory and that itself inside the program.
    /// Additionaly a <see cref="Stopwatch"/> provides time metrics for the rewriting.
    /// </summary>
    /// <param name="verseCode"><c>verseCode</c> is the Verse code to interpret.</param>
    /// <param name="wrapperFactory"><c>wrapperFactory</c> is the factory from which the wrapper is retrieved.</param>
    /// <returns>
    /// <see cref="Fail"/> if either the number of syntax tree errors from ANTLR4 or the free variables of the program are not zero.
    /// Otherwise the rewritten <see cref="Expression"/> with as many rewrite rules applied as possible.
    /// </returns>
    public Expression Interpret(string verseCode, Func<Expression, Wrapper> wrapperFactory)
    {
        _renderer.DisplayMessage($"Input was:\n{verseCode}\n");
        _renderer.DisplayMessage("Getting character stream...");
        ICharStream stream = CharStreams.fromString(verseCode);
        _renderer.DisplayMessage("Creating lexer...");
        ITokenSource lexer = new VerseLexer(stream);
        _renderer.DisplayMessage("Constructing common token stream...");
        ITokenStream tokens = new CommonTokenStream(lexer);
        _renderer.DisplayMessage("Constructing parse tree...");
        VerseParser parser = new(tokens) { BuildParseTree = true };
        VerseParser.ProgramContext programContext = parser.program();
        
        if (parser.NumberOfSyntaxErrors is not 0)
        {
            _renderer.DisplayMessage("Unable to parse verse program.");

            return new Fail();
        }

        _renderer.DisplayMessage("Converting and desugaring parse tree...");
        VerseProgram verseProgram = _syntaxTreeBuilder.BuildCustomSyntaxTree(programContext, wrapperFactory);

        if (FreeVariables.Of(verseProgram.E).Count() is not 0)
        {
            _renderer.DisplayMessage("Invalid verse program, free variables must be zero.");

            return new Fail();
        }

        _renderer.DisplayParsedProgram(verseProgram);
        _renderer.DisplayMessage("\nRewriting parse tree...");
        Stopwatch stopwatch = Stopwatch.StartNew();
        Expression result = _rewriter.Rewrite(verseProgram);
        stopwatch.Stop();
        _renderer.DisplayMessage($"Finished rewriting in {stopwatch.Elapsed.TotalSeconds} seconds.\n");
        _renderer.DisplayResult(result);

        return result;
    }
}
