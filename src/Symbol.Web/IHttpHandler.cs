/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 定义使用自定义 HTTP 处理程序同步处理 HTTP Web 请求而实现的协定。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IHttpHandler {
        /// <summary>
        /// 获取一个值，该值指示其他请求是否可以使用实例。
        /// </summary>
        bool IsReusable { get; }
        /// <summary>
        /// 实现自定义 HTTP Web 请求的处理。
        /// </summary>
        /// <param name="context">当前HTTP请求上下文。</param>
        void ProcessRequest(IHttpContext context);
    }
}