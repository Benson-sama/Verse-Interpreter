using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

public class AlphaConversionHandler : ISyntaxTreeNodeVisitor
{
    private readonly Variable _targetVariable;
    private readonly Variable _newVariable;

    public AlphaConversionHandler(Variable targetVariable, Variable newVariable)
        => (_targetVariable,  _newVariable) = (targetVariable, newVariable);

    public Variable TargetVariable => _targetVariable;

    public Variable NewVariable => _newVariable;

    public void ApplyAlphaConversionIncludingParameter(Lambda lambda)
    {
        lambda.Parameter = NewVariable;

        lambda.E.Accept(this);
    }

    public void ApplyAlphaConversionIncludingBinder(Exists exists)
    {
        exists.V = NewVariable;

        exists.E.Accept(this);
    }

    public void ApplyAlphaConversion(Expression expression) => expression.Accept(this);

    public void Visit(Variable variable) { }

    public void Visit(Integer integer) { }

    public void Visit(VerseString verseString) { }

    public void Visit(Operator verseOperator) { }

    public void Visit(VerseTuple verseTuple)
    {
        Value[] values = verseTuple.ToArray();

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] is Variable v && v == TargetVariable)
                values[i] = NewVariable;
            else
                values[i].Accept(this);
        }

        verseTuple.Values = values;
    }

    public void Visit(Lambda lambda)
    {
        if (lambda.Parameter is Variable v1 && v1 == TargetVariable)
            return;

        if (lambda.E is Variable v2 && v2 == TargetVariable)
            lambda.E = NewVariable;
        else
            lambda.E.Accept(this);
    }

    public void Visit(Equation equation)
    {
        if (equation.V is Variable v1 && v1 == TargetVariable)
            equation.V = NewVariable;
        else
            equation.V.Accept(this);

        if (equation.E is Variable v2 && v2 == TargetVariable)
            equation.E = NewVariable;
        else
            equation.E.Accept(this);
    }

    public void Visit(Eqe eqe)
    {
        if (eqe.Eq is Variable v1 && v1 == TargetVariable)
            eqe.Eq = NewVariable;
        else
            eqe.Eq.Accept(this);

        if (eqe.E is Variable v2 && v2 == TargetVariable)
            eqe.E = NewVariable;
        else
            eqe.E.Accept(this);
    }

    public void Visit(Exists exists)
    {
        if (exists.V == TargetVariable)
            return;

        if (exists.E is Variable v && v == TargetVariable)
            exists.E = NewVariable;
        else
            exists.E.Accept(this);
    }

    public void Visit(Fail fail) => throw new NotImplementedException();

    public void Visit(Choice choice)
    {
        if (choice.E1 is Variable v1 && v1 == TargetVariable)
            choice.E1 = NewVariable;
        else
            choice.E1.Accept(this);

        if (choice.E2 is Variable v2 && v2 == TargetVariable)
            choice.E2 = NewVariable;
        else
            choice.E2.Accept(this);
    }

    public void Visit(Application application)
    {
        if (application.V1 is Variable v1 && v1 == TargetVariable)
            application.V1 = NewVariable;
        else
            application.V1.Accept(this);

        if (application.V2 is Variable v2 && v2 == TargetVariable)
            application.V2 = NewVariable;
        else
            application.V2.Accept(this);
    }

    public void Visit(One one) => VisitWrapper(one);

    public void Visit(All all) => VisitWrapper(all);

    private void VisitWrapper(Wrapper wrapper)
    {
        if (wrapper.E is Variable v && v == TargetVariable)
            wrapper.E = NewVariable;
        else
            wrapper.E.Accept(this);
    }
}
