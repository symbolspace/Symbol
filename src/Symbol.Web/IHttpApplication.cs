/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 定义应用程序中的所有应用程序对象共有的方法、属性和事件。此类是用户在站点配置文档 handlers节点的applicationType属性所定义的应用程序的基类。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IHttpApplication : System.IDisposable {
        /// <summary>
        /// 当前请求上下文实例，可以获取到 Request、Response、Session 等对象（只有处理请求时有值，在初始化，或者其它时候是没有值的）。
        /// </summary>
        IHttpContext Context { get; }

        /// <summary>
        /// 在添加所有事件处理程序模块之后执行自定义初始化代码。
        /// </summary>
        void Init();
        /// <summary>
        /// 设置当前上下文。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        void SetContext(IHttpContext context);
    }
}