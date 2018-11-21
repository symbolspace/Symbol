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

    #region fields
    private static readonly string _textKey = "Text";
    #endregion

    #region properties
    /// <summary>
    /// 常量名称
    /// </summary>
    public string Key { get; set; }
    /// <summary>
    /// 常量值
    /// </summary>
    public string Value { get; set; }
    #endregion

    #region ctor
    /// <summary>
    /// 标识一个常量，常量名称为：Text
    /// </summary>
    /// <param name="value">常量值</param>
    public ConstAttribute(string value)
        : this(_textKey, value) {
    }
    /// <summary>
    /// 标识一个常量
    /// </summary>
    /// <param name="key">常量名称</param>
    /// <param name="value">常量值</param>
    public ConstAttribute(string key, string value) {
        Key = key;
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
    private static readonly string _textKey = "Text";
    #endregion

    #region methods

    #region GetAttributes
    private static ConstAttribute[] GetAttributes(ICustomAttributeProvider provider) {

        ConstAttribute[] result = (ConstAttribute[])provider.GetCustomAttributes(typeof(ConstAttribute), true);
        if (result != null && result.Length > 0)
            return result;
        Type type = provider as Type;
        if (type != null) {
#if NETDNX
            if(type.GetTypeInfo().IsInterface)
                return _empty;
#else
            if (type.IsInterface)
                return _empty;
#endif
            foreach (Type @interface in type.GetInterfaces()) {
#if NETDNX
                result = GetAttributes(@interface.GetTypeInfo());
#else
                result = GetAttributes(@interface);
#endif
                if (@result.Length > 0)
                    return @result;
            }
        }

        MemberInfo member = provider as MemberInfo;
        if (member == null)
            return _empty;
        if (member.DeclaringType != null) {
            foreach (Type @interface in member.DeclaringType.GetInterfaces()) {
                MemberInfo[] members = @interface.GetMember(member.Name);
                if (members == null || members.Length == 0)
                    continue;
                foreach (MemberInfo m in members) {
                    result = (ConstAttribute[])m.GetCustomAttributes(typeof(ConstAttribute), true);
                    if (result != null && result.Length > 0)
                        return result;
                }
            }
        }

        return _empty;

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
        return Const(provider, _textKey);
    }
    /// <summary>
    /// 获取常量标识值
    /// </summary>
    /// <param name="provider">可获取特性的对象</param>
    /// <param name="key">常名称</param>
    /// <returns>返回此常量名称对应的值，如果不存在，返回 string.Empty</returns>
    public static string Const(
#if !net20
        this 
#endif
        ICustomAttributeProvider provider, string key) {
        foreach (ConstAttribute item in GetAttributes(provider)) {
            if (item.Key == key)
                return item.Value;
        }
        return string.Empty;
    }
    #endregion
#if netcore
    #region Const
    /// <summary>
    /// 获取常量标识值，常量名称为：Text
    /// </summary>
    /// <param name="provider">可获取特性的对象</param>
    /// <returns>返回此常量名称对应的值，如果不存在，返回 string.Empty</returns>
    public static string Const(
        this 
        System.Type provider) {
        return Const(provider, _textKey);
    }
    /// <summary>
    /// 获取常量标识值
    /// </summary>
    /// <param name="provider">可获取特性的对象</param>
    /// <param name="key">常名称</param>
    /// <returns>返回此常量名称对应的值，如果不存在，返回 string.Empty</returns>
    public static string Const(
        this 
        System.Type provider, string key) {
        foreach (ConstAttribute item in GetAttributes(provider.GetTypeInfo())) {
            if (item.Key == key)
                return item.Value;
        }
        return string.Empty;
    }
    #endregion
#endif

    #endregion
}