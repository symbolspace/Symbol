/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Formatting {

    /// <summary>
    /// 标记在序列化时输出的名称。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class AliasAttribute : Attribute {
        /// <summary>
        /// 获取或设置输出名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 创建AliasAttribute实例。
        /// </summary>
        /// <param name="name">输出名称。</param>
        public AliasAttribute(string name) {
            Name = name;
        }
    }

}