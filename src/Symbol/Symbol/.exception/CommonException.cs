/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
#if netcore
using System.Reflection;
#endif

namespace Symbol {
    /// <summary>
    /// 通用异常类
    /// </summary>
    [System.Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class CommonException : System.Exception {

        #region properties
        /// <summary>
        /// 异常相关的名称。
        /// </summary>
        public string Name { get; private set; }
        #endregion

        #region ctor
        /// <summary>
        /// 创建 CommonException 的实例。
        /// </summary>
        /// <param name="name">相关名称。</param>
        /// <param name="message">异常消息。</param>
        public CommonException(string name, string message)
            : base(message) {
            Name = name;
        }
        /// <summary>
        /// 创建 CommonException 的实例。
        /// </summary>
        /// <param name="name">相关名称。</param>
        /// <param name="message">异常消息。</param>
        /// <param name="innerException">附加异常实例。</param>
        public CommonException(string name, string message, System.Exception innerException)
            : base(message, innerException) {
            Name = name;
        }
        /// <summary>
        /// 创建 CommonException 的实例。
        /// </summary>
        /// <param name="name">相关名称。</param>
        /// <param name="innerException">附加异常实例。</param>
        public CommonException(string name, System.Exception innerException)
            : base(innerException.Message, innerException) {
            Name = name;
        }
#if !netcore
        /// <summary>
        /// 创建 CommonException 的实例。
        /// </summary>
        /// <param name="info">序列化信息实例。</param>
        /// <param name="context">序列化上下文实例。</param>
        protected CommonException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
            Name = (string)info.GetValue("Name", typeof(string));
        }
#endif
        #endregion

        #region methods
#if !netcore
        #region GetObjectData
        /// <summary>
        /// 从序列化中读取数据。
        /// </summary>
        /// <param name="info">序列化信息实例。</param>
        /// <param name="context">序列化上下文实例。</param>
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("Name", Name, typeof(string));
        }
        #endregion
#endif

        /// <summary>
		/// 格式化指定的异常信息。
		/// </summary>
		/// <param name="message">要格式化的异常信息。</param>
		/// <param name="args">格式化信息的参数。</param>
		/// <returns>格式化后的异常信息。</returns>
		private static string Format(string message, params object[] args) {
            return string.Format( message, args);
        }

        #region 临时方法

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
        public static void CheckArgumentNull(object value, string paramName) {
            if (value == null) {
                ThrowArgumentNull(paramName);
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
        public static void CheckArgumentNull(string value, string paramName) {
            if (string.IsNullOrEmpty(value)) {
                ThrowArgumentNull(paramName);
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
        public static void CheckArgumentNull<T>(T value, string paramName) {
            if (value == null) {
                ThrowArgumentNull(paramName);
            }
        }

        /// <summary>
        /// 检查委托的类型是否合法。
        /// </summary>
        /// <param name="type">委托的类型。</param>
        /// <param name="paramName">参数的名称。</param>
        internal static void CheckDelegateType(System.Type type, string paramName) {
            CheckArgumentNull(type, paramName);
#if netcore
            System.Type baseType = type.GetTypeInfo().BaseType;
#else
            System.Type baseType = type.BaseType;
#endif
            if (baseType != typeof(System.MulticastDelegate)) {
                ThrowMustBeDelegate(paramName);
            }
        }

        #endregion 
        /// <summary>
        /// 抛出 已存在 异常。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <exception cref="ExistedException">对象。</exception>
        public static void ThrowExisted(string message) {
            throw new ExistedException(message);
        }
        /// <summary>
        /// 抛出 格式 异常。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <exception cref="System.FormatException">对象。</exception>
        public static void ThrowFormat(string message) {
            throw new System.FormatException(message);
        }
        /// <summary>
        /// 抛出 不支持 异常。
        /// </summary>
        /// <exception cref="System.NotSupportedException"> 对象。</exception>
        public static void ThrowNotSupported() {
            throw new System.NotSupportedException();
        }
        /// <summary>
        /// 抛出 不支持 异常。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <exception cref="System.NotSupportedException"> 对象。</exception>
        public static void ThrowNotSupported(string message) {
            throw new System.NotSupportedException(message);
        }
        /// <summary>
        /// 抛出 平台不支持 异常。
        /// </summary>
        /// <exception cref="System.PlatformNotSupportedException"> 对象。</exception>
        public static void ThrowPlatformNotSupported() {
            throw new System.PlatformNotSupportedException();
        }
        /// <summary>
        /// 抛出 平台不支持 异常。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <exception cref="System.PlatformNotSupportedException"> 对象。</exception>
        public static void ThrowPlatformNotSupported(string message) {
            throw new System.PlatformNotSupportedException(message);
        }
        /// <summary>
        /// 抛出 未实现 异常。
        /// </summary>
        /// <exception cref="System.NotImplementedException"> 对象。</exception>
        public static void ThrowNotImplemented() {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// 抛出 未实现 异常。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <exception cref="System.NotImplementedException"> 对象。</exception>
        public static void ThrowNotImplemented(string message) {
            throw new System.NotImplementedException(message);
        }
        /// <summary>
        /// 抛出 空引用 异常。
        /// </summary>
        /// <exception cref="System.NullReferenceException"> 对象。</exception>
        public static void ThrowNullReference() {
            throw new System.NullReferenceException();
        }
        /// <summary>
        /// 抛出 空引用 异常。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <exception cref="System.NullReferenceException"> 对象。</exception>
        public static void ThrowNullReference(string message) {
            throw new System.NullReferenceException(message);
        }
        /// <summary>
        /// 抛出 文件不存在 异常。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <exception cref="System.IO.FileNotFoundException"> 对象。</exception>
        public static void ThrowFileNotFound(string message) {
            throw new System.IO.FileNotFoundException(message);
        }
        /// <summary>
        /// 抛出 无效的操作 异常。
        /// </summary>
        /// <exception cref="System.InvalidOperationException"> 对象。</exception>
        public static void ThrowInvalidOperation(string message) {
            throw new System.InvalidOperationException(message);
        }
        /// <summary>
        /// 抛出 无效的转换 异常。
        /// </summary>
        /// <exception cref="System.InvalidCastException"> 对象。</exception>
        public static void ThrowInvalidCast(string message) {
            throw new System.InvalidCastException(message);
        }
        /// <summary>
        /// 抛出 类型加载 异常。
        /// </summary>
        /// <exception cref="System.TypeLoadException"> 对象。</exception>
        public static void ThrowTypeLoad(string message) {
            throw new System.TypeLoadException(message);
        }
        /// <summary>
        /// 抛出 类型不匹配 异常。
        /// </summary>
        /// <exception cref="TypeMismatchException"> 对象。</exception>
        public static void ThrowTypeMismatch(string message) {
            throw new TypeMismatchException(message);
        }
        #region System.ArgumentException
        /// <summary>
        /// 抛出 参数无效 异常。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        public static void ThrowArgument(string paramName) {
            throw new System.ArgumentException(paramName);
        }
        /// <summary>
        /// 抛出 参数无效 异常。
        /// </summary>
        /// <param name="paramName">参数名。</param>
        /// <param name="message">错误消息。</param>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        public static void ThrowArgument(string paramName, string message) {
            throw new System.ArgumentException(message, paramName);
        }

        #region 数组异常

        /// <summary>
        /// 抛出 数组为空 异常。
        /// </summary>
        /// <param name="paramName">产生异常的参数名称。</param>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        public static void ThrowArrayEmpty(string paramName) {
            throw new System.ArgumentException("数组为空", paramName);
        }
        /// <summary>
        /// 抛出 数组下限不为 <c>0</c> 异常。
        /// </summary>
        /// <param name="paramName">产生异常的参数名称。</param>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        public static void ThrowArrayNonZeroLowerBound(string paramName) {
            throw new System.ArgumentException("返回数组下限不为 0", paramName);
        }
        /// <summary>
        /// 抛出 目标数组太小而不能复制集合 异常。
        /// </summary>
        /// <param name="paramName">产生异常的参数名称。</param>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        public static void ThrowArrayTooSmall(string paramName) {
            throw new System.ArgumentException("数组太小", paramName);
        }
        /// <summary>
        /// 抛出 偏移量和长度超出界限 异常。
        /// </summary>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        public static void ThrowInvalidOffsetLength() {
            throw new System.ArgumentException("偏移量和长度超出界限");
        }
        /// <summary>
        /// 抛出 数组长度不同 异常。
        /// </summary>
        /// <param name="paramName">产生异常的参数名称。</param>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        public static void ThrowArrayLengthsDiffer(string paramName) {
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
        public static void ThrowArgumentNull(string paramName) {
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
        public static void ThrowArgumentMustBePositive(string paramName, object actualValue) {
            throw new System.ArgumentOutOfRangeException(paramName, actualValue, "参数小于等于零");
        }
        /// <summary>
        /// 抛出 参数小于零 异常。
        /// </summary>
        /// <param name="paramName">异常参数的名称。</param>
        /// <param name="actualValue">导致此异常的参数值。</param>
        /// <exception cref="System.ArgumentOutOfRangeException"> 对象。</exception>
        public static void ThrowArgumentNegative(string paramName, object actualValue) {
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
        public static void ThrowArgumentOutOfRange(string paramName, object actualValue) {
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
        public static void ThrowArgumentOutOfRange(string paramName, object actualValue,
            object begin, object end) {
            throw new System.ArgumentOutOfRangeException(paramName, actualValue,
                Format("参数超出范围{0},{1}", begin, end));
        }
        /// <summary>
        /// 抛出 参数最小值大于最大值 异常。
        /// </summary>
        /// <param name="minParamName">表示最小值的参数名称。</param>
        /// <param name="maxParamName">表示最大值的参数名称。</param>
        /// <exception cref="System.ArgumentOutOfRangeException"> 对象。</exception>
        public static void ThrowArgumentMinMaxValue(string minParamName, string maxParamName) {
            throw new System.ArgumentOutOfRangeException(
                Format("参数最小值大于最大值,{0}>{1}", minParamName, maxParamName));
        }

        #endregion // System.ArgumentOutOfRangeException

        #region DelegateBuilder 异常

        /// <summary>
        /// 抛出 绑定到目标方法出错 异常。
        /// </summary>
        /// <param name="paramName">产生异常的参数名称。</param>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        internal static void ThrowBindTargetMethod(string paramName) {
            throw new System.ArgumentException("绑定到目标方法出错", paramName);
        }
        /// <summary>
        /// 抛出 绑定到目标属性出错 异常。
        /// </summary>
        /// <param name="paramName">产生异常的参数名称。</param>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        internal static void ThrowBindTargetProperty(string paramName) {
            throw new System.ArgumentException("绑定到目标属性出错", paramName);
        }
        /// <summary>
        /// 抛出 绑定到目标属性出错，不存在 set 访问器 异常。
        /// </summary>
        /// <param name="paramName">产生异常的参数名称。</param>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        internal static void ThrowBindTargetPropertyNoSet(string paramName) {
            throw new System.ArgumentException("绑定到目标属性出错，不存在 set 访问器的异常", paramName);
        }
        /// <summary>
        /// 抛出 绑定到目标属性出错，不存在 get 访问器 异常。
        /// </summary>
        /// <param name="paramName">产生异常的参数名称。</param>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        internal static void ThrowBindTargetPropertyNoGet(string paramName) {
            throw new System.ArgumentException("绑定到目标属性出错，不存在 get 访问器的异常", paramName);
        }
        /// <summary>
        /// 抛出 绑定到目标字段出错 异常。
        /// </summary>
        /// <param name="paramName">产生异常的参数名称。</param>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        internal static void ThrowBindTargetField(string paramName) {
            throw new System.ArgumentException("绑定到目标字段出错的异常", paramName);
        }
        /// <summary>
        /// 抛出 类型必须从委托派生 异常。
        /// </summary>
        /// <param name="paramName">产生异常的参数名称。</param>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        internal static void ThrowMustBeDelegate(string paramName) {
            throw new System.ArgumentException("类型必须从委托派生", paramName);
        }
        /// <summary>
        /// 抛出 不能是开放泛型类型 异常。
        /// </summary>
        /// <param name="paramName">产生异常的参数名称。</param>
        /// <exception cref="System.ArgumentException"> 对象。</exception>
        internal static void ThrowUnboundGenParam(string paramName) {
            throw new System.ArgumentException("不能是开放泛型类型", paramName);
        }

        #endregion // DelegateBuilder 异常

        #endregion
    }
}