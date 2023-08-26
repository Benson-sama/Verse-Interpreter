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
        string verseCode = "x:any; y:=x+9; x=3; x+y";
        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });

        // Assert.
        Assert.IsTrue(result is Integer { Value: 15 });
    }

    [TestMethod]
    public void TestFlatMap()
    {
        // Arrange.
        string verseCode = "flatMap:=([f,xs] => for{i:any; t:=xs(i); f[t]}); numbers:=[1,2,3,4,5]; add:=([a] => (a+1)); flatMap[add,numbers]";
        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });
        ICollection tuple = (result as VerseTuple)!.ToArray();

        // Assert.
        CollectionAssert.AreEqual(tuple, new Value[] { new Integer(2), new Integer(3), new Integer(4), new Integer(5), new Integer(6) });
    }

    [TestMethod]
    public void TestRunFunctionBackwards()
    {
        // Arrange.
        string verseCode = "a:any; f:=([x,y] => [y,x]); [1,2]=(f(a)); a";

        // Act.
        Expression result = _verseInterpreter!.Interpret(verseCode, (e) => new One { E = e });
        ICollection tuple = (result as VerseTuple)!.ToArray();

        // Assert.
        CollectionAssert.AreEqual(tuple, new Value[] { new Integer(2), new Integer(1) });
    }
}
