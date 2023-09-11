//-------------------------------------------------------------------
// <copyright file="DeepCopyHandler.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the DeepCopyHandler class.</summary>
//-------------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

/// <summary>
/// Class <see cref="DeepCopyHandler"/> serves as an <see cref="ISyntaxTreeNodeVisitor{IExpressionOrEquation}"/>
/// that provides functionalities to copy <see cref="IExpressionOrEquation"/> instances while avoiding any shared references.
/// Additionally a <see cref="Choice"/> can be specified that will be replaced by a specified <see cref="Expression"/>.
/// </summary>
public class DeepCopyHandler : ISyntaxTreeNodeVisitor<IExpressionOrEquation>
{
    /// <summary>
    /// Field <c>_excludedChoice</c> represents the <see cref="Choice"/> to be excluded and replaced by <c>_choiceReplacement</c>.
    /// </summary>
    private Choice? _excludedChoice = null;

    /// <summary>
    /// Field <c>_choiceReplacement</c> represents the <see cref="Expression"/> that will replace the <c>_excludedChoice</c>.
    /// </summary>
    private Expression _choiceReplacement = new Fail();

    /// <summary>
    /// Initialises a new instance of the <see cref="DeepCopyHandler"/> class without using the <see cref="Choice"/> exclusion feature.
    /// </summary>
    public DeepCopyHandler()
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="DeepCopyHandler"/> class using the <see cref="Choice"/> exclusion feature.
    /// </summary>
    /// <param name="excludedChoice"><c>excludedChoice</c> represents the <see cref="Choice"/> to be excluded and replaced by <c>ChoiceReplacement</c>.</param>
    public DeepCopyHandler(Choice excludedChoice)
        => _excludedChoice = excludedChoice;

    /// <summary>
    /// Property <c>ExcludedChoice</c> represents the <see cref="Choice"/> to be excluded and replaced by <c>ChoiceReplacement</c>.
    /// </summary>
    public Choice? ExcludedChoice => _excludedChoice;

    /// <summary>
    /// Property <c>ChoiceReplacement</c> represents the <see cref="Expression"/> that will replace the <c>ExcludedChoice</c>.
    /// </summary>
    public Expression ChoiceReplacement => _choiceReplacement;

    /// <summary>
    /// This method creates a deep copy of <paramref name="expression"/> that has no shared references.
    /// </summary>
    /// <param name="expression"><c>expression</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="expression"/>.</returns>
    public Expression DeepCopy(Expression expression)
    {
        _excludedChoice = null;

        return DeepCopyExpression(expression);
    }

    /// <summary>
    /// This method creates a deep copy of <paramref name="value"/> that has no shared references.
    /// </summary>
    /// <param name="value"><c>value</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="value"/>.</returns>
    public Value DeepCopy(Value value)
    {
        _excludedChoice = null;

        return DeepCopyValue(value);
    }

    /// <summary>
    /// This method creates a deep copy of <paramref name="value"/> that has no shared references
    /// while also replacing <c>ExcludedChoice</c> with <paramref name="choiceReplacement"/>.
    /// </summary>
    /// <param name="expression"><c>expression</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <param name="choiceReplacement"><c>choiceReplacement</c> represents the <see cref="Expression"/> that will replace the <c>ExcludedChoice</c>.</param>
    /// <returns>The deep copy of <paramref name="expression"/>.</returns>
    public Expression DeepCopyExceptChoice(Expression expression, Expression choiceReplacement)
    {
        if (ExcludedChoice is null)
            throw new InvalidOperationException("Unable to deep copy except choice without specifying a choice to exclude.");

        _choiceReplacement = choiceReplacement;

        return DeepCopyExpression(expression);
    }

    /// <summary>
    /// This method creates a deep copy of <paramref name="variable"/> that has no shared references.
    /// </summary>
    /// <param name="variable"><c>variable</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="variable"/>.</returns>
    public IExpressionOrEquation Visit(Variable variable)
        => new Variable(variable.Name);

    /// <summary>
    /// This method creates a deep copy of <paramref name="integer"/> that has no shared references.
    /// </summary>
    /// <param name="integer"><c>integer</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="integer"/>.</returns>
    public IExpressionOrEquation Visit(Integer integer)
        => new Integer(integer.Value);

    /// <summary>
    /// This method creates a deep copy of <paramref name="verseString"/> that has no shared references.
    /// </summary>
    /// <param name="verseString"><c>verseString</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="verseString"/>.</returns>
    public IExpressionOrEquation Visit(VerseString verseString)
        => new VerseString(verseString.Text);

    /// <summary>
    /// This method creates a deep copy of <paramref name="verseTuple"/> that has no shared references.
    /// </summary>
    /// <param name="verseTuple"><c>verseTuple</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="verseTuple"/>.</returns>
    public IExpressionOrEquation Visit(VerseTuple verseTuple)
    {
        Value[] values = verseTuple.Values.ToArray();

        for (int i = 0; i < values.Length; i++)
            values[i] = DeepCopyValue(values[i]);

        return new VerseTuple(values);
    }

    /// <summary>
    /// This method creates a deep copy of <paramref name="lambda"/> that has no shared references.
    /// </summary>
    /// <param name="lambda"><c>lambda</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="lambda"/>.</returns>
    public IExpressionOrEquation Visit(Lambda lambda)
    {
        return new Lambda
        {
            Parameter = new Variable(lambda.Parameter.Name),
            E = DeepCopyExpression(lambda.E)
        };
    }

    /// <summary>
    /// This method creates a deep copy of <paramref name="add"/> that has no shared references.
    /// </summary>
    /// <param name="add"><c>add</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="add"/>.</returns>
    public IExpressionOrEquation Visit(Add add) => new Add();

    /// <summary>
    /// This method creates a deep copy of <paramref name="sub"/> that has no shared references.
    /// </summary>
    /// <param name="sub"><c>sub</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="sub"/>.</returns>
    public IExpressionOrEquation Visit(Sub sub) => new Sub();

    /// <summary>
    /// This method creates a deep copy of <paramref name="mult"/> that has no shared references.
    /// </summary>
    /// <param name="mult"><c>mult</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="mult"/>.</returns>
    public IExpressionOrEquation Visit(Mult mult) => new Mult();

    /// <summary>
    /// This method creates a deep copy of <paramref name="div"/> that has no shared references.
    /// </summary>
    /// <param name="div"><c>div</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="div"/>.</returns>
    public IExpressionOrEquation Visit(Div div) => new Div();

    /// <summary>
    /// This method creates a deep copy of <paramref name="gt"/> that has no shared references.
    /// </summary>
    /// <param name="gt"><c>gt</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="gt"/>.</returns>
    public IExpressionOrEquation Visit(Gt gt) => new Gt();

    /// <summary>
    /// This method creates a deep copy of <paramref name="lt"/> that has no shared references.
    /// </summary>
    /// <param name="lt"><c>lt</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="lt"/>.</returns>
    public IExpressionOrEquation Visit(Lt lt) => new Lt();

    /// <summary>
    /// This method creates a deep copy of <paramref name="equation"/> that has no shared references.
    /// </summary>
    /// <param name="equation"><c>equation</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="equation"/>.</returns>
    public IExpressionOrEquation Visit(Equation equation)
    {
        return new Equation
        {
            V = DeepCopyValue(equation.V),
            E = DeepCopyExpression(equation.E)
        };
    }

    /// <summary>
    /// This method creates a deep copy of <paramref name="eqe"/> that has no shared references.
    /// </summary>
    /// <param name="eqe"><c>eqe</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="eqe"/>.</returns>
    public IExpressionOrEquation Visit(Eqe eqe)
    {
        return new Eqe
        {
            Eq = DeepCopyExpressionOrEquation(eqe.Eq),
            E = DeepCopyExpression(eqe.E)
        };
    }

    /// <summary>
    /// This method creates a deep copy of <paramref name="exists"/> that has no shared references.
    /// </summary>
    /// <param name="exists"><c>exists</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="exists"/>.</returns>
    public IExpressionOrEquation Visit(Exists exists)
    {
        return new Exists
        {
            V = new Variable(exists.V.Name),
            E = DeepCopyExpression(exists.E)
        };
    }

    /// <summary>
    /// This method creates a deep copy of <paramref name="fail"/> that has no shared references.
    /// </summary>
    /// <param name="fail"><c>fail</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="fail"/>.</returns>
    public IExpressionOrEquation Visit(Fail fail)
        => new Fail();

    /// <summary>
    /// This method creates a deep copy of <paramref name="choice"/> that has no shared references.
    /// </summary>
    /// <param name="choice"><c>choice</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="choice"/>.</returns>
    public IExpressionOrEquation Visit(Choice choice)
    {
        if (choice == ExcludedChoice)
            return ChoiceReplacement;

        return new Choice
        {
            E1 = DeepCopyExpression(choice.E1),
            E2 = DeepCopyExpression(choice.E2),
        };
    }

    /// <summary>
    /// This method creates a deep copy of <paramref name="application"/> that has no shared references.
    /// </summary>
    /// <param name="application"><c>application</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="application"/>.</returns>
    public IExpressionOrEquation Visit(Application application)
    {
        return new Application
        {
            V1 = DeepCopyValue(application.V1),
            V2 = DeepCopyValue(application.V2)
        };
    }

    /// <summary>
    /// This method creates a deep copy of <paramref name="one"/> that has no shared references.
    /// </summary>
    /// <param name="one"><c>one</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="one"/>.</returns>
    public IExpressionOrEquation Visit(One one)
        => new One { E = DeepCopyExpression(one.E) };

    /// <summary>
    /// This method creates a deep copy of <paramref name="all"/> that has no shared references.
    /// </summary>
    /// <param name="all"><c>all</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="all"/>.</returns>
    public IExpressionOrEquation Visit(All all)
        => new All { E = DeepCopyExpression(all.E) };

    /// <summary>
    /// This method tries to create a deep copy of <paramref name="value"/> that has no shared references.
    /// </summary>
    /// <param name="value"><c>value</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="value"/> as a <see cref="Value"/>.</returns>
    /// <exception cref="ArgumentNullException">Is raised when the deep copy result is not a <see cref="Value"/>.</exception>
    private Value DeepCopyValue(Value value)
    {
        if (value.Accept(this) is not Value newValue)
            throw new ArgumentNullException(nameof(value), "Cannot be null");

        return newValue;
    }

    /// <summary>
    /// This method tries to create a deep copy of <paramref name="expression"/> that has no shared references.
    /// </summary>
    /// <param name="expression"><c>expression</c> represents the <see cref="Expression"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="expression"/> as an <see cref="Expression"/>.</returns>
    /// <exception cref="ArgumentNullException">Is raised when the deep copy result is not a <see cref="Expression"/>.</exception>
    private Expression DeepCopyExpression(Expression expression)
    {
        if (expression.Accept(this) is not Expression newExpression)
            throw new ArgumentNullException(nameof(expression), "Cannot be null");

        return newExpression;
    }

    /// <summary>
    /// This method creates a deep copy of <paramref name="eq"/> that has no shared references.
    /// </summary>
    /// <param name="eq"><c>eq</c> represents the <see cref="IExpressionOrEquation"/> to deep copy.</param>
    /// <returns>The deep copy of <paramref name="eq"/> as an <see cref="IExpressionOrEquation"/>.</returns>
    private IExpressionOrEquation DeepCopyExpressionOrEquation(IExpressionOrEquation eq)
        => eq.Accept(this);
}
