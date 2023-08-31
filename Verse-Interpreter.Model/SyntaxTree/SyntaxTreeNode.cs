//--------------------------------------------------------------
// <copyright file=".cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the  class.</summary>
//--------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree.Expressions;

namespace Verse_Interpreter.Model.SyntaxTree;

public abstract class SyntaxTreeNode
{
    public abstract SyntaxTreeNode DeepCopy();

    public abstract SyntaxTreeNode DeepCopyButReplaceChoice(Choice choice, Expression newExpression);
}
