/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Cloud {

    /// <summary>
    /// 云参数特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CloudArgsAttribute : Attribute {
        /// <summary>
        /// 获取或设置类型。
        /// </summary>
        public System.Type Type { get; set; }

        /// <summary>
        /// 创建CloudArgsAttribute实例。
        /// </summary>
        /// <param name="type">类型。</param>
        public CloudArgsAttribute(System.Type type) {
            Type = type;
        }

        /// <summary>
        /// 加载。
        /// </summary>
        /// <param name="list">用于存储的列表。</param>
        /// <param name="attributeProvider">特性提供者。</param>
        public static void Load(ParameterInfoList list, System.Reflection.ICustomAttributeProvider attributeProvider) {
            if (list == null || attributeProvider == null)
                return;
            foreach (var attribute in AttributeExtensions.GetCustomAttributes<CloudArgsAttribute>(attributeProvider, true)) {
                if (attribute.Type == null)
                    continue;
                list.AddRange(PropertyParameterInfo.As(
                                       FastWrapper.GetProperties(attribute.Type, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, true, true)
                                 )
                             );
            }
        }
    }
}
