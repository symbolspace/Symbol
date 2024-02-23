namespace Symbol.Tests;

[TestClass()]
public class IDictionaryExtensionsTests
{
    [TestMethod()]
    public void GetValueTest()
    {
        IDictionary<string, object> list = new Dictionary<string, object>();

        list.SetValue("name", "测试");
        Assert.AreEqual(list.GetValue("name"), "测试");

        list.SetValue("name", 3333);
        Assert.AreEqual(list.GetValue("name"), 3333);
    }
}