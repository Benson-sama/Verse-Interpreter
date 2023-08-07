using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;

namespace Verse_Interpreter.Model;

public interface IRewriter
{
    Expression Rewrite(VerseProgram verseProgram);
}
