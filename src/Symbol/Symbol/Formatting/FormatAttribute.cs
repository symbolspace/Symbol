/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Formatting {

    /// <summary>
    /// 标记在序列化时将值按指定定格式串输出。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class FormatAttribute : Attribute {
        /// <summary>
        /// 获取或设置格式串。
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 创建FormatAttribute实例。
        /// </summary>
        /// <param name="format">格式串，不为null或empty时生效。</param>
        public FormatAttribute(string format) {
            Format = format;
        }
    }

}