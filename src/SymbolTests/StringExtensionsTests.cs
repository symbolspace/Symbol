namespace Symbol.Tests;

[TestClass()]
public class StringExtensionsTests
{
    [TestMethod()]
    public void LeftTest()
    {
        string text = "1234567890";
        Assert.AreEqual("123", text.Left(3));
        Assert.AreEqual(text, text.Left(15));
        Assert.AreEqual("", text.Left(0));
        Assert.AreEqual("", text.Left(-1));
    }

    [TestMethod()]
    public void RightTest()
    {
        string text = "1234567890";
        Assert.AreEqual("890", text.Right(3));
        Assert.AreEqual(text, text.Right(15));
        Assert.AreEqual("", text.Right(0));
        Assert.AreEqual("", text.Right(-1));
    }

    [TestMethod()]
    public void MiddleTest()
    {
        string text = "1234567890";
        Assert.AreEqual("45", text.Middle(3, 2));
        Assert.AreEqual("890", text.Middle(7, 15));
        Assert.AreEqual(text, text.Middle(0, 15));
        Assert.AreEqual("", text.Middle(3, 0));
        Assert.AreEqual("", text.Middle(3, -1));
        Assert.AreEqual("", text.Middle(-1, 15));
    }

    [TestMethod()]
    public void ReplaceTest_Char()
    {
        string text = "AaBbCcDdEe";
        Assert.AreEqual("AazbCcDdEe", text.Replace('B', 'z', false));
        Assert.AreEqual("AaBzCcDdEe", text.Replace('b', 'z', false));
        Assert.AreEqual("AazzCcDdEe", text.Replace('B', 'z', true));
        Assert.AreEqual("AazzCcDdEe", text.Replace('b', 'z', true));
    }

    [TestMethod()]
    public void ReplaceTest_String()
    {
        string text = "AaBbCcDdEe";
        Assert.AreEqual("AaBbCcdEe", text.Replace("D", "", false));
        Assert.AreEqual("AaBbCcDEe", text.Replace("d", "", false));
        Assert.AreEqual("AaBbCcEe", text.Replace("D", "", true));
        Assert.AreEqual("AaBbCcEe", text.Replace("d", "", true));
    }
}