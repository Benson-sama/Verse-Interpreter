using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Rewrite.Utility;

public static class Substitution
{
    public static void SubstituteUntilEqe(this Expression expression, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (expression == finalEqe)
        {
            if (finalEqe.E is Variable x && x == variable)
                finalEqe.E = replacingValue;
            else
                finalEqe.E.AvoidCapturingSubstitute(variable, replacingValue);

            return;
        }

        switch (expression)
        {
            case Value value:
                value.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
            case Eqe eqe:
                eqe.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
            case Exists exists:
                exists.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
            case Choice choice:
                choice.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
            case Application application:
                application.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
            case Wrapper wrapper:
                wrapper.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
        }
    }

    private static void SubstituteUntilEqe(this Value value, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        switch (value)
        {
            case Lambda lambda:
                lambda.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
            case VerseTuple tuple:
                tuple.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
        }
    }

    private static void SubstituteUntilEqe(this VerseTuple tuple, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        Value[] values = tuple.ToArray();

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] is Variable v && v == variable)
                values[i] = replacingValue;
            else
                values[i].SubstituteUntilEqe(finalEqe, variable, replacingValue);
        }

        tuple.Values = values;
    }

    private static void SubstituteUntilEqe(this Lambda lambda, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (lambda.E is Variable v2 && v2 == variable)
            lambda.E = replacingValue;
        else
            lambda.E.SubstituteUntilEqe(finalEqe, variable, replacingValue);
    }

    private static void SubstituteUntilEqe(this Eqe eqe, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (eqe.Eq is Variable v1 && v1 == variable)
            eqe.Eq = replacingValue;
        else
            eqe.Eq.SubstituteUntilEqe(finalEqe, variable, replacingValue);

        if (eqe.E is Variable v2 && v2 == variable)
            eqe.E = replacingValue;
        else
            eqe.E.SubstituteUntilEqe(finalEqe, variable, replacingValue);
    }

    public static void SubstituteUntilEqe(this IExpressionOrEquation eq, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        switch (eq)
        {
            case Expression e:
                e.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
            case Equation equation:
                equation.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
        }
    }

    private static void SubstituteUntilEqe(this Equation equation, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (equation.V is Variable v1 && v1 == variable)
            equation.V = replacingValue;
        else
            equation.V.SubstituteUntilEqe(finalEqe, variable, replacingValue);

        if (equation.E is Variable v2 && v2 == variable)
            equation.E = replacingValue;
        else
            equation.E.SubstituteUntilEqe(finalEqe, variable, replacingValue);
    }

    private static void SubstituteUntilEqe(this Exists exists, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (exists.E is Variable v && v == variable)
            exists.E = replacingValue;
        else
            exists.E.SubstituteUntilEqe(finalEqe, variable, replacingValue);
    }

    private static void SubstituteUntilEqe(this Choice choice, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (choice.E1 is Variable v1 && v1 == variable)
            choice.E1 = replacingValue;
        else
            choice.E1.SubstituteUntilEqe(finalEqe, variable, replacingValue);

        if (choice.E2 is Variable v2 && v2 == variable)
            choice.E2 = replacingValue;
        else
            choice.E2.SubstituteUntilEqe(finalEqe, variable, replacingValue);
    }

    private static void SubstituteUntilEqe(this Application application, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (application.V1 is Variable v1 && v1 == variable)
            application.V1 = replacingValue;
        else
            application.V1.SubstituteUntilEqe(finalEqe, variable, replacingValue);

        if (application.V2 is Variable v2 && v2 == variable)
            application.V2 = replacingValue;
        else
            application.V2.SubstituteUntilEqe(finalEqe, variable, replacingValue);
    }

    private static void SubstituteUntilEqe(this Wrapper wrapper, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (wrapper.E is Variable v && v == variable)
            wrapper.E = replacingValue;
        else
            wrapper.E.SubstituteUntilEqe(finalEqe, variable, replacingValue);
    }

    private static void AvoidCapturingSubstitute(this Expression expression, Variable variable, Value replacingValue)
    {
        switch (expression)
        {
            case Value value:
                value.AvoidCapturingSubstitute(variable, replacingValue);
                break;
            case Eqe eqe:
                eqe.AvoidCapturingSubstitute(variable, replacingValue);
                break;
            case Exists exists:
                exists.AvoidCapturingSubstitute(variable, replacingValue);
                break;
            case Choice choice:
                choice.AvoidCapturingSubstitute(variable, replacingValue);
                break;
            case Application application:
                application.AvoidCapturingSubstitute(variable, replacingValue);
                break;
            case Wrapper wrapper:
                wrapper.AvoidCapturingSubstitute(variable, replacingValue);
                break;
        }
    }

    private static void AvoidCapturingSubstitute(this Value value, Variable variable, Value replacingValue)
    {
        switch (value)
        {
            case Lambda lambda:
                lambda.AvoidCapturingSubstitute(variable, replacingValue);
                break;
            case VerseTuple tuple:
                tuple.AvoidCapturingSubstitute(variable, replacingValue);
                break;
        }
    }

    private static void AvoidCapturingSubstitute(this VerseTuple tuple, Variable variable, Value replacingValue)
    {
        Value[] values = tuple.ToArray();

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] is Variable v && v == variable)
                values[i] = replacingValue;
            else
                values[i].AvoidCapturingSubstitute(variable, replacingValue);
        }

        tuple.Values = values;
    }

    private static void AvoidCapturingSubstitute(this Lambda lambda, Variable variable, Value replacingValue)
    {
        if (lambda.Parameter is Variable v1 && v1 == variable)
            return;

        if (lambda.E is Variable v2 && v2 == variable)
            lambda.E = replacingValue;
        else
            lambda.E.AvoidCapturingSubstitute(variable, replacingValue);
    }

    private static void AvoidCapturingSubstitute(this Eqe eqe, Variable variable, Value replacingValue)
    {
        if (eqe.Eq is Variable v1 && v1 == variable)
            eqe.Eq = replacingValue;
        else
            eqe.Eq.AvoidCapturingSubstitute(variable, replacingValue);

        if (eqe.E is Variable v2 && v2 == variable)
            eqe.E = replacingValue;
        else
            eqe.E.AvoidCapturingSubstitute(variable, replacingValue);
    }

    private static void AvoidCapturingSubstitute(this IExpressionOrEquation eq, Variable variable, Value replacingValue)
    {
        switch (eq)
        {
            case Expression e:
                e.AvoidCapturingSubstitute(variable, replacingValue);
                break;
            case Equation equation:
                equation.AvoidCapturingSubstitute(variable, replacingValue);
                break;
        }
    }

    private static void AvoidCapturingSubstitute(this Equation equation, Variable variable, Value replacingValue)
    {
        if (equation.V is Variable v1 && v1 == variable)
            equation.V = replacingValue;
        else
            equation.V.AvoidCapturingSubstitute(variable, replacingValue);

        if (equation.E is Variable v2 && v2 == variable)
            equation.E = replacingValue;
        else
            equation.E.AvoidCapturingSubstitute(variable, replacingValue);
    }

    private static void AvoidCapturingSubstitute(this Exists exists, Variable variable, Value replacingValue)
    {
        if (exists.V == variable)
            return;

        if (exists.E is Variable v && v == variable)
            exists.E = replacingValue;
        else
            exists.E.AvoidCapturingSubstitute(variable, replacingValue);
    }

    private static void AvoidCapturingSubstitute(this Choice choice, Variable variable, Value replacingValue)
    {
        if (choice.E1 is Variable v1 && v1 == variable)
            choice.E1 = replacingValue;
        else
            choice.E1.AvoidCapturingSubstitute(variable, replacingValue);

        if (choice.E2 is Variable v2 && v2 == variable)
            choice.E2 = replacingValue;
        else
            choice.E2.AvoidCapturingSubstitute(variable, replacingValue);
    }

    private static void AvoidCapturingSubstitute(this Application application, Variable variable, Value replacingValue)
    {
        if (application.V1 is Variable v1 && v1 == variable)
            application.V1 = replacingValue;
        else
            application.V1.AvoidCapturingSubstitute(variable, replacingValue);

        if (application.V2 is Variable v2 && v2 == variable)
            application.V2 = replacingValue;
        else
            application.V2.AvoidCapturingSubstitute(variable, replacingValue);
    }

    private static void AvoidCapturingSubstitute(this Wrapper wrapper, Variable variable, Value replacingValue)
    {
        if (wrapper.E is Variable v && v == variable)
            wrapper.E = replacingValue;
        else
            wrapper.E.AvoidCapturingSubstitute(variable, replacingValue);
    }
}
