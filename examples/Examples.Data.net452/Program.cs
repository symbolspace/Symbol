using System;
using Symbol.Data;

namespace Examples.Data {
    class Program {

        static void Main(string[] args) {

            var type = typeof(System.Data.SqlClient.SqlConnection);
            Console.WriteLine(type.FullName);

            {
                //创建数据上下文对象
                //IDataContext db = CreateDataContext("mssql2012");
                IDataContext db = CreateDataContext("mysql");
                //IDataContext db = CreateDataContext("pgsql");
                //IDataContext db = CreateDataContext("sqlite");

                //初始化 &  数据
                DatabaseSchema(db);
                //增 删 改 查  常规操作
                DatabaseCRUD(db);
                //泛型
                QueryGeneric(db);

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
                case "pgsql":
                    connectionOptions = new {
                        host = "192.168.247.119",               //服务器
                        port = 5432,                            //端口，可以与服务器写在一起，例如127.0.0.1:5432
                        name = "test",                          //数据库名称
                        account = "test",                       //登录账号
                        password = "test",                      //登录密码
                    };
                    break;
                case "sqlite":
                    connectionOptions = new {
                        name = "test",                          //数据库名称
                        memory = true,                          //内存数据库
                    };
                    break;
            }
            //Provider 自动扫描Symbol.Data.*.dll
            return Symbol.Data.Provider.CreateDataContext(type, connectionOptions);
            //return new Symbol.Data.SqlServer2012Provider().CreateDataContext(connectionOptions);
        }
        static void DatabaseSchema(IDataContext db) {
            switch (db.Provider.GetType().Name) {
                case "PostgreSQLProvider": {

                        #region 创建表：t_user
                        if (!db.TableExists("t_user")){
                            db.ExecuteNonQuery(@"
                                create table t_user(
                                    id bigserial not null,
                                    ""type"" smallint  not null,
                                    account character varying(64) not null,
                                    ""password"" character varying(32) not null,
                                    CONSTRAINT ""pk_t_User_id"" PRIMARY KEY(id)
                                )
                                WITH(
                                    OIDS = FALSE
                                );");
                        }
                        #endregion
                        #region 创建表：test
                        if (!db.TableExists("test")) {
                            db.ExecuteNonQuery(@"
                                create table test(
                                   id bigserial not null,
                                   name character varying(255),
                                   ""count"" bigint  not null,
                                   ""data"" jsonb null,
                                   CONSTRAINT ""pk_test_id"" PRIMARY KEY(id)
                                )
                                WITH(
                                  OIDS = FALSE
                                ); ");
                            #region 初始测试数据
                            db.Insert("test", new {
                                name= "test",
                                count= 24234
                            });
                            db.Insert("test", new {
                                name = "test24",
                                count = 466
                            });
                            db.Insert("test", new {
                                name = "test214",
                                count = 347693,
                                data=new {
                                    a=true,
                                    list=new object[] { 32,"test" }
                                }
                            });
                            for (int i = 0; i < 15; i++) {
                                db.ExecuteNonQuery("insert into test(name,\"count\",\"data\") select name,\"count\", \"data\" from test;");
                            }
                            #endregion
                        }
                        #endregion

                    }
                    break;
                case "SQLiteProvider": {
                        #region 创建表
                        db.ExecuteNonQuery(@"
                            create table test(
                                id integer primary key autoincrement not null,
                                name nvarchar(64) not null,
                                [count] bigint not null,
                                [data] ntext null
                            )
                        ");
                        db.ExecuteNonQuery(@"
                            create table t_User(
                                id integer primary key autoincrement not null,
                                [type] tinyint not null,
                                account nvarchar(64) not null,
                                [password] varchar(32) null,
                                [data] ntext null
                            )
                        ");
                        #endregion
                        #region 初始测试数据
                        db.Insert("test", new {
                            name = "test",
                            count = 24234
                        });
                        db.Insert("test", new {
                            name = "test24",
                            count = 466
                        });
                        db.Insert("test", new {
                            name = "test214",
                            count = 347693,
                            data = new {
                                a = true,
                                list = new object[] { 32, "test" }
                            }
                        });
                        for (int i = 0; i < 15; i++) {
                            db.ExecuteNonQuery("insert into test(name,[count],[data]) select name,[count], [data] from test;");
                        }
                        #endregion
                    }
                    break;
            }
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
            var q = db.FindAll("test", "{ 'name':['test','test214'] }");

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


        static void QueryGeneric(IDataContext db) {
            //t_User
            foreach (var item in db.FindAll<t_User>("t_user")) {
                Console.WriteLine($"{JSON.ToJSON(item)}");
            }
            Console.ReadKey();
        }
    }
}
