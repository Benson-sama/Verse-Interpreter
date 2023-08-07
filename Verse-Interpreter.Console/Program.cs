using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Verse_Interpreter.Console;
using Verse_Interpreter.Model;

Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine("""
     _   _                      _____      _                           _              __   _____ 
    | | | |                    |_   _|    | |                         | |            /  | |  _  |
    | | | | ___ _ __ ___  ___    | | _ __ | |_ ___ _ __ _ __  _ __ ___| |_ ___ _ __  `| | | |/' |
    | | | |/ _ \ '__/ __|/ _ \   | || '_ \| __/ _ \ '__| '_ \| '__/ _ \ __/ _ \ '__|  | | |  /| |
    \ \_/ /  __/ |  \__ \  __/  _| || | | | ||  __/ |  | |_) | | |  __/ ||  __/ |    _| |_\ |_/ /
     \___/ \___|_|  |___/\___|  \___/_| |_|\__\___|_|  | .__/|_|  \___|\__\___|_|    \___(_)___/ 
                                                       | |                                       
                                                       |_|                                       
    """);
Console.ResetColor();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IRenderer, ConsoleRenderer>();
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

    verseInterpreter.Interpret(verseCode);
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
