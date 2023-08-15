using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model;

public static class Extensions
{
    public static IEnumerable<Variable> FreeVariables(this Expression expression)
    {
        VariableBuffer variableBuffer = new();
        expression.FreeVariables(variableBuffer);

        return variableBuffer.FreeVariables;
    }

    public static IEnumerable<Variable> FreeVariables(this Expression expression, VariableBuffer variableBuffer)
    {
        return expression switch
        {
            Eqe eqe => eqe.FreeVariables(variableBuffer),
            Equation eq => eq.FreeVariables(variableBuffer),
            Lambda lambda => lambda.FreeVariables(variableBuffer),
            VerseTuple tuple => tuple.FreeVariables(variableBuffer),
            Variable variable => variable.FreeVariables(variableBuffer),
            Application application => application.FreeVariables(variableBuffer),
            Choice choice => choice.FreeVariables(variableBuffer),
            Exists exists => exists.FreeVariables(variableBuffer),
            Wrapper wrapper => wrapper.FreeVariables(variableBuffer),
            _ => Enumerable.Empty<Variable>()
        };
    }

    private static IEnumerable<Variable> FreeVariables(this Eqe eqe, VariableBuffer variableBuffer)
    {
        _ = eqe.Eq.FreeVariables(variableBuffer);
        return eqe.E.FreeVariables(variableBuffer);
    }

    private static IEnumerable<Variable> FreeVariables(this Equation eq, VariableBuffer variableBuffer)
    {
        _ = eq.V.FreeVariables(variableBuffer);
        return eq.E.FreeVariables(variableBuffer);
    }

    private static IEnumerable<Variable> FreeVariables(this Lambda lambda, VariableBuffer variableBuffer)
    {
        variableBuffer.BoundVariables = variableBuffer.BoundVariables.Append(lambda.Parameter);
        return lambda.E.FreeVariables(variableBuffer);
    }

    private static IEnumerable<Variable> FreeVariables(this VerseTuple tuple, VariableBuffer variableBuffer)
    {
        foreach (Value value in tuple)
            _ = value.FreeVariables(variableBuffer);

        return variableBuffer.FreeVariables;
    }

    private static IEnumerable<Variable> FreeVariables(this Variable variable, VariableBuffer variableBuffer)
    {
        if (!variableBuffer.BoundVariables.Contains(variable) && !variableBuffer.FreeVariables.Contains(variable))
            variableBuffer.FreeVariables = variableBuffer.FreeVariables.Append(variable);

        return variableBuffer.FreeVariables;
    }

    private static IEnumerable<Variable> FreeVariables(this Application application, VariableBuffer variableBuffer)
    {
        _ = application.V1.FreeVariables(variableBuffer);
        return application.V2.FreeVariables(variableBuffer);
    }

    private static IEnumerable<Variable> FreeVariables(this Choice choice, VariableBuffer variableBuffer)
    {
        _ = choice.E1.FreeVariables(variableBuffer);
        return choice.E2.FreeVariables(variableBuffer);
    }

    private static IEnumerable<Variable> FreeVariables(this Exists exists, VariableBuffer variableBuffer)
    {
        variableBuffer.BoundVariables = variableBuffer.BoundVariables.Append(exists.V);
        return exists.E.FreeVariables(variableBuffer);
    }

    private static IEnumerable<Variable> FreeVariables(this Wrapper wrapper, VariableBuffer variableBuffer)
        => wrapper.E.FreeVariables(variableBuffer);

    public static void ApplyAlphaConversion(this Expression expression, Variable previousVariable, Variable newVariable)
    {
        switch (expression)
        {
            case Eqe eqe:
                eqe.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case Equation equation:
                equation.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case Value value:
                value.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case Application application:
                application.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case Choice choice:
                choice.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case Exists exists:
                exists.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case Wrapper wrapper:
                wrapper.ApplyAlphaConversion(previousVariable, newVariable);
                break;
        }
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

    private static void ApplyAlphaConversion(this Value value, Variable previousVariable, Variable newVariable)
    {
        switch (value)
        {
            case Lambda lambda:
                lambda.ApplyAlphaConversion(previousVariable, newVariable);
                break;
            case VerseTuple tuple:
                tuple.ApplyAlphaConversion(previousVariable, newVariable);
                break;
        }
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

    private static void ApplyAlphaConversion(this Exists exists, Variable previousVariable, Variable newVariable)
    {
        if (exists.V.Equals(previousVariable))
            return;

        if (exists.E is Variable v && v.Equals(previousVariable))
            exists.E = newVariable;
        else
            exists.E.ApplyAlphaConversion(previousVariable, newVariable);
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
            finalEqe.E.AvoidCapturingSubstitute(variable, replacingValue);
            return;
        }

        switch (expression)
        {
            case Eqe eqe:
                eqe.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
            case Equation equation:
                equation.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
            case Value value:
                value.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
            case Application application:
                application.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
            case Choice choice:
                choice.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
            case Exists exists:
                exists.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
            case Wrapper wrapper:
                wrapper.SubstituteUntilEqe(finalEqe, variable, replacingValue);
                break;
        }
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

    private static void SubstituteUntilEqe(this Lambda lambda, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (lambda.E is Variable v2 && v2.Equals(variable))
            lambda.E = replacingValue;
        else
            lambda.E.SubstituteUntilEqe(finalEqe, variable, replacingValue);
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

    private static void SubstituteUntilEqe(this Exists exists, Eqe finalEqe, Variable variable, Value replacingValue)
    {
        if (exists.E is Variable v && v.Equals(variable))
            exists.E = replacingValue;
        else
            exists.E.SubstituteUntilEqe(finalEqe, variable, replacingValue);
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
            case Eqe eqe:
                eqe.AvoidCapturingSubstitute(variable, replacingValue);
                break;
            case Equation equation:
                equation.AvoidCapturingSubstitute(variable, replacingValue);
                break;
            case Value value:
                value.AvoidCapturingSubstitute(variable, replacingValue);
                break;
            case Application application:
                application.AvoidCapturingSubstitute(variable, replacingValue);
                break;
            case Choice choice:
                choice.AvoidCapturingSubstitute(variable, replacingValue);
                break;
            case Exists exists:
                exists.AvoidCapturingSubstitute(variable, replacingValue);
                break;
            case Wrapper wrapper:
                wrapper.AvoidCapturingSubstitute(variable, replacingValue);
                break;
        }
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

    private static void AvoidCapturingSubstitute(this Lambda lambda, Variable variable, Value replacingValue)
    {
        if (lambda.Parameter is Variable v1 && v1.Equals(variable))
            return;

        if (lambda.E is Variable v2 && v2.Equals(variable))
            lambda.E = replacingValue;
        else
            lambda.E.AvoidCapturingSubstitute(variable, replacingValue);
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

    private static void AvoidCapturingSubstitute(this Application application, Eqe finalEqe, Variable variable, Value replacingValue)
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

    private static void AvoidCapturingSubstitute(this Exists exists, Variable variable, Value replacingValue)
    {
        if (exists.V.Equals(variable))
            return;

        if (exists.E is Variable v && v.Equals(variable))
            exists.E = replacingValue;
        else
            exists.E.AvoidCapturingSubstitute(variable, replacingValue);
    }

    private static void AvoidCapturingSubstitute(this Wrapper wrapper, Variable variable, Value replacingValue)
    {
        if (wrapper.E is Variable v && v.Equals(variable))
            wrapper.E = replacingValue;
        else
            wrapper.E.AvoidCapturingSubstitute(variable, replacingValue);
    }
}
