/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Cloud {

    /// <summary>
    /// 云Action过滤器特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method| AttributeTargets.Class,Inherited=true, AllowMultiple =true)]
    public abstract class CloudActionFilterAttribute : Attribute {

        #region properties

        /// <summary>
        /// 获取或设置顺序，为0表示保持默认顺序（与特性标记的顺序不一致）。
        /// </summary>
        public double Sort { get; set; }

        #endregion

        #region ctor
        /// <summary>
        /// 创建CloudActionFilterAttribute实例。
        /// </summary>
        public CloudActionFilterAttribute() {
        }
        #endregion

        #region methods

        /// <summary>
        /// Action调用前（如果设置context.Processed为true，将导致ActionExecute不执行）。
        /// </summary>
        /// <param name="context">云调用上下文。</param>
        public virtual void ActionExecuteBefore(CloudContext context) {
        }
        /// <summary>
        /// Action调用。
        /// </summary>
        /// <param name="context">云调用上下文。</param>
        public virtual void ActionExecute(CloudContext context) {
        }
        /// <summary>
        /// Action调用后。
        /// </summary>
        /// <param name="context">云调用上下文。</param>
        public virtual void ActionExecuteAfter(CloudContext context) {
        }
        #endregion
    }
}
