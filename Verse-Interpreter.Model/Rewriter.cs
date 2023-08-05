using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Tuple = Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Tuple;

namespace Verse_Interpreter.Model;

public class Rewriter : IRewriter
{
    private readonly Func<Expression, Expression>[] _rewriteRules;

    public Rewriter()
    {
        _rewriteRules = new Func<Expression, Expression>[]
        {
            TryRewriteWithAppAdd,
            TryRewriteWithAppGtAndAppGtFail
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
                // TODO: Output ~APP-ADD
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
                    // TODO: Output ~APP-GT
                    return rewrittenExpression;
                }
                else
                {
                    Expression rewrittenExpression = new Fail();
                    RuleApplied = true;
                    // TODO: Output ~APP-GT-FAIL
                    return rewrittenExpression;
                }
            }
        }

        return expression;
    }

    [Obsolete]
    private RewriteResult TryRewriteAppAddWithSwitch(Expression expression)
    {
        return expression switch
        {
            Application { V1: Add, V2: Tuple { Values: IEnumerable<Value> values } }
                when values.Count() is 2 && values.ElementAt(0) is Integer i1 && values.ElementAt(1) is Integer i2 =>
                    new RewriteResult(RuleApplied: true, RewrittenExpression: new Integer(i1.Value + i2.Value), AppliedRuleName: "APP-ADD"),
            _ => new RewriteResult(RuleApplied: false, expression)
        };
    }
}
