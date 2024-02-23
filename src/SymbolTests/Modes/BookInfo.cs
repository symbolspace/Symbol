namespace Symbol.Tests;

/// <summary>
/// 图书信息
/// </summary>
[Const("图书信息")]
[Const("TableName", "t_Book")]
public class BookInfo {

    /// <summary>
    /// 名称
    /// </summary>
    [Const("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [Const("数量")]
    public int Count { get; set; }

    [Const("继承测试")]
    public virtual void Foo() {
    }
}



public interface IBookBuy {
    [Const("购买")]
    void Buy([Const("买家")] string buyer, [Const("数量")] int count);
}

public class NewBookInfo : BookInfo, IBookBuy {
    public override void Foo() {
        //猜猜Const能不能取到值
    }

    public void Buy(string buyer, [Const("数量改名")] int count) {
        //猜猜Const能不能取到值
    }
}
public class User {
    public string Name { get; set; }

}

public class UserBook {
    public User User { get; set; }
    public List<BookInfo> Books { get; set; } = new List<BookInfo>();
}