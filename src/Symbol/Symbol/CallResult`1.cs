namespace Symbol;

/// <summary>
/// 调用返回结果（泛型）
/// </summary>
/// <typeparam name="T"></typeparam>
public class CallResult<T> : CallResult {
    /// <summary>
    /// 返回数据（T）。
    /// </summary>
    /// <remarks>尝试转换父类的<see cref="CallResult.ResultData"/>类型为<typeparamref name="T"/></remarks>
    public virtual T Result {
        get {
            return (T)ResultData;
        }
    }

    /// <summary>
    /// 创建实例
    /// </summary>
    public CallResult() :
        base() {
    }

    /// <summary>
    /// 创建实例
    /// </summary>
    /// <param name="code">状态码</param>
    /// <param name="message">状态消息</param>
    public CallResult(int code, string message)
        : base(code, message) {
    }
    /// <summary>
    /// 创建实例
    /// </summary>
    /// <param name="code">状态码</param>
    /// <param name="message">状态消息</param>
    /// <param name="result">返回数据</param>
    public CallResult(int code, string message, T result)
        : base(code, message) {
        ResultData = result;
    }
    /// <summary>
    /// 创建实例
    /// </summary>
    /// <param name="result">返回数据</param>
    public CallResult(T result)
        : base() {
        ResultData = result;
    }
}