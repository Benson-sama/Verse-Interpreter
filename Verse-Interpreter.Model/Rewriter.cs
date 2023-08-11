using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model;

public class Rewriter : IRewriter
{
    private readonly IRenderer _renderer;

    private readonly IVariableFactory _variableFactory;

    private readonly Func<Expression, Expression>[] _rewriteRules;

    public Rewriter(IRenderer renderer, IVariableFactory variableFactory)
    {
        _renderer = renderer;
        _variableFactory = variableFactory;
        _rewriteRules = new Func<Expression, Expression>[]
        {
            // Application.
            AppAdd,
            AppGt,
            AppGtFail,
            //AppBeta,
            AppTup,
            AppTup0,
            // Unification.
            ULit,
            UTup,
            UFail,
            //UOccurs,
            //Subst,
            HnfSwap,
            //VarSwap,
            //SeqSwap,
            // Elimination.
            ValElim,
            ExiElim,
            //EqnElim,
            //FailElim,
            // Normalisation.
            //ExiFloat,
            SeqAssoc,
            EqnFloat,
            ExiSwap,
            // Choice.
            OneFail,
            OneValue,
            OneChoice,
            AllFail,
            AllValue,
            AllChoice,
            ChooseR,
            ChooseL,
            ChooseAssoc,
            //Choose
        };
    }

    private IRenderer Renderer { get => _renderer; }

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
        => expression.FreeVariables(new VariableBuffer());

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
        if (expression is Application { V1: Add, V2: VerseTuple tuple })
        {
            if (tuple.Count() is 2 && tuple.ElementAt(0) is Integer i1 && tuple.ElementAt(1) is Integer i2)
            {
                Expression rewrittenExpression = new Integer(i1.Value + i2.Value);
                Renderer.DisplayRuleApplied("APP-ADD");
                RuleApplied = true;
                return rewrittenExpression;
            }
        }

        return expression;
    }

    [RewriteRule]
    private Expression AppGt(Expression expression)
    {
        if (expression is Application { V1: Gt, V2: VerseTuple tuple })
        {
            if (tuple.Count() is 2 && tuple.ElementAt(0) is Integer i1 && tuple.ElementAt(1) is Integer i2)
            {
                if (i1.Value > i2.Value)
                {
                    Expression rewrittenExpression = i1;
                    RuleApplied = true;
                    Renderer.DisplayRuleApplied("APP-GT");
                    return rewrittenExpression;
                }
            }
        }

        return expression;
    }

    [RewriteRule]
    private Expression AppGtFail(Expression expression)
    {
        if (expression is Application { V1: Gt, V2: VerseTuple tuple })
        {
            if (tuple.Count() is 2 && tuple.ElementAt(0) is Integer i1 && tuple.ElementAt(1) is Integer i2)
            {
                if (i1.Value <= i2.Value)
                {
                    Expression rewrittenExpression = new Fail();
                    RuleApplied = true;
                    Renderer.DisplayRuleApplied("APP-GT-FAIL");
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
        if (expression is Application { V1: VerseTuple tuple, V2: Value value })
        {
            int count = tuple.Count();

            if (count is > 0)
            {
                RuleApplied = true;
                Renderer.DisplayRuleApplied("APP-TUP");

                Variable variable = _variableFactory.Next();

                if (FreeVariablesOf(tuple).Contains(variable))
                    throw new Exception($"Variable {variable} must not be an element of fvs({tuple})");

                if (count is 1)
                {
                    return new Eqe
                    {
                        Eq = new Equation
                        {
                            V = variable,
                            E = new Integer(0)
                        },
                        E = tuple.First()
                    };
                }

                return new Exists
                {
                    V = variable,
                    E = new Eqe
                    {
                        Eq = new Equation
                        {
                            V = variable,
                            E = value
                        },
                        E = BuildChoiceFromTupleRecursively(tuple, variable, 0)
                    }
                };
            }
        }

        return expression;
    }

    private static Choice BuildChoiceFromTupleRecursively(IEnumerable<Value> tuple, Variable variable, int i)
    {
        return tuple.Count() switch
        {
            > 2 => new Choice
            {
                E1 = new Eqe
                {
                    Eq = new Equation
                    {
                        V = variable,
                        E = new Integer(i)
                    },
                    E = tuple.First()
                },
                E2 = BuildChoiceFromTupleRecursively(tuple.Skip(1), variable, i + 1)
            },
            2 => new Choice
            {
                E1 = new Eqe
                {
                    Eq = new Equation
                    {
                        V = variable,
                        E = new Integer(i++)
                    },
                    E = tuple.First()
                },
                E2 = new Eqe
                {
                    Eq = new Equation
                    {
                        V = variable,
                        E = new Integer(i)
                    },
                    E = tuple.Last()
                }
            },
            _ => throw new Exception("Unable to build choice from tuple. Pattern must be (Value, Choice) or(Value, Value).")
        };
    }

    [RewriteRule]
    private Expression AppTup0(Expression expression)
    {
        if (expression is Application { V1: VerseTuple tuple, V2: Value })
        {
            if (tuple.Count() is 0)
            {
                Expression rewrittenExpression = new Fail();
                Renderer.DisplayRuleApplied("APP-TUP-0");
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
                Renderer.DisplayRuleApplied("U-LIT");
                return e;
            }
        }

        return expression;
    }

    [RewriteRule]
    private Expression UTup(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: VerseTuple t1, E: VerseTuple t2 }, E: Expression e })
        {
            if (t1.Count() == t2.Count())
            {
                RuleApplied = true;
                Renderer.DisplayRuleApplied("U-TUP");
                return BuildTupleEqeRecursively(t1.Zip(t2), e);
            }
        }

        return expression;
    }

    private Expression BuildTupleEqeRecursively(IEnumerable<(Value leftSideValue, Value rightSideValue)> values, Expression e)
    {
        if (values.Count() is 0)
            return e;

        Equation equation = new() { V = values.First().leftSideValue, E = values.First().rightSideValue };

        if (values.Count() is 1)
            return new Eqe { Eq = equation, E = e };

        return new Eqe { Eq = equation, E = BuildTupleEqeRecursively(values.Skip(1), e) };
    }

    [RewriteRule]
    private Expression UFail(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: HeadNormalForm, E: HeadNormalForm }, E: Expression })
        {
            RuleApplied = true;
            Renderer.DisplayRuleApplied("U-FAIL");
            return new Fail();
        }

        return expression;
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
        if (expression is Eqe { Eq: Equation { V: HeadNormalForm hnf, E: Variable v } eq, E: Expression } eqe)
        {
            eq.V = v;
            eq.E = hnf;
            RuleApplied = true;
            Renderer.DisplayRuleApplied("HNF-SWAP");
        }

        return expression;
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
            Renderer.DisplayRuleApplied("VAL-ELIM");
            return e;
        }

        return expression;
    }

    [RewriteRule]
    private Expression ExiElim(Expression expression)
    {
        if (expression is Exists { V: Variable v, E: Expression e } && !FreeVariablesOf(e).Contains(v))
        {
            RuleApplied = true;
            Renderer.DisplayRuleApplied("EXI-ELIM");
            return e;
        }

        return expression;
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
        if (expression is Eqe { Eq: Eqe { Eq: Expression eq, E: Expression e1 }, E: Expression e2 })
        {
            RuleApplied = true;
            Renderer.DisplayRuleApplied("SEQ-ASSOC");
            return new Eqe
            {
                Eq = eq,
                E = new Eqe
                {
                    Eq = e1,
                    E = e2
                }
            };
        }

        return expression;
    }

    [RewriteRule]
    private Expression EqnFloat(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: Value v, E: Eqe { Eq: Expression eq, E: Expression e1 }, E: Expression e2 } })
        {
            RuleApplied = true;
            Renderer.DisplayRuleApplied("EQN-FLOAT");
            return new Eqe
            {
                Eq = eq,
                E = new Eqe
                {
                    Eq = new Equation
                    {
                        V = v,
                        E = e1
                    },
                    E = e2
                }
            };
        }

        return expression;
    }

    [RewriteRule]
    private Expression ExiSwap(Expression expression)
    {
        if (expression is Exists { V: Variable, E: Exists { V: Variable, E: Expression e } existsY } existsX)
        {
            RuleApplied = true;
            Renderer.DisplayRuleApplied("EXI-SWAP");
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
            Renderer.DisplayRuleApplied("ONE-FAIL");
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
            Renderer.DisplayRuleApplied("ONE-VALUE");
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
            Renderer.DisplayRuleApplied("ONE-CHOICE");
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
            Renderer.DisplayRuleApplied("ALL-FAIL");
            return VerseTuple.Empty;
        }

        return expression;
    }

    [RewriteRule]
    private Expression AllValue(Expression expression)
    {
        if (expression is All { E: Value v })
        {
            RuleApplied = true;
            Renderer.DisplayRuleApplied("ALL-VALUE");
            return new VerseTuple(new Value[] { v });
        }

        return expression;
    }

    [RewriteRule]
    private Expression AllChoice(Expression expression)
    {
        if (expression is All { E: Choice choice } && IsChoiceWithOnlyValues(choice))
        {
            RuleApplied = true;
            Renderer.DisplayRuleApplied("ALL-CHOICE");
            return BuildTupleFromChoiceRecursively(choice);
        }

        return expression;
    }

    private static bool IsChoiceWithOnlyValues(Choice choice)
    {
        return (choice.E1, choice.E2) switch
        {
            (Value, Choice nestedChoice) => IsChoiceWithOnlyValues(nestedChoice),
            (Value, Value) => true,
            _ => false
        };
    }

    private static VerseTuple BuildTupleFromChoiceRecursively(Choice choice)
    {
        return (choice.E1, choice.E2) switch
        {
            (Value v1, Choice nestedChoice) => new VerseTuple(BuildTupleFromChoiceRecursively(nestedChoice).Prepend(v1)),
            (Value v1, Value v2) => new VerseTuple(v1, v2),
            _ => throw new Exception("Unable to build tuple from choice. Pattern must be (Value, Choice) or (Value, Value).")
        };
    }

    [RewriteRule]
    private Expression ChooseR(Expression expression)
    {
        if (expression is Choice { E1: Fail, E2: Expression e })
        {
            RuleApplied = true;
            Renderer.DisplayRuleApplied("CHOOSE-R");
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
            Renderer.DisplayRuleApplied("CHOOSE-L");
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
            Renderer.DisplayRuleApplied("CHOOSE-ASSOC");
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
