namespace Verse_Interpreter.Test;

[TestClass]
public class Verse2
{
    private VerseInterpreter? _verseInterpreter;

    [TestInitialize]
    public void Initialise()
    {
        IRenderer renderer = Mock.Of<IRenderer>();
        IVariableFactory variableFactory = new VariableFactory();
        SyntaxDesugarer desugar = new(variableFactory);
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
    
    [TestMethod]
    public void TestForOneToThreeDoTimesTwo()
    {
        // Arrange.
        string verseCode = "for(i:=1..3; i) do i+i";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });
        ICollection tuple = (result as VerseTuple)!.ToArray();

        // Assert.
        CollectionAssert.AreEqual(tuple, new Value[] { new Integer(2), new Integer(4), new Integer(6) });
    }

    [TestMethod]
    public void TestBindingWithChoice()
    {
        // Arrange.
        string verseCode = """
            x:=(1|7|2);
            x+1
            """;

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new All { E = e });
        ICollection tuple = (result as VerseTuple)!.ToArray();

        // Assert.
        CollectionAssert.AreEqual(tuple, new Value[] { new Integer(2), new Integer(8), new Integer(3) });
    }
}
