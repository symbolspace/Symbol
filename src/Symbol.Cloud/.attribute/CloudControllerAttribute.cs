/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Cloud {

    /// <summary>
    /// 云控制器特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CloudControllerAttribute : Attribute {

        #region properties
        /// <summary>
        /// 获取或设置显示名称。
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 获取或设置调用路径，以/开头和结尾；{withTypeName}采用类名规则为：area_user_test，实际为/area/user/test/；{withNamespace}采用命名空间规则为：area.user.test，实际为：/area/user/test/。
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 获取或设置描述信息。
        /// </summary>
        public string Description { get; protected set; }
        /// <summary>
        /// 获取或设置是否为单例模式(此设置运行基于Web上的controller无效)。
        /// </summary>
        public bool Singleton { get; set; }
        /// <summary>
        /// 获取或设置是否强制替换已存在的的API
        /// </summary>
        public bool ReplaceExists { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// 创建CloudControllerAttribute实例。
        /// </summary>
        /// <param name="displayName">显示名称</param>
        /// <param name="url">调用路径，以/开头和结尾；{withTypeName}采用类名规则为：area_user_test，实际为/area/user/test/；{withNamespace}采用命名空间规则为：area.user.test，实际为：/area/user/test/</param>
        /// <param name="description">描述信息。</param>
        public CloudControllerAttribute(string displayName,string url,string description="") {
            DisplayName = displayName;
            Url = url;
            Description = description;
        }
        #endregion
    }
}
