using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Symbol.Localization
{
    /// <summary>
    /// 接口：区域资源管理器。
    /// </summary>
    public interface ICultureResourceManager
    {
        
        /// <summary>
        /// 获取区域数量。
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 获取区域名称清单。
        /// </summary>
        string[] Names { get; }
        /// <summary>
        /// 获取区域清单。
        /// </summary>
        IEnumerable<CultureInfo> Cultures { get; }

        /// <summary>
        /// 添加资源。
        /// </summary>
        /// <param name="resourceManager">资源管理对象。</param>
        /// <remarks>区域名称为 System.Threading.Thread.CurrentThread.CurrentUICulture.Name。</remarks>
        void AddResource(IResourceManager resourceManager);
        /// <summary>
        /// 添加资源。
        /// </summary>
        /// <param name="resourceManager">资源管理对象。</param>
        /// <param name="cultureName">区域名称，e.g zh-CN。</param>
        void AddResource(IResourceManager resourceManager, string cultureName);
        /// <summary>
        /// 添加资源。
        /// </summary>
        /// <param name="resourceManager">资源管理对象。</param>
        /// <remarks>区域名称为 System.Threading.Thread.CurrentThread.CurrentUICulture.Name。</remarks>
        void AddResource(ResourceManager resourceManager);
        /// <summary>
        /// 添加资源。
        /// </summary>
        /// <param name="resourceManager">资源管理对象。</param>
        /// <param name="cultureName">区域名称，e.g zh-CN。</param>
        void AddResource(ResourceManager resourceManager, string cultureName);
        /// <summary>
        /// 添加资源。
        /// </summary>
        /// <param name="cultureResourceAttribute">区域资源特性。</param>
        void AddResource(CultureResourceAttribute cultureResourceAttribute);
        /// <summary>
        /// 扫描资源。
        /// </summary>
        /// <remarks>从当前已加载的程序集中扫描。</remarks>
        void Scan();
        /// <summary>
        /// 扫描资源。
        /// </summary>
        /// <param name="assemblies">待扫描的程序集清单，为空不会报错。</param>
        void Scan(IEnumerable<Assembly> assemblies);
        /// <summary>
        /// 扫描资源。
        /// </summary>
        /// <param name="assembly">包含区域资源特性的程序集。e.g [assembly: CultureResource("zh-CN", typeof(Resources.zh_CN))]。</param>
        void Scan(Assembly assembly);


        /// <summary>
        /// 获取资源清单。
        /// </summary>
        /// <returns>返回该区域的所有资源。</returns>
        /// <remarks>区域名称为 System.Threading.Thread.CurrentThread.CurrentUICulture.Name，区域为空、区域不存在，均返回null。</remarks>
        IEnumerable<IResourceManager> GetResources();
        /// <summary>
        /// 获取资源清单。
        /// </summary>
        /// <param name="cultureName">区域名称，e.g zh-CN。</param>
        /// <returns>返回该区域的所有资源。</returns>
        /// <remarks>区域为空、区域不存在，均返回null。</remarks>
        IEnumerable<IResourceManager> GetResources(string cultureName);


        /// <summary>
        /// 获取资源文本（当前区域）。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的文本内容。</returns>
        /// <remarks>区域名称为 System.Threading.Thread.CurrentThread.CurrentUICulture.Name，资源标识为空、资源标识不存在，均返回为string.Empty。</remarks>
        string GetString(string key);
        /// <summary>
        /// 获取资源文本。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <param name="cultureName">区域名称，e.g zh-CN</param>
        /// <returns>返回该资源标识对应的文本内容。</returns>
        /// <remarks>资源标识为空、区域名称为空、资源标识不存在，均返回为string.Empty。</remarks>
        string GetString(string key, string cultureName);

        /// <summary>
        /// 获取资源对象。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        /// <remarks>区域名称为 System.Threading.Thread.CurrentThread.CurrentUICulture.Name，资源标识为空、区域名称为空、资源标识不存在，均返回为null。</remarks>
        object GetObject(string key);
        /// <summary>
        /// 获取资源对象。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <param name="cultureName">区域名称，e.g zh-CN</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        /// <remarks>资源标识为空、区域名称为空、资源标识不存在，均返回为null。</remarks>
        object GetObject(string key, string cultureName);


        /// <summary>
        /// 获取资源流。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        /// <remarks>区域名称为 System.Threading.Thread.CurrentThread.CurrentUICulture.Name，资源标识为空、区域名称为空、资源标识不存在，均返回为null。</remarks>
        Stream GetStream(string key);
        /// <summary>
        /// 获取资源流。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <param name="cultureName">区域名称，e.g zh-CN</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        /// <remarks>资源标识为空、区域名称为空、资源标识不存在，均返回为null。</remarks>
        Stream GetStream(string key, string cultureName);
    }

}