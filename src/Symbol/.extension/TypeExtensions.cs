/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

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
#if netcore
        if (type.GetTypeInfo().IsGenericType) {
#else
        if (type.IsGenericType) {
#endif
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
    public static T Convert<T>(
#if !net20
        this
#endif
        object value, T defaultValue)
        where T : struct {
        T? wrapValue = Convert<T?>(value);
        if (!wrapValue.HasValue)
            wrapValue = defaultValue;
        return wrapValue.Value;
    }
    /// <summary>
    /// 强制转换为另一个类型
    /// </summary>
    /// <typeparam name="T">任意类型</typeparam>
    /// <param name="value">需要转换的对象</param>
    /// <returns>返回需要转换的类型</returns>
    public static T Convert<T>(
#if !net20
        this
#endif
        object value) {
        T result = default(T);
        object result2 = Convert(value, typeof(T));
        if (result2 == null)
            return result;
        else
            return (T)result2;
    }
    static bool? TryBool(object value, bool isNullable) {
        string valueString = value.ToString();
        if (!string.IsNullOrEmpty(valueString)) {
            if (valueString.StartsWith("true", StringComparison.OrdinalIgnoreCase)
                || valueString.StartsWith("-1", StringComparison.OrdinalIgnoreCase)
                || valueString.StartsWith("1", StringComparison.OrdinalIgnoreCase)
                || valueString.StartsWith("t", StringComparison.OrdinalIgnoreCase)
                || valueString.StartsWith("yes", StringComparison.OrdinalIgnoreCase)
                || valueString.StartsWith("ok", StringComparison.OrdinalIgnoreCase)
                || valueString.StartsWith("good", StringComparison.OrdinalIgnoreCase)
                || valueString.StartsWith("right", StringComparison.OrdinalIgnoreCase)) {
                return true;
            }
            if (valueString.StartsWith("false", StringComparison.OrdinalIgnoreCase)
                || valueString.StartsWith("0", StringComparison.OrdinalIgnoreCase)
                || valueString.StartsWith("no", StringComparison.OrdinalIgnoreCase)
                || valueString.StartsWith("f", StringComparison.OrdinalIgnoreCase)
                || valueString.StartsWith("bad", StringComparison.OrdinalIgnoreCase)
                || valueString.StartsWith("not", StringComparison.OrdinalIgnoreCase)) {
                return false;
            }
        }
        if (!isNullable)
            Symbol.CommonException.ThrowFormat("无法将“" + valueString + "”转换为bool类型");
        return null;
    }
    static System.Guid? TryGuid(object value, bool isNullable) {
        try {
            return new Guid((string)value);
        } catch (FormatException) {
            if (!isNullable)
                throw;
        } catch (ArgumentException) {
            if (!isNullable)
                throw;
        } catch (OverflowException) {
            if (!isNullable)
                throw;
        } catch (Exception) {
            if (!isNullable)
                throw;
        }
        return null;
    }
    static System.TimeSpan? TryTimeSpan(object value, bool isNullable) {
        try {
            if (value is string) {
                return TimeSpan.Parse((string)value);
            }
            if (value is long) {
                return new TimeSpan((long)value);
            }
            if (value is int) {
                return new TimeSpan((int)value);
            }
        } catch (FormatException) {
            if (!isNullable)
                throw;
        } catch (OverflowException) {
            if (!isNullable)
                throw;
        } catch (Exception) {
            if (!isNullable)
                throw;
        }
        if (!isNullable)
            Symbol.CommonException.ThrowFormat("无法将“" + value + "”转换为TimeSpan类型");

        return null;
    }
    static object TryTimeSpanAs(object value, System.Type type, bool isNullable) {
        TimeSpan timeSpan = (TimeSpan)value;
        if (type == typeof(string))
            return timeSpan.ToString();
        if (type == typeof(double))
            return timeSpan.TotalMilliseconds;
        if (type == typeof(long)) {
            return timeSpan.Ticks;
        }
        
        if (!isNullable)
            Symbol.CommonException.ThrowFormat("无法将“" + value + "”转换为“"+type.FullName+"”类型");

        return null;
    }
    static System.Text.Encoding TryEncoding(object value) {
        try {
            if (value is string) {
                return System.Text.Encoding.GetEncoding((string)value);
            }
            if (value is int) {
                return System.Text.Encoding.GetEncoding((int)value);
            }
            if (value is long) {
                return System.Text.Encoding.GetEncoding((int)(long)value);
            }
        } catch (ArgumentOutOfRangeException) {
        } catch (ArgumentException) {
        } catch (NotSupportedException) {
        }
        return null;
    }
    static object TryEncodingAs(object value, System.Type type, bool isNullable) {
        var encoding = (System.Text.Encoding)value;
        if (type == typeof(string))
            return encoding.WebName;
        if (type == typeof(int) || type == typeof(long))
            return encoding.CodePage;

        if (!isNullable)
            Symbol.CommonException.ThrowFormat("无法将“" + value + "”转换为“" + type.FullName + "”类型");
        return null;
    }
    static object TryEnum(object value, System.Type type, bool isNullable) {
        try {
            return Enum.Parse(type, value.ToString());
        }catch(ArgumentException) {
            try {
                return Enum.Parse(type, value.ToString(), true);
            } catch {
                if (!isNullable)
                    throw;
            }
        } catch (Exception) {
            if (!isNullable)
                throw;
        }
        return null;
    }
    static Array TryToArray(Array array, System.Type type) {
        Type elementType = type.GetElementType();
        Array array2 = Array.CreateInstance(elementType, array.Length);
        for (int i = 0; i < array2.Length; i++) {
            array2.SetValue(Convert(array.GetValue(i), elementType), i);
        }
        return array2;
    }
    static Array TryToArray(System.Collections.IEnumerable source, System.Type type) {
        if (source == null)
            return null;
        Type elementType = type.GetElementType();
        Type typeList = typeof(System.Collections.Generic.List<>).MakeGenericType(elementType);
        System.Collections.IList list = (System.Collections.IList)Activator.CreateInstance(typeList);
        foreach (object item in source) {
            list.Add(Convert(item,elementType));
        }
        Array array2 = Array.CreateInstance(elementType, list.Count);
        list.CopyTo(array2, 0);
        return array2;
    }
    static Array TryToArray(System.Collections.IEnumerator source, System.Type type) {
        if (source == null)
            return null;
        Type elementType = type.GetElementType();
        Type typeList = typeof(System.Collections.Generic.List<>).MakeGenericType(elementType);
        System.Collections.IList list = (System.Collections.IList)Activator.CreateInstance(typeList);
        while (source.MoveNext()) {
            list.Add(Convert(source.Current, elementType));
        }
        Array array2 = Array.CreateInstance(elementType, list.Count);
        list.CopyTo(array2, 0);
        return array2;
    }
    /// <summary>
    /// 强制转换为另一个类型
    /// </summary>
    /// <param name="value">需要转换的对象</param>
    /// <param name="type">目标类型</param>
    /// <returns>返回需要转换的类型</returns>
    /// <remarks>DBNull识别为null,支持数组转换</remarks>
    public static object Convert(
#if !net20
        this
#endif
        object value, Type type) {
        object result = null;// DefaultValue(type);
        if (value == DBNull.Value)
            value = null;
        if (value == null || (value is string && (string)value == string.Empty)) {
            return result;
        }
        bool isNullable = IsNullableType(type);
        bool valueIsArray = value.GetType().IsArray;
        bool typeIsArray = type.IsArray;
        bool isEnum = false;
        if (isNullable)
            type = Nullable.GetUnderlyingType(type);
        System.Type valueType = value.GetType();
#if netcore
        var valueTypeInfo = valueType.GetTypeInfo();
        var typeInfo = type.GetTypeInfo();
        isEnum = typeInfo.IsEnum;
#else
        System.Type valueTypeInfo = valueType;
        var typeInfo = type;
        isEnum = type.IsEnum;
#endif

        if (valueType == type || IsInheritFrom(valueType, type))
            return value;
        if (type == typeof(bool)) 
            return TryBool(value, isNullable);
        if (isEnum) {
            return TryEnum(value, type, isNullable);
        }
        if (type == typeof(TimeSpan)) {
            return TryTimeSpan(value, isNullable);
        }
        if (valueType == typeof(TimeSpan)) {
            return TryTimeSpanAs(value, type, isNullable);
        }
        if (type == typeof(System.Text.Encoding)) {
            return TryEncoding(value);
        }
        if (valueType == typeof(System.Text.Encoding)) {
            return TryEncodingAs(value, type, isNullable);
        }
        if (typeIsArray) {
            if (valueIsArray)
                return TryToArray((Array)value, type);
            if (type.GetElementType() == typeof(byte) && valueTypeInfo.IsValueType) 
                return StructureToByte(value, valueType);
            result = TryToArray(value as System.Collections.IEnumerable, type);
            if (result != null)
                return result;
            result = TryToArray(value as System.Collections.IEnumerator, type);
            if (result != null)
                return result;
        }
        if (valueIsArray) {
            if (valueType.GetElementType() == typeof(byte) && typeInfo.IsValueType)
                return TryByteToStructure((byte[])value, type, isNullable);
        }
        try {
            if (valueType == typeof(string) && type == typeof(System.Guid)) {
                return TryGuid(value, isNullable);
            } else if (valueType == typeof(System.Guid) && type == typeof(string)) {
                return ((System.Guid)value).ToString("D");
            }
            try {
                result = System.Convert.ChangeType(value, type);
            } catch (InvalidCastException) {
                object tryValue;
                if (TryImplicitConversion(value.GetType(), type, value, out tryValue)) {
                    return tryValue;
                }
                throw;
            }
            
        } catch (Exception e) {
            if (e is InvalidCastException) {
                if (value is DateTime && type == typeof(TimeSpan)) {
                    result = new TimeSpan(((DateTime)value).Ticks);
                    return result;
                } else if (value is TimeSpan && type == typeof(DateTime)) {
                    result = new DateTime(((TimeSpan)value).Ticks);
                    return result;
                }
            }

//#if DEBUG
//            Console.WriteLine(e);
//#endif
            if (!isNullable)
                throw;
        }
        return result;
    }
    /// <summary>  
    /// 由结构体转换为byte数组  
    /// </summary>  
    public static byte[] StructureToByte<T>(T structure) where T : struct {
        return StructureToByte(structure, typeof(T));
    }
    /// <summary>  
    /// 由结构体转换为byte数组  
    /// </summary>  
    public static byte[] StructureToByte(object structure, System.Type type) {
        int size = System.Runtime.InteropServices.Marshal.SizeOf(type);
        byte[] buffer = new byte[size];
        IntPtr bufferIntPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
        try {
            System.Runtime.InteropServices.Marshal.StructureToPtr(structure, bufferIntPtr, true);
            System.Runtime.InteropServices.Marshal.Copy(bufferIntPtr, buffer, 0, size);
        } finally {
            System.Runtime.InteropServices.Marshal.FreeHGlobal(bufferIntPtr);
        }
        return buffer;
    }
    /// <summary>  
    /// 由byte数组转换为结构体  
    /// </summary>  
    public static T ByteToStructure<T>(byte[] buffer) where T : struct {
        return (T)ByteToStructure(buffer, typeof(T));
    }
    /// <summary>  
    /// 由byte数组转换为结构体  
    /// </summary>  
    /// <param name="buffer"></param>
    /// <param name="type"></param>
    public static object ByteToStructure(byte[] buffer, System.Type type) {
        object structure = null;
        int size = System.Runtime.InteropServices.Marshal.SizeOf(type);
        IntPtr allocIntPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
        try {
            System.Runtime.InteropServices.Marshal.Copy(buffer, 0, allocIntPtr, size);
            structure = System.Runtime.InteropServices.Marshal.PtrToStructure(allocIntPtr, type);
        } finally {
            System.Runtime.InteropServices.Marshal.FreeHGlobal(allocIntPtr);
        }
        return structure;
    }
    /// <summary>  
    /// 由byte数组转换为结构体  
    /// </summary>  
    /// <param name="buffer"></param>
    /// <param name="type"></param>
    /// <param name="isNullable"></param>
    public static object TryByteToStructure(byte[] buffer, System.Type type, bool isNullable) {
        object structure = null;
        int size = System.Runtime.InteropServices.Marshal.SizeOf(type);
        IntPtr allocIntPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
        try {
            System.Runtime.InteropServices.Marshal.Copy(buffer, 0, allocIntPtr, size);
            structure = System.Runtime.InteropServices.Marshal.PtrToStructure(allocIntPtr, type);
        } catch {
            if (!isNullable)
                throw;
            structure = DefaultValue(type);
        } finally {
            System.Runtime.InteropServices.Marshal.FreeHGlobal(allocIntPtr);
        }
        return structure;
    }


    /// <summary>
    /// 隐式类型转换方法的名称。
    /// </summary>
    private const string ImplicitConversionName = "op_Implicit";
    /// <summary>
    /// 显式类型转换方法的名称。
    /// </summary>
    private const string ExplicitConviersionName = "op_Explicit";
    static bool TryImplicitConversion(System.Type type, System.Type targetType, object value, out object tryValue) {
        tryValue = null;
        {
            // 尝试让目标类型自己来转换
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < methods.Length; i++) {
                MethodInfo method = methods[i];
                bool opImplicit = method.Name.Equals(ImplicitConversionName, StringComparison.Ordinal);
                //// 如果 opImplicit 已经为 true，则不需要再次进行方法名称的比较。
                //bool opExplicit = opImplicit ? false : method.Name.Equals(ExplicitConviersionName, StringComparison.Ordinal);
                if (!opImplicit)
                    continue;
                System.Type implicitType = method.GetParameters()[0].ParameterType;
                if (type == implicitType) {// || IsInheritFrom(type, implicitType)) {
                    try {
                        tryValue = method.Invoke(null, new object[] { value });
                        return true;
                    } catch (Exception) {

                    }
                }
            }
        }
        {
            //尝试让当前类型来转换
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < methods.Length; i++) {
                MethodInfo method = methods[i];
                //bool opImplicit = method.Name.Equals(ImplicitConversionName, StringComparison.Ordinal);
                //// 如果 opImplicit 已经为 true，则不需要再次进行方法名称的比较。
                bool opExplicit = method.Name.Equals(ExplicitConviersionName, StringComparison.Ordinal);
                if (!opExplicit)
                    continue;
                System.Type implicitType = method.GetParameters()[0].ParameterType;
                if (type == implicitType) {// || IsInheritFrom(type, implicitType)) {
                    try {
                        tryValue = method.Invoke(null, new object[] { value });
                        return true;
                    } catch (Exception) {

                    }
                }
            }
        }
        return false;
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
        Symbol.CommonException.CheckArgumentNull(type, "type");
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

//        Symbol.CommonException.CheckArgumentNull(type, "type");

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
//                Symbol.CommonException.ThrowNotSupported("暂不支持此成员类型“"+info.MemberType+"”");
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
    public delegate object ConvertValue(object value, System.Type type);

    #endregion
}