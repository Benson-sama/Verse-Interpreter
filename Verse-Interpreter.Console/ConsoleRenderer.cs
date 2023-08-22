using Verse_Interpreter.Model;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;

namespace Verse_Interpreter.Console;

public class ConsoleRenderer : IRenderer
{
    private string? _intermediateResultCache;

    public void DisplayMessage(string message)
        => System.Console.WriteLine(message);

    public void DisplayRuleApplied(string message)
        => System.Console.Write($"[~{message}]\t");

    public void DisplayResult(Expression expression)
    {
        System.Console.Write("\n\nResult: ");
        WriteMessageInColor(expression.ToString(), ConsoleColor.Green);
    }

    public void DisplayIntermediateResult(Expression expression)
    {
        if (expression.ToString() is not string resultText)
            return;

        ConsoleColor previousForegroundColor = System.Console.ForegroundColor;
        System.Console.ForegroundColor = ConsoleColor.Yellow;

        int i = 0;
        bool isDiffReached = false;
        foreach (char c in resultText)
        {
            if (!isDiffReached && (_intermediateResultCache is null || i >= _intermediateResultCache.Length || _intermediateResultCache[i++] != c))
            {
                System.Console.ForegroundColor = previousForegroundColor;
                isDiffReached = true;
            }

            System.Console.Write(c);
        }

        System.Console.ForegroundColor = previousForegroundColor;
        System.Console.WriteLine();

        _intermediateResultCache = resultText;
    }

    public void DisplayParsedProgram(VerseProgram verseProgram)
    {
        System.Console.Write("\nVerse program: ");
        WriteMessageInColor(verseProgram.ToString(), ConsoleColor.Blue);
    }

    private static void WriteMessageInColor(string? message, ConsoleColor consoleColor)
    {
        ConsoleColor previousForegroundColor = System.Console.ForegroundColor;

        System.Console.ForegroundColor = consoleColor;
        System.Console.WriteLine(message);

        System.Console.ForegroundColor = previousForegroundColor;
    }

    public static void DisplayHeader(string header)
        => WriteMessageInColor(header, ConsoleColor.Blue);
}
