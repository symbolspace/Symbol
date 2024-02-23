/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Symbol;

/// <summary>
/// 类型扩展类
/// </summary>
public static class TypeExtensions {

    #region fields
    /// <summary>
    /// 通用BindingFlags。
    /// </summary>
    public static readonly BindingFlags BindingFlags = BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
    private static readonly MethodInfo DefaultMethodInfo = typeof(TypeExtensions).GetMethod("Default", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
    private static readonly object[] EmptyParams = new object[0];
    private static readonly ICollection<Type> _numbericType = new Type[]{
                typeof(byte),
                typeof(short),
                typeof(int),typeof(uint),
                typeof(long),typeof(ulong),
                typeof(float),
                typeof(decimal),
                typeof(double),
                typeof(byte?),
                typeof(short?),
                typeof(int?),typeof(uint?),
                typeof(long?),typeof(ulong?),
                typeof(float?),
                typeof(decimal?),
                typeof(double?),
        };
    private static System.Collections.Generic.Dictionary<string, string> _list_type_keywords = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
        { "System.Boolean", "bool" },
        { "System.Byte", "byte"},
        { "System.SByte", "sbyte"},
        { "System.Int16", "short"},
        { "System.UInt16", "ushort"},
        { "System.Int32", "int"},
        { "System.UInt32", "uint"},
        { "System.Int64", "long"},
        { "System.UInt64", "ulong"},
        { "System.Single", "float"},
        { "System.Double", "double"},
        { "System.Decimal", "decimal"},
        { "System.String", "string"},
        { "System.Char", "char"},
        { "System.Void", "void"},
        { "System.Object", "object"},
    };
        

    #endregion

    #region methods

    #region IsInheritFrom
    /// <summary>
    /// 判断两个类型是否有继承关系。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <param name="parent">用于检查的类型，判断type是否有继承parent。</param>
    /// <returns>支持接口、类，返回true表示有继承关系。</returns>
    public static bool IsInheritFrom(
#if !net20
        this
#endif
        Type type, Type parent) {
        if (type == null || parent == null)
            return false;
#if netcore
        if (parent.GetTypeInfo().IsInterface) {
            Type[] types = type.GetInterfaces();
            if (types == null || types.Length == 0)
                return false;
            if (Array.Exists(types, p => p.FullName == parent.FullName))
                return true;
            goto lb_Base;
        } else {
            if (type.GetTypeInfo().IsSubclassOf(parent))
                return true;
            goto lb_Base;
        }
#else
        if (parent.IsInterface) {
            Type[] types = type.GetInterfaces();
            if (types == null || types.Length == 0)
                return false;
            if (Array.Exists(types, p => p.FullName == parent.FullName))
                return true;
            goto lb_Base;
        } else {
            if (type.IsSubclassOf(parent))
                return true;
            goto lb_Base;
        }
#endif
        lb_Base:
        return false;
    }
    #endregion

    #region FullName2
    /// <summary>
    /// 完善Type.FullName的输出，主要是针对泛型类。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <returns>返回完带的类型名称输出。</returns>
    public static string FullName2(
#if !net20
        this
#endif
        Type type) {
        return FullName2(type, false);
    }
    /// <summary>
    /// 完善Type.FullName的输出，主要是针对泛型类。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <param name="typeNameOnly">是否仅输出类型名</param>
    /// <returns>返回完带的类型名称输出。</returns>
    public static string FullName2(
#if !net20
        this
#endif
        Type type, bool typeNameOnly) {
        if (type == null)
            return null;
        string result = typeNameOnly ? type.Name : type.FullName;
        if (type.IsGenericType) {
            if (typeNameOnly) {
                result = type.Name;
            } else {
                result = string.Format("{0}.{1}", type.Namespace, type.Name);
            }
            int i = result.IndexOf('`');
            if (i != -1)
                result = result.Substring(0, i);
            result += '<';
            Type[] types = type.GetGenericArguments();
            if (types != null) {
                for (int j = 0; j < types.Length; j++) {
                    if (j > 0)
                        result += ",";
                    result += FullName2(types[j]);
                }
            }
            result += '>';
        }
        return result;
    }
    #endregion

    #region GetFullName
    /// <summary>
    /// 获取完整名称（方便展示）
    /// </summary>
    /// <param name="type">Type的实例</param>
    /// <returns>返回完带的类型名称输出。</returns>
    public static string GetFullName(System.Type type) {
        return GetFullName(type, false, false);
    }
    /// <summary>
    /// 获取完整名称（方便展示）
    /// </summary>
    /// <param name="type">Type的实例</param>
    /// <param name="isTypeDefine">是否为类型定义。</param>
    /// <param name="isNamespaceEq">是否匹配命名空间</param>
    /// <returns>返回完带的类型名称输出。</returns>
    public static string GetFullName(System.Type type, bool isTypeDefine, bool isNamespaceEq) {
        if (IsNullableType(type)) {
            return GetFullName(GetNullableType(type)) + "?";
        } else if (type.IsArray) {
            return GetFullName(type.GetElementType()) + "[]";
        } else if (type.IsGenericType) {
            System.Type genericType = type.GetGenericTypeDefinition();
            string genericName = FullName2(type.GetGenericTypeDefinition()).Split('<')[0];
            if (isTypeDefine) {
                genericName = type.GetGenericTypeDefinition().Name.Split('`')[0];
            }
            return genericName 
                    + "<" 
                    + string.Join(
                            ",", 
                            LinqHelper.Select(
                                type.GetGenericArguments(), 
                                p => GetFullName(p))
#if net20 || net35
                                .ToArray()
#endif
                                ) 
                    + ">";
        } else {
            if (isTypeDefine)
                return type.Name;

            string typeName = FullName2(type, isNamespaceEq);
            string value = null;
            if (_list_type_keywords.TryGetValue(typeName, out value))
                typeName = value;

            return typeName.Replace('+', '.');
        }
    }
    #endregion

    #region DisplayName
    //    /// <summary>
    //    /// 从ICustomAttributeProvider实例上读取DisplayNameAttribute特性，如果有将返回它的DisplayName属性
    //    /// </summary>
    //    /// <param name="memberInfo">ICustomAttributeProvider的实例</param>
    //    /// <returns>返回DisplayName值，如果没有这个特性将返回memberInfo.Name</returns>
    //    public static string DisplayName(
    //#if !net20
    //        this
    //#endif
    //        ICustomAttributeProvider memberInfo) {
    //        DisplayNameAttribute[] attributes = (DisplayNameAttribute[])memberInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true);
    //        if (attributes == null || attributes.Length == 0)
    //            return FastWrapper.Get(memberInfo, "Name") as string;
    //        return attributes[0].DisplayName;
    //    }
    #endregion

    #region DefaultValue
    /// <summary>
    /// 获取一个类型的默认值，结果与系统的default(T) 是一样的结果。
    /// </summary>
    /// <param name="type">Type的实例</param>
    /// <returns>返回默认值</returns>
    public static object DefaultValue(Type type) {
        return DefaultMethodInfo.MakeGenericMethod(type).Invoke(null, EmptyParams);
    }
    private static T Default<T>() {
        return default(T);
    }
    #endregion

    #region Convert
    /// <summary>
    /// 强制转换为另一个类型（仅限struct类型）
    /// </summary>
    /// <typeparam name="T">任意struct类型</typeparam>
    /// <param name="value">需要转换的对象</param>
    /// <param name="defaultValue">如果转换不成功时采用的默认值</param>
    /// <returns>返回需要转换的类型</returns>
    [Obsolete("请更换为：ConvertExtensions.Convert<T>(value, defaultValue)", true)]
    public static T Convert<T>(
        object value, T defaultValue)
        where T : struct {
        return ConvertExtensions.Convert<T>(value, defaultValue);
    }
    /// <summary>
    /// 强制转换为另一个类型
    /// </summary>
    /// <typeparam name="T">任意类型</typeparam>
    /// <param name="value">需要转换的对象</param>
    /// <returns>返回需要转换的类型</returns>
    [Obsolete("请更换为：ConvertExtensions.Convert<T>(value)", true)]
    public static T Convert<T>(
        object value) {
        return ConvertExtensions.Convert<T>(value);
    }
    /// <summary>
    /// 强制转换为另一个类型
    /// </summary>
    /// <param name="value">需要转换的对象</param>
    /// <param name="type">目标类型</param>
    /// <returns>返回需要转换的类型</returns>
    /// <remarks>DBNull识别为null,支持数组转换</remarks>
    [Obsolete("请更换为：ConvertExtensions.Convert(value, type)", true)]
    public static object Convert(
        object value, Type type) {
        return ConvertExtensions.Convert(value, type);
    }
    #endregion

    #region IsNullableType
    /// <summary>
    /// 判断是否为可为空类型，比如int?这种类型。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <returns>返回为true表示此类型为struct类型，并且采用的是Nullable&lt;T&gt;。</returns>
    public static bool IsNullableType(
#if !net20
        this
#endif
        Type type) {
        Throw.CheckArgumentNull(type, "type");
#if netcore
        return (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
#else
        return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
#endif
    }
    #endregion
    #region GetNullableType
    /// <summary>
    /// 获取可为空类型的原始类型。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <returns>如果为非可为空类型，返回的就是它自己，反之而是被包装的类型。</returns>
    public static Type GetNullableType(
#if !net20
        this
#endif
        Type type) {
        if (!IsNullableType(type))
            return type;
        else
            return Nullable.GetUnderlyingType(type);
    }
    #endregion

    #region IsNumbericType
    /// <summary>
    /// 是否为数字类型。
    /// </summary>
    /// <param name="value">当前值。</param>
    /// <returns>为null时返回false，反之为数字类型时返回为true。</returns>
    public static bool IsNumbericType(
#if !net20
        this
#endif
        object value) {
        return IsNumbericType(value == null ? null : value.GetType());
    }
    /// <summary>
    /// 是否为数字类型。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <returns>为null时返回false，反之为数字类型时返回为true。</returns>
    public static bool IsNumbericType(
#if !net20
        this
#endif
        Type type) {
        if (type == null)
            return false;
        return _numbericType.Contains(type);
    }
    #endregion

    #region GetValueMemberType
//    /// <summary>
//    /// 获取对象成员类型。
//    /// </summary>
//    /// <param name="obj">当前对象。</param>
//    /// <param name="name">成员名称。</param>
//    /// <returns>返回成员的类型。</returns>
//    public static Type GetValueMemberType(
//#if !net20
//        this
//#endif
//        object obj, string name) {
//        return GetValueMemberType(obj, name, false);
//    }
//    /// <summary>
//    /// 获取对象成员类型。
//    /// </summary>
//    /// <param name="obj">当前对象。</param>
//    /// <param name="name">成员名称。</param>
//    /// <param name="ignoreCase">是否忽略大小写。</param>
//    /// <returns>返回成员的类型。</returns>
//    public static Type GetValueMemberType(
//#if !net20
//        this
//#endif
//        object obj, string name, bool ignoreCase) {
//        if (obj == null)
//            return null;
//        else
//            return GetValueMemberType(obj.GetType(), name, ignoreCase);
//    }
//    /// <summary>
//    /// 获取类型的成员类型。
//    /// </summary>
//    /// <param name="type">当前类型。</param>
//    /// <param name="name">成员名称。</param>
//    /// <returns>返回成员的类型。</returns>
//    public static Type GetValueMemberType(
//#if !net20
//        this
//#endif
//        Type type, string name) {
//        return GetValueMemberType(type, name, false);
//    }
//    /// <summary>
//    /// 获取类型的成员类型。
//    /// </summary>
//    /// <param name="type">当前类型。</param>
//    /// <param name="name">成员名称。</param>
//    /// <param name="ignoreCase">是否忽略大小写。</param>
//    /// <returns>返回成员的类型。</returns>
//    public static Type GetValueMemberType(
//#if !net20
//        this
//#endif
//        Type type, string name, bool ignoreCase) {

//        Throw.CheckArgumentNull(type, "type");

//        BindingFlags flags = BindingFlags;
//        if (ignoreCase)
//            flags |= BindingFlags.IgnoreCase;
//        MemberInfo[] infos = type.GetMember(name, flags);
//        if (infos == null || infos.Length != 1)
//            return null;
//        MemberInfo info = infos[0];
//        Type result = null;
//        switch (info.MemberType) {
//            case MemberTypes.Constructor:
//                result = type;
//                break;
//            case MemberTypes.Field:
//                result = ((FieldInfo)info).FieldType;
//                break;
//            case MemberTypes.Property:
//                result = ((PropertyInfo)info).PropertyType;
//                break;
//            case MemberTypes.Method:
//                result = ((MethodInfo)info).ReturnType;
//                break;
//            case MemberTypes.Event:
//                result = ((EventInfo)info).EventHandlerType;
//                break;
//            default:
//                Throw.NotSupported("暂不支持此成员类型“"+info.MemberType+"”");
//                break;
//        }
//        return result;
//    }
    #endregion

    #region IsSystemBaseType
    /// <summary>
    /// 是否为系统基础类型。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <returns>为true表示为基础类型，比如string int。</returns>
    public static bool IsSystemBaseType(
#if !net20
        this
#endif
        Type type) {
        if (type == typeof(object))
            return true;
        type = GetNullableType(type);
#if netcore
        System.Reflection.TypeInfo typeInfo = type.GetTypeInfo();
        return type != null
            && typeInfo.BaseType != null                     //基类型必须有
            && typeInfo.Assembly == typeof(string).GetTypeInfo().Assembly //必须是系统类型
            && IsInheritFrom(type, typeof(IConvertible));
#else
        return type != null
            && type.BaseType != null                     //基类型必须有
            && type.Assembly == typeof(string).Assembly //必须是系统类型
            && IsInheritFrom(type, typeof(IConvertible));
#endif

    }
    #endregion
    #region IsAnonymousType
    /// <summary>
    /// 是否为匿名类型。
    /// </summary>
    /// <param name="value">当前值。</param>
    /// <returns>为null返回false，反之为匿名对象时返回true。</returns>
    public static bool IsAnonymousType(
#if !net20
        this
#endif
        object value) {
        return value == null ? false : IsAnonymousType(value.GetType());
    }
    /// <summary>
    /// 是否为匿名类型。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <returns>为null返回false，反之为匿名对象时返回true。</returns>
    public static bool IsAnonymousType(
#if !net20
        this
#endif
        Type type) {
        return type != null
            && type.Namespace == null
#if netcore
            && type.GetTypeInfo().BaseType == typeof(object)
#else
            && type.BaseType == typeof(object)
#endif
            && (
                type.Name.StartsWith("<>f__AnonymousType")
                || type.Name.StartsWith("VB$AnonymousType")
            );
            //&& type.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false);
    }
    #endregion

    #region Get
    /// <summary>
    /// 获取对象的成员值（属性或字段）。
    /// </summary>
    /// <param name="instance">对象实例。</param>
    /// <param name="name">成员名称。</param>
    /// <param name="indexs">索引序列（普通属性和字段不传）。</param>
    /// <returns>返回成员的值。</returns>
    public static object Get(
#if !net20
    this
#endif
        object instance, string name, object[] indexs = null) {
        if (indexs == null) {
            return FastWrapper.Get(instance, name);
        }
        return FastWrapper.Get(instance, name, indexs);
    }
    #endregion
    #region Set
    /// <summary>
    /// 设置对象的成员值（属性或字段）。
    /// </summary>
    /// <param name="instance">对象实例。</param>
    /// <param name="name">成员名称。</param>
    /// <param name="value">成员的值。</param>
    /// <param name="indexs">索引序列（普通属性和字段不传）。</param>
    public static void Set(
#if !net20
    this
#endif
        object instance, string name, object value, object[] indexs = null) {
        if (indexs == null) {
            FastWrapper.Set(instance, name, value, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.SetField);
        }
        FastWrapper.Set(instance, name, value, indexs, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.SetField);
    }
    #endregion
    #region MethodInvoke
    /// <summary>
    /// 调用类型的 实例 方法。
    /// </summary>
    /// <param name="name">方法名称。</param>
    /// <param name="instance">当前实例。</param>
    /// <param name="args">参数列表。</param>
    /// <returns>返回方法调用的结果。</returns>
    public static object MethodInvoke(
#if !net20
    this
#endif
        object instance, string name,params object[] args) {
        return FastWrapper.MethodInvoke(instance.GetType(), name, instance, args);
    }
    /// <summary>
    /// 调用类型的 实例 方法。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <param name="name">方法名称。</param>
    /// <param name="instance">当前实例。</param>
    /// <param name="args">参数列表。</param>
    /// <returns>返回方法调用的结果。</returns>
    public static object MethodInvoke(
#if !net20
    this
#endif
        object instance,string name, System.Type type,params object[] args) {
        return FastWrapper.MethodInvoke(type, name, instance, args);
    }
    #endregion

    #endregion


    #region types
    /// <summary>
    /// 数据转换委托。
    /// </summary>
    /// <param name="value">值。</param>
    /// <param name="type">目标类型。</param>
    /// <returns></returns>
    [Obsolete("请更换为：全局ConvertValueFunc", true)]
    public delegate object ConvertValue(object value, System.Type type);
    #endregion
}