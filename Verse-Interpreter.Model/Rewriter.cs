using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;
using Tuple = Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Tuple;

namespace Verse_Interpreter.Model;

public class Rewriter : IRewriter
{
    private readonly IVerseLogger _logger;
    private readonly Func<Expression, Expression>[] _rewriteRules;

    public Rewriter(IVerseLogger logger)
    {
        _logger = logger;
        _rewriteRules = new Func<Expression, Expression>[]
        {
            // Application.
            TryRewriteWithAppAdd,
            TryRewriteWithAppGtAndAppGtFail,
            // Unification.
            TryRewriteWithULit,
            // Elimination.
            TryRewriteWithValElim,
            // Normalisation.
            TryRewriteWithExiSwap,
            // Choice.
            TryRewriteWithOneFail,
            TryRewriteWithOneValue
        };
    }

    public bool RuleApplied { get; private set; }

    public Expression TryRewrite(Expression expression)
    {
        RuleApplied = false;
        Expression rewrittenExpression = expression;

        foreach (Func<Expression, Expression> tryRewriteWithNextRule in _rewriteRules)
        {
            rewrittenExpression = tryRewriteWithNextRule(expression);

            if (RuleApplied)
                return rewrittenExpression;
        }

        return rewrittenExpression;
    }

    private Expression TryRewriteWithAppAdd(Expression expression)
    {
        if (expression is Application { V1: Add, V2: Tuple { Values: IEnumerable<Value> values } })
        {
            if (values.Count() is 2 && values.ElementAt(0) is Integer i1 && values.ElementAt(1) is Integer i2)
            {
                Expression rewrittenExpression = new Integer(i1.Value + i2.Value);
                _logger.LogRuleApplied("APP-ADD");
                RuleApplied = true;
                return rewrittenExpression;
            }
        }

        return expression;
    }

    private Expression TryRewriteWithAppGtAndAppGtFail(Expression expression)
    {
        if (expression is Application { V1: Gt, V2: Tuple { Values: IEnumerable<Value> values } })
        {
            if (values.Count() is 2 && values.ElementAt(0) is Integer i1 && values.ElementAt(1) is Integer i2)
            {
                if (i1.Value > i2.Value)
                {
                    Expression rewrittenExpression = i1;
                    RuleApplied = true;
                    _logger.LogRuleApplied("APP-GT");
                    return rewrittenExpression;
                }
                else
                {
                    Expression rewrittenExpression = new Fail();
                    RuleApplied = true;
                    _logger.LogRuleApplied("APP-GT-FAIL");
                    return rewrittenExpression;
                }
            }
        }

        return expression;
    }

    private Expression TryRewriteWithULit(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: Integer k1, E: Integer k2 }, E: Expression e })
        {
            if (k1.Value == k2.Value)
            {
                RuleApplied = true;
                _logger.LogRuleApplied("U-LIT");
                return e;
            }
        }

        return expression;
    }

    private Expression TryRewriteWithValElim(Expression expression)
    {
        if (expression is Eqe { Eq: Value, E: Expression e })
        {
            RuleApplied = true;
            _logger.LogRuleApplied("VAL-ELIM");
            return e;
        }

        return expression;
    }

    private Expression TryRewriteWithExiSwap(Expression expression)
    {
        if (expression is Exists { V: Variable, E: Exists { V: Variable, E: Expression e } existsY } existsX)
        {
            RuleApplied = true;
            _logger.LogRuleApplied("EXI-SWAP");
            return existsY.E = existsX.E = e;
        }

        return expression;
    }

    private Expression TryRewriteWithOneFail(Expression expression)
    {
        if (expression is One { E: Fail fail })
        {
            RuleApplied = true;
            _logger.LogRuleApplied("ONE-FAIL");
            return fail;
        }

        return expression;
    }

    private Expression TryRewriteWithOneValue(Expression expression)
    {
        if (expression is One { E: Value value })
        {
            RuleApplied = true;
            _logger.LogRuleApplied("ONE-VALUE");
            return value;
        }

        return expression;
    }

    //[Obsolete]
    //private RewriteResult TryRewriteAppAddWithSwitch(Expression expression)
    //{
    //    return expression switch
    //    {
    //        Application { V1: Add, V2: Tuple { Values: IEnumerable<Value> values } }
    //            when values.Count() is 2 && values.ElementAt(0) is Integer i1 && values.ElementAt(1) is Integer i2 =>
    //                new RewriteResult(RuleApplied: true, RewrittenExpression: new Integer(i1.Value + i2.Value), AppliedRuleName: "APP-ADD"),
    //        _ => new RewriteResult(RuleApplied: false, expression)
    //    };
    //}
}
