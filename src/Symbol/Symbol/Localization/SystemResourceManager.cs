using Symbol;
using Symbol.Localization;
using System.IO;
using System.Resources;

[assembly: TypeImplementMap(typeof(IResourceManager), typeof(SystemResourceManager), "System")]

namespace Symbol.Localization
{
    /// <summary>
    /// 实现：系统资源管理器。
    /// </summary>
    class SystemResourceManager: IResourceManager
    {
        private readonly ResourceManager _resourceManager;

        /// <summary>
        /// 创建对象实例。
        /// </summary>
        /// <param name="resourceManager">资源管理对象。</param>
        public SystemResourceManager(ResourceManager resourceManager)
        {
            Throw.CheckArgumentNull(resourceManager, nameof(resourceManager));
            _resourceManager = resourceManager;
        }


        /// <summary>
        /// 获取资源文本。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的文本内容。</returns>
        public virtual string GetString(string key)
        {
            Throw.CheckArgumentNull(key, nameof(key));
            return _resourceManager.GetString(key);
        }
        /// <summary>
        /// 获取资源对象。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        public virtual object GetObject(string key)
        {
            Throw.CheckArgumentNull(key, nameof(key));
            return _resourceManager.GetObject(key);
        }
        /// <summary>
        /// 获取资源流。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        public virtual Stream GetStream(string key)
        {
            Throw.CheckArgumentNull(key, nameof(key));
            return _resourceManager.GetStream(key);
        }

        /// <summary>
        /// 显式转换。
        /// </summary>
        /// <param name="resourceManager">资源管理对象。</param>
        public static explicit operator ResourceManager(SystemResourceManager resourceManager)
        {
            return resourceManager._resourceManager;
        }
        /// <summary>
        /// 隐式转换。
        /// </summary>
        /// <param name="resourceManager">资源管理对象。</param>
        public static implicit operator SystemResourceManager(ResourceManager resourceManager)
        {
            return new SystemResourceManager(resourceManager);
        }
    }

}