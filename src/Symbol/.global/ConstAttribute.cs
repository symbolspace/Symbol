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

    #region GetParameter
    static ParameterInfo GetParameter(MethodInfo methodInfo, string parameterName) {
        var parameters = methodInfo.GetParameters();
        var parameter = LinqHelper.FirstOrDefault(parameters, p => p.Name == parameterName);
        if (parameter == null) {
            parameter = LinqHelper.FirstOrDefault(parameters, p => string.Equals(p.Name, parameterName, StringComparison.OrdinalIgnoreCase));
        }
        return parameter;
    }
    #endregion

    #region GetValues

    static Symbol.Collections.Generic.NameValueCollection<string> GetValues(ICustomAttributeProvider provider) {
        Func<ICustomAttributeProvider, Symbol.Collections.Generic.NameValueCollection<string>> func = (p) => {
            var list = new Symbol.Collections.Generic.NameValueCollection<string>(StringComparer.OrdinalIgnoreCase);
            GetValues(provider, list, true);
            return list;
        };
        return (provider is ParameterInfo) ? func(provider) : _list.GetOrAdd(provider, func);
    }
    static Symbol.Collections.Generic.NameValueCollection<string> GetValues(MethodInfo methodInfo, ParameterInfo parameter) {
        return _list.GetOrAdd(methodInfo, (p) => {
            var list = new Symbol.Collections.Generic.NameValueCollection<string>(StringComparer.OrdinalIgnoreCase);
            GetValues(methodInfo, parameter, list);
            return list;
        });
    }
    static void GetValues(MethodInfo methodInfo, ParameterInfo parameter, Symbol.Collections.Generic.NameValueCollection<string> list) {
        if (methodInfo.DeclaringType == null)
            return;
        foreach (Type interfaceType in methodInfo.DeclaringType.GetInterfaces()) {
            MemberInfo[] members = interfaceType.GetMember(methodInfo.Name);
            if (members == null || members.Length == 0)
                continue;
            foreach (MemberInfo member in members) {
                MethodInfo interfaceMethod = member as MethodInfo;
                if (interfaceMethod == null)
                    continue;
                var interfaceMethodParameter = GetParameter(interfaceMethod, parameter.Name);
                if (interfaceMethodParameter == null)
                    continue;
                GetValues(interfaceMethodParameter, list, false);
            }
        }
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

    #region Const Parameter

    /// <summary>
    /// 获取常量标识值，常量名称为：Text
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <param name="parameterName">参数名称，优先区分大小写，其次不区分；null或empty将返回methodInfo本身的常量值</param>
    /// <returns></returns>
    public static string ConstParameter(
#if !net20
        this
#endif
        MethodInfo methodInfo, string parameterName) {
        return ConstParameter(methodInfo, parameterName, "Text");
    }
    /// <summary>
    /// 获取常量标识值
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <param name="parameterName">参数名称，优先区分大小写，其次不区分；null或empty将返回methodInfo本身的常量值</param>
    /// <param name="key">常量名称</param>
    /// <returns></returns>
    public static string ConstParameter(
#if !net20
        this
#endif
        MethodInfo methodInfo, string parameterName, string key) {
        if (methodInfo == null)
            return "";
        if (string.IsNullOrEmpty(parameterName)) {
            return Const((ICustomAttributeProvider)methodInfo, key);
        }
        var parameter = GetParameter(methodInfo, parameterName);
        if (parameter == null)
            return "";
        return GetValues(methodInfo, parameter)[string.IsNullOrEmpty(key) ? "Text" : key] ?? "";
    }
    #endregion
    #region Const Assembly/TypeInfo/MethodInfo/PropertyInfo/FieldInfo/EventInfo/ICustomAttributeProvider
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
        if (provider == null)
            return "";
        var list = GetValues(provider);
        return list[string.IsNullOrEmpty(key) ? "Text" : key] ?? "";
    }
    #endregion
    #region Const instance/ICustomAttributeProvider
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
        if (instance == null)
            return "";
        {
            var methodInfo = instance as MethodInfo;
            if (methodInfo != null) {
                return ConstParameter(methodInfo, memberName, key);
            }
        }
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
        if (provider == null)
            return "";
        return GetValues(provider)[string.IsNullOrEmpty(key) ? "Text" : key] ?? "";
    }
    #endregion

    #endregion
}