//-----------------------------------------------------------
// <copyright file="Wrapper.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Wrapper class.</summary>
//-----------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

/// <summary>
/// Class <see cref="Wrapper"/> serves as a base class for wrappers like <see cref="One"/> and <see cref="All"/> in the Verse syntax.
/// </summary>
public abstract class Wrapper : Expression
{
    /// <summary>
    /// Property <c>E</c> represents the <see cref="Wrapper"/>'s child <see cref="Expression"/>.
    /// </summary>
    public required Expression E { get; set; }
}
