//-------------------------------------------------------------------
// <copyright file="ConsoleRenderer.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ConsoleRenderer class.</summary>
//-------------------------------------------------------------------

using Verse_Interpreter.Model;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;

namespace Verse_Interpreter.Console;

/// <summary>
/// 
/// </summary>
public class ConsoleRenderer : IRenderer
{
    /// <summary>
    /// 
    /// </summary>
    private string? _intermediateResultCache;

    /// <summary>
    /// 
    /// </summary>
    public RenderMode Mode { get; set; } = RenderMode.Default;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public void DisplayMessage(string message)
        => System.Console.WriteLine(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public void DisplayRuleApplied(string message)
    {
        if (Mode == RenderMode.Silent)
            return;

        System.Console.Write($"[~{message}]{(Mode == RenderMode.Debug ? "\t" : "")}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expression"></param>
    public void DisplayResult(Expression expression)
    {
        if (Mode != RenderMode.Silent)
            System.Console.Write("\n\n");

        System.Console.Write("Result: ");
        WriteMessageInColor(expression.ToString(), ConsoleColor.Green);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expression"></param>
    public void DisplayIntermediateResult(Expression expression)
    {
        if (Mode != RenderMode.Debug)
            return;

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="verseProgram"></param>
    public void DisplayParsedProgram(VerseProgram verseProgram)
    {
        System.Console.Write("\nVerse program: ");
        WriteMessageInColor(verseProgram.ToString(), ConsoleColor.Blue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="consoleColor"></param>
    private static void WriteMessageInColor(string? message, ConsoleColor consoleColor)
    {
        ConsoleColor previousForegroundColor = System.Console.ForegroundColor;

        System.Console.ForegroundColor = consoleColor;
        System.Console.WriteLine(message);

        System.Console.ForegroundColor = previousForegroundColor;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="header"></param>
    public static void DisplayHeader(string header)
        => WriteMessageInColor(header, ConsoleColor.Blue);
}
