//-------------------------------------------------------------------------
// <copyright file="IExpressionOrEquation.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the IExpressionOrEquation interface.</summary>
//-------------------------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

/// <summary>
/// Interface <see cref="IExpressionOrEquation"/> serves as an interface for
/// <see cref="Expression"/> and <see cref="Equation"/> classes in the Verse syntax.
/// </summary>
public interface IExpressionOrEquation : ISyntaxTreeNodeVisitable
{
}
