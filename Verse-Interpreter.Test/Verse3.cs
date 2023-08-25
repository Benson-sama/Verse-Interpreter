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
