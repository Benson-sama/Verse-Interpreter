//------------------------------------------------------------
// <copyright file="Operator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Operator class.</summary>
//------------------------------------------------------------

namespace Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;

/// <summary>
/// Class <see cref="Operator"/> serves as a base class for operators in the Verse syntax.
/// </summary>
public abstract class Operator : HeadNormalForm
{
    /// <summary>
    /// This method creates a deep copy of this <see cref="Operator"/> to avoid shared references.
    /// </summary>
    /// <returns>The new <see cref="Operator"/> as a deep copy of this instance.</returns>
    public abstract override Operator DeepCopy();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="choice"></param>
    /// <param name="newExpression"></param>
    /// <returns></returns>
    public abstract override Operator DeepCopyButReplaceChoice(Choice choice, Expression newExpression);

    public override void Accept(ISyntaxTreeNodeVisitor visitor)
        => visitor.Visit(this);

    public override T Accept<T>(ISyntaxTreeNodeVisitor<T> visitor)
        => visitor.Visit(this);
}
