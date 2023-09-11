//----------------------------------------------------------------------
// <copyright file="EquationEliminator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the EquationEliminator class.</summary>
//----------------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

/// <summary>
/// Class <see cref="EquationEliminator"/> serves as an <see cref="ISyntaxTreeNodeVisitor"/>
/// that replaces an <see cref="Eqe"/> containing a specific <see cref="Equation"/> with its child <see cref="Expression"/>.
/// Its search pattern when eliminating is based on an "Execution Context" including an additional <see cref="Exists"/>,
/// therefore leaving many <c>Visit</c> methods empty.
/// </summary>
public class EquationEliminator : ISyntaxTreeNodeVisitor
{
    /// <summary>
    /// Field <c>_targetEqe</c> represents the <see cref="Eqe"/> that is holding the <see cref="Equation"/> to eliminate.
    /// </summary>
    private readonly Eqe _targetEqe;

    /// <summary>
    /// Initialises a new instance of the <see cref="EquationEliminator"/> class.
    /// </summary>
    /// <param name="targetEqe"><c>targetEqe</c> represents the <see cref="Eqe"/> that is holding the <see cref="Equation"/> to eliminate.</param>
    public EquationEliminator(Eqe targetEqe) => _targetEqe = targetEqe;

    /// <summary>
    /// Property <c>TargetEqe</c> represents the <see cref="Eqe"/> that is holding the <see cref="Equation"/> to eliminate.
    /// </summary>
    public Eqe TargetEqe => _targetEqe;

    /// <summary>
    /// This method eliminates the <see cref="Equation"/> occuring in the <c>TargetEqe</c>
    /// by traversing through the given <paramref name="expression"/>.
    /// </summary>
    /// <param name="expression"><c>expression</c> represents the <see cref="Expression"/> where elimination happens.</param>
    public void EliminateEquationIn(Expression expression) => expression.Accept(this);

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
    /// This method tries to locate the <c>TargetEqe</c> in the <paramref name="equation"/>
    /// in order to eliminate its <see cref="Equation"/>.
    /// </summary>
    /// <param name="equation"><c>equation</c> represents the <see cref="Expression"/> where elimination happens.</param>
    public void Visit(Equation equation)
    {
        if (equation.E == TargetEqe)
            equation.E = TargetEqe.E;
        else
            equation.E.Accept(this);
    }

    /// <summary>
    /// This method tries to locate the <c>TargetEqe</c> in the <paramref name="eqe"/>
    /// in order to eliminate its <see cref="Equation"/>.
    /// </summary>
    /// <param name="eqe"><c>eqe</c> represents the <see cref="Expression"/> where elimination happens.</param>
    public void Visit(Eqe eqe)
    {
        if (eqe.Eq == TargetEqe)
            eqe.Eq = TargetEqe.E;
        else
            eqe.Eq.Accept(this);

        if (eqe.E == TargetEqe)
            eqe.E = TargetEqe.E;
        else
            eqe.E.Accept(this);
    }

    /// <summary>
    /// This method tries to locate the <c>TargetEqe</c> in the <paramref name="exists"/>
    /// in order to eliminate its <see cref="Equation"/>.
    /// </summary>
    /// <param name="exists"><c>exists</c> represents the <see cref="Expression"/> where elimination happens.</param>
    public void Visit(Exists exists)
    {
        if (exists.E == TargetEqe)
            exists.E = TargetEqe.E;
        else
            exists.E.Accept(this);
    }

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
