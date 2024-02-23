/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol {
    /// <summary>
    /// 类型不匹配异常类。
    /// </summary>
    [System.Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class TypeMismatchException : System.Exception {

        #region ctor
        /// <summary>
        /// 创建 TypeMismatchException 的实例。
        /// </summary>
        public TypeMismatchException() { }
        /// <summary>
        /// 创建 TypeMismatchException 的实例。
        /// </summary>
        /// <param name="message">异常消息。</param>
        public TypeMismatchException(string message)
            : base(message) { }
        /// <summary>
        /// 创建 TypeMismatchException 的实例。
        /// </summary>
        /// <param name="message">异常消息。</param>
        /// <param name="innerException">附加异常。</param>
        public TypeMismatchException(string message, System.Exception innerException)
            : base(message, innerException) { }
        #endregion
    }
}
