/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 提供对 HttpListener 类使用的请求和响应对象的访问。能够读取客户端在 Web 请求期间发送的 HTTP 值。
    /// </summary>
    public class HttpListenerContext : System.EventArgs, IHttpListenerContext {

        #region fields
        private IHttpListenerRequest _request;
        private IHttpListenerResponse _response;
        private bool _disposed;
        #endregion

        #region properties
        /// <summary>
        /// 获取表示客户端对资源的请求的 IHttpListenerRequest。
        /// </summary>
        public IHttpListenerRequest Request {
            get {
                return _request;
            }
        }

        /// <summary>
        /// 获取 IHttpListenerResponse 对象，该对象将被发送到客户端以响应客户端的请求。
        /// </summary>
        public IHttpListenerResponse Response {
            get {
                return _response;
            }
        }


        #endregion

        #region ctor
        /// <summary>
        /// 创建HttpListenerContext实例。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public HttpListenerContext(IHttpListenerRequest request, IHttpListenerResponse response) {
            _request = request;
            _response = response;
        }
        #endregion

        #region methods

        #region Dispose
        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose() {
            if (_disposed)
                return;
            if (_request != null) {
                _request.Dispose();
                _request = null;
            }
            if (_response != null) {
                _response.Dispose();
                _response = null;
            }
            _disposed = true;
        }
        #endregion

        #endregion
    }

}