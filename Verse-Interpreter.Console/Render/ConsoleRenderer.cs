//-------------------------------------------------------------------
// <copyright file="ConsoleRenderer.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ConsoleRenderer class.</summary>
//-------------------------------------------------------------------

using Verse_Interpreter.Model.Render;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;

namespace Verse_Interpreter.Console.Render;

/// <summary>
/// Class <see cref="ConsoleRenderer"/> serves as an <see cref="IRenderer"/> for the console.
/// </summary>
public class ConsoleRenderer : IRenderer
{
    /// <summary>
    /// Field <c>_intermediateResultCache</c> serves as a cache for intermediate results.
    /// </summary>
    private string? _intermediateResultCache;

    /// <summary>
    /// Property <c>Mode</c> represents the <see cref="ConsoleRenderer"/>'s render mode.
    /// Default value is <see cref="RenderMode.Default"/>.
    /// </summary>
    public RenderMode Mode { get; set; } = RenderMode.Default;

    /// <summary>
    /// This method prints the <paramref name="header"/> to the console in blue foreground
    /// and current background colour without changing the <see cref="System.Console.ForegroundColor"/>
    /// and <see cref="System.Console.BackgroundColor"/>.
    /// </summary>
    /// <param name="header">The <c>header</c> to be displayed.</param>
    public static void DisplayHeader(string header)
    {
        WriteMessageInColour(
            header,
            foregroundColour: ConsoleColor.Blue,
            backgroundColour: System.Console.BackgroundColor);
    }

    /// <summary>
    /// This method simply uses <see cref="System.Console.WriteLine(string)"/> to display the <paramref name="message"/>.
    /// </summary>
    /// <param name="message"><c>message</c> represents the message to display.</param>
    public void DisplayMessage(string message)
        => System.Console.WriteLine(message);

    /// <summary>
    /// This method prints the string representation of the <c>verseProgram</c> to the console 
    /// in blue foreground colour and current background colour without changing the
    /// <see cref="System.Console.ForegroundColor"/> and <see cref="System.Console.BackgroundColor"/>.
    /// </summary>
    /// <param name="verseProgram"><c>verseProgram</c> represents the Verse program to display.</param>
    public void DisplayParsedProgram(VerseProgram verseProgram)
    {
        System.Console.Write("\nVerse program: ");
        WriteMessageInColour(
            verseProgram.ToString(),
            foregroundColour: ConsoleColor.Blue,
            backgroundColour: System.Console.BackgroundColor);
    }

    /// <summary>
    /// This method prints the <paramref name="appliedRule"/> in the format "[~<c>appliedRule</c>]" to the console
    /// if the <c>Mode</c> is not <see cref="RenderMode.Silent"/>.
    /// It also adds a tabulator spacing afterwards if the <c>Mode</c> is <see cref="RenderMode.Debug"/>.
    /// </summary>
    /// <param name="appliedRule"><c>appliedRule</c> represents the applied rule to display.</param>
    public void DisplayRuleApplied(string appliedRule)
    {
        if (Mode == RenderMode.Silent)
            return;

        System.Console.Write($"[~{appliedRule}]{(Mode == RenderMode.Debug ? "\t" : "")}");
    }

    /// <summary>
    /// This method prints the <paramref name="expression"/> to the console if the <c>Mode</c> is
    /// <see cref="RenderMode.Debug"/> and saves it in the <c>_intermediateResultCache</c> afterwards.
    /// It is using a yellow foreground colour for each character as long as the new <paramref name="expression"/>
    /// is equal to the one from the <c>_intermediateResultCache</c> to aid the reader in finding the changes.
    /// </summary>
    /// <param name="expression"><c>expression</c> represents the intermediate result to display.</param>
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
    /// This method prints the <paramref name="elapsedTime"/> and the <paramref name="expression"/>
    /// to the console in green foreground and current background colour without changing the
    /// <see cref="System.Console.ForegroundColor"/> and <see cref="System.Console.BackgroundColor"/>.
    /// </summary>
    /// <param name="expression"><c>expression</c> represents the result to display.</param>
    /// <param name="elapsedTime"><c>elapsedTime</c> represents the time it took to get the result.</param>
    public void DisplayResult(Expression expression, TimeSpan elapsedTime)
    {
        if (Mode == RenderMode.Debug)
            System.Console.WriteLine();

        DisplayMessage($"Finished rewriting in {elapsedTime.TotalSeconds} seconds.");

        System.Console.Write("\nResult: ");
        WriteMessageInColour(
            expression.ToString() ?? "null",
            foregroundColour: ConsoleColor.Green,
            backgroundColour: System.Console.BackgroundColor);
    }

    /// <summary>
    /// This method uses <see cref="System.Console.WriteLine(string)"/> to display the <paramref name="message"/>
    /// in the specified <paramref name="foregroundColour"/> and <paramref name="backgroundColour"/>.
    /// It resets the <see cref="System.Console.ForegroundColor"/> and <see cref="System.Console.BackgroundColor"/>
    /// to its previous values afterwards.
    /// </summary>
    /// <param name="message">The <c>message</c> to print.</param>
    /// <param name="foregroundColour">The <c>foregroundColour</c> for the <c>message</c>.</param>
    /// /// <param name="backgroundColour">The <c>backgroundColour</c> for the <c>message</c>.</param>
    private static void WriteMessageInColour(string message, ConsoleColor foregroundColour, ConsoleColor backgroundColour)
    {
        ConsoleColor previousForegroundColour = System.Console.ForegroundColor;
        ConsoleColor previousBackgroundColour = System.Console.BackgroundColor;

        System.Console.ForegroundColor = foregroundColour;
        System.Console.BackgroundColor = backgroundColour;
        System.Console.WriteLine(message);

        System.Console.ForegroundColor = previousForegroundColour;
        System.Console.BackgroundColor = previousBackgroundColour;
    }
}
