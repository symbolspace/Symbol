/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
namespace Symbol.IO.Packing {
    /// <summary>
    /// 打包属性特性，请求对指定类型或对象的属性进行打包。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Struct)]
    public sealed class PropertyPackageAttribute : Attribute {
        /// <summary>
        /// 创建PropertyPackageAttribute的实例。
        /// </summary>
        public PropertyPackageAttribute() {
        }
    }
}