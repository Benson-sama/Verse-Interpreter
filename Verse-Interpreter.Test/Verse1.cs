using Moq;
using Verse_Interpreter.Model;
using Verse_Interpreter.Model.SyntaxTree.Expressions;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Values.HeadNormalForms;
using Verse_Interpreter.Model.SyntaxTree.Expressions.Wrappers;

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
        string verseCode = "x:any; y:any; z:any; x=[y,3]; x=[2,z]; y";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 2 });
    }

    [TestMethod]
    public void TestAssignment()
    {
        // Arrange.
        string verseCode = "x:=3; x";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 3 });
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
    public void TestFunkyOrder()
    {
        // Arrange.
        string verseCode = "x:any; y:=x+9; x=3; x+y";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 15 });
    }

    //[TestMethod]
    //public void TestFunction()
    //{
    //    // Arrange.
    //    string code = "f[x]:=x+1; f[3]";

    //    // Act.

    //    // Assert.
    //    // Result: 4
    //}

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
    public void TestFirstAndSecond()
    {
        // Arrange.
        string verseCode = "fst:=([x,y] => x); snd:=([x,y] => y); z:=[3,4]; a:=fst z; b:=snd [5,6]; a+b";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 9 });
    }

    [TestMethod]
    public void TestFactorialFunction()
    {
        // Arrange.
        string verseCode = "fac:=([x] => (if(x=0; x): 1 else: a:=(x-1); b:=fac[a]; x*b)); fac[2]";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 24 });
    }
}
