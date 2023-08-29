//-------------------------------------------------------------------
// <copyright file="RenderMode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the RenderMode enum.</summary>
//-------------------------------------------------------------------

namespace Verse_Interpreter.Console.Renderer;

/// <summary>
/// Enum <see cref="RenderMode"/> specifies constants that define different rendering modes.
/// </summary>
public enum RenderMode
{
    /// <summary>
    /// The default rendering mode.
    /// </summary>
    Default,

    /// <summary>
    /// The silent rendering mode used to provide less rendering.
    /// </summary>
    Silent,

    /// <summary>
    /// The debug rendering mode used to provide additional rendering.
    /// </summary>
    Debug
}
