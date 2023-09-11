//--------------------------------------------------------------------------
// <copyright file="VerseSyntaxTreeBuilder.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the VerseSyntaxTreeBuilder class.</summary>
//--------------------------------------------------------------------------

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

/// <summary>
/// Class <see cref="VerseSyntaxTreeBuilder"/> serves as an <see cref="IVerseVisitor{Expression}"/>
/// to convert and desugar ANTLR4 parse trees.
/// </summary>
public class VerseSyntaxTreeBuilder : IVerseSyntaxTreeBuilder, IVerseVisitor<Expression>
{
    /// <summary>
    /// Field <c>_variableFactory</c> represents the factory used to retrieve fresh variables.
    /// </summary>
    private readonly IVariableFactory _variableFactory;

    /// <summary>
    /// Field <c>_syntaxDesugarer</c> represents the component used for various desugaring rules.
    /// </summary>
    private readonly SyntaxDesugarer _syntaxDesugarer;

    /// <summary>
    /// Initialises a new instance of the <see cref="VerseSyntaxTreeBuilder"/> class.
    /// </summary>
    /// <param name="variableFactory"><c>variableFactory</c> represents the factory used to retrieve fresh variables.</param>
    /// <param name="syntaxDesugarer"><c>syntaxDesugarer</c> represents the component used for various desugaring rules.</param>
    public VerseSyntaxTreeBuilder(IVariableFactory variableFactory, SyntaxDesugarer syntaxDesugarer)
        => (_variableFactory, _syntaxDesugarer) = (variableFactory, syntaxDesugarer);

    /// <summary>
    /// This method converts and desugars the ANTLR4 <paramref name="context"/>
    /// and places the resulting <see cref="Expression"/> in the <see cref="Wrapper"/> from the <paramref name="wrapperFactory"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree.</param>
    /// <param name="wrapperFactory"><c>wrapperFactory</c> represents the factory used to get the <see cref="Wrapper"/>.</param>
    /// <returns>The resulting <see cref="VerseProgram"/> of the conversion and desugaring.</returns>
    public VerseProgram BuildCustomSyntaxTree(VerseParser.ProgramContext context, Func<Expression, Wrapper> wrapperFactory)
    {
        return new VerseProgram()
        {
            E = wrapperFactory(GetExpression(context.e()))
        };
    }

    /// <summary>
    /// This method is not supported as the generic type <see cref="Expression"/> is not compatible with <see cref="VerseProgram"/>.
    /// </summary>
    /// <param name="_"><c>_</c> represents the unused <see cref="VerseParser.ProgramContext"/>.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="NotSupportedException">Is always raised.</exception>
    public Expression VisitProgram([NotNull] VerseParser.ProgramContext _)
        => throw new NotSupportedException("Visit program context is not supported because of return type of Expression.");

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitIfElseExp([NotNull] VerseParser.IfElseExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));
        Expression e3 = GetExpression(context.e(2));

        Expression ifThenElseExpression = _syntaxDesugarer.DesugarIfThenElse(e1, e2, e3);

        if (context.e(3) is null)
            return ifThenElseExpression;

        return new Eqe
        {
            Eq = ifThenElseExpression,
            E = GetExpression(context.e(3))
        };
    }

    /// <summary>
    /// This method creates a new <see cref="Fail"/> instance.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    /// <returns>Always a new instance of the <see cref="Fail"/> class.</returns>
    public Expression VisitFailExp([NotNull] VerseParser.FailExpContext _)
        => new Fail();

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
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

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    /// <exception cref="Exception">Is raised when the Plus and Minus terminal nodes of <paramref name="context"/> are null.</exception>
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

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitForExp([NotNull] VerseParser.ForExpContext context)
    {
        Expression e = GetExpression(context.e());

        return new All
        {
            E = e
        };
    }

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitAssignmentExp([NotNull] VerseParser.AssignmentExpContext context)
    {
        Variable x = GetVariable(context.VARIABLE());
        _variableFactory.RegisterUsedName(x.Name);
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));

        return SyntaxDesugarer.DesugarAssignment(x, e1, e2);
    }

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitForDoExp([NotNull] VerseParser.ForDoExpContext context)
    {
        Expression e1 = GetExpression(context.e(0));
        Expression e2 = GetExpression(context.e(1));

        return _syntaxDesugarer.DesugarForDo(e1, e2);
    }

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    /// <exception cref="Exception">Is raised when the Mult and Div terminal nodes of <paramref name="context"/> are null.</exception>
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

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
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

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitOneExp([NotNull] VerseParser.OneExpContext context)
    {
        Expression e = GetExpression(context.e());

        return new One
        {
            E = e
        };
    }

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    /// <exception cref="Exception">Is raised when the Gt and Lt terminal nodes of <paramref name="context"/> are null.</exception>
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

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
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

    /// <summary>
    /// This method will convert and desugar child of <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitParenthesisExp([NotNull] VerseParser.ParenthesisExpContext context)
        => GetExpression(context.e());

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
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

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitValueExp([NotNull] VerseParser.ValueExpContext context)
        => context.v().Accept(this);

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    /// <exception cref="VerseParseException">Is raised when the first integer is bigger than the second.</exception>
    public Expression VisitRangeChoiceExp([NotNull] VerseParser.RangeChoiceExpContext context)
    {
        Integer k1 = GetInteger(context.INTEGER(0));
        Integer k2 = GetInteger(context.INTEGER(1));

        if (k1.Value > k2.Value)
            throw new VerseParseException("Invalid range expression, the first value cannot be greater than the second.");

        return BuildIntegerChoiceRecursively(k1.Value, k2.Value);
    }

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitVariableValue([NotNull] VerseParser.VariableValueContext context)
        => GetVariable(context.VARIABLE());

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitHnfValue([NotNull] VerseParser.HnfValueContext context)
        => context.hnf().Accept(this);

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitIntegerHnf([NotNull] VerseParser.IntegerHnfContext context)
        => GetInteger(context.INTEGER());

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitStringHnf([NotNull] VerseParser.StringHnfContext context)
        => context.@string().Accept(this);

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitTupleHnf([NotNull] VerseParser.TupleHnfContext context)
        => context.tuple().Accept(this);

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitLambdaHnf([NotNull] VerseParser.LambdaHnfContext context)
        => context.lambda().Accept(this);

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitString([NotNull] VerseParser.StringContext context)
        => context.content().Accept(this);

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitContent([NotNull] VerseParser.ContentContext context)
        => new VerseString(context.GetText());

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitTuple([NotNull] VerseParser.TupleContext context)
    {
        VerseParser.ElementsContext elementsContext = context.elements();

        if (elementsContext is null)
            return VerseTuple.Empty;
        else
            return elementsContext.Accept(this);
    }

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    public Expression VisitElements([NotNull] VerseParser.ElementsContext context)
        => new VerseTuple(GetTupleElements(context).ToArray());

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    /// <exception cref="VerseParseException">Is raised when the tuple contains non-variable values.</exception>
    public Expression VisitLambda([NotNull] VerseParser.LambdaContext context)
    {
        VerseTuple tuple = GetTuple(context.tuple());

        IEnumerable<Variable> variableTuple = tuple.Select(
            v => v as Variable ?? throw new VerseParseException("Lambda parameters must be variables.")).ToArray();

        Expression e = GetExpression(context.e());

        return _syntaxDesugarer.DesugarLambda(variableTuple, e);
    }

    /// <summary>
    /// This method is not supported.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="NotSupportedException">Is always raised.</exception>
    public Expression Visit(IParseTree _) => throw new NotSupportedException();

    /// <summary>
    /// This method is not supported.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="NotSupportedException">Is always raised.</exception>
    public Expression VisitChildren(IRuleNode _) => throw new NotSupportedException();

    /// <summary>
    /// This method is not supported.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="NotSupportedException">Is always raised.</exception>
    public Expression VisitTerminal(ITerminalNode _) => throw new NotSupportedException();

    /// <summary>
    /// This method is not supported.
    /// </summary>
    /// <param name="node"><c>node</c> represents the source for the error text.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="Exception">Is always raised with the error text.</exception>
    public Expression VisitErrorNode(IErrorNode node) => throw new Exception(node.GetText());

    /// <summary>
    /// This method will convert and desugar the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The resulting <see cref="Expression"/> of the conversion and desugaring.</returns>
    private Expression GetExpression(VerseParser.EContext context)
        => context.Accept(this);

    /// <summary>
    /// This method will make the <paramref name="context"/> accept this <see cref="IVerseVisitor{Result}"/>
    /// and return the result of it.
    /// </summary>
    /// <param name="context"><c>context</c> represents the <see cref="VerseParser.VContext"/> to accept
    /// this <see cref="IVerseVisitor{Result}"/>.</param>
    /// <returns>The result of the visitor handshake as a <see cref="Value"/>.</returns>
    /// <exception cref="VerseParseException">Is raised when the result is not a <see cref="Value"/>.</exception>
    private Value GetValue(VerseParser.VContext context)
    {
        if (context.Accept(this) is not Value value)
            throw new VerseParseException($"Unable to parse value from context: {context}");

        return value;
    }

    /// <summary>
    /// This method will make the <paramref name="context"/> accept this <see cref="IVerseVisitor{Result}"/>
    /// and return the result of it.
    /// </summary>
    /// <param name="context"><c>context</c> represents the <see cref="VerseParser.TupleContext"/> to accept
    /// this <see cref="IVerseVisitor{Result}"/>.</param>
    /// <returns>The result of the visitor handshake as a <see cref="VerseTuple"/>.</returns>
    /// <exception cref="VerseParseException">Is raised when the result is not a <see cref="VerseTuple"/>.</exception>
    private VerseTuple GetTuple(VerseParser.TupleContext context)
    {
        if (context.Accept(this) is not VerseTuple verseTuple)
            throw new VerseParseException($"Unable to parse verse tuple in context: {context}");

        return verseTuple;
    }

    /// <summary>
    /// This method tries to parse an <see cref="Integer"/> from the <paramref name="terminalNode"/>.
    /// </summary>
    /// <param name="terminalNode"><c>terminalNode</c> represents the ANTLR4 parse tree context
    /// used to get the value of the <see cref="Integer"/>.</param>
    /// <returns>The parsed <see cref="Integer"/>.</returns>
    /// <exception cref="VerseParseException">
    /// Is raised if the text of the <paramref name="terminalNode"/> is not a well formated
    /// <see cref="int"/> between <see cref="int.MinValue"/> and <see cref="int.MaxValue"/>.
    /// </exception>
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

    /// <summary>
    /// This method parses a <see cref="Variable"/> from the <paramref name="terminalNode"/>.
    /// </summary>
    /// <param name="terminalNode"><c>terminalNode</c> represents the ANTLR4 parse tree context
    /// used to get the name of the <see cref="Variable"/>.</param>
    /// <returns>The parsed <see cref="Variable"/>.</returns>
    private static Variable GetVariable(ITerminalNode terminalNode)
    {
        string name = terminalNode.GetText();

        return new Variable(name);
    }

    /// <summary>
    /// This method recursively retrieves the values of the <paramref name="context"/>.
    /// </summary>
    /// <param name="context"><c>context</c> represents the ANTLR4 parse tree context used to retrieve child expressions.</param>
    /// <returns>The retrieved values.</returns>
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

    /// <summary>
    /// This method builds a recursive <see cref="Choice"/> of consisting solely of <see cref="Integer"/>.
    /// </summary>
    /// <param name="min"><c>min</c> represents the lower bound of the <see cref="Choice"/> range.</param>
    /// <param name="max"><c>max</c> represents the upper bound of the <see cref="Choice"/> range.</param>
    /// <returns></returns>
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

    /// <summary>
    /// This method builds a simple arithmetic <see cref="Expression"/> if both <paramref name="e1"/>
    /// and <paramref name="e2"/> consist of <see cref="Integer"/> or <see cref="Variable"/>
    /// and builds a desugared arithmetic <see cref="Expression"/> otherwise.
    /// </summary>
    /// <param name="e1"><c>e1</c> represents the first arithmetic operand.</param>
    /// <param name="op"><c>op</c> represents the operator to use in the arithmetic <see cref="Expression"/>.</param>
    /// <param name="e2"><c>e2</c> represents the second arithmetic operand.</param>
    /// <returns></returns>
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

        return SyntaxDesugarer.DesugarAssignment(x1, e1, SyntaxDesugarer.DesugarAssignment(x2, e2, new VerseTuple(x1, x2)));
    }
}
