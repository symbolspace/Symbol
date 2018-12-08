/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
namespace Symbol.IO.Packing {
    /// <summary>
    /// 打包字段特性，请求对指定类型或对象的字段进行打包。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Struct)]
    public sealed class FieldPackageAttribute : Attribute {
        /// <summary>
        /// 创建FieldPackageAttribute的实例。
        /// </summary>
        public FieldPackageAttribute() {
        }

    }
}