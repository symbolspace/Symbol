using System;
using System.Resources;

namespace Symbol.Localization
{
    /// <summary>
    /// 特性：区域资源。
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class CultureResourceAttribute : Attribute
    {
        private readonly string _cultureName;
        private readonly IResourceManager _resourceManager;

        /// <summary>
        /// 获取区域名称。
        /// </summary>
        public string CultureName { get { return _cultureName; } }
        /// <summary>
        /// 获取资源管理对象。
        /// </summary>
        public IResourceManager ResourceManager { get { return _resourceManager; } }

        /// <summary>
        /// 创建对象实例。
        /// </summary>
        /// <param name="cultureName">区域名称，e.g zh-CN。</param>
        /// <param name="resourceManager">资源管理对象。</param>
        public CultureResourceAttribute(string cultureName, IResourceManager resourceManager)
        {
            Throw.CheckArgumentNull(cultureName, nameof(cultureName));
            Throw.CheckArgumentNull(resourceManager, nameof(resourceManager));

            _cultureName = cultureName;
            _resourceManager = resourceManager;
        }
        /// <summary>
        /// 创建对象实例。
        /// </summary>
        /// <param name="cultureName">区域名称，e.g zh-CN。</param>
        /// <param name="resourceManager">资源管理对象。</param>
        public CultureResourceAttribute(string cultureName, ResourceManager resourceManager)
        {
            Throw.CheckArgumentNull(cultureName, nameof(cultureName));
            Throw.CheckArgumentNull(resourceManager, nameof(resourceManager));

            _cultureName = cultureName;
            _resourceManager = new SystemResourceManager(resourceManager);
        }

    }

}