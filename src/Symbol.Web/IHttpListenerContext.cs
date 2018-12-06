/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 提供对 HttpListener 类使用的请求和响应对象的访问。能够读取客户端在 Web 请求期间发送的 HTTP 值。
    /// </summary>
    public interface IHttpListenerContext : System.IDisposable{

        /// <summary>
        /// 获取表示客户端对资源的请求的 IHttpListenerRequest。
        /// </summary>
        IHttpListenerRequest Request { get; }
        /// <summary>
        /// 获取 IHttpListenerResponse 对象，该对象将被发送到客户端以响应客户端的请求。
        /// </summary>
        IHttpListenerResponse Response { get; }
        
    }
}