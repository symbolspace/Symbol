using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Symbol;

/// <summary>
/// 静态：类型实现映射
/// </summary>
public static class TypeImplementMap {
    private static readonly object _syncObject;
    private static readonly HashSet<string> _list_family;
    private static readonly HashSet<Type> _list_baseType;
    private static readonly ConcurrentDictionary<Type, List<Type>> _list_baseType_targetTypes;
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, List<Type>>> _list_baseType_targetTypes_family;
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>> _list_baseType_singleton_type;
    static TypeImplementMap() {
        _syncObject = new object();
        _list_family = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _list_baseType = new HashSet<Type>();

        _list_baseType_targetTypes = new ConcurrentDictionary<Type, List<Type>>();
        _list_baseType_targetTypes_family = new ConcurrentDictionary<Type, ConcurrentDictionary<string, List<Type>>>();
        _list_baseType_singleton_type = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>>();
        Scan(typeof(TypeImplementMap).Assembly);
        Scan();
    }

    /// <summary>
    /// 获取家族数量。
    /// </summary>
    public static int FamilyCount {
        get { return _list_family.Count; }
    }
    /// <summary>
    /// 获取基础类型数量。
    /// </summary>
    public static int BaseTypeCount {
        get { return _list_baseType.Count; }
    }
    #region 获取目标单例集合 GetTargetSingletons
    /// <summary>
    /// 获取目标单例集合（构造函数无参数）。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <returns>返回最终实现的实例集合。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static ReadOnlyCollection<TBaseType> GetTargetSingletons<TBaseType>() {
        return GetTargetSingletons<TBaseType>(null, null);
    }
    /// <summary>
    /// 获取目标单例集合。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <param name="args">构造函数参数清单。</param>
    /// <returns>返回最终实现的实例集合。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static ReadOnlyCollection<TBaseType> GetTargetSingletons<TBaseType>(object[] args) {
        return GetTargetSingletons<TBaseType>(null, args);
    }
    /// <summary>
    /// 获取目标单例集合（构造函数无参数）。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <param name="familyName">家族名称，为空表示不限家族。</param>
    /// <returns>返回最终实现的实例集合。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static ReadOnlyCollection<TBaseType> GetTargetSingletons<TBaseType>(string familyName) {
        return GetTargetSingletons<TBaseType>(familyName, null);
    }
    /// <summary>
    /// 获取目标单例集合。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <param name="familyName">家族名称，为空表示不限家族。</param>
    /// <param name="args">构造函数参数清单。</param>
    /// <returns>返回最终实现的实例集合。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static ReadOnlyCollection<TBaseType> GetTargetSingletons<TBaseType>(string familyName, object[] args) {
        return GetTargetSingletons<TBaseType>(typeof(TBaseType), familyName, args);
    }

    /// <summary>
    /// 获取目标单例集合（构造函数无参数）。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <returns>返回最终实现的实例集合。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static ReadOnlyCollection<object> GetTargetSingletons(
#if !NET20
        this 
#endif
        Type baseType) {
        return GetTargetSingletons(baseType, null, null);
    }
    /// <summary>
    /// 获取目标单例集合。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="args">构造函数参数清单。</param>
    /// <returns>返回最终实现的实例集合。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static ReadOnlyCollection<object> GetTargetSingletons(
#if !NET20
        this 
#endif
        Type baseType, object[] args) {
        return GetTargetSingletons(baseType, null, args);
    }
    /// <summary>
    /// 获取目标单例集合（构造函数无参数）。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="familyName">家族名称，为空表示不限家族。</param>
    /// <returns>返回最终实现的实例集合。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static ReadOnlyCollection<object> GetTargetSingletons(
#if !NET20
        this 
#endif
        Type baseType, string familyName) {
        return GetTargetSingletons(baseType, familyName, null);
    }
    /// <summary>
    /// 获取目标单例集合。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="familyName">家族名称，为空表示不限家族。</param>
    /// <param name="args">构造函数参数清单。</param>
    /// <returns>返回最终实现的实例集合。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static ReadOnlyCollection<object> GetTargetSingletons(
#if !NET20
        this 
#endif
        Type baseType, string familyName, object[] args) {
        return GetTargetSingletons<object>(baseType, familyName, args);
    }
    static ReadOnlyCollection<TResult> GetTargetSingletons<TResult>(
#if !NET20
        this 
#endif
        Type baseType, string familyName, object[] args) {
        var list_instance = new List<TResult>();
        var targets = GetTargetTypesInternal(baseType, familyName);
        if (targets == null || targets.Count == 0) {
            if (string.IsNullOrEmpty(familyName)) {
                Throw.NotSupported(string.Format("未找到“{0}”的实现映射，家族为“{1}”", baseType.FullName, familyName));
            } else {
                Throw.NotSupported(string.Format("未找到“{0}”的实现映射", baseType.FullName));
            }
        }

        var list = _list_baseType_singleton_type.GetOrAdd(baseType, (key) => new ConcurrentDictionary<Type, object>());
        foreach (var type in targets) {
            list_instance.Add((TResult)list.GetOrAdd(type, (key) => FastObject.CreateInstance(type, args)));
        }
        return new ReadOnlyCollection<TResult>(list_instance);
    }
    #endregion

    #region 获取目标单例 GetTargetSingleton
    /// <summary>
    /// 获取目标单例（构造函数无参数）。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static TBaseType GetTargetSingleton<TBaseType>() {
        return GetTargetSingleton<TBaseType>(null, null);
    }
    /// <summary>
    /// 获取目标单例。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <param name="args">构造函数参数清单。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static TBaseType GetTargetSingleton<TBaseType>(object[] args) {
        return GetTargetSingleton<TBaseType>(null, args);
    }
    /// <summary>
    /// 获取目标单例（构造函数无参数）。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <param name="familyName">家族名称，为空表示不限家族。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static TBaseType GetTargetSingleton<TBaseType>(string familyName) {
        return GetTargetSingleton<TBaseType>(familyName, null);
    }
    /// <summary>
    /// 获取目标单例。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <param name="familyName">家族名称，为空表示不限家族。</param>
    /// <param name="args">构造函数参数清单。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static TBaseType GetTargetSingleton<TBaseType>(string familyName, object[] args) {
        return (TBaseType)GetTargetSingleton(typeof(TBaseType), familyName, args);
    }

    /// <summary>
    /// 获取目标单例（构造函数无参数）。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static object GetTargetSingleton(
#if !NET20
        this 
#endif
        Type baseType) {
        return GetTargetSingleton(baseType, null, null);
    }
    /// <summary>
    /// 获取目标单例。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="args">构造函数参数清单。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static object GetTargetSingleton(
#if !NET20
        this 
#endif
        Type baseType, object[] args) {
        return GetTargetSingleton(baseType, null, args);
    }
    /// <summary>
    /// 获取目标单例（构造函数无参数）。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="familyName">家族名称，为空表示不限家族。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static object GetTargetSingleton(
#if !NET20
        this 
#endif
        Type baseType, string familyName) {
        return GetTargetSingleton(baseType, familyName, null);
    }
    /// <summary>
    /// 获取目标单例。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="familyName">家族名称，为空表示不限家族。</param>
    /// <param name="args">构造函数参数清单。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static object GetTargetSingleton(
#if !NET20
        this 
#endif
        Type baseType, string familyName, object[] args) {
        var list = _list_baseType_singleton_type.GetOrAdd(baseType, (key) => new ConcurrentDictionary<Type, object>());
        if (string.IsNullOrEmpty(familyName) && list.Count > 0) {
            return LinqHelper.FirstOrDefault(list.Values);
        }
        var targets = GetTargetTypesInternal(baseType, familyName);
        if (targets == null || targets.Count == 0) {
            if (string.IsNullOrEmpty(familyName)) {
                Throw.NotSupported(string.Format("未找到“{0}”的实现映射，家族为“{1}”", baseType.FullName, familyName));
            } else {
                Throw.NotSupported(string.Format("未找到“{0}”的实现映射", baseType.FullName));
            }
        }
        var type = targets[0];
        return list.GetOrAdd(type, (key) => FastObject.CreateInstance(type, args));
    }
    #endregion
    #region 创建目标实例 CreateTargetInstance
    /// <summary>
    /// 创建目标实例（构造函数无参数）。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static TBaseType CreateTargetInstance<TBaseType>() {
        return CreateTargetInstance<TBaseType>(null, null);
    }
    /// <summary>
    /// 创建目标实例。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <param name="args">构造函数参数清单。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static TBaseType CreateTargetInstance<TBaseType>(object[] args) {
        return CreateTargetInstance<TBaseType>(null, args);
    }
    /// <summary>
    /// 创建目标实例（构造函数无参数）。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <param name="familyName">家族名称，为空表示不限家族。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static TBaseType CreateTargetInstance<TBaseType>(string familyName) {
        return CreateTargetInstance<TBaseType>(familyName, null);
    }
    /// <summary>
    /// 创建目标实例。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <param name="familyName">家族名称，为空表示不限家族。</param>
    /// <param name="args">构造函数参数清单。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static TBaseType CreateTargetInstance<TBaseType>(string familyName, object[] args) {
        return (TBaseType)CreateTargetInstance(typeof(TBaseType), familyName, args);
    }

    /// <summary>
    /// 创建目标实例（构造函数无参数）。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static object CreateTargetInstance(
#if !NET20
        this 
#endif
        Type baseType) {
        return CreateTargetInstance(baseType, null, null);
    }
    /// <summary>
    /// 创建目标实例。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="args">构造函数参数清单。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static object CreateTargetInstance(
#if !NET20
        this 
#endif
        Type baseType, object[] args) {
        return CreateTargetInstance(baseType, null, args);
    }
    /// <summary>
    /// 创建目标实例（构造函数无参数）。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="familyName">家族名称，为空表示不限家族。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static object CreateTargetInstance(
#if !NET20
        this 
#endif
        Type baseType, string familyName) {
        return CreateTargetInstance(baseType, familyName, null);
    }
    /// <summary>
    /// 创建目标实例。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="familyName">家族名称，为空表示不限家族。</param>
    /// <param name="args">构造函数参数清单。</param>
    /// <returns>返回最终实现的实例。</returns>
    /// <remarks>家族名称不区分大小写。未指定家族时采用第一个，同一个家族有多个实现时采用第一个。</remarks>
    public static object CreateTargetInstance(
#if !NET20
        this 
#endif
        Type baseType, string familyName, object[] args) {
        var targets = GetTargetTypesInternal(baseType, familyName);
        if (targets == null || targets.Count == 0) {
            if (string.IsNullOrEmpty(familyName)) {
                Throw.NotSupported(string.Format("未找到“{0}”的实现映射，家族为“{1}”", baseType.FullName, familyName));
            } else {
                Throw.NotSupported(string.Format("未找到“{0}”的实现映射", baseType.FullName));
            }
        }

        return FastObject.CreateInstance(targets[0], args);
    }
    #endregion

    #region 获取目标类型集合 GetTargetTypes
    /// <summary>
    /// 获取目标类型集合。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <returns>返回匹配的类型集合。</returns>
    public static ReadOnlyCollection<Type> GetTargetTypes<TBaseType>() {
        return GetTargetTypes(typeof(TBaseType), null);
    }
    /// <summary>
    /// 获取目标类型集合。
    /// </summary>
    /// <typeparam name="TBaseType">基础类型。</typeparam>
    /// <param name="familyName">家族名称，为空表示不限家族。家族名称不区分大小写。</param>
    /// <returns>返回匹配的类型集合。</returns>
    public static ReadOnlyCollection<Type> GetTargetTypes<TBaseType>(string familyName) {
        return GetTargetTypes(typeof(TBaseType), familyName);
    }
    /// <summary>
    /// 获取目标类型集合。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <returns>返回匹配的类型集合。</returns>
    public static ReadOnlyCollection<Type> GetTargetTypes(
#if !NET20
        this 
#endif
        Type baseType) {
        return GetTargetTypes(baseType, null);
    }
    /// <summary>
    /// 获取目标类型集合。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="familyName">家族名称，为空表示不限家族。家族名称不区分大小写。</param>
    /// <returns>返回匹配的类型集合。</returns>
    public static ReadOnlyCollection<Type> GetTargetTypes(
#if !NET20
        this 
#endif
        Type baseType, string familyName) {
        return new ReadOnlyCollection<Type>(GetTargetTypesInternal(baseType, familyName) ?? new List<Type>());
    }
    /// <summary>
    /// 获取目标类型集合。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="familyName">家族名称，为空表示不限家族。家族名称不区分大小写。</param>
    /// <returns>返回匹配的类型集合。</returns>
    static List<Type> GetTargetTypesInternal(
#if !NET20
        this 
#endif
        Type baseType, string familyName) {
        Throw.CheckArgumentNull(baseType, nameof(baseType));
        if (!_list_baseType.Contains(baseType))
            return null;
        if (string.IsNullOrEmpty(familyName)) {
            return IDictionaryExtensions.GetValue(_list_baseType_targetTypes, baseType);
        }
        var list = IDictionaryExtensions.GetValue(_list_baseType_targetTypes_family, baseType);
        return IDictionaryExtensions.GetValue(list, familyName);
    }
    #endregion

    #region 扫描 Scan
    /// <summary>
    /// 扫描。
    /// </summary>
    /// <param name="paths">路径规则，e.g “*.Logger.*.dll” 。</param>
    public static void Scan(string paths) {
        if (string.IsNullOrEmpty(paths))
            return;
        foreach (var file in IO.FileHelper.Scan(paths)) {
            Scan(AssemblyLoader.Load(file));
        }
    }
    /// <summary>
    /// 扫描。
    /// </summary>
    /// <remarks>从当前已加载的程序集中扫描。</remarks>
    public static void Scan() {
        Scan(AssemblyLoader.GetAssemblies());
    }
    /// <summary>
    /// 扫描。
    /// </summary>
    /// <param name="assemblies">待扫描的程序集清单，为空不会报错。</param>
    public static void Scan(IEnumerable<Assembly> assemblies) {
        if (assemblies == null)
            return;
        foreach (Assembly assembly in assemblies) {
            Scan(assembly);
        }
    }
    /// <summary>
    /// 扫描。
    /// </summary>
    /// <param name="assembly">待扫描的程序集，为空不会报错。</param>
    public static void Scan(Assembly assembly) {
        if (assembly == null)
            return;
        var attributes = AttributeExtensions.GetCustomAttributes<TypeImplementMapAttribute>(assembly, true);
        foreach (var attribute in attributes) {
            MapType(attribute);
        }
    }
    #endregion
    #region 映射类型 MapType
    /// <summary>
    /// 映射类型。
    /// </summary>
    /// <param name="typeImplementMapAttribute">特性实例。</param>
    /// <returns>返回是否注册成功。</returns>
    public static bool MapType(TypeImplementMapAttribute typeImplementMapAttribute) {
        Throw.CheckArgumentNull(typeImplementMapAttribute, nameof(typeImplementMapAttribute));
        return MapType(typeImplementMapAttribute.BaseType, typeImplementMapAttribute.TargetType, typeImplementMapAttribute.FamilyName);
    }
    /// <summary>
    /// 映射类型。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="targetType">目标类型，必须继承自基础类型。</param>
    /// <returns>返回是否注册成功。</returns>
    public static bool MapType(Type baseType, Type targetType) {
        return MapType(baseType, targetType, null);
    }
    /// <summary>
    /// 映射类型。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="targetType">目标类型，必须继承自基础类型。</param>
    /// <param name="familyName">家族名称，同一个家族表示为同系列，默认为程序集名称。</param>
    /// <returns>返回是否注册成功。</returns>
    public static bool MapType(Type baseType, Type targetType, string familyName) {
        Throw.CheckArgumentNull(baseType, nameof(baseType));
        Throw.CheckArgumentNull(targetType, nameof(targetType));

        if (!TypeExtensions.IsInheritFrom(targetType, baseType))
            return false;
        if (string.IsNullOrEmpty(familyName)) {
            familyName = targetType.Assembly.FullName;
        }
        lock (_syncObject) {
            _list_baseType.Add(baseType);
            _list_family.Add(familyName);
            {
                var list_type = _list_baseType_targetTypes.GetOrAdd(baseType, (key) => new List<Type>());
                if (!list_type.Contains(targetType)) {
                    list_type.Add(targetType);
                }
            }
            {
                var list_type_family = _list_baseType_targetTypes_family.GetOrAdd(baseType, (key) => new ConcurrentDictionary<string, List<Type>>(StringComparer.OrdinalIgnoreCase));
                var list_type = list_type_family.GetOrAdd(familyName, (key) => new List<Type>());
                if (!list_type.Contains(targetType)) {
                    list_type.Add(targetType);
                }
            }
        }
        return true;
    }
    #endregion
}