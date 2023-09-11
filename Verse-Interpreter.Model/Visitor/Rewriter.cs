//------------------------------------------------------------
// <copyright file="Rewriter.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Rewriter class.</summary>
//------------------------------------------------------------

using Verse_Interpreter.Model.Build;
using Verse_Interpreter.Model.Render;
using Verse_Interpreter.Model.Rewrite;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

/// <summary>
/// Class <see cref="Rewriter"/> serves as an <see cref="ISyntaxTreeNodeVisitor"/> that
/// rewrites <see cref="Expression"/> instances using rewrite rules described in the
/// Verse Paper from March 2023.
/// </summary>
public class Rewriter : IRewriter, ISyntaxTreeNodeVisitor
{
    /// <summary>
    /// Field <c>_renderer</c> represents the component for displaying messages to the user.
    /// </summary>
    private readonly IRenderer _renderer;

    /// <summary>
    /// Field <c>_variableFactory</c> represents the component used for retrieving fresh variables.
    /// </summary>
    private readonly IVariableFactory _variableFactory;

    /// <summary>
    /// Field <c>_rewriteRules</c> represents the collection of rewrite rules in the form of methods.
    /// </summary>
    private readonly Func<Expression, Expression>[] _rewriteRules;

    /// <summary>
    /// Field <c>_variablesAnalyser</c> represents a helper component used to determine the
    /// free and bound variables of expressions.
    /// </summary>
    private readonly VariablesAnalyser _variablesAnalyser = new();

    /// <summary>
    /// Field <c>_currentVerseProgram</c> represents the Verse program that is currently being rewritten.
    /// </summary>
    private VerseProgram? _currentVerseProgram;

    /// <summary>
    /// Initialises a new instance of the <see cref="Rewriter"/> class.
    /// </summary>
    /// <param name="renderer"><c>renderer</c> represents the component for displaying messages to the user.</param>
    /// <param name="variableFactory"><c>variableFactory</c> represents the component used for retrieving fresh variables.</param>
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

    /// <summary>
    /// Property <c>Renderer</c> represents the component for displaying messages to the user.
    /// </summary>
    private IRenderer Renderer => _renderer;

    /// <summary>
    /// Property <c>VariablesAnalyser</c> represents a helper component used to determine the
    /// free and bound variables of expressions.
    /// </summary>
    private VariablesAnalyser VariablesAnalyser => _variablesAnalyser;

    /// <summary>
    /// Property <c>RuleApplied</c> represents the value indicating whether or not a rule has been previously applied.
    /// </summary>
    private bool RuleApplied { get; set; }

    /// <summary>
    /// Property <c>_currentVerseProgram</c> represents the Verse program that is currently being rewritten.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// Is raised when <c>_currentVerseProgram</c> or <c>value</c> is null.
    /// </exception>
    private VerseProgram CurrentVerseProgram
    {
        get
        {
            return _currentVerseProgram
                ?? throw new ArgumentNullException(nameof(_currentVerseProgram), "Is null");
        }

        set
        {
            _currentVerseProgram = value
                ?? throw new ArgumentNullException(nameof(value), "Cannot be null.");
        }
    }

    /// <summary>
    /// This method continuosly applies rewrite rules while they match and returns the
    /// child expression of <paramref name="verseProgram"/> when done.
    /// </summary>
    /// <param name="verseProgram"><c>verseProgram</c> represents the Verse program to rewrite ("execute").</param>
    /// <returns>The result of rewriting the <paramref name="verseProgram"/>.</returns>
    public Expression Rewrite(VerseProgram verseProgram)
    {
        CurrentVerseProgram = verseProgram;

        do
        {
            RuleApplied = false;
            verseProgram.E = ApplyRules(verseProgram.E);

            if (!RuleApplied)
                verseProgram.E.Accept(this);

            if (RuleApplied)
                Renderer.DisplayIntermediateResult(verseProgram.E);
        }
        while (RuleApplied);

        return verseProgram.E;
    }

    /// <summary>
    /// This method applies rewrite rules to <paramref name="eq"/> if it is an <see cref="Expression"/>.
    /// Otherwise returns <paramref name="eq"/> unchanged.
    /// </summary>
    /// <param name="eq"><c>eq</c> represents the <see cref="IExpressionOrEquation"/> to possibly apply rewrite rules on.</param>
    /// <returns>The result of this rewrite attempt.</returns>
    public IExpressionOrEquation ApplyRules(IExpressionOrEquation eq)
    {
        if (eq is Expression e)
            return ApplyRules(e);

        return eq;
    }

    /// <summary>
    /// This method tries to match every rewrite rule on <paramref name="expression"/> until one matches successfully.
    /// </summary>
    /// <param name="expression"><c>expression</c> represents the target of this rewriting.</param>
    /// <returns>The rewritten <see cref="Expression"/> or the <paramref name="expression"/> unchanged
    /// if no rewrite rule matched at all.</returns>
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

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Variable _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Integer _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(VerseString _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Operator _) { }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(VerseTuple _) { }

    /// <summary>
    /// This method tries to apply rules to <paramref name="lambda"/> and its child expressions until
    /// a rewrite rule successfully matched and applied.
    /// </summary>
    /// <param name="lambda"><c>lambda</c> is the target of this rewriting attempt.</param>
    public void Visit(Lambda lambda)
    {
        lambda.E = ApplyRules(lambda.E);

        if (RuleApplied)
            return;

        lambda.E.Accept(this);
    }

    /// <summary>
    /// This method tries to apply rules to <paramref name="equation"/> and its child expressions until
    /// a rewrite rule successfully matched and applied.
    /// </summary>
    /// <param name="equation"><c>equation</c> is the target of this rewriting attempt.</param>
    public void Visit(Equation equation)
    {
        equation.E = ApplyRules(equation.E);

        if (RuleApplied)
            return;

        equation.E.Accept(this);
    }

    /// <summary>
    /// This method tries to apply rules to <paramref name="eqe"/> and its child expressions until
    /// a rewrite rule successfully matched and applied.
    /// </summary>
    /// <param name="eqe"><c>eqe</c> is the target of this rewriting attempt.</param>
    public void Visit(Eqe eqe)
    {
        eqe.Eq = ApplyRules(eqe.Eq);

        if (RuleApplied)
            return;

        eqe.Eq.Accept(this);

        if (RuleApplied)
            return;

        eqe.E = ApplyRules(eqe.E);

        if (RuleApplied)
            return;

        eqe.E.Accept(this);
    }

    /// <summary>
    /// This method tries to apply rules to <paramref name="exists"/> and its child expressions until
    /// a rewrite rule successfully matched and applied.
    /// </summary>
    /// <param name="exists"><c>exists</c> is the target of this rewriting attempt.</param>
    public void Visit(Exists exists)
    {
        exists.E = ApplyRules(exists.E);

        if (RuleApplied)
            return;

        exists.E.Accept(this);
    }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Fail _) { }

    /// <summary>
    /// This method tries to apply rules to <paramref name="choice"/> and its child expressions until
    /// a rewrite rule successfully matched and applied.
    /// </summary>
    /// <param name="choice"><c>choice</c> is the target of this rewriting attempt.</param>
    public void Visit(Choice choice)
    {
        choice.E1 = ApplyRules(choice.E1);

        if (RuleApplied)
            return;

        choice.E1.Accept(this);

        if (RuleApplied)
            return;

        choice.E2 = ApplyRules(choice.E2);

        if (RuleApplied)
            return;

        choice.E2.Accept(this);
    }

    /// <summary>
    /// This method does nothing.
    /// </summary>
    /// <param name="_"><c>_</c> represents an unused argument.</param>
    public void Visit(Application _) { }

    /// <summary>
    /// This method simply calls <see cref="VisitWrapper(Wrapper)"/> with <paramref name="one"/> to try
    /// to apply rules to <paramref name="one"/> and its child expressions until a rewrite rule
    /// successfully matched and applied.
    /// </summary>
    /// <param name="one"><c>one</c> is the target of this rewriting attempt.</param>
    public void Visit(One one) => VisitWrapper(one);

    /// <summary>
    /// This method simply calls <see cref="VisitWrapper(Wrapper)"/> with <paramref name="all"/> to try
    /// to apply rules to <paramref name="all"/> and its child expressions until a rewrite rule
    /// successfully matched and applied.
    /// </summary>
    /// <param name="all"><c>all</c> is the target of this rewriting attempt.</param>
    public void Visit(All all) => VisitWrapper(all);

    /// <summary>
    /// This method tries to apply rules to <paramref name="wrapper"/> and its child expressions until
    /// a rewrite rule successfully matched and applied.
    /// </summary>
    /// <param name="wrapper"><c>wrapper</c> is the target of this rewriting attempt.</param>
    private void VisitWrapper(Wrapper wrapper)
    {
        wrapper.E = ApplyRules(wrapper.E);

        if (RuleApplied)
            return;

        wrapper.E.Accept(this);
    }

    #region Application

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "APP-ADD".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "APP-SUB".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "APP-MULT".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "APP-DIV".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "APP-GT".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "APP-LT".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "APP-GT-FAIL".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "APP-LT-FAIL".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "APP-BETA".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression AppBeta(Expression expression)
    {
        if (expression is Application { V1: Lambda { E: Expression e } lambda, V2: Value v })
        {
            if (VariablesAnalyser.FreeVariablesOf(v).Contains(lambda.Parameter))
                ApplyAlphaConversionWithoutCapturingVariablesOfValue(lambda, v);

            expression = new Exists
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

            OnRuleApplied("APP-BETA");
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "APP-TUP".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression AppTup(Expression expression)
    {
        if (expression is Application { V1: VerseTuple tuple, V2: Value value })
        {
            int count = tuple.Count();

            if (count > 0)
            {
                OnRuleApplied("APP-TUP");

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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "APP-TUP-0".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression AppTup0(Expression expression)
    {
        if (expression is Application { V1: VerseTuple tuple, V2: Value })
        {
            if (!tuple.Any())
            {
                expression = new Fail();
                
                OnRuleApplied("APP-TUP-0");
            }
        }

        return expression;
    }

    #endregion

    #region Unification

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "U-LIT".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression ULit(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: Integer k1, E: Integer k2 }, E: Expression e })
        {
            if (k1 == k2)
            {
                expression = e;

                OnRuleApplied("U-LIT");
            }
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "U-TUP".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression UTup(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: VerseTuple t1, E: VerseTuple t2 }, E: Expression e })
        {
            if (t1.Count() == t2.Count())
            {
                expression = BuildTupleEqeRecursively(t1.Zip(t2), e);

                OnRuleApplied("U-TUP");
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "U-FAIL".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression UFail(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: HeadNormalForm, E: HeadNormalForm }, E: Expression })
        {
            expression = new Fail();
            
            OnRuleApplied("U-FAIL");
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "U-OCCURS".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression UOccurs(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: Variable v, E: VerseTuple tuple }, E: Expression })
        {
            if (VariableOccursInVerseTuple(v, tuple))
            {
                expression = new Fail();
                
                OnRuleApplied("U-OCCURS");
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "SUBST".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression Subst(Expression expression)
    {
        (bool isFound, Eqe? eqe) = IsExecutionContextWithEquationIncludingHole(expression);

        if (isFound)
        {
            if (eqe is not Eqe { Eq: Equation { V: Variable x, E: Value v } equation, E: Expression e })
                throw new Exception("Final Eqe in substitute must match the rule.");

            if (VariablesAnalyser.FreeVariablesOf(expression).Contains(x)
                && VariablesAnalyser.FreeVariablesOf(e).Contains(x)
                && !VariablesAnalyser.FreeVariablesOf(v).Contains(x))
            {
                if (v is not Variable || (v is Variable y
                    && VariablesAnalyser.VariableBoundInsideVariable(CurrentVerseProgram.E, x, y)))
                {
                    SubstitutionHandler substitutionHandler = new(equation);
                    substitutionHandler.SubstituteButLeaveEquationUntouched(expression);

                    OnRuleApplied("SUBST");
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "HNF-SWAP".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression HnfSwap(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: HeadNormalForm hnf, E: Variable v } eq, E: Expression })
        {
            eq.V = v;
            eq.E = hnf;

            OnRuleApplied("HNF-SWAP");
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "VAR-SWAP".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression VarSwap(Expression expression)
    {
        if (expression is Eqe { Eq: Equation { V: Variable y, E: Variable x }, E: Expression e }
            && x != y && VariablesAnalyser.VariableBoundInsideVariable(CurrentVerseProgram.E, x, y))
        {
            expression = new Eqe
            {
                Eq = new Equation
                {
                    V = x,
                    E = y
                },
                E = e
            };

            OnRuleApplied("VAR-SWAP");
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "SEQ-SWAP".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression SeqSwap(Expression expression)
    {
        if (expression is Eqe { Eq: IExpressionOrEquation eq, E: Eqe { Eq: Equation { V: Variable x, E: Value v }, E: Expression e } })
        {
            if (eq is not Equation { V: Variable, E: Value }
            || (eq is Equation { V: Variable y, E: Value }
            && !(x == y || VariablesAnalyser.VariableBoundInsideVariable(CurrentVerseProgram.E, y, x))))
            {
                expression = new Eqe
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

                OnRuleApplied("SEQ-SWAP");
            }
        }

        return expression;
    }

    #endregion

    #region Elimination

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "VAL-ELIM".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression ValElim(Expression expression)
    {
        if (expression is Eqe { Eq: Value, E: Expression e })
        {
            expression = e;

            OnRuleApplied("VAL-ELIM");
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "EXI-ELIM".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression ExiElim(Expression expression)
    {
        if (expression is Exists { V: Variable v, E: Expression e } && !VariablesAnalyser.FreeVariablesOf(e).Contains(v))
        {
            expression = e;

            OnRuleApplied("EXI-ELIM");
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "EQN-ELIM".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
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

                    OnRuleApplied("EQN-ELIM");
                }
            }
        }

        return expression;
    }

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "FAIL-ELIM".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression FailElim(Expression expression)
    {
        if (IsExecutionContextFailingExcludingHole(expression))
        {
            expression = new Fail();

            OnRuleApplied("FAIL-ELIM");
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "EXI-FLOAT".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
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
            expression = exists;

            OnRuleApplied("EXI-FLOAT");
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

    /// <summary>
    /// Tries to rewrite the given expression using the rewrite rule "SEQ-ASSOC".
    /// </summary>
    /// <param name="expression">The given expression to rewrite.</param>
    /// <returns>The rewritten expression if the rule applies, or the unchanged expression if not.</returns>
    [RewriteRule]
    private Expression SeqAssoc(Expression expression)
    {
        if (expression is Eqe { Eq: Eqe { Eq: IExpressionOrEquation eq, E: Expression e1 }, E: Expression e2 })
        {
            expression = new Eqe
            {
                Eq = eq,
                E = new Eqe
                {
                    Eq = e1,
                    E = e2
                }
            };

            OnRuleApplied("SEQ-ASSOC");
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

            OnRuleApplied("EQN-FLOAT");
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

            OnRuleApplied("EXI-SWAP");
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

            OnRuleApplied("ONE-FAIL");
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

            OnRuleApplied("ONE-VALUE");
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

            OnRuleApplied("ONE-CHOICE");
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

            OnRuleApplied("ALL-FAIL");
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

            OnRuleApplied("ALL-VALUE");
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

            OnRuleApplied("ALL-CHOICE");
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

            OnRuleApplied("CHOOSE-R");
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

            OnRuleApplied("CHOOSE-L");
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

            OnRuleApplied("CHOOSE-ASSOC");
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

            OnRuleApplied("CHOOSE");
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

    /// <summary>
    /// This method sets <c>RuleApplied</c> to true and displays the <paramref name="ruleName"/> as an applied rule.
    /// </summary>
    /// <param name="ruleName"><c>ruleName</c> is the rule to display as applied.</param>
    private void OnRuleApplied(string ruleName)
    {
        RuleApplied = true;
        Renderer.DisplayRuleApplied(ruleName);
    }
}
