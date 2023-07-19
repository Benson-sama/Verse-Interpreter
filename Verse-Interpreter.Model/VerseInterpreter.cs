using Antlr4.Runtime;
using Microsoft.Extensions.Logging;
using Verse_Interpreter.Model.SyntaxTree;

namespace Verse_Interpreter.Model;

public class VerseInterpreter
{
    private readonly ILogger _logger;
    private readonly IVerseSyntaxTreeBuilder _syntaxTreeBuilder;

    public VerseInterpreter(ILogger<VerseInterpreter> logger, IVerseSyntaxTreeBuilder syntaxTreeBuilder)
    {
        _logger = logger;
        _syntaxTreeBuilder = syntaxTreeBuilder;
    }

    public VerseProgram GenerateParseTreeFromString(string input)
    {
        _logger.LogInformation("Getting input stream...");
        ICharStream stream = CharStreams.fromString(input);
        _logger.LogInformation("Starting lexer...");
        ITokenSource lexer = new VerseLexer(stream);
        _logger.LogInformation("Getting token stream...");
        ITokenStream tokens = new CommonTokenStream(lexer);
        _logger.LogInformation("Starting parser...");
        VerseParser parser = new(tokens)
        {
            BuildParseTree = true
        };

        _logger.LogInformation("Finished generating parse tree. Converting and desugaring...");
        VerseParser.ProgramContext programContext = parser.program();
        return _syntaxTreeBuilder.BuildCustomSyntaxTreeWrappedInOne(programContext);
    }
}
