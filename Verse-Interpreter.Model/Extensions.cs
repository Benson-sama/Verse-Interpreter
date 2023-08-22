﻿using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model;

public static class Extensions
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
            if (values[i] is Variable v && v.Equals(previousVariable))
                values[i] = newVariable;
            else
                values[i].ApplyAlphaConversion(previousVariable, newVariable);
        }

        tuple.Values = values;
    }

    private static void ApplyAlphaConversion(this Lambda lambda, Variable previousVariable, Variable newVariable)
    {
        if (lambda.Parameter is Variable v1 && v1.Equals(previousVariable))
            return;

        if (lambda.E is Variable v2 && v2.Equals(previousVariable))
            lambda.E = newVariable;
        else
            lambda.E.ApplyAlphaConversion(previousVariable, newVariable);
    }

    private static void ApplyAlphaConversion(this Eqe eqe, Variable previousVariable, Variable newVariable)
    {
        if (eqe.Eq is Variable v1 && v1.Equals(previousVariable))
            eqe.Eq = newVariable;
        else
            eqe.Eq.ApplyAlphaConversion(previousVariable, newVariable);

        if (eqe.E is Variable v2 && v2.Equals(previousVariable))
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
        if (equation.V is Variable v1 && v1.Equals(previousVariable))
            equation.V = newVariable;
        else
            equation.V.ApplyAlphaConversion(previousVariable, newVariable);

        if (equation.E is Variable v2 && v2.Equals(previousVariable))
            equation.E = newVariable;
        else
            equation.E.ApplyAlphaConversion(previousVariable, newVariable);
    }

    private static void ApplyAlphaConversion(this Exists exists, Variable previousVariable, Variable newVariable)
    {
        if (exists.V.Equals(previousVariable))
            return;

        if (exists.E is Variable v && v.Equals(previousVariable))
            exists.E = newVariable;
        else
            exists.E.ApplyAlphaConversion(previousVariable, newVariable);
    }

    private static void ApplyAlphaConversion(this Choice choice, Variable previousVariable, Variable newVariable)
    {
        if (choice.E1 is Variable v1 && v1.Equals(previousVariable))
            choice.E1 = newVariable;
        else
            choice.E1.ApplyAlphaConversion(previousVariable, newVariable);

        if (choice.E2 is Variable v2 && v2.Equals(previousVariable))
            choice.E2 = newVariable;
        else
            choice.E2.ApplyAlphaConversion(previousVariable, newVariable);
    }

    private static void ApplyAlphaConversion(this Application application, Variable previousVariable, Variable newVariable)
    {
        if (application.V1 is Variable v1 && v1.Equals(previousVariable))
            application.V1 = newVariable;
        else
            application.V1.ApplyAlphaConversion(previousVariable, newVariable);

        if (application.V2 is Variable v2 && v2.Equals(previousVariable))
            application.V2 = newVariable;
        else
            application.V2.ApplyAlphaConversion(previousVariable, newVariable);
    }

    private static void ApplyAlphaConversion(this Wrapper wrapper, Variable previousVariable, Variable newVariable)
    {
        if (wrapper.E is Variable v && v.Equals(previousVariable))
            wrapper.E = newVariable;
        else
            wrapper.E.ApplyAlphaConversion(previousVariable, newVariable);
    }

    public static void SubstituteUntilEqe(this Expression expression, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (expression == finalEqe)
        {
            if (finalEqe.E is Variable x && x.Equals(variable))
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
            if (values[i] is Variable v && v.Equals(variable))
                values[i] = replacingValue;
            else
                values[i].SubstituteUntilEqe(finalEqe, variable, replacingValue);
        }

        tuple.Values = values;
    }

    private static void SubstituteUntilEqe(this Lambda lambda, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (lambda.E is Variable v2 && v2.Equals(variable))
            lambda.E = replacingValue;
        else
            lambda.E.SubstituteUntilEqe(finalEqe, variable, replacingValue);
    }

    private static void SubstituteUntilEqe(this Eqe eqe, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (eqe.Eq is Variable v1 && v1.Equals(variable))
            eqe.Eq = replacingValue;
        else
            eqe.Eq.SubstituteUntilEqe(finalEqe, variable, replacingValue);

        if (eqe.E is Variable v2 && v2.Equals(variable))
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
        if (equation.V is Variable v1 && v1.Equals(variable))
            equation.V = replacingValue;
        else
            equation.V.SubstituteUntilEqe(finalEqe, variable, replacingValue);

        if (equation.E is Variable v2 && v2.Equals(variable))
            equation.E = replacingValue;
        else
            equation.E.SubstituteUntilEqe(finalEqe, variable, replacingValue);
    }

    private static void SubstituteUntilEqe(this Exists exists, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (exists.E is Variable v && v.Equals(variable))
            exists.E = replacingValue;
        else
            exists.E.SubstituteUntilEqe(finalEqe, variable, replacingValue);
    }

    private static void SubstituteUntilEqe(this Choice choice, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (choice.E1 is Variable v1 && v1.Equals(variable))
            choice.E1 = replacingValue;
        else
            choice.E1.SubstituteUntilEqe(finalEqe, variable, replacingValue);

        if (choice.E2 is Variable v2 && v2.Equals(variable))
            choice.E2 = replacingValue;
        else
            choice.E2.SubstituteUntilEqe(finalEqe, variable, replacingValue);
    }

    private static void SubstituteUntilEqe(this Application application, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (application.V1 is Variable v1 && v1.Equals(variable))
            application.V1 = replacingValue;
        else
            application.V1.SubstituteUntilEqe(finalEqe, variable, replacingValue);

        if (application.V2 is Variable v2 && v2.Equals(variable))
            application.V2 = replacingValue;
        else
            application.V2.SubstituteUntilEqe(finalEqe, variable, replacingValue);
    }

    private static void SubstituteUntilEqe(this Wrapper wrapper, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (wrapper.E is Variable v && v.Equals(variable))
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
            if (values[i] is Variable v && v.Equals(variable))
                values[i] = replacingValue;
            else
                values[i].AvoidCapturingSubstitute(variable, replacingValue);
        }

        tuple.Values = values;
    }

    private static void AvoidCapturingSubstitute(this Lambda lambda, Variable variable, Value replacingValue)
    {
        if (lambda.Parameter is Variable v1 && v1.Equals(variable))
            return;

        if (lambda.E is Variable v2 && v2.Equals(variable))
            lambda.E = replacingValue;
        else
            lambda.E.AvoidCapturingSubstitute(variable, replacingValue);
    }

    private static void AvoidCapturingSubstitute(this Eqe eqe, Variable variable, Value replacingValue)
    {
        if (eqe.Eq is Variable v1 && v1.Equals(variable))
            eqe.Eq = replacingValue;
        else
            eqe.Eq.AvoidCapturingSubstitute(variable, replacingValue);

        if (eqe.E is Variable v2 && v2.Equals(variable))
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
        if (equation.V is Variable v1 && v1.Equals(variable))
            equation.V = replacingValue;
        else
            equation.V.AvoidCapturingSubstitute(variable, replacingValue);

        if (equation.E is Variable v2 && v2.Equals(variable))
            equation.E = replacingValue;
        else
            equation.E.AvoidCapturingSubstitute(variable, replacingValue);
    }

    private static void AvoidCapturingSubstitute(this Exists exists, Variable variable, Value replacingValue)
    {
        if (exists.V.Equals(variable))
            return;

        if (exists.E is Variable v && v.Equals(variable))
            exists.E = replacingValue;
        else
            exists.E.AvoidCapturingSubstitute(variable, replacingValue);
    }

    private static void AvoidCapturingSubstitute(this Choice choice, Variable variable, Value replacingValue)
    {
        if (choice.E1 is Variable v1 && v1.Equals(variable))
            choice.E1 = replacingValue;
        else
            choice.E1.AvoidCapturingSubstitute(variable, replacingValue);

        if (choice.E2 is Variable v2 && v2.Equals(variable))
            choice.E2 = replacingValue;
        else
            choice.E2.AvoidCapturingSubstitute(variable, replacingValue);
    }

    private static void AvoidCapturingSubstitute(this Application application, Variable variable, Value replacingValue)
    {
        if (application.V1 is Variable v1 && v1.Equals(variable))
            application.V1 = replacingValue;
        else
            application.V1.AvoidCapturingSubstitute(variable, replacingValue);

        if (application.V2 is Variable v2 && v2.Equals(variable))
            application.V2 = replacingValue;
        else
            application.V2.AvoidCapturingSubstitute(variable, replacingValue);
    }

    private static void AvoidCapturingSubstitute(this Wrapper wrapper, Variable variable, Value replacingValue)
    {
        if (wrapper.E is Variable v && v.Equals(variable))
            wrapper.E = replacingValue;
        else
            wrapper.E.AvoidCapturingSubstitute(variable, replacingValue);
    }

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

    private static void ReplaceExistsWithItsExpression(this Eqe eqe, Exists exists)
    {
        if (eqe.Eq == exists)
            eqe.Eq = exists.E;
        else
            eqe.Eq.ReplaceExistsWithItsExpression(exists);

        if (eqe.E == exists)
            eqe.E = exists.E;
        else
            eqe.E.ReplaceExistsWithItsExpression(exists);
    }

    public static void ReplaceExistsWithItsExpression(this IExpressionOrEquation eq, Exists exists)
    {
        switch (eq)
        {
            case Eqe eqe:
                eqe.ReplaceExistsWithItsExpression(exists);
                break;
            case Equation equation:
                equation.ReplaceExistsWithItsExpression(exists);
                break;
        }
    }

    private static void ReplaceExistsWithItsExpression(this Equation equation, Exists exists)
    {
        if (equation.E == exists)
            equation.E = exists.E;
        else
            equation.E.ReplaceExistsWithItsExpression(exists);
    }

    public static void DuplicateChoiceContextUsingChoice(this Wrapper outerScopeContext, Expression cx, Choice targetChoice)
    {
        if (outerScopeContext.E == cx)
        {
            outerScopeContext.E = DuplicateUsingDeepCopy(cx, targetChoice);
            
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
