using Verse_Interpreter.Model.SyntaxTree;
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
    private readonly IRenderer _logger;
    private readonly Func<Expression, Expression>[] _rewriteRules;

    public Rewriter(IRenderer logger)
    {
        _logger = logger;
        _rewriteRules = new Func<Expression, Expression>[]
        {
            // Application.
            AppAdd,
            AppGtAndAppGtFail,
            // Unification.
            ULit,
            // Elimination.
            ValElim,
            // Normalisation.
            ExiSwap,
            // Choice.
            OneFail,
            OneValue
        };
    }

    private bool RuleApplied { get; set; }

    public Expression Rewrite(VerseProgram verseProgram)
    {
        do
        {
            verseProgram.Wrapper.E = ApplyRules(verseProgram.Wrapper.E);

            if (!RuleApplied)
                RewriteInnerExpressions(verseProgram.Wrapper.E);
        }
        while (RuleApplied);

        return ApplyRules(verseProgram.Wrapper);
    }

    public Expression ApplyRules(Expression expression)
    {
        RuleApplied = false;
        Expression rewrittenExpression = expression;

        foreach (Func<Expression, Expression> applyRule in _rewriteRules)
        {
            rewrittenExpression = applyRule(expression);

            if (RuleApplied)
                return rewrittenExpression;
        }

        return rewrittenExpression;
    }

    private void RewriteInnerExpressions(Expression expression)
    {
        switch (expression)
        {
            case Eqe eqe:
                Rewrite(eqe);
                break;
            case Equation eq:
                Rewrite(eq);
                break;
            case Lambda lambda:
                Rewrite(lambda);
                break;
            case Choice choice:
                Rewrite(choice);
                break;
            case Exists exists:
                Rewrite(exists);
                break;
            case Wrapper wrapper:
                Rewrite(wrapper);
                break;
        }
    }

    private void Rewrite(Eqe eqe)
    {
        eqe.Eq = ApplyRules(eqe.Eq);

        if (RuleApplied)
            return;

        RewriteInnerExpressions(eqe.Eq);

        if (RuleApplied)
            return;

        eqe.E = ApplyRules(eqe.E);

        if (RuleApplied)
            return;

        RewriteInnerExpressions(eqe.E);
    }

    private void Rewrite(Equation eq)
    {
        eq.E = ApplyRules(eq.E);

        if (RuleApplied)
            return;

        RewriteInnerExpressions(eq.E);
    }

    private void Rewrite(Lambda lambda)
    {
        lambda.E = ApplyRules(lambda.E);

        if (RuleApplied)
            return;

        RewriteInnerExpressions(lambda.E);
    }

    private void Rewrite(Choice choice)
    {
        choice.E1 = ApplyRules(choice.E1);

        if (RuleApplied)
            return;

        RewriteInnerExpressions(choice.E1);

        if (RuleApplied)
            return;

        choice.E2 = ApplyRules(choice.E2);

        if (RuleApplied)
            return;

        RewriteInnerExpressions(choice.E2);
    }

    private void Rewrite(Exists exists)
    {
        exists.E = ApplyRules(exists.E);

        if (RuleApplied)
            return;

        RewriteInnerExpressions(exists.E);
    }

    private void Rewrite(Wrapper wrapper)
    {
        wrapper.E = ApplyRules(wrapper.E);

        if (RuleApplied)
            return;

        RewriteInnerExpressions(wrapper.E);
    }

    [RewriteRule]
    private Expression AppAdd(Expression expression)
    {
        if (expression is Application { V1: Add, V2: Tuple { Values: IEnumerable<Value> values } })
        {
            if (values.Count() is 2 && values.ElementAt(0) is Integer i1 && values.ElementAt(1) is Integer i2)
            {
                Expression rewrittenExpression = new Integer(i1.Value + i2.Value);
                _logger.DisplayRuleApplied("APP-ADD");
                RuleApplied = true;
                return rewrittenExpression;
            }
        }

        return expression;
    }

    [RewriteRule]
    private Expression AppGtAndAppGtFail(Expression expression)
    {
        if (expression is Application { V1: Gt, V2: Tuple { Values: IEnumerable<Value> values } })
        {
            if (values.Count() is 2 && values.ElementAt(0) is Integer i1 && values.ElementAt(1) is Integer i2)
            {
                if (i1.Value > i2.Value)
                {
                    Expression rewrittenExpression = i1;
                    RuleApplied = true;
                    _logger.DisplayRuleApplied("APP-GT");
                    return rewrittenExpression;
                }
                else
                {
                    Expression rewrittenExpression = new Fail();
                    RuleApplied = true;
                    _logger.DisplayRuleApplied("APP-GT-FAIL");
                    return rewrittenExpression;
                }
            }
        }

        return expression;
    }

    [RewriteRule]
    private Expression ULit(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: Integer k1, E: Integer k2 }, E: Expression e })
        {
            if (k1.Value == k2.Value)
            {
                RuleApplied = true;
                _logger.DisplayRuleApplied("U-LIT");
                return e;
            }
        }

        return expression;
    }

    [RewriteRule]
    private Expression ValElim(Expression expression)
    {
        if (expression is Eqe { Eq: Value, E: Expression e })
        {
            RuleApplied = true;
            _logger.DisplayRuleApplied("VAL-ELIM");
            return e;
        }

        return expression;
    }

    [RewriteRule]
    private Expression ExiSwap(Expression expression)
    {
        if (expression is Exists { V: Variable, E: Exists { V: Variable, E: Expression e } existsY } existsX)
        {
            RuleApplied = true;
            _logger.DisplayRuleApplied("EXI-SWAP");
            return existsY.E = existsX.E = e;
        }

        return expression;
    }

    [RewriteRule]
    private Expression OneFail(Expression expression)
    {
        if (expression is One { E: Fail fail })
        {
            RuleApplied = true;
            _logger.DisplayRuleApplied("ONE-FAIL");
            return fail;
        }

        return expression;
    }

    [RewriteRule]
    private Expression OneValue(Expression expression)
    {
        if (expression is One { E: Value value })
        {
            RuleApplied = true;
            _logger.DisplayRuleApplied("ONE-VALUE");
            return value;
        }

        return expression;
    }
}
