//-----------------------------------------------------------------
// <copyright file="ChoiceFloater.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ChoiceFloater class.</summary>
//-----------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

/// <summary>
/// Class <see cref="ChoiceFloater"/> serves as an <see cref="ISyntaxTreeNodeVisitor"/>
/// that floats a specific <see cref="Choice"/> to the top.
/// That means: Duplicate the surrounding <c>ChoiceContext</c> for each choice in <c>TargetChoice</c>.
/// Its search pattern when floating is based on "Scope Context" and "Choice Context" combined,
/// therefore leaving many <c>Visit</c> methods empty.
/// </summary>
public class ChoiceFloater : ISyntaxTreeNodeVisitor
{
    /// <summary>
    /// Field <c>_choiceContext</c> represents the surrounding choice context.
    /// </summary>
    private readonly Expression _choiceContext;

    /// <summary>
    /// Field _targetChoice represents the <see cref="Choice"/> that is inside the <c>ChoiceContext</c>.
    /// </summary>
    private readonly Choice _targetChoice;

    /// <summary>
    /// Initialises a new instance of the <see cref="ChoiceFloater"/> class.
    /// </summary>
    /// <param name="choiceContext"><c>choiceContext</c> represents the surrounding choice context.</param>
    /// <param name="targetChoice"><c>targetChoice</c> represents the <see cref="Choice"/> that is inside the <paramref name="choiceContext"/>.</param>
    public ChoiceFloater(Expression choiceContext, Choice targetChoice)
        => (_choiceContext, _targetChoice) = (choiceContext, targetChoice);

    /// <summary>
    /// Property <c>ChoiceContext</c> represents the surrounding choice context.
    /// </summary>
    public Expression ChoiceContext => _choiceContext;

    /// <summary>
    /// Property TargetChoice represents the <see cref="Choice"/> that is inside the <c>ChoiceContext</c>.
    /// </summary>
    public Choice TargetChoice => _targetChoice;

    /// <summary>
    /// This method floats the <c>TargetChoice</c> to the top using the <c>ChoiceContext</c>
    /// in the specified <paramref name="outerScopeContext"/>.
    /// </summary>
    /// <param name="outerScopeContext"><c>outerScopeContext</c> represents the <see cref="Expression"/> where floating happens.</param>
    public void FloatChoiceToTheTopIn(Wrapper outerScopeContext)
    {
        if (outerScopeContext.E == ChoiceContext)
        {
            outerScopeContext.E = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);

            return;
        }

        outerScopeContext.E.Accept(this);
    }

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
    /// This method tries to float the <c>TargetChoice</c> to the top using the <c>ChoiceContext</c>
    /// in the specified <paramref name="equation"/>, or keep searching if it was not possible.
    /// </summary>
    /// <param name="equation"><c>equation</c> represents the <see cref="Equation"/> where floating happens.</param>
    public void Visit(Equation equation)
    {
        if (equation.E == ChoiceContext)
            equation.E = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);
        else
            equation.E.Accept(this);
    }

    /// <summary>
    /// This method tries to float the <c>TargetChoice</c> to the top using the <c>ChoiceContext</c>
    /// in the specified <paramref name="eqe"/>.
    /// </summary>
    /// <param name="eqe"><c>eqe</c> represents the <see cref="Eqe"/> where floating happens.</param>
    public void Visit(Eqe eqe)
    {
        if (eqe.Eq == ChoiceContext)
            eqe.Eq = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);
        else
            eqe.Eq.Accept(this);

        if (eqe.E == ChoiceContext)
            eqe.E = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);
        else
            eqe.E.Accept(this);
    }

    /// <summary>
    /// This method tries to float the <c>TargetChoice</c> to the top using the <c>ChoiceContext</c>
    /// in the specified <paramref name="exists"/>, or keep searching if it was not possible.
    /// </summary>
    /// <param name="exists"><c>exists</c> represents the <see cref="Exists"/> where floating happens.</param>
    public void Visit(Exists exists)
    {
        if (exists.E == ChoiceContext)
            exists.E = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);
        else
            exists.E.Accept(this);
    }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Fail _) { }

    /// <summary>
    /// This method tries to float the <c>TargetChoice</c> to the top using the <c>ChoiceContext</c>
    /// in the specified <paramref name="choice"/>, or keep searching if it was not possible.
    /// </summary>
    /// <param name="choice"><c>choice</c> represents the <see cref="Choice"/> where floating happens.</param>
    public void Visit(Choice choice)
    {
        if (choice.E1 == ChoiceContext)
            choice.E1 = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);
        else
            choice.E1.Accept(this);

        if (choice.E2 == ChoiceContext)
            choice.E2 = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);
        else
            choice.E2.Accept(this);
    }

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

    /// <summary>
    /// This method builds a new <see cref="Choice"/> with deep copies of the <paramref name="choiceContext"/>
    /// where the original <paramref name="choice"/> is replaced by its own choices respectively.
    /// </summary>
    /// <param name="choiceContext"><c>choiceContext</c> represents surrounding choice context to duplicate.</param>
    /// <param name="choice"><c>choice</c> represents the original choice to replace by its own choices respectively.</param>
    /// <returns>The new floated <see cref="Choice"/>.</returns>
    private static Expression DuplicateUsingDeepCopy(Expression choiceContext, Choice choice)
    {
        DeepCopyHandler deepCopyExceptChoiceHandler = new(choice);

        return new Choice
        {
            E1 = deepCopyExceptChoiceHandler.DeepCopyExceptChoice(choiceContext, choice.E1),
            E2 = deepCopyExceptChoiceHandler.DeepCopyExceptChoice(choiceContext, choice.E2)
        };
    }
}
