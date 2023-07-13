using Verse_Interpreter.Model;

/*
using Antlr4.Runtime;

string input = File.OpenText(@"C:\Users\User\Documents\GitHub\Verse-Interpreter\Input.verse").ReadToEnd();
ICharStream stream = CharStreams.fromString(input);
ITokenSource lexer = new VerseLexer(stream);
ITokenStream tokens = new CommonTokenStream(lexer);
VerseParser parser = new(tokens)
{
    BuildParseTree = true
};

var tree = parser.program();

Console.WriteLine(tree.ToStringTree());
*/

Console.WriteLine("-- Verse-Interpreter Console --");

Node integer = new Integer();
Node value = new Value();

Console.WriteLine(integer is Value);
Console.WriteLine(value is Value);
