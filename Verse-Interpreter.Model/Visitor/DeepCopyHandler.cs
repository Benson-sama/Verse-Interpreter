using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Visitor;

public class DeepCopyHandler : ISyntaxTreeNodeVisitor<IExpressionOrEquation>
{
    private Choice? _excludedChoice = null;

    private Expression _choiceReplacement = new Fail();

    public DeepCopyHandler()
    {
    }

    public DeepCopyHandler(Choice excludedChoice)
        => _excludedChoice = excludedChoice;

    public Choice? ExcludedChoice => _excludedChoice;

    public Expression ChoiceReplacement => _choiceReplacement;

    public Expression DeepCopy(Expression expression)
    {
        _excludedChoice = null;

        return DeepCopyExpression(expression);
    }

    public Value DeepCopy(Value value)
    {
        _excludedChoice = null;

        return DeepCopyValue(value);
    }

    public Expression DeepCopyExceptChoice(Expression expression, Expression choiceReplacement)
    {
        _choiceReplacement = choiceReplacement;

        return DeepCopyExpression(expression);
    }

    public IExpressionOrEquation Visit(Variable variable)
        => new Variable(variable.Name);

    public IExpressionOrEquation Visit(Integer integer)
        => new Integer(integer.Value);

    public IExpressionOrEquation Visit(VerseString verseString)
        => new VerseString(verseString.Text);

    public IExpressionOrEquation Visit(VerseTuple verseTuple)
    {
        Value[] values = verseTuple.Values.ToArray();

        for (int i = 0; i < values.Length; i++)
            values[i] = DeepCopyValue(values[i]);

        return new VerseTuple(values);
    }

    public IExpressionOrEquation Visit(Lambda lambda)
    {
        return new Lambda
        {
            Parameter = new Variable(lambda.Parameter.Name),
            E = DeepCopyExpression(lambda.E)
        };
    }

    public IExpressionOrEquation Visit(Add add) => new Add();

    public IExpressionOrEquation Visit(Sub sub) => new Sub();
    
    public IExpressionOrEquation Visit(Mult mult) => new Mult();
    
    public IExpressionOrEquation Visit(Div div) => new Div();
    
    public IExpressionOrEquation Visit(Gt gt) => new Gt();
    
    public IExpressionOrEquation Visit(Lt lt) => new Lt();
    
    public IExpressionOrEquation Visit(Equation equation)
    {
        return new Equation
        {
            V = DeepCopyValue(equation.V),
            E = DeepCopyExpression(equation.E)
        };
    }
    
    public IExpressionOrEquation Visit(Eqe eqe)
    {
        return new Eqe
        {
            Eq = DeepCopyExpressionOrEquation(eqe.Eq),
            E = DeepCopyExpression(eqe.E)
        };
    }
    
    public IExpressionOrEquation Visit(Exists exists)
    {
        return new Exists
        {
            V = new Variable(exists.V.Name),
            E = DeepCopyExpression(exists.E)
        };
    }
    
    public IExpressionOrEquation Visit(Fail fail)
        => new Fail();
    
    public IExpressionOrEquation Visit(Choice choice)
    {
        if (choice == ExcludedChoice)
            return ChoiceReplacement;

        return new Choice
        {
            E1 = DeepCopyExpression(choice.E1),
            E2 = DeepCopyExpression(choice.E2),
        };
    }
    
    public IExpressionOrEquation Visit(Application application)
    {
        return new Application
        {
            V1 = DeepCopyValue(application.V1),
            V2 = DeepCopyValue(application.V2)
        };
    }

    public IExpressionOrEquation Visit(One one)
        => new One { E = DeepCopyExpression(one.E) };

    public IExpressionOrEquation Visit(All all)
        => new All { E = DeepCopyExpression(all.E) };

    private Value DeepCopyValue(Value value)
    {
        if (value.Accept(this) is not Value newValue)
            throw new ArgumentNullException(nameof(value), "Cannot be null");

        return newValue;
    }

    private Expression DeepCopyExpression(Expression expression)
    {
        if (expression.Accept(this) is not Expression newExpression)
            throw new ArgumentNullException(nameof(expression), "Cannot be null");

        return newExpression;
    }

    private IExpressionOrEquation DeepCopyExpressionOrEquation(IExpressionOrEquation eq)
        => eq.Accept(this);
}
