//-----------------------------------------------------------
// <copyright file="Program.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Program class.</summary>
//-----------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Verse_Interpreter.Console.Render;
using Verse_Interpreter.Model;
using Verse_Interpreter.Model.Build;
using Verse_Interpreter.Model.Render;
using Verse_Interpreter.Model.Rewrite;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;
using Verse_Interpreter.Model.Visitor;

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
ConsoleRenderer consoleRenderer = new();
ConsoleRenderer.DisplayHeader(header);

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IVariableFactory, VariableFactory>();
        services.AddSingleton<IRenderer>(consoleRenderer);
        services.AddSingleton<IVerseSyntaxTreeBuilder, VerseSyntaxTreeBuilder>();
        services.AddSingleton<SyntaxDesugarer>();
        services.AddSingleton<IRewriter, Rewriter>();
        services.AddSingleton<VerseInterpreter>();
    })
    .Build();

VerseInterpreter verseInterpreter = host.Services.GetRequiredService<VerseInterpreter>();
GetRequestedCommand(args)();

Action GetRequestedCommand(string[] args)
{
    if (args.Length < 3)
        return ExecuteInvalidArgumentsCommand;

    string rendererMode = args[0];
    string mode = args[1];
    string command = args[2];

    consoleRenderer.Mode = rendererMode.ToLower() switch
    {
        "-default" => RenderMode.Default,
        "-silent" => RenderMode.Silent,
        "-debug" => RenderMode.Debug,
        _ => RenderMode.Default
    };

    Func<Expression, Wrapper>? wrapperFactory = mode.ToLower() switch
    {
        "-one" => (e) => new One() { E = e },
        "-all" => (e) => new All() { E = e },
        _ => null
    };

    if (wrapperFactory is null)
        return ExecuteInvalidArgumentsCommand;

    return command.ToLower() switch
    {
        "-code" => () => ExecuteCodeCommand(args.ElementAtOrDefault(3), wrapperFactory),
        "-interactive" => () => ExecuteInteractiveCommand(wrapperFactory),
        "-file" => () => ExecuteFileCommand(args.ElementAtOrDefault(3), wrapperFactory),
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

    Interpret(verseCode, wrapperFactory);
}

void ExecuteInteractiveCommand(Func<Expression, Wrapper> wrapperFactory)
{
    Console.WriteLine("Please enter your Verse code:\n");
    string? verseCode = Console.ReadLine();

    if (verseCode is null)
        Console.WriteLine("No verse code entered.");
    else
        Interpret(verseCode, wrapperFactory);
}

void ExecuteFileCommand(string? filePath, Func<Expression, Wrapper> wrapperFactory)
{
    using StreamReader sr = File.OpenText(filePath ?? GetFilePathFromUser());
    string verseCode = sr.ReadToEnd();
    Interpret(verseCode, wrapperFactory);
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

          {renderMode} {resultMode} {command}

          {renderMode}: -default | -silent | -debug
          {resultMode}: -one | -all
          {command}: -code {code} | -interactive | -file {filePath}
        """);
    Environment.Exit(1);
}

void Interpret(string verseCode, Func<Expression, Wrapper> wrapperFactory)
{
    try
    {
        verseInterpreter.Interpret(verseCode, wrapperFactory);
    }
    catch (VerseParseException e)
    {
        consoleRenderer.DisplayMessage(e.Message);
    }
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
