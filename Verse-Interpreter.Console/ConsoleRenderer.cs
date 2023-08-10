using Verse_Interpreter.Model;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;

namespace Verse_Interpreter.Console;

public class ConsoleRenderer : IRenderer
{
    public void DisplayMessage(string message)
        => System.Console.WriteLine(message);

    public void DisplayRuleApplied(string message)
        => System.Console.Write($"[~{message}] ");

    public void DisplayResult(Expression expression)
    {
        System.Console.Write("\n\nResult: ");
        WriteMessageInColor(expression.ToString(), ConsoleColor.Blue);
    }

    public void DisplayParsedProgram(VerseProgram verseProgram)
    {
        System.Console.Write("\nVerse program: ");
        WriteMessageInColor(verseProgram.ToString(), ConsoleColor.Red);
    }

    private static void WriteMessageInColor(string? message, ConsoleColor consoleColor)
    {
        System.Console.ForegroundColor = ConsoleColor.Blue;
        System.Console.WriteLine(message);

        System.Console.ResetColor();
    }
}
