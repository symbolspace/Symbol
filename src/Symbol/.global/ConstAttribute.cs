/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Reflection;

/// <summary>
/// 用于在设置常量的特性
/// </summary>
[System.Diagnostics.DebuggerDisplay("Key = {Key}, Value={Value}")]
[Serializable]
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class ConstAttribute : Attribute {


    #region properties
    /// <summary>
    /// 常量名称
    /// </summary>
    public string Key { get; protected set; }
    /// <summary>
    /// 常量值
    /// </summary>
    public string Value { get; protected set; }
    #endregion

    #region ctor
    /// <summary>
    /// 标识一个常量，常量名称为：Text
    /// </summary>
    /// <param name="value">常量值</param>
    public ConstAttribute(string value)
        : this("Text", value) {
    }
    /// <summary>
    /// 标识一个常量
    /// </summary>
    /// <param name="key">常量名称</param>
    /// <param name="value">常量值</param>
    public ConstAttribute(string key, string value) {
        Key = string.IsNullOrEmpty(key) ? "Text" : key;
        Value = value;
    }
    #endregion
}

/// <summary>
/// ConstAttribute的扩展类
/// </summary>
public static class ConstAttributeExtensions {

    #region fields
    private static readonly ConstAttribute[] _empty = new ConstAttribute[0];
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<ICustomAttributeProvider, Symbol.Collections.Generic.NameValueCollection<string>> _list = new System.Collections.Concurrent.ConcurrentDictionary<ICustomAttributeProvider, Symbol.Collections.Generic.NameValueCollection<string>>();

    #endregion

    #region methods

    #region GetValues

    static Symbol.Collections.Generic.NameValueCollection<string> GetValues(ICustomAttributeProvider provider) {
        Symbol.Collections.Generic.NameValueCollection<string> list;
        if (!_list.TryGetValue(provider, out list)) {
            ThreadHelper.Block(_list, () => {
                if (!_list.TryGetValue(provider, out list)) {
                    list = new Symbol.Collections.Generic.NameValueCollection<string>(StringComparer.OrdinalIgnoreCase);
                    GetValues(provider, list, true);
                    _list.TryAdd(provider, list);
                }
            });
        }
        return list;
    }
    static void GetValues(ICustomAttributeProvider provider, Symbol.Collections.Generic.NameValueCollection<string> list, bool inherit) {
        if (inherit) {
            {
                Type type = provider as Type;
                if (type != null) {
                    foreach (Type p in type.GetInterfaces()) {
                        GetValues(p, list, false);
                    }
                }
            }
            {
                MemberInfo member = provider as MemberInfo;
                if (member != null && member.DeclaringType != null) {
                    foreach (Type p in member.DeclaringType.GetInterfaces()) {
                        MemberInfo[] members = p.GetMember(member.Name);
                        if (members == null || members.Length == 0)
                            continue;
                        foreach (MemberInfo m in members) {
                            GetValues(m, list, false);
                        }
                    }
                }
            }
        }
        {
            var attributes = provider.GetCustomAttributes(typeof(ConstAttribute), true);
            if (attributes != null) {
                foreach (ConstAttribute p in attributes) {
                    list[p.Key] = p.Value;
                }
            }
        }
    }
    #endregion

    #region Const
    /// <summary>
    /// 获取常量标识值，常量名称为：Text
    /// </summary>
    /// <param name="provider">可获取特性的对象</param>
    /// <returns>返回此常量名称对应的值，如果不存在，返回 string.Empty</returns>
    public static string Const(
#if !net20
        this
#endif
        ICustomAttributeProvider provider) {
        return Const(provider, "Text");
    }
    /// <summary>
    /// 获取常量标识值
    /// </summary>
    /// <param name="provider">可获取特性的对象</param>
    /// <param name="key">常量名称</param>
    /// <returns>返回此常量名称对应的值，如果不存在，返回 string.Empty</returns>
    public static string Const(
#if !net20
        this
#endif
        ICustomAttributeProvider provider, string key) {
        var list = GetValues(provider);
        return list[string.IsNullOrEmpty(key) ? "Text" : key] ?? "";
    }

    /// <summary>
    /// 获取常量标识值
    /// </summary>
    /// <param name="instance">包含特性的实例</param>
    /// <returns>返回此常量名称对应的值，如果不存在，返回 string.Empty</returns>
    public static string Const(
#if !net20
        this
#endif
        object instance) {
        return Const(instance, null, null);
    }
    /// <summary>
    /// 获取常量标识值
    /// </summary>
    /// <param name="instance">包含特性的实例</param>
    /// <param name="keyOrMemberName">常量名称或成员名称</param>
    /// <returns>返回此常量名称对应的值，如果不存在，返回 string.Empty</returns>
    public static string Const(
#if !net20
        this
#endif
        object instance, string keyOrMemberName) {
        return Const(instance, keyOrMemberName, null);
    }
    /// <summary>
    /// 获取常量标识值
    /// </summary>
    /// <param name="instance">包含特性的实例</param>
    /// <param name="memberName">成员名称</param>
    /// <param name="key">常量名称</param>
    /// <returns>返回此常量名称对应的值，如果不存在，返回 string.Empty</returns>
    public static string Const(
#if !net20
        this
#endif
        object instance, string memberName, string key) {
        var provider = instance as ICustomAttributeProvider;
        if (provider == null) {
            var type = instance.GetType();
            if (string.IsNullOrEmpty(memberName)) {
                provider = type;
            } else {
                var members = type.GetMember(memberName);
                if (members == null || members.Length == 0) {
                    provider = type;
                    if (string.IsNullOrEmpty(key))
                        key = memberName;
                } else {
                    provider = members[0];
                }
            }
        }
        var list = GetValues(provider);
        return list[string.IsNullOrEmpty(key) ? "Text" : key] ?? "";
    }
    #endregion




    #endregion
}