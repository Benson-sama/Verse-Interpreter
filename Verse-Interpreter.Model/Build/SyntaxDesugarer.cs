//-------------------------------------------------------------------
// <copyright file="SyntaxDesugarer.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the SyntaxDesugarer class.</summary>
//-------------------------------------------------------------------

using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Build;

/// <summary>
/// Class <see cref="SyntaxDesugarer"/> serves as a helper class to desugar
/// Verse expressions that are not syntactically legal in the Verse Calculus.
/// It is described on page 3 of "The Verse Calculus: a Core Calculus for Functional Logic Programming"
/// </summary>
public class SyntaxDesugarer
{
    /// <summary>
    /// Field <c>_variableFactory</c> represents the <see cref="Variable"/> factory used to retrieve fresh variables.
    /// </summary>
    private readonly IVariableFactory _variableFactory;

    /// <summary>
    /// Initialises a new instance of the <see cref="SyntaxDesugarer"/> class.
    /// </summary>
    /// <param name="variableFactory"><c>variableFactory</c> is the factory used to retrieve fresh variables.</param>
    public SyntaxDesugarer(IVariableFactory variableFactory)
        => _variableFactory = variableFactory;

    /// <summary>
    /// This method desugars the addition of two expressions.
    /// (e1 + e2 means add⟨e1, e2⟩)
    /// </summary>
    /// <param name="e1"><c>e1</c> is the first operand.</param>
    /// <param name="e2"><c>e2</c> is the second operand.</param>
    /// <returns>The desugared addition <see cref="Expression"/>.</returns>
    public Expression DesugarPlus(Expression e1, Expression e2) =>
        DesugarExpressionApplication(new Add(), DesugarExpressionTuple(new Expression[] { e1, e2 }));

    /// <summary>
    /// This method desugars the greater than comparison of two <see cref="Expression"/>.
    /// (e1 > e2 means gt⟨e1, e2⟩)
    /// </summary>
    /// <param name="e1"><c>e1</c> is the first operand.</param>
    /// <param name="e2"><c>e2</c> is the second operand.</param>
    /// <returns>The desugared greater than comparison <see cref="Expression"/>.</returns>
    public Expression DesugarGreaterThan(Expression e1, Expression e2) =>
        DesugarExpressionApplication(new Gt(), DesugarExpressionTuple(new Expression[] { e1, e2 }));

    /// <summary>
    /// This method desugars an exists with multiple variables into several <see cref="Exists"/>.
    /// (∃x1 x2 ··· xn. e means ∃x1. ∃x2. ···∃xn. e)
    /// </summary>
    /// <param name="variables"><c>variables</c> are the variables for which the exists are constructed.</param>
    /// <param name="e"><c>e</c> is the <see cref="Expression"/> where the <paramref name="variables"/> are in scope.</param>
    /// <returns>The desugared multiple exists or simply <paramref name="e"/> if the number of
    /// <paramref name="variables"/> are zero.</returns>
    public static Expression DesugarMultipleExists(IEnumerable<Variable> variables, Expression e)
    {
        if (variables.Count() is 0)
            return e;

        return new Exists
        {
            V = variables.First(),
            E = DesugarMultipleExists(variables.Skip(1), e)
        };
    }

    /// <summary>
    /// This method desugars the assignment of an <see cref="Expression"/> with a second expression in sequence.
    /// (x := e1; e2 means ∃x. x = e1; e2)
    /// </summary>
    /// <param name="x"><c>x</c> is the <see cref="Variable"/> that gets assigned.</param>
    /// <param name="e1"><c>e1</c> is the assigned <see cref="Expression"/> to <paramref name="x"/>.</param>
    /// <param name="e2"><c>e2</c> is the <see cref="Expression"/> sequenced after the assignment.</param>
    /// <returns>The desugared <see cref="Expression"/>.</returns>
    public static Exists DesugarAssignment(Variable x, Expression e1, Expression e2)
    {
        return new Exists
        {
            V = x,
            E = new Eqe
            {
                Eq = new Equation
                {
                    V = x,
                    E = e1
                },
                E = e2
            }
        };
    }

    /// <summary>
    /// This method desugars the application of an <see cref="Expression"/> to another <see cref="Expression"/>.
    /// (e1 e2 means f := e1; x := e2; f x)
    /// </summary>
    /// <param name="e1"><c>e1</c> is the <see cref="Expression"/> that gets <paramref name="e2"/> applied.</param>
    /// <param name="e2"><c>e2</c> is the <see cref="Expression"/> that gets applied to <paramref name="e1"/>.</param>
    /// <returns>The desugared <see cref="Expression"/>.</returns>
    public Expression DesugarExpressionApplication(Expression e1, Expression e2)
    {
        Variable f = _variableFactory.Next();
        _variableFactory.RegisterUsedName(f.Name);
        Variable x = _variableFactory.Next();
        _variableFactory.RegisterUsedName(x.Name);

        Application application = new()
        {
            V1 = f,
            V2 = x
        };

        return DesugarAssignment(f, e1, DesugarAssignment(x, e2, application));
    }

    /// <summary>
    /// This method desugars an an expression tuple.
    /// (⟨e1, ···, en⟩ means x1 := e1; ···; xn := en; ⟨x1, ···, xn⟩)
    /// </summary>
    /// <param name="expressions"><c>expressions</c> are the elements of the tuple.</param>
    /// <returns>The desugared expression tuple.</returns>
    /// <exception cref="Exception">Is raised when the number of <paramref name="expressions"/> is zero.</exception>
    public Exists DesugarExpressionTuple(IEnumerable<Expression> expressions)
    {
        if (expressions.Count() is 0)
            throw new Exception("Expression tuple should never be empty in desugaring.");

        return BuildExpressionTupleRecursively(expressions, Enumerable.Empty<Variable>());
    }

    /// <summary>
    /// This method desugars an expression equation.
    /// (e1 = e2 means x := e1; x = e2; x)
    /// </summary>
    /// <param name="e1"><c>e1</c> is the left side <see cref="Expression"/> of the <see cref="Equation"/>.</param>
    /// <param name="e2"><c>e2</c> is the right side <see cref="Expression"/> of the <see cref="Equation"/>.</param>
    /// <returns>The desugared expression equation.</returns>
    public Expression DesugarExpressionEquation(Expression e1, Expression e2)
    {
        Variable x = _variableFactory.Next();
        _variableFactory.RegisterUsedName(x.Name);
        Eqe eqe = new()
        {
            Eq = new Equation
            {
                V = x,
                E = e2
            },
            E = x
        };

        return DesugarAssignment(x, e1, eqe);
    }

    /// <summary>
    /// This method desugars a lambda with multiple parameters.
    /// (𝜆⟨x1, ···, xn⟩. e means 𝜆p. ∃x1 ··· xn. p = ⟨x1, ···, xn⟩; e)
    /// </summary>
    /// <param name="parameters"><c>parameters</c> are the parameters of the lambda.</param>
    /// <param name="e"><c>e</c> represents the body of the lambda expression.</param>
    /// <returns>The desugared lambda.</returns>
    public Lambda DesugarLambda(IEnumerable<Variable> parameters, Expression e)
    {
        Variable p = _variableFactory.Next();
        _variableFactory.RegisterUsedName(p.Name);

        Eqe eqe = new()
        {
            Eq = new Equation
            {
                V = p,
                E = new VerseTuple(parameters)
            },
            E = e
        };

        return new Lambda
        {
            Parameter = p,
            E = DesugarMultipleExists(parameters, eqe)
        };
    }

    /// <summary>
    /// This method desugars an if then else <see cref="Expression"/>.
    /// (if (∃x1 ···xn. e1) then e2 else e3 means (one{(∃x1 ···xn. e1; 𝜆⟨⟩. e2) (𝜆⟨⟩. e3)})⟨⟩)
    /// </summary>
    /// <param name="e1"><c>e1</c> is the conditional <see cref="Expression"/>.</param>
    /// <param name="e2"><c>e2</c> is the <see cref="Expression"/> that gets evaluated if <paramref name="e1"/> succeeds.
    /// (Yields any values)</param>
    /// <param name="e3"><c>e2</c> is the <see cref="Expression"/> that gets evaluated if <paramref name="e1"/> fails.</param>
    /// <returns>The desugared if then else <see cref="Expression"/>.</returns>
    public Expression DesugarIfThenElse(Expression e1, Expression e2, Expression e3)
    {
        One one = new()
        {
            E = new Choice
            {
                E1 = new Eqe
                {
                    Eq = e1,
                    E = DesugarLambda(Enumerable.Empty<Variable>(), e2)
                },
                E2 = DesugarLambda(Enumerable.Empty<Variable>(), e3)
            }
        };

        return DesugarExpressionApplication(one, VerseTuple.Empty);
    }

    /// <summary>
    /// This method desugars a for do <see cref="Expression"/>.
    /// Note: As the interpreter is currently having troubles with recursion, this desugaring
    /// uses flatMap instead of map.
    /// (for(∃x1 ···xn. e1) do e2 means v := all{∃x1 ···xn. e1; 𝜆⟨⟩. e2 }; map⟨𝜆z. z⟨⟩, v⟩))
    /// </summary>
    /// <param name="e1"><c>e1</c> is the <see cref="Expression"/> inside the for.</param>
    /// <param name="e2"><c>e2</c> is the <see cref="Expression"/> after the do.</param>
    /// <returns></returns>
    public Expression DesugarForDo(Expression e1, Expression e2)
    {
        Variable v = _variableFactory.Next();
        _variableFactory.RegisterUsedName(v.Name);
        Variable z = _variableFactory.Next();
        _variableFactory.RegisterUsedName(z.Name);
        All all = new()
        {
            E = new Eqe
            {
                Eq = e1,
                E = DesugarLambda(Enumerable.Empty<Variable>(), e2)
            }
        };
        Lambda f = DesugarLambda(Enumerable.Repeat(z, 1), new Application { V1 = z, V2 = VerseTuple.Empty });
        All flatMap = DesugarFlatMap(f, v);

        return DesugarAssignment(v, all, flatMap);
    }

    /// <summary>
    /// This method desugars the head function.
    /// (head (𝑥𝑠) := 𝑥𝑠 (0))
    /// </summary>
    /// <param name="xs"><c>xs</c> is the parameter of the head function.</param>
    /// <returns>The desugared head function.</returns>
    public static Application DesugarHead(Variable xs)
    {
        return new Application
        {
            V1 = xs,
            V2 = new Integer(0)
        };
    }

    /// <summary>
    /// This method desugars the tail function.
    /// (tail(𝑥𝑠) := all{∃i. i > 0; 𝑥𝑠 (i)})
    /// </summary>
    /// <param name="xs"><c>xs</c> is the parameter of the tail function.</param>
    /// <returns>The desugared tail function.</returns>
    public Expression DesugarTail(Variable xs)
    {
        Variable i = _variableFactory.Next();
        _variableFactory.RegisterUsedName(i.Name);

        return new All
        {
            E = new Exists
            {
                V = i,
                E = new Eqe
                {
                    Eq = new Application
                    {
                        V1 = new Gt(),
                        V2 = new VerseTuple(i, new Integer(0))
                    },
                    E = new Application
                    {
                        V1 = xs,
                        V2 = i
                    }
                }
            }
        };
    }

    /// <summary>
    /// This method desugars the cons function.
    /// (cons⟨x, 𝑥𝑠⟩ := all{x ∃i. 𝑥𝑠 (i)})
    /// </summary>
    /// <param name="x"><c>x</c> is the first parameter of the cons function.</param>
    /// <param name="xs"><c>xs</c> is the second parameter of the cons function.</param>
    /// <returns>The desugared cons function.</returns>
    public Expression DesugarCons(Variable x, Variable xs)
    {
        Variable i = _variableFactory.Next();

        return new All
        {
            E = new Choice
            {
                E1 = x,
                E2 = new Exists
                {
                    V = i,
                    E = new Application
                    {
                        V1 = xs,
                        V2 = i
                    }
                }
            }
        };
    }

    /// <summary>
    /// This method desugars the flatMap function.
    /// (flatMap⟨f , 𝑥𝑠⟩ := all{∃i. f (𝑥𝑠 (i))})
    /// </summary>
    /// <param name="f"><c>f</c> is the <see cref="Lambda"/> that gets mapped over <paramref name="xs"/>.</param>
    /// <param name="xs">Is the tuple onto which <paramref name="f"/> gets mapped over.</param>
    /// <returns>The desugared flatMap function.</returns>
    public All DesugarFlatMap(Lambda f, Variable xs)
    {
        Variable i = _variableFactory.Next();
        Variable j = _variableFactory.Next();

        return new All
        {
            E = new Exists
            {
                V = i,
                E = DesugarAssignment(j, new Application { V1 = xs, V2 = i }, new Application { V1 = f, V2 = new VerseTuple(j) })
            }
        };
    }

    /// <summary>
    /// This method desugars an expression tuple recursively.
    /// </summary>
    /// <param name="expressions"><c>expressions</c> are the elements of the expression tuple.</param>
    /// <param name="variables"><c>variables</c> is the collection of variables that have already been
    /// assigned via this method.</param>
    /// <returns>The desugared expression tuple.</returns>
    /// <exception cref="ArgumentException">Is raised when the number of <paramref name="expressions"/> is zero.</exception>
    private Exists BuildExpressionTupleRecursively(IEnumerable<Expression> expressions, IEnumerable<Variable> variables)
    {
        if (!expressions.Any())
            throw new ArgumentException("Cannot be empty.", nameof(expressions));

        Variable x = _variableFactory.Next();
        _variableFactory.RegisterUsedName(x.Name);

        if (expressions.Count() is 1)
            return DesugarAssignment(x, expressions.First(), new VerseTuple(variables.Append(x)));

        return DesugarAssignment(x, expressions.First(), BuildExpressionTupleRecursively(expressions.Skip(1), variables.Append(x)));
    }
}
