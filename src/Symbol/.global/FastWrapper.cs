/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Globalization;
using System.Reflection;


/// <summary>
/// 快速包装（反射调用）类。
/// </summary>
public partial class FastWrapper {

    #region fields
    private static readonly bool _isFramework40 = typeof(object).AssemblyQualifiedName.Contains("Version=4.0.0.0");
    /// <summary>
    /// 默认BindingFlags
    /// </summary>
    public static readonly BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.NonPublic;

    #endregion

    #region properties
    /// <summary>
    /// 是否为.net framework4.0。
    /// </summary>
    public static bool IsFramework40 {
        get { return _isFramework40; }
    }
    /// <summary>
    /// 当前类型。
    /// </summary>
    public Type Type { get; private set; }
    /// <summary>
    /// 获取或设置当前的实例。
    /// </summary>
    public object Instance { get; set; }
    /// <summary>
    /// 获取或设置是否忽略大小写。
    /// </summary>
    public bool IgnoreCase { get; set; }

    /// <summary>
    /// 获取或设置当前 实例/静态 的成员值。
    /// </summary>
    /// <param name="name">属性或字段名称。</param>
    /// <returns>返回属性或字段的值。</returns>
    public object this[string name] {
        get { return Get(name); }
        set { Set(name, value); }
    }
    /// <summary>
    /// 获取或设置当前 实例/静态 的成员值。
    /// </summary>
    /// <param name="name">属性或字段名称。</param>
    /// <param name="index">索引序列。</param>
    /// <returns>返回属性或字段的值。</returns>
    public object this[string name, object[] index] {
        get { return Get(name, index); }
        set { Set(name, value, index); }
    }

    #endregion

    #region ctor
    /// <summary>
    /// 创建 FastWrapper 的实例。
    /// </summary>
    /// <param name="type">需要包装的类型。</param>
    /// <param name="autoInstance">是否自动创建实例。</param>
    public FastWrapper(string type, bool autoInstance) : this(GetWarpperType(type), autoInstance) {
    }
    /// <summary>
    /// 创建 FastWrapper 的实例。
    /// </summary>
    /// <param name="type">需要包装的类型。</param>
    /// <param name="args">构造函数需要用到的参数。</param>
    public FastWrapper(string type, params object[] args) : this(GetWarpperType(type), args) {
    }

    /// <summary>
    /// 创建 FastWrapper 的实例。
    /// </summary>
    /// <param name="type">需要包装的类型。</param>
    /// <param name="args">构造函数需要用到的参数。</param>
    public FastWrapper(Type type, params object[] args)
        : this(type, false) {
        Instance = CreateInstance(Type, args);
    }
    /// <summary>
    /// 创建 FastWrapper 的实例。
    /// </summary>
    /// <param name="type">需要包装的类型。</param>
    /// <param name="autoInstance">是否自动创建实例。</param>
    public FastWrapper(Type type, bool autoInstance) {
        Symbol.CommonException.CheckArgumentNull(type, "type");

        Type = type;
        if (Instance == null && autoInstance) {
            Instance = CreateInstance(Type);
        }
    }
    /// <summary>
    /// 创建 FastWrapper 的实例。
    /// </summary>
    /// <param name="instance">当前实例。</param>
    public FastWrapper(object instance) : this(instance, instance == null ? typeof(object) : instance.GetType()) { }
    /// <summary>
    /// 创建 FastWrapper 的实例。
    /// </summary>
    /// <param name="instance">当前实例。</param>
    /// <param name="type">类型。</param>
    public FastWrapper(object instance, System.Type type) {
        Symbol.CommonException.CheckArgumentNull(instance, "instance");
        Symbol.CommonException.CheckArgumentNull(type, "type");

        Instance = instance;
        Type = type;
    }

    #endregion

    #region methods

    #region GetBindingFlags
    /// <summary>
    /// 在默认BindingFlags上创建一个新的 BindingFlags。
    /// </summary>
    /// <param name="flags">附加 BindingFlags 。</param>
    /// <returns>返回新的 BindingFlags 。</returns>
    public BindingFlags GetBindingFlags(BindingFlags flags) {
        BindingFlags result = DefaultBindingFlags | flags;
        if (IgnoreCase)
            result |= BindingFlags.IgnoreCase;
        return result;
    }
    #endregion
#if !NETDNX
    #region InvokeMember
    /// <summary>
    /// 调用成员。
    /// </summary>
    /// <param name="name">成员名称。</param>
    /// <param name="bindingFlags">BindingFlags。</param>
    /// <param name="args">参数列表。</param>
    /// <returns>返回调用结果。</returns>
    public object InvokeMember(string name, BindingFlags bindingFlags, object[] args) {
        return InvokeMember(Type, name, Instance, GetBindingFlags(bindingFlags), args);
    }
    #endregion
#endif
    #region MethodInvoke
    /// <summary>
    /// 调用方法。
    /// </summary>
    /// <param name="name">方法名称。</param>
    /// <param name="args">参数列表。</param>
    /// <returns>返回调用结果。</returns>
    public object MethodInvoke(string name, params object[] args) {
#if NETDNX
        return MethodInvoke(Type, name, Instance, args);
#else
        return InvokeMember(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.InvokeMethod,
            args);
#endif
    }
    #endregion
    #region Get
    //o index[]
    /// <summary>
    /// 获取成员的值。
    /// </summary>
    /// <param name="name">属性或字段名称。</param>
    /// <returns>返回属性或字段的值。</returns>
    public object Get(string name) {
#if NETDNX
        return Get(Type, name, Instance, new object[0]);
#else
        return InvokeMember(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.GetField,
            new object[0]);
#endif
    }
    /// <summary>
    /// 获取成员的值。
    /// </summary>
    /// <param name="name">属性或字段名称。</param>
    /// <param name="index">索引序列号。</param>
    /// <returns>返回属性或字段的值。</returns>
    public object Get(string name, object[] index) {
        return Get(Type, name, Instance, index);
    }
    #endregion
    #region Set
    //o index[]
    /// <summary>
    /// 设置成员的值。
    /// </summary>
    /// <param name="name">属性或字段名称。</param>
    /// <param name="value">属性或字段的值。</param>
    public void Set(string name, object value) {
#if NETDNX
        Set(Type, name,value, Instance, new object[0]);
#else
        InvokeMember(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.SetField,
            new object[] { value });
#endif

    }
    /// <summary>
    /// 设置成员的值。
    /// </summary>
    /// <param name="name">属性或字段名称。</param>
    /// <param name="value">属性或字段的值。</param>
    /// <param name="index">索引序列号。</param>
    public void Set(string name, object value, object[] index) {
        Set(Type, name, value, Instance, index);
    }
    #endregion

    #region CreateInstance
    /// <summary>
    /// 创建指定类型的实例。
    /// </summary>
    /// <param name="type">类型。</param>
    /// <param name="args">构造函数的参数。</param>
    /// <returns>返回新实例。</returns>
    public static object CreateInstance(Type type, params object[] args) {
        Symbol.CommonException.CheckArgumentNull(type, "type");

        //return type.InvokeMember(".ctor",
        //    DefaultBindingFlags | BindingFlags.CreateInstance | BindingFlags.Instance,
        //    null, null, args);
        return Activator.CreateInstance(type,
            DefaultBindingFlags | BindingFlags.Instance,
            null, args, CultureInfo.CurrentCulture);
    }
    /// <summary>
    /// 创建指定类型的实例（泛型）。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    /// <param name="args">构造函数的参数。</param>
    /// <returns>返回新实例。</returns>
    public static T CreateInstance<T>(params object[] args) {
        return (T)CreateInstance(typeof(T), args);
    }
    /// <summary>
    /// 创建指定类型的实例。
    /// </summary>
    /// <param name="type">类型。</param>
    /// <param name="args">构造函数的参数。</param>
    /// <returns>返回新实例。</returns>
    public static T CreateInstance<T>(Type type, params object[] args) {
        return (T)CreateInstance(type, args);
    }
    #endregion

    #region Get
    /// <summary>
    /// 获取对象成员的值。
    /// </summary>
    /// <param name="instance">当前实例。</param>
    /// <param name="name">属性或字段名称。</param>
    /// <returns>返回属性或字段的值。</returns>
    public static object Get(object instance, string name) {
        return Get(instance, name, null);
    }
    /// <summary>
    /// 获取对象成员的值。
    /// </summary>
    /// <param name="instance">当前实例。</param>
    /// <param name="name">属性或字段名称。</param>
    /// <param name="index">索引序列号。</param>
    /// <returns>返回属性或字段的值。</returns>
    public static object Get(object instance, string name, object[] index) {
        Symbol.CommonException.CheckArgumentNull(instance, "instance");
        return Get(instance.GetType(), name, instance, index);
    }
    /// <summary>
    /// 获取类型静态成员的值。
    /// </summary>
    /// <param name="type">当前实例。</param>
    /// <param name="name">属性或字段名称。</param>
    /// <returns>返回属性或字段的值。</returns>
    public static object Get(Type type, string name) {
        return Get(type, name, null);
    }
    /// <summary>
    /// 获取类型静态成员的值。
    /// </summary>
    /// <param name="type">当前实例。</param>
    /// <param name="name">属性或字段名称。</param>
    /// <param name="index">索引序列号。</param>
    /// <returns>返回属性或字段的值。</returns>
    public static object Get(Type type, string name, object[] index) {
        return Get(type, name, null, index);
    }

    private static object Get(Type type, string name, object instance, object[] index) {
        Symbol.CommonException.CheckArgumentNull(type, "type");
        PropertyInfo propertyInfo = type.GetProperty(name, DefaultBindingFlags | BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Instance);
        if (propertyInfo != null) {
            return propertyInfo.GetValue(instance, index);
        }
        FieldInfo fieldInfo = type.GetField(name, DefaultBindingFlags | BindingFlags.GetField | BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Instance);
        if (fieldInfo != null) {
            return fieldInfo.GetValue(instance);
        }
        Symbol.CommonException.ThrowNotSupported("“" + TypeExtensions.FullName2(type) + "”不支持此属性或字段");
        return null;
    }

    /// <summary>
    /// 获取对象成员的值。
    /// </summary>
    /// <param name="type">类型。</param>
    /// <param name="name">属性或字段名称。</param>
    /// <param name="instance">当前实例。</param>
    /// <param name="index">索引序列号。</param>
    /// <param name="value">输出值。</param>
    /// <returns>返回是否成功。</returns>
    public static bool TryGet(Type type, string name, object instance, object[] index,out object value) {
        Symbol.CommonException.CheckArgumentNull(type, "type");
        PropertyInfo propertyInfo = type.GetProperty(name, DefaultBindingFlags | BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Instance) ;
        if (propertyInfo != null) {
            value= propertyInfo.GetValue(instance, index);
            return true;
        }
        FieldInfo fieldInfo = type.GetField(name, DefaultBindingFlags | BindingFlags.GetField | BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Instance);
        if (fieldInfo != null) {
            value= fieldInfo.GetValue(instance);
            return true;
        }
        value = null;
        return false;
        //Symbol.CommonException.ThrowNotSupported("“" + TypeExtensions.FullName2(type) + "”不支持此属性或字段");
    }
    
    #endregion

    #region Set
    /// <summary>
    /// 设置对象成员的值。
    /// </summary>
    /// <param name="instance">当前实例。</param>
    /// <param name="name">属性或字段名称。</param>
    /// <param name="value">值。</param>
    /// <param name="bindingFlags">附加BindingFlags</param>
    public static void Set(object instance, string name, object value, BindingFlags bindingFlags = BindingFlags.Public) {
        Set(instance, name, value, null, bindingFlags);
    }
    /// <summary>
    /// 设置对象成员的值。
    /// </summary>
    /// <param name="instance">当前实例。</param>
    /// <param name="name">属性或字段名称。</param>
    /// <param name="value">值。</param>
    /// <param name="index">索引序列号。</param>
    /// <param name="bindingFlags">附加BindingFlags</param>
    public static void Set(object instance, string name, object value, object[] index, BindingFlags bindingFlags = BindingFlags.Public) {
        Symbol.CommonException.CheckArgumentNull(instance, "instance");
        Set(instance.GetType(), name, value, instance, index, bindingFlags);
    }
    /// <summary>
    /// 设置类型静态成员的值。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <param name="name">属性或字段名称。</param>
    /// <param name="value">值。</param>
    /// <param name="bindingFlags">附加BindingFlags</param>
    public static void Set(Type type, string name, object value, BindingFlags bindingFlags = BindingFlags.Public) {
        Set(type, name, value, null, bindingFlags);
    }
    /// <summary>
    /// 设置类型静态成员的值。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <param name="name">属性或字段名称。</param>
    /// <param name="value">值。</param>
    /// <param name="index">索引序列号。</param>
    /// <param name="bindingFlags">附加BindingFlags</param>
    public static void Set(Type type, string name, object value, object[] index, BindingFlags bindingFlags = BindingFlags.Public) {
        Set(type, name, value, null, index, bindingFlags);
    }

    private static void Set(Type type, string name, object value, object instance, object[] index, BindingFlags bindingFlags = BindingFlags.Public) {
        Symbol.CommonException.CheckArgumentNull(type, "type");
        PropertyInfo propertyInfo = type.GetProperty(name, bindingFlags | DefaultBindingFlags | BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Instance);
        if (propertyInfo != null && (propertyInfo.GetSetMethod()!=null|| propertyInfo.GetSetMethod(true)!=null)) {
            propertyInfo.SetValue(instance, TypeExtensions.Convert(value, propertyInfo.PropertyType), index);
            return;
        }
        FieldInfo fieldInfo = type.GetField(name, bindingFlags | DefaultBindingFlags | BindingFlags.SetField | BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Instance);
        if (fieldInfo != null) {
            fieldInfo.SetValue(instance, TypeExtensions.Convert(value, fieldInfo.FieldType));
            return;
        }
        Symbol.CommonException.ThrowNotSupported("“" + TypeExtensions.FullName2(type) + "”不支持此属性或字段");

    }
    #endregion
#if !NETDNX
    #region InvokeMember
    /// <summary>
    /// 调用对像的成员。
    /// </summary>
    /// <param name="instance">当前实例。</param>
    /// <param name="name">成员名称。</param>
    /// <param name="bindingFlags">BindingFlags。</param>
    /// <param name="args">参数列表。</param>
    /// <returns>返回调用结果。</returns>
    public static object InvokeMember(object instance, string name, BindingFlags bindingFlags, params object[] args) {
        Symbol.CommonException.CheckArgumentNull(instance, "instance");
        return InvokeMember(instance.GetType(), name, instance, bindingFlags, args);
    }
    /// <summary>
    /// 调用类型的静态成员。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <param name="name">成员名称。</param>
    /// <param name="bindingFlags">BindingFlags。</param>
    /// <param name="args">参数列表。</param>
    /// <returns>返回调用结果。</returns>
    public static object InvokeMember(Type type, string name, BindingFlags bindingFlags, params object[] args) {
        return InvokeMember(type, name, null, bindingFlags, args);
    }

    private static object InvokeMember(Type type, string name, object instance, BindingFlags bindingFlags, object[] args) {
        Symbol.CommonException.CheckArgumentNull(type, "type");
#if netcore
        return type.InvokeMember(name, bindingFlags, null, instance, args, CultureInfo.CurrentCulture);
#else
        return type.InvokeMember(name, bindingFlags, null, instance, args, CultureInfo.CurrentCulture);
#endif

    }
    #endregion
#endif

    #region MethodInvoke
    /// <summary>
    /// 调用类型的静态方法。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <param name="name">方法名称。</param>
    /// <param name="args">参数列表。</param>
    /// <returns>返回方法调用的结果。</returns>
    public static object MethodInvoke(Type type, string name, params object[] args) {
        return MethodInvoke(type, name, null, args);
    }
    /// <summary>
    /// 调用类型的 实例/静态 方法。
    /// </summary>
    /// <param name="type">当前类型。</param>
    /// <param name="name">方法名称。</param>
    /// <param name="instance">当前实例。</param>
    /// <param name="args">参数列表。</param>
    /// <returns>返回方法调用的结果。</returns>
    public static object MethodInvoke(Type type, string name, object instance, object[] args) {
#if netcore
        System.Reflection.MethodInfo methodInfo = null;
        if (instance == null) {
            methodInfo = type.GetMethod(name, DefaultBindingFlags | BindingFlags.Static | BindingFlags.InvokeMethod);
        } else {
            methodInfo = type.GetMethod(name, DefaultBindingFlags | BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Static);
        }
        if (methodInfo == null)
            Symbol.CommonException.ThrowNotSupported(string.Format("未找到方法 {0}.{1}", type.FullName2(), name));

        return methodInfo.Invoke(instance, args);
#else
        return InvokeMember(type, name, instance, DefaultBindingFlags | BindingFlags.Static | BindingFlags.Instance | BindingFlags.InvokeMethod, args);
#endif
    }
    #endregion

    #region GetWarpperType
    /// <summary>
    /// 快速获取包装类型。
    /// </summary>
    /// <param name="typeFullName">类型的全名称。</param>
    /// <returns>返回对应的类型。</returns>
    public static Type GetWarpperType(string typeFullName) {
        if (string.IsNullOrEmpty(typeFullName))
            return null;

        Type result = Type.GetType(typeFullName);
        if (result != null)
            return result;
        if (_isFramework40)
            result = Type.GetType(System.Text.RegularExpressions.Regex.Replace(typeFullName, "(Version\\=[^,]*)", "Version=4.0.0.0"));
        if (result == null)
            result = Type.GetType(typeFullName.Split(',')[0]);
        return result;
    }
    #endregion

    #region GetWarpperType
    /// <summary>
    /// 快速获取包装类型。
    /// </summary>
    /// <param name="typeFullName">类型的全名称。</param>
    /// <param name="assemblyFile">程序集文件名</param>
    /// <returns>返回对应的类型。</returns>
    public static Type GetWarpperType(string typeFullName, string assemblyFile) {
        bool tried = false;
        lb_Retry:
        Type type = GetWarpperType(typeFullName);
        if (type == null && !tried) {
            tried = true;
            if (!string.IsNullOrEmpty(assemblyFile)) {
                //System.Reflection.Emit.AssemblyBuilder
//#if netcore
//                try { 
//                    System.Reflection.AssemblyName assemblyName = System.Runtime.Loader.AssemblyLoadContext.GetAssemblyName(assemblyFile);
//                    System.Reflection.Assembly.Load(assemblyName);
//                } catch { }
//#else
                try { System.Reflection.Assembly.LoadFrom(assemblyFile); } catch { }
//#endif
            }
            goto lb_Retry;
        }
        return type;
    }
    #endregion

    #region GetField
    /// <summary>
    /// 获取指定类型的字段（按默认的BindingFlags）。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name">字段名称。</param>
    /// <returns>返回字段。</returns>
    public static FieldInfo GetField(Type type, string name) {
        return GetField(
                         type, 
                         name, 
                         DefaultBindingFlags
#if !netcore
                            | BindingFlags.GetField | BindingFlags.SetField
#endif
                            | BindingFlags.Static | BindingFlags.Instance);
    }
    /// <summary>
    /// 获取指定类型的字段。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name">字段名称。</param>
    /// <param name="bindingFlags">BindingFlags</param>
    /// <returns>返回字段。</returns>
    public static FieldInfo GetField(Type type, string name, BindingFlags bindingFlags) {
        if (type == null || string.IsNullOrEmpty(name))
            return null;
        return type.GetField(name, bindingFlags);
    }
    #endregion

    #region GetProperty
    /// <summary>
    /// 获取指定类型的属性（按默认的BindingFlags）。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name">属性名称。</param>
    /// <returns>返回属性。</returns>
    public static PropertyInfo GetProperty(Type type, string name) {
        return GetProperty(
                            type, 
                            name, 
                            DefaultBindingFlags
#if !netcore
                                | BindingFlags.GetProperty | BindingFlags.SetProperty
#endif
                                | BindingFlags.Static | BindingFlags.Instance);
    }
    /// <summary>
    /// 获取指定类型的属性。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name">属性名称。</param>
    /// <param name="bindingFlags">BindingFlags</param>
    /// <returns>返回属性。</returns>
    public static PropertyInfo GetProperty(Type type, string name, BindingFlags bindingFlags) {
        if (type == null || string.IsNullOrEmpty(name))
            return null;
        return type.GetProperty(name, bindingFlags);
    }
    #endregion
    #region GetProperties
    /// <summary>
    /// 获取指定类型的所有属性列表（按默认的BindingFlags）。
    /// </summary>
    /// <param name="type"></param>
    /// <returns>返回属性列表。</returns>
    public static PropertyInfo[] GetProperties(Type type) {
        return GetProperties(
                                type, 
                                DefaultBindingFlags
#if !netcore
                                    | BindingFlags.GetProperty | BindingFlags.SetProperty
#endif
                                    | BindingFlags.Static | BindingFlags.Instance);
    }
    /// <summary>
    /// 获取指定类型的所有属性列表。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="bindingFlags">BindingFlags</param>
    /// <returns>返回属性列表。</returns>
    public static PropertyInfo[] GetProperties(Type type, BindingFlags bindingFlags) {
        if (type == null)
            return new PropertyInfo[0];
        return type.GetProperties(bindingFlags);
    }
    /// <summary>
    /// 获取指定类型的所有属性列表。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="bindingFlags">BindingFlags</param>
    /// <param name="get">是否可读，为null表示不过滤。</param>
    /// <param name="set">是否可写，为null表示不过滤。</param>
    /// <returns>返回属性列表。</returns>
    public static PropertyInfo[] GetProperties(Type type, BindingFlags bindingFlags, bool? get, bool? set) {
        if (type == null)
            return new PropertyInfo[0];
        var array= type.GetProperties(bindingFlags);
        if (get != null || set != null) {
            System.Collections.Generic.List<PropertyInfo> list = new System.Collections.Generic.List<PropertyInfo>();
            for (int i = 0; i < array.Length; i++) {
                if (get != null && array[i].CanRead != get)
                    continue;
                if (set != null && array[i].CanWrite != set)
                    continue;
                list.Add(array[i]);
            }
            return list.ToArray();
        }
        return array;
    }
    #endregion


    #endregion

}
