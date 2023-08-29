namespace Verse_Interpreter.Test;

[TestClass]
public class Verse3
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
    public void TestFunkyOrder()
    {
        // Arrange.
        string verseCode = """
            x:any;
            y:=x+9;
            x=3;
            x+y
            """;

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 15 });
    }

    [TestMethod]
    public void TestFlatMap()
    {
        // Arrange.
        string verseCode = """
            flatMap:=([f,xs] => for{i:any; t:=xs(i); f[t]});
            numbers:=[1,2,3,4,5];
            add:=([a] => (a+1));
            flatMap[add,numbers]
            """;

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });
        ICollection tuple = (result as VerseTuple)!.ToArray();

        // Assert.
        CollectionAssert.AreEqual(tuple, new Value[] { new Integer(2), new Integer(3), new Integer(4), new Integer(5), new Integer(6) });
    }

    [TestMethod]
    public void TestAppend()
    {
        // Arrange.
        string verseCode = """
            a:=[1,2];
            b:=[3,4];
            append:=([x,y] => (for{(i:any; x(i)) | (j:any; y(j))}));
            append[a,b]
            """;

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });
        ICollection tuple = (result as VerseTuple)!.ToArray();

        // Assert.
        CollectionAssert.AreEqual(tuple, new Value[] { new Integer(1), new Integer(2), new Integer(3), new Integer(4) });
    }

    [TestMethod]
    public void TestRunFunctionBackwards()
    {
        // Arrange.
        string verseCode = """
            a:any;
            f:=([x,y] => [y,x]);
            [1,2]=(f(a));
            a
            """;

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });
        ICollection tuple = (result as VerseTuple)!.ToArray();

        // Assert.
        CollectionAssert.AreEqual(tuple, new Value[] { new Integer(2), new Integer(1) });
    }

    [TestMethod]
    public void TestFunctionApplicationWithChoiceEvaluatesAllChoices()
    {
        // Arrange.
        string verseCode = """
            x:=(1..5);
            f:=([a] => (a*a));
            f[x]
            """;

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new All { E = e });
        ICollection tuple = (result as VerseTuple)!.ToArray();

        // Assert.
        CollectionAssert.AreEqual(tuple, new Value[] { new Integer(1), new Integer(4), new Integer(9), new Integer(16), new Integer(25) });
    }

    [TestMethod]
    public void TestFunctionApplicationGivesVariableValue()
    {
        // Arrange.
        string verseCode = """
            x:any;
            f:=([a] => (a=10; a));
            f[x];
            x
            """;

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 10 });
    }

    [DataTestMethod]
    [DataRow(68)]
    [DataRow(69)]
    [DataRow(70)]
    public void TestIfWithOptionalExpressionAfterwards(int number)
    {
        // Arrange.
        string verseCode = $"""
            x:any;
            f:=([a] => (a={number}; a));
            is69:=(if (x=69; x): \"yes\" else: \"no\");
            f[x];
            is69
            """;
        string expectedText = number == 69 ? "yes" : "no";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.AreEqual(result, new VerseString(expectedText));
    }
}
