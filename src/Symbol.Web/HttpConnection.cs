/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// Http连接对象包装类。
    /// </summary>
    public class HttpConnection : IHttpConnection {

        #region fields
        private CloseDelegate _closeAction;
#if net40
        private CloseAsyncDelegate _closeAsycAction;
#endif
        private IsClientConnectedDelegate _isClientConnectedAction;
        #endregion

        #region properties
        /// <summary>
        /// 获取一个值，通过该值指示客户端是否仍连接在服务器上。
        /// </summary>
        /// <returns>如果客户端当前仍在连接，则为 true；否则为 false。</returns>
        public bool IsClientConnected {
            get {
                return _isClientConnectedAction == null ? false : _isClientConnectedAction();
            }
        }

        /// <summary>
        /// 关闭到客户端的连接而不发送响应。
        /// </summary>
        public void Close() {
            _closeAction?.Invoke();
            _closeAction = null;
#if net40
            _closeAsycAction?.Invoke();
            _closeAsycAction = null;
#endif
            _isClientConnectedAction = null;
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建HttpConnection实例。
        /// </summary>
        /// <param name="closeAction"></param>
        /// <param name="isClientConnectedAction"></param>
        public HttpConnection(CloseDelegate closeAction, IsClientConnectedDelegate isClientConnectedAction) {
            _closeAction = closeAction;
            _isClientConnectedAction = isClientConnectedAction;
        }
#if net40
        /// <summary>
        /// 创建HttpConnection实例。
        /// </summary>
        /// <param name="closeAction"></param>
        /// <param name="isClientConnectedAction"></param>
        public HttpConnection(CloseAsyncDelegate closeAction, IsClientConnectedDelegate isClientConnectedAction) {
            _closeAsycAction = closeAction;
            _isClientConnectedAction = isClientConnectedAction;
        }
#endif
        #endregion

        #region types
        /// <summary>
        /// 客户端是否断开连接委托。
        /// </summary>
        /// <returns>返回true表示未断开。</returns>
        public delegate bool IsClientConnectedDelegate();
        /// <summary>
        /// 关闭客户端连接委托。
        /// </summary>
        public delegate void CloseDelegate();
#if net40
        /// <summary>
        /// 关闭客户端连接委托。
        /// </summary>
        public delegate System.Threading.Tasks.Task CloseAsyncDelegate();
#endif
        #endregion
    }
}