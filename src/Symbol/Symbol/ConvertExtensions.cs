/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Symbol;

/// <summary>
/// 静态类：值转换。
/// </summary>
public static class ConvertExtensions {


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
            Throw.Format("无法将“" + valueString + "”转换为bool类型");
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
            if (value is double) {
                return TimeSpan.FromMilliseconds((double)value);
            }
            if (value is float) {
                return TimeSpan.FromMilliseconds((float)value);
            }
            if (value is decimal) {
                return TimeSpan.FromMilliseconds((double)(decimal)value);
            }
            if (value is DateTime) {
                return new TimeSpan(((DateTime)value).Ticks);
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
            Throw.Format("无法将“" + value + "”转换为TimeSpan类型");

        return null;
    }
    static object TryTimeSpanAs(object value, System.Type type, bool isNullable) {
        TimeSpan timeSpan = (TimeSpan)value;
        if (type == typeof(string))
            return timeSpan.ToString();
        if (type == typeof(double))
            return timeSpan.TotalMilliseconds;
        if (type == typeof(float))
            return (float)timeSpan.TotalMilliseconds;
        if (type == typeof(decimal))
            return (decimal)timeSpan.TotalMilliseconds;
        if (type == typeof(long))
            return timeSpan.Ticks;
        if (type == typeof(int))
            return (int)timeSpan.Ticks;
        if (type == typeof(DateTime))
            return new DateTime(((TimeSpan)value).Ticks);

        if (!isNullable)
            Throw.Format("无法将“" + value + "”转换为“"+type.FullName+"”类型");

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
        if (type == typeof(int))
            return encoding.CodePage;
        if (type == typeof(long))
            return (long)encoding.CodePage;

        if (!isNullable)
            Throw.Format("无法将“" + value + "”转换为“" + type.FullName + "”类型");
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
        if (value == null || (value is string && (string)value == string.Empty)) {
            return result;
        }
        if (value is DBNull)
            return null;

        bool isNullable = TypeExtensions.IsNullableType(type);
        bool valueIsArray = value.GetType().IsArray;
        bool typeIsArray = type.IsArray;
        bool isEnum = false;
        if (isNullable)
            type = Nullable.GetUnderlyingType(type);
        isEnum = type.IsEnum;
        System.Type valueType = value.GetType();

        if (valueType == type || TypeExtensions.IsInheritFrom(valueType, type))
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
        if (TypeExtensions.IsInheritFrom(valueType, typeof(System.Text.Encoding))) {
            return TryEncodingAs(value, type, isNullable);
        }
        if (typeIsArray) {
            if (valueIsArray)
                return TryToArray((Array)value, type);
            if (type.GetElementType() == typeof(byte) && valueType.IsValueType) 
                return StructureToByte(value, valueType);
            result = TryToArray(value as System.Collections.IEnumerable, type);
            if (result != null)
                return result;
            result = TryToArray(value as System.Collections.IEnumerator,type);
            if (result != null)
                return result;
        }
        if (valueIsArray) {
            if (valueType.GetElementType() == typeof(byte) && type.IsValueType)
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
                if (TryImplicitConversion(value.GetType(), type, value, out object tryValue)) {
                    return tryValue;
                }
                throw;
            }
            
        } catch{
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
            structure = TypeExtensions.DefaultValue(type);
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


}
/// <summary>
/// 数据转换Func。
/// </summary>
/// <param name="value">值。</param>
/// <param name="type">目标类型。</param>
/// <returns></returns>
public delegate object ConvertValueFunc(object value, Type type);