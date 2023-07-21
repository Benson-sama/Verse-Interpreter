using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;
using Tuple = Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Tuple;

namespace Verse_Interpreter.Model;

public static class Desugar
{
    public static Expression Plus(Expression e1, Expression e2) =>
        ExpressionApplication(new Add(), ExpressionTuple(new Expression[] { e1, e2 }));

    public static Expression GreaterThan(Expression e1, Expression e2) =>
        ExpressionApplication(new Gt(), ExpressionTuple(new Expression[] { e1, e2 }));

    public static Expression MultipleExists(IEnumerable<Variable> variables, Expression e)
    {
        if (variables.Count() is 0)
            return e;

        return new Exists
        {
            V = variables.First(),
            E = MultipleExists(variables.Skip(1), e)
        };
    }

    public static Eqe Assignment(Variable x, Expression e1, Expression e2)
    {
        return new Eqe
        {
            Eq = new Exists
            {
                V = x,
                E = new Equation
                {
                    V = x,
                    E = e1
                }
            },
            E = e2
        };
    }

    public static Expression ExpressionApplication(Expression e1, Expression e2)
    {
        // TODO: Ensure freshness!
        Variable f = new(Guid.NewGuid().ToString());
        Variable x = new(Guid.NewGuid().ToString());

        Application application = new()
        {
            V1 = f,
            V2 = x
        };

        return Assignment(f, e1, Assignment(x, e2, application));
    }

    public static Eqe ExpressionTuple(IEnumerable<Expression> expressions)
    {
        throw new NotImplementedException();
    }

    public static Expression ExpressionEquation(Expression e1, Expression e2)
    {
        // TODO: Ensure freshness!
        Variable x = new(Guid.NewGuid().ToString());
        Eqe eqe = new()
        {
            Eq = new Equation
            {
                V = x,
                E = e2
            },
            E = x
        };

        return Assignment(x, e1, eqe);
    }

    public static Lambda Lambda(IEnumerable<Variable> parameters, Expression e)
    {
        if (parameters.Count() is 0)
            throw new Exception("Cannot desugar lambda with zero parameters.");

        // TODO: Ensure freshness!
        Variable p = new(Guid.NewGuid().ToString());

        Eqe eqe = new()
        {
            Eq = new Equation
            {
                V = p,
                E = new Tuple { Values = parameters }
            },
            E = e
        };

        return new Lambda
        {
            Parameters = parameters,
            E = MultipleExists(parameters, eqe)
        };
    }

    public static Expression IfThenElse(Expression e1, Expression e2, Expression e3)
    {
        One one = new()
        {
            E = new Choice
            {
                E1 = new Eqe
                {
                    Eq = e1,
                    E = new Lambda
                    {
                        Parameters = Enumerable.Empty<Variable>(),
                        E = e2
                    }
                },
                E2 = new Lambda
                {
                    Parameters = Enumerable.Empty<Variable>(),
                    E = e3
                }
            }
        };

        return ExpressionApplication(one, Tuple.Empty);
    }
}
