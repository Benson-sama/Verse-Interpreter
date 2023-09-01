using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

public class VariablesAnalyser : ISyntaxTreeNodeVisitor
{
    private Expression? _finalExpression;

    private readonly List<Variable> _boundVariables = new();

    private readonly List<Variable> _freeVariables = new();

    private bool IsFinalExpressionReached { get; set; }

    public IEnumerable<Variable> BoundVariables { get => _boundVariables; }

    public IEnumerable<Variable> FreeVariables { get => _freeVariables; }

    public void Reset()
    {
        IsFinalExpressionReached = false;

        _boundVariables.Clear();
        _freeVariables.Clear();
    }

    public IEnumerable<Variable> FreeVariablesOf(Expression expression, Expression? finalExpression = null)
    {
        AnalyseVariablesOf(expression, finalExpression);

        return FreeVariables;
    }

    public void AnalyseVariablesOf(Expression expression, Expression? finalExpression = null)
    {
        _finalExpression = finalExpression;

        Reset();
        AcceptUnlessFinalExpressionReached(expression);
    }

    public bool IsBound(Variable variable)
        => BoundVariables.Contains(variable);

    public bool IsFree(Variable variable)
        => !IsBound(variable);

    public void Visit(Variable variable)
    {
        if (!_boundVariables.Contains(variable) && !FreeVariables.Contains(variable))
            _freeVariables.Add(variable);
    }

    public void Visit(Integer integer) { }

    public void Visit(VerseString verseString) { }

    public void Visit(Operator verseOperator) { }

    public void Visit(VerseTuple verseTuple)
    {
        foreach (Value value in verseTuple)
            AcceptUnlessFinalExpressionReached(value);
    }

    public void Visit(Lambda lambda)
    {
        _boundVariables.Add(lambda.Parameter);

        AcceptUnlessFinalExpressionReached(lambda.E);
    }

    public void Visit(Equation equation)
    {
        AcceptUnlessFinalExpressionReached(equation.V);
        AcceptUnlessFinalExpressionReached(equation.E);
    }

    public void Visit(Eqe eqe)
    {
        AcceptUnlessFinalExpressionReached(eqe.Eq);
        AcceptUnlessFinalExpressionReached(eqe.E);
    }

    public void Visit(Exists exists)
    {
        _boundVariables.Add(exists.V);

        AcceptUnlessFinalExpressionReached(exists.E);
    }

    public void Visit(Fail fail) { }

    public void Visit(Choice choice)
    {
        AcceptUnlessFinalExpressionReached(choice.E1);
        AcceptUnlessFinalExpressionReached(choice.E2);
    }

    public void Visit(Application application)
    {
        AcceptUnlessFinalExpressionReached(application.V1);
        AcceptUnlessFinalExpressionReached(application.V2);
    }

    public void Visit(One one) => VisitWrapper(one);

    public void Visit(All all) => VisitWrapper(all);

    private void VisitWrapper(Wrapper wrapper) => AcceptUnlessFinalExpressionReached(wrapper.E);

    private void AcceptUnlessFinalExpressionReached(IExpressionOrEquation eq)
    {
        if (eq == _finalExpression)
        {
            IsFinalExpressionReached = true;
            return;
        }
        else if (IsFinalExpressionReached)
        {
            return;
        }

        eq.Accept(this);
    }
}
