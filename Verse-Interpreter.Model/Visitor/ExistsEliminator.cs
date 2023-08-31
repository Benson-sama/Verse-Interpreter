using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

public class ExistsEliminator : ISyntaxTreeNodeVisitor
{
    private readonly Exists _targetExists;

    public ExistsEliminator(Exists targetExists) => _targetExists = targetExists;

    public Exists TargetExists => _targetExists;

    public void EliminateExistsIn(Expression expression) => expression.Accept(this);

    public void Visit(Variable variable) { }

    public void Visit(Integer integer) { }

    public void Visit(VerseString verseString) { }

    public void Visit(Operator verseOperator) { }

    public void Visit(VerseTuple verseTuple) { }

    public void Visit(Lambda lambda) { }

    public void Visit(Equation equation)
    {
        if (equation.E == TargetExists)
            equation.E = TargetExists.E;
        else
            equation.E.Accept(this);
    }

    public void Visit(Eqe eqe)
    {
        if (eqe.Eq == TargetExists)
            eqe.Eq = TargetExists.E;
        else
            eqe.Eq.Accept(this);

        if (eqe.E == TargetExists)
            eqe.E = TargetExists.E;
        else
            eqe.E.Accept(this);
    }

    public void Visit(Exists exists) { }

    public void Visit(Fail fail) { }

    public void Visit(Choice choice) { }

    public void Visit(Application application) { }

    public void Visit(One one) { }

    public void Visit(All all) { }
}
