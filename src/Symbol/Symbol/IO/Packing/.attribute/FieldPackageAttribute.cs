/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
using System;
namespace Symbol.IO.Packing {
    /// <summary>
    /// ����ֶ����ԣ������ָ�����ͻ������ֶν��д����
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Struct)]
    public sealed class FieldPackageAttribute : Attribute {
        /// <summary>
        /// ����FieldPackageAttribute��ʵ����
        /// </summary>
        public FieldPackageAttribute() {
        }

    }
}