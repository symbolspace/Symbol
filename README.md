# Symbol project

## Download
* Symbol.dll [![Available on NuGet https://www.nuget.org/packages/Symbol/](https://img.shields.io/nuget/v/Symbol.svg?style=flat)](https://www.nuget.org/packages/Symbol/)
* Symbol.Net.dll [![Available on NuGet https://www.nuget.org/packages/Symbol.Net/](https://img.shields.io/nuget/v/Symbol.Net.svg?style=flat)](https://www.nuget.org/packages/Symbol.Net/)
* Symbol.Web.dll [![Available on NuGet https://www.nuget.org/packages/Symbol.Web/](https://img.shields.io/nuget/v/Symbol.Web.svg?style=flat)](https://www.nuget.org/packages/Symbol.Web/) 
* Symbol.Logger.NLog.dll [![Available on NuGet https://www.nuget.org/packages/Symbol.Logger.Serilog/](https://img.shields.io/nuget/v/Symbol.Logger.NLog.svg?style=flat)](https://www.nuget.org/packages/Symbol.Logger.NLog/)
* Symbol.Logger.Serilog.dll [![Available on NuGet https://www.nuget.org/packages/Symbol.Logger.Serilog/](https://img.shields.io/nuget/v/Symbol.Logger.Serilog.svg?style=flat)](https://www.nuget.org/packages/Symbol.Logger.Serilog/)

## Release Notes   [More](https://github.com/symbolspace/Symbol/wiki/Version-history)

## Iteration plan
* Symbol.Cloud 移植；
* Symbol.Cloud 微服务家族；
* Symbol.Cloud 多种实现方式；


## Support runtime
* .net framework 2.0;
* .net framework 3.5;
* .net framework 4.0;
* .net framework 4.5;
* .net framework 4.5.2;
* .net framework 4.6;
* .net framework 4.6.1;
* .net framework 4.7;
* .net framework 4.8;
* .net standard 2.0
* .net core app 3.1;
* .net 5.0;
* .net 6.0;
* .net 7.0;
* .net 8.0;


## Wiki
* [Symbol.dll](https://github.com/symbolspace/Symbol/wiki/Symbol.dll)
* [Symbol.Data.dll](https://github.com/symbolspace/Symbol/wiki/Symbol.Data.dll)
* [Symbol.Net.dll](https://github.com/symbolspace/Symbol/wiki/Symbol.Net.dll)
* [Symbol.Web.dll](https://github.com/symbolspace/Symbol/wiki/Symbol.Web.dll)
* [Symbol.Drawing.dll](https://github.com/symbolspace/Symbol/wiki/Symbol.Drawing.dll)
* [Symbol.IO.Packing.TreePackage.dll](https://github.com/symbolspace/Symbol/wiki/Symbol.IO.Packing.TreePackage.dll)
* [Symbol.ImageRecognition.Verification.dll](https://github.com/symbolspace/Symbol/wiki/Symbol.ImageRecognition.Verification.dll)


## Contribute
We gladly accept community contributions.
* Issues: Please report bugs using the Issues section of GitHub
* Source Code Contributions:
* See [C# Coding Style](https://github.com/symbolspace/Symbol/wiki/C%23-Coding-Style) for reference on coding style.


## 静态扩展：Type判断
> *TypeExtensions*

### 判断是否为可为空类型，比如int?这种类型
> *public static bool IsNullableType(this Type type)* <br>
> 返回为true表示此类型为struct类型，并且采用的是Nullable&lt;T&gt;。

```csharp
Console.WriteLine(  typeof(bool).IsNullableType()  );
Console.WriteLine(  typeof(bool?).IsNullableType()  );
Console.WriteLine(  typeof(string).IsNullableType()  );
/*
False
True
False
*/
```

### 获取可为空类型的原始类型
> *public static Type GetNullableType(this Type type)* <br>
> 如果为非可为空类型，返回的就是它自己，反之而是被包装的类型。

```csharp
Console.WriteLine(  typeof(bool).GetNullableType().FullName  );
Console.WriteLine(  typeof(bool?).GetNullableType().FullName  );
Console.WriteLine(  typeof(string).GetNullableType().FullName  );
/*
System.Boolean
System.Boolean
System.String
*/
```

### 是否为匿名类型
> *public static bool IsAnonymousType(this object value)* <br>
> *public static bool IsAnonymousType(this Type type)* <br>
> 为null返回false，反之为匿名对象时返回true。

```csharp
var builder = new StringBuilder();
var info = new { name = "test" };

Console.WriteLine(  builder.IsAnonymousType()  );
Console.WriteLine(  info.IsAnonymousType()  );
Console.WriteLine(  typeof(string).IsAnonymousType()  );
/*
False
True
True
*/
```

### 是否为系统基础类型
> *public static bool IsSystemBaseType(this Type type)* <br>
> 为true表示为基础类型，比如string int。

```csharp
Console.WriteLine(  typeof(int).IsSystemBaseType()  );
Console.WriteLine(  typeof(Program).IsSystemBaseType()  );
/*
True
False
*/
```

### 是否为数字类型
> *public static bool IsNumbericType(this Type type)* <br>
> 为null时返回false，反之为数字类型时返回为true。 <br>
> 已知：byte,short,int,uint,long,ulong,float,decimal,double <br>
>      byte?,short?,int?,uint?,long?,ulong?,float?,decimal?,double?

```csharp
Console.WriteLine(  typeof(int).IsNumbericType()  );
Console.WriteLine(  typeof(string).IsNumbericType()  );
Console.WriteLine(  "test".IsNumbericType()  );
Console.WriteLine(  (3.42F).IsNumbericType()  );
/*
True
False
False
True
*/
```

### 判断两个类型是否有继承关系。
> *public static bool IsInheritFrom(this Type type, Type parent)* <br>
> 支持接口、类，返回true表示有继承关系。
```csharp

interface IBase{ }
interface IFly : IBase { }
class Birds: IFly {  }
class Pigeon : Birds {  }
class Fish : IBase { }
static void Main()
{
    Console.WriteLine(  typeof(Pigeon).IsInheritFrom(  typeof(Birds)  )  );
    Console.WriteLine(  typeof(Pigeon).IsInheritFrom(  typeof(IFly)  )  );
    Console.WriteLine(  typeof(Pigeon).IsInheritFrom(  typeof(IBase)  )  );
    Console.WriteLine(  typeof(Fish).IsInheritFrom(  typeof(IBase)  )  );
    Console.WriteLine(  typeof(Fish).IsInheritFrom(  typeof(Birds)  )  );
    Console.WriteLine(  typeof(IFly).IsInheritFrom(  typeof(IBase)  )  );
}
/*
True
True
True
True
False
True
*/
```

## 静态扩展：值转换
> *ConvertExtensions*

### 强制转换为另一个类型（仅限struct类型）
> *public static T Convert<T>(this object value, T defaultValue) where T : struct* <br>
> 返回需要转换的类型，如果转换不成功时采用的默认值。

### 强制转换为另一个类型（泛型）
> *public static T Convert<T>(this object value)* <br>
> 返回需要转换的类型。

### 强制转换为另一个类型（非泛型）
> *public static object Convert(this object value, Type type)* <br>
> 返回需要转换的类型，DBNull识别为null,支持数组转换。

### 调用示例：带默认值的用法

```csharp
int count = "3".Convert<int>(0);    //完整书写
int count1 = "3".Convert(0);        //简写（推荐）
int count2 = "aa".Convert(-1);

Console.WriteLine(count);
Console.WriteLine(count1);
Console.WriteLine(count2);

/*
3
3
-1
*/
```

### 调用示例：常规转换

```csharp
string text = (1024).Convert<string>();
string value = DBNull.Value.Convert<string>();
int? age = "32".Convert<int?>();
int? age1 = "aaa".Convert<int?>();

Console.WriteLine(date);
Console.WriteLine(text);
Console.WriteLine(value);
Console.WriteLine(age);
Console.WriteLine(age1);

/*
1024
null
32
null
*/
```

### 调用示例：Boolean（布尔类型）
```csharp
Console.WriteLine(  "true".Convert<bool>()  );
Console.WriteLine(  "ok".Convert<bool>()  );
Console.WriteLine(  "yes".Convert<bool>()  );
Console.WriteLine(  "good".Convert<bool>()  );
Console.WriteLine(  "t".Convert<bool>()  );
Console.WriteLine(  "True".Convert<bool>()  );
Console.WriteLine(  "TRUE".Convert<bool>()  );
Console.WriteLine(  "1".Convert<bool>()  );
Console.WriteLine(  1.Convert<bool>()  );
Console.WriteLine(  "-1".Convert<bool>()  );
Console.WriteLine(  (-1).Convert<bool>()  );

Console.WriteLine(  "----"  );

Console.WriteLine(  "false".Convert<bool>()  );
Console.WriteLine(  "no".Convert<bool>()  );
Console.WriteLine(  "f".Convert<bool>()  );
Console.WriteLine(  "bad".Convert<bool>()  );
Console.WriteLine(  "not".Convert<bool>()  );
Console.WriteLine(  "False".Convert<bool>()  );
Console.WriteLine(  "FALSE".Convert<bool>()  );
Console.WriteLine(  "0".Convert<bool>()  );
Console.WriteLine(  0.Convert<bool>()  );

/*
True
True
True
True
True
True
True
True
True
True
True
----
False
False
False
False
False
False
False
False
False
*/
```

### 调用示例：Encoding

```csharp
Console.WriteLine(  "utf-8".Convert<Encoding>().WebName  );
Console.WriteLine(  Encoding.UTF8.Convert<string>()  );
Console.WriteLine(  (65001).Convert<Encoding>().CodePage  );
Console.WriteLine(  (65001L).Convert<Encoding>().CodePage  );
Console.WriteLine(  Encoding.UTF8.Convert<int>()  );
Console.WriteLine(  Encoding.UTF8.Convert<long>()  );
Console.WriteLine(  "xxxx".Convert<Encoding>()  );

/*
utf-8
utf-8
65001
65001
65001
65001
null
*/
```

### 调用示例：Enum（枚举）
```csharp
Console.WriteLine(  "ReadOnly, System".Convert<FileAttributes>() == (FileAttributes.ReadOnly | FileAttributes.System)  );
Console.WriteLine(  "readOnly,system".Convert<FileAttributes>() == (FileAttributes.ReadOnly | FileAttributes.System)  );
Console.WriteLine(  "readOnly, System".Convert<FileAttributes>() ==  FileAttributes.ReadOnly | FileAttributes.System );
Console.WriteLine(  (5).Convert<FileAttributes>() == (FileAttributes.ReadOnly | FileAttributes.System)  );
Console.WriteLine(  "xxxx".Convert<FileAttributes?>()  );

Console.WriteLine(  "----"  );

Console.WriteLine(  (FileAttributes.ReadOnly | FileAttributes.System).Convert<string>() == "ReadOnly, System"  );
Console.WriteLine(  (FileAttributes.ReadOnly | FileAttributes.System).Convert<int>() == 5  );
Console.WriteLine(  (FileAttributes.ReadOnly | FileAttributes.System).Convert<long>() == 5L  );

/*
True
True
True
True
null
----
True
True
True
*/
```

###  调用示例：GUID
```csharp
Guid guid = new Guid("{B625052B-8D72-42CB-B71D-0AA5D116EB37}");

Console.WriteLine(  "B625052B-8D72-42CB-B71D-0AA5D116EB37".Convert<Guid>() == guid  );
Console.WriteLine(  "{B625052B-8D72-42CB-B71D-0AA5D116EB37}".Convert<Guid>() == guid  );
Console.WriteLine(  guid.Convert<string>()  );

/*
True
True
b625052b-8d72-42cb-b71d-0aa5d116eb37
*/
```

###  调用示例：TimeSpan
```csharp
Console.WriteLine(  "01:02:03".Convert<TimeSpan>() == new TimeSpan(01, 02, 03)  );
Console.WriteLine(  "01:02:03.033".Convert<TimeSpan>() == new TimeSpan(00, 01, 02, 03, 33)  );
Console.WriteLine(  "5.01:02:03.033".Convert<TimeSpan>() == new TimeSpan(05, 01, 02, 03, 33)  );
Console.WriteLine(  "----"  );

Console.WriteLine(  new TimeSpan(00, 01, 02, 03, 33).Convert<string>()  );
Console.WriteLine(  new TimeSpan(00, 01, 02, 03).Convert<string>()  );
Console.WriteLine(  new TimeSpan(05, 01, 02, 03, 33).Convert<string>()  );
Console.WriteLine(  "----"  );

Console.WriteLine(  (1024).Convert<TimeSpan>() == new TimeSpan(1024)  );
Console.WriteLine(  (10240000L).Convert<TimeSpan>() == new TimeSpan(10240000L)  );
Console.WriteLine(  new TimeSpan(1024).Convert<int>() == 1024  );
Console.WriteLine(  new TimeSpan(10240000L).Convert<long>() == 10240000L  );
Console.WriteLine(  "----"  );

Console.WriteLine(  (10240000D).Convert<TimeSpan>() == TimeSpan.FromMilliseconds(10240000D)  );
Console.WriteLine(  (10240000F).Convert<TimeSpan>() == TimeSpan.FromMilliseconds(10240000F)  );
Console.WriteLine(  (10240000M).Convert<TimeSpan>() == TimeSpan.FromMilliseconds((double)10240000M)  );
Console.WriteLine(  TimeSpan.FromMilliseconds(10240000D).Convert<double>() == 10240000D  );
Console.WriteLine(  TimeSpan.FromMilliseconds(10240000F).Convert<float>() == 10240000F  );
Console.WriteLine(  TimeSpan.FromMilliseconds((double)10240000M).Convert<decimal>() == 10240000M  );
Console.WriteLine(  "----"  );

Console.WriteLine(  "test".Convert<TimeSpan?>()  );
Console.WriteLine(  "99999.99:99:99.999999999999".Convert<TimeSpan?>()  );
Console.WriteLine(  "----"  );
/*
True
True
True
----
01:02:03.0330000
01:02:03
5.01:02:03.0330000
----
True
True
True
True
----
True
True
True
True
True
True
----
null
null
----
*/
```

### 调用示例：DateTime
```csharp
Console.WriteLine(  "2007-12-20".Convert<DateTime>() == new DateTime(2007, 12, 20)  );

int totalDays = (int)(new DateTime(2017, 02, 03) - DateTime.MinValue).TotalDays;
DateTime date = new DateTime(2017, 02, 03, 12, 30, 55, 99);
TimeSpan time = new TimeSpan(totalDays, 12, 30, 55, 99);

Console.WriteLine(  date.Convert<TimeSpan>() == time  );
Console.WriteLine(  time.Convert<DateTime>() == date  );

/*
True
True
True
*/
```

###  调用示例：Array
```csharp
static void PrintArray<T>(T[] array)
{
    for( int i =0; i < array.Length; i++ )
    {
        Console.WriteLine($"[{i}]{array[i]}");
    }
    Console.WriteLine("------------");
}
static void Main()
{
    string[] array_source = new string[]
    {
        "1","1.5","3.2","999"
    };
    PrintArray(  array_source.Convert<int?[]>()  );
    PrintArray(  array_source.Convert<decimal[]>()  );
    PrintArray(  new List<string>(array_source).Convert<int?[]>()  );
    PrintArray(  array_source.GetEnumerator().Convert<decimal[]>()  );
}

/*
[0]1
[1]null
[2]null
[3]999
------------
[0]1
[1]1.5
[2]3.2
[3]999
------------
[0]1
[1]null
[2]null
[3]999
------------
[0]1
[1]1.5
[2]3.2
[3]999
------------
*/
```

###  调用示例：Structure（结构体）
```csharp
byte[] bytes = new byte[] { 5, 1, 0, 0 };
int number = 261;

int intValue = bytes.Convert<int>();           //将二进制转换为结构体
Console.WriteLine(  number == intValue  );

byte[] intToBytes = number.Convert<byte[]>();  //将结构体转换为二进制数组
bool eq = true;
for( int i =0; i < intToBytes.Length; i++ )
{
    if ( intToBytes[i] != bytes[i] )
    {
        eq = false;
        break;
    }
}
Console.WriteLine(  eq  );

/*
True
True
*/
```

###  调用示例：Implicit/Explicit（隐式/显式转换）
```csharp
class CustomString 
{
    public string? Value { get; set; }

    public CustomString() {
        Value = null;
    }
    public CustomString(string? value)
    {
        Value = value;
    }

    public override string? ToString()
    {
        return Value;
    }
    //显式转换：需要手动显式的转换，例如 int age = (int)32L; 显式将一个long转换为int。
    public static explicit operator string?(CustomString customString)
    {
        return customString?.Value;
    }
    //隐式转换：在使用的时候无感，就如同 long value = 32 ;是隐式将int转换为了long
    public static implicit operator CustomString(string? value)
    {
        return new CustomString(value);
    }
    //重写的目的只是为了方便测试：使两者相等。
    public override bool Equals(object? obj)
    {
        if (obj == null)
            return string.IsNullOrEmpty(Value);
        if(obj is string)
            return string.Equals(Value, (string)obj);
        if(obj is CustomString customString)
            return customString.Value == Value;
        return base.Equals(obj);
    }
}

static void Main()
{
    CustomString customString = new CustomString("test");
    string text = "test";

    Console.WriteLine(  text.Convert<CustomString>() == customString  );  //触发：隐式转换
    Console.WriteLine(  customString.Convert<string>() == text  );        //触发：显式转换
}
/*
True
True
*/
```
