using Antlr4.Runtime;

namespace Verse_Interpreter.Model;

public class VerseInterpreter
{
    public VerseParser.ProgramContext GenerateParseTreeFromString(string input)
    {
        ICharStream stream = CharStreams.fromString(input);
        ITokenSource lexer = new VerseLexer(stream);
        ITokenStream tokens = new CommonTokenStream(lexer);
        VerseParser parser = new(tokens)
        {
            BuildParseTree = true
        };

        return parser.program();
    }
}
