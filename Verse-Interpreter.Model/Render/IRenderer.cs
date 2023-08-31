//-------------------------------------------------------------
// <copyright file="IRenderer.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the IRenderer interface.</summary>
//-------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;

namespace Verse_Interpreter.Model.Render;

/// <summary>
/// Interface <see cref="IRenderer"/> serves as the render component for the <see cref="VerseInterpreter"/>.
/// </summary>
public interface IRenderer
{
    /// <summary>
    /// This method displays the <paramref name="message"/> to the user.
    /// </summary>
    /// <param name="message"><c>message</c> represents the message to display.</param>
    void DisplayMessage(string message);

    /// <summary>
    /// This method displays the parsed <see cref="VerseProgram"/> to the user.
    /// </summary>
    /// <param name="verseProgram"><c>verseProgram</c> represents the parsed program to display.</param>
    void DisplayParsedProgram(VerseProgram verseProgram);

    /// <summary>
    /// This method displays the <paramref name="appliedRule"/> to the user.
    /// </summary>
    /// <param name="appliedRule"><c>appliedRule</c> represents the rule to display.</param>
    void DisplayRuleApplied(string appliedRule);

    /// <summary>
    /// This method displays the <paramref name="expression"/> as an intermediate result to the user.
    /// </summary>
    /// <param name="expression"><c>expression</c> represents the intermediate result to display.</param>
    void DisplayIntermediateResult(Expression expression);

    /// <summary>
    /// This method displays the <paramref name="expression"/> as a final result to the user
    /// while also providing the <paramref name="elapsedTime"/> it took to get the result.
    /// </summary>
    /// <param name="expression"><c>expression</c> represents the final result to display.</param>
    /// <param name="elapsedTime"><c>elapsedTime</c> represents the time span it took to get the result.</param>
    void DisplayResult(Expression expression, TimeSpan elapsedTime);
}
