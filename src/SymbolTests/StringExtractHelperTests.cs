using Symbol.Text;

namespace Symbol.Tests;

[TestClass()]
public class StringExtractHelperTests
{
    [TestMethod()]
    public void TagReplaceTest()
    {
        string html = "<a href=\"/home\" target=\"_blank\"><img alt=\"Logo\" src=\"/static/logo.png\" /><span>首页</span></a>";
        Assert.AreEqual("<a href=\"/home\" target=\"_blank\"><img alt=\"Logo\" src=\"/static/logo.png\" /><strong>首页</strong></a>", html.TagReplace("span", "strong"));
        Assert.AreEqual("<a href=\"/home\" target=\"_blank\"><link alt=\"Logo\" src=\"/static/logo.png\" /><span>首页</span></a>", html.TagReplace("img", "link"));
        Assert.AreEqual("<link href=\"/home\" target=\"_blank\"><img alt=\"Logo\" src=\"/static/logo.png\" /><span>首页</span></link>", html.TagReplace("a", "link"));
        Assert.AreEqual("", "".TagReplace("a", "link"));
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
        Assert.AreEqual("", ((string)null).TagReplace("a", "link"));
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
    }

    [TestMethod()]
    public void ClearTagTest()
    {
        string html = "<a href=\"/home\" target=\"_blank\"><img alt=\"Logo\" src=\"/static/logo.png\" /><span>首&nbsp;&nbsp;页</span></a>  ";
        Assert.AreEqual("首页", html.ClearTag());
        Assert.AreEqual("首&nbsp;&nbsp;页", html.ClearTag(false, true));
        Assert.AreEqual("首&nbsp;&nbsp;页  ", html.ClearTag(false, false));
        Assert.AreEqual("首页  ", html.ClearTag(true, false));
        Assert.AreEqual("", "".ClearTag());
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
        Assert.AreEqual("", ((string)null).ClearTag());
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
    }

    [TestMethod()]
    public void StringIndexOfTest()
    {
        string html = "<a href=\"/home\" target=\"_blank\"><img alt=\"Logo\" src=\"/static/logo.png\" /><span>首&nbsp;&nbsp;页</span><img alt=\"LogoRight\" src=\"/static/logo-right.png\" /></a>  ";
        string find = "首&nbsp;&nbsp;页";
        Assert.AreEqual(79, html.StringIndexOf(ref find, false));
        Assert.AreEqual("首&nbsp;&nbsp;页", find);

        string findAny = "首[*]页";
        Assert.AreEqual(79, html.StringIndexOf(ref findAny, false));
        Assert.AreEqual("首&nbsp;&nbsp;页", findAny);

        string findImg = "<img[*]>";
        Assert.AreEqual(100, html.StringIndexOf(ref findImg, 79, false));
        Assert.AreEqual("<img alt=\"LogoRight\" src=\"/static/logo-right.png\" /></a>", findImg);

        findImg = "<img[*]>";
        Assert.AreEqual(-1, html.StringIndexOf(ref findImg, 180, false));
        Assert.AreEqual(-1, html.StringIndexOf(ref findImg, 157, false));


        string findError = "xxxxx";
        Assert.AreEqual(-1, "".StringIndexOf(ref findError, false));

        string findEmpty = "";
        Assert.AreEqual(0, html.StringIndexOf(ref findEmpty, false));
    }

    [TestMethod()]
    public void StringsStartEndTest()
    {
        string html = "<a href=\"/home\" target=\"_blank\"><img alt=\"Logo\" src=\"/static/logo.png\" /><span>首&nbsp;&nbsp;页</span><img alt=\"LogoRight\" src=\"/static/logo-right.png\" /></a>  ";

        Assert.AreEqual("首&nbsp;&nbsp;页", html.StringsStartEnd("<span>", "</span>"));
    }

    [TestMethod()]
    public void RulesStringsStartEndTest()
    {
        string html = "<a href=\"/home\" target=\"_blank\"><img alt=\"Logo\" src=\"/static/logo.png\" /><span>首&nbsp;&nbsp;页</span><img alt=\"LogoRight\" src=\"/static/logo-right.png\" /></a>  ";
        CollectionAssert.AreEqual(new string[]
        {
            "<img alt=\"Logo\" src=\"/static/logo.png\" />",
            "<img alt=\"LogoRight\" src=\"/static/logo-right.png\" />"
        }, html.RulesStringsStartEnd("<img", new string[] { ">" }, 0, true, true, false).ToArray());
    }

}