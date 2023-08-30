using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Rewrite.Utility;

public static class DuplicateChoice
{
    public static void DuplicateChoiceContextUsingChoice(this Wrapper outerScopeContext, Expression cx, Choice targetChoice)
    {
        if (outerScopeContext.E == cx)
        {
            outerScopeContext.E = cx.DuplicateUsingDeepCopy(targetChoice);

            return;
        }

        switch (outerScopeContext)
        {
            case One one:
                one.E.DuplicateChoiceContextUsingChoice(cx, targetChoice);
                break;
            case All all:
                all.E.DuplicateChoiceContextUsingChoice(cx, targetChoice);
                break;
            default:
                throw new Exception("Unknown outer scope context in duplicate choice context.");
        }
    }

    public static void DuplicateChoiceContextUsingChoice(this Expression expression, Expression cx, Choice targetChoice)
    {
        switch (expression)
        {
            case Choice choice:
                choice.DuplicateChoiceContextUsingChoice(cx, targetChoice);
                break;
            case Eqe eqe:
                eqe.DuplicateChoiceContextUsingChoice(cx, targetChoice);
                break;
            case Exists exists:
                exists.DuplicateChoiceContextUsingChoice(cx, targetChoice);
                break;
        }
    }

    private static void DuplicateChoiceContextUsingChoice(this Eqe eqe, Expression cx, Choice targetChoice)
    {
        if (eqe.Eq == cx)
            eqe.Eq = cx.DuplicateUsingDeepCopy(targetChoice);
        else
            eqe.Eq.DuplicateChoiceContextUsingChoice(cx, targetChoice);

        if (eqe.E == cx)
            eqe.E = cx.DuplicateUsingDeepCopy(targetChoice);
        else
            eqe.E.DuplicateChoiceContextUsingChoice(cx, targetChoice);
    }

    private static void DuplicateChoiceContextUsingChoice(this IExpressionOrEquation eq, Expression cx, Choice targetChoice)
    {
        switch (eq)
        {
            case Expression e:
                e.DuplicateChoiceContextUsingChoice(cx, targetChoice);
                break;
            case Equation equation:
                equation.DuplicateChoiceContextUsingChoice(cx, targetChoice);
                break;
        }
    }

    private static void DuplicateChoiceContextUsingChoice(this Equation equation, Expression cx, Choice targetChoice)
    {
        if (equation.E == cx)
            equation.E = cx.DuplicateUsingDeepCopy(targetChoice);
        else
            equation.E.DuplicateChoiceContextUsingChoice(cx, targetChoice);
    }

    private static void DuplicateChoiceContextUsingChoice(this Exists exists, Expression cx, Choice targetChoice)
    {
        if (exists.E == cx)
            exists.E = cx.DuplicateUsingDeepCopy(targetChoice);
        else
            exists.E.DuplicateChoiceContextUsingChoice(cx, targetChoice);
    }

    private static void DuplicateChoiceContextUsingChoice(this Choice choice, Expression cx, Choice targetChoice)
    {
        if (choice.E1 == cx)
            choice.E1 = cx.DuplicateUsingDeepCopy(targetChoice);
        else
            choice.E1.DuplicateChoiceContextUsingChoice(cx, targetChoice);

        if (choice.E2 == cx)
            choice.E2 = cx.DuplicateUsingDeepCopy(targetChoice);
        else
            choice.E2.DuplicateChoiceContextUsingChoice(cx, targetChoice);
    }

    private static Expression DuplicateUsingDeepCopy(this Expression cx, Choice choice)
    {
        return new Choice
        {
            E1 = cx.DeepCopyButReplaceChoice(choice, choice.E1),
            E2 = cx.DeepCopyButReplaceChoice(choice, choice.E2)
        };
    }
}
