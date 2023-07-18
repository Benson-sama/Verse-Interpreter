using Verse_Interpreter.Model.SyntaxTree;

namespace Verse_Interpreter.Model;

public interface IVerseSyntaxTreeBuilder
{
    VerseProgram BuildCustomSyntaxTreeWrappedInOne(VerseParser.ProgramContext context);

    VerseProgram BuildCustomSyntaxTreeWrappedInAll(VerseParser.ProgramContext context);
}
