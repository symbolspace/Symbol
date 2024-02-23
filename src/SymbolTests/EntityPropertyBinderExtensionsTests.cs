namespace Symbol.Tests;

[TestClass()]
public class EntityPropertyBinderExtensionsTests
{
    [TestMethod()]
    public void PropertyValueGetterTest()
    {
        IDictionary<string, object> dic_string_object = new Dictionary<string, object>()
        {
            { "name", "张三" },
            { "age", 16 },
            { "address", "中国.成都" },
            { "sex", "男" },
            { "birthday", new DateTime(2003, 11, 13) },
            { "money", DBNull.Value},
        };
        var getter_string_object = dic_string_object.PropertyValueGetter();
        Assert.AreEqual(getter_string_object("name"), "张三");
        Assert.AreEqual(getter_string_object("age"), 16);
        Assert.AreEqual(getter_string_object("birthday"), new DateTime(2003, 11, 13));
        Assert.AreEqual(getter_string_object("money"), null);
        Assert.AreEqual(getter_string_object("nokey"), null);
        Assert.AreEqual(getter_string_object(""), null);
        Assert.AreEqual(getter_string_object(null), null);


        IDictionary<string, int> dic_string_int = new Dictionary<string, int>()
        {
            { "no.1", 111 },
            { "no.2", 100 },
            { "no.3", 90 },
        };
        var getter_string_int = dic_string_int.PropertyValueGetter();
        Assert.AreEqual(getter_string_int("no.1"), 111);
        Assert.AreEqual(getter_string_int("nokey"), 0);
        Assert.AreEqual(getter_string_int(""), 0);
        Assert.AreEqual(getter_string_int(null), 0);

    }
}