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
}
