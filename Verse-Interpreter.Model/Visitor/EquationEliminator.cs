using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

public class EquationEliminator : ISyntaxTreeNodeVisitor
{
    private readonly Eqe _targetEqe;

    public EquationEliminator(Eqe targetEqe) => _targetEqe = targetEqe;

    public Eqe TargetEqe => _targetEqe;

    public void EliminateEquationIn(Expression expression) => expression.Accept(this);

    public void Visit(Variable variable) { }

    public void Visit(Integer integer) { }

    public void Visit(VerseString verseString) { }

    public void Visit(Operator verseOperator) { }

    public void Visit(VerseTuple verseTuple) { }

    public void Visit(Lambda lambda)
    {
        if (lambda.E == TargetEqe)
            lambda.E = TargetEqe.E;
        else
            lambda.E.Accept(this);
    }

    public void Visit(Equation equation)
    {
        if (equation.E == TargetEqe)
            equation.E = TargetEqe.E;
        else
            equation.E.Accept(this);
    }

    public void Visit(Eqe eqe)
    {
        if (eqe.Eq == TargetEqe)
            eqe.Eq = TargetEqe.E;
        else
            eqe.Eq.Accept(this);

        if (eqe.E == TargetEqe)
            eqe.E = TargetEqe.E;
        else
            eqe.E.Accept(this);
    }

    public void Visit(Exists exists)
    {
        if (exists.E == TargetEqe)
            exists.E = TargetEqe.E;
        else
            exists.E.Accept(this);
    }

    public void Visit(Fail fail) { }

    public void Visit(Choice choice)
    {
        if (choice.E1 == TargetEqe)
            choice.E1 = TargetEqe.E;
        else
            choice.E1.Accept(this);

        if (choice.E2 == TargetEqe)
            choice.E2 = TargetEqe.E;
        else
            choice.E2.Accept(this);
    }

    public void Visit(Application application) { }

    public void Visit(One one) => VisitWrapper(one);

    public void Visit(All all) => VisitWrapper(all);

    private void VisitWrapper(Wrapper wrapper)
    {
        if (wrapper.E == TargetEqe)
            wrapper.E = TargetEqe.E;
        else
            wrapper.E.Accept(this);
    }
}
