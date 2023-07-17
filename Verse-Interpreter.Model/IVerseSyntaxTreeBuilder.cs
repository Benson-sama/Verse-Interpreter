namespace Verse_Interpreter.Model;

public interface IVerseSyntaxTreeBuilder
{
    VerseProgram BuildCustomSyntaxTree(VerseParser.ProgramContext context);
}
