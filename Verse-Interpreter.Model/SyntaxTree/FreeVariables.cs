using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.SyntaxTree;

public static class FreeVariables
{
    private static readonly VariableBuffer _variableBuffer = new();

    public static VariableBuffer VariableBuffer { get => _variableBuffer; }

    public static IEnumerable<Variable> Of(Expression expression, Expression? finalExpression = null)
    {
        _variableBuffer.Clear();
        FreeVariablesOf(expression, finalExpression);

        return _variableBuffer.FreeVariables;
    }

    private static IEnumerable<Variable> FreeVariablesOf(Expression expression, Expression? finalExpression = null)
    {
        if (finalExpression is not null && expression == finalExpression)
            return Enumerable.Empty<Variable>();

        return expression switch
        {
            Value value => value switch
            {
                Variable variable => FreeVariablesOf(variable),
                VerseTuple tuple => FreeVariablesOf(tuple),
                Lambda lambda => FreeVariablesOf(lambda),
                _ => Enumerable.Empty<Variable>()
            },
            Eqe eqe => FreeVariablesOf(eqe),
            Exists exists => FreeVariablesOf(exists),
            Choice choice => FreeVariablesOf(choice),
            Application application => FreeVariablesOf(application),
            Wrapper wrapper => FreeVariablesOf(wrapper),
            _ => Enumerable.Empty<Variable>()
        };
    }

    private static IEnumerable<Variable> FreeVariablesOf(Eqe eqe)
    {
        _ = FreeVariablesOf(eqe.Eq);

        return FreeVariablesOf(eqe.E);
    }

    private static IEnumerable<Variable> FreeVariablesOf(IExpressionOrEquation eq)
    {
        return eq switch
        {
            Expression e => FreeVariablesOf(e),
            Equation equation => FreeVariablesOf(equation),
            _ => throw new Exception($"Unable to match pattern of type: {eq.GetType()}")
        };
    }

    private static IEnumerable<Variable> FreeVariablesOf(Equation equation)
    {
        _ = FreeVariablesOf(equation.V);

        return FreeVariablesOf(equation.E);
    }

    private static IEnumerable<Variable> FreeVariablesOf(Lambda lambda)
    {
        _variableBuffer.BoundVariables = _variableBuffer.BoundVariables.Append(lambda.Parameter);

        return FreeVariablesOf(lambda.E);
    }

    private static IEnumerable<Variable> FreeVariablesOf(VerseTuple tuple)
    {
        foreach (Value value in tuple)
            _ = FreeVariablesOf(value);

        return _variableBuffer.FreeVariables;
    }

    private static IEnumerable<Variable> FreeVariablesOf(Variable variable)
    {
        if (!_variableBuffer.BoundVariables.Contains(variable) && !_variableBuffer.FreeVariables.Contains(variable))
            _variableBuffer.FreeVariables = _variableBuffer.FreeVariables.Append(variable);

        return _variableBuffer.FreeVariables;
    }

    private static IEnumerable<Variable> FreeVariablesOf(Application application)
    {
        _ = FreeVariablesOf(application.V1);

        return FreeVariablesOf(application.V2);
    }

    private static IEnumerable<Variable> FreeVariablesOf(Choice choice)
    {
        _ = FreeVariablesOf(choice.E1);

        return FreeVariablesOf(choice.E2);
    }

    private static IEnumerable<Variable> FreeVariablesOf(Exists exists)
    {
        _variableBuffer.BoundVariables = _variableBuffer.BoundVariables.Append(exists.V);

        return FreeVariablesOf(exists.E);
    }

    private static IEnumerable<Variable> FreeVariablesOf(Wrapper wrapper)
        => FreeVariablesOf(wrapper.E);
}
