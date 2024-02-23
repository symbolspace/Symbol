/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
namespace Symbol.IO.Packing {
    /// <summary>
    /// 自定义打包特性，请求对指定类型或对象进行自定义打包。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Struct)]
    public sealed class CustomPackageAttribute : Attribute {
        /// <summary>
        /// 实现了Symbol.IO.Packing.ICustomPackage接口的类型，并且是非抽象类和拥有一个无参数的public构造函数。
        /// </summary>
        public Type CustomPackageType { get; private set; }
        /// <summary>
        /// 创建CustomPackageAttribute的实例。
        /// </summary>
        public CustomPackageAttribute(Type customPackageType) {
            if (customPackageType == null)
                throw new ArgumentNullException("customPackageType", "customPackageType 不能为空。");
            if (!TypeExtensions.IsInheritFrom(customPackageType,typeof(ICustomPackage)))
                throw new ArgumentException("customPackageType", "customPackageType必须继承 Symbol.IO.Packing.ICustomPackage");
            if (customPackageType.IsAbstract)
                throw new ArgumentException("customPackageType", "customPackageType不能是抽象类。");
            if (customPackageType.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException("customPackageType", "customPackageType必须拥有一个无参数的public构造函数。");
            CustomPackageType = customPackageType;
        }

        /// <summary>
        /// 将对象打包为byte[]
        /// </summary>
        /// <param name="instance">需要保存的实例，没有null的情况。</param>
        /// <returns>返回打包后的数据。</returns>
        public byte[] Save(object instance) {
            return ((ICustomPackage)FastObject.CreateInstance(CustomPackageType)).Save(instance);
        }
        /// <summary>
        /// 从byte[]中加载对象。
        /// </summary>
        /// <param name="buffer">对象的数据。</param>
        /// <returns>返回解析后的对象。</returns>
        public object Load(byte[] buffer) {
            return ((ICustomPackage)FastObject.CreateInstance(CustomPackageType)).Load(buffer);
        }
    }
}