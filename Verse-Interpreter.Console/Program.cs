using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Verse_Interpreter.Model;

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
string verseCode = File.OpenText(@"C:\Users\User\source\repos\Benson-sama\Verse-Interpreter\Input.verse")
                       .ReadToEnd();
verseInterpreter.Interpret(verseCode);

Console.WriteLine("Done.");
