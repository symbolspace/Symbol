using System.ComponentModel;

namespace Symbol.Tests;

[TestClass()]
public class ConstAttributeExtensionsTests
{
    [TestMethod()]
    public void ConstTest_Method()
    {
        var type = typeof(TestFilter);
        var methodInfo = type.GetMethod("Filter");

        Assert.AreEqual("过滤", methodInfo.Const());
        Assert.AreEqual("名称", methodInfo.ConstParameter("name"));
        Assert.AreEqual("名称", methodInfo.ConstParameter("Name"));
    }
    [TestMethod()]
    public void ConstTest_Type()
    {
        var type = typeof(TestFilter);
        Assert.AreEqual("测试", type.Const());
        Assert.AreEqual("这是一个测试", type.Const("Description"));
        Assert.AreEqual("system", type.Const("Author"));
    }
    [TestMethod()]
    public void ConstTest_Object()
    {
        IFilter filter = new TestFilter();
        Assert.AreEqual("测试", filter.Const());
        Assert.AreEqual("这是一个测试", filter.Const("Description"));
        Assert.AreEqual("{AF58882B-54BD-4142-BFB0-C806A360B80A}", filter.Const("Hash"));
        Assert.AreEqual("system", filter.Const("Author"));
    }

    [Const("Author", "system")]
    interface IFilter
    {
        [Const("过滤")]
        void Filter(
            [Const("名称")]
            string name);
    }

    [DisplayName("测试")]
    [System.ComponentModel.Description("这是一个测试")]
    [Const("Hash", "{AF58882B-54BD-4142-BFB0-C806A360B80A}")]
    class TestFilter : IFilter
    {
        public void Filter(string name)
        {
            throw new NotImplementedException();
        }
    }


    [TestMethod()]
    public void PrintText() {
        //Const用法
        var type = typeof(BookInfo);
        Console.WriteLine(type.Const());//默认key为Text
        Console.WriteLine(type.Const("Text"));
        Console.WriteLine(type.Const("TableName"));
        //成员上的属性
        Console.WriteLine(type.GetProperty("Name").Const());

        //对象方式
        var model = new BookInfo();
        Console.WriteLine(model.Const());
        Console.WriteLine(model.Const("Name"));
        //获取成员特性值是，推荐用此方式，更明确实
        Console.WriteLine(model.Const("Name", "Text"));

        //assembly上的也可以拿到的
        Console.Write("来自[assembly:Const(\"Test\", \"Value\"]) ：");
        Console.WriteLine(type.Assembly.Const("Test"));

        //获取基类中的
        var type_new = typeof(NewBookInfo);
        Console.Write("来自基类Count属性[Const(\"数量\")] ：");
        Console.WriteLine(type_new.GetProperty("Count").Const());
        //继承并重写之后也能拿到
        Console.Write("已经被重写[Const(\"继承测试\")] ：");
        Console.WriteLine(type_new.GetMethod("Foo").Const());

        //可以取到方法、参数上的值
        var buyMethod = type_new.GetMethod("Buy");
        var parameters = buyMethod.GetParameters();
        Console.WriteLine(buyMethod.Const());

        //实现类中，参数上没有值，所以取不到值
        Console.WriteLine(parameters.First(p => p.Name == "buyer").Const());
        //正确取值方式
        Console.WriteLine(buyMethod.ConstParameter("buyer"));

        //可以替换标记值
        Console.WriteLine(parameters.First(p => p.Name == "count").Const());

    }
}