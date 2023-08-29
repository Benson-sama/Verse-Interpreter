namespace Verse_Interpreter.Test;

[TestClass]
public class Verse1
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
    public void TestVersePaperIntroductionSample()
    {
        // Arrange.
        string verseCode = """
            x:any;
            y:any;
            z:any;
            x=[y,3];
            x=[2,z];
            y
            """;

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 2 });
    }

    [TestMethod]
    public void TestSimpleBinding()
    {
        // Arrange.
        string verseCode = "x:=3; x+x";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 6 });
    }

    [DataTestMethod]
    [DataRow("x:any; y:any; x=3; y=x+1; x*y")]
    [DataRow("x:any; y:any; y=x+1; x=3; x*y")]
    public void TestOrderOfAssignmentsDoesNotMatter(string verseCode)
    {
        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 12 });
    }

    [TestMethod]
    public void TestStringAddition()
    {
        // Arrange.
        string verseCode = """
            firstName:="tim";
            lastName:="sweeney";
            hyphen:="-";
            temp:=firstName + hyphen;
            temp + lastName
            """;

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is VerseString { Text: "tim-sweeney" });
    }

    [TestMethod]
    public void Test3Plus4Equals7()
    {
        // Arrange.
        string verseCode = "x:=3+4; x=7; x";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 7 });
    }

    [TestMethod]
    public void TestSubtraction()
    {
        // Arrange.
        string verseCode = "x:=3; x-2";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 1 });
    }

    [TestMethod]
    public void TestMultiplication()
    {
        // Arrange.
        string verseCode = "x:=3; x*2";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 6 });
    }

    [TestMethod]
    public void TestDivision()
    {
        // Arrange.
        string verseCode = "x:=4; x/2";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 2 });
    }

    [TestMethod]
    public void TestEquationsWithAllArithmeticOperations()
    {
        // Arrange.
        string verseCode = """
            x:any; y:any; z:any; f:any;
            x=3+y;
            y=6*2;
            z=x-f;
            f=8/4;
            z
            """;

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 13 });
    }

    [TestMethod]
    public void TestLambda()
    {
        // Arrange.
        string verseCode = "f:=([x]=>(x+1)); f[3]";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 4 });
    }

    [TestMethod]
    public void TestFirstAndSecond()
    {
        // Arrange.
        string verseCode = """
            fst:=([x,y] => x);
            snd:=([x,y] => y);
            z:=[3,4];
            a:=fst(z);
            b:=snd[5,6];
            a+b
            """;

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 9 });
    }

    [TestMethod]
    public void TestFactorialFunction()
    {
        // Arrange.
        string verseCode = """
            fac:=([x] => (
                if(x=0; x):
                    1
                else:
                    (a:=(x-1); b:=fac[a]; (x*b)))
                );
            fac[2]
            """;

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 24 });
    }
}
