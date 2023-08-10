using Antlr4.Runtime;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model;

public class VerseInterpreter
{
    private readonly IRenderer _renderer;
    private readonly IVerseSyntaxTreeBuilder _syntaxTreeBuilder;
    private readonly IRewriter _rewriter;

    public VerseInterpreter(IRenderer renderer, IVerseSyntaxTreeBuilder syntaxTreeBuilder, IRewriter rewriter)
        => (_renderer, _syntaxTreeBuilder, _rewriter) = (renderer, syntaxTreeBuilder, rewriter);

    public Expression Interpret(string verseCode)
    {
        _renderer.DisplayMessage("Getting character stream...");
        ICharStream stream = CharStreams.fromString(verseCode);
        _renderer.DisplayMessage("Creating lexer...");
        ITokenSource lexer = new VerseLexer(stream);
        _renderer.DisplayMessage("Constructing common token stream...");
        ITokenStream tokens = new CommonTokenStream(lexer);
        _renderer.DisplayMessage("Constructing parse tree...");
        VerseParser parser = new(tokens) { BuildParseTree = true };

        VerseParser.ProgramContext programContext = parser.program();
        _renderer.DisplayMessage("Converting and desugaring parse tree...");
        VerseProgram verseProgram = _syntaxTreeBuilder.BuildCustomSyntaxTreeWrappedInOne(programContext);
        IEnumerable<Variable> freeVariablesOfE = _rewriter.FreeVariablesOf(verseProgram.Wrapper.E);

        if (freeVariablesOfE.Count() is not 0)
        {
            _renderer.DisplayMessage("Invalid verse program, free variables must be zero.");
            return verseProgram.Wrapper;
        }

        _renderer.DisplayMessage(verseProgram.ToString());
        _renderer.DisplayMessage("Rewriting parse tree...");
        Expression result = _rewriter.Rewrite(verseProgram);
        _renderer.DisplayResult(result);
        return result;
    }
}
