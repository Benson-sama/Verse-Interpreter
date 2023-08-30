using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;

namespace Verse_Interpreter.Model.Rewrite.Utility;

public static class ExistsElimination
{
    private static void ReplaceExistsWithItsExpression(this Eqe eqe, Exists exists)
    {
        if (eqe.Eq == exists)
            eqe.Eq = exists.E;
        else
            eqe.Eq.ReplaceExistsWithItsExpression(exists);

        if (eqe.E == exists)
            eqe.E = exists.E;
        else
            eqe.E.ReplaceExistsWithItsExpression(exists);
    }

    public static void ReplaceExistsWithItsExpression(this IExpressionOrEquation eq, Exists exists)
    {
        switch (eq)
        {
            case Eqe eqe:
                eqe.ReplaceExistsWithItsExpression(exists);
                break;
            case Equation equation:
                equation.ReplaceExistsWithItsExpression(exists);
                break;
        }
    }

    private static void ReplaceExistsWithItsExpression(this Equation equation, Exists exists)
    {
        if (equation.E == exists)
            equation.E = exists.E;
        else
            equation.E.ReplaceExistsWithItsExpression(exists);
    }
}
