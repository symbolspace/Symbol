/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Reflection;

namespace Symbol {
    /// <summary>
    /// Attribute 扩展类
    /// </summary>
    public static class AttributeExtensions {
        #region fields
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.IList> _list_key = new System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.IList>();
        #endregion

        #region methods

        #region IsDefined
        /// <summary>
        /// 检查是否定义此特性（继承）
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="type">特性类型, instance is type。</param>
        /// <returns></returns>
        public static bool IsDefined(
#if !net20
            this
#endif
            System.Type customAttributeProvider, System.Type type) {
            return GetCustomAttribute(customAttributeProvider, type, true) != null;
        }
        /// <summary>
        /// 检查是否定义此特性
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="type">特性类型, instance is type。</param>
        /// <param name="inherit">如果为 true，则指定还在 element 的祖先中搜索自定义特性。</param>
        /// <returns></returns>
        public static bool IsDefined(
#if !net20
            this
#endif
            System.Type customAttributeProvider, System.Type type, bool inherit) {
            return GetCustomAttribute(customAttributeProvider, type, inherit) != null;
        }
        /// <summary>
        /// 检查是否定义此特性（继承）
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="type">特性类型。</param>
        /// <returns></returns>
        public static bool IsDefined(
#if !net20
            this
#endif
            System.Reflection.ICustomAttributeProvider customAttributeProvider, System.Type type) {
            return GetCustomAttribute(customAttributeProvider, type, true) != null;
        }
        /// <summary>
        /// 检查是否定义此特性
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="type">特性类型, instance is type。</param>
        /// <param name="inherit">如果为 true，则指定还在 element 的祖先中搜索自定义特性。</param>
        /// <returns></returns>
        public static bool IsDefined(
#if !net20
            this
#endif
            System.Reflection.ICustomAttributeProvider customAttributeProvider, System.Type type, bool inherit) {
            return GetCustomAttribute(customAttributeProvider, type, inherit) != null;
        }
        /// <summary>
        /// 检查是否定义此特性（继承）
        /// </summary>
        /// <typeparam name="T">识别instance is T</typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <returns></returns>
        public static bool IsDefined<T>(
#if !net20
            this
#endif
            System.Type customAttributeProvider) where T : System.Attribute {
            return GetCustomAttribute<T>(customAttributeProvider, true) != null;
        }
        /// <summary>
        /// 检查是否定义此特性
        /// </summary>
        /// <typeparam name="T">识别instance is T</typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <param name="inherit">如果为 true，则指定还在 element 的祖先中搜索自定义特性。</param>
        /// <returns></returns>
        public static bool IsDefined<T>(
#if !net20
            this
#endif
            System.Type customAttributeProvider, bool inherit) where T : System.Attribute {
            return GetCustomAttribute<T>(customAttributeProvider, inherit) != null;
        }
        /// <summary>
        /// 检查是否定义此特性（继承）
        /// </summary>
        /// <typeparam name="T">识别instance is T</typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <returns></returns>
        public static bool IsDefined<T>(
#if !net20
            this
#endif
            System.Reflection.ICustomAttributeProvider customAttributeProvider) where T : System.Attribute {
            return GetCustomAttribute<T>(customAttributeProvider, true) != null;
        }
        /// <summary>
        /// 检查是否定义此特性
        /// </summary>
        /// <typeparam name="T">识别instance is T</typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <param name="inherit">如果为 true，则指定还在 element 的祖先中搜索自定义特性。</param>
        /// <returns></returns>
        public static bool IsDefined<T>(
#if !net20
            this
#endif
            System.Reflection.ICustomAttributeProvider customAttributeProvider, bool inherit) where T : System.Attribute {
            return GetCustomAttribute<T>(customAttributeProvider, inherit) != null;
        }
        #endregion

        #region GetCustomAttribute
        /// <summary>
        /// 获取自定义Attribute中的第一个对象（继承）
        /// </summary>
        /// <typeparam name="T">识别instance is T</typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(
#if !net20
            this
#endif
            System.Type customAttributeProvider) where T : System.Attribute {
            return (T)GetCustomAttribute(customAttributeProvider, typeof(T), true);
        }
        /// <summary>
        /// 获取自定义Attribute中的第一个对象
        /// </summary>
        /// <typeparam name="T">识别instance is T</typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <param name="inherit">如果为 true，则指定还在 element 的祖先中搜索自定义特性。</param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(
#if !net20
            this
#endif
            System.Type customAttributeProvider, bool inherit) where T : System.Attribute {
            return (T)GetCustomAttribute(customAttributeProvider, typeof(T), inherit);
        }

        /// <summary>
        /// 获取自定义Attribute中的第一个对象（继承）
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="type">特性类型, instance is type。</param>
        /// <returns></returns>
        public static Attribute GetCustomAttribute(
#if !net20
            this
#endif
            System.Type customAttributeProvider, System.Type type) {
            return GetCustomAttribute((System.Reflection.ICustomAttributeProvider)customAttributeProvider, type, true);
        }
        /// <summary>
        /// 获取自定义Attribute中的第一个对象
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="type">特性类型, instance is type。</param>
        /// <param name="inherit">如果为 true，则指定还在 element 的祖先中搜索自定义特性。</param>
        /// <returns></returns>
        public static Attribute GetCustomAttribute(
#if !net20
            this
#endif
            System.Type customAttributeProvider, System.Type type, bool inherit) {
            return GetCustomAttribute((System.Reflection.ICustomAttributeProvider)customAttributeProvider, type, inherit);
        }
        /// <summary>
        /// 获取自定义Attribute中的第一个对象（继承）
        /// </summary>
        /// <typeparam name="T">识别instance is T</typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(
#if !net20
            this
#endif
            System.Reflection.ICustomAttributeProvider customAttributeProvider) where T : System.Attribute {
            return (T)GetCustomAttribute(customAttributeProvider, typeof(T), true);
        }
        /// <summary>
        /// 获取自定义Attribute中的第一个对象
        /// </summary>
        /// <typeparam name="T">识别instance is T</typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <param name="inherit">如果为 true，则指定还在 element 的祖先中搜索自定义特性。</param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(
#if !net20
            this
#endif
            System.Reflection.ICustomAttributeProvider customAttributeProvider, bool inherit) where T : System.Attribute {
            return (T)GetCustomAttribute(customAttributeProvider, typeof(T), inherit);
        }
        /// <summary>
        /// 获取自定义Attribute中的第一个对象（继承）
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="type">特性类型, instance is type。</param>
        /// <returns></returns>
        public static Attribute GetCustomAttribute(
#if !net20
            this
#endif
            System.Reflection.ICustomAttributeProvider customAttributeProvider, System.Type type) {
            var list = GetCustomAttributes(customAttributeProvider, type, true);
            return list.Count == 0 ? null : (Attribute)list[0];
        }
        /// <summary>
        /// 获取自定义Attribute中的第一个对象
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="type">特性类型, instance is type。</param>
        /// <param name="inherit">如果为 true，则指定还在 element 的祖先中搜索自定义特性。</param>
        /// <returns></returns>
        public static Attribute GetCustomAttribute(
#if !net20
            this
#endif
            System.Reflection.ICustomAttributeProvider customAttributeProvider, System.Type type, bool inherit) {
            var list = GetCustomAttributes(customAttributeProvider, type, inherit);
            return list.Count == 0 ? null : (Attribute)list[0];
        }
        #endregion
        #region GetCustomAttributes
        /// <summary>
        /// 获取自定义Attribute列表（继承）
        /// </summary>
        /// <typeparam name="T">识别instance is T</typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <returns></returns>
        public static System.Collections.Generic.List<T> GetCustomAttributes<T>(
#if !net20
            this
#endif
            System.Type customAttributeProvider) where T : System.Attribute {
            return (System.Collections.Generic.List<T>)GetCustomAttributes(customAttributeProvider, typeof(T), true);
        }
        /// <summary>
        /// 获取自定义Attribute列表
        /// </summary>
        /// <typeparam name="T">识别instance is T</typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <param name="inherit">如果为 true，则指定还在 element 的祖先中搜索自定义特性。</param>
        /// <returns></returns>
        public static System.Collections.Generic.List<T> GetCustomAttributes<T>(
#if !net20
            this
#endif
            System.Type customAttributeProvider, bool inherit) where T : System.Attribute {
            return (System.Collections.Generic.List<T>)GetCustomAttributes(customAttributeProvider, typeof(T), inherit);
        }

        /// <summary>
        /// 获取自定义Attribute列表（继承）
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="type">特性类型, instance is type。</param>
        /// <returns></returns>
        public static System.Collections.IList GetCustomAttributes(
#if !net20
            this
#endif
            System.Type customAttributeProvider, System.Type type) {
            return GetCustomAttributes((System.Reflection.ICustomAttributeProvider)customAttributeProvider, type, true);
        }
        /// <summary>
        /// 获取自定义Attribute列表
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="type">特性类型, instance is type。</param>
        /// <param name="inherit">如果为 true，则指定还在 element 的祖先中搜索自定义特性。</param>
        /// <returns></returns>
        public static System.Collections.IList GetCustomAttributes(
#if !net20
            this
#endif
            System.Type customAttributeProvider, System.Type type, bool inherit) {
            return GetCustomAttributes((System.Reflection.ICustomAttributeProvider)customAttributeProvider, type, inherit);
        }
        /// <summary>
        /// 获取自定义Attribute列表（继承）
        /// </summary>
        /// <typeparam name="T">识别instance is T</typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <returns></returns>
        public static System.Collections.Generic.List<T> GetCustomAttributes<T>(
#if !net20
            this
#endif
            System.Reflection.ICustomAttributeProvider customAttributeProvider) where T : System.Attribute {
            return (System.Collections.Generic.List<T>)GetCustomAttributes(customAttributeProvider, typeof(T), true);
        }
        /// <summary>
        /// 获取自定义Attribute列表
        /// </summary>
        /// <typeparam name="T">识别instance is T</typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <param name="inherit">如果为 true，则指定还在 element 的祖先中搜索自定义特性。</param>
        /// <returns></returns>
        public static System.Collections.Generic.List<T> GetCustomAttributes<T>(
#if !net20
            this
#endif
            System.Reflection.ICustomAttributeProvider customAttributeProvider, bool inherit) where T : System.Attribute {
            return (System.Collections.Generic.List<T>)GetCustomAttributes(customAttributeProvider, typeof(T), inherit);
        }
        /// <summary>
        /// 获取自定义Attribute列表（继承）
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="type">特性类型, instance is type。</param>
        /// <returns></returns>
        public static System.Collections.IList GetCustomAttributes(
#if !net20
            this
#endif
            System.Reflection.ICustomAttributeProvider customAttributeProvider, System.Type type) {
            return GetCustomAttributes(customAttributeProvider, type, true);
        }
        /// <summary>
        /// 获取自定义Attribute列表
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="type">特性类型, instance is type。</param>
        /// <param name="inherit">如果为 true，则指定还在 element 的祖先中搜索自定义特性。</param>
        /// <returns></returns>
        public static System.Collections.IList GetCustomAttributes(
#if !net20
            this
#endif
            System.Reflection.ICustomAttributeProvider customAttributeProvider, System.Type type, bool inherit) {
            if (customAttributeProvider == null || type == null)
                return CreateList(type);

            System.Collections.IList list;
            string key = string.Concat(GetKeyBefore(customAttributeProvider), "_", type.AssemblyQualifiedName);

            if (!_list_key.TryGetValue(key, out list)) {
                ThreadHelper.Block(_list_key, () => {
                    if (!_list_key.TryGetValue(key, out list)) {
                        list = CreateList(type);
                        foreach (var item in customAttributeProvider.GetCustomAttributes(inherit)) {
                            if (item.GetType() == type || TypeExtensions.IsInheritFrom(item.GetType(), type))
                                list.Add(item);
                        }
                        _list_key.TryAdd(key, list);
                    }
                });
            }

            return list;
        }
        static object GetKeyBefore(System.Reflection.ICustomAttributeProvider customAttributeProvider) {
            {
                var value = customAttributeProvider as System.Type;
                if (value != null)
                    return value.AssemblyQualifiedName;
            }
            {
                var value = customAttributeProvider as System.Reflection.MemberInfo;
                if (value != null)
                    return value.DeclaringType.AssemblyQualifiedName + "/" + value.Name;
            }
            return customAttributeProvider.GetHashCode();

        }
        static System.Collections.IList CreateList(System.Type type) {
            return (System.Collections.IList)FastWrapper.CreateInstance(typeof(System.Collections.Generic.List<>).MakeGenericType(type));
        }
        #endregion

        #endregion
    }

}