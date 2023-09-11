//-----------------------------------------------------------------------
// <copyright file="SubstitutionHandler.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the SubstitutionHandler class.</summary>
//-----------------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

/// <summary>
/// Class <see cref="SubstitutionHandler"/> serves as an <see cref="ISyntaxTreeNodeVisitor"/>
/// that handles the substitution of a specified <see cref="Variable"/> with a specified deep copied <see cref="Value"/>
/// while also leaving a specified <see cref="Equation"/> untouched.
/// </summary>
public class SubstitutionHandler : ISyntaxTreeNodeVisitor
{
    /// <summary>
    /// Field <c>_originalEquation</c> represents the <see cref="Equation"/> to leave untouched.
    /// </summary>
    private readonly Equation _originalEquation;

    /// <summary>
    /// Field <c>_targetVariable</c> represents the <see cref="Variable"/> to substitute.
    /// </summary>
    private readonly Variable _targetVariable;

    /// <summary>
    /// Field <c>_replacingValue</c> represents the <see cref="Value"/> to substitute with.
    /// </summary>
    private readonly Value _replacingValue;

    /// <summary>
    /// Field <c>_deepCopyHandler</c> represents the component to deep copy the <c>_replacingValue</c> for substitution.
    /// </summary>
    private readonly DeepCopyHandler _deepCopyHandler = new();

    /// <summary>
    /// Initialises a new instance of the <see cref="SubstitutionHandler"/> class.
    /// </summary>
    /// <param name="originalEquation"><c>originalEquation</c> represents the <see cref="Equation"/> to leave untouched.</param>
    /// <exception cref="Exception">
    /// Is raised when the <see cref="Value"/> of <paramref name="originalEquation"/> is not a <see cref="Variable"/>
    /// or the <see cref="Expression"/> of <paramref name="originalEquation"/> is not a <see cref="Value"/>.
    /// </exception>
    public SubstitutionHandler(Equation originalEquation)
    {
        if (originalEquation.V is not Variable variable || originalEquation.E is not Value value)
            throw new Exception("Equation must consist of Variable=Value when used in substitution.");

        (_originalEquation, _targetVariable, _replacingValue) = (originalEquation, variable, value);
    }

    /// <summary>
    /// Property <c>OriginalEquation</c> represents the <see cref="Equation"/> to leave untouched.
    /// </summary>
    public Equation OriginalEquation => _originalEquation;

    /// <summary>
    /// Property <c>TargetVariable</c> represents the <see cref="Variable"/> to substitute.
    /// </summary>
    public Variable TargetVariable => _targetVariable;

    /// <summary>
    /// Property <c>ReplacingValue</c> represents the <see cref="Value"/> to substitute with.
    /// </summary>
    public Value ReplacingValue => _replacingValue;

    /// <summary>
    /// Property <c>DeepCopyHandler</c> represents the component to deep copy the <c>ReplacingValue</c> for substitution.
    /// </summary>
    private DeepCopyHandler DeepCopyHandler => _deepCopyHandler;

    /// <summary>
    /// This method will make <paramref name="eq"/> accept this <see cref="ISyntaxTreeNodeVisitor"/> in order
    /// to substitute all occurrences of <c>TargetVariable</c> with a deep copied instance of <c>ReplacingValue</c>
    /// while leaving the <c>OriginalEquation</c> untouched.
    /// </summary>
    /// <param name="eq"><c>eq</c> represents the <see cref="IExpressionOrEquation"/> to accept this <see cref="ISyntaxTreeNodeVisitor"/>.</param>
    public void SubstituteButLeaveEquationUntouched(IExpressionOrEquation eq) => eq.Accept(this);

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
    /// This method substitutes all occurrences of <c>TargetVariable</c> in <paramref name="verseTuple"/>
    /// with a deep copied instance of <c>ReplacingValue</c> while leaving the <c>OriginalEquation</c> untouched.
    /// </summary>
    /// <param name="verseTuple"><c>verseTuple</c> represents the <see cref="Expression"/> where substitution happens.</param>
    public void Visit(VerseTuple verseTuple)
    {
        Value[] values = verseTuple.ToArray();

        for (int i = 0; i < values.Length; i++)
        {
            if (IsTargetVariable(values[i]))
                values[i] = DeepCopiedValue();
            else
                values[i].Accept(this);
        }

        verseTuple.Values = values;
    }

    /// <summary>
    /// This method substitutes all occurrences of <c>TargetVariable</c> in <paramref name="lambda"/>
    /// with a deep copied instance of <c>ReplacingValue</c> while leaving the <c>OriginalEquation</c> untouched.
    /// </summary>
    /// <param name="lambda"><c>lambda</c> represents the <see cref="Expression"/> where substitution happens.</param>
    public void Visit(Lambda lambda)
    {
        if (IsTargetVariable(lambda.Parameter))
            return;

        if (IsTargetVariable(lambda.E))
            lambda.E = DeepCopiedValue();
        else
            AcceptUnlessEquation(lambda.E);
    }

    /// <summary>
    /// This method substitutes all occurrences of <c>TargetVariable</c> in <paramref name="equation"/>
    /// with a deep copied instance of <c>ReplacingValue</c> while leaving the <c>OriginalEquation</c> untouched.
    /// </summary>
    /// <param name="equation"><c>equation</c> represents the <see cref="Expression"/> where substitution happens.</param>
    public void Visit(Equation equation)
    {
        if (IsTargetVariable(equation.V))
            equation.V = DeepCopiedValue();
        else
            equation.V.Accept(this);

        if (IsTargetVariable(equation.E))
            equation.E = DeepCopiedValue();
        else
            AcceptUnlessEquation(equation.E);
    }

    /// <summary>
    /// This method substitutes all occurrences of <c>TargetVariable</c> in <paramref name="eqe"/>
    /// with a deep copied instance of <c>ReplacingValue</c> while leaving the <c>OriginalEquation</c> untouched.
    /// </summary>
    /// <param name="eqe"><c>eqe</c> represents the <see cref="Expression"/> where substitution happens.</param>
    public void Visit(Eqe eqe)
    {
        if (IsTargetVariable(eqe.Eq))
            eqe.Eq = DeepCopiedValue();
        else
            AcceptUnlessEquation(eqe.Eq);

        if (IsTargetVariable(eqe.E))
            eqe.E = DeepCopiedValue();
        else
            AcceptUnlessEquation(eqe.E);
    }

    /// <summary>
    /// This method substitutes all occurrences of <c>TargetVariable</c> in <paramref name="exists"/>
    /// with a deep copied instance of <c>ReplacingValue</c> while leaving the <c>OriginalEquation</c> untouched.
    /// </summary>
    /// <param name="exists"><c>exists</c> represents the <see cref="Expression"/> where substitution happens.</param>
    public void Visit(Exists exists)
    {
        if (IsTargetVariable(exists.V))
            return;

        if (IsTargetVariable(exists.E))
            exists.E = DeepCopiedValue();
        else
            AcceptUnlessEquation(exists.E);
    }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Fail _) { }

    /// <summary>
    /// This method substitutes all occurrences of <c>TargetVariable</c> in <paramref name="choice"/>
    /// with a deep copied instance of <c>ReplacingValue</c> while leaving the <c>OriginalEquation</c> untouched.
    /// </summary>
    /// <param name="choice"><c>choice</c> represents the <see cref="Expression"/> where substitution happens.</param>
    public void Visit(Choice choice)
    {
        if (IsTargetVariable(choice.E1))
            choice.E1 = DeepCopiedValue();
        else
            AcceptUnlessEquation(choice.E1);

        if (IsTargetVariable(choice.E2))
            choice.E2 = DeepCopiedValue();
        else
            AcceptUnlessEquation(choice.E2);
    }

    /// <summary>
    /// This method substitutes all occurrences of <c>TargetVariable</c> in <paramref name="application"/>
    /// with a deep copied instance of <c>ReplacingValue</c> while leaving the <c>OriginalEquation</c> untouched.
    /// </summary>
    /// <param name="application"><c>application</c> represents the <see cref="Expression"/> where substitution happens.</param>
    public void Visit(Application application)
    {
        if (IsTargetVariable(application.V1))
            application.V1 = DeepCopiedValue();
        else
            application.V1.Accept(this);

        if (IsTargetVariable(application.V2))
            application.V2 = DeepCopiedValue();
        else
            application.V2.Accept(this);
    }

    /// <summary>
    /// This method calls <see cref="VisitWrapper(Wrapper)"/> with <paramref name="one"/> in order to substitute
    /// all occurrences of <c>TargetVariable</c> in <paramref name="one"/>
    /// with a deep copied instance of <c>ReplacingValue</c> while leaving the <c>OriginalEquation</c> untouched.
    /// </summary>
    /// <param name="one"><c>one</c> represents the <see cref="Expression"/> where substitution happens.</param>
    public void Visit(One one) => VisitWrapper(one);

    /// <summary>
    /// This method calls <see cref="VisitWrapper(Wrapper)"/> with <paramref name="all"/> in order to substitute
    /// all occurrences of <c>TargetVariable</c> in <paramref name="all"/>
    /// with a deep copied instance of <c>ReplacingValue</c> while leaving the <c>OriginalEquation</c> untouched.
    /// </summary>
    /// <param name="all"><c>all</c> represents the <see cref="Expression"/> where substitution happens.</param>
    public void Visit(All all) => VisitWrapper(all);

    /// <summary>
    /// This method substitutes all occurrences of <c>TargetVariable</c> in <paramref name="wrapper"/>
    /// with a deep copied instance of <c>ReplacingValue</c> while leaving the <c>OriginalEquation</c> untouched.
    /// </summary>
    /// <param name="wrapper"><c>wrapper</c> represents the <see cref="Expression"/> where substitution happens.</param>
    public void VisitWrapper(Wrapper wrapper)
    {
        if (IsTargetVariable(wrapper.E))
            wrapper.E = DeepCopiedValue();
        else
            AcceptUnlessEquation(wrapper.E);
    }

    /// <summary>
    /// This method creates a deep copy of <c>ReplacingValue</c> using the <c>DeepCopyHandler</c>.
    /// </summary>
    /// <returns>The deep copy of <c>ReplacingValue</c>.</returns>
    private Value DeepCopiedValue() => DeepCopyHandler.DeepCopy(ReplacingValue);

    /// <summary>
    /// This method will make <paramref name="eq"/> accept this <see cref="ISyntaxTreeNodeVisitor"/>
    /// unless <paramref name="eq"/> is the <c>OriginalEquation</c>.
    /// </summary>
    /// <param name="eq">
    /// <c>eq</c> represents the <see cref="IExpressionOrEquation"/>
    /// to possibly accept this <see cref="ISyntaxTreeNodeVisitor"/>.
    /// </param>
    private void AcceptUnlessEquation(IExpressionOrEquation eq)
    {
        if (eq == OriginalEquation)
            return;

        eq.Accept(this);
    }

    /// <summary>
    /// This method determines whether or not <paramref name="eq"/> is equal to <c>TargetVariable</c>.
    /// </summary>
    /// <param name="eq"><c>eq</c> represents the object to compare with <c>TargetVariable</c>.</param>
    /// <returns>A value indicating whether or not <paramref name="eq"/> is equal to <c>TargetVariable</c>.</returns>
    private bool IsTargetVariable(IExpressionOrEquation eq)
    {
        if (eq is not Variable v)
            return false;

        return v == TargetVariable;
    }
}
