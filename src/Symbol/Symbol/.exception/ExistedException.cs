/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace Symbol {
    /// <summary>
    /// 对象已存在异常类
    /// </summary>
    [System.Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ExistedException : CommonException {

        #region ctor
        /// <summary>
        /// 创建 ExistedException 的实例。
        /// </summary>
        /// <param name="name">相关名称。</param>
        public ExistedException(string name)
            : base(name, name + "已存在") {
        }
        /// <summary>
        /// 创建 ExistedException 的实例。
        /// </summary>
        /// <param name="name">相关名称。</param>
        /// <param name="message">异常消息。</param>
        public ExistedException(string name, string message)
            : base(name, message) {
        }
        /// <summary>
        /// 创建 ExistedException 的实例。
        /// </summary>
        /// <param name="name">相关名称。</param>
        /// <param name="message">异常消息。</param>
        /// <param name="innerException">附加异常实例。</param>
        public ExistedException(string name, string message, System.Exception innerException)
            : base(name, message, innerException) {
        }
        /// <summary>
        /// 创建 ExistedException 的实例。
        /// </summary>
        /// <param name="name">相关名称。</param>
        /// <param name="innerException">附加异常实例。</param>
        public ExistedException(string name, System.Exception innerException)
            : base(name, innerException) {
        }
#if !netcore
        /// <summary>
        /// 创建 ExistedException 的实例。
        /// </summary>
        /// <param name="info">序列化信息实例。</param>
        /// <param name="context">序列化上下文实例。</param>
        protected ExistedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {

        }
#endif
        #endregion

    }
}