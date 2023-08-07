using Verse_Interpreter.Model;
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
        System.Console.Write("\nResult is: ");

        System.Console.ForegroundColor = ConsoleColor.Blue;
        System.Console.WriteLine(expression);

        System.Console.ResetColor();
    }
}
