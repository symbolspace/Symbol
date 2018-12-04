/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Formatting {

    /// <summary>
    /// 数据主体特性（表示参数可以尝试从数据主体构建）。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class DataBodyAttribute : Attribute {
        /// <summary>
        /// 获取或设置无效的键名（多个用英文分号分隔）。
        /// </summary>
        public string InvalidKeys { get; set; }

        /// <summary>
        /// 创建DataBodyAttribute实例。
        /// </summary>
        public DataBodyAttribute() {
        }
        /// <summary>
        /// 创建DataBodyAttribute实例。
        /// </summary>
        /// <param name="invalidKeys">无效的键名（多个用英文分号分隔）。</param>
        public DataBodyAttribute(string invalidKeys) {
        }

    }

}