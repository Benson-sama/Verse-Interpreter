using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model.Build;

public interface IVerseSyntaxTreeBuilder
{
    VerseProgram BuildCustomSyntaxTree(VerseParser.ProgramContext context, Func<Expression, Wrapper> wrapperFactory);
}
