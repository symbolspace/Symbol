using Symbol;
using Symbol.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;

[assembly: TypeImplementMap(typeof(ICultureResourceManager), typeof(CultureResourceManager), "System")]

namespace Symbol.Localization {
    /// <summary>
    /// 实现：区域资源管理器。
    /// </summary>
    public class CultureResourceManager : ICultureResourceManager {
#if NET35
        private IDictionary<string, HashSet<IResourceManager>> _list_culture;
#else
        private System.Collections.Concurrent.ConcurrentDictionary<string, HashSet<IResourceManager>> _list_culture;
#endif

        /// <summary>
        /// 创建对象实例。
        /// </summary>
        public CultureResourceManager() {
#if NET35
            _list_culture= new Dictionary<string, HashSet<IResourceManager>>(StringComparer.OrdinalIgnoreCase);
#else
            _list_culture = new System.Collections.Concurrent.ConcurrentDictionary<string, HashSet<IResourceManager>>(StringComparer.OrdinalIgnoreCase);
#endif
            Scan();
        }
        /// <summary>
        /// 获取全局对象。
        /// </summary>
        public static ICultureResourceManager Instance { get { return TypeImplementMap.GetTargetSingleton<ICultureResourceManager>(); } }

        /// <summary>
        /// 获取区域数量。
        /// </summary>
        public int Count { get { return _list_culture.Count; } }

        /// <summary>
        /// 获取区域名称清单。
        /// </summary>
        public string[] Names { get { return LinqHelper.ToArray(_list_culture.Keys); } }

        /// <summary>
        /// 获取区域清单。
        /// </summary>
        public IEnumerable<CultureInfo> Cultures {
            get {
                foreach (var key in _list_culture.Keys) {
                    yield return new CultureInfo() {
                        Name = key,
                        Text = GetString(string.Format("CultureName.{0}", key), key)
                    };
                }
            }
        }

        /// <summary>
        /// 添加资源。
        /// </summary>
        /// <param name="resourceManager">资源管理对象。</param>
        /// <remarks>区域名称为 System.Threading.Thread.CurrentThread.CurrentUICulture.Name。</remarks>
        public virtual void AddResource(IResourceManager resourceManager) {
            AddResource(resourceManager, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
        }
        /// <summary>
        /// 添加资源。
        /// </summary>
        /// <param name="resourceManager">资源管理对象。</param>
        /// <param name="cultureName">区域名称，e.g zh-CN。</param>
        public virtual void AddResource(IResourceManager resourceManager, string cultureName) {
            Throw.CheckArgumentNull(resourceManager, nameof(resourceManager));
            Throw.CheckArgumentNull(cultureName, nameof(cultureName));

            HashSet<IResourceManager> list;
#if NET35
            if(!_list_culture.TryGetValue(cultureName, out list))
            {
                lock (_list_culture)
                {
                    if (!_list_culture.TryGetValue(cultureName, out list))
                    {
                        list = new HashSet<IResourceManager>();
                        _list_culture.Add(cultureName, list);
                    }
                }
            }
#else
            list = _list_culture.GetOrAdd(cultureName, (key) => new HashSet<IResourceManager>());
#endif
            lock (list) {
                list.Add(resourceManager);
            }
        }
        /// <summary>
        /// 添加资源。
        /// </summary>
        /// <param name="resourceManager">资源管理对象。</param>
        /// <remarks>区域名称为 System.Threading.Thread.CurrentThread.CurrentUICulture.Name。</remarks>
        public virtual void AddResource(ResourceManager resourceManager) {
            AddResource(resourceManager, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
        }
        /// <summary>
        /// 添加资源。
        /// </summary>
        /// <param name="resourceManager">资源管理对象。</param>
        /// <param name="cultureName">区域名称，e.g zh-CN。</param>
        public virtual void AddResource(ResourceManager resourceManager, string cultureName) {
            Throw.CheckArgumentNull(resourceManager, nameof(resourceManager));
            AddResource(new SystemResourceManager(resourceManager), cultureName);
        }
        /// <summary>
        /// 添加资源。
        /// </summary>
        /// <param name="cultureResourceAttribute">区域资源特性。</param>
        public virtual void AddResource(CultureResourceAttribute cultureResourceAttribute) {
            Throw.CheckArgumentNull(cultureResourceAttribute, nameof(cultureResourceAttribute));
            AddResource(cultureResourceAttribute.ResourceManager, cultureResourceAttribute.CultureName);
        }
        /// <summary>
        /// 扫描资源。
        /// </summary>
        /// <remarks>从当前已加载的程序集中扫描。</remarks>
        public virtual void Scan() {
            Scan(AssemblyLoader.GetAssemblies());
        }
        /// <summary>
        /// 扫描资源。
        /// </summary>
        /// <param name="assemblies">待扫描的程序集清单，为空不会报错。</param>
        public virtual void Scan(IEnumerable<Assembly> assemblies) {
            if (assemblies == null)
                return;
            foreach (Assembly assembly in assemblies) {
                Scan(assembly);
            }
        }
        /// <summary>
        /// 扫描资源。
        /// </summary>
        /// <param name="assembly">包含区域资源特性的程序集。e.g [assembly: CultureResource("zh-CN", typeof(Resources.zh_CN))]。</param>
        public virtual void Scan(Assembly assembly) {
            if (assembly == null)
                return;
            var attributes = AttributeExtensions.GetCustomAttributes<CultureResourceAttribute>(assembly, true);
            foreach (var attribute in attributes) {
                AddResource(attribute);
            }
        }


        /// <summary>
        /// 获取资源清单。
        /// </summary>
        /// <returns>返回该区域的所有资源。</returns>
        /// <remarks>区域名称为 System.Threading.Thread.CurrentThread.CurrentUICulture.Name，区域为空、区域不存在，均返回null。</remarks>
        public virtual IEnumerable<IResourceManager> GetResources() {
            return GetResources(System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
        }
        /// <summary>
        /// 获取资源清单。
        /// </summary>
        /// <param name="cultureName">区域名称，e.g zh-CN。</param>
        /// <returns>返回该区域的所有资源。</returns>
        /// <remarks>区域为空、区域不存在，均返回null。</remarks>
        public virtual IEnumerable<IResourceManager> GetResources(string cultureName) {
            return IDictionaryExtensions.GetValue(_list_culture, cultureName);
        }


        /// <summary>
        /// 获取资源文本（当前区域）。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的文本内容。</returns>
        /// <remarks>区域名称为 System.Threading.Thread.CurrentThread.CurrentUICulture.Name，资源标识为空、资源标识不存在，均返回为string.Empty。</remarks>
        public virtual string GetString(string key) {
            return GetString(key, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
        }
        /// <summary>
        /// 获取资源文本。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <param name="cultureName">区域名称，e.g zh-CN</param>
        /// <returns>返回该资源标识对应的文本内容。</returns>
        /// <remarks>资源标识为空、区域名称为空、资源标识不存在，均返回为string.Empty。</remarks>
        public virtual string GetString(string key, string cultureName) {
            if (string.IsNullOrEmpty(key))
                return string.Empty;
            var list = GetResources(cultureName);
            if (list == null)
                return string.Empty;
            foreach (var resouceManager in list) {
                try {
                    string value = resouceManager.GetString(key);
                    if (string.IsNullOrEmpty(value))
                        continue;
                    return value;
                } catch (MissingManifestResourceException) {
                    continue;
                } catch (InvalidOperationException) {
                    continue;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取资源对象。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        /// <remarks>区域名称为 System.Threading.Thread.CurrentThread.CurrentUICulture.Name，资源标识为空、区域名称为空、资源标识不存在，均返回为null。</remarks>
        public virtual object GetObject(string key) {
            return GetObject(key, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
        }
        /// <summary>
        /// 获取资源对象。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <param name="cultureName">区域名称，e.g zh-CN</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        /// <remarks>资源标识为空、区域名称为空、资源标识不存在，均返回为null。</remarks>
        public virtual object GetObject(string key, string cultureName) {
            if (string.IsNullOrEmpty(key))
                return null;
            var list = GetResources(cultureName);
            if (list == null)
                return null;
            foreach (var resouceManager in list) {
                try {
                    var value = resouceManager.GetObject(key);
                    if (value == null)
                        continue;
                    return value;
                } catch (MissingManifestResourceException) {
                    continue;
                } catch (InvalidOperationException) {
                    continue;
                }
            }
            return null;
        }


        /// <summary>
        /// 获取资源流。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        /// <remarks>区域名称为 System.Threading.Thread.CurrentThread.CurrentUICulture.Name，资源标识为空、区域名称为空、资源标识不存在，均返回为null。</remarks>
        public virtual Stream GetStream(string key) {
            return GetStream(key, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
        }
        /// <summary>
        /// 获取资源流。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <param name="cultureName">区域名称，e.g zh-CN</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        /// <remarks>资源标识为空、区域名称为空、资源标识不存在，均返回为null。</remarks>
        public virtual Stream GetStream(string key, string cultureName) {
            if (string.IsNullOrEmpty(key))
                return null;
            var list = GetResources(cultureName);
            if (list == null)
                return null;
            foreach (var resouceManager in list) {
                try {
                    var value = resouceManager.GetStream(key);
                    if (value == null)
                        continue;
                    return value;
                } catch (MissingManifestResourceException) {
                    continue;
                } catch (InvalidOperationException) {
                    continue;
                }
            }
            return null;
        }
    }

}