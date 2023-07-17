using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Verse_Interpreter.Model;

public class VerseVisitor : IVerseVisitor<Node>
{
    public Node Visit(IParseTree tree) => throw new NotImplementedException();
    
    public Node VisitAssignmentExp([NotNull] VerseParser.AssignmentExpContext context)
    {
        Variable variable = new Variable(context.children[0].ToString());
        Expression e1 = context.children[2].Accept(this) as Expression;
        Expression e2 = context.children[4].Accept(this) as Expression;

        return new Exists()
        {
            V = variable,
            E = new Eqe()
            {
                Eq = new Equation()
                {
                    V = variable,
                    E = e1
                },
                E = e2
            }
        };
    }

    public Node VisitChildren(IRuleNode node) => throw new NotImplementedException();
    public Node VisitChoiceExp([NotNull] VerseParser.ChoiceExpContext context) => throw new NotImplementedException();
    public Node VisitComment([NotNull] VerseParser.CommentContext context) => throw new NotImplementedException();
    public Node VisitCommentExp([NotNull] VerseParser.CommentExpContext context) => throw new NotImplementedException();
    public Node VisitElements([NotNull] VerseParser.ElementsContext context) => throw new NotImplementedException();
    
    public Node VisitEqeExp([NotNull] VerseParser.EqeExpContext context)
    {
        throw new NotImplementedException();
    }

    public Node VisitErrorNode(IErrorNode node) => throw new NotImplementedException();
    public Node VisitExpApplicationExp([NotNull] VerseParser.ExpApplicationExpContext context) => throw new NotImplementedException();
    public Node VisitExpEquationExp([NotNull] VerseParser.ExpEquationExpContext context) => throw new NotImplementedException();
    public Node VisitFailExp([NotNull] VerseParser.FailExpContext context) => throw new NotImplementedException();
    public Node VisitForExp([NotNull] VerseParser.ForExpContext context) => throw new NotImplementedException();
    public Node VisitGreaterThanExp([NotNull] VerseParser.GreaterThanExpContext context) => throw new NotImplementedException();
    public Node VisitHnf([NotNull] VerseParser.HnfContext context) => throw new NotImplementedException();
    public Node VisitIfElseExp([NotNull] VerseParser.IfElseExpContext context) => throw new NotImplementedException();
    public Node VisitLambda([NotNull] VerseParser.LambdaContext context) => throw new NotImplementedException();
    public Node VisitLessThanExp([NotNull] VerseParser.LessThanExpContext context) => throw new NotImplementedException();
    public Node VisitMultDivExp([NotNull] VerseParser.MultDivExpContext context) => throw new NotImplementedException();
    public Node VisitParenthesisExp([NotNull] VerseParser.ParenthesisExpContext context) => throw new NotImplementedException();
    public Node VisitPlusMinusExp([NotNull] VerseParser.PlusMinusExpContext context) => throw new NotImplementedException();
    
    public Node VisitProgram([NotNull] VerseParser.ProgramContext context)
    {
        if (context.children[0].Accept(this) is not Expression e)
            throw new Exception("The node contained in a program must be an expression.");

        return new VerseProgram()
        {
            Wrapper = new One()
            {
                E = e
            }
        };
    }

    public Node VisitRangeChoiceExp([NotNull] VerseParser.RangeChoiceExpContext context) => throw new NotImplementedException();
    public Node VisitTerminal(ITerminalNode node) => throw new NotImplementedException();
    public Node VisitTuple([NotNull] VerseParser.TupleContext context) => throw new NotImplementedException();
    public Node VisitV([NotNull] VerseParser.VContext context) => throw new NotImplementedException();
    public Node VisitValueApplicationExp([NotNull] VerseParser.ValueApplicationExpContext context) => throw new NotImplementedException();
    public Node VisitValueExp([NotNull] VerseParser.ValueExpContext context) => throw new NotImplementedException();
}
