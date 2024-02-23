using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Symbol.Localization
{
    /// <summary>
    /// 接口：本地化渲染器管理。
    /// </summary>
    public interface ILocalizationRenderManager
    {
        /// <summary>
        /// 获取区域资源管理对象。
        /// </summary>
        ICultureResourceManager CultureResourceManager { get; }

        /// <summary>
        /// 获取元素数量。
        /// </summary>
        int ElementCount { get; }

        /// <summary>
        /// 获取或设置当前区域名称。
        /// </summary>
        /// <remarks>不接受空值设置。成功设置后自动应用到已注册的渲染器。</remarks>
        string CultureName { get; set; }

        /// <summary>
        /// 事件：区域名称变更。
        /// </summary>
        event LocalizationCultureNameChangedHandler CultureNameChanged;

        /// <summary>
        /// 添加渲染器。
        /// </summary>
        /// <param name="render">渲染器对象。</param>
        /// <param name="elementType">元素类型。</param>
        void AddRender(ILocalizationRender render, Type elementType);
        /// <summary>
        /// 添加渲染器。
        /// </summary>
        /// <param name="localizationRenderAttribute">渲染器特性。</param>
        void AddRender(LocalizationRenderAttribute localizationRenderAttribute);

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
        /// <param name="assembly">包含区域资源特性的程序集。e.g [assembly: LocalizationRender( typeof(XXXControl), typeof(XXXXRender))]。</param>
        void Scan(Assembly assembly);


        /// <summary>
        /// 添加元素。
        /// </summary>
        /// <param name="element">目标元素。</param>
        void AddElement(object element);
        /// <summary>
        /// 移除元素。
        /// </summary>
        /// <param name="element">目标元素。</param>
        void RemoveElement(object element);

        /// <summary>
        /// 渲染所有元素。
        /// </summary>
        void RenderAll();
        /// <summary>
        /// 渲染元素。
        /// </summary>
        /// <param name="element">目标元素。</param>
        void Render(object element);
        /// <summary>
        /// 渲染元素。
        /// </summary>
        /// <param name="element">目标元素。</param>
        /// <param name="rootElement">根元素，定义或容纳当前元素的对象。</param>
        void Render(object element, object rootElement);

        /// <summary>
        /// 获取资源文本。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的文本内容。</returns>
        /// <remarks>资源标识为空、资源标识不存在，均返回为string.Empty。</remarks>
        string GetString(string key);
        /// <summary>
        /// 获取资源文本。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>返回该资源标识对应的文本内容。</returns>
        /// <remarks>资源标识为空、资源标识不存在，均返回为默认值。</remarks>
        string GetString(string key, string defaultValue);

        /// <summary>
        /// 获取资源对象。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        /// <remarks>资源标识为空、资源标识不存在，均返回为null。</remarks>
        object GetObject(string key);
        /// <summary>
        /// 获取资源流。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        /// <remarks>资源标识为空、资源标识不存在，均返回为null。</remarks>
        Stream GetStream(string key);
    }

    /// <summary>
    /// 事件委托：区域名称变更。
    /// </summary>
    /// <param name="sender">事件触发对象。</param>
    /// <param name="oldValue">变更前的值。</param>
    /// <param name="newValue">变更后的值。</param>
    public delegate void LocalizationCultureNameChangedHandler(ILocalizationRenderManager sender, string oldValue, string newValue);
}