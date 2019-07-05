/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Net {
    /// <summary>
    /// PeekSocket错误消息类
    /// </summary>
    public class PeekSocketErrorEventArgs : System.EventArgs {

        #region properties
        /// <summary>
        /// 获取是否来自发送时的错误。
        /// </summary>
        public bool FromSend { get; private set; }
        /// <summary>
        /// 获取是否为取消，比如SocketStream.Stop或连接中断/超时。
        /// </summary>
        public bool Cancelled { get; private set; }
        /// <summary>
        /// 获取错误消息。
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// 获取详细的异常信息，它不一定有值。
        /// </summary>
        public System.Exception Error { get; private set; }
        #endregion

        #region ctor
        /// <summary>
        /// 创建PeekSocketErrorEventArgs类的实例
        /// </summary>
        /// <param name="fromSend">是否来自发送时的错误</param>
        /// <param name="cancelled">是否为取消，比如PeekSocket.Stop或连接中断/超时</param>
        /// <param name="message">错误消息</param>
        public PeekSocketErrorEventArgs(bool fromSend, bool cancelled, string message) {
            FromSend = fromSend;
            Cancelled = cancelled;
            Message = message;
            Error = null;
        }
        /// <summary>
        /// 创建PeekSocketErrorEventArgs类的实例
        /// </summary>
        /// <param name="fromSend">是否来自发送时的错误</param>
        /// <param name="cancelled">是否为取消，比如PeekSocket.Stop或连接中断/超时</param>
        /// <param name="error">异常信息</param>
        public PeekSocketErrorEventArgs(bool fromSend, bool cancelled, System.Exception error) {
            FromSend = fromSend;
            Cancelled = cancelled;
            Message = error.Message;
            Error = error;
        }
        #endregion
    }
}
