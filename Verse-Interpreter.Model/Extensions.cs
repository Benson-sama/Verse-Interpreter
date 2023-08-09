using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;
using Tuple = Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Tuple;

namespace Verse_Interpreter.Model;

public static class Extensions
{
    public static IEnumerable<Variable> FreeVariables(this Expression expression, VariableBuffer variableBuffer)
    {
        return expression switch
        {
            Eqe eqe => eqe.FreeVariables(variableBuffer),
            Equation eq => eq.FreeVariables(variableBuffer),
            Lambda lambda => lambda.FreeVariables(variableBuffer),
            Tuple tuple => tuple.FreeVariables(variableBuffer),
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
        eqe.Eq.FreeVariables(variableBuffer);
        return eqe.E.FreeVariables(variableBuffer);
    }

    private static IEnumerable<Variable> FreeVariables(this Equation eq, VariableBuffer variableBuffer)
    {
        eq.V.FreeVariables(variableBuffer);
        return eq.E.FreeVariables(variableBuffer);
    }

    private static IEnumerable<Variable> FreeVariables(this Lambda lambda, VariableBuffer variableBuffer)
    {
        if (lambda.Parameter is not null)
            variableBuffer.BoundVariables = variableBuffer.BoundVariables.Append(lambda.Parameter);

        return lambda.E.FreeVariables(variableBuffer);
    }

    private static IEnumerable<Variable> FreeVariables(this Tuple tuple, VariableBuffer variableBuffer)
    {
        foreach (Value value in tuple.Values)
        {
            value.FreeVariables(variableBuffer);
        }

        return variableBuffer.FreeVariables;
    }

    private static IEnumerable<Variable> FreeVariables(this Variable variable, VariableBuffer variableBuffer)
    {
        if (!variableBuffer.BoundVariables.Contains(variable))
            variableBuffer.FreeVariables = variableBuffer.FreeVariables.Append(variable);

        return variableBuffer.FreeVariables;
    }

    private static IEnumerable<Variable> FreeVariables(this Application application, VariableBuffer variableBuffer)
    {
        application.V1.FreeVariables(variableBuffer);
        return application.V2.FreeVariables(variableBuffer);
    }

    private static IEnumerable<Variable> FreeVariables(this Choice choice, VariableBuffer variableBuffer)
    {
        choice.E1.FreeVariables(variableBuffer);
        return choice.E2.FreeVariables(variableBuffer);
    }

    private static IEnumerable<Variable> FreeVariables(this Exists exists, VariableBuffer variableBuffer)
    {
        variableBuffer.BoundVariables = variableBuffer.BoundVariables.Append(exists.V);
        return exists.E.FreeVariables(variableBuffer);
    }

    private static IEnumerable<Variable> FreeVariables(this Wrapper wrapper, VariableBuffer variableBuffer)
        => wrapper.E.FreeVariables(variableBuffer);
}
