using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Rewrite.Utility;

public static class AlphaConversion
{
    public static void ApplyAlphaConversion(this Expression expression, Variable previousVariable, Variable newVariable)
    {
        switch (expression)
        {
            case Value value:
                value.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case Eqe eqe:
                eqe.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case Exists exists:
                exists.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case Choice choice:
                choice.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case Application application:
                application.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case Wrapper wrapper:
                wrapper.ApplyAlphaConversion(previousVariable, newVariable);
                break;
        }
    }

    private static void ApplyAlphaConversion(this Value value, Variable previousVariable, Variable newVariable)
    {
        switch (value)
        {
            case VerseTuple tuple:
                tuple.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case Lambda lambda:
                lambda.ApplyAlphaConversion(previousVariable, newVariable);
                break;
        }
    }

    private static void ApplyAlphaConversion(this VerseTuple tuple, Variable previousVariable, Variable newVariable)
    {
        Value[] values = tuple.ToArray();

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] is Variable v && v == previousVariable)
                values[i] = newVariable;
            else
                values[i].ApplyAlphaConversion(previousVariable, newVariable);
        }

        tuple.Values = values;
    }

    private static void ApplyAlphaConversion(this Lambda lambda, Variable previousVariable, Variable newVariable)
    {
        if (lambda.Parameter is Variable v1 && v1 == previousVariable)
            return;

        if (lambda.E is Variable v2 && v2 == previousVariable)
            lambda.E = newVariable;
        else
            lambda.E.ApplyAlphaConversion(previousVariable, newVariable);
    }

    private static void ApplyAlphaConversion(this Eqe eqe, Variable previousVariable, Variable newVariable)
    {
        if (eqe.Eq is Variable v1 && v1 == previousVariable)
            eqe.Eq = newVariable;
        else
            eqe.Eq.ApplyAlphaConversion(previousVariable, newVariable);

        if (eqe.E is Variable v2 && v2 == previousVariable)
            eqe.E = newVariable;
        else
            eqe.E.ApplyAlphaConversion(previousVariable, newVariable);
    }

    private static void ApplyAlphaConversion(this IExpressionOrEquation eq, Variable previousVariable, Variable newVariable)
    {
        switch (eq)
        {
            case Expression e:
                e.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case Equation equation:
                equation.ApplyAlphaConversion(previousVariable, newVariable);
                break;
        }
    }

    private static void ApplyAlphaConversion(this Equation equation, Variable previousVariable, Variable newVariable)
    {
        if (equation.V is Variable v1 && v1 == previousVariable)
            equation.V = newVariable;
        else
            equation.V.ApplyAlphaConversion(previousVariable, newVariable);

        if (equation.E is Variable v2 && v2 == previousVariable)
            equation.E = newVariable;
        else
            equation.E.ApplyAlphaConversion(previousVariable, newVariable);
    }

    private static void ApplyAlphaConversion(this Exists exists, Variable previousVariable, Variable newVariable)
    {
        if (exists.V == previousVariable)
            return;

        if (exists.E is Variable v && v == previousVariable)
            exists.E = newVariable;
        else
            exists.E.ApplyAlphaConversion(previousVariable, newVariable);
    }

    private static void ApplyAlphaConversion(this Choice choice, Variable previousVariable, Variable newVariable)
    {
        if (choice.E1 is Variable v1 && v1 == previousVariable)
            choice.E1 = newVariable;
        else
            choice.E1.ApplyAlphaConversion(previousVariable, newVariable);

        if (choice.E2 is Variable v2 && v2 == previousVariable)
            choice.E2 = newVariable;
        else
            choice.E2.ApplyAlphaConversion(previousVariable, newVariable);
    }

    private static void ApplyAlphaConversion(this Application application, Variable previousVariable, Variable newVariable)
    {
        if (application.V1 is Variable v1 && v1 == previousVariable)
            application.V1 = newVariable;
        else
            application.V1.ApplyAlphaConversion(previousVariable, newVariable);

        if (application.V2 is Variable v2 && v2 == previousVariable)
            application.V2 = newVariable;
        else
            application.V2.ApplyAlphaConversion(previousVariable, newVariable);
    }

    private static void ApplyAlphaConversion(this Wrapper wrapper, Variable previousVariable, Variable newVariable)
    {
        if (wrapper.E is Variable v && v == previousVariable)
            wrapper.E = newVariable;
        else
            wrapper.E.ApplyAlphaConversion(previousVariable, newVariable);
    }
}
