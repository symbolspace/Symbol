/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 封装有关个别 HTTP 请求的所有 HTTP 特定的信息。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IHttpContext {
        /// <summary>
        /// 创建HttpContext的时间。
        /// </summary>
        System.DateTime Timestamp { get; }

        /// <summary>
        /// 站点的唯一标识，每次WebServer启动时会随机分配。
        /// </summary>
        string WebsiteId { get; }
        /// <summary>
        /// 获取当前 HTTP 请求的 IHttpRequest 对象。
        /// </summary>
        IHttpRequest Request { get; }
        /// <summary>
        /// 获取当前 HTTP 响应的 IHttpResponse 对象。
        /// </summary>
        IHttpResponse Response { get; }
        /// <summary>
        /// 获取提供用于处理 Web 请求的方法的 IHttpServerUtility 对象。
        /// </summary>
        IHttpServerUtility Server { get; }
        /// <summary>
        /// 为当前 HTTP 请求获取 IHttpSessionState 对象（注意当配置中设置session状态的enable为false时，这里将返回null值，不会创建session对象。）。
        /// </summary>
        IHttpSessionState Session { get; }
        /// <summary>
        /// 获取当前 HTTP 请求的 IHttpApplicationState 对象。
        /// </summary>
        IHttpApplicationState Application { get; }
        /// <summary>
        /// 获取当前 HTTP 请求的 IHttpApplication 对象（配置中handlers的applicationType属性设置正确，并且成功创建了实例才会有值。）。
        /// </summary>
        IHttpApplication ApplicationInstance { get; }

        /// <summary>
        /// 重写Session，实现Session共享，未启用Session时，调用后没有任何效果。
        /// </summary>
        /// <param name="sessionId">如果这个session不存在，会创建一个新的（新的sessionid会与这个参数不相同）。</param>
        void RewriteSession(string sessionId);
    }
}