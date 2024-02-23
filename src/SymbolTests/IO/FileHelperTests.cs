using System.Collections.Generic;

namespace Symbol.IO.Tests;

[TestClass()]
public class FileHelperTests
{
    private decimal[] lengths = new decimal[]
    {
        512,
        1024M,
        1024M*1024,
        1024M*1024*1024,
        1024M*1024*1024*1024,
        1024M*1024*1024*1024*1024,
        1024M*1024*1024*1024*1024*1024,
        1024M*1024*1024*1024*1024*1024*1024,
        1024M*1024*1024*1024*1024*1024*1024*1024,
        1024M*1024*1024*1024*1024*1024*1024*1024*1024,
    };
    private string[] lengths_text = new string[]
    {
        "512B",
        "1K",
        "1M",
        "1G",
        "1T",
        "1P",
        "1E",
        "1Z",
        "1Y",
        "1024Y"
    };
    [TestMethod()]
    public void LengthToStringTest_FileSize()
    {
        for (int i = 0; i < lengths.Length; i++)
        {
            Assert.AreEqual(lengths_text[i], FileHelper.LengthToString(lengths[i]));
        }
    }

    [TestMethod()]
    public void LengthToStringTest_Speed()
    {
        for (int i = 0; i < lengths.Length; i++)
        {
            Assert.AreEqual(lengths_text[i] + "/S", FileHelper.LengthToString(lengths[i], true));
        }
    }

    [TestMethod()]
    public void ScanTest()
    {
        //扫描系统目录：关于mstsc程序相关文件。
        var list = FileHelper.Scan("mstsc*.*", "C:\\Windows\\System32");
        foreach(var file in list)
        {
            Console.WriteLine(file);
        }
        Assert.IsTrue(list.Count > 0);
    }

    [TestMethod()]
    public void ScanTest_AppPath()
    {
        //扫描当前测试项目的pdb文件。
        var list = FileHelper.Scan("*.pdb", AppHelper.AppPath);
        foreach (var file in list)
        {
            Console.WriteLine(file);
        }
        Assert.IsTrue(list.Count > 0);
        
        //默认就为AppHelper.AppPath
        var list2= FileHelper.Scan("*.pdb");
        CollectionAssert.AreEqual(list, list2);

        //appPath为null也为AppHelper.AppPath
        var list3 = FileHelper.Scan("*.pdb", null);
        CollectionAssert.AreEqual(list, list3);

    }
    [TestMethod()]
    public void ScanTest_Mulit()
    {
        //多条规则扫描
        var list = FileHelper.Scan("*.pdb;*.dll", AppHelper.AppPath);
        foreach (var file in list)
        {
            Console.WriteLine(file);
        }
        Assert.IsTrue(list.Count > 0);
    }
    [TestMethod()]
    public void ScanTest_Other()
    {
        Assert.IsTrue(FileHelper.Scan(null).Count==0);
        Assert.IsTrue(FileHelper.Scan(";;").Count==0);
        Assert.IsTrue(FileHelper.Scan("~/testhost.dll").Count==1);

        //将理想中的文件，扫描出来，并转换为实际的物理位置。
        var list = FileHelper.Scan("xxx.exe;testhost.dll;zh-Hant/Microsoft.TestPlatform.CoreUtilities.resources.dll");
        foreach (var file in list)
        {
            Console.WriteLine(file);
        }
        Assert.IsTrue(list.Count == 2);

    }
}