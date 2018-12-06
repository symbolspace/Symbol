/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Cloud {

    /// <summary>
    /// 云Action特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,Inherited=true, AllowMultiple =true)]
    public class CloudActionAttribute : Attribute {

        #region properties
        /// <summary>
        /// 获取或设置显示名称。
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 获取或设置描述信息。
        /// </summary>
        public string Description { get; protected set; }
        /// <summary>
        /// 获取或设置Url规则。
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 获取或设置不返回数据(仅在纯TCP通道中有此效果)。
        /// </summary>
        public bool NoReturn { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// 创建CloudActionAttribute实例。
        /// </summary>
        /// <param name="displayName">显示名称</param>
        /// <param name="description">描述信息。</param>
        public CloudActionAttribute(string displayName, string description = "") {
            DisplayName = displayName;
            Description = description;
        }
        #endregion
    }

}
