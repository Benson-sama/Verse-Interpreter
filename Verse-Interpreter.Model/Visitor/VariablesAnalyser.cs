//---------------------------------------------------------------------
// <copyright file="VariablesAnalyser.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the VariablesAnalyser class.</summary>
//---------------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

/// <summary>
/// Class <see cref="VariablesAnalyser"/> serves as an <see cref="ISyntaxTreeNodeVisitor"/>
/// that provides functionalites to determine the free and bound variables of expressions.
/// </summary>
public class VariablesAnalyser : ISyntaxTreeNodeVisitor
{
    /// <summary>
    /// Field <c>_finalExpression</c> represents the <see cref="Expression"/> where analysing stops.
    /// </summary>
    private Expression? _finalExpression;

    /// <summary>
    /// Field <c>_boundVariables</c> represents the list of bound variables.
    /// </summary>
    private readonly List<Variable> _boundVariables = new();

    /// <summary>
    /// Field <c>_freeVariables</c> represents the list of bound variables.
    /// </summary>
    private readonly List<Variable> _freeVariables = new();

    /// <summary>
    /// Property <c>IsFinalExpressionReached</c> represents a value that is indicating whether or not the stop condition is met.
    /// </summary>
    private bool IsFinalExpressionReached { get; set; }

    /// <summary>
    /// Property <c>BoundVariables</c> represents the list of bound variables.
    /// </summary>
    public IEnumerable<Variable> BoundVariables => _boundVariables;

    /// <summary>
    /// Property <c>FreeVariables</c> represents the list of free variables.
    /// </summary>
    public IEnumerable<Variable> FreeVariables => _freeVariables;

    /// <summary>
    /// This method sets <c>IsFinalExpressionReached</c> to false and clears the free and bound variables lists.
    /// </summary>
    public void Reset()
    {
        IsFinalExpressionReached = false;

        _boundVariables.Clear();
        _freeVariables.Clear();
    }

    /// <summary>
    /// This method makes use of <see cref="AnalyseVariablesOf(Expression, Expression?)"/> while also returning the <c>FreeVariables</c>.
    /// </summary>
    /// <param name="expression"><c>expression</c> represents the <see cref="Expression"/> to analyse.</param>
    /// <param name="finalExpression"><c>finalExpression</c> represents the <see cref="Expression"/> where analysing stops.</param>
    /// <returns>The free variables of <paramref name="expression"/>.</returns>
    public IEnumerable<Variable> FreeVariablesOf(Expression expression, Expression? finalExpression = null)
    {
        AnalyseVariablesOf(expression, finalExpression);

        return FreeVariables;
    }

    /// <summary>
    /// This method analyses the free and bound variables of <paramref name="expression"/> optionally stopping at <paramref name="finalExpression"/>.
    /// </summary>
    /// <param name="expression"><c>expression</c> represents the <see cref="Expression"/> to analyse.</param>
    /// <param name="finalExpression"><c>finaleExpression</c> represents the <see cref="Expression"/> where analysing stops.</param>
    public void AnalyseVariablesOf(Expression expression, Expression? finalExpression = null)
    {
        _finalExpression = finalExpression;

        Reset();
        AcceptUnlessFinalExpressionReached(expression);
    }

    /// <summary>
    /// This method determines if <paramref name="x"/> is bound inside <paramref name="y"/> in <paramref name="expression"/>.
    /// </summary>
    /// <param name="expression"><c>expression</c> represents the <see cref="Expression"/> to check.</param>
    /// <param name="x"><c>x</c> represents the <see cref="Variable"/> that is bound inside.</param>
    /// <param name="y"><c>y</c> represents the <see cref="Variable"/> that is bound outside.</param>
    /// <returns>A value indicating whether or not <paramref name="x"/> is bound inside <paramref name="y"/>.</returns>
    /// <exception cref="Exception">Is raised when <paramref name="expression"/> is null or <paramref name="x"/> is not found.</exception>
    public bool VariableBoundInsideVariable(Expression expression, Variable x, Variable y)
    {
        if (expression is null)
            throw new Exception("Cannot check bound variable order if verse program is null.");

        bool foundY = false;

        AnalyseVariablesOf(expression);

        foreach (Variable variable in BoundVariables)
        {
            if (variable == y)
            {
                foundY = true;
                continue;
            }

            if (variable == x && !foundY)
                return false;
            else if (variable == x && foundY)
                return true;
        }

        throw new Exception($"Did not find bound variable {x}");
    }

    /// <summary>
    /// This method adds <paramref name="variable"/> to <c>FreeVariables</c> if it is not bound and not free.
    /// </summary>
    /// <param name="variable"><c>variable</c> represents the variable to analyse.</param>
    public void Visit(Variable variable)
    {
        if (!_boundVariables.Contains(variable) && !FreeVariables.Contains(variable))
            _freeVariables.Add(variable);
    }

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
    /// This method analyses the free and bound variables of <paramref name="verseTuple"/>.
    /// </summary>
    /// <param name="verseTuple"><c>verseTuple</c> represents the <see cref="Expression"/> to analyse.</param>
    public void Visit(VerseTuple verseTuple)
    {
        foreach (Value value in verseTuple)
            AcceptUnlessFinalExpressionReached(value);
    }

    /// <summary>
    /// This method adds the parameter of <paramref name="lambda"/> to the <c>BoundVariables</c>
    /// and further analyses the free and bound variables of the child <see cref="Expression"/> of <paramref name="lambda"/>.
    /// </summary>
    /// <param name="lambda"><c>lambda</c> represents the <see cref="Expression"/> to analyse.</param>
    public void Visit(Lambda lambda)
    {
        _boundVariables.Add(lambda.Parameter);

        AcceptUnlessFinalExpressionReached(lambda.E);
    }

    /// <summary>
    /// This method analyses the free and bound variables of the <paramref name="equation"/>.
    /// </summary>
    /// <param name="equation"><c>equation</c> represents the <see cref="Expression"/> to analyse.</param>
    public void Visit(Equation equation)
    {
        AcceptUnlessFinalExpressionReached(equation.V);
        AcceptUnlessFinalExpressionReached(equation.E);
    }

    /// <summary>
    /// This method analyses the free and bound variables of the <paramref name="eqe"/>.
    /// </summary>
    /// <param name="eqe"><c>eqe</c> represents the <see cref="Expression"/> to analyse.</param>
    public void Visit(Eqe eqe)
    {
        AcceptUnlessFinalExpressionReached(eqe.Eq);
        AcceptUnlessFinalExpressionReached(eqe.E);
    }

    /// <summary>
    /// This method adds the variable of <paramref name="exists"/> to the <c>BoundVariables</c>
    /// and further analyses the free and bound variables of the child <see cref="Expression"/> of <paramref name="exists"/>.
    /// </summary>
    /// <param name="exists"><c>exists</c> represents the <see cref="Expression"/> to analyse.</param>
    public void Visit(Exists exists)
    {
        _boundVariables.Add(exists.V);

        AcceptUnlessFinalExpressionReached(exists.E);
    }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Fail _) { }

    /// <summary>
    /// This method analyses the free and bound variables of the <paramref name="choice"/>.
    /// </summary>
    /// <param name="choice"><c>choice</c> represents the <see cref="Expression"/> to analyse.</param>
    public void Visit(Choice choice)
    {
        AcceptUnlessFinalExpressionReached(choice.E1);
        AcceptUnlessFinalExpressionReached(choice.E2);
    }

    /// <summary>
    /// This method analyses the free and bound variables of the <paramref name="application"/>.
    /// </summary>
    /// <param name="application"><c>application</c> represents the <see cref="Expression"/> to analyse.</param>
    public void Visit(Application application)
    {
        AcceptUnlessFinalExpressionReached(application.V1);
        AcceptUnlessFinalExpressionReached(application.V2);
    }

    /// <summary>
    /// This method calls <see cref="VisitWrapper(Wrapper)"/> with <paramref name="one"/>
    /// to analyse the free and bound variables of <paramref name="one"/>.
    /// </summary>
    /// <param name="one"><c>one</c> represents the <see cref="Expression"/> to analyse.</param>
    public void Visit(One one) => VisitWrapper(one);

    /// <summary>
    /// This method calls <see cref="VisitWrapper(Wrapper)"/> with <paramref name="all"/>
    /// to analyse the free and bound variables of <paramref name="all"/>.
    /// </summary>
    /// <param name="all"><c>all</c> represents the <see cref="Expression"/> to analyse.</param>
    public void Visit(All all) => VisitWrapper(all);

    /// <summary>
    /// This method analyses the free and bound variables of the <paramref name="wrapper"/>.
    /// </summary>
    /// <param name="wrapper"><c>wrapper</c> represents the <see cref="Expression"/> to analyse.</param>
    private void VisitWrapper(Wrapper wrapper) => AcceptUnlessFinalExpressionReached(wrapper.E);

    /// <summary>
    /// This method will make the <paramref name="eq"/> accept this <see cref="ISyntaxTreeNodeVisitor"/>
    /// unless <paramref name="eq"/> is the <c>_finalExpression</c> or <c>IsFinalExpressionReached</c> is true.
    /// </summary>
    /// <param name="eq">
    /// <c>eq</c> represents the <see cref="IExpressionOrEquation"/> to possibly 
    /// accept this <see cref="ISyntaxTreeNodeVisitor"/>.
    /// </param>
    private void AcceptUnlessFinalExpressionReached(IExpressionOrEquation eq)
    {
        if (eq == _finalExpression)
        {
            IsFinalExpressionReached = true;
            return;
        }
        else if (IsFinalExpressionReached)
        {
            return;
        }

        eq.Accept(this);
    }
}
