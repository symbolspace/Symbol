/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Cloud {

    /// <summary>
    /// 云控制器接口
    /// </summary>
    public interface ICloudController :IDisposable{
        /// <summary>
        /// 单例初始化。
        /// </summary>
        /// <param name="server">服务器实例。</param>
        void SingletonInit(ICloudServer server);
        /// <summary>
        /// 初始化控制器。
        /// </summary>
        /// <param name="context">云调用上下文。</param>
        void Init(CloudContext context);
        /// <summary>
        /// Action调用前（如果设置context.Processed为true，将导致ActionExecute不执行）。
        /// </summary>
        /// <param name="context">云调用上下文。</param>
        void ActionExecuteBefore(CloudContext context);
        /// <summary>
        /// Action调用。
        /// </summary>
        /// <param name="context">云调用上下文。</param>
        void ActionExecute(CloudContext context);
        /// <summary>
        /// Action调用后。
        /// </summary>
        /// <param name="context">云调用上下文。</param>
        void ActionExecuteAfter(CloudContext context);

    }


    
}
