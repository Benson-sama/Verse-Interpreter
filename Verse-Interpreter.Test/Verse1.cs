namespace Verse_Interpreter.Test;

[TestClass]
internal class Verse1
{
    [TestMethod]
    public void TestAssignment()
    {
        // Arrange.
        string code = "x:=3; x";

        // Act.

        // Assert.
        // Result: 3
    }

    [TestMethod]
    public void TestAddition()
    {
        // Arrange.
        string code = "x:=3; x+4";

        // Act.

        // Assert.
        // Result: 7
    }

    [TestMethod]
    public void TestSubtraction()
    {
        // Arrange.
        string code = "x:=3; x-2";

        // Act.

        // Assert.
        // Result: 1
    }

    [TestMethod]
    public void TestMultiplication()
    {
        // Arrange.
        string code = "x:=3; x*5";

        // Act.

        // Assert.
        // Result: 15
    }

    [TestMethod]
    public void TestDivision()
    {
        // Arrange.
        string code = "x:=6; x/2";

        // Act.

        // Assert.
        // Result: 3
    }

    [TestMethod]
    public void TestFunkyOrder()
    {
        // Arrange.
        string code = "y:=x+1; x:=3; x*y";

        // Act.

        // Assert.
        // Result: 15
    }

    [TestMethod]
    public void TestFunction()
    {
        // Arrange.
        string code = "f(x):=x+1; f(3)";

        // Act.

        // Assert.
        // Result: 4
    }

    [TestMethod]
    public void TestLambda()
    {
        // Arrange.
        string code = "f:=(x=>x+1); f(3)";

        // Act.

        // Assert.
        // Result: 4
    }

    [TestMethod]
    public void TestFactorialFunction()
    {
        // Arrange.
        string code = "fac(x):= if(x=0): 1 else: x * fac(x-1)";

        // Act.

        // Assert.
        // Result: 4
    }
}
