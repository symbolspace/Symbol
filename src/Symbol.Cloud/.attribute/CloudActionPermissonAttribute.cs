/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Cloud {
    /// <summary>
    /// 云Action权限集特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true,AllowMultiple=true)]
    public class CloudActionPermissonAttribute : Attribute {

        #region properties
        /// <summary>
        /// 获取或设置权限集。
        /// </summary>
        public string[] Permissons { get; set; }
        /// <summary>
        /// 获取是否允许任意身份调用。
        /// </summary>
        public bool IsAny { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// 创建CloudActionAttribute实例。
        /// </summary>
        /// <param name="permissons">权限集</param>
        public CloudActionPermissonAttribute(string permissons=null) {
            Permissons = string.IsNullOrEmpty(permissons) ? new string[0] : permissons.Split(',');
            IsAny = Permissons.Length == 0 || Array.Exists(Permissons, p => p == "*");
        }
        #endregion
    }
}
