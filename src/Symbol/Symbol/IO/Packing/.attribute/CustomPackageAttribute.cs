/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
using System;
namespace Symbol.IO.Packing {
    /// <summary>
    /// �Զ��������ԣ������ָ�����ͻ��������Զ�������
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Struct)]
    public sealed class CustomPackageAttribute : Attribute {
        /// <summary>
        /// ʵ����Symbol.IO.Packing.ICustomPackage�ӿڵ����ͣ������Ƿǳ������ӵ��һ���޲�����public���캯����
        /// </summary>
        public Type CustomPackageType { get; private set; }
        /// <summary>
        /// ����CustomPackageAttribute��ʵ����
        /// </summary>
        public CustomPackageAttribute(Type customPackageType) {
            if (customPackageType == null)
                throw new ArgumentNullException("customPackageType", "customPackageType ����Ϊ�ա�");
            if (!TypeExtensions.IsInheritFrom(customPackageType,typeof(ICustomPackage)))
                throw new ArgumentException("customPackageType", "customPackageType����̳� Symbol.IO.Packing.ICustomPackage");
            if (customPackageType.IsAbstract)
                throw new ArgumentException("customPackageType", "customPackageType�����ǳ����ࡣ");
            if (customPackageType.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException("customPackageType", "customPackageType����ӵ��һ���޲�����public���캯����");
            CustomPackageType = customPackageType;
        }

        /// <summary>
        /// ��������Ϊbyte[]
        /// </summary>
        /// <param name="instance">��Ҫ�����ʵ����û��null�������</param>
        /// <returns>���ش��������ݡ�</returns>
        public byte[] Save(object instance) {
            return ((ICustomPackage)FastObject.CreateInstance(CustomPackageType)).Save(instance);
        }
        /// <summary>
        /// ��byte[]�м��ض���
        /// </summary>
        /// <param name="buffer">��������ݡ�</param>
        /// <returns>���ؽ�����Ķ���</returns>
        public object Load(byte[] buffer) {
            return ((ICustomPackage)FastObject.CreateInstance(CustomPackageType)).Load(buffer);
        }
    }
}