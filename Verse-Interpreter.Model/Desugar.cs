﻿using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model;

public class Desugar
{
    private readonly IVariableFactory _variableFactory;

    public Desugar(IVariableFactory variableFactory)
        => _variableFactory = variableFactory;

    public Expression Plus(Expression e1, Expression e2) =>
        ExpressionApplication(new Add(), ExpressionTuple(new Expression[] { e1, e2 }));

    public Expression GreaterThan(Expression e1, Expression e2) =>
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

    public static Exists Assignment(Variable x, Expression e1, Expression e2)
    {
        return new Exists
        {
            V = x,
            E = new Eqe
            {
                Eq = new Equation
                {
                    V = x,
                    E = e1
                },
                E = e2
            }
        };
    }

    public Expression ExpressionApplication(Expression e1, Expression e2)
    {
        Variable f = _variableFactory.Next();
        Variable x = _variableFactory.Next();

        Application application = new()
        {
            V1 = f,
            V2 = x
        };

        return Assignment(f, e1, Assignment(x, e2, application));
    }

    public Exists ExpressionTuple(IEnumerable<Expression> expressions)
    {
        if (expressions.Count() is 0)
            throw new Exception("Unable to parse empty expression tuple.");

        return BuildExpressionTupleRecursively(expressions, Enumerable.Empty<Variable>());
    }

    public Expression ExpressionEquation(Expression e1, Expression e2)
    {
        Variable x = _variableFactory.Next();
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

    public Lambda Lambda(IEnumerable<Variable> parameters, Expression e)
    {
        if (parameters.Count() is < 0)
            throw new Exception("Cannot desugar lambda with less than zero parameters.");

        Variable p = _variableFactory.Next();

        Eqe eqe = new()
        {
            Eq = new Equation
            {
                V = p,
                E = new VerseTuple(parameters)
            },
            E = e
        };

        return new Lambda
        {
            Parameter = p,
            E = MultipleExists(parameters, eqe)
        };
    }

    public Expression IfThenElse(Expression e1, Expression e2, Expression e3)
    {
        One one = new()
        {
            E = new Choice
            {
                E1 = new Eqe
                {
                    Eq = e1,
                    E = Lambda(Enumerable.Empty<Variable>(), e2)
                },
                E2 = Lambda(Enumerable.Empty<Variable>(), e3)
            }
        };

        return ExpressionApplication(one, VerseTuple.Empty);
    }

    private Exists BuildExpressionTupleRecursively(IEnumerable<Expression> expressions, IEnumerable<Variable> variables)
    {
        Variable x = _variableFactory.Next();

        if (expressions.Count() is 1)
            return Assignment(x, expressions.First(), new VerseTuple(variables.Append(x)));

        return Assignment(x, expressions.First(), BuildExpressionTupleRecursively(expressions.Skip(1), variables.Append(x)));
    }
}
