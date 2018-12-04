/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Formatting {

    /// <summary>
    /// 标记在序列化时额外输出path结果。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ExtraPathAttribute : FormatAttribute {
        /// <summary>
        /// 获取或设置输出名称。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 获取或设置Path路径。
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 获取或设置默认值。
        /// </summary>
        public object DefaultValue { get; set; }
        /// <summary>
        /// 创建ExtraPathAttribute实例。
        /// </summary>
        /// <param name="name">输出名称。</param>
        /// <param name="path">Path路径。</param>
        public ExtraPathAttribute(string name, string path)
            : this(name, path, null) {
        }
        /// <summary>
        /// 创建ExtraPathAttribute实例。
        /// </summary>
        /// <param name="name">输出名称。</param>
        /// <param name="path">Path路径。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <param name="format">格式串，不为null或empty时生效。</param>
        public ExtraPathAttribute(string name, string path, object defaultValue, string format = null) : base(format) {
            Name = name;
            Path = path;
            DefaultValue = defaultValue;
        }
    }

}