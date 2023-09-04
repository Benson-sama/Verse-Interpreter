using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Verse_Interpreter.Model.Build;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

public class VerseSyntaxTreeBuilder : IVerseSyntaxTreeBuilder, IVerseVisitor<Expression>
{
    private readonly IVariableFactory _variableFactory;
    private readonly SyntaxDesugarer _syntaxDesugarer;

    public VerseSyntaxTreeBuilder(IVariableFactory variableFactory, SyntaxDesugarer desugar)
        => (_variableFactory, _syntaxDesugarer) = (variableFactory, desugar);

    public VerseProgram BuildCustomSyntaxTree(VerseParser.ProgramContext context, Func<Expression, Wrapper> wrapperFactory)
    {
        return new VerseProgram()
        {
            E = wrapperFactory(GetExpression(context.e()))
        };
    }

    public Expression VisitProgram([NotNull] VerseParser.ProgramContext context)
        => throw new NotSupportedException("Visit program context is not supported because of return type of Expression.");

    public Expression VisitIfElseExp([NotNull] VerseParser.IfElseExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));
        Expression e3 = GetExpression(context.e(2));

        Expression ifThenElseExpression = _syntaxDesugarer.IfThenElse(e1, e2, e3);

        if (context.e(3) is null)
            return ifThenElseExpression;

        return new Eqe
        {
            Eq = ifThenElseExpression,
            E = GetExpression(context.e(3))
        };
    }

    public Expression VisitFailExp([NotNull] VerseParser.FailExpContext context)
        => new Fail();

    public Expression VisitEqualityExp([NotNull] VerseParser.EqualityExpContext context)
    {
        Value v = GetValue(context.v());
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));

        return new Eqe
        {
            Eq = new Equation { V = v, E = e1 },
            E = e2
        };
    }

    public Expression VisitPlusOrMinusExp([NotNull] VerseParser.PlusOrMinusExpContext context)
    {
        Value v1 = GetValue(context.v(0));
        Value v2 = GetValue(context.v(1));
        Operator op;

        if (context.PLUS() is not null)
            op = new Add();
        else if (context.MINUS() is not null)
            op = new Sub();
        else
            throw new Exception("Invalid addition or subtraction expression.");

        return BuildArithmeticExpression(v1, op, v2);
    }

    public Expression VisitForExp([NotNull] VerseParser.ForExpContext context)
    {
        Expression e = GetExpression(context.e());

        return new All
        {
            E = e
        };
    }

    public Expression VisitAssignmentExp([NotNull] VerseParser.AssignmentExpContext context)
    {
        Variable x = GetVariable(context.VARIABLE());
        _variableFactory.RegisterUsedName(x.Name);
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));

        return SyntaxDesugarer.Assignment(x, e1, e2);
    }

    public Expression VisitForDoExp([NotNull] VerseParser.ForDoExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));

        return _syntaxDesugarer.ForDo(e1, e2);
    }

    public Expression VisitMultOrDivExp([NotNull] VerseParser.MultOrDivExpContext context)
    {
        Value v1 = GetValue(context.v(0));
        Value v2 = GetValue(context.v(1));
        Operator op;

        if (context.ASTERISK() is not null)
            op = new Mult();
        else if (context.SLASH() is not null)
            op = new Div();
        else
            throw new Exception("Invalid multiplication or division expression.");

        return BuildArithmeticExpression(v1, op, v2);
    }

    public Expression VisitChoiceExp([NotNull] VerseParser.ChoiceExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));

        return new Choice()
        {
            E1 = e1,
            E2 = e2
        };
    }

    public Expression VisitOneExp([NotNull] VerseParser.OneExpContext context)
    {
        Expression e = GetExpression(context.e());

        return new One
        {
            E = e
        };
    }

    public Expression VisitComparisonExp([NotNull] VerseParser.ComparisonExpContext context)
    {
        Value v1 = GetValue(context.v(0));
        Value v2 = GetValue(context.v(1));
        Operator op;

        if (context.GREATERTHAN() is not null)
            op = new Gt();
        else if (context.LESSTHAN() is not null)
            op = new Lt();
        else
            throw new Exception("Invalid comparison expression.");

        Expression arithmeticExpression = BuildArithmeticExpression(v1, op, v2);

        if (context.e() is null)
            return arithmeticExpression;

        return new Eqe
        {
            Eq = arithmeticExpression,
            E = GetExpression(context.e())
        };
    }

    public Expression VisitBringIntoScopeExp([NotNull] VerseParser.BringIntoScopeExpContext context)
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

    public Expression VisitParenthesisExp([NotNull] VerseParser.ParenthesisExpContext context)
        => GetExpression(context.e());

    public Expression VisitValueApplicationExp([NotNull] VerseParser.ValueApplicationExpContext context)
    {
        Value v1 = GetValue(context.v(0));
        Value v2 = GetValue(context.v(1));

        Application application = new()
        {
            V1 = v1,
            V2 = v2
        };

        if (context.e() is null)
            return application;

        return new Eqe
        {
            Eq = application,
            E = GetExpression(context.e())
        };
    }

    public Expression VisitValueExp([NotNull] VerseParser.ValueExpContext context)
        => context.v().Accept(this);

    public Expression VisitRangeChoiceExp([NotNull] VerseParser.RangeChoiceExpContext context)
    {
        Integer k1 = GetInteger(context.INTEGER(0));
        Integer k2 = GetInteger(context.INTEGER(1));

        if (k1.Value > k2.Value)
            throw new VerseParseException("Invalid range expression, the first value cannot be greater than the second.");

        return BuildIntegerChoiceRecursively(k1.Value, k2.Value);
    }

    public Expression VisitVariableValue([NotNull] VerseParser.VariableValueContext context)
        => GetVariable(context.VARIABLE());

    public Expression VisitHnfValue([NotNull] VerseParser.HnfValueContext context)
        => context.hnf().Accept(this);

    public Expression VisitIntegerHnf([NotNull] VerseParser.IntegerHnfContext context)
        => GetInteger(context.INTEGER());

    public Expression VisitStringHnf([NotNull] VerseParser.StringHnfContext context)
        => context.@string().Accept(this);

    public Expression VisitTupleHnf([NotNull] VerseParser.TupleHnfContext context)
        => context.tuple().Accept(this);

    public Expression VisitLambdaHnf([NotNull] VerseParser.LambdaHnfContext context)
        => context.lambda().Accept(this);

    public Expression VisitString([NotNull] VerseParser.StringContext context)
        => context.content().Accept(this);

    public Expression VisitContent([NotNull] VerseParser.ContentContext context)
        => new VerseString(context.GetText());

    public Expression VisitTuple([NotNull] VerseParser.TupleContext context)
    {
        VerseParser.ElementsContext elementsContext = context.elements();

        if (elementsContext is null)
            return VerseTuple.Empty;
        else
            return elementsContext.Accept(this);
    }

    public Expression VisitElements([NotNull] VerseParser.ElementsContext context)
        => new VerseTuple(GetTupleElements(context).ToArray());

    public Expression VisitLambda([NotNull] VerseParser.LambdaContext context)
    {
        VerseTuple tuple = GetTuple(context.tuple());

        IEnumerable<Variable> variableTuple = tuple.Select(
            v => v as Variable ?? throw new VerseParseException("Lambda parameters must be variables.")).ToArray();

        Expression e = GetExpression(context.e());

        return _syntaxDesugarer.Lambda(variableTuple, e);
    }

    public Expression Visit(IParseTree tree) => throw new NotSupportedException();

    public Expression VisitChildren(IRuleNode node) => throw new NotSupportedException();

    public Expression VisitTerminal(ITerminalNode node) => throw new NotSupportedException();

    public Expression VisitErrorNode(IErrorNode node) => throw new Exception(node.GetText());

    private Expression GetExpression(VerseParser.EContext context)
        => context.Accept(this);

    private Value GetValue(VerseParser.VContext context)
    {
        if (context.Accept(this) is not Value value)
            throw new VerseParseException($"Unable to parse value from context: {context}");

        return value;
    }

    private VerseTuple GetTuple(VerseParser.TupleContext context)
    {
        if (context.Accept(this) is not VerseTuple verseTuple)
            throw new VerseParseException($"Unable to parse verse tuple in context: {context}");

        return verseTuple;
    }

    private static Integer GetInteger(ITerminalNode terminalNode)
    {
        string integerText = terminalNode.GetText();

        try
        {
            return new Integer(int.Parse(integerText));
        }
        catch (Exception)
        {
            throw new VerseParseException($"Unable to parse integer with value: {integerText}");
        }
    }

    private static Variable GetVariable(ITerminalNode terminalNode)
    {
        string name = terminalNode.GetText();

        return new Variable(name);
    }

    private IEnumerable<Value> GetTupleElements(VerseParser.ElementsContext context)
    {
        if (context.ChildCount == 0)
            yield break;

        yield return GetValue(context.v());

        if (context.ChildCount == 1)
            yield break;

        foreach (Value value in GetTupleElements(context.elements(0)))
        {
            yield return value;
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

    private Expression BuildArithmeticExpression(Expression e1, Operator op, Expression e2)
    {
        if (e1 is Value v1 && e2 is Value v2)
        {
            if (v1 is Integer or Variable && v2 is Integer or Variable)
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

        return SyntaxDesugarer.Assignment(x1, e1, SyntaxDesugarer.Assignment(x2, e2, new VerseTuple(x1, x2)));
    }
}
