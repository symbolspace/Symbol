using Symbol;
using Symbol.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

[assembly: TypeImplementMap(typeof(ILocalizationRenderManager), typeof(LocalizationRenderManager), "System")]

namespace Symbol.Localization
{
    /// <summary>
    /// 实现：本地化渲染器管理。
    /// </summary>
    public class LocalizationRenderManager : ILocalizationRenderManager
    {
        private ICultureResourceManager _cultureResourceManager;
        private string _cultureName;
        private HashSet<object> _list_element;
#if NET35
        private IDictionary<Type, HashSet<ILocalizationRender>> _list_render;
#else
        private System.Collections.Concurrent.ConcurrentDictionary<Type, HashSet<ILocalizationRender>> _list_render;
#endif

        /// <summary>
        /// 创建对象实例。
        /// </summary>
        public LocalizationRenderManager()
            :this(null)
        {

        }
        /// <summary>
        /// 创建对象实例。
        /// </summary>
        /// <param name="cultureResourceManager">区域资源管理对象。</param>
        public LocalizationRenderManager(ICultureResourceManager cultureResourceManager)
        {
            _cultureResourceManager = cultureResourceManager ?? TypeImplementMap.GetTargetSingleton<ICultureResourceManager>();
            _cultureName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            _list_element = new HashSet<object>();
#if NET35
            _list_render= new Dictionary<Type, HashSet<ILocalizationRender>>();
#else
            _list_render = new System.Collections.Concurrent.ConcurrentDictionary<Type, HashSet<ILocalizationRender>>();
#endif
            Scan();
            ScanPlugins();
        }

        /// <summary>
        /// 获取全局对象。
        /// </summary>
        public static ILocalizationRenderManager Instance { get { return TypeImplementMap.GetTargetSingleton<ILocalizationRenderManager>(); } }

        /// <summary>
        /// 获取区域资源管理对象。
        /// </summary>
        public ICultureResourceManager CultureResourceManager { get { return _cultureResourceManager; } }
        /// <summary>
        /// 获取元素数量。
        /// </summary>
        public int ElementCount { get { return _list_element.Count; } }

        /// <summary>
        /// 获取或设置当前区域名称。
        /// </summary>
        /// <remarks>不接受空值设置。成功设置后自动应用到已注册的渲染器。</remarks>
        public virtual string CultureName
        {
            get { return _cultureName; }
            set
            {
                if (string.IsNullOrEmpty(value) || _cultureName == value)
                    return;
                var oldValue = _cultureName;
                _cultureName = value;

                RenderAll();
                CultureNameChanged?.Invoke(this, oldValue, value);
            }
        }

        /// <summary>
        /// 事件：区域名称变更。
        /// </summary>
        public event LocalizationCultureNameChangedHandler CultureNameChanged;

        /// <summary>
        /// 添加渲染器。
        /// </summary>
        /// <param name="render">渲染器对象。</param>
        /// <param name="elementType">元素类型。</param>
        public virtual void AddRender(ILocalizationRender render, Type elementType)
        {
            Throw.CheckArgumentNull(render, nameof(render));
            Throw.CheckArgumentNull(elementType, nameof(elementType));

            HashSet<ILocalizationRender> list;
#if NET35
            if (!_list_render.TryGetValue(elementType, out list))
            {
                lock (_list_render)
                {
                    if (!_list_render.TryGetValue(elementType, out list))
                    {
                        list = new HashSet<ILocalizationRender>();
                        _list_render.Add(elementType, list);
                    }
                }
            }
#else
            list = _list_render.GetOrAdd(elementType, (key) => new HashSet<ILocalizationRender>());
#endif
            lock (list)
            {
                list.Add(render);
            }
        }
        /// <summary>
        /// 添加渲染器。
        /// </summary>
        /// <param name="localizationRenderAttribute">渲染器特性。</param>
        public virtual void AddRender(LocalizationRenderAttribute localizationRenderAttribute)
        {
            Throw.CheckArgumentNull(localizationRenderAttribute, nameof(localizationRenderAttribute));
            AddRender(FastObject.CreateInstance<ILocalizationRender>(localizationRenderAttribute.RenderType), localizationRenderAttribute.ElementType);
        }

        void ScanPlugins()
        {
            foreach (var file in Symbol.IO.FileHelper.Scan("*.Localization.*.dll"))
            {
                Scan(AssemblyLoader.Load(file));
            }
        }
        /// <summary>
        /// 扫描资源。
        /// </summary>
        /// <remarks>从当前已加载的程序集中扫描。</remarks>
        public virtual void Scan()
        {
            Scan(AssemblyLoader.GetAssemblies());
        }
        /// <summary>
        /// 扫描资源。
        /// </summary>
        /// <param name="assemblies">待扫描的程序集清单，为空不会报错。</param>
        public virtual void Scan(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
                return;
            foreach (Assembly assembly in assemblies)
            {
                Scan(assembly);
            }
        }
        /// <summary>
        /// 扫描资源。
        /// </summary>
        /// <param name="assembly">包含区域资源特性的程序集。e.g [assembly: LocalizationRender( typeof(XXXControl), typeof(XXXXRender))]。</param>
        public virtual void Scan(Assembly assembly)
        {
            if (assembly == null)
                return;
            var attributes = AttributeExtensions.GetCustomAttributes<LocalizationRenderAttribute>(assembly, true);
            foreach (var attribute in attributes)
            {
                AddRender(attribute);
            }
        }

        /// <summary>
        /// 添加元素。
        /// </summary>
        /// <param name="element">目标元素。</param>
        public virtual void AddElement(object element)
        {
            Throw.CheckArgumentNull(element, nameof(element));
            lock (_list_element)
            {
                _list_element.Add(element);
                var elementType = element.GetType();
                foreach (Type type in _list_render.Keys)
                {
                    if (!TypeExtensions.IsInheritFrom(elementType, type))
                        continue;
                    var list = _list_render[type];
                    foreach (var render in list)
                    {
                        BindLocalization(render, element);
                    }
                }
            }
        }
        /// <summary>
        /// 本地化绑定。
        /// </summary>
        /// <param name="render">渲染器对象。</param>
        /// <param name="element">目标元素。</param>
        protected virtual void BindLocalization(ILocalizationRender render, object element)
        {
            try
            {
                render.BindLocalization(this, element);
            }
            catch (ObjectDisposedException)
            {

            }
        }


        /// <summary>
        /// 移除元素。
        /// </summary>
        /// <param name="element">目标元素。</param>
        public virtual void RemoveElement(object element)
        {
            Throw.CheckArgumentNull(element, nameof(element));
            lock (_list_element)
            {
                _list_element.Remove(element);
            }
        }


        /// <summary>
        /// 渲染所有元素。
        /// </summary>
        public virtual void RenderAll()
        {
            foreach(var element in _list_element)
            {
                Render(element);
            }
        }
        /// <summary>
        /// 渲染元素。
        /// </summary>
        /// <param name="element">目标元素。</param>
        public virtual void Render(object element)
        {
            Render(element, null);
        }
        /// <summary>
        /// 渲染元素。
        /// </summary>
        /// <param name="element">目标元素。</param>
        /// <param name="rootElement">根元素，定义或容纳当前元素的对象。</param>
        public virtual void Render(object element, object rootElement)
        {
            Throw.CheckArgumentNull(element, nameof(element));
            var elementType = element.GetType();
            foreach(Type type in _list_render.Keys)
            {
                if (!TypeExtensions.IsInheritFrom(elementType, type))
                    continue;
                var list = _list_render[type];
                foreach(var render in list)
                {
                    RenderLocalization(render, rootElement, element);
                }
            }
        }
        /// <summary>
        /// 本地化渲染。
        /// </summary>
        /// <param name="render">渲染器对象。</param>
        /// <param name="element">目标元素。</param>
        /// <param name="rootElement">根元素，定义或容纳当前元素的对象。</param>
        protected virtual void RenderLocalization(ILocalizationRender render, object element, object rootElement)
        {
            try
            {
                render.RenderLocalization(this, rootElement, element);
            }
            catch (ObjectDisposedException)
            {

            }
        }
        /// <summary>
        /// 获取资源文本。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的文本内容。</returns>
        /// <remarks>资源标识为空、资源标识不存在，均返回为string.Empty。</remarks>
        public virtual string GetString(string key)
        {
            return _cultureResourceManager.GetString(key, _cultureName);
        }
        /// <summary>
        /// 获取资源文本。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>返回该资源标识对应的文本内容。</returns>
        /// <remarks>资源标识为空、资源标识不存在，均返回为默认值。</remarks>
        public virtual string GetString(string key, string defaultValue)
        {
            string value = _cultureResourceManager.GetString(key, _cultureName);
            if (string.IsNullOrEmpty(value))
                value = defaultValue;
            return value;
        }

        /// <summary>
        /// 获取资源对象。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        /// <remarks>资源标识为空、资源标识不存在，均返回为null。</remarks>
        public virtual object GetObject(string key)
        {
            return _cultureResourceManager.GetObject(key, _cultureName);
        }
        /// <summary>
        /// 获取资源流。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        /// <remarks>资源标识为空、资源标识不存在，均返回为null。</remarks>
        public virtual Stream GetStream(string key)
        {
            return _cultureResourceManager.GetStream(key, _cultureName);
        }
    }

}