
namespace Symbol.Tests;
/// <summary>
/// 用户类型集
/// </summary>
[Flags]
[Const("用户类型集")]
public enum UserTypes : int {
    /// <summary>
    /// 普通用户
    /// </summary>
    [Const("普通用户")]
    [Const("Order", "1")]
    User = 1,
    /// <summary>
    /// 管理员
    /// </summary>
    [Const("管理员")]
    [Const("Order", "2")]
    Manager = 2,
    /// <summary>
    /// 代理商
    /// </summary>
    [Const("代理商")]
    [Const("Order", "4")]
    Agent = 4,
    /// <summary>
    /// 商家
    /// </summary>
    [Const("商家")]
    [Const("Order", "8")]
    Business = 8
}