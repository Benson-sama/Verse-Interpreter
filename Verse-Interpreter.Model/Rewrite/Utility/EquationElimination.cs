using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Rewrite.Utility;

public static class EquationElimination
{
    public static void EliminateEquation(this Expression expression, Eqe targetEqe)
    {
        switch (expression)
        {
            case Eqe eqe:
                eqe.EliminateEquation(targetEqe);
                break;
            case Lambda lambda:
                lambda.EliminateEquation(targetEqe);
                break;
            case Choice choice:
                choice.EliminateEquation(targetEqe);
                break;
            case Exists exists:
                exists.EliminateEquation(targetEqe);
                break;
            case Wrapper wrapper:
                wrapper.EliminateEquation(targetEqe);
                break;
        }
    }

    private static void EliminateEquation(this Eqe eqe, Eqe targetEqe)
    {
        if (eqe.Eq == targetEqe)
            eqe.Eq = targetEqe.E;
        else
            eqe.Eq.EliminateEquation(targetEqe);

        if (eqe.E == targetEqe)
            eqe.E = targetEqe.E;
        else
            eqe.E.EliminateEquation(targetEqe);
    }

    public static void EliminateEquation(this IExpressionOrEquation eq, Eqe targetEqe)
    {
        switch (eq)
        {
            case Expression e:
                e.EliminateEquation(targetEqe);
                break;
            case Equation equation:
                equation.EliminateEquation(targetEqe);
                break;
        }
    }

    private static void EliminateEquation(this Equation equation, Eqe targetEqe)
    {
        if (equation.E == targetEqe)
            equation.E = targetEqe.E;
        else
            equation.E.EliminateEquation(targetEqe);
    }

    private static void EliminateEquation(this Lambda lambda, Eqe targetEqe)
    {
        if (lambda.E == targetEqe)
            lambda.E = targetEqe.E;
        else
            lambda.E.EliminateEquation(targetEqe);
    }

    private static void EliminateEquation(this Choice choice, Eqe targetEqe)
    {
        if (choice.E1 == targetEqe)
            choice.E1 = targetEqe.E;
        else
            choice.E1.EliminateEquation(targetEqe);

        if (choice.E2 == targetEqe)
            choice.E2 = targetEqe.E;
        else
            choice.E2.EliminateEquation(targetEqe);
    }

    private static void EliminateEquation(this Exists exists, Eqe targetEqe)
    {
        if (exists.E == targetEqe)
            exists.E = targetEqe.E;
        else
            exists.E.EliminateEquation(targetEqe);
    }

    private static void EliminateEquation(this Wrapper wrapper, Eqe targetEqe)
    {
        if (wrapper.E == targetEqe)
            wrapper.E = targetEqe.E;
        else
            wrapper.E.EliminateEquation(targetEqe);
    }
}
