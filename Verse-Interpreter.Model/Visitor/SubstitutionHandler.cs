using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

public class SubstitutionHandler : ISyntaxTreeNodeVisitor
{
    private readonly Equation _originalEquation;
    private readonly Variable _targetVariable;
    private readonly Value _replacingValue;
    private readonly DeepCopyHandler _deepCopyHandler = new();

    public SubstitutionHandler(Equation originalEquation)
    {
        if (originalEquation.V is not Variable variable || originalEquation.E is not Value value)
            throw new Exception("Equation must consist of Variable=Value when used in substitution.");

        (_originalEquation, _targetVariable, _replacingValue) = (originalEquation, variable, value);
    }

    public Equation OriginalEquation => _originalEquation;

    public Variable TargetVariable => _targetVariable;

    public Value ReplacingValue => _replacingValue;

    private DeepCopyHandler DeepCopyHandler => _deepCopyHandler;

    public void SubstituteButLeaveEquationUntouched(IExpressionOrEquation eq) => eq.Accept(this);

    public void Visit(Variable variable) { }

    public void Visit(Integer integer) { }

    public void Visit(VerseString verseString) { }

    public void Visit(Operator verseOperator) { }

    public void Visit(VerseTuple verseTuple)
    {
        Value[] values = verseTuple.ToArray();

        for (int i = 0; i < values.Length; i++)
        {
            if (IsTargetVariable(values[i]))
                values[i] = DeepCopiedValue();
            else
                values[i].Accept(this);
        }

        verseTuple.Values = values;
    }

    public void Visit(Lambda lambda)
    {
        if (IsTargetVariable(lambda.Parameter))
            return;

        if (IsTargetVariable(lambda.E))
            lambda.E = DeepCopiedValue();
        else
            AcceptUnlessEquation(lambda.E);
    }

    public void Visit(Equation equation)
    {
        if (IsTargetVariable(equation.V))
            equation.V = DeepCopiedValue();
        else
            equation.V.Accept(this);

        if (IsTargetVariable(equation.E))
            equation.E = DeepCopiedValue();
        else
            AcceptUnlessEquation(equation.E);
    }

    public void Visit(Eqe eqe)
    {
        if (IsTargetVariable(eqe.Eq))
            eqe.Eq = DeepCopiedValue();
        else
            AcceptUnlessEquation(eqe.Eq);

        if (IsTargetVariable(eqe.E))
            eqe.E = DeepCopiedValue();
        else
            AcceptUnlessEquation(eqe.E);
    }

    public void Visit(Exists exists)
    {
        if (IsTargetVariable(exists.V))
            return;

        if (IsTargetVariable(exists.E))
            exists.E = DeepCopiedValue();
        else
            AcceptUnlessEquation(exists.E);
    }

    public void Visit(Fail fail) { }

    public void Visit(Choice choice)
    {
        if (IsTargetVariable(choice.E1))
            choice.E1 = DeepCopiedValue();
        else
            AcceptUnlessEquation(choice.E1);

        if (IsTargetVariable(choice.E2))
            choice.E2 = DeepCopiedValue();
        else
            AcceptUnlessEquation(choice.E2);
    }

    public void Visit(Application application)
    {
        if (IsTargetVariable(application.V1))
            application.V1 = DeepCopiedValue();
        else
            application.V1.Accept(this);

        if (IsTargetVariable(application.V2))
            application.V2 = DeepCopiedValue();
        else
            application.V2.Accept(this);
    }

    public void Visit(One one) => VisitWrapper(one);

    public void Visit(All all) => VisitWrapper(all);

    public void VisitWrapper(Wrapper wrapper)
    {
        if (IsTargetVariable(wrapper.E))
            wrapper.E = DeepCopiedValue();
        else
            AcceptUnlessEquation(wrapper.E);
    }

    private Value DeepCopiedValue() => DeepCopyHandler.DeepCopy(ReplacingValue);

    private void AcceptUnlessEquation(IExpressionOrEquation eq)
    {
        if (eq == OriginalEquation)
            return;

        eq.Accept(this);
    }

    private bool IsTargetVariable(IExpressionOrEquation eq)
    {
        if (eq is not Variable v)
            return false;

        return v == TargetVariable;
    }
}
