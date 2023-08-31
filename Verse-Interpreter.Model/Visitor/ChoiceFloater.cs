using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

public class ChoiceFloater : ISyntaxTreeNodeVisitor
{
    private readonly Expression _choiceContext;

    private readonly Choice _targetChoice;

    public ChoiceFloater(Expression choiceContext, Choice targetChoice)
        => (_choiceContext, _targetChoice) = (choiceContext, targetChoice);

    public void FloatChoiceToTheTop(Wrapper outerScopeContext)
    {
        if (outerScopeContext.E == _choiceContext)
        {
            outerScopeContext.E = DuplicateUsingDeepCopy(_choiceContext, _targetChoice);

            return;
        }

        outerScopeContext.E.Accept(this);
    }

    public void Visit(Variable variable) { }

    public void Visit(Integer integer) { }

    public void Visit(VerseString verseString) { }

    public void Visit(Operator verseOperator) { }

    public void Visit(VerseTuple verseTuple) { }

    public void Visit(Lambda lambda) { }

    public void Visit(Equation equation)
    {
        if (equation.E == _choiceContext)
            equation.E = DuplicateUsingDeepCopy(_choiceContext, _targetChoice);
        else
            equation.E.Accept(this);
    }

    public void Visit(Eqe eqe)
    {
        if (eqe.Eq == _choiceContext)
            eqe.Eq = DuplicateUsingDeepCopy(_choiceContext, _targetChoice);
        else
            eqe.Eq.Accept(this);

        if (eqe.E == _choiceContext)
            eqe.E = DuplicateUsingDeepCopy(_choiceContext, _targetChoice);
        else
            eqe.E.Accept(this);
    }

    public void Visit(Exists exists)
    {
        if (exists.E == _choiceContext)
            exists.E = DuplicateUsingDeepCopy(_choiceContext, _targetChoice);
        else
            exists.E.Accept(this);
    }

    public void Visit(Fail fail) { }

    public void Visit(Choice choice)
    {
        if (choice.E1 == _choiceContext)
            choice.E1 = DuplicateUsingDeepCopy(_choiceContext, _targetChoice);
        else
            choice.E1.Accept(this);

        if (choice.E2 == _choiceContext)
            choice.E2 = DuplicateUsingDeepCopy(_choiceContext, _targetChoice);
        else
            choice.E2.Accept(this);
    }

    public void Visit(Application application) { }

    public void Visit(One one) { }

    public void Visit(All all) { }

    private static Expression DuplicateUsingDeepCopy(Expression choiceContext, Choice choice)
    {
        return new Choice
        {
            E1 = choiceContext.DeepCopyButReplaceChoice(choice, choice.E1),
            E2 = choiceContext.DeepCopyButReplaceChoice(choice, choice.E2)
        };
    }
}
