using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbol.Tests;

[TestClass()]
public class ConvertExtensionsTests
{
    [TestMethod()]
    public void ConvertTest_DefaultValue()
    {
        Assert.AreEqual("3".Convert<int>(0), 3);
        Assert.AreEqual("3".Convert(0), 3);
        Assert.AreEqual("aa".Convert(-1), -1);
    }
    [TestMethod()]
    public void ConvertTest_NormalValue()
    {
        Assert.AreEqual((1024).Convert<string>(), "1024");
        Assert.IsNull(DBNull.Value.Convert<string>());

        Assert.AreEqual("32".Convert<int?>(), 32);
        Assert.IsNull("aaa".Convert<int?>());

        Assert.AreEqual(0, ((object)null).Convert<int>());
    }
    [TestMethod()]
    public void ConvertTest_Boolean()
    {
        Assert.IsTrue("true".Convert<bool>());
        Assert.IsTrue("ok".Convert<bool>());
        Assert.IsTrue("yes".Convert<bool>());
        Assert.IsTrue("good".Convert<bool>());
        Assert.IsTrue("t".Convert<bool>());
        Assert.IsTrue("right".Convert<bool>());
        Assert.IsTrue("True".Convert<bool>());
        Assert.IsTrue("TRUE".Convert<bool>());
        Assert.IsTrue("1".Convert<bool>());
        Assert.IsTrue(1.Convert<bool>());
        Assert.IsTrue("-1".Convert<bool>());
        Assert.IsTrue((-1).Convert<bool>());

        Assert.IsFalse("false".Convert<bool>());
        Assert.IsFalse("no".Convert<bool>());
        Assert.IsFalse("f".Convert<bool>());
        Assert.IsFalse("bad".Convert<bool>());
        Assert.IsFalse("not".Convert<bool>());
        Assert.IsFalse("False".Convert<bool>());
        Assert.IsFalse("FALSE".Convert<bool>());
        Assert.IsFalse("0".Convert<bool>());
        Assert.IsFalse(0.Convert<bool>());
    }
    [TestMethod()]
    public void ConvertTest_Encoding()
    {
        Assert.AreEqual("utf-8".Convert<Encoding>(), Encoding.UTF8);
        Assert.AreEqual(Encoding.UTF8.Convert<string>(), "utf-8");
        Assert.AreEqual((65001).Convert<Encoding>(), Encoding.UTF8);
        Assert.AreEqual((65001L).Convert<Encoding>(), Encoding.UTF8);
        Assert.AreEqual(Encoding.UTF8.Convert<int>(), 65001);
        Assert.AreEqual(Encoding.UTF8.Convert<long>(), 65001L);

        Assert.IsNull("xxxx".Convert<Encoding>());
    }
    [TestMethod()]
    public void ConvertTest_Enum()
    {
        Assert.AreEqual("ReadOnly, System".Convert<FileAttributes>(), FileAttributes.ReadOnly | FileAttributes.System);
        Assert.AreEqual("readOnly,system".Convert<FileAttributes>(), FileAttributes.ReadOnly | FileAttributes.System);
        Assert.AreEqual((5).Convert<FileAttributes>(), FileAttributes.ReadOnly | FileAttributes.System);
        Assert.IsNull("xxxx".Convert<FileAttributes?>());

        Assert.AreEqual((FileAttributes.ReadOnly | FileAttributes.System).Convert<string>(), "ReadOnly, System");
        Assert.AreEqual((FileAttributes.ReadOnly | FileAttributes.System).Convert<int>(), 5);
        Assert.AreEqual((FileAttributes.ReadOnly | FileAttributes.System).Convert<long>(), 5L);
    }
    [TestMethod()]
    public void ConvertTest_Guid()
    {
        Assert.AreEqual("B625052B-8D72-42CB-B71D-0AA5D116EB37".Convert<Guid>(), _guid);
        Assert.AreEqual("{B625052B-8D72-42CB-B71D-0AA5D116EB37}".Convert<Guid>(), _guid);
        Assert.AreEqual(_guid.Convert<string>(), "b625052b-8d72-42cb-b71d-0aa5d116eb37");
    }
    private static readonly Guid _guid = new Guid("{B625052B-8D72-42CB-B71D-0AA5D116EB37}");

    [TestMethod()]
    public void ConvertTest_TimeSpan()
    {
        Assert.AreEqual("01:02:03".Convert<TimeSpan>(), new TimeSpan(01, 02, 03));
        Assert.AreEqual("01:02:03.033".Convert<TimeSpan>(), new TimeSpan(00, 01, 02, 03, 33));
        Assert.AreEqual("5.01:02:03.033".Convert<TimeSpan>(), new TimeSpan(05, 01, 02, 03, 33));

        Assert.AreEqual(new TimeSpan(00, 01, 02, 03, 33).Convert<string>(), "01:02:03.0330000");
        Assert.AreEqual(new TimeSpan(00, 01, 02, 03).Convert<string>(), "01:02:03");
        Assert.AreEqual(new TimeSpan(05, 01, 02, 03, 33).Convert<string>(), "5.01:02:03.0330000");

        Assert.AreEqual((1024).Convert<TimeSpan>(), new TimeSpan(1024));
        Assert.AreEqual((10240000L).Convert<TimeSpan>(), new TimeSpan(10240000L));
        Assert.AreEqual(new TimeSpan(1024).Convert<int>(), 1024);
        Assert.AreEqual(new TimeSpan(10240000L).Convert<long>(), 10240000L);

        Assert.AreEqual((10240000D).Convert<TimeSpan>(), TimeSpan.FromMilliseconds(10240000D));
        Assert.AreEqual((10240000F).Convert<TimeSpan>(), TimeSpan.FromMilliseconds(10240000F));
        Assert.AreEqual((10240000M).Convert<TimeSpan>(), TimeSpan.FromMilliseconds((double)10240000M));
        Assert.AreEqual(TimeSpan.FromMilliseconds(10240000D).Convert<double>(), 10240000D);
        Assert.AreEqual(TimeSpan.FromMilliseconds(10240000F).Convert<float>(), 10240000F);
        Assert.AreEqual(TimeSpan.FromMilliseconds((double)10240000M).Convert<decimal>(), 10240000M);

        Assert.IsNull("test".Convert<TimeSpan?>());
        Assert.IsNull("99999.99:99:99.999999999999".Convert<TimeSpan?>());
    }
    [TestMethod()]
    public void ConvertTest_DateTime()
    {
        Assert.AreEqual(new DateTime(2007, 12, 20), "2007-12-20".Convert<DateTime>());
        Assert.AreEqual(
            new TimeSpan(
                (int)(new DateTime(2017, 02, 03) - DateTime.MinValue).TotalDays,
                12, 30, 55, 99),
            new DateTime(2017, 02, 03, 12, 30, 55, 99).Convert<TimeSpan>()
        );
        Assert.AreEqual(
            new DateTime(2017, 02, 03, 12, 30, 55, 99),
            new TimeSpan(
                (int)(new DateTime(2017, 02, 03) - DateTime.MinValue).TotalDays,
                12, 30, 55, 99).Convert<DateTime>()
        );
    }

    [TestMethod()]
    public void ConvertTest_Array()
    {
        string[] array_source = new string[]
        {
            "1","1.5","3.2","999"
        };
        CollectionAssert.AreEqual(array_source.Convert<int?[]>(), new int?[] { 1, null, null, 999 });
        CollectionAssert.AreEqual(array_source.Convert<decimal[]>(), new decimal[] { 1M, 1.5M, 3.2M, 999M });

        CollectionAssert.AreEqual(new List<string>(array_source).Convert<int?[]>(), new int?[] { 1, null, null, 999 });
        CollectionAssert.AreEqual(array_source.GetEnumerator().Convert<decimal[]>(), new decimal[] { 1M, 1.5M, 3.2M, 999M });
    }

    [TestMethod()]
    public void ConvertTest_Structure()
    {
        CollectionAssert.AreEqual((261).Convert<byte[]>(), new byte[] { 5, 1, 0, 0 });
        Assert.AreEqual(261, new byte[] { 5, 1, 0, 0 }.Convert<int>());
    }

    [TestMethod()]
    public void ConvertTest_ImplicitExplicit()
    {
        Assert.AreEqual("test".Convert<CustomString>(), new CustomString("test"));
        Assert.AreEqual(new CustomString("test").Convert<string>(), "test");
    }

#pragma warning disable CS0659 // 类型重写 Object.Equals(object o)，但不重写 Object.GetHashCode()
    class CustomString
#pragma warning restore CS0659 // 类型重写 Object.Equals(object o)，但不重写 Object.GetHashCode()
    {
        public string? Value { get; set; }

        public CustomString()
        {
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
            if (obj is string)
                return string.Equals(Value, (string)obj);
            if (obj is CustomString customString)
                return customString.Value == Value;
            return base.Equals(obj);
        }
    }
}