/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

/// <summary>
/// Enum扩展类
/// </summary>
public static class EnumExtensions {

    #region methods

    #region HasFlag2
    /// <summary>
    /// 判断current的值是否包含value（逻辑操作： (current &amp; value)==value  ）
    /// </summary>
    /// <param name="current">当前值</param>
    /// <param name="value">判断值</param>
    /// <returns>返回是否包含在内。</returns>
    public static bool HasFlag2(
#if !net20
        this
#endif
        Enum current, Enum value) {
        return HasFlag2(TypeExtensions.Convert<long>(current), TypeExtensions.Convert<long>(value));
    }
    /// <summary>
    /// 判断current的值是否包含value（逻辑操作： (current &amp; value)==value  ）
    /// </summary>
    /// <param name="current">当前值</param>
    /// <param name="value">判断值</param>
    /// <returns>返回是否包含在内。</returns>
    public static bool HasFlag2(
#if !net20
        this
#endif
        Enum current, long value) {
        return HasFlag2(TypeExtensions.Convert<long>(current), value);
    }
    /// <summary>
    /// 判断current的值是否包含value（逻辑操作： (current &amp; value)==value  ）
    /// </summary>
    /// <param name="current">当前值</param>
    /// <param name="value">判断值</param>
    /// <returns>返回是否包含在内。</returns>
    public static bool HasFlag2(
#if !net20
        this
#endif
        long current, Enum value) {
        return HasFlag2(current, TypeExtensions.Convert<long>(value));
    }
    /// <summary>
    /// 判断current的值是否包含value（逻辑操作： (current &amp; value)==value  ）
    /// </summary>
    /// <param name="current">当前值</param>
    /// <param name="value">判断值</param>
    /// <returns>返回是否包含在内。</returns>
    public static bool HasFlag2(
#if !net20
        this
#endif
        long current, long value) {
        return (current & value) == value;
    }
    #endregion

    #region ToValues
    /// <summary>
    /// 将当前枚举的值，变成数组，通常用于多值的枚举。比如将 Abc.A | Abc.B 变成Abc[]{ Abc.A,Abc.B }。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <param name="value">当前枚举值。</param>
    /// <returns>返回一个值的数组。</returns>
    public static T[] ToValues<T>(
#if !net20
        this
#endif
        T value) where T : struct {
        var list = new System.Collections.Generic.List<T>();
        long values = TypeExtensions.Convert<long>(value);
        foreach (T item in Enum.GetValues(typeof(T))) {
            long p = TypeExtensions.Convert<long>(item);
            if ((values & p) == p)
                list.Add(item);
        }
        return list.ToArray();
    }
    #endregion

    #region ToNames
    /// <summary>
    /// 将当前枚举的值，变成名称数组（特性），通常用于多值的枚举。比如将 Abc.A | Abc.B 变成Abc[]{ Abc.A,Abc.B }。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <param name="value">当前枚举值。</param>
    /// <returns>返回一个值的数组。</returns>
    public static string[] ToNames<T>(
#if !net20
        this
#endif
        T value) where T : struct {
        return ToNames(value, false);
    }
    /// <summary>
    /// 将当前枚举的值，变成名称数组，通常用于多值的枚举。比如将 Abc.A | Abc.B 变成Abc[]{ Abc.A,Abc.B }。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <param name="value">当前枚举值。</param>
    /// <param name="defineName">是否为定义名称，为false时表示特性名称。</param>
    /// <returns>返回一个值的数组。</returns>
    public static string[] ToNames<T>(
#if !net20
        this
#endif
        T value, bool defineName) where T : struct {
        var list = new System.Collections.Generic.List<string>();
        long values = TypeExtensions.Convert<long>(value);
        foreach (T item in Enum.GetValues(typeof(T))) {
            long p = TypeExtensions.Convert<long>(item);
            if ((values & p) == p) {
                string name;
                if (defineName || string.IsNullOrEmpty(name = ConstAttributeExtensions.Const(typeof(T).GetField(item.ToString()))))
                    name = item.ToString();
                list.Add(name);
            }
        }
        return list.ToArray();
    }
    #endregion

    #region ToName
    /// <summary>
    /// 将当前枚举的值，变成名称串（特性），通常用于多值的枚举。比如将 Abc.A | Abc.B 变成Abc[]{ Abc.A,Abc.B }。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <param name="value">当前枚举值。</param>
    /// <returns>返回一个值的数组。</returns>
    public static string ToName<T>(
#if !net20
        this
#endif
        T value) where T : struct {
        return ToName(value, false);
    }
    /// <summary>
    /// 将当前枚举的值，变成名称串，通常用于多值的枚举。比如将 Abc.A | Abc.B 变成Abc[]{ Abc.A,Abc.B }。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <param name="value">当前枚举值。</param>
    /// <param name="defineName">是否为定义名称，为false时表示特性名称。</param>
    /// <returns>返回一个值的数组。</returns>
    public static string ToName<T>(
#if !net20
        this
#endif
        T value, bool defineName) where T : struct {
        string text = "";
        long values = TypeExtensions.Convert<long>(value);
        foreach (T item in Enum.GetValues(typeof(T))) {
            long p = TypeExtensions.Convert<long>(item);
            if ((values & p) == p) {
                string name;
                if (defineName || string.IsNullOrEmpty(name = ConstAttributeExtensions.Const(typeof(T).GetField(item.ToString()))))
                    name = item.ToString();
                if (text.Length > 0)
                    text += defineName ? "," : "，";
                text += name;
            }
        }
        return text;
    }
    /// <summary>
    /// 将当前枚举的值，变成名称串，通常用于多值的枚举。比如将 Abc.A | Abc.B 变成Abc[]{ Abc.A,Abc.B }。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <param name="value">当前枚举值。</param>
    /// <param name="key">指定属性名称。</param>
    /// <returns>返回一个值的数组。</returns>
    public static string ToName<T>(
#if !net20
        this
#endif
        T value, string key) where T : struct {
        if (string.IsNullOrEmpty(key))
            key = "Text";

        string text = "";
        long values = TypeExtensions.Convert<long>(value);
        foreach (T item in Enum.GetValues(typeof(T))) {
            long p = TypeExtensions.Convert<long>(item);
            if ((values & p) == p) {
                string name;
                if (string.IsNullOrEmpty(name = ConstAttributeExtensions.Const(typeof(T).GetField(item.ToString()), key)))
                    name = item.ToString();
                if (text.Length > 0)
                    text += key == "Text" ? "，" : ",";
                text += name;
            }
        }
        return text;
    }
    #endregion

    #region GetProperty
    /// <summary>
    /// 获取某一个特定的属性值。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <param name="value">当前枚举值。</param>
    /// <param name="key">属性名称。</param>
    /// <returns>返回属性的值，未找到时，将是string.Empty。</returns>
    public static string GetProperty<T>(
#if !net20
        this
#endif
        T value, string key) where T : struct {

        if (string.IsNullOrEmpty(key))
            key = "Text";

        long values = TypeExtensions.Convert<long>(value);
        foreach (T item in Enum.GetValues(typeof(T))) {
            long p = TypeExtensions.Convert<long>(item);
            if ((values & p) == p) {
                string name;
                if (!string.IsNullOrEmpty(name = ConstAttributeExtensions.Const(typeof(T).GetField(item.ToString()), key)))
                    return name;
            }
        }
        return "";
    }
    #endregion


    #endregion

}