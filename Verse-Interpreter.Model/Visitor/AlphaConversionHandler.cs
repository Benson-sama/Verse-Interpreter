//--------------------------------------------------------------------------
// <copyright file="AlphaConversionHandler.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the AlphaConversionHandler class.</summary>
//--------------------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

/// <summary>
/// Class <see cref="AlphaConversionHandler"/> serves as an <see cref="ISyntaxTreeNodeVisitor"/>
/// that provides functionalities to rename all occurrences of <c>TargetVariable</c> to <c>NewVariable</c>
/// in expressions. Some <c>Visit</c> methods are left empty, because these do not contain any variables.
/// </summary>
public class AlphaConversionHandler : ISyntaxTreeNodeVisitor
{
    /// <summary>
    /// Field <c>_targetVariable</c> represents the <see cref="Variable"/> that should be renamed.
    /// </summary>
    private readonly Variable _targetVariable;

    /// <summary>
    /// Field <c>_newVariable</c> represents the <see cref="Variable"/> that is the replacement for the <c>_targetVariable</c>.
    /// </summary>
    private readonly Variable _newVariable;

    /// <summary>
    /// Initialises a new instance of the <see cref="AlphaConversionHandler"/> class.
    /// </summary>
    /// <param name="targetVariable"><c>targetVariable</c> represents the <see cref="Variable"/> that should be renamed.</param>
    /// <param name="newVariable"><c>newVariable</c> represents the <see cref="Variable"/> that is the replacement for the <c>_targetVariable</c>.</param>
    public AlphaConversionHandler(Variable targetVariable, Variable newVariable)
        => (_targetVariable,  _newVariable) = (targetVariable, newVariable);

    /// <summary>
    /// Property <c>TargetVariable</c> represents the <see cref="Variable"/> that should be renamed.
    /// </summary>
    public Variable TargetVariable => _targetVariable;

    /// <summary>
    /// Property <c>_newVariable</c> represents the <see cref="Variable"/> that is the replacement for the <c>_targetVariable</c>.
    /// </summary>
    public Variable NewVariable => _newVariable;

    /// <summary>
    /// This method renames all occurrences of <c>TargetVariable</c> to <c>NewVariable</c>
    /// in <paramref name="lambda"/> including its parameter. Stops traversing when a binder
    /// (<see cref="Exists"/> or <see cref="Lambda"/>) with <c>TargetVariable</c> is encountered.
    /// </summary>
    /// <param name="lambda"><c>lambda</c> represents the <see cref="Expression"/> where alpha conversion happens.</param>
    public void ApplyAlphaConversionIncludingParameter(Lambda lambda)
    {
        lambda.Parameter = NewVariable;

        lambda.E.Accept(this);
    }

    /// <summary>
    /// This method renames all occurrences of <c>TargetVariable</c> to <c>NewVariable</c>
    /// in <paramref name="exists"/> including its <see cref="Variable"/>. Stops traversing
    /// when a binder (<see cref="Exists"/> or <see cref="Lambda"/>) with <c>TargetVariable</c> is encountered.
    /// </summary>
    /// <param name="exists"><c>exists</c> represents the <see cref="Expression"/> where alpha conversion happens.</param>
    public void ApplyAlphaConversionIncludingBinder(Exists exists)
    {
        exists.V = NewVariable;

        exists.E.Accept(this);
    }

    /// <summary>
    /// This method renames all occurrences of <c>TargetVariable</c> to <c>NewVariable</c>
    /// in <paramref name="expression"/>. Stops traversing when a binder (<see cref="Exists"/> or <see cref="Lambda"/>)
    /// with <c>TargetVariable</c> is encountered.
    /// </summary>
    /// <param name="expression"><c>expression</c> represents the <see cref="Expression"/> where alpha conversion happens.</param>
    public void ApplyAlphaConversion(Expression expression) => expression.Accept(this);

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
    /// This method renames all occurrences of <c>TargetVariable</c> to <c>NewVariable</c>
    /// in <paramref name="verseTuple"/>. Stops traversing when a binder
    /// (<see cref="Exists"/> or <see cref="Lambda"/>) with <c>TargetVariable</c> is encountered.
    /// </summary>
    /// <param name="verseTuple"><c>verseTuple</c> represents the <see cref="Expression"/> where alpha conversion happens.</param>
    public void Visit(VerseTuple verseTuple)
    {
        Value[] values = verseTuple.ToArray();

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] is Variable v && v == TargetVariable)
                values[i] = NewVariable;
            else
                values[i].Accept(this);
        }

        verseTuple.Values = values;
    }

    /// <summary>
    /// This method renames all occurrences of <c>TargetVariable</c> to <c>NewVariable</c>
    /// in <paramref name="lambda"/>. Stops traversing when a binder
    /// (<see cref="Exists"/> or <see cref="Lambda"/>) with <c>TargetVariable</c> is encountered.
    /// </summary>
    /// <param name="lambda"><c>lambda</c> represents the <see cref="Expression"/> where alpha conversion happens.</param>
    public void Visit(Lambda lambda)
    {
        if (lambda.Parameter is Variable v1 && v1 == TargetVariable)
            return;

        if (lambda.E is Variable v2 && v2 == TargetVariable)
            lambda.E = NewVariable;
        else
            lambda.E.Accept(this);
    }

    /// <summary>
    /// This method renames all occurrences of <c>TargetVariable</c> to <c>NewVariable</c>
    /// in <paramref name="equation"/>. Stops traversing when a binder
    /// (<see cref="Exists"/> or <see cref="Lambda"/>) with <c>TargetVariable</c> is encountered.
    /// </summary>
    /// <param name="equation"><c>equation</c> represents the <see cref="Expression"/> where alpha conversion happens.</param>
    public void Visit(Equation equation)
    {
        if (equation.V is Variable v1 && v1 == TargetVariable)
            equation.V = NewVariable;
        else
            equation.V.Accept(this);

        if (equation.E is Variable v2 && v2 == TargetVariable)
            equation.E = NewVariable;
        else
            equation.E.Accept(this);
    }

    /// <summary>
    /// This method renames all occurrences of <c>TargetVariable</c> to <c>NewVariable</c>
    /// in <paramref name="eqe"/>. Stops traversing when a binder
    /// (<see cref="Exists"/> or <see cref="Lambda"/>) with <c>TargetVariable</c> is encountered.
    /// </summary>
    /// <param name="eqe"><c>eqe</c> represents the <see cref="Expression"/> where alpha conversion happens.</param>
    public void Visit(Eqe eqe)
    {
        if (eqe.Eq is Variable v1 && v1 == TargetVariable)
            eqe.Eq = NewVariable;
        else
            eqe.Eq.Accept(this);

        if (eqe.E is Variable v2 && v2 == TargetVariable)
            eqe.E = NewVariable;
        else
            eqe.E.Accept(this);
    }

    /// <summary>
    /// This method renames all occurrences of <c>TargetVariable</c> to <c>NewVariable</c>
    /// in <paramref name="exists"/>. Stops traversing when a binder
    /// (<see cref="Exists"/> or <see cref="Lambda"/>) with <c>TargetVariable</c> is encountered.
    /// </summary>
    /// <param name="exists"><c>exists</c> represents the <see cref="Expression"/> where alpha conversion happens.</param>
    public void Visit(Exists exists)
    {
        if (exists.V == TargetVariable)
            return;

        if (exists.E is Variable v && v == TargetVariable)
            exists.E = NewVariable;
        else
            exists.E.Accept(this);
    }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Fail _) { }

    /// <summary>
    /// This method renames all occurrences of <c>TargetVariable</c> to <c>NewVariable</c>
    /// in <paramref name="choice"/>. Stops traversing when a binder
    /// (<see cref="Exists"/> or <see cref="Lambda"/>) with <c>TargetVariable</c> is encountered.
    /// </summary>
    /// <param name="choice"><c>choice</c> represents the <see cref="Expression"/> where alpha conversion happens.</param>
    public void Visit(Choice choice)
    {
        if (choice.E1 is Variable v1 && v1 == TargetVariable)
            choice.E1 = NewVariable;
        else
            choice.E1.Accept(this);

        if (choice.E2 is Variable v2 && v2 == TargetVariable)
            choice.E2 = NewVariable;
        else
            choice.E2.Accept(this);
    }

    /// <summary>
    /// This method renames all occurrences of <c>TargetVariable</c> to <c>NewVariable</c>
    /// in <paramref name="application"/>. Stops traversing when a binder
    /// (<see cref="Exists"/> or <see cref="Lambda"/>) with <c>TargetVariable</c> is encountered.
    /// </summary>
    /// <param name="application"><c>application</c> represents the <see cref="Expression"/> where alpha conversion happens.</param>
    public void Visit(Application application)
    {
        if (application.V1 is Variable v1 && v1 == TargetVariable)
            application.V1 = NewVariable;
        else
            application.V1.Accept(this);

        if (application.V2 is Variable v2 && v2 == TargetVariable)
            application.V2 = NewVariable;
        else
            application.V2.Accept(this);
    }

    /// <summary>
    /// This method calls <see cref="VisitWrapper(Wrapper)"/> with <paramref name="one"/>
    /// to rename all occurrences of <c>TargetVariable</c> to <c>NewVariable</c> in it.
    /// Stops traversing when a binder (<see cref="Exists"/> or <see cref="Lambda"/>) with
    /// <c>TargetVariable</c> is encountered.
    /// </summary>
    /// <param name="one"><c>one</c> represents the <see cref="Expression"/> where alpha conversion happens.</param>
    public void Visit(One one) => VisitWrapper(one);

    /// <summary>
    /// This method calls <see cref="VisitWrapper(Wrapper)"/> with <paramref name="all"/>
    /// to rename all occurrences of <c>TargetVariable</c> to <c>NewVariable</c> in it.
    /// Stops traversing when a binder (<see cref="Exists"/> or <see cref="Lambda"/>) with
    /// <c>TargetVariable</c> is encountered.
    /// </summary>
    /// <param name="all"><c>all</c> represents the <see cref="Expression"/> where alpha conversion happens.</param>
    public void Visit(All all) => VisitWrapper(all);

    /// <summary>
    /// This method renames all occurrences of <c>TargetVariable</c> to <c>NewVariable</c>
    /// in <paramref name="wrapper"/>. Stops traversing when a binder
    /// (<see cref="Exists"/> or <see cref="Lambda"/>) with <c>TargetVariable</c> is encountered.
    /// </summary>
    /// <param name="wrapper"><c>wrapper</c> represents the <see cref="Expression"/> where alpha conversion happens.</param>
    private void VisitWrapper(Wrapper wrapper)
    {
        if (wrapper.E is Variable v && v == TargetVariable)
            wrapper.E = NewVariable;
        else
            wrapper.E.Accept(this);
    }
}
