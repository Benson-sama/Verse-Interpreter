using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values;

namespace Verse_Interpreter.Model.Rewrite;

public interface IRewriter
{
    Expression Rewrite(VerseProgram verseProgram);
}
