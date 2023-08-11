using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Verse_Interpreter.Console;
using Verse_Interpreter.Model;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

ConsoleRenderer renderer = new();
string header = """
     _   _                      _____      _                           _            
    | | | |                    |_   _|    | |                         | |           
    | | | | ___ _ __ ___  ___    | | _ __ | |_ ___ _ __ _ __  _ __ ___| |_ ___ _ __ 
    | | | |/ _ \ '__/ __|/ _ \   | || '_ \| __/ _ \ '__| '_ \| '__/ _ \ __/ _ \ '__|
    \ \_/ /  __/ |  \__ \  __/  _| || | | | ||  __/ |  | |_) | | |  __/ ||  __/ |   
     \___/ \___|_|  |___/\___|  \___/_| |_|\__\___|_|  | .__/|_|  \___|\__\___|_|   
                                                       | |                                       
    ---------------------------------------------------|_|--------------------------                                       
    """;
renderer.DisplayHeader(header);

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IVariableFactory, VariableFactory>();
        services.AddSingleton<IRenderer>(renderer);
        services.AddSingleton<IVerseSyntaxTreeBuilder, VerseSyntaxTreeBuilder>();
        services.AddSingleton<Desugar>();
        services.AddSingleton<IRewriter, Rewriter>();
        services.AddSingleton<VerseInterpreter>();
    })
    .Build();

VerseInterpreter verseInterpreter = host.Services.GetRequiredService<VerseInterpreter>();
GetRequestedCommand(args)();

Action GetRequestedCommand(string[] args)
{
    if (args.Length < 2)
        return ExecuteInvalidArgumentsCommand;

    string mode = args[0];
    string command = args[1];

    Func<Expression, Wrapper>? wrapperFactory = mode switch
    {
        "-one" => (e) => new One() { E = e },
        "-all" => (e) => new All() { E = e },
        _ => null
    };

    if (wrapperFactory is null)
        return ExecuteInvalidArgumentsCommand;

    return command switch
    {
        "-code" => () => ExecuteCodeCommand(args.ElementAtOrDefault(2), wrapperFactory),
        "-interactive" => () => ExecuteInteractiveCommand(wrapperFactory),
        "-file" => () => ExecuteFileCommand(args.ElementAtOrDefault(2), wrapperFactory),
        { } => ExecuteUnknownCommand,
        _ => ExecuteInvalidArgumentsCommand
    };
}

void ExecuteCodeCommand(string? verseCode, Func<Expression, Wrapper> wrapperFactory)
{
    if (verseCode is null)
    {
        Console.WriteLine("Supplied code is invalid.");
        return;
    }

    verseInterpreter.Interpret(verseCode, wrapperFactory);
}

void ExecuteInteractiveCommand(Func<Expression, Wrapper> wrapperFactory)
{
    Console.WriteLine("Please enter your Verse code:\n");
    string? verseCode = Console.ReadLine();

    if (verseCode is null)
        Console.WriteLine("No verse code entered.");
    else
        verseInterpreter.Interpret(verseCode, wrapperFactory);
}

void ExecuteFileCommand(string? filePath, Func<Expression, Wrapper> wrapperFactory)
{
    using StreamReader sr = File.OpenText(filePath ?? GetFilePathFromUser());
    string verseCode = sr.ReadToEnd();
    verseInterpreter.Interpret(verseCode, wrapperFactory);
}

void ExecuteUnknownCommand()
{
    Console.WriteLine("Unknown command specified.");
    Environment.Exit(1);
}

void ExecuteInvalidArgumentsCommand()
{
    Console.WriteLine("""
        Command line arguments are not valid. The following format is required:

          {mode} {command}

          {mode}: -one | -all
          {command}: -code {code} | -interactive | -file {filePath}
        """);
    Environment.Exit(1);
}

static string GetFilePathFromUser()
{
    string? path;

    do
    {
        Console.WriteLine("Please enter the path of the file containing the verse code.");
        path = Console.ReadLine();
    } while (!File.Exists(path?.Trim('"')));

    return path;
}
