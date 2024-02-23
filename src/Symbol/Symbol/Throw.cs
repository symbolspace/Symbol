namespace Symbol;

/// <summary>
/// 静态：抛出异常。
/// </summary>
public static class Throw {
    /// <summary>
    /// 检查参数是否为 <c>null</c>，如果为 <c>null</c> 则抛出异常。
    /// </summary>
    /// <param name="value">要检查的参数值。</param>
    /// <param name="paramName">要检查的参数名。</param>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="value"/> 为 <c>null</c>。</exception>
    /// <overloads>
    /// <summary>
    /// 检查参数是否为 <c>null</c>，如果为 <c>null</c> 则抛出异常。
    /// </summary>
    /// </overloads>
    public static void CheckArgumentNull(
#if !NET20
        this 
#endif
        object value, string paramName) {
        if (value == null) {
            ArgumentNull(paramName);
        }
    }
    /// <summary>
    /// 检查参数是否为 <c>null</c> 或 <c>“”</c>，如果成立则抛出异常。
    /// </summary>
    /// <param name="value">要检查的参数值。</param>
    /// <param name="paramName">要检查的参数名。</param>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="value"/> 为 <c>null</c> 或 <c>“”</c>。</exception>
    /// <overloads>
    /// <summary>
    /// 检查参数是否为 <c>null</c> 或 <c>“”</c>，如果成立则抛出异常。
    /// </summary>
    /// </overloads>
    public static void CheckArgumentNull(
#if !NET20
        this 
#endif
        string value, string paramName) {
        if (string.IsNullOrEmpty(value)) {
            ArgumentNull(paramName);
        }
    }
    /// <summary>
    /// 检查参数是否为 <c>null</c>，如果为 <c>null</c> 则抛出异常。
    /// 对于值类型，不会抛出异常。
    /// </summary>
    /// <typeparam name="T">要检查的参数的类型。</typeparam>
    /// <param name="value">要检查的参数值。</param>
    /// <param name="paramName">要检查的参数名。</param>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="value"/> 为 <c>null</c>。</exception>
    public static void CheckArgumentNull<T>(
#if !NET20
        this 
#endif
        T value, string paramName) {
        if (value == null) {
            ArgumentNull(paramName);
        }
    }

    /// <summary>
    /// 抛出 格式 异常。
    /// </summary>
    /// <param name="message">错误消息。</param>
    /// <exception cref="System.FormatException">对象。</exception>
    public static void Format(string message) {
        throw new System.FormatException(message);
    }
    /// <summary>
    /// 抛出 不支持 异常。
    /// </summary>
    /// <exception cref="System.NotSupportedException"> 对象。</exception>
    public static void NotSupported() {
        throw new System.NotSupportedException();
    }
    /// <summary>
    /// 抛出 不支持 异常。
    /// </summary>
    /// <param name="message">错误消息。</param>
    /// <exception cref="System.NotSupportedException"> 对象。</exception>
    public static void NotSupported(string message) {
        throw new System.NotSupportedException(message);
    }
    /// <summary>
    /// 抛出 平台不支持 异常。
    /// </summary>
    /// <exception cref="System.PlatformNotSupportedException"> 对象。</exception>
    public static void PlatformNotSupported() {
        throw new System.PlatformNotSupportedException();
    }
    /// <summary>
    /// 抛出 平台不支持 异常。
    /// </summary>
    /// <param name="message">错误消息。</param>
    /// <exception cref="System.PlatformNotSupportedException"> 对象。</exception>
    public static void PlatformNotSupported(string message) {
        throw new System.PlatformNotSupportedException(message);
    }
    /// <summary>
    /// 抛出 未实现 异常。
    /// </summary>
    /// <exception cref="System.NotImplementedException"> 对象。</exception>
    public static void NotImplemented() {
        throw new System.NotImplementedException();
    }
    /// <summary>
    /// 抛出 未实现 异常。
    /// </summary>
    /// <param name="message">错误消息。</param>
    /// <exception cref="System.NotImplementedException"> 对象。</exception>
    public static void NotImplemented(string message) {
        throw new System.NotImplementedException(message);
    }
    /// <summary>
    /// 抛出 空引用 异常。
    /// </summary>
    /// <exception cref="System.NullReferenceException"> 对象。</exception>
    public static void NullReference() {
        throw new System.NullReferenceException();
    }
    /// <summary>
    /// 抛出 空引用 异常。
    /// </summary>
    /// <param name="message">错误消息。</param>
    /// <exception cref="System.NullReferenceException"> 对象。</exception>
    public static void NullReference(string message) {
        throw new System.NullReferenceException(message);
    }
    /// <summary>
    /// 抛出 文件不存在 异常。
    /// </summary>
    /// <param name="message">错误消息。</param>
    /// <exception cref="System.IO.FileNotFoundException"> 对象。</exception>
    public static void FileNotFound(string message) {
        throw new System.IO.FileNotFoundException(message);
    }
    /// <summary>
    /// 抛出 无效的操作 异常。
    /// </summary>
    /// <exception cref="System.InvalidOperationException"> 对象。</exception>
    public static void InvalidOperation(string message) {
        throw new System.InvalidOperationException(message);
    }
    /// <summary>
    /// 抛出 无效的转换 异常。
    /// </summary>
    /// <exception cref="System.InvalidCastException"> 对象。</exception>
    public static void InvalidCast(string message) {
        throw new System.InvalidCastException(message);
    }
    /// <summary>
    /// 抛出 类型加载 异常。
    /// </summary>
    /// <exception cref="System.TypeLoadException"> 对象。</exception>
    public static void TypeLoad(string message) {
        throw new System.TypeLoadException(message);
    }
    /// <summary>
    /// 抛出 已存在 异常。
    /// </summary>
    /// <param name="message">错误消息。</param>
    /// <exception cref="ExistedException">对象。</exception>
    public static void Existed(string message) {
        throw new ExistedException(message);
    }

    /// <summary>
    /// 抛出 类型不匹配 异常。
    /// </summary>
    /// <exception cref="TypeMismatchException"> 对象。</exception>
    public static void TypeMismatch(string message) {
        throw new TypeMismatchException(message);
    }

    #region System.ArgumentException
    /// <summary>
    /// 抛出 参数无效 异常。
    /// </summary>
    /// <param name="paramName">参数名。</param>
    /// <exception cref="System.ArgumentException"> 对象。</exception>
    public static void Argument(string paramName) {
        throw new System.ArgumentException(paramName);
    }
    /// <summary>
    /// 抛出 参数无效 异常。
    /// </summary>
    /// <param name="paramName">参数名。</param>
    /// <param name="message">错误消息。</param>
    /// <exception cref="System.ArgumentException"> 对象。</exception>
    public static void Argument(string paramName, string message) {
        throw new System.ArgumentException(message, paramName);
    }

    #region 数组异常

    /// <summary>
    /// 抛出 数组为空 异常。
    /// </summary>
    /// <param name="paramName">产生异常的参数名称。</param>
    /// <exception cref="System.ArgumentException"> 对象。</exception>
    public static void ArrayEmpty(string paramName) {
        throw new System.ArgumentException("数组为空", paramName);
    }
    /// <summary>
    /// 抛出 数组下限不为 <c>0</c> 异常。
    /// </summary>
    /// <param name="paramName">产生异常的参数名称。</param>
    /// <exception cref="System.ArgumentException"> 对象。</exception>
    public static void ArrayNonZeroLowerBound(string paramName) {
        throw new System.ArgumentException("返回数组下限不为 0", paramName);
    }
    /// <summary>
    /// 抛出 目标数组太小而不能复制集合 异常。
    /// </summary>
    /// <param name="paramName">产生异常的参数名称。</param>
    /// <exception cref="System.ArgumentException"> 对象。</exception>
    public static void ArrayTooSmall(string paramName) {
        throw new System.ArgumentException("数组太小", paramName);
    }
    /// <summary>
    /// 抛出 偏移量和长度超出界限 异常。
    /// </summary>
    /// <exception cref="System.ArgumentException"> 对象。</exception>
    public static void InvalidOffsetLength() {
        throw new System.ArgumentException("偏移量和长度超出界限");
    }
    /// <summary>
    /// 抛出 数组长度不同 异常。
    /// </summary>
    /// <param name="paramName">产生异常的参数名称。</param>
    /// <exception cref="System.ArgumentException"> 对象。</exception>
    public static void ArrayLengthsDiffer(string paramName) {
        throw new System.ArgumentException("数组长度不同", paramName);
    }

    #endregion // 数组异常

    #endregion // System.ArgumentException
    #region System.ArgumentNullException

    /// <summary>
    /// 抛出 参数为 <c>null</c> 异常。
    /// </summary>
    /// <param name="paramName">为 <c>null</c> 的参数名。</param>
    /// <exception cref="System.ArgumentNullException"> 对象。</exception>
    public static void ArgumentNull(string paramName) {
        throw new System.ArgumentNullException(paramName);
    }

    #endregion // System.ArgumentNullException
    #region System.ArgumentOutOfRangeException

    /// <summary>
    /// 抛出 参数小于等于零 异常。
    /// </summary>
    /// <param name="paramName">异常参数的名称。</param>
    /// <param name="actualValue">导致此异常的参数值。</param>
    /// <exception cref="System.ArgumentOutOfRangeException"> 对象。</exception>
    public static void ArgumentMustBePositive(string paramName, object actualValue) {
        throw new System.ArgumentOutOfRangeException(paramName, actualValue, "参数小于等于零");
    }
    /// <summary>
    /// 抛出 参数小于零 异常。
    /// </summary>
    /// <param name="paramName">异常参数的名称。</param>
    /// <param name="actualValue">导致此异常的参数值。</param>
    /// <exception cref="System.ArgumentOutOfRangeException"> 对象。</exception>
    public static void ArgumentNegative(string paramName, object actualValue) {
        throw new System.ArgumentOutOfRangeException(paramName, actualValue, "参数小于零");
    }
    /// <summary>
    /// 抛出 参数超出范围 异常。
    /// </summary>
    /// <param name="paramName">超出范围的参数名称。</param>
    /// <param name="actualValue">导致此异常的参数值。</param>
    /// <exception cref="System.ArgumentOutOfRangeException"> 对象。</exception>
    /// <overloads>
    /// <summary>
    /// 抛出 参数超出范围 异常。
    /// </summary>
    /// </overloads>
    public static void ArgumentOutOfRange(string paramName, object actualValue) {
        throw new System.ArgumentOutOfRangeException(paramName, actualValue, "参数超出范围");
    }
    /// <summary>
    /// 抛出 参数超出范围 异常。
    /// </summary>
    /// <param name="paramName">超出范围的参数名称。</param>
    /// <param name="actualValue">导致此异常的参数值。</param>
    /// <param name="begin">参数有效范围的起始值。</param>
    /// <param name="end">参数有效范围的结束值。</param>
    /// <exception cref="System.ArgumentOutOfRangeException"> 对象。</exception>
    public static void ArgumentOutOfRange(string paramName, object actualValue,
        object begin, object end) {
        throw new System.ArgumentOutOfRangeException(paramName, actualValue,
            string.Format("参数超出范围{0},{1}", begin, end));
    }
    /// <summary>
    /// 抛出 参数最小值大于最大值 异常。
    /// </summary>
    /// <param name="minParamName">表示最小值的参数名称。</param>
    /// <param name="maxParamName">表示最大值的参数名称。</param>
    /// <exception cref="System.ArgumentOutOfRangeException"> 对象。</exception>
    public static void ArgumentMinMaxValue(string minParamName, string maxParamName) {
        throw new System.ArgumentOutOfRangeException(
            string.Format("参数最小值大于最大值,{0}>{1}", minParamName, maxParamName));
    }

    #endregion // System.ArgumentOutOfRangeException

}