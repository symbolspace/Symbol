namespace Symbol;

/// <summary>
/// 调用返回结果。
/// </summary>
public class CallResult {

    /// <summary>
    /// 是否为成功。
    /// </summary>
    public virtual bool IsSuccess { get { return Code == 1000; } }
    /// <summary>
    /// 状态码。
    /// </summary>
    public virtual int Code { get; set; }
    /// <summary>
    /// 状态消息。
    /// </summary>
    public virtual string Message { get; set; }
    /// <summary>
    /// 返回数据。
    /// </summary>
    public virtual object ResultData { get; set; }

    /// <summary>
    /// 创建实例
    /// </summary>
    public CallResult() :
        this(1000, "", null) {
    }

    /// <summary>
    /// 创建实例
    /// </summary>
    /// <param name="code">状态码</param>
    /// <param name="message">状态消息</param>
    public CallResult(int code, string message)
        : this(code, message, null) {
    }
    /// <summary>
    /// 创建实例
    /// </summary>
    /// <param name="code">状态码</param>
    /// <param name="message">状态消息</param>
    /// <param name="resultData">返回数据</param>
    public CallResult(int code, string message, object resultData) {
        Code = code;
        Message = message;
        ResultData = resultData;
    }
    /// <summary>
    /// 创建实例
    /// </summary>
    /// <param name="resultData">返回数据</param>
    public CallResult(object resultData)
        : base() {
        ResultData = resultData;
    }
    /// <summary>
    /// 获取HashCode.
    /// </summary>
    /// <returns>返回 <see cref="Code"/></returns>
    public override int GetHashCode() {
        return Code;
    }

}