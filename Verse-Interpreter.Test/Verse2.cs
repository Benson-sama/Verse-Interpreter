﻿namespace Verse_Interpreter.Test;

[TestClass]
public class Verse2
{
    private VerseInterpreter? _verseInterpreter;

    [TestInitialize]
    public void Initialise()
    {
        IRenderer renderer = Mock.Of<IRenderer>();
        IVariableFactory variableFactory = new VariableFactory();
        Desugar desugar = new(variableFactory);
        IVerseSyntaxTreeBuilder verseSyntaxTreeBuilder = new VerseSyntaxTreeBuilder(variableFactory, desugar);
        IRewriter rewriter = new Rewriter(renderer, variableFactory);
        _verseInterpreter = new(renderer, verseSyntaxTreeBuilder, rewriter);
    }

    [TestMethod]
    public void TestLambdaWithReturningFalseQuestionMarkAppliedResultsInFailure()
    {
        // Arrange.
        string verseCode = "f:=([x, y] => (z:=69; s:=5; false?)); f[1, 2]";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Fail);
    }

    [DataTestMethod]
    [DataRow(4, 5)]
    [DataRow(5, 5)]
    [DataRow(6, 5)]
    public void TestIfFirstNumberGreaterSecondNumber(int k1, int k2)
    {
        // Arrange.
        string verseCode = $"if({k1}>{k2}): 6 else: 9";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });
        int expectedResult = k1 > k2 ? 6 : 9;

        // Assert.
        Assert.IsTrue(result is Integer integer && integer.Value == expectedResult);
    }
}