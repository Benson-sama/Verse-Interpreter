//--------------------------------------------------------------------
// <copyright file="ExistsEliminator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ExistsEliminator class.</summary>
//--------------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

/// <summary>
/// Class <see cref="ExistsEliminator"/> serves as an <see cref="ISyntaxTreeNodeVisitor"/>
/// that replaces a specific <see cref="Exists"/> with its child <see cref="Expression"/>.
/// Its search pattern when eliminating is based on an "Execution Context",
/// therefore leaving many <c>Visit</c> methods empty.
/// </summary>
public class ExistsEliminator : ISyntaxTreeNodeVisitor
{
    /// <summary>
    /// Field <c>_targetExists</c> represents the <see cref="Exists"/> instance that should be eliminated.
    /// </summary>
    private readonly Exists _targetExists;

    /// <summary>
    /// Initialises a new instance of the <see cref="ExistsEliminator"/> class.
    /// </summary>
    /// <param name="targetExists"><c>targetExists</c> represents the <see cref="Exists"/> instance that should be eliminated.</param>
    public ExistsEliminator(Exists targetExists) => _targetExists = targetExists;

    /// <summary>
    /// Property <c>TargetExists</c> represents the <see cref="Exists"/> instance that should be eliminated.
    /// </summary>
    public Exists TargetExists => _targetExists;

    /// <summary>
    /// This method eliminates the <c>TargetExists</c> in the specified <paramref name="expression"/>.
    /// </summary>
    /// <param name="expression"><c>expression</c> is the <see cref="Expression"/> where the <c>TargetExists</c> should be eliminated.</param>
    public void EliminateExistsIn(Expression expression) => expression.Accept(this);

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Variable _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Integer _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(VerseString _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Operator _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(VerseTuple _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Lambda _) { }

    /// <summary>
    /// This method tries to eliminate the <c>TargetExists</c> in the <paramref name="equation"/>,
    /// or keep searching if it was not possible.
    /// </summary>
    /// <param name="equation"><c>equation</c> represents the <see cref="Equation"/> where elimination happens.</param>
    public void Visit(Equation equation)
    {
        if (equation.E == TargetExists)
            equation.E = TargetExists.E;
        else
            equation.E.Accept(this);
    }

    /// <summary>
    /// This method tries to eliminate the <c>TargetExists</c> in the <paramref name="eqe"/>,
    /// or keep searching if it was not possible.
    /// </summary>
    /// <param name="eqe"><c>eqe</c> represents the <see cref="Eqe"/> where elimination happens.</param>
    public void Visit(Eqe eqe)
    {
        if (eqe.Eq == TargetExists)
            eqe.Eq = TargetExists.E;
        else
            eqe.Eq.Accept(this);

        if (eqe.E == TargetExists)
            eqe.E = TargetExists.E;
        else
            eqe.E.Accept(this);
    }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Exists _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Fail _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Choice _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Application _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(One _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(All _) { }
}
