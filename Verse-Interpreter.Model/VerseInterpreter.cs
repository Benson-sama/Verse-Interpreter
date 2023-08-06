using Antlr4.Runtime;
using Microsoft.Extensions.Logging;
using Verse_Interpreter.Model.SyntaxTree;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Equations;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

namespace Verse_Interpreter.Model;

public class VerseInterpreter
{
    private readonly IVerseLogger _logger;
    private readonly IVerseSyntaxTreeBuilder _syntaxTreeBuilder;
    private readonly IRewriter _rewriter;

    public VerseInterpreter(IVerseLogger logger, IVerseSyntaxTreeBuilder syntaxTreeBuilder, IRewriter rewriter)
    {
        _logger = logger;
        _syntaxTreeBuilder = syntaxTreeBuilder;
        _rewriter = rewriter;
    }

    public Expression Interpret(string verseCode)
    {
        VerseProgram verseProgram = GenerateParseTreeFromString(verseCode);
        return Rewrite(verseProgram);
    }

    private void Rewrite(Expression expression)
    {
        switch (expression)
        {
            case Eqe eqe:
                Rewrite(eqe);
                break;
            case Equation eq:
                Rewrite(eq);
                break;
            case Lambda lambda:
                Rewrite(lambda);
                break;
            case Application application:
                Rewrite(application);
                break;
            case Choice choice:
                Rewrite(choice);
                break;
            case Exists exists:
                Rewrite(exists);
                break;
            case Wrapper wrapper:
                Rewrite(wrapper);
                break;
        }
    }

    private Expression Rewrite(VerseProgram verseProgram)
    {
        _logger.Log("Rewriting...\n");

        do
        {
            verseProgram.Wrapper.E = _rewriter.TryRewrite(verseProgram.Wrapper.E);

            if (!_rewriter.RuleApplied)
                Rewrite(verseProgram.Wrapper.E);
        }
        while (_rewriter.RuleApplied);

        return _rewriter.TryRewrite(verseProgram.Wrapper);
    }

    private void Rewrite(Eqe eqe)
    {
        eqe.Eq = _rewriter.TryRewrite(eqe.Eq);

        if (_rewriter.RuleApplied)
            return;

        Rewrite(eqe.Eq);

        if (_rewriter.RuleApplied)
            return;

        eqe.E = _rewriter.TryRewrite(eqe.E);

        if (_rewriter.RuleApplied)
            return;

        Rewrite(eqe.E);
    }

    private void Rewrite(Equation eq)
    {
        throw new NotImplementedException();
    }

    private void Rewrite(Lambda lambda)
    {
        throw new NotImplementedException();
    }

    private void Rewrite(Application application)
    {
        throw new NotImplementedException();
    }

    private void Rewrite(Choice choice)
    {
        throw new NotImplementedException();
    }

    private void Rewrite(Exists exists)
    {
        throw new NotImplementedException();
    }

    private void Rewrite(Wrapper wrapper)
    {
        wrapper.E = _rewriter.TryRewrite(wrapper.E);

        if (_rewriter.RuleApplied)
            return;

        Rewrite(wrapper.E);
    }

    public VerseProgram GenerateParseTreeFromString(string input)
    {
        _logger.Log("Getting input stream...");
        ICharStream stream = CharStreams.fromString(input);
        _logger.Log("Starting lexer...");
        ITokenSource lexer = new VerseLexer(stream);
        _logger.Log("Getting token stream...");
        ITokenStream tokens = new CommonTokenStream(lexer);
        _logger.Log("Starting parser...");
        VerseParser parser = new(tokens)
        {
            BuildParseTree = true
        };

        _logger.Log("Converting and desugaring parse tree...");
        VerseParser.ProgramContext programContext = parser.program();
        return _syntaxTreeBuilder.BuildCustomSyntaxTreeWrappedInOne(programContext);
    }
}
