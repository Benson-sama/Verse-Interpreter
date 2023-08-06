using System;
using System.Collections.Generic;
using System.Linq;
using Verse_Interpreter.Model;

namespace Verse_Interpreter.Console;

public class ConsoleVerseLogger : IVerseLogger
{
    public void Log(string message)
        => System.Console.WriteLine(message);

    public void LogRuleApplied(string message)
        => System.Console.Write($"[~{message}] ");
}
