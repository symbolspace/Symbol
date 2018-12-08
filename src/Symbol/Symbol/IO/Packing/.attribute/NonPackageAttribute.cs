/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
namespace Symbol.IO.Packing {
    /// <summary>
    /// 不打包特性，请求不要将指定的类型或对象进行打包处理。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Struct)]
    public sealed class NonPackageAttribute : Attribute {
        /// <summary>
        /// 创建NonPackageAttribute的实例。
        /// </summary>
        public NonPackageAttribute() {
        }

    }
}