using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Verse_Interpreter.Console;
using Verse_Interpreter.Model;
using Verse_Interpreter.Model.SyntaxTree.Expressions;

Console.WriteLine("-- Verse-Interpreter Console --");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IVerseLogger, ConsoleVerseLogger>();
        services.AddSingleton<IVerseSyntaxTreeBuilder, VerseSyntaxTreeBuilder>();
        services.AddSingleton<IRewriter, Rewriter>();
        services.AddSingleton<VerseInterpreter>();
    })
    .Build();

VerseInterpreter verseInterpreter = host.Services.GetRequiredService<VerseInterpreter>();
GetRequestedCommand(args)();

Action GetRequestedCommand(string[] args)
{
    return args.FirstOrDefault() switch
    {
        "-code" => () => ExecuteCodeCommand(args.ElementAtOrDefault(1)),
        "-interactive" => ExecuteInteractiveCommand,
        "-file" => ExecuteFileCommand,
        { } => ExecuteUnknownCommand,
        _ => ExecuteMissingCommand
    };
}

void ExecuteCodeCommand(string? verseCode)
{
    if (verseCode is null)
    {
        Console.WriteLine("Supplied code is invalid.");
        return;
    }

    Expression result = verseInterpreter.Interpret(verseCode);
    Console.WriteLine("\n");
    Console.WriteLine("Result: " + result);
}

void ExecuteInteractiveCommand()
{
    Console.WriteLine("Please enter your Verse code:\n");
    string? verseCode = Console.ReadLine();

    if (verseCode is null)
        Console.WriteLine("No verse code entered.");
    else
        verseInterpreter.Interpret(verseCode);
}

void ExecuteFileCommand()
{
    string filePath = GetFilePathFromUser();
    using StreamReader sr = File.OpenText(filePath);
    string verseCode = sr.ReadToEnd();
    verseInterpreter.Interpret(verseCode);
}

void ExecuteUnknownCommand()
    => Console.WriteLine("Unknown command specified.");

void ExecuteMissingCommand()
{
    Console.WriteLine("""
        No command specified. Possible commands are:

          -code {code}
          -interactive
          -file {filePath}
        
        """);
}

static string GetFilePathFromUser()
{
    string? path;

    do
    {
        Console.WriteLine("Please enter the path of the file containing the verse code.");
        path = Console.ReadLine();
    } while (!File.Exists(path));

    return path;
}
