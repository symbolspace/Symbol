/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
using System;
namespace Symbol.IO.Packing {
    /// <summary>
    /// ��������ԣ�����Ҫ��ָ�������ͻ������д������
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Struct)]
    public sealed class NonPackageAttribute : Attribute {
        /// <summary>
        /// ����NonPackageAttribute��ʵ����
        /// </summary>
        public NonPackageAttribute() {
        }

    }
}