/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
#if netcore
using System.Reflection;
#endif

namespace Symbol;
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
