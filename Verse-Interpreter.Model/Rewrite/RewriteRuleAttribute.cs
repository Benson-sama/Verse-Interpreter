//------------------------------------------------------------------------
// <copyright file="RewriteRuleAttribute.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the RewriteRuleAttribute class.</summary>
//------------------------------------------------------------------------

namespace Verse_Interpreter.Model.Rewrite;

/// <summary>
/// Class <see cref="RewriteRuleAttribute"/> serves as an <see cref="Attribute"/> for
/// marking methods as rewrite rules.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RewriteRuleAttribute : Attribute
{
}
