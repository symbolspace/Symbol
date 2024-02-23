/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace Symbol {
    /// <summary>
    /// 对象已释放异常类
    /// </summary>
    [System.Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class DisposedException : System.Exception {

        #region ctor
        /// <summary>
        /// 创建 DisposedException 的实例。
        /// </summary>
        public DisposedException()
            : this("对象已释放") { }
        /// <summary>
        /// 创建 DisposedException 的实例。
        /// </summary>
        /// <param name="message">异常消息。</param>
        public DisposedException(string message)
            : base(message) { }
        /// <summary>
        /// 创建 DisposedException 的实例。
        /// </summary>
        /// <param name="message">异常消息。</param>
        /// <param name="innerException">附加异常实例。</param>
        public DisposedException(string message, System.Exception innerException)
            : base(message, innerException) { }
        #endregion

    }
}