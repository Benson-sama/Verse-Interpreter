using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;

namespace Verse_Interpreter.Model;

public interface IRenderer
{
    void DisplayMessage(string message);

    void DisplayRuleApplied(string message);

    void DisplayParsedProgram(VerseProgram verseProgram);

    void DisplayIntermediateResult(Expression expression);

    void DisplayResult(Expression expression);
}
