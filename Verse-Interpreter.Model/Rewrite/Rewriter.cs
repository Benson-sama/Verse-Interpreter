using Verse_Interpreter.Model.Build;
using Verse_Interpreter.Model.Render;
using Verse_Interpreter.Model.Rewrite.Utility;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;
using Verse_Interpreter.Model.Visitor;

namespace Verse_Interpreter.Model.Rewrite;

public class Rewriter : IRewriter
{
    private readonly IRenderer _renderer;

    private readonly IVariableFactory _variableFactory;

    private readonly Func<Expression, Expression>[] _rewriteRules;

    private readonly VariablesAnalyser _variablesAnalyser = new();

    public Rewriter(IRenderer renderer, IVariableFactory variableFactory)
    {
        _renderer = renderer;
        _variableFactory = variableFactory;
        _rewriteRules = new Func<Expression, Expression>[]
        {
            // Application.
            AppAdd,
            AppSub,
            AppMult,
            AppDiv,
            AppGt,
            AppLt,
            AppGtFail,
            AppLtFail,
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
            //ExiSwap,
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
            Choose
        };
    }

    private IRenderer Renderer => _renderer;

    private VariablesAnalyser VariablesAnalyser => _variablesAnalyser;

    private bool RuleApplied { get; set; }

    private VerseProgram? CurrentVerseProgram { get; set; }

    public Expression Rewrite(VerseProgram verseProgram)
    {
        CurrentVerseProgram = verseProgram;
        do
        {
            RuleApplied = false;
            verseProgram.E = ApplyRules(verseProgram.E);

            if (!RuleApplied)
                RewriteInnerExpressions(verseProgram.E);

            if (RuleApplied)
                Renderer.DisplayIntermediateResult(verseProgram.E);
        }
        while (RuleApplied);

        return verseProgram.E;
    }

    public IExpressionOrEquation ApplyRules(IExpressionOrEquation eq)
    {
        if (eq is Expression e)
            return ApplyRules(e);

        return eq;
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

    public bool VariableBoundInsideVariable(Variable x, Variable y)
    {
        if (CurrentVerseProgram is null)
            throw new Exception("Cannot check bound variable order if verse program is null.");

        bool foundY = false;

        VariablesAnalyser.AnalyseVariablesOf(CurrentVerseProgram.E);

        foreach (Variable variable in VariablesAnalyser.BoundVariables)
        {
            if (variable == y)
            {
                foundY = true;
                continue;
            }

            if (variable == x && !foundY)
                return false;
            else if (variable == x && foundY)
                return true;
        }

        throw new Exception($"Did not find bound variable {x}");
    }

    private void RewriteInnerExpressions(IExpressionOrEquation eq)
    {
        switch (eq)
        {
            case Expression e:
                RewriteInnerExpressions(e);
                break;
            case Equation equation:
                Rewrite(equation);
                break;
        }
    }

    private void RewriteInnerExpressions(Expression expression)
    {
        switch (expression)
        {
            case Lambda lambda:
                Rewrite(lambda);
                break;
            case Eqe eqe:
                Rewrite(eqe);
                break;
            case Exists exists:
                Rewrite(exists);
                break;
            case Choice choice:
                Rewrite(choice);
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
                expression = new Integer(i1.Value + i2.Value);

                OnRuleApplied("APP-ADD-INT");
            }
            else if (tuple.Count() is 2 && tuple.ElementAt(0) is VerseString s1 && tuple.ElementAt(1) is VerseString s2)
            {
                expression = new VerseString(s1.Text + s2.Text);

                OnRuleApplied("APP-ADD-STRING");
            }
        }

        return expression;
    }

    [RewriteRule]
    private Expression AppSub(Expression expression)
    {
        if (expression is Application { V1: Sub, V2: VerseTuple tuple })
        {
            if (tuple.Count() is 2 && tuple.ElementAt(0) is Integer i1 && tuple.ElementAt(1) is Integer i2)
            {
                expression = new Integer(i1.Value - i2.Value);

                OnRuleApplied("APP-SUB");
            }
        }

        return expression;
    }

    [RewriteRule]
    private Expression AppMult(Expression expression)
    {
        if (expression is Application { V1: Mult, V2: VerseTuple tuple })
        {
            if (tuple.Count() is 2 && tuple.ElementAt(0) is Integer i1 && tuple.ElementAt(1) is Integer i2)
            {
                expression = new Integer(i1.Value * i2.Value);

                OnRuleApplied("APP-MULT");
            }
        }

        return expression;
    }

    [RewriteRule]
    private Expression AppDiv(Expression expression)
    {
        if (expression is Application { V1: Div, V2: VerseTuple tuple })
        {
            if (tuple.Count() is 2 && tuple.ElementAt(0) is Integer i1 && tuple.ElementAt(1) is Integer i2)
            {
                expression = new Integer(i1.Value / i2.Value);

                OnRuleApplied("APP-DIV");
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
                    expression = i1;

                    OnRuleApplied("APP-GT");
                }
            }
            else if (tuple.Count() is 2 && tuple.ElementAt(0) is VerseString s1 && tuple.ElementAt(1) is VerseString s2)
            {
                if (s1.Text.Length > s2.Text.Length)
                {
                    expression = s1;

                    OnRuleApplied("APP-GT-STRING");
                }
            }
        }

        return expression;
    }

    [RewriteRule]
    private Expression AppLt(Expression expression)
    {
        if (expression is Application { V1: Lt, V2: VerseTuple tuple })
        {
            if (tuple.Count() is 2 && tuple.ElementAt(0) is Integer i1 && tuple.ElementAt(1) is Integer i2)
            {
                if (i1.Value < i2.Value)
                {
                    expression = i1;

                    OnRuleApplied("APP-LT");
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
                    expression = new Fail();

                    OnRuleApplied("APP-GT-FAIL");
                }
            }
            else if (tuple.Count() is 2 && tuple.ElementAt(0) is VerseString s1 && tuple.ElementAt(1) is VerseString s2)
            {
                if (s1.Text.Length <= s2.Text.Length)
                {
                    expression = new Fail();

                    OnRuleApplied("APP-GT-FAIL-STRING");
                }
            }
        }

        return expression;
    }

    [RewriteRule]
    private Expression AppLtFail(Expression expression)
    {
        if (expression is Application { V1: Lt, V2: VerseTuple tuple })
        {
            if (tuple.Count() is 2 && tuple.ElementAt(0) is Integer i1 && tuple.ElementAt(1) is Integer i2)
            {
                if (i1.Value >= i2.Value)
                {
                    expression = new Fail();

                    OnRuleApplied("APP-LT-FAIL");
                }
            }
            else if (tuple.Count() is 2 && tuple.ElementAt(0) is VerseString && tuple.ElementAt(1) is VerseString)
            {
                expression = new Fail();

                OnRuleApplied("APP-LT-FAIL-STRING");
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

            if (VariablesAnalyser.FreeVariablesOf(v).Contains(lambda.Parameter))
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
        Variable targetVariable = lambda.Parameter;
        Variable newVariable;

        do
        {
            newVariable = _variableFactory.Next();
        } while (VariablesAnalyser.FreeVariablesOf(v).Contains(newVariable));

        AlphaConversionHandler alphaConversionHandler = new(targetVariable, newVariable);
        alphaConversionHandler.ApplyAlphaConversionIncludingParameter(lambda);
    }

    [RewriteRule]
    private Expression AppTup(Expression expression)
    {
        if (expression is Application { V1: VerseTuple tuple, V2: Value value })
        {
            int count = tuple.Count();

            if (count > 0)
            {
                RuleApplied = true;
                Renderer.DisplayRuleApplied("APP-TUP");

                Variable variable = _variableFactory.Next();

                if (VariablesAnalyser.FreeVariablesOf(tuple).Contains(variable))
                    throw new Exception($"Variable {variable} must not be an element of fvs({tuple})");

                if (count == 1)
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
            if (!tuple.Any())
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
            if (k1 == k2)
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
        if (!values.Any())
            return e;

        Equation equation = new() { V = values.First().leftSideValue, E = values.First().rightSideValue };

        if (values.Count() == 1)
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
                Variable v => v == x,
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

            if (VariablesAnalyser.FreeVariablesOf(expression).Contains(x)
                && VariablesAnalyser.FreeVariablesOf(e).Contains(x)
                && !VariablesAnalyser.FreeVariablesOf(v).Contains(x))
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
            && x != y && VariableBoundInsideVariable(x, y))
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
        if (expression is Eqe { Eq: IExpressionOrEquation eq, E: Eqe { Eq: Equation { V: Variable x, E: Value v }, E: Expression e } })
        {
            if (eq is not Equation { V: Variable, E: Value }
            || (eq is Equation { V: Variable y, E: Value } && !(x == y || VariableBoundInsideVariable(y, x))))
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
        if (expression is Exists { V: Variable v, E: Expression e } && !VariablesAnalyser.FreeVariablesOf(e).Contains(v))
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

                if (existsX == equationX
                    && !VariablesAnalyser.FreeVariablesOf(existsE, finalEqe).Contains(existsX)
                    && !VariablesAnalyser.FreeVariablesOf(v).Contains(existsX)
                    && !VariablesAnalyser.FreeVariablesOf(equationE).Contains(existsX))
                {
                    EquationEliminator equationEliminator = new(finalEqe);
                    equationEliminator.EliminateEquationIn(expression);

                    RuleApplied = true;
                    Renderer.DisplayRuleApplied("EQN-ELIM");
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

            if (VariablesAnalyser.FreeVariablesOf(expression, finalExpression: exists).Contains(exists.V))
            {
                Variable newVariable = _variableFactory.Next();
                AlphaConversionHandler alphaConversionHandler = new(exists.V, newVariable);
                alphaConversionHandler.ApplyAlphaConversionIncludingBinder(exists);
            }

            ExistsEliminator existsEliminator = new(exists);
            existsEliminator.EliminateExistsIn(expression);
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
        if (expression is Eqe { Eq: Eqe { Eq: IExpressionOrEquation eq, E: Expression e1 }, E: Expression e2 })
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "EQN-FLOAT".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression EqnFloat(Expression expression)
    {
        if (expression is Eqe
            {
                Eq: Equation
                {
                    V: Value v,
                    E: Eqe
                    {
                        Eq: IExpressionOrEquation eq,
                        E: Expression e1
                    }
                },
                E: Expression e2
            })
        {
            expression = new Eqe
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

            RuleApplied = true;
            Renderer.DisplayRuleApplied("EQN-FLOAT");
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "EXI-SWAP".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression ExiSwap(Expression expression)
    {
        if (expression is Exists { V: Variable x, E: Exists { V: Variable y, E: Expression e } })
        {
            expression = new Exists
            {
                V = y,
                E = new Exists
                {
                    V = x,
                    E = e
                }
            };

            RuleApplied = true;
            Renderer.DisplayRuleApplied("EXI-SWAP");
        }

        return expression;
    }

    #endregion

    #region Choice

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "ONE-FAIL".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression OneFail(Expression expression)
    {
        if (expression is One { E: Fail fail })
        {
            expression = fail;

            RuleApplied = true;
            Renderer.DisplayRuleApplied("ONE-FAIL");
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "ONE-VALUE".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression OneValue(Expression expression)
    {
        if (expression is One { E: Value value })
        {
            expression = value;

            RuleApplied = true;
            Renderer.DisplayRuleApplied("ONE-VALUE");
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "ONE-CHOICE".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression OneChoice(Expression expression)
    {
        if (expression is One { E: Choice { E1: Value v, E2: Expression } })
        {
            expression = v;

            RuleApplied = true;
            Renderer.DisplayRuleApplied("ONE-CHOICE");
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "ALL-FAIL".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression AllFail(Expression expression)
    {
        if (expression is All { E: Fail })
        {
            expression = VerseTuple.Empty;

            RuleApplied = true;
            Renderer.DisplayRuleApplied("ALL-FAIL");
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "ALL-VALUE".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression AllValue(Expression expression)
    {
        if (expression is All { E: Value v })
        {
            expression = new VerseTuple(v);

            RuleApplied = true;
            Renderer.DisplayRuleApplied("ALL-VALUE");
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "ALL-CHOICE".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression AllChoice(Expression expression)
    {
        if (expression is All { E: Choice choice } && IsChoiceWithOnlyValues(choice))
        {
            expression = new VerseTuple(BuildTupleFromChoiceRecursively(choice).ToArray());

            RuleApplied = true;
            Renderer.DisplayRuleApplied("ALL-CHOICE");
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "CHOOSE-R".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression ChooseR(Expression expression)
    {
        if (expression is Choice { E1: Fail, E2: Expression e })
        {
            expression = e;

            RuleApplied = true;
            Renderer.DisplayRuleApplied("CHOOSE-R");
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "CHOOSE-L".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression ChooseL(Expression expression)
    {
        if (expression is Choice { E1: Expression e, E2: Fail, })
        {
            expression = e;

            RuleApplied = true;
            Renderer.DisplayRuleApplied("CHOOSE-L");
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "CHOOSE-ASSOC".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression ChooseAssoc(Expression expression)
    {
        if (expression is Choice { E1: Choice { E1: Expression e1, E2: Expression e2 }, E2: Expression e3 })
        {
            expression = new Choice
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
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "CHOOSE".
    /// CHOOSE  SX[𝐶𝑋[e1|e2]] −→ SX[𝐶𝑋[e1] | 𝐶𝑋[e2]] if 𝐶𝑋 ≠ □
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    /// <exception cref="Exception"></exception>
    [RewriteRule]
    private Expression Choose(Expression expression)
    {
        (bool isFound, Expression? cx, Choice? foundChoice) = IsOuterScopeContextIncludingHole(expression);

        if (isFound)
        {
            if (cx is null || foundChoice is null || expression is not Wrapper outerScopeContext)
                throw new Exception("Choice context or found choice cannot be null when the method states they have been found.");

            ChoiceFloater choiceFloater = new(cx, foundChoice);
            choiceFloater.FloatChoiceToTheTopIn(outerScopeContext);

            RuleApplied = true;
            Renderer.DisplayRuleApplied("CHOOSE");
        }

        return expression;
    }

    private (bool isFound, Expression? cx, Choice? foundChoice) IsOuterScopeContextIncludingHole(Expression expression)
    {
        return expression switch
        {
            One { E: Expression sc } => IsInnerScopeContextIncludingHole(sc),
            All { E: Expression sc } => IsInnerScopeContextIncludingHole(sc),
            _ => default
        };
    }

    private (bool isFound, Expression? cx, Choice? foundChoice) IsInnerScopeContextIncludingHole(Expression expression)
    {
        (bool isFound, Choice? foundChoice) = IsChoiceContextExcludingHole(expression);

        if (isFound)
            return (isFound, expression, foundChoice);

        (bool isFound, Expression? cx, Choice? foundChoice) result = default;

        if (expression is Choice { E1: Expression e1, E2: Expression e2 })
        {
            result = IsInnerScopeContextIncludingHole(e1);

            if (result.isFound)
                return result;

            result = IsInnerScopeContextIncludingHole(e2);
        }

        return result;
    }

    private (bool isFound, Choice? foundChoice) IsChoiceContextIncludingHole(IExpressionOrEquation eq)
    {
        if (eq is Choice { E1: Expression, E2: Expression } choice)
            return (true, choice);

        return IsChoiceContextExcludingHole(eq);
    }

    private (bool isFound, Choice? foundChoice) IsChoiceContextExcludingHole(IExpressionOrEquation eq)
    {
        (bool isFound, Choice? foundChoice) result = default;

        if (eq is Equation { V: Value, E: Expression cx1 })
            result = IsChoiceContextIncludingHole(cx1);

        if (result.isFound)
            return result;

        if (eq is Eqe { Eq: IExpressionOrEquation cx2, E: Expression })
            result = IsChoiceContextIncludingHole(cx2);

        if (result.isFound)
            return result;

        // Note: ce should be checked for IsChoiceFreeExpression instead of the following according to the paper.
        // But this is okay since a context here can never be a hole and therefore never let an equation stand on its own.
        if (eq is Eqe { Eq: IExpressionOrEquation ce, E: Expression cx3 } && IsChoiceFreeExpressionOrEquation(ce))
            result = IsChoiceContextIncludingHole(cx3);

        if (result.isFound)
            return result;

        if (eq is Exists { V: Variable, E: Expression cx4 })
            result = IsChoiceContextIncludingHole(cx4);

        return result;
    }

    private bool IsChoiceFreeExpression(IExpressionOrEquation eq)
    {
        return eq switch
        {
            Value => true,
            Eqe { Eq: IExpressionOrEquation ceq, E: Expression ce }
                when IsChoiceFreeExpressionOrEquation(ceq) && IsChoiceFreeExpression(ce) => true,
            One => true,
            All => true,
            Exists { V: Variable, E: Expression ce } when IsChoiceFreeExpression(ce) => true,
            Application { V1: Operator, V2: Value } => true,
            _ => false
        };
    }

    private bool IsChoiceFreeExpressionOrEquation(IExpressionOrEquation eq)
    {
        return eq switch
        {
            Expression ce when IsChoiceFreeExpression(ce) => true,
            Equation { V: Value, E: Expression ce } when IsChoiceFreeExpression(ce) => true,
            _ => false
        };
    }

    #endregion

    private void OnRuleApplied(string ruleName)
    {
        RuleApplied = true;
        Renderer.DisplayRuleApplied(ruleName);
    }
}
