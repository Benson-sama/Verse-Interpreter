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
            AppTup0,
            // Unification.
            ULit,
            // Elimination.
            ValElim,
            // Normalisation.
            ExiSwap,
            // Choice.
            OneFail,
            OneValue,
            OneChoice,
            AllFail,
            AllValue,
            ChooseL,
            ChooseR,
            ChooseAssoc
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

    public IEnumerable<Variable> FreeVariablesOf(Expression expression)
        => FreeVariablesOf(expression, new VariableBuffer());

    private IEnumerable<Variable> FreeVariablesOf(Expression expression, VariableBuffer variableBuffer)
    {
        return expression switch
        {
            Eqe eqe => FreeVariablesOf(eqe, variableBuffer),
            Equation eq => FreeVariablesOf(eq, variableBuffer),
            Lambda lambda => FreeVariablesOf(lambda, variableBuffer),
            Tuple tuple => FreeVariablesOf(tuple, variableBuffer),
            Variable variable => FreeVariablesOf(variable, variableBuffer),
            Application application => FreeVariablesOf(application, variableBuffer),
            Choice choice => FreeVariablesOf(choice, variableBuffer),
            Exists exists => FreeVariablesOf(exists, variableBuffer),
            Wrapper wrapper => FreeVariablesOf(wrapper, variableBuffer),
            _ => Enumerable.Empty<Variable>()
        };
    }

    private IEnumerable<Variable> FreeVariablesOf(Eqe eqe, VariableBuffer variableBuffer)
    {
        FreeVariablesOf(eqe.Eq, variableBuffer);
        return FreeVariablesOf(eqe.E, variableBuffer);
    }

    private IEnumerable<Variable> FreeVariablesOf(Equation eq, VariableBuffer variableBuffer)
    {
        FreeVariablesOf(eq.V, variableBuffer);
        return FreeVariablesOf(eq.E, variableBuffer);
    }

    private IEnumerable<Variable> FreeVariablesOf(Lambda lambda, VariableBuffer variableBuffer)
    {
        if (lambda.Parameter is not null)
            variableBuffer.BoundVariables = variableBuffer.BoundVariables.Append(lambda.Parameter);

        return FreeVariablesOf(lambda.E, variableBuffer);
    }

    private IEnumerable<Variable> FreeVariablesOf(Tuple tuple, VariableBuffer variableBuffer)
    {
        foreach (Value value in tuple.Values)
        {
            FreeVariablesOf(value, variableBuffer);
        }

        return variableBuffer.FreeVariables;
    }

    private static IEnumerable<Variable> FreeVariablesOf(Variable variable, VariableBuffer variableBuffer)
    {
        if (!variableBuffer.BoundVariables.Contains(variable))
            variableBuffer.FreeVariables = variableBuffer.FreeVariables.Append(variable);

        return variableBuffer.FreeVariables;
    }

    private IEnumerable<Variable> FreeVariablesOf(Application application, VariableBuffer variableBuffer)
    {
        FreeVariablesOf(application.V1, variableBuffer);
        return FreeVariablesOf(application.V2, variableBuffer);
    }

    private IEnumerable<Variable> FreeVariablesOf(Choice choice, VariableBuffer variableBuffer)
    {
        FreeVariablesOf(choice.E1, variableBuffer);
        return FreeVariablesOf(choice.E2, variableBuffer);
    }

    private IEnumerable<Variable> FreeVariablesOf(Exists exists, VariableBuffer variableBuffer)
    {
        variableBuffer.BoundVariables = variableBuffer.BoundVariables.Append(exists.V);
        return FreeVariablesOf(exists.E, variableBuffer);
    }

    private IEnumerable<Variable> FreeVariablesOf(Wrapper wrapper, VariableBuffer variableBuffer)
        => FreeVariablesOf(wrapper.E, variableBuffer);

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

    #region Application

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
    private Expression AppBeta(Expression expression)
    {
        throw new NotImplementedException();
    }

    [RewriteRule]
    private Expression AppTup(Expression expression)
    {
        throw new NotImplementedException();
    }

    [RewriteRule]
    private Expression AppTup0(Expression expression)
    {
        if (expression is Application { V1: Tuple { Values: IEnumerable<Value> values }, V2: Value })
        {
            if (values.Count() is 0)
            {
                Expression rewrittenExpression = new Fail();
                _logger.DisplayRuleApplied("APP-TUP-0");
                RuleApplied = true;
                return rewrittenExpression;
            }
        }

        return expression;
    }

    #endregion

    #region Unification

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
    private Expression UTup(Expression expression)
    {
        throw new NotImplementedException();
    }

    [RewriteRule]
    private Expression UFail(Expression expression)
    {
        throw new NotImplementedException();
    }

    [RewriteRule]
    private Expression UOccurs(Expression expression)
    {
        throw new NotImplementedException();
    }

    [RewriteRule]
    private Expression Subst(Expression expression)
    {
        throw new NotImplementedException();
    }


    [RewriteRule]
    private Expression HnfSwap(Expression expression)
    {
        throw new NotImplementedException();
    }

    [RewriteRule]
    private Expression VarSwap(Expression expression)
    {
        throw new NotImplementedException();
    }

    [RewriteRule]
    private Expression SeqSwap(Expression expression)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Elimination

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
    private Expression ExiElim(Expression expression)
    {
        throw new NotImplementedException();
    }

    [RewriteRule]
    private Expression EqnElim(Expression expression)
    {
        throw new NotImplementedException();
    }

    [RewriteRule]
    private Expression FailElim(Expression expression)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Normalisation

    [RewriteRule]
    private Expression ExiFloat(Expression expression)
    {
        throw new NotImplementedException();
    }

    [RewriteRule]
    private Expression SeqAssoc(Expression expression)
    {
        throw new NotImplementedException();
    }

    [RewriteRule]
    private Expression EqnFloat(Expression expression)
    {
        throw new NotImplementedException();
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

    #endregion

    #region Choice

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

    [RewriteRule]
    private Expression OneChoice(Expression expression)
    {
        if (expression is One { E: Choice { E1: Value v, E2: Expression } })
        {
            RuleApplied = true;
            _logger.DisplayRuleApplied("ONE-CHOICE");
            return v;
        }

        return expression;
    }

    [RewriteRule]
    private Expression AllFail(Expression expression)
    {
        if (expression is All { E: Fail })
        {
            RuleApplied = true;
            _logger.DisplayRuleApplied("ALL-FAIL");
            return Tuple.Empty;
        }

        return expression;
    }

    [RewriteRule]
    private Expression AllValue(Expression expression)
    {
        if (expression is All { E: Value v })
        {
            RuleApplied = true;
            _logger.DisplayRuleApplied("ALL-VALUE");
            return new Tuple { Values = new Value[] { v } };
        }

        return expression;
    }

    [RewriteRule]
    private Expression AllChoice(Expression expression)
    {
        throw new NotImplementedException();
    }

    [RewriteRule]
    private Expression ChooseR(Expression expression)
    {
        if (expression is Choice { E1: Fail, E2: Expression e })
        {
            RuleApplied = true;
            _logger.DisplayRuleApplied("CHOOSE-R");
            return e;
        }

        return expression;
    }

    [RewriteRule]
    private Expression ChooseL(Expression expression)
    {
        if (expression is Choice { E1: Expression e, E2: Fail, })
        {
            RuleApplied = true;
            _logger.DisplayRuleApplied("CHOOSE-R");
            return e;
        }

        return expression;
    }

    [RewriteRule]
    private Expression ChooseAssoc(Expression expression)
    {
        if (expression is Choice { E1: Choice { E1: Expression e1, E2: Expression e2 } innerChoice, E2: Expression e3 } outerChoice)
        {
            Expression rewrittenExpression = new Choice
            {
                E1 = e1,
                E2 = new Choice
                {
                    E1 = e2,
                    E2 = e3
                }
            };
            RuleApplied = true;
            _logger.DisplayRuleApplied("CHOOSE-ASSOC");
            return rewrittenExpression;
        }

        return expression;
    }

    [RewriteRule]
    private Expression Choose(Expression expression)
    {
        throw new NotImplementedException();
    }

    #endregion
}
