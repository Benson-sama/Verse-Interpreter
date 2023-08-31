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

    public Expression ChoiceContext => _choiceContext;

    public Choice TargetChoice => _targetChoice;

    public void FloatChoiceToTheTopIn(Wrapper outerScopeContext)
    {
        if (outerScopeContext.E == ChoiceContext)
        {
            outerScopeContext.E = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);

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
        if (equation.E == ChoiceContext)
            equation.E = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);
        else
            equation.E.Accept(this);
    }

    public void Visit(Eqe eqe)
    {
        if (eqe.Eq == ChoiceContext)
            eqe.Eq = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);
        else
            eqe.Eq.Accept(this);

        if (eqe.E == ChoiceContext)
            eqe.E = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);
        else
            eqe.E.Accept(this);
    }

    public void Visit(Exists exists)
    {
        if (exists.E == ChoiceContext)
            exists.E = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);
        else
            exists.E.Accept(this);
    }

    public void Visit(Fail fail) { }

    public void Visit(Choice choice)
    {
        if (choice.E1 == ChoiceContext)
            choice.E1 = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);
        else
            choice.E1.Accept(this);

        if (choice.E2 == ChoiceContext)
            choice.E2 = DuplicateUsingDeepCopy(ChoiceContext, TargetChoice);
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
