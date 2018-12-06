/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Cloud {
    /// <summary>
    /// 云Action返回代码特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true,AllowMultiple=true)]
    public class CloudActionCodeAttribute : Attribute {

        #region properties
        /// <summary>
        /// 获取返回代码值。
        /// </summary>
        public int Code{get;protected set;}
        /// <summary>
        /// 获取返回代码描述。
        /// </summary>
        public string Description{get; protected set;}
        #endregion

        #region ctor
        /// <summary>
        /// 创建CloudActionCodeAttribute实例。
        /// </summary>
        /// <param name="code">返回代码值</param>
        /// <param name="description">返回代码描述</param>
        public CloudActionCodeAttribute(int code,string description) {
            Code = code;
            Description = description;
        }
        #endregion
    }
}
