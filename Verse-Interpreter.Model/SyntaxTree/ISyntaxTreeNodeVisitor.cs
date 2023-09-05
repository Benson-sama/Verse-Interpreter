using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms.Operators;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.SyntaxTree;

public interface ISyntaxTreeNodeVisitor
{
    void Visit(Variable variable);

    void Visit(Integer integer);

    void Visit(VerseString verseString);

    void Visit(Operator verseOperator);

    void Visit(VerseTuple verseTuple);

    void Visit(Lambda lambda);

    void Visit(Equation equation);

    void Visit(Eqe eqe);

    void Visit(Exists exists);

    void Visit(Fail fail);

    void Visit(Choice choice);

    void Visit(Application application);

    void Visit(One one);

    void Visit(All all);
}

public interface ISyntaxTreeNodeVisitor<T>
{
    T Visit(Variable variable);

    T Visit(Integer integer);

    T Visit(VerseString verseString);

    T Visit(VerseTuple verseTuple);

    T Visit(Lambda lambda);

    T Visit(Add add);

    T Visit(Sub sub);

    T Visit(Mult mult);

    T Visit(Div div);

    T Visit(Gt gt);

    T Visit(Lt lt);

    T Visit(Equation equation);

    T Visit(Eqe eqe);

    T Visit(Exists exists);

    T Visit(Fail fail);

    T Visit(Choice choice);

    T Visit(Application application);

    T Visit(One one);

    T Visit(All all);
}
