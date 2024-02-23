using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symbol;
using System.Text;
using System.Runtime.CompilerServices;

namespace Symbol.Tests;

[TestClass()]
public class TypeExtensionsTests
{
    [TestMethod()]
    public void IsNullableTypeTest()
    {
        Assert.IsFalse(typeof(bool).IsNullableType());
        Assert.IsTrue(typeof(bool?).IsNullableType());
        Assert.IsFalse(typeof(string).IsNullableType());
    }

    [TestMethod()]
    public void GetNullableTypeTest()
    {
        Assert.AreEqual(typeof(bool).GetNullableType(), typeof(bool));
        Assert.AreEqual(typeof(bool?).GetNullableType(), typeof(bool));
        Assert.AreEqual(typeof(string).GetNullableType(), typeof(string));
    }

    [TestMethod()]
    public void IsInheritFromTest()
    {
        Assert.IsTrue(typeof(Pigeon).IsInheritFrom(typeof(Birds)));
        Assert.IsTrue(typeof(Pigeon).IsInheritFrom(typeof(IFly)));
        Assert.IsTrue(typeof(Pigeon).IsInheritFrom(typeof(IBase)));
        Assert.IsTrue(typeof(Fish).IsInheritFrom(typeof(IBase)));
        Assert.IsFalse(typeof(Fish).IsInheritFrom(typeof(Birds)));
        Assert.IsTrue(typeof(IFly).IsInheritFrom(typeof(IBase)));
    }
    interface IBase { }
    interface IFly : IBase { }
    class Birds : IFly { }
    class Pigeon : Birds { }
    class Fish : IBase { }

    [TestMethod()]
    public void IsAnonymousTypeTest()
    {
        var builder = new StringBuilder();
        var info = new { name = "test" };

        Assert.IsFalse(builder.IsAnonymousType());
        Assert.IsTrue(info.IsAnonymousType());
        Assert.IsFalse(typeof(string).IsAnonymousType());
    }

    [TestMethod()]
    public void IsSystemBaseTypeTest()
    {
        var info = new { name = "test" };

        Assert.IsTrue(typeof(int).IsSystemBaseType());
        Assert.IsFalse(this.GetType().IsSystemBaseType());
        Assert.IsFalse(info.GetType().IsSystemBaseType());
    }

    [TestMethod()]
    public void IsNumbericTypeTest()
    {
        Assert.IsTrue(typeof(byte).IsNumbericType());
        Assert.IsTrue(typeof(short).IsNumbericType());
        Assert.IsTrue(typeof(int).IsNumbericType());
        Assert.IsTrue(typeof(uint).IsNumbericType());
        Assert.IsTrue(typeof(long).IsNumbericType());
        Assert.IsTrue(typeof(ulong).IsNumbericType());
        Assert.IsTrue(typeof(float).IsNumbericType());
        Assert.IsTrue(typeof(decimal).IsNumbericType());
        Assert.IsTrue(typeof(double).IsNumbericType());
        Assert.IsTrue(typeof(byte?).IsNumbericType());
        Assert.IsTrue(typeof(short?).IsNumbericType());
        Assert.IsTrue(typeof(int?).IsNumbericType());
        Assert.IsTrue(typeof(uint?).IsNumbericType());
        Assert.IsTrue(typeof(long?).IsNumbericType());
        Assert.IsTrue(typeof(ulong?).IsNumbericType());
        Assert.IsTrue(typeof(float?).IsNumbericType());
        Assert.IsTrue(typeof(decimal?).IsNumbericType());
        Assert.IsTrue(typeof(double?).IsNumbericType());

        Assert.IsFalse(typeof(string).IsNumbericType());
        Assert.IsFalse("test".IsNumbericType());
        Assert.IsTrue((3.42F).IsNumbericType());

    }
}