namespace Verse_Interpreter.Model;

public class VerseSyntaxTreeBuilder : IVerseSyntaxTreeBuilder
{
    private readonly IVerseVisitor<Node> _verseVisitor;

    public VerseSyntaxTreeBuilder(IVerseVisitor<Node> verseVisitor)
        => _verseVisitor = verseVisitor;

    public VerseProgram BuildCustomSyntaxTree(VerseParser.ProgramContext context)
    {
        if (context.Accept(_verseVisitor) is not VerseProgram program)
            throw new Exception("Unable to parse the verse program.");

        return program;
    }
}
