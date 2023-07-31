using Antlr4.Runtime.Tree;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;
using Tuple = Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Tuple;

namespace Verse_Interpreter.Model;

public class VerseSyntaxTreeBuilder : IVerseSyntaxTreeBuilder
{
    public VerseProgram BuildCustomSyntaxTreeWrappedInOne(VerseParser.ProgramContext context)
    {
        return new VerseProgram()
        {
            Wrapper = new One()
            {
                E = GetExpression(context.e())
            }
        };
    }

    public VerseProgram BuildCustomSyntaxTreeWrappedInAll(VerseParser.ProgramContext context)
    {
        return new VerseProgram()
        {
            Wrapper = new All()
            {
                E = GetExpression(context.e())
            }
        };
    }

    private Expression GetExpression(VerseParser.EContext context) => context switch
    {
        VerseParser.ParenthesisExpContext c => GetExpression(c.e()),
        VerseParser.MultExpContext c => GetConcreteExpression(c),
        VerseParser.DivExpContext c => GetConcreteExpression(c),
        VerseParser.PlusExpContext c => GetConcreteExpression(c),
        VerseParser.MinusExpContext c => GetConcreteExpression(c),
        VerseParser.GreaterThanExpContext c => GetConcreteExpression(c),
        VerseParser.LessThanExpContext c => GetConcreteExpression(c),
        VerseParser.AssignmentExpContext c => GetConcreteExpression(c),
        VerseParser.ValueExpContext c => GetValue(c.v()),
        VerseParser.FailExpContext => new Fail(),
        VerseParser.RangeChoiceExpContext c => GetConcreteExpression(c),
        VerseParser.ChoiceExpContext c => GetConcreteExpression(c),
        VerseParser.ValueApplicationExpContext c => GetConcreteExpression(c),
        VerseParser.ExpApplicationExpContext c => GetConcreteExpression(c),
        VerseParser.ExpEquationExpContext c => GetConcreteExpression(c),
        VerseParser.IfElseExpContext c => GetConcreteExpression(c),
        VerseParser.ForExpContext c => GetConcreteExpression(c),
        VerseParser.EqeExpContext c => GetConcreteExpression(c),
        { } => throw new Exception($"Unable to match context type: {context.GetType()}"),
        _ => throw new ArgumentNullException(nameof(context), "Cannot be null or empty.")
    };

    private Expression GetConcreteExpression(VerseParser.MultExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));
        Operator mult = new Mult();

        return BuildArithmeticExpression(e1, mult, e2);
    }

    private Expression GetConcreteExpression(VerseParser.DivExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));
        Operator div = new Div();

        return BuildArithmeticExpression(e1, div, e2);
    }

    private Expression GetConcreteExpression(VerseParser.PlusExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));
        Operator add = new Add();

        return BuildArithmeticExpression(e1, add, e2);
    }

    private Expression GetConcreteExpression(VerseParser.MinusExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));
        Operator sub = new Sub();

        return BuildArithmeticExpression(e1, sub, e2);
    }

    private Expression GetConcreteExpression(VerseParser.GreaterThanExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));
        Operator greaterThan = new Gt();

        return BuildArithmeticExpression(e1, greaterThan, e2);
    }

    private Expression GetConcreteExpression(VerseParser.LessThanExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));
        Operator lessThan = new Lt();

        return BuildArithmeticExpression(e1, lessThan, e2);
    }

    private Expression GetConcreteExpression(VerseParser.AssignmentExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Variable x = new(context.VARIABLE().GetText());
        Exists exists = new()
        {
            V = x,
            E = new Equation()
            {
                V = x,
                E = e1
            }
        };

        if (context.ChildCount is 3)
            return exists;

        Expression e2 = GetExpression(context.e(1));
        return new Eqe()
        {
            Eq = exists,
            E = e2
        };
    }

    private Expression GetConcreteExpression(VerseParser.RangeChoiceExpContext context)
    {
        int i1 = GetInteger(context.INTEGER(0));
        int i2 = GetInteger(context.INTEGER(1));

        if (i1 > i2)
            throw new Exception("Invalid range expression, the first value cannot be greater than the second.");

        return BuildIntegerChoiceRecursively(i1, i2);
    }

    private Expression GetConcreteExpression(VerseParser.ChoiceExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));

        return new Choice()
        {
            E1 = e1,
            E2 = e2
        };
    }

    private Expression GetConcreteExpression(VerseParser.ValueApplicationExpContext context)
    {
        Value v1 = GetValue(context.v(0));
        Value v2 = GetValue(context.v(1));

        return new Application()
        {
            V1 = v1,
            V2 = v2
        };
    }

    // TODO: Ensure freshness!
    private Expression GetConcreteExpression(VerseParser.ExpApplicationExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));
        Variable f = new(Guid.NewGuid().ToString());
        Variable x = new(Guid.NewGuid().ToString());

        return new Eqe
        {
            Eq = new Exists
            {
                V = f,
                E = new Equation { V = f, E = e1 }
            },
            E = new Eqe
            {
                Eq = new Exists
                {
                    V = x,
                    E = new Equation { V = x, E = e2 }
                },
                E = new Application { V1 = f, V2 = x }
            }
        };
    }

    // TODO: Ensure freshness!
    private Expression GetConcreteExpression(VerseParser.ExpEquationExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));
        Variable x = new(Guid.NewGuid().ToString());

        return new Eqe
        {
            Eq = new Exists
            {
                V = x,
                E = new Equation { V = x, E = e1 }
            },
            E = new Eqe
            {
                Eq = new Equation { V = x, E = e2 },
                E = x
            }
        };
    }

    private Expression GetConcreteExpression(VerseParser.IfElseExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));
        Expression e3 = GetExpression(context.e(2));

        return Desugar.IfThenElse(e1, e2, e3);
    }

    // TODO: Implement.
    private Expression GetConcreteExpression(VerseParser.ForExpContext context)
    {
        throw new NotImplementedException();
    }

    private Expression GetConcreteExpression(VerseParser.EqeExpContext context)
    {
        Value v = GetValue(context.v());
        Expression e1 = GetExpression(context.e(0));
        Equation eq = new()
        {
            V = v,
            E = e1
        };

        if (context.ChildCount is 3)
            return eq;

        Expression e2 = GetExpression(context.e(1));
        return new Eqe()
        {
            Eq = eq,
            E = e2
        };
    }

    private Value GetValue(VerseParser.VContext context)
    {
        return context switch
        {
            VerseParser.VariableValueContext c => new Variable(c.VARIABLE().GetText()),
            VerseParser.HnfValueContext c => GetHeadNormalForm(c.hnf()),
            { } => throw new Exception("Unable to match value context."),
            _ => throw new ArgumentNullException(nameof(context), "Cannot be null.")
        };
    }

    // TODO: Implement Tuple parsing.
    private Tuple GetTuple(VerseParser.TupleContext context)
    {
        return new Tuple() { Values = Array.Empty<Tuple>() };
    }

    private HeadNormalForm GetHeadNormalForm(VerseParser.HnfContext context)
    {
        return context switch
        {
            VerseParser.IntegerHnfContext c => new Integer(GetInteger(c.INTEGER())),
            VerseParser.TupleHnfContext c => GetTuple(c.tuple()),
            VerseParser.LambdaHnfContext c => GetLambda(c.lambda()),
            { } => throw new Exception("Unable to match value context."),
            _ => throw new ArgumentNullException(nameof(context), "Cannot be null.")
        };
    }

    private Lambda GetLambda(VerseParser.LambdaContext context)
    {
        Tuple tuple = GetTuple(context.tuple());

        if (tuple.Values is not IEnumerable<Variable> variables)
            throw new Exception("Lambda parameters must be variables.");

        Expression e = GetExpression(context.e());

        return Desugar.Lambda(variables, e);
    }

    private static int GetInteger(ITerminalNode terminalNode)
    {
        try
        {
            return int.Parse(terminalNode.GetText());
        }
        catch (Exception)
        {
            throw new Exception($"Unable to parse integer with value: {terminalNode.GetText()}");
        }
    }

    private static Expression BuildIntegerChoiceRecursively(int min, int max)
    {
        if (min == max)
            return new Integer(max);

        return new Choice()
        {
            E1 = new Integer(min),
            E2 = BuildIntegerChoiceRecursively(min + 1, max)
        };
    }

    // TODO: Ensure freshness!
    private static Exists BuildArithmeticExpression(Expression e1, Operator op, Expression e2)
    {
        Variable v1 = new(Guid.NewGuid().ToString());
        Variable v2 = new(Guid.NewGuid().ToString());

        return new Exists()
        {
            V = v1,
            E = new Eqe()
            {
                Eq = new Equation()
                {
                    V = v1,
                    E = e1
                },
                E = new Exists()
                {
                    V = v2,
                    E = new Eqe()
                    {
                        Eq = new Equation()
                        {
                            V = v2,
                            E = e2
                        },
                        E = new Application()
                        {
                            V1 = op,
                            V2 = new Tuple()
                            {
                                Values = new Value[]
                                {
                                    v1,
                                    v2
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
