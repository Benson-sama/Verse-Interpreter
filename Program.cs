using Antlr4.Runtime;

Console.WriteLine("Hello World!");

string input = "42 + 69";
ICharStream stream = CharStreams.fromString(input);
ITokenSource lexer = new VerseLexer(stream);
ITokenStream tokens = new CommonTokenStream(lexer);
VerseParser parser = new(tokens)
{
    BuildParseTree = true
};

var tree = parser.expression();

Console.WriteLine(tree.ToStringTree());
