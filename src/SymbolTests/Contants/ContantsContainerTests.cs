using System.Collections;
using System.Dynamic;

namespace Symbol.Contants.Tests;

[TestClass()]
public class ContantsContainerTests
{
    [TestMethod()]
    public void Add_IDictionaryStringObject()
    {
        var list = new Dictionary<string, object>()
        {
            { "name", "测试" },
            { "age", 18 }
        };
        ContantsContainer.Add(list);

        var container = ContantsContainer.Global;
        Assert.AreEqual("测试", container["name"]);

        Assert.AreEqual("测试", ContantsContainer.GetValue("name"));
        Assert.AreEqual(18, ContantsContainer.GetValue<int>("age"));
        Assert.AreEqual(0, ContantsContainer.GetValue("agexx",0));

        list.SetValue("address", "1111");
        Assert.AreEqual("1111", ContantsContainer.GetValue("address"));

        ContantsContainer.Remove(list);
        Assert.AreEqual(null, ContantsContainer.GetValue("name"));
        
    }

    [TestMethod()]
    public void Add_IDictionary()
    {
        var list = new Hashtable()
        {
            { "name", "测试" },
            { "age", 18 }
        };
        ContantsContainer.Add(list);

        var container = ContantsContainer.Global;
        Assert.AreEqual("测试", container["name"]);

        Assert.AreEqual("测试", ContantsContainer.GetValue("name"));
        Assert.AreEqual(18, ContantsContainer.GetValue<int>("age"));
        Assert.AreEqual(0, ContantsContainer.GetValue("agexx", 0));

        list.SetValue((object)"address", "1111");
        Assert.AreEqual("1111", ContantsContainer.GetValue("address"));

        ContantsContainer.Remove(list);
        Assert.AreEqual(null, ContantsContainer.GetValue("name"));
    }

    [TestMethod()]
    public void Add_DynamicObject()
    {
        dynamic list = new ExpandoObject();
        list.name = "测试";
        list.age = 18;
        ContantsContainer.Add(list);

        var container = ContantsContainer.Global;
        Assert.AreEqual("测试", container["name"]);

        Assert.AreEqual("测试", ContantsContainer.GetValue("name"));
        Assert.AreEqual(18, ContantsContainer.GetValue<int>("age"));
        Assert.AreEqual(0, ContantsContainer.GetValue("agexx", 0));

        list.address = "1111";
        Assert.AreEqual("1111", ContantsContainer.GetValue("address"));

        ContantsContainer.Remove(list);
        Assert.AreEqual(null, ContantsContainer.GetValue("name"));
    }
}