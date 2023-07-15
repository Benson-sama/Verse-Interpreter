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

Variable x = new(nameof(x));
Variable y = new(nameof(y));

Verse_Interpreter.Model.Program program = new()
{
    Wrapper = new One()
    {
        E = new Exists()
        {
            V = x,
            E = new Exists()
            {
                V = y,
                E = new Eqe()
                {
                    Eq = new Equation()
                    {
                        V = x,
                        E = new Choice()
                        {
                            E1 = new Integer(7),
                            E2 = new Integer(22)
                        }
                    },
                    E = new Eqe()
                    {
                        Eq = new Equation()
                        {
                            V = y,
                            E = new Choice()
                            {
                                E1 = new Integer(31),
                                E2 = new Integer(5)
                            }
                        },
                        E = new Verse_Interpreter.Model.Tuple()
                        {
                            Values = new Value[]
                            {
                                x,
                                y
                            }
                        }
                    }
                }
            }
        }
    }
};

Console.WriteLine("Done.");
