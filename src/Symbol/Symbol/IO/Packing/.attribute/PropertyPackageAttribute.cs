/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
using System;
namespace Symbol.IO.Packing {
    /// <summary>
    /// ����������ԣ������ָ�����ͻ��������Խ��д����
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Struct)]
    public sealed class PropertyPackageAttribute : Attribute {
        /// <summary>
        /// ����PropertyPackageAttribute��ʵ����
        /// </summary>
        public PropertyPackageAttribute() {
        }
    }
}