using System;
using System.Linq;
using Symbol;

namespace Examples.Enums {
    /*
        枚举在定义之后，显示在界面上的顺序不一致，可以通过[Const("Order","1")]进行排序
        排序规则可以自己实现，包括顺序是否支持小数
        
    */

    class Program {
        static void Main(string[] args) {
            

            //枚举示例
            EnumExample();

            //Const示例
            ConstExample();

        }

        static void EnumExample() {
            //输出定义
            PrintFields(typeof(UserTypes));
            PrintFields(typeof(OrderStates));

            Console.WriteLine();
            //多值
            PrintValue(UserTypes.Agent | UserTypes.Business);
            //单值
            PrintValue(OrderStates.PendingShipment);

            Console.WriteLine("枚举示例演示完毕，按任意键继续 ...");
            Console.ReadKey();

        }
        static void PrintFields(Type type) {
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
        static void PrintValue<T>(T value) where T:Enum {
            var type = value.GetType();
            Console.WriteLine($"type: {type.Name}");
            Console.WriteLine($"名称：{value.ToName()}");
            Console.WriteLine($"属性：{value.ToName("Order")}");
            Console.WriteLine($"名称数组：{JSON.ToJSON(value.ToNames())}");
            Console.WriteLine($"值数组：{JSON.ToJSON(value.ToValues())}");
            Console.WriteLine();
        }

        static void ConstExample() {
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

            Console.WriteLine("Const示例演示完毕，按任意键继续 ...");
            Console.ReadKey();
        }
    }

    

}
