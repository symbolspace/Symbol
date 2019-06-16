using System;
using Symbol.Data;

namespace Examples.Data {
    class Program {

        static void Main(string[] args) {

            var type = typeof(System.Data.SqlClient.SqlConnection);
            Console.WriteLine(type.FullName);

            {
                //创建数据上下文对象
                IDataContext db = CreateDataContext("mssql2012");
                //IDataContext db = CreateDataContext("mysql");

                //增 删 改 查  常规操作
                DatabaseCRUD(db);

                //性能测试
                QueryPerf(db);

            }
            Console.ReadKey();
        }
        static IDataContext CreateDataContext(string type) {
            object connectionOptions = null;
            switch (type) {
                case "mssql2012":
                    connectionOptions = new {
                        host = "192.168.247.119\\MSSQL2014",    //服务器，端口为默认，所以不用写
                        name = "test",                          //数据库名称
                        account = "test",                       //登录账号
                        password = "test",                      //登录密码
                    };
                    break;
                case "mysql":
                    connectionOptions = new {
                        host = "192.168.247.119",               //服务器
                        port = 3306,                            //端口，可以与服务器写在一起，例如127.0.0.1:3306
                        name = "test",                          //数据库名称
                        account = "test",                       //登录账号
                        password = "test",                      //登录密码
                    };
                    break;
            }
            //Provider 自动扫描Symbol.Data.*.dll
            return Symbol.Data.Provider.CreateDataContext(type, connectionOptions);
            //return new Symbol.Data.SqlServer2012Provider().CreateDataContext(connectionOptions);
        }
        static void DatabaseCRUD(IDataContext db) {
            //常规测试
            {
                //删除数据
                Console.WriteLine("delete count={0}", db.Delete("test", new {
                    name = "xxxxxxxxx",         //name为xxxxxxxxx
                    id = "{ '$gt': 200000 }"     //id大于200000，C#语法不支持JSON，但我们支持嵌套JSON语句 :)
                }));
                //插入数据
                var id = db.Insert("test", new {
                    name = "xxxxxxxxx",
                    count = 9999,
                    data = new {//JSON类型测试
                        url = "https://www.baidu.com/",
                        guid = System.Guid.NewGuid(),
                        datetime = DateTime.Now,
                        values = FastWrapper.As(new {//嵌套复杂对象测试
                            nickName = "昵尔",
                            account = "test"
                        })
                    }
                });
                Console.WriteLine($"insert id={id}");
                //查询数据
                Console.WriteLine("select");
                Console.WriteLine(JSON.ToNiceJSON(db.Find("test", new { name = "xxxxxxxxx" })));
                //更新数据
                var updated = db.Update("test", new { name = "fsafhakjshfksjhf", count = 88 }, new { id }) == 1;
                Console.WriteLine($"update {updated}");
                //验证是否真的修改到
                Console.WriteLine("select new value");
                Console.WriteLine(JSON.ToNiceJSON(db.Find("test", new { id })));
            }
            Console.ReadKey();
            {
                //枚举测试
                var id = db.Insert("t_user", new {
                    account = "admin",
                    type = UserTypes.Manager
                });
                Console.WriteLine(JSON.ToNiceJSON(db.Find("t_user", new { id })));
            }
            Console.ReadKey();
        }
        static void QueryPerf(IDataContext db) {
            var q = db.FindAll("test", "{ 'name':'test' }");

            int max = 0;
            int count = 0;
            int time = 0;
            var results = new System.Collections.Generic.List<string>();
            Action<int> begin = (p) => {
                count = 0;
                max = p;
                time = Environment.TickCount;
            };
            Action<string> end = (p) => {
                time = Environment.TickCount - time;
                string log = $"data read {p} {max} used time {time}ms, avg:{(decimal)time / 1000}ms.";
                results.Add(log);
                Console.WriteLine(log);
            };
            Action print = () => {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(string.Join("\r\n", results.ToArray()));
            };
            {
                begin(10000);
                foreach (var item in q) {
                    count++;
                    //Console.WriteLine(JSON.ToJSON(db.Find("test_copy", new { id = item.Path("id") })));
                    Console.WriteLine(JSON.ToJSON(item));
                    if (count == max) {
                        break;
                    }
                }
                end("普通for");
            }
            {
                begin(10000);
                System.Threading.Tasks.Parallel.ForEach(q, (p, forState) => {
                    int n = System.Threading.Interlocked.Increment(ref count);
                    Console.WriteLine($"【{n}】{System.Threading.Thread.CurrentThread.ManagedThreadId}:{JSON.ToJSON(p)}");
                    if (n == max) {
                        forState.Break();
                        end("并行for");
                    }
                });
            }
            print();
        }
       
    }
}
