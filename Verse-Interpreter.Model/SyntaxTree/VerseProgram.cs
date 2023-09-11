//----------------------------------------------------------------
// <copyright file="VerseProgram.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the VerseProgram interface.</summary>
//----------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree.Expressions;

namespace Verse_Interpreter.Model.SyntaxTree;

/// <summary>
/// Class <see cref="VerseProgram"/> serves as a root node for Verse programs.
/// </summary>
public class VerseProgram : SyntaxTreeNode
{
    /// <summary>
    /// Field <c>E</c> represents the child <see cref="Expression"/>.
    /// </summary>
    public required Expression E { get; set; }

    /// <summary>
    /// This method creates a string representation of this <see cref="VerseProgram"/>.
    /// </summary>
    /// <returns>A string in the format "Program{E}".</returns>
    public override string ToString() => $"Program{E}";
}
