//-------------------------------------------------------------------
// <copyright file="VerseInterpreter.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the VerseInterpreter class.</summary>
//-------------------------------------------------------------------

using System.Diagnostics;
using Antlr4.Runtime;
using Verse_Interpreter.Model.Build;
using Verse_Interpreter.Model.Render;
using Verse_Interpreter.Model.Rewrite;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;
using Verse_Interpreter.Model.Visitor;

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
        VerseParser.ProgramContext programContext = ParseAntlrProgramContext(verseCode);
        VerseProgram verseProgram = ConvertAndDesugar(programContext, wrapperFactory);
        Expression result = Rewrite(verseProgram);

        return result;
    }

    /// <summary>
    /// This method uses the generated <see cref="VerseLexer"/> and <see cref="VerseParser"/> by ANTLR4
    /// to build the <see cref="VerseParser.ProgramContext"/>.
    /// </summary>
    /// <param name="verseCode"><c>verseCode</c> represents the Verse code to parse with ANTLR4.</param>
    /// <returns>The parsed <see cref="VerseParser.ProgramContext"/>.</returns>
    /// <exception cref="VerseParseException">Is raised when the number of syntax errors while parsing is not zero.</exception>
    private VerseParser.ProgramContext ParseAntlrProgramContext(string verseCode)
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

        if (parser.NumberOfSyntaxErrors != 0)
            throw new VerseParseException("Number of syntax errors in ANTLR4 is not zero.");

        return programContext;
    }

    /// <summary>
    /// This method converts and desugars the <paramref name="programContext"/> using the <c>_syntaxTreeBuilder</c>
    /// and the <paramref name="wrapperFactory"/>. Displays the parsed program when done.
    /// </summary>
    /// <param name="programContext"><c>programContext</c> represents the original ANTLR4 parse tree to convert and desugar.</param>
    /// <param name="wrapperFactory">
    /// <c>wrapperFactory</c> represents the factory used to
    /// place the converted and desugared <see cref="Expression"/> in.
    /// </param>
    /// <returns>The resulting <see cref="VerseProgram"/> of the conversion and desugaring.</returns>
    /// <exception cref="VerseParseException">
    /// Is raised when the free variables of the parsed <see cref="VerseProgram"/> are not zero.
    /// </exception>
    private VerseProgram ConvertAndDesugar(VerseParser.ProgramContext programContext, Func<Expression, Wrapper> wrapperFactory)
    {

        _renderer.DisplayMessage("Converting and desugaring parse tree...");
        VerseProgram verseProgram = _syntaxTreeBuilder.BuildCustomSyntaxTree(programContext, wrapperFactory);
        VariablesAnalyser variablesAnalyser = new();

        if (variablesAnalyser.FreeVariablesOf(verseProgram.E).Any())
            throw new VerseParseException("Invalid verse program, free variables must be zero.");

        _renderer.DisplayParsedProgram(verseProgram);

        return verseProgram;
    }

    /// <summary>
    /// This method rewrites the <paramref name="verseProgram"/> using the <c>_rewriter</c> while measuring the
    /// time using a <see cref="Stopwatch"/>. Displays the result when done.
    /// </summary>
    /// <param name="verseProgram"><c>verseProgram</c> represents the <see cref="VerseProgram"/> to rewrite.</param>
    /// <returns>The result of rewriting the <paramref name="verseProgram"/>.</returns>
    private Expression Rewrite(VerseProgram verseProgram)
    {
        _renderer.DisplayMessage("\nRewriting parse tree...\n");
        Stopwatch stopwatch = Stopwatch.StartNew();
        Expression result = _rewriter.Rewrite(verseProgram);
        stopwatch.Stop();
        _renderer.DisplayResult(result, stopwatch.Elapsed);

        return result;
    }
}
