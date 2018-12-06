/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Cloud {
    /// <summary>
    /// 云Action通信协议特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CloudActionProtocolAttribute : Attribute {

        #region properties
        /// <summary>
        /// 获取配置的名称（运行时找不到配置时，将采用默认的）。
        /// </summary>
        public string Name{get; protected set;}

        #endregion

        #region ctor
        /// <summary>
        /// 创建CloudActionProtocolAttribute实例。
        /// </summary>
        /// <param name="name">配置的名称（运行时找不到配置时，将采用默认的）。</param>
        public CloudActionProtocolAttribute(string name) {
            Name = name;
        }
        #endregion
    }
}
