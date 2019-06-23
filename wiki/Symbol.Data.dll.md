# 简介
* 轻量级ORM，实体与查询无需一致；
* 支持T-SQL和NoSQL语法混用；
* 支持数据库架构版本检测；
* 数据库：MSSQL 2005/2008/2012/2014/2016+，引用 **Symbol.Data.SqlServer** ；
* 数据库：PostgreSQL，引用 **Symbol.Data.PostgreSQL**，基于Npgsql.dll；
* 数据库：MySql，引用 **Symbol.Data.MySql**，基于 MySql.Data.dll；
* 数据库：SQLite，引用 **Symbol.Data.SQLite**，基于 System.Data.SQLite.Core.dll，自适应x64/x86，自动识别；

# 使用Symbol.Data
* 引用 Symbol.Data；
* 引用数据库引擎类库，例如 Symbol.Data.SqlServer；
* 创建DataContext；
```csharp
//连接参数，可以是json字符串、匿名对象、IDictionary<string, object>
//省去一些记不住的连接参数，只需要设置关键的参数即可
var connectionOptions = new {
    host = "192.168.247.119\\MSSQL2014",    //服务器，端口为默认，所以不用写
    name = "test",                          //数据库名称
    account = "test",                       //登录账号
    password = "test",                      //登录密码
};

//db 类型：Symbol.Data.IDataContext
var db = Symbol.Data.Provider.CreateDataContext("mssql2012", connectionOptions);
//也可以这样构建
//var db = return new Symbol.Data.SqlServer2012Provider().CreateDataContext(connectionOptions);

//除了使用connectionOptions，也支持原始的连接字符串
//var db = return new Symbol.Data.SqlServer2012Provider().CreateDataContext("Data Source=.;.....");
```
* 查询数据
```csharp
/* -------- 非泛型 -------- */
{
    //单条数据
    var item = db.Find("test", new { name = "xxxxxxxxx" });
    Console.WriteLine( JSON.ToNiceJSON( item ) );
    //查询对象，不进行数据读取操作时，不会实际访问数据库
    var q = db.FindAll("test", new { name = "xxxxxxxxx" }, new { id = "desc" });
    Console.WriteLine( JSON.ToNiceJSON( q ) );
    //可以快速输出List<T>
    var list = q.ToList();
    //可以快速取出第一条 db.Find 就是基于它
    var firstItem = q.FirstOrDefault();
    
    //传统SQL用法
    var q2 = db.CreateQuery("select * from [test] where [name]=@p1 order by [id] desc", "xxxxxxxxx");
}
/* ---------- 泛型 -------- */
{
    //泛型基本上与非泛型差不多，不同之处是指定实体类
    var q = db.FindAll<t_sys_User>(new{ age = 25 });
    //与EF 和 Dapper 不同之处的是，实体类可以不作为表名
    var q2 = db.FindAll<UserInfo>("t_sys_User", new { age = 25 });
}

```
* 插入数据
```csharp
//非泛型操作，指定表名
var id = db.Insert("test", new {
    name = "xxxxxxxxx",
    count = 9999,
    data = new { //JSON类型测试，自动序列化
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

//泛型插入，唯一用处不用写表名，另外类名发生改变，不用担心错误
var id2 = db.Insert<t_sys_User>(new{
     //类型可以为t_sys_User，也可以为匿名类、IDictionary<string, object>
     //...
});

//如果主键为Guid等特殊类型时
//由于重载有冲突，因此末尾的参数必须传
var guid = db.Insert<System.Guid>("guidtable", {
    name = "test"
},new string[0]);
var guid2 = db.Insert<t_sys_User, System.Guid>(new {
    //...
});

//可以使用ExecuteScalar实例传统的SQL操作
var testId = db.ExecuteScalar<int>("insert into [test]([name]) values(@p1)","xxxxx");

```
* 更新数据
```csharp
//非泛型操作，指定表名
var count = db.Update(
    "test",   //表名
     new {    //更新数据
         name = "fsafhakjshfksjhf",
         count = 88
     }, new { //更新目标，相当于 where 条件
         id 
     });
var updated = (count == 1);
Console.WriteLine($"update {updated}");
```
* 删除数据
```csharp
//匹配规则 类似 mongodb 的规则
var count = db.Delete(
    "test",                          //表名
    new {
        name = "xxxxxxxxx",          //name为xxxxxxxxx
        id = "{ '$gt': 200000 }"     //id大于200000，C#语法不支持JSON，但我们支持嵌套JSON语句 :)
    });
Console.WriteLine($"delete count={count}");

//也可以使用db.ExecuteNoQuery 执行传统SQL
var count2 = db.ExecuteNoQuery("delete from [test] where [name]=@p1 and [id]>@p2", "xxxxxxxxx", 200000);
//@p1 @p2 可以用问号代替
var count3 = db.ExecuteNoQuery("delete from [test] where [name]=? and [id]>?", "xxxxxxxxx", 200000);
```

