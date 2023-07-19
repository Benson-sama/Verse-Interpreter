using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Verse_Interpreter.Model;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;
using Tuple = Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Tuple;

Console.WriteLine("-- Verse-Interpreter Console --");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddLogging();
        services.AddSingleton<IVerseSyntaxTreeBuilder, VerseSyntaxTreeBuilder>();
        services.AddSingleton<VerseInterpreter>();
    })
    .Build();

VerseInterpreter verseInterpreter = host.Services.GetRequiredService<VerseInterpreter>();
string input = File.OpenText(@"C:\Users\User\Documents\GitHub\Verse-Interpreter\Input.verse")
                   .ReadToEnd();
VerseProgram verseProgram = verseInterpreter.GenerateParseTreeFromString(input);

Variable x = new(nameof(x));
Variable y = new(nameof(y));

VerseProgram customVerseProgram = new()
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
                        E = new Tuple()
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
