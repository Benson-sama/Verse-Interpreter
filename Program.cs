using System.Net.WebSockets;
using Antlr4.Runtime;

namespace Verse_Interpreter;

public class Program
{
    static void Main(string[] args)
    {
        _ = args;
        Console.WriteLine("Hello World!");

        string input = "42 + 69";
        ICharStream stream = CharStreams.fromString(input);
        ITokenSource lexer = new VerseLexer(stream);
        ITokenStream tokens = new CommonTokenStream(lexer);
        VerseParser parser = new(tokens)
        {
            BuildParseTree = true
        };

        var tree = parser.operation();

        Console.WriteLine(tree.ToStringTree());
    }
}
