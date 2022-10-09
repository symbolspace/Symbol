/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

/// <summary>
/// Enum扩展类
/// </summary>
public static class EnumExtensions {


    #region fields
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Symbol.Collections.Generic.NameValueCollection<object>> _list_define;
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> _list_define_json;
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.Generic.List<object>> _list_define_object;
    #endregion

    #region cctor
    static EnumExtensions() {
        _list_define = new System.Collections.Concurrent.ConcurrentDictionary<string, Symbol.Collections.Generic.NameValueCollection<object>>();
        _list_define_json = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
        _list_define_object = new System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.Generic.List<object>>();
    }
    #endregion

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
        T value) where T:Enum {
        string[] values = value.ToString().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
        return TypeExtensions.Convert<T[]>(values);
    }
    #endregion

    #region ToNames
    /// <summary>
    /// 将当前枚举的值，变成名称数组（特性），通常用于多值的枚举。比如将 Abc.A | Abc.B 变成Abc[]{ Abc.A,Abc.B }。
    /// </summary>
    /// <param name="value">当前枚举值。</param>
    /// <returns>返回一个值的数组。</returns>
    public static string[] ToNames(
#if !net20
        this
#endif
        Enum value) {
        return ToNames(value, false);
    }
    /// <summary>
    /// 将当前枚举的值，变成名称数组，通常用于多值的枚举。比如将 Abc.A | Abc.B 变成Abc[]{ Abc.A,Abc.B }。
    /// </summary>
    /// <param name="value">当前枚举值。</param>
    /// <param name="defineName">是否为定义名称，为false时表示特性名称。</param>
    /// <returns>返回一个值的数组。</returns>
    public static string[] ToNames(
#if !net20
        this
#endif
        Enum value, bool defineName) {
        string[] values = value.ToString().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
        if (!defineName) {
            var type = value.GetType();
            for (int i = 0; i < values.Length; i++) {
                string name = ToName_Field(type.GetField(values[i]), "Text");
                if (!string.IsNullOrEmpty(name))
                    values[i] = name;
            }
        }
        return values;
    }
    #endregion

    #region ToName

    static string ToName_Field(System.Reflection.FieldInfo fieldInfo, string key) {

        var value = ConstAttributeExtensions.Const(fieldInfo, key);
        if (!string.IsNullOrEmpty(value))
            return value;
        if (key == "Text") {
            value = Symbol.AttributeExtensions.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>(fieldInfo)?.DisplayName;
            if (string.IsNullOrEmpty(value)) {
                key = "Description";
                goto lb_Description;
            }
        }
    lb_Description:
        if (key == "Description") {
            value = Symbol.AttributeExtensions.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>(fieldInfo)?.Description;
        }
        return value;
    }

    /// <summary>
    /// 将当前枚举的值，变成名称串（特性），通常用于多值的枚举。比如将 Abc.A | Abc.B 变成Abc[]{ Abc.A,Abc.B }。
    /// </summary>
    /// <param name="value">当前枚举值。</param>
    /// <returns>返回一个值的数组。</returns>
    public static string ToName(
#if !net20
        this
#endif
        Enum value) {
        return ToName(value, false);
    }
    /// <summary>
    /// 将当前枚举的值，变成名称串，通常用于多值的枚举。比如将 Abc.A | Abc.B 变成Abc[]{ Abc.A,Abc.B }。
    /// </summary>
    /// <param name="value">当前枚举值。</param>
    /// <param name="defineName">是否为定义名称，为false时表示特性名称。</param>
    /// <returns>返回一个值的数组。</returns>
    public static string ToName(
#if !net20
        this
#endif
        Enum value, bool defineName) {
        string[] values = value.ToString().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
        if (!defineName) {
            var type = value.GetType();
            for (int i = 0; i < values.Length; i++) {
                string name = ToName_Field(type.GetField(values[i]), "Text");
                if (!string.IsNullOrEmpty(name))
                    values[i] = name;
            }
        }
        return string.Join(defineName ? "," : "，", values);
    }
    /// <summary>
    /// 将当前枚举的值，变成名称串，通常用于多值的枚举。比如将 Abc.A | Abc.B 变成Abc[]{ Abc.A,Abc.B }。
    /// </summary>
    /// <param name="value">当前枚举值。</param>
    /// <param name="key">指定属性名称。</param>
    /// <returns>返回一个值的数组。</returns>
    public static string ToName(
#if !net20
        this
#endif
        Enum value, string key) {
        if (string.IsNullOrEmpty(key))
            key = "Text";

        string[] values = value.ToString().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
        var type = value.GetType();
        for (int i = 0; i < values.Length; i++) {
            string name = ToName_Field(type.GetField(values[i]), key);
            if (!string.IsNullOrEmpty(name))
                values[i] = name;
        }
        return string.Join(key == "Text" ? "," : "，", values);
    }
    #endregion

    #region GetProperty
    /// <summary>
    /// 获取某一个特定的属性值。
    /// </summary>
    /// <param name="value">当前枚举值。</param>
    /// <param name="key">属性名称。</param>
    /// <returns>返回属性的值，未找到时，将是string.Empty。</returns>
    public static string GetProperty(
#if !net20
        this
#endif
        Enum value, string key) {

        if (string.IsNullOrEmpty(key))
            key = "Text";
        string[] values = value.ToString().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
        var type = value.GetType();
        for (int i = 0; i < values.Length; i++) {
            string name = ToName_Field(type.GetField(values[i]), key);
            if (!string.IsNullOrEmpty(name))
                return name;
        }
        return "";
    }
    #endregion

    #region ToDefine
    /// <summary>
    /// 输出枚举定义数组。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <returns>返回枚举定义数组 T[]。</returns>
    public static T[] ToDefine<T>() where T : Enum {
        var array = Enum.GetValues(typeof(T));
        if (array is T[] result) {
            return result;
        }
        result = new T[array.Length];
        for (int i = 0; i < array.Length; i++) {
            result[i] = (T)array.GetValue(i);
        }
        return result;
    }
    /// <summary>
    /// 输出枚举定义数组（用于非泛型时）。
    /// </summary>
    /// <param name="type">枚举类型。</param>
    /// <returns>返回枚举定义数组 type[]。</returns>
    public static Array ToDefine(System.Type type) {
        Symbol.CommonException.CheckArgumentNull(type, "type");
        if (!type.IsEnum)
            Symbol.CommonException.ThrowArgument("type", $"“{type.AssemblyQualifiedName}”必须是枚举类型");
        var array = Enum.GetValues(type);
        if (array.GetType().GetElementType() == type)
            return array;
        var result = Array.CreateInstance(type, array.Length);
        array.CopyTo(result, 0);
        return result;
    }
    #endregion
    #region ToDefineJson
    /// <summary>
    /// 输出枚举定义Json（首字母小写）。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <returns>返回定义Json。</returns>
    public static string ToDefineJson<T>() where T : Enum {
        return ToDefineJson<T>(true, false);
    }
    /// <summary>
    /// 输出枚举定义Json。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <param name="lowerFirstLetter">首字母小写。</param>
    /// <param name="containsToName">包含ToName定义 [{key}Name]=ToName()。</param>
    /// <returns>返回定义Json。</returns>
    public static string ToDefineJson<T>(bool lowerFirstLetter, bool containsToName) where T : Enum {
        return ToDefineJson(typeof(T), lowerFirstLetter, containsToName);
    }
    /// <summary>
    /// 输出枚举定义Json（首字母小写）。
    /// </summary>
    /// <param name="type">枚举类型。</param>
    /// <returns>返回定义Json。</returns>
    public static string ToDefineJson(System.Type type) {
        return ToDefineJson(type, true, false);
    }
    /// <summary>
    /// 输出枚举定义Json。
    /// </summary>
    /// <param name="type">枚举类型。</param>
    /// <param name="lowerFirstLetter">首字母小写。</param>
    /// <param name="containsToName">包含ToName定义 [{key}Name]=ToName()。</param>
    /// <returns>返回定义Json。</returns>
    public static string ToDefineJson(System.Type type, bool lowerFirstLetter, bool containsToName) {
        string key = $"object_{type.AssemblyQualifiedName}_{lowerFirstLetter}_{containsToName}";
        return _list_define_json.GetOrAdd(key, (p) => {
            return JSON.ToJSON(ToDefineObject(type, lowerFirstLetter, containsToName));
        });
    }
    #endregion
    #region ToDefineObject
    /// <summary>
    /// 输出枚举定义对象（首字母小写）。
    /// </summary>
    /// <returns>返回定义对象。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> ToDefineObject<T>() where T : Enum {
        return ToDefineObject<T>(true, false);
    }
    /// <summary>
    /// 输出枚举定义对象。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <param name="lowerFirstLetter">首字母小写。</param>
    /// <param name="containsToName">包含ToName定义 [{key}Name]=ToName()。</param>
    /// <returns>返回定义对象。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> ToDefineObject<T>(bool lowerFirstLetter, bool containsToName) where T : Enum {
        return ToDefineObject(typeof(T), lowerFirstLetter, containsToName);
    }
    /// <summary>
    /// 输出枚举定义对象（首字母小写）。
    /// </summary>
    /// <param name="type">枚举类型。</param>
    /// <returns>返回定义对象。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> ToDefineObject(System.Type type) {
        return ToDefineObject(type, true, false);
    }
    /// <summary>
    /// 输出枚举定义对象。
    /// </summary>
    /// <param name="type">枚举类型。</param>
    /// <param name="lowerFirstLetter">首字母小写。</param>
    /// <param name="containsToName">包含ToName定义，额外数据 [{key}Text]=ToName()。</param>
    /// <returns>返回定义对象。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> ToDefineObject(System.Type type, bool lowerFirstLetter, bool containsToName) {
        Symbol.CommonException.CheckArgumentNull(type, "type");
        if (!type.IsEnum)
            Symbol.CommonException.ThrowArgument("type", $"“{type.AssemblyQualifiedName}”必须是枚举类型");

        string key = $"{type.AssemblyQualifiedName}_{lowerFirstLetter}_{containsToName}";
        return _list_define.GetOrAdd(key, (p) => {
            var list = new Symbol.Collections.Generic.NameValueCollection<object>();
            foreach (Enum item in Enum.GetValues(type)) {
                string name = item.ToString();
                if (lowerFirstLetter)
                    name = name.Substring(0, 1).ToLower() + name.Substring(1);
                list[name] = item.GetHashCode();
                if (containsToName) {
                    list[name + "Text"] = ToName(item);
                }
            }
            return list;
        });
    }
    #endregion

    #region ToDefineObjectsJson
    /// <summary>
    /// 输出枚举定义Json [ { name,value,text } ]（首字母小写）。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <returns>返回定义Json。</returns>
    public static string ToDefineObjectsJson<T>() where T : Enum {
        return ToDefineObjectsJson<T>(true);
    }
    /// <summary>
    /// 输出枚举定义Json [ { name,value,text } ]。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <param name="lowerFirstLetter">首字母小写。</param>
    /// <returns>返回定义Json。</returns>
    public static string ToDefineObjectsJson<T>(bool lowerFirstLetter) where T : Enum {
        return ToDefineObjectsJson(typeof(T), lowerFirstLetter);
    }
    /// <summary>
    /// 输出枚举定义Json [ { name,value,text } ]（首字母小写）。
    /// </summary>
    /// <param name="type">枚举类型。</param>
    /// <returns>返回定义Json。</returns>
    public static string ToDefineObjectsJson(System.Type type) {
        return ToDefineObjectsJson(type, true);
    }
    /// <summary>
    /// 输出枚举定义Json [ { name,value,text } ]。
    /// </summary>
    /// <param name="type">枚举类型。</param>
    /// <param name="lowerFirstLetter">首字母小写。</param>
    /// <returns>返回定义Json。</returns>
    public static string ToDefineObjectsJson(System.Type type, bool lowerFirstLetter) {
        string key = $"objects_{type.AssemblyQualifiedName}_{lowerFirstLetter}";
        return _list_define_json.GetOrAdd(key, (p) => {
            return JSON.ToJSON(ToDefineObjects(type, lowerFirstLetter));
        });
    }
    #endregion
    #region ToDefineObjects
    /// <summary>
    /// 输出枚举定义集合[ { name,value,text } ]（首字母小写）。
    /// </summary>
    /// <returns>返回定义集合。</returns>
    public static System.Collections.Generic.List<object> ToDefineObjects<T>() where T : Enum {
        return ToDefineObjects<T>(true);
    }
    /// <summary>
    /// 输出枚举定义集合[ { name,value,text } ]。
    /// </summary>
    /// <typeparam name="T">任意枚举类型。</typeparam>
    /// <param name="lowerFirstLetter">首字母小写。</param>
    /// <returns>返回定义集合。</returns>
    public static System.Collections.Generic.List<object> ToDefineObjects<T>(bool lowerFirstLetter) where T : Enum {
        return ToDefineObjects(typeof(T), lowerFirstLetter);
    }

    /// <summary>
    /// 输出枚举定义集合[ { name,value,text } ]（首字母小写）。
    /// </summary>
    /// <param name="type">枚举类型。</param>
    /// <returns>返回定义集合。</returns>
    public static System.Collections.Generic.List<object> ToDefineObjects(System.Type type) {
        return ToDefineObjects(type, true);
    }
    /// <summary>
    /// 输出枚举定义集合[ { name,value,text } ]。
    /// </summary>
    /// <param name="type">枚举类型。</param>
    /// <param name="lowerFirstLetter">首字母小写。</param>
    /// <returns>返回定义集合。</returns>
    public static System.Collections.Generic.List<object> ToDefineObjects(System.Type type, bool lowerFirstLetter) {
        Symbol.CommonException.CheckArgumentNull(type, "type");
        if (!type.IsEnum)
            Symbol.CommonException.ThrowArgument("type", $"“{type.AssemblyQualifiedName}”必须是枚举类型");

        string key = $"{type.AssemblyQualifiedName}_{lowerFirstLetter}";
        return _list_define_object.GetOrAdd(key, (p) => {
            var list = new System.Collections.Generic.List<object>();
            foreach (Enum item in Enum.GetValues(type)) {
                string name = item.ToString();
                if (lowerFirstLetter)
                    name = name.Substring(0, 1).ToLower() + name.Substring(1);
                list.Add(new {
                    name = name,
                    value = item.GetHashCode(),
                    text = ToName(item)
                });
            }
            return list;
        });
    }
    #endregion


    #endregion

}