using Antlr4.Runtime.Tree;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model;

public class VerseSyntaxTreeBuilder : IVerseSyntaxTreeBuilder
{
    private readonly IVariableFactory _variableFactory;
    private readonly Desugar _desugar;

    public VerseSyntaxTreeBuilder(IVariableFactory variableFactory, Desugar desugar)
        => (_variableFactory, _desugar) = (variableFactory, desugar);

    public VerseProgram BuildCustomSyntaxTree(VerseParser.ProgramContext context, Func<Expression, Wrapper> wrapperFactory)
    {
        return new VerseProgram()
        {
            Wrapper = wrapperFactory(GetExpression(context.e()))
        };
    }

    private Expression GetExpression(VerseParser.EContext context) => context switch
    {
        VerseParser.ParenthesisExpContext c => GetExpression(c.e()),
        VerseParser.BringIntoScopeExpContext c => GetConcreteExpression(c),
        VerseParser.MultExpContext c => GetConcreteExpression(c),
        VerseParser.DivExpContext c => GetConcreteExpression(c),
        VerseParser.PlusExpContext c => GetConcreteExpression(c),
        VerseParser.MinusExpContext c => GetConcreteExpression(c),
        VerseParser.GreaterThanExpContext c => GetConcreteExpression(c),
        VerseParser.LessThanExpContext c => GetConcreteExpression(c),
        VerseParser.AssignmentExpContext c => GetConcreteExpression(c),
        VerseParser.ValueExpContext c => GetValue(c.v()),
        VerseParser.ExpTupleExpContext c => GetExpressionTuple(c),
        VerseParser.FailExpContext => new Fail(),
        VerseParser.RangeChoiceExpContext c => GetConcreteExpression(c),
        VerseParser.ChoiceExpContext c => GetConcreteExpression(c),
        VerseParser.ExpApplicationExpContext c => GetConcreteExpression(c),
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

        if (e1 is Integer i1 && e2 is Integer i2)
        {
            return new Application
            {
                V1 = new Add(),
                V2 = new VerseTuple(i1, new Integer(-i2.Value))
            };
        }

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

    private Expression GetConcreteExpression(VerseParser.BringIntoScopeExpContext context)
    {
        Variable variable = GetVariable(context.VARIABLE());
        _variableFactory.RegisterUsedName(variable.Name);
        Expression e = GetExpression(context.e());

        return new Exists
        {
            V = variable,
            E = e
        };
    }

    private Expression GetConcreteExpression(VerseParser.AssignmentExpContext context)
    {
        Variable x = GetVariable(context.VARIABLE());
        _variableFactory.RegisterUsedName(x.Name);
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));

        return Desugar.Assignment(x, e1, e2);
    }

    private static Expression GetConcreteExpression(VerseParser.RangeChoiceExpContext context)
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

    // TODO: Implement.
    private Value GetExpressionTuple(VerseParser.ExpTupleExpContext c)
    {
        throw new NotImplementedException();
    }

    private Expression GetConcreteExpression(VerseParser.ExpApplicationExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));

        if (e1 is Value v1 && e2 is Value v2)
        {
            return new Application
            {
                V1 = v1,
                V2 = v2
            };
        }

        Variable f = _variableFactory.Next();
        Variable x = _variableFactory.Next();

        Application application = new()
        {
            V1 = f,
            V2 = x
        };

        return Desugar.Assignment(f, e1, Desugar.Assignment(x, e2, application));
    }

    private Expression GetConcreteExpression(VerseParser.IfElseExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));
        Expression e3 = GetExpression(context.e(2));

        return _desugar.IfThenElse(e1, e2, e3);
    }

    // TODO: Implement.
    private Expression GetConcreteExpression(VerseParser.ForExpContext context)
    {
        throw new NotImplementedException();
    }

    private Expression GetConcreteExpression(VerseParser.EqeExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));
        Expression e3 = GetExpression(context.e(2));

        if (e1 is Value v)
        {
            return new Eqe
            {
                Eq = new Equation { V = v, E = e2 },
                E = e3
            };
        }

        return new Eqe()
        {
            Eq = _desugar.ExpressionEquation(e1, e2),
            E = e2
        };
    }

    private Value GetValue(VerseParser.VContext context)
    {
        return context switch
        {
            VerseParser.VariableValueContext c => GetVariable(c.VARIABLE()),
            VerseParser.HnfValueContext c => GetHeadNormalForm(c.hnf()),
            { } => throw new Exception("Unable to match value context."),
            _ => throw new ArgumentNullException(nameof(context), "Cannot be null.")
        };
    }

    private VerseTuple GetTuple(VerseParser.TupleContext context)
    {
        if (context.ChildCount is 0)
            return VerseTuple.Empty;
        else
            return new VerseTuple(GetTupleElements(context.elements()).ToArray());
    }

    private IEnumerable<Value> GetTupleElements(VerseParser.ElementsContext context)
    {
        if (context.ChildCount is 0)
            yield break;

        yield return GetValue(context.v());

        if (context.ChildCount is 1)
            yield break;

        foreach (Value value in GetTupleElements(context.elements(0)))
        {
            yield return value;
        }
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
        VerseTuple tuple = GetTuple(context.tuple());

        IEnumerable<Variable> variableTuple = tuple.Select(
            v => v as Variable ?? throw new Exception("Lambda parameters must be variables.")).ToArray();

        Expression e = GetExpression(context.e());

        return _desugar.Lambda(variableTuple, e);
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

    private static Variable GetVariable(ITerminalNode terminalNode)
    {
        string name = terminalNode.GetText();

        return new Variable(name);
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

    private Expression BuildArithmeticExpression(Expression e1, Operator op, Expression e2)
    {
        if (e1 is Value v1 && e2 is Value v2)
        {
            if ((v1 is Integer or Variable) && (v2 is Integer or Variable))
            {
                return new Application
                {
                    V1 = op,
                    V2 = new VerseTuple(v1, v2)
                };
            }
        }

        Variable x1 = _variableFactory.Next();
        Variable x2 = _variableFactory.Next();

        return Desugar.Assignment(x1, e1, Desugar.Assignment(x2, e2, new VerseTuple(x1, x2)));
    }
}
