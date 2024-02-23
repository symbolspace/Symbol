namespace Symbol.Tests;

[TestClass()]
public class EnumExtensionsTests
{
    [TestMethod()]
    public void GetPropertyTest()
    {
        Assert.AreEqual("#FF0000", Colors.Red.GetProperty("Hex"));
        Assert.AreEqual("这是绿色", Colors.Green.GetProperty("Description"));
        Assert.AreEqual("这是蓝色", Colors.Blue.GetProperty("Description"));
        Assert.AreEqual("蓝色", Colors.Blue.GetProperty("Text"));         //默认为Text
        Assert.AreEqual("蓝色", Colors.Blue.GetProperty(""));             //默认为Text
        Assert.AreEqual("蓝色", Colors.Blue.GetProperty(null));           //默认为Text
        Assert.AreEqual("", Colors.Other.GetProperty("XXXXX"));           //GetProperty没有值为""。
    }

    [TestMethod()]
    public void ToNameTest_Text()
    {
        Assert.AreEqual("红色", Colors.Red.ToName());
        Assert.AreEqual("绿色", Colors.Green.ToName());
        Assert.AreEqual("蓝色", Colors.Blue.ToName());
        Assert.AreEqual("红色, 蓝色", (Colors.Red | Colors.Blue).ToName());
        Assert.AreEqual("Red,Blue", (Colors.Red | Colors.Blue).ToName(true));
    }

    [TestMethod()]
    public void ToNameTest_Custom()
    {
        Assert.AreEqual("#FF0000", Colors.Red.ToName("Hex"));
        Assert.AreEqual("这是绿色", Colors.Green.ToName("Description"));
        Assert.AreEqual("蓝色", Colors.Blue.ToName("Text"));              //默认为Text
        Assert.AreEqual("绿色", Colors.Green.ToName(null));               //默认为Text
        Assert.AreEqual("蓝色", Colors.Blue.ToName(""));                  //默认为Text
        Assert.AreEqual("Other", Colors.Other.ToName("Text"));            //默认为Text
        Assert.AreEqual("Other", Colors.Other.ToName("XXXXX"));           //ToName没有取到值时会默认以定义为名。
    }

    [TestMethod()]
    public void PrintFields() {
        //输出定义
        PrintFields(typeof(UserTypes));
        PrintFields(typeof(OrderStates));

        Console.WriteLine();
        //多值
        PrintValue(UserTypes.Agent | UserTypes.Business);
        //单值
        PrintValue(OrderStates.PendingShipment);
    }

    void PrintFields(Type type) {
        string typeName = type.Const();
        if (!string.IsNullOrEmpty(typeName)) {
            Console.WriteLine("/// <summary>");
            Console.WriteLine($"/// {typeName}");
            Console.WriteLine("/// </summary>");
            Console.WriteLine($"[Const(\"{typeName}\")]");
        }
        if (type.IsDefined<FlagsAttribute>()) {
            Console.WriteLine($"[Flags]");
        }
        Console.WriteLine($"public enum {type.Name} {{");

        var q = from p in Enum.GetValues(type).Cast<Enum>()
                orderby p.Const("Order").Convert(0D)
                select new {
                    field = p.ToString(),
                    value = p.Convert<long>(),
                    name = p.ToName(),
                    order = p.Const("Order")
                };

        bool first = true;
        Action<string> print = (p) => {
            Console.Write($"    {p}");
        };
        Action<string> printLine = (p) => {
            Console.WriteLine($"    {p}");
        };

        foreach (var p in q) {
            if (first) {
                first = false;
            } else {
                Console.WriteLine(",");
            }
            printLine("/// <summary>");
            printLine($"/// {p.name}");
            printLine("/// </summary>");
            printLine($"[Const(\"{p.name}\")]");

            if (!string.IsNullOrEmpty(p.order)) {
                printLine($"[Const(\"Order\", \"{p.order}\")]");
            }

            print($"{p.field} = {p.value}");
        }
        if (!first)
            Console.WriteLine();

        Console.WriteLine("}");
    }
    void PrintValue<T>(T value) where T : Enum {
        var type = value.GetType();
        Console.WriteLine($"type: {type.Name}");
        Console.WriteLine($"名称：{value.ToName()}");
        Console.WriteLine($"属性：{value.ToName("Order")}");
        Console.WriteLine($"名称数组：{JSON.ToJSON(value.ToNames())}");
        Console.WriteLine($"值数组：{JSON.ToJSON(value.ToValues())}");
        Console.WriteLine();
    }

    [Const("颜色集")]
    [Flags]
    enum Colors
    {
        [Const("红色")]
        [Const("Description", "这是红色")]
        [Const("Hex", "#FF0000")]
        Red = 1,
        [Const("绿色")]
        [System.ComponentModel.Description("这是绿色")]
        [Const("Hex", "#00FF00")]
        Green = 2,
        [Const("蓝色")]
        [Const("Description", "这是蓝色")]
        [Const("Hex", "#0000FF")]
        Blue = 4,
        Other = 128,
    }

    
}