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
            AppBeta,
            AppTup,
            AppTup0,
            // Unification.
            ULit,
            UTup,
            UFail,
            UOccurs,
            Subst,
            HnfSwap,
            VarSwap,
            SeqSwap,
            // Elimination.
            ValElim,
            ExiElim,
            EqnElim,
            FailElim,
            // Normalisation.
            ExiFloat,
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

    private VerseProgram? CurrentVerseProgram { get; set; }

    public Expression Rewrite(VerseProgram verseProgram)
    {
        CurrentVerseProgram = verseProgram;

        do
        {
            RuleApplied = false;
            RewriteInnerExpressions(verseProgram.Wrapper.E);

            if (!RuleApplied)
                verseProgram.Wrapper.E = ApplyRules(verseProgram.Wrapper.E);

            if (RuleApplied)
                Renderer.DisplayMessage(verseProgram.Wrapper.E.ToString()!);
        }
        while (RuleApplied);

        return ApplyRules(verseProgram.Wrapper);
    }

    public Expression ApplyRules(Expression expression)
    {
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
        => expression.FreeVariables();

    public static IEnumerable<Variable> FreeVariablesOf(Expression expression, Expression? finalExpression = null)
        => expression.FreeVariables(finalExpression);

    public bool VariableBoundInsideVariable(Variable x, Variable y)
    {
        bool foundY = false;

        VariableBuffer variableBuffer = new();
        CurrentVerseProgram?.Wrapper.E.FreeVariables(variableBuffer);

        foreach (Variable variable in variableBuffer.BoundVariables)
        {
            if (variable.Equals(y))
            {
                foundY = true;
                continue;
            }

            if (variable.Equals(x) && !foundY)
                return false;
            else if (variable.Equals(x) && foundY)
                return true;
        }

        throw new Exception($"Did not find bound variable {x}");
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
        RewriteInnerExpressions(eqe.E);

        if (RuleApplied)
            return;

        eqe.E = ApplyRules(eqe.E);

        if (RuleApplied)
            return;

        RewriteInnerExpressions(eqe.Eq);

        if (RuleApplied)
            return;

        eqe.Eq = ApplyRules(eqe.Eq);
    }

    private void Rewrite(Equation eq)
    {
        RewriteInnerExpressions(eq.E);

        if (RuleApplied)
            return;

        eq.E = ApplyRules(eq.E);
    }

    private void Rewrite(Lambda lambda)
    {
        RewriteInnerExpressions(lambda.E);

        if (RuleApplied)
            return;

        lambda.E = ApplyRules(lambda.E);
    }

    private void Rewrite(Choice choice)
    {
        RewriteInnerExpressions(choice.E2);

        if (RuleApplied)
            return;

        choice.E2 = ApplyRules(choice.E2);

        if (RuleApplied)
            return;

        RewriteInnerExpressions(choice.E1);

        if (RuleApplied)
            return;

        choice.E1 = ApplyRules(choice.E1);
    }

    private void Rewrite(Exists exists)
    {
        RewriteInnerExpressions(exists.E);

        if (RuleApplied)
            return;

        exists.E = ApplyRules(exists.E);
    }

    private void Rewrite(Wrapper wrapper)
    {
        RewriteInnerExpressions(wrapper.E);

        if (RuleApplied)
            return;

        wrapper.E = ApplyRules(wrapper.E);
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
        if (expression is Application { V1: Lambda { E: Expression e } lambda, V2: Value v })
        {
            RuleApplied = true;
            Renderer.DisplayRuleApplied("APP-BETA");

            if (FreeVariablesOf(v).Contains(lambda.Parameter))
                ApplyAlphaConversionWithoutCapturingVariablesOfValue(lambda, v);

            return new Exists
            {
                V = lambda.Parameter,
                E = new Eqe
                {
                    Eq = new Equation
                    {
                        V = lambda.Parameter,
                        E = v
                    },
                    E = e
                }
            };
        }

        return expression;
    }

    private void ApplyAlphaConversionWithoutCapturingVariablesOfValue(Lambda lambda, Value v)
    {
        Variable previousVariable = lambda.Parameter;
        Variable newVariable;

        do
        {
            newVariable = _variableFactory.Next();
        } while (FreeVariablesOf(v).Contains(newVariable));

        lambda.Parameter = newVariable;
        lambda.E.ApplyAlphaConversion(previousVariable, newVariable);
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
        if (expression is Eqe { Eq: Equation { V: Variable v, E: VerseTuple tuple }, E: Expression })
        {
            if (VariableOccursInVerseTuple(v, tuple))
            {
                RuleApplied = true;
                Renderer.DisplayRuleApplied("U-OCCURS");
                return new Fail();
            }
        }

        return expression;
    }

    private bool VariableOccursInVerseTuple(Variable x, VerseTuple tuple)
    {
        bool isOccured = false;

        foreach (Value value in tuple)
        {
            isOccured = value switch
            {
                Variable v => v.Equals(x),
                VerseTuple nestedTuple => VariableOccursInVerseTuple(x, nestedTuple),
                _ => isOccured
            };
        }

        return isOccured;
    }

    [RewriteRule]
    private Expression Subst(Expression expression)
    {
        (bool isFound, Eqe? eqe) = IsExecutionContextWithEquationIncludingHole(expression);

        if (isFound)
        {
            if (eqe is not Eqe { Eq: Equation { V: Variable x, E: Value v }, E: Expression e } finalEqe)
                throw new Exception("Final Eqe in substitute must match the rule.");

            if (FreeVariablesOf(expression).Contains(x) && FreeVariablesOf(e).Contains(x) && !FreeVariablesOf(v).Contains(x))
            {
                if (v is not Variable || (v is Variable y && VariableBoundInsideVariable(x, y)))
                {
                    expression.SubstituteUntilEqe(finalEqe, x, v.DeepCopy());
                    RuleApplied = true;
                    Renderer.DisplayRuleApplied("SUBST");
                }
            }
        }

        return expression;
    }

    private (bool isFound, Eqe? eqe) IsExecutionContextWithEquationIncludingHole(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: Variable, E: Value }, E: Expression } eqe)
            return (true, eqe);

        (bool isFound, Eqe? eqe) result = (false, null);

        if (expression is Eqe { Eq: Equation { V: Value, E: Expression x }, E: Expression })
            result = IsExecutionContextWithEquationIncludingHole(x);

        if (result.isFound)
            return result;

        if (expression is Eqe { Eq: Expression eq, E: Expression e })
        {
            result = IsExecutionContextWithEquationIncludingHole(eq);

            if (result.isFound)
                return result;

            result = IsExecutionContextWithEquationIncludingHole(e);
        }

        return result;
    }

    private IEnumerable<Eqe> AlternativeIsExecutionContextWithEquationIncludingHole(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: Variable, E: Value }, E: Expression } eqe)
            yield return eqe;

        if (expression is Eqe { Eq: Equation { V: Value, E: Expression x }, E: Expression })
        {
            foreach (Eqe foundEqe in AlternativeIsExecutionContextWithEquationIncludingHole(x))
            {
                yield return foundEqe;
            }
        }

        if (expression is Eqe { Eq: Expression eq, E: Expression e })
        {
            foreach (Eqe foundEqe in AlternativeIsExecutionContextWithEquationIncludingHole(eq))
            {
                yield return foundEqe;
            }

            foreach (Eqe foundEqe in AlternativeIsExecutionContextWithEquationIncludingHole(e))
            {
                yield return foundEqe;
            }
        }
    }

    [RewriteRule]
    private Expression HnfSwap(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: HeadNormalForm hnf, E: Variable v } eq, E: Expression })
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
        if (expression is Eqe { Eq: Equation { V: Variable y, E: Variable x }, E: Expression e }
            && !x.Equals(y) && VariableBoundInsideVariable(x, y))
        {
            RuleApplied = true;
            Renderer.DisplayRuleApplied("VAR-SWAP");

            return new Eqe
            {
                Eq = new Equation
                {
                    V = x,
                    E = y
                },
                E = e
            };
        }

        return expression;
    }

    [RewriteRule]
    private Expression SeqSwap(Expression expression)
    {
        if (expression is Eqe { Eq: Expression eq, E: Eqe { Eq: Equation { V: Variable x, E: Value v }, E: Expression e } })
        {
            if (eq is not Equation { V: Variable, E: Value }
            || (eq is Equation { V: Variable y, E: Value } && !(x.Equals(y) || VariableBoundInsideVariable(y, x))))
            {
                RuleApplied = true;
                Renderer.DisplayRuleApplied("SEQ-SWAP");

                return new Eqe
                {
                    Eq = new Equation
                    {
                        V = x,
                        E = v,
                    },
                    E = new Eqe
                    {
                        Eq = eq,
                        E = e
                    }
                };
            }
        }

        return expression;
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
        if (expression is Exists { V: Variable existsX, E: Expression existsE })
        {
            (bool isFound, Eqe? eqe) = IsExecutionContextWithEquationIncludingHole(existsE);

            if (isFound)
            {
                if (eqe is not Eqe { Eq: Equation { V: Variable equationX, E: Value v }, E: Expression equationE } finalEqe)
                    throw new Exception("Final Eqe in EQN-ELIM must match the rule.");

                if (existsX.Equals(equationX)
                    && !FreeVariablesOf(existsE, finalEqe).Contains(existsX)
                    && !FreeVariablesOf(v).Contains(existsX)
                    && !FreeVariablesOf(equationE).Contains(existsX))
                {
                    expression.EliminateEquation(finalEqe);
                    RuleApplied = true;
                    Renderer.DisplayRuleApplied("EQN-ELIM");
                }
            }
        }

        return expression;
    }

    [RewriteRule]
    private Expression AlternativeEqnElim(Expression expression)
    {
        if (expression is Exists { V: Variable existsX, E: Expression existsE })
        {
            foreach (Eqe eqe in AlternativeIsExecutionContextWithEquationIncludingHole(existsE))
            {
                if (eqe is not Eqe { Eq: Equation { V: Variable equationX, E: Value v }, E: Expression equationE } finalEqe)
                    throw new Exception("Final Eqe in EQN-ELIM must match the rule.");

                if (existsX.Equals(equationX)
                    && !FreeVariablesOf(existsE, finalEqe).Contains(existsX)
                    && !FreeVariablesOf(v).Contains(existsX)
                    && !FreeVariablesOf(equationE).Contains(existsX))
                {
                    expression.EliminateEquation(finalEqe);
                    RuleApplied = true;
                    Renderer.DisplayRuleApplied("EQN-ELIM");

                    return expression;
                }
            }
        }

        return expression;
    }

    [RewriteRule]
    private Expression FailElim(Expression expression)
    {
        if (IsExecutionContextFailingExcludingHole(expression))
        {
            RuleApplied = true;
            Renderer.DisplayRuleApplied("FAIL-ELIM");
            return new Fail();
        }

        return expression;
    }

    private bool IsExecutionContextFailingExcludingHole(Expression expression)
    {
        bool isFailing = false;

        if (expression is Eqe { Eq: Equation { V: Value, E: Expression x }, E: Expression })
            isFailing = IsExecutionContextFailingIncludingHole(x);

        if (expression is Eqe { Eq: Expression eq, E: Expression e })
            isFailing = IsExecutionContextFailingIncludingHole(eq) || IsExecutionContextFailingIncludingHole(e);

        return isFailing;
    }

    private bool IsExecutionContextFailingIncludingHole(Expression expression)
    {
        if (expression is Fail)
            return true;

        return IsExecutionContextFailingExcludingHole(expression);
    }

    #endregion

    #region Normalisation

    [RewriteRule]
    private Expression ExiFloat(Expression expression)
    {
        (bool isFound, Exists? exists) = IsExecutionContextWithExistsExcludingHole(expression);

        if (isFound)
        {
            if (exists is null)
                throw new Exception("Exists cannot be null if execution context matched in EXI-FLOAT.");

            if (FreeVariablesOf(expression, finalExpression: exists).Contains(exists.V))
            {
                Variable newVariable = _variableFactory.Next();
                exists.E.ApplyAlphaConversion(exists.V, newVariable);
                exists.V = newVariable;
            }

            expression.ReplaceExistsWithItsExpression(exists);
            exists.E = expression;

            RuleApplied = true;
            Renderer.DisplayRuleApplied("EXI-FLOAT");

            return exists;
        }

        return expression;
    }

    private (bool isFound, Exists? eqe) IsExecutionContextWithExistsIncludingHole(Expression expression)
    {
        if (expression is Exists exists)
            return (true, exists);

        return IsExecutionContextWithExistsExcludingHole(expression);
    }

    private (bool isFound, Exists? eqe) IsExecutionContextWithExistsExcludingHole(Expression expression)
    {
        (bool isFound, Exists? eqe) result = (false, null);

        if (expression is Eqe { Eq: Equation { V: Value, E: Expression x }, E: Expression })
            result = IsExecutionContextWithExistsIncludingHole(x);

        if (result.isFound)
            return result;

        if (expression is Eqe { Eq: Expression eq, E: Expression e })
        {
            result = IsExecutionContextWithExistsIncludingHole(eq);

            if (result.isFound)
                return result;

            result = IsExecutionContextWithExistsIncludingHole(e);
        }

        return result;
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
        if (expression is Exists { V: Variable x, E: Exists { V: Variable y, E: Expression e } })
        {
            RuleApplied = true;
            Renderer.DisplayRuleApplied("EXI-SWAP");
            return new Exists
            {
                V = y,
                E = new Exists
                {
                    V = x,
                    E = e
                }
            };
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
        if (expression is Choice { E1: Choice { E1: Expression e1, E2: Expression e2 }, E2: Expression e3 })
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
