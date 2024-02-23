namespace Symbol.Tests;

[TestClass()]
public class JSONTests {
    [TestMethod()]
    public void TimespanWithZone() {
        var json = "{\"Timestamp\":\"/Date(1686658327629+0800)/\"}";
        var values = JSON.Parse(json);
        var time = values.Path("Timestamp").Convert<DateTime>();
        var target = new DateTime(2023, 06, 13, 20, 12, 07, 629);

        Assert.AreEqual(target, time);
    }

    [TestMethod()]
    public void ToObject_IsObject() {
        var json = "{ \"count\": 1 }";
        var o = JSON.ToObject(json, typeof(object));
        Console.WriteLine(o.Path("count"));
    }

    [TestMethod()]
    public void ToObject_ListEntity() {
        var userBook = new UserBook() {
            User = new User() { Name = "张三" },
            Books = new System.Collections.Generic.List<BookInfo>() {
                     new BookInfo() {
                          Name="人文英语",
                           Count=1
                     }
                 }
        };
        var json = JSON.ToNiceJSON(userBook);
        Console.WriteLine(json);
        var userBook2 = JSON.ToObject<UserBook>(json, true);

        Assert.AreEqual(userBook.User.Name, userBook2.User.Name);
        Assert.AreEqual(userBook.Books[0].Name, userBook2.Books[0].Name);

    }
}